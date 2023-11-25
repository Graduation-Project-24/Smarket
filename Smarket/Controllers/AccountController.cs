using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smarket.Models;
using Smarket.Models.Dtos;
using Smarket.Services;
using Smarket.Services.IServices;
using Stripe;
using System.Security.Claims;

namespace Smarket.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;

        public AccountController(ITokenService tokenService, UserManager<User> userManager,
            SignInManager<User> signInManager,IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _emailService = emailService;
        }

        [HttpGet("ExternalLogin")]

        public IActionResult ExternalLogin(string provider, string returnUrl = null)

        {
            if (provider == "Google")
            {
                var redirectUrl = Url.Action(nameof(ExternalLoginCallbackGoogle), "Account", new { returnUrl });
                var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
                return Challenge(properties, provider);
            }
            if (provider == "Microsoft")
            {
                var redirectUrl = Url.Action(nameof(ExternalLoginCallbackMicrosoft), "Account", new { returnUrl });
                var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
                return Challenge(properties, provider);
            }
            return BadRequest("Invalid Provider");



        }

        [HttpGet("signin-google")]

        public async Task<IActionResult> ExternalLoginCallbackGoogle(string returnUrl = null, string remoteError = null)
        {
            returnUrl = "/api/Values";  // Capture returnUrl

            if (remoteError != null)
            {
                return LocalRedirect(returnUrl); // Redirect to returnUrl
            }

            var emailClaim = HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var loginProviderClaim = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (emailClaim == null || loginProviderClaim == null)
            {
                return LocalRedirect(returnUrl); // Redirect to returnUrl
            }

            // If the user is already signed in, link the external account.
            var result = await _signInManager.ExternalLoginSignInAsync(loginProviderClaim, emailClaim, isPersistent: false);
            if (result.Succeeded)
            {
                return LocalRedirect(returnUrl); // Redirect to returnUrl
            }

            // If the user doesn't have an account, create a new one.
            var user = new User { UserName = emailClaim, Email = emailClaim };
            var createResult = await _userManager.CreateAsync(user);

            if (createResult.Succeeded)
            {
                var externalLoginResult = await _userManager.AddLoginAsync(user, new UserLoginInfo(loginProviderClaim, emailClaim, "Google"));
                if (externalLoginResult.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl); // Redirect to returnUrl
                }
            }


            // Handle errors during external login process.
            // For simplicity, just redirect to returnUrl.
            return LocalRedirect(returnUrl); // Redirect to returnUrl
        }

        [HttpGet("signin-microsoft")]

        public async Task<IActionResult> ExternalLoginCallbackMicrosoft(string returnUrl = null, string remoteError = null)
        {
            returnUrl = "/api/Values";  // Capture returnUrl

            if (remoteError != null)
            {
                return LocalRedirect(returnUrl); // Redirect to returnUrl
            }

            var emailClaim = HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var loginProviderClaim = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (emailClaim == null || loginProviderClaim == null)
            {
                return LocalRedirect(returnUrl); // Redirect to returnUrl
            }

            // If the user is already signed in, link the external account.
            var result = await _signInManager.ExternalLoginSignInAsync(loginProviderClaim, emailClaim, isPersistent: false);
            if (result.Succeeded)
            {
                return LocalRedirect(returnUrl); // Redirect to returnUrl
            }

            // If the user doesn't have an account, create a new one.
            var user = new User { UserName = emailClaim, Email = emailClaim };
            var createResult = await _userManager.CreateAsync(user);

            if (createResult.Succeeded)
            {
                var externalLoginResult = await _userManager.AddLoginAsync(user, new UserLoginInfo(loginProviderClaim, emailClaim, "Facebook"));
                if (externalLoginResult.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl); // Redirect to returnUrl
                }
            }

            // Handle errors during external login process.
            // For simplicity, just redirect to returnUrl.
            return LocalRedirect(returnUrl); // Redirect to returnUrl
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await _userManager.Users.AnyAsync(x => x.UserName == registerDto.UserName))
            {
                return BadRequest("Username is taken");
            }

            if (await _userManager.Users.AnyAsync(x => x.Email == registerDto.Email))
            {
                return BadRequest("Email is taken");
            }

            var user = new User
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                ImageId =registerDto.ImageId,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account", new { token, email = user.Email }, Request.Scheme);

                await _emailService.EmailSender(user.Email, "Confirm Your Email", confirmationLink);

                return new UserDto
                {
                    UserName = user.UserName,
                    Token = await _tokenService.CreateToken(user),
                };
            }

            return BadRequest("Problem registering user");
        }






        [HttpPost("SendConfirmationEmail")]
        public async Task<ActionResult> SendConfirmationEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return BadRequest("User not found");
            }

            if (user.EmailConfirmed)
            {
                return BadRequest("Email is already confirmed.");
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account", new { token, email }, Request.Scheme);

            await _emailService.EmailSender(email, "Email Confirmation", confirmationLink);

            return Ok("Email sent");
        }

        [HttpPost("resend-confirmation-email")]
        public async Task<IActionResult> ResendConfirmationEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest("Invalid email.");
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (user.EmailConfirmed)
            {
                return BadRequest("Email is already confirmed.");
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account", new { token, email }, Request.Scheme);

            await _emailService.EmailSender(email, "Email Confirmation", confirmationLink);

            return Ok("Email confirmation link resent.");
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(email))
            {
                return BadRequest("Invalid token or email.");
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                return Ok("EmailConfirmed");
            }
            else
            {
                return BadRequest("Email confirmation failed.");
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.Users
                .SingleOrDefaultAsync(x => x.Email == loginDto.Email);

            if (user == null) return Unauthorized("Invalid username or password");

            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!result) return Unauthorized("Invalid username or password");

            return new UserDto
            {
                UserName = user.UserName,
                Token = await _tokenService.CreateToken(user),
            };
        }




        [HttpPost]
        [Route("forgot")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);

                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return Ok("If the provided email exists in our system, a password reset link has been sent.");
                }

                // Generate password reset token
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                // Create the password reset link
                var resetLink = Url.Action("ResetPassword", "ResetPassword", new { token, email = forgotPasswordDto.Email }, Request.Scheme);

                // Compose the email message
                var emailSubject = "Reset Your Password";
                var emailBody = $"Please click the following link to reset your password: {resetLink}";

                // Send password reset email
                await _emailService.EmailSender(forgotPasswordDto.Email, emailSubject, emailBody);

                return Ok("If the provided email exists in our system, a password reset link has been sent.");
            }            

            // If model state is not valid
            return BadRequest(ModelState);
        }

       

        [HttpPost]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }
    }
}
