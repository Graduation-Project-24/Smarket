using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smarket.Models;
using Smarket.Models.Dtos;
using Smarket.Models.DTOs;
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
        private readonly ILogger<AccountController> _logger;
        public AccountController(ITokenService tokenService, UserManager<User> userManager,
            SignInManager<User> signInManager, IEmailService emailService, IImageService imageService , ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _emailService = emailService;
            _imageService = imageService;
            _logger = logger;
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            try
            {
                _logger.LogInformation("Registration attempt for email: {Email}", registerDto.Email);

                if (await _userManager.Users.AnyAsync(x => x.Email == registerDto.Email))
                {
                    _logger.LogWarning("Email {Email} is already taken.", registerDto.Email);
                    return BadRequest("Email is already taken");
                }

                var user = new User
                {
                    UserName = registerDto.Email.Substring(0, registerDto.Email.IndexOf("@")),
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    DateOfBirth = registerDto.DateOfBirth,
                    Email = registerDto.Email,
                    PhoneNumber = registerDto.PhoneNumber,
                    EmailConfirmed = false
                };

                var result = await _userManager.CreateAsync(user, registerDto.Password);

                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to register user: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                    return BadRequest("Failed to register user");
                }

                await _userManager.AddToRoleAsync(user, "User");

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account", new { token, email = user.Email }, Request.Scheme);

                await _emailService.EmailSender(user.Email, "Confirm Your Email", confirmationLink);

                _logger.LogInformation("User registered successfully: {Email}", user.Email);

                return new UserDto
                {
                    UserName = user.UserName,
                    Token = await _tokenService.CreateToken(user),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during user registration.");
                return StatusCode(500, "An error occurred during user registration.");
            }
        }


        [HttpPost("Registerers")]
        public async Task<ActionResult<IEnumerable<UserDto>>> Registerers(IEnumerable<RegisterDto> registerDtos)
        {
            try
            {
                _logger.LogInformation("Registration attempt for {Count} users", registerDtos.Count());

                var userDtos = new List<UserDto>();

                foreach (var registerDto in registerDtos)
                {
                    if (await _userManager.Users.AnyAsync(x => x.Email == registerDto.Email))
                    {
                        _logger.LogWarning("Email {Email} is already taken.", registerDto.Email);
                        continue; // Skip this user and continue with the next one
                    }

                    var user = new User
                    {
                        UserName = registerDto.Email.Substring(0, registerDto.Email.IndexOf("@")),
                        FirstName = registerDto.FirstName,
                        LastName = registerDto.LastName,
                        DateOfBirth = registerDto.DateOfBirth,
                        Email = registerDto.Email,
                        PhoneNumber = registerDto.PhoneNumber,
                        EmailConfirmed = false
                    };

                    var result = await _userManager.CreateAsync(user, registerDto.Password);

                    if (!result.Succeeded)
                    {
                        _logger.LogError("Failed to register user: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                        continue; // Skip this user and continue with the next one
                    }

                    await _userManager.AddToRoleAsync(user, "User");

/*                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);*/

/*                    var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account", new { token, email = user.Email }, Request.Scheme);

                    await _emailService.EmailSender(user.Email, "Confirm Your Email", confirmationLink);*/

                    _logger.LogInformation("User registered successfully: {Email}", user.Email);

                    userDtos.Add(new UserDto
                    {
                        UserName = user.UserName,
                        Token = await _tokenService.CreateToken(user),
                    });
                }

                return userDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during user registration.");
                return StatusCode(500, "An error occurred during user registration.");
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

            [HttpGet("ConfirmEmail")]
            public async Task<ActionResult> ConfirmEmail(string token, string email)
            {
                if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(email))
                {
                    return BadRequest("Token or email is missing.");
                }

                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return BadRequest("User not found.");
                }

                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return Ok("Email confirmed successfully.");
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




        [HttpPost("forgotpassword")]
        public async Task<IActionResult> ForgotPassword(string email)
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

        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
            {
                return BadRequest("User not found.");
            }
            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);

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
