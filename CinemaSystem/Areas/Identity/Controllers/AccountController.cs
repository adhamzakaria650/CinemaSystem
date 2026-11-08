using CinemaSystem.Models;
using CinemaSystem.Repositories;
using CinemaSystem.ViewModels.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CinemaSystem.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IRepo<ApplicationUserOTP> _otpRepo;

        public AccountController(UserManager<ApplicationUser> userManager, IEmailSender emailSender, SignInManager<ApplicationUser> signInManager, IRepo<ApplicationUserOTP> otpRepo)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _signInManager = signInManager;
            _otpRepo = otpRepo;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please Fill All Fields Correctly";
                return View(registerVM);
            }
            ApplicationUser user = new ApplicationUser()
            {
                Name = registerVM.Name,
                UserName = registerVM.UserName,
                Address = registerVM.Address,
                Email = registerVM.Email
            };
            var result = await _userManager.CreateAsync(user, registerVM.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                TempData["Error"] = "Please Fill All Fields Correctly";
                return View(registerVM);
            }
            var ConfirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action("ConfirmEmail", "Account", new { area = "Identity", Id = user.Id, Token = ConfirmEmailToken }, Request.Scheme);
            var message = $@"
            <div style='margin:0;padding:32px 12px;background-color:#f2f3f8;font-family:Segoe UI,Helvetica,Arial,sans-serif;'>
              <table role='presentation' width='100%' cellpadding='0' cellspacing='0' border='0' style='max-width:560px;margin:0 auto;background-color:#ffffff;border-radius:12px;overflow:hidden;'>

                <tr>
                  <td align='center' style='background-color:#1b1a2e;padding:26px 24px;font-size:26px;font-weight:700;color:#e8b64c;'>
                    Cinema
                  </td>
                </tr>

                <tr>
                  <td style='padding:34px 32px 8px 32px;color:#1d2030;'>
                    <h1 style='margin:0 0 14px 0;font-size:22px;line-height:1.3;color:#1d2030;'>Verify your email address</h1>
                    <p style='margin:0 0 24px 0;font-size:15px;line-height:1.6;color:#4a4e63;'>
                      Thanks for creating your Cinema account. Please confirm your email address to activate it.
                    </p>
                  </td>
                </tr>

                <tr>
                  <td align='center' style='padding:0 32px 26px 32px;'>
                    <a href='{link}' target='_blank'
                       style='display:inline-block;padding:14px 36px;background-color:#e8b64c;color:#2a2005;font-size:16px;font-weight:700;text-decoration:none;border-radius:8px;'>
                      Verify my email
                    </a>
                  </td>
                </tr>

                <tr>
                  <td style='padding:0 32px 26px 32px;font-size:13px;line-height:1.5;color:#6b6f85;'>
                    If the button doesn't work, copy and paste this link into your browser:<br>
                    <a href='{link}' target='_blank' style='color:#b8862a;word-break:break-all;'>{link}</a>
                  </td>
                </tr>

                <tr>
                  <td style='padding:18px 32px 26px 32px;border-top:1px solid #e3e5ee;font-size:13px;line-height:1.6;color:#6b6f85;'>
                    If you didn't create a Cinema account, you can safely ignore this email.
                  </td>
                </tr>
              </table>

              <p style='margin:18px 0 0 0;text-align:center;font-size:12px;color:#9a9cb5;'>
                &copy; {DateTime.Now.Year} Cinema. All rights reserved.
              </p>
            </div>";
            await _emailSender.SendEmailAsync(
                email: registerVM.Email,
                "CinemaSystem Email Confirmation",
                message
                );
            TempData["Success"] = "Account Created Successfully, Please Check Your Email To Verify Your Account";
            return RedirectToAction(nameof(Login));
        }
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string Id, string Token)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
            {
                TempData["Warning"] = "User Not Found";
                return NotFound();
            }
            await _userManager.ConfirmEmailAsync(user, Token);
            TempData["Success"] = "Email Confirmed Successfully, You Can Login Now";
            return RedirectToAction(nameof(Login));
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            var user = await _userManager.FindByNameAsync(loginVM.UserNameOrEmail) ??
                await _userManager.FindByEmailAsync(loginVM.UserNameOrEmail);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid UserName Or Email Or Password");
                TempData["Error"] = "Invalid UserName Or Email Or Password";
                return View(loginVM);
            }
            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.RememberMe, true);
            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {

                    ModelState.AddModelError("", "To Many Attempts Please Try Again Later");
                    TempData["Error"] = "To Many Attempts Please Try Again Later";
                    return View(loginVM);
                }
                else
                {
                    ModelState.AddModelError("", "Invalid UserName Or Email Or Password");
                    TempData["Error"] = "Invalid UserName Or Email Or Password";
                    return View(loginVM);
                }
                

            }
            TempData["Success"] = "Logged In Successfully";
            return RedirectToAction("Index", "Home", new { Area = "Customer" });
        }
        [HttpGet]
        public IActionResult ResendEmailConfirmation()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfiramtionVM resendEmailConfiramtionVM)
        {
            var user = await _userManager.FindByEmailAsync(resendEmailConfiramtionVM.UserNameOrEmail) ??
                await _userManager.FindByNameAsync(resendEmailConfiramtionVM.UserNameOrEmail);
            if (user == null)
            {
                ModelState.AddModelError("", "User Not Found");
                TempData["Error"] = "User Not Found";
                return View(resendEmailConfiramtionVM);
            }
            var ConfirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action(nameof(ConfirmEmail), "Account", new { area = nameof(Identity), Id = user.Id, Token = ConfirmEmailToken }, Request.Scheme);
            var message = $@"
            <div style='margin:0;padding:32px 12px;background-color:#f2f3f8;font-family:Segoe UI,Helvetica,Arial,sans-serif;'>
              <table role='presentation' width='100%' cellpadding='0' cellspacing='0' border='0' style='max-width:560px;margin:0 auto;background-color:#ffffff;border-radius:12px;overflow:hidden;'>

                <tr>
                  <td align='center' style='background-color:#1b1a2e;padding:26px 24px;font-size:26px;font-weight:700;color:#e8b64c;'>
                    Cinema
                  </td>
                </tr>

                <tr>
                  <td style='padding:34px 32px 8px 32px;color:#1d2030;'>
                    <h1 style='margin:0 0 14px 0;font-size:22px;line-height:1.3;color:#1d2030;'>Verify your email address</h1>
                    <p style='margin:0 0 24px 0;font-size:15px;line-height:1.6;color:#4a4e63;'>
                      Thanks for creating your Cinema account. Please confirm your email address to activate it.
                    </p>
                  </td>
                </tr>

                <tr>
                  <td align='center' style='padding:0 32px 26px 32px;'>
                    <a href='{link}' target='_blank'
                       style='display:inline-block;padding:14px 36px;background-color:#e8b64c;color:#2a2005;font-size:16px;font-weight:700;text-decoration:none;border-radius:8px;'>
                      Verify my email
                    </a>
                  </td>
                </tr>

                <tr>
                  <td style='padding:0 32px 26px 32px;font-size:13px;line-height:1.5;color:#6b6f85;'>
                    If the button doesn't work, copy and paste this link into your browser:<br>
                    <a href='{link}' target='_blank' style='color:#b8862a;word-break:break-all;'>{link}</a>
                  </td>
                </tr>

                <tr>
                  <td style='padding:18px 32px 26px 32px;border-top:1px solid #e3e5ee;font-size:13px;line-height:1.6;color:#6b6f85;'>
                    If you didn't create a Cinema account, you can safely ignore this email.
                  </td>
                </tr>
              </table>

              <p style='margin:18px 0 0 0;text-align:center;font-size:12px;color:#9a9cb5;'>
                &copy; {DateTime.Now.Year} Cinema. All rights reserved.
              </p>
            </div>";
            await _emailSender.SendEmailAsync(
                user.Email,
                "CinemaSystem Email Confirmation",
                message
                );
            TempData["Success"] = "Email Sent Successfully, Please Check Your Email To Verify Your Account";
            return RedirectToAction(nameof(Login));
        }
        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordVM forgetPasswordVM)
        {
            var user = await _userManager.FindByEmailAsync(forgetPasswordVM.UserNameOrEmail) ??
                await _userManager.FindByNameAsync(forgetPasswordVM.UserNameOrEmail);
            if (user == null)
            {
                ModelState.AddModelError("", "User NotFound");
                TempData["Error"] = "User NotFound";
                return View(forgetPasswordVM);
            }
            var Otp = new Random().Next(1000, 10000).ToString();
            ApplicationUserOTP applicationUserOTP = new ApplicationUserOTP(user.Id, Otp);
            await _otpRepo.AddAsync(applicationUserOTP);
            await _otpRepo.CommitAsync();
            var message = $@"
            <div style='margin:0;padding:32px 12px;background-color:#f2f3f8;font-family:Segoe UI,Helvetica,Arial,sans-serif;'>
              <table role='presentation' width='100%' cellpadding='0' cellspacing='0' border='0' style='max-width:560px;margin:0 auto;background-color:#ffffff;border-radius:12px;overflow:hidden;'>

                <tr>
                  <td align='center' style='background-color:#1b1a2e;padding:26px 24px;font-size:26px;font-weight:700;color:#e8b64c;'>
                    Cinema
                  </td>
                </tr>

                <tr>
                  <td style='padding:34px 32px 8px 32px;color:#1d2030;'>
                    <h1 style='margin:0 0 14px 0;font-size:22px;line-height:1.3;color:#1d2030;'>Reset your password</h1>
                    <p style='margin:0 0 24px 0;font-size:15px;line-height:1.6;color:#4a4e63;'>
                      We received a request to reset the password for your Cinema account. Use the code below to continue.
                    </p>
                  </td>
                </tr>

                <tr>
                  <td align='center' style='padding:0 32px 26px 32px;'>
                    <div style='display:inline-block;padding:16px 36px;background-color:#fbf5e6;border:2px solid #e8b64c;border-radius:10px;font-size:34px;font-weight:700;letter-spacing:10px;color:#1d2030;'>
                      {Otp}
                    </div>
                  </td>
                </tr>

                <tr>
                  <td style='padding:0 32px 26px 32px;font-size:14px;line-height:1.6;color:#4a4e63;'>
                    Enter this code on the verification page to set a new password. Never share it with anyone. Cinema will never ask you for it.
                  </td>
                </tr>

                <tr>
                  <td style='padding:18px 32px 26px 32px;border-top:1px solid #e3e5ee;font-size:13px;line-height:1.6;color:#6b6f85;'>
                    If you didn't request a password reset, you can safely ignore this email. Your password will stay the same.
                  </td>
                </tr>
              </table>

              <p style='margin:18px 0 0 0;text-align:center;font-size:12px;color:#9a9cb5;'>
                &copy; {DateTime.Now.Year} Cinema. All rights reserved.
              </p>
            </div>";
            await _emailSender.SendEmailAsync(
                user.Email,
                "CinemaSystem Forget Password",
                message
                );
            TempData["Success"] = "OTP Sent Successfully, Please Check Your Email To Verify Your Account";
            return RedirectToAction(nameof(VerifyOtp), new { Id = user.Id });
        }
        [HttpGet]
        public IActionResult VerifyOtp(string Id)
        {
            return View(new VerifyOtpVM() { Id = Id });
        }
        [HttpPost]
        public async Task<IActionResult> VerifyOtp(VerifyOtpVM verifyOtpVM)
        {
            var UserOTPs = _otpRepo.GetAll(uo =>
                uo.ApplicationUserId == verifyOtpVM.Id &&
                uo.ExpirationTime > DateTime.UtcNow &&
                uo.IsUsed == false);
            var UserOTP = UserOTPs.OrderBy(uo => uo.CreatedAt).LastOrDefault();
            if (UserOTP == null || UserOTP.OTP != verifyOtpVM.OTP)
            {
                ModelState.AddModelError("", "Invalid / Expired OTP ");
                TempData["Error"] = "Invalid / Expired OTP ";
                return View(verifyOtpVM);
            }
            UserOTP.IsUsed = true;
            await _otpRepo.CommitAsync();
            var user = await _userManager.FindByIdAsync(verifyOtpVM.Id);
            if (user == null)
            {
                ModelState.AddModelError("", "User Not Found");
                TempData["Error"] = "User Not Found";
                return View(verifyOtpVM);
            }
            var ResetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            TempData["Success"] = "OTP Verified Successfully, Please Set Your New Password";
            return RedirectToAction(nameof(ResetPassword), new { UserId = user.Id, Token = ResetPasswordToken });
        }
        [HttpGet]
        public IActionResult ResetPassword(string UserId, string Token)
        {

            return View(new ResetPasswordVM { UserId = UserId, Token = Token });
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM resetPasswordVM)
        {
            var user = await _userManager.FindByIdAsync(resetPasswordVM.UserId);
            if (user == null)
            {
                ModelState.AddModelError("", "User Not Found");
                TempData["Error"] = "User Not Found";
                return View(resetPasswordVM);
            }
            var result = await _userManager.ResetPasswordAsync(user, resetPasswordVM.Token, resetPasswordVM.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                TempData["Error"] = "Please Fill All Fields Correctly";
                return View(resetPasswordVM);
            }
            TempData["Success"] = "Password Reset Successfully, You Can Login Now";
            return RedirectToAction(nameof(Login));
        }
    }
}