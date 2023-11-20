using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;

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

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return Unauthorized("Invalid credentials.");
            }
            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!result)
            {
                return Unauthorized("Invalid Username or Invalid Password.");

            }
           var token = await _tokenService.CreateToken(user);
           
            return Ok(token);
           

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = registerDto.UsertName,
                    Email = registerDto.Email,
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    DateOfBirth = registerDto.DateOfBirth,
                    City = registerDto.City,
                    State = registerDto.State,
                    ImageId = registerDto.ImageId
       


    };

                var result = await _userManager.CreateAsync(user, registerDto.Password);

                if (result.Succeeded)
                {
                    // Handle successful registration
                    return Ok("Registration successful!");
                }
                else
                {
                    // Handle errors if registration fails
                    return BadRequest(result.Errors);
                }
            }

            // If model state is not valid
            return BadRequest(ModelState);
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
        [Route("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto confirmEmailDto)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(confirmEmailDto.Email);

                if (user == null)
                {
                    // User not found
                    return BadRequest("User not found.");
                }

                // Confirm user's email
                var result = await _userManager.ConfirmEmailAsync(user, confirmEmailDto.Token);

                if (result.Succeeded)
                {
                    // Email confirmed successfully
                    return Ok("Email confirmed successfully!");
                }
                else
                {
                    // Error confirming email
                    return BadRequest("Error confirming email.");
                }
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
