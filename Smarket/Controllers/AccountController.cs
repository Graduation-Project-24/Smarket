using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Smarket.Models;
using Smarket.Services.IServices;
using System.Security.Claims;

namespace Smarket.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
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
    }
}
