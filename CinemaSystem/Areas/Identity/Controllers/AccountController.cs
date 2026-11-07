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
                return View(registerVM);
            }
            ApplicationUser user = new ApplicationUser(){
                Name=registerVM.Name,
                UserName=registerVM.UserName,
                Address=registerVM.Address,
                Email=registerVM.Email
            };
            var result = await _userManager.CreateAsync(user,registerVM.Password);
            if (!result.Succeeded)
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("",error.Description);
                }
                return View(registerVM);
            }
            var ConfirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user); 
            var link = Url.Action("ConfirmEmail", "Account", new {area="Identity",Id = user.Id,Token = ConfirmEmailToken},Request.Scheme);
            await _emailSender.SendEmailAsync(
                email:registerVM.Email,
                "CinemaSystem Email Confirmation",
                $"Please Click On That <a href='{link}'>Link</a> For Verifying"
                );
            return RedirectToAction(nameof(Login));
        }
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string Id, string Token)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if(user == null)
            {
                return NotFound();
            }
            await _userManager.ConfirmEmailAsync(user,Token);
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
                return View(loginVM);
            }
            var result =  await _signInManager.PasswordSignInAsync(user,loginVM.Password, loginVM.RememberMe,true);
            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("","To Many Attempts Please Try Again Later");
                }
                else
                {
                    ModelState.AddModelError("","Invalid UserName Or Email Or Password");
                }
                return View(loginVM);

            }
            return RedirectToAction("Index", "Home", new {Area="Customer"});
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
            if(user == null)
            {
                ModelState.AddModelError("","User Not Found");
                return View(resendEmailConfiramtionVM);
            }
            var ConfirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var Link = Url.Action(nameof(ConfirmEmail), "Account", new {area=nameof(Identity), Id = user.Id, Token = ConfirmEmailToken}, Request.Scheme);
            await _emailSender.SendEmailAsync(
                user.Email,
                "CinemaSystem Email Confirmation",
                $"Please Click On That <a href='{Link}'>Link</a> For Verifying Your Email"
                );
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
            var user = await _userManager.FindByEmailAsync(forgetPasswordVM.UserNameOrEmail)??
                await _userManager.FindByNameAsync(forgetPasswordVM.UserNameOrEmail);
            if (user == null)
            {
                ModelState.AddModelError("", "User NotFound");
                return View(forgetPasswordVM);
            }
            var Otp = new Random().Next(1000,10000).ToString();
            ApplicationUserOTP applicationUserOTP = new ApplicationUserOTP(user.Id, Otp);
            await _otpRepo.AddAsync(applicationUserOTP);
            await _otpRepo.CommitAsync();
            await _emailSender.SendEmailAsync(
                user.Email,
                "CinemaSystem Forget Password",
                $"Use This OTP {Otp} To Reset Your Password"
                );
            return RedirectToAction(nameof(VerifyOtp), new {Id = user.Id});
        }
        [HttpGet]
        public IActionResult VerifyOtp(string Id)
        {
            return View(new VerifyOtpVM() { Id = Id});
        }
        [HttpPost]
        public async Task<IActionResult> VerifyOtp(VerifyOtpVM verifyOtpVM)
        {
            var UserOTPs = _otpRepo.GetAll(uo=>
                uo.ApplicationUserId==verifyOtpVM.Id &&
                uo.ExpirationTime > DateTime.UtcNow&&
                uo.IsUsed==false);
            var UserOTP = UserOTPs.OrderBy(uo=>uo.CreatedAt).LastOrDefault();
            if(UserOTP == null || UserOTP.OTP != verifyOtpVM.OTP)
            {
                ModelState.AddModelError("", "Invalid / Expired OTP ");
                return View(verifyOtpVM);
            }
            UserOTP.IsUsed = true;
            await _otpRepo.CommitAsync();
            var user = await _userManager.FindByIdAsync(verifyOtpVM.Id);
            if(user == null)
            {
                ModelState.AddModelError("", "User Not Found");
                return View(verifyOtpVM);
            }
            var ResetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            return RedirectToAction(nameof(ResetPassword),new {UserId = user.Id,Token  = ResetPasswordToken});
        }
        [HttpGet]
        public IActionResult ResetPassword(string UserId,string Token)
        {

            return View(new ResetPasswordVM{UserId=UserId,Token=Token});
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM resetPasswordVM)
        {
            var user = await _userManager.FindByIdAsync(resetPasswordVM.UserId);
            if (user == null)
            {
                ModelState.AddModelError("", "User Not Found");
                return View(resetPasswordVM);
            }
            var result= await _userManager.ResetPasswordAsync(user, resetPasswordVM.Token, resetPasswordVM.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(resetPasswordVM);
            }
            return RedirectToAction(nameof(Login));
        }
    }
}