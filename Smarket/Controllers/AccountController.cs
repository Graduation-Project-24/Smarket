using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smarket.Models;
using Smarket.Models.Dtos;
using Smarket.Services.IServices;
using System.Security.Claims;

namespace Smarket.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IImageService _imageService;

        public AccountController(ITokenService tokenService, UserManager<User> userManager,
            SignInManager<User> signInManager, IEmailService emailService, IImageService imageService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _emailService = emailService;
            _imageService = imageService;
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
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                DateOfBirth = registerDto.DateOfBirth,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                City = registerDto.City,
                State = registerDto.State,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            await _userManager.AddToRoleAsync(user, "User");

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


        [HttpPut]
        [Route("EditUser")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> EditUser([FromForm] EditUserDto dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var image = await _imageService.AddPhotoAsync(dto.formFile);

            var cloudImage = new Image
            {
                PublicId = image.PublicId,
                Url = image.Url.ToString(),
            };

            user.UserName = dto.UserName;
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.DateOfBirth = dto.DateOfBirth;
            user.Email = dto.Email;
            user.Image = cloudImage;
            user.PhoneNumber = dto.PhoneNumber;
            user.City = dto.City;
            user.State = dto.State;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok(new
                {
                    Name = dto.UserName,
                    Email = dto.Email,
                    Image = cloudImage.Url.ToString(),
                });
            }
            return BadRequest("Failed to update user");
        }




        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromForm] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = Url.Action(nameof(ResetPassword), "Account", new { token, email = email }, Request.Scheme);

            // Send the reset password link to the user's email
            await _emailService.EmailSender(email, "Reset Your Password", resetLink);

            return Ok("Reset password link sent to your email.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(string Email, string Token)
        {
            var user = await _userManager.FindByEmailAsync(Email);

            if (user == null)
            {
                return BadRequest("User not found.");
            }
            var NewPassword = "123456a";
            var result = await _userManager.ResetPasswordAsync(user, Token, NewPassword);

            if (result.Succeeded)
            {
                return Ok("Password reset successful.");
            }
            else
            {
                return BadRequest("Password reset failed.");
            }
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user is null)
                    return BadRequest("Email Not Found");

                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    return Ok();
                }
                return BadRequest(result.Errors);
            }
            return BadRequest(model);
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteAccount(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return NotFound("User not found");
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return NoContent();
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }


        [HttpPost]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
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
    }
}
