using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using GHM_ERP.Models;
using BusinessObjects;
using BusinessLogic;
using GHM_ERP.Util;

namespace GHM_ERP.Controllers
{
    [Authorize]
    public class AccountController : GHMController
    {
        BusinessLogic.UsersManager userManager = new UsersManager() ;

        public AccountController() 
        {
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        ////
        //// POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                ClaimsIdentity identity;
                var user = userManager.Authenticate(model.UserName, model.Password, out identity);
                if (user != null)
                {
                    SignInAsync(identity, model.RememberMe);
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Register
        [Authorize(Roles="admin")]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [Authorize(Roles = "admin")] 
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User() { UserName = model.UserName, FirstName = model.FirstName, LastName = model.LastName, Phone = model.Phone };
                userManager.CreateUser(user, model.Password);
                userManager.SetUserRole(model.UserName, model.Role);

                SetInfoAlert("User " + model.UserName + " created successfully");
                return RedirectToAction("Index");

            }

            // If we got this far, something failed, redisplay form
            return View(model); 
        }

       
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";

            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        [HttpPost]
        public ActionResult Manage(ManageUserViewModel manageUser)
        {
            if(ModelState.IsValid)
            {
                ClaimsIdentity identity;
                var user = userManager.Authenticate(GetCurrentLoggedUser().UserName, manageUser.OldPassword, out identity);

                if(user == null)
                {
                    ModelState.AddModelError("", "Invalid password");
                    return View();
                }
                else
                {
                    userManager.SetUserPassword(GetCurrentLoggedUser().UserName, manageUser.NewPassword);
                    return RedirectToAction("Manage", new { message = ManageMessageId.ChangePasswordSuccess });
                }
            }
            return View();

        }

        [Authorize(Roles="admin")]
        public ActionResult Index()
        {
            var users = userManager.LoadAllUsers();
            ViewBag.CurrentUserName = GetCurrentLoggedUser().UserName;
            return View(users);
        }

     
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");

        }

        [Authorize(Roles = "admin")]
        public ActionResult PasswordReset(string id)
        {
            var username = id;
            if(username == GetCurrentLoggedUser().UserName)
            {
                return RedirectToAction("Manage");
            }

            ViewBag.UserName = username;

            return View();
        }



        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult PasswordReset(string id, PasswordResetViewModel resetModel)
        {
            var username = id;
            if (ModelState.IsValid)
            {
                userManager.SetUserPassword(username, resetModel.NewPassword);
                SetInfoAlert("Password Changed Successfully");
                return RedirectToAction("PasswordReset", new { id = username });
            }
            return View();

        }


        //---------------------- HELPERS -=---------------

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

       

     

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private void SignInAsync(ClaimsIdentity identity, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }


        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
    }
}