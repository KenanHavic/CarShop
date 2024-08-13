using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProdajaAutomobila.Models;
using ProdajaAutomobila.Services;
using System.ComponentModel.DataAnnotations;

namespace ProdajaAutomobila.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IConfiguration configuration;
        private object error;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
        }
        public IActionResult Register()
        {
            if(signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(registerDto);
            }
            var user = new ApplicationUser()
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                UserName = registerDto.Email,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                Address = registerDto.Address,
                CreatedAt = DateTime.Now,
            };

            var result = await userManager.CreateAsync(user, registerDto.Password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "client");
                await signInManager.SignInAsync(user, false);
                return RedirectToAction("Index", "Home");
                
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
             return View(registerDto);
        }

        public async Task<IActionResult> Logout()
        {
            if (signInManager.IsSignedIn(User))
            {
                await signInManager.SignOutAsync();
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Login()
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(loginDto);

            }
            var result = await signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password,
                loginDto.RememberMe, false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else 
            {
                ViewBag.ErrorMessage = "Pogrešan pokušaj prijave";
            }

            return View(loginDto);
        }
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var appUser = await userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var profileDto = new ProfileDto()
            {
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
                Email = appUser.Email,
                PhoneNumber = appUser.PhoneNumber,
                Address = appUser.Address,

            };

            return View(profileDto);
        }

		[Authorize]
        [HttpPost]
		public async Task<IActionResult> Profile(ProfileDto profileDto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Molimo Vas ispunite polja";
                return View(profileDto);
            }
            var appUser = await userManager.GetUserAsync (User);
            if (appUser == null)
            {
                return RedirectToAction("Index", "Home");
            }

            appUser.FirstName = profileDto.FirstName;
            appUser.LastName = profileDto.LastName;
            appUser.UserName = profileDto.Email;
            appUser.Email = profileDto.Email;
            appUser.PhoneNumber = profileDto.PhoneNumber;
            appUser.Address = profileDto.Address;

            var result = await userManager.UpdateAsync (appUser);


            if (result.Succeeded)
            {
				ViewBag.SuccessMessage = "Profil je uspješno uređen!";

			}
			else
            {
                ViewBag.ErrorMessage = "Nije moguće trenutno urediti profil: " + result.Errors.First().Description;

            }


            return View(profileDto);
        }

        [Authorize]
        public IActionResult Password()
        {
            return View();
        }

		[Authorize]
        [HttpPost]
		public async Task<IActionResult> Password(PasswordDto passwordDto)
        {
            if (!ModelState.IsValid)
            {
				return View();
			}
            var appUser = await userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return RedirectToAction("Index", "Home");
                
            }
            var result = await userManager.ChangePasswordAsync(appUser,
                passwordDto.CurrentPassword, passwordDto.NewPassword);

            if (result.Succeeded)
            {
                ViewBag.SuccessMessage = "Šifra je uspješno izmijenjena!";
                
            }
            else
            {
                ViewBag.ErrorMessage = "Greška: " + result.Errors.First().Description;
            }

            return View();
		}


		public IActionResult AccessDenied()
        {
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ForgotPassword()
        {
            if(signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ForgotPassword([Required, EmailAddress] string email)
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Email = email;
            if(!ModelState.IsValid)
            {
                ViewBag.EmailError = ModelState["email"].Errors.First().ErrorMessage ?? "Pogrešna email adresa";
                return View();

            }

            var user = await userManager.FindByEmailAsync(email);

            if (user != null)
            {
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                string resetUrl = Url.ActionLink("ResetPassword", "Account", new { token }) ?? "URL Error";

                string senderName = configuration["BrevoSettings:SenderName"] ?? "";
                string senderEmail = configuration["BrevoSettings:SenderEmail"] ?? "";
                string username = user.FirstName + " " + user.LastName;
                string subject = "Promjena šifre";
                string message = "Poštovani " + username + ",\n\n" +
                    "Možete promijeniti Vašu šifru preko sljedećeg linka:\n\n" +
                    "Pozdrav";

                EmailSender.SendEmail(senderName, senderEmail, username, email, subject, message);
            }
            ViewBag.SuccessMessage = "Molimo provjerite Vaš email i pritisnite na link za restartovanje šifre!";

            return View();
        }

    }
}
