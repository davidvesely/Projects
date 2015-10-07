using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web.Mvc;
using CarRental.Web.Core;
using CarRental.Web.Models;
using WebMatrix.WebData;

namespace CarRental.Web.Controllers
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RoutePrefix("account")]
    public class AccountController : ViewControllerBase
    {
        [ImportingConstructor]
        public AccountController(ISecurityAdapter securityAdapter)
        {
            _SecurityAdapter = securityAdapter;
        }

        ISecurityAdapter _SecurityAdapter;

        [HttpGet]
        [Route("register")]
        public ActionResult Register()
        {
            _SecurityAdapter.Initialize();
            
            return View();
        }

        [HttpGet]
        [Route("login")]
        public ActionResult Login(string returnUrl)
        {
            _SecurityAdapter.Initialize();

            return View(new AccountLoginModel() { ReturnUrl = returnUrl });
        }

        [HttpGet]
        [Route("logout")]
        public ActionResult Logout()
        {
            WebSecurity.Logout();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("changepassword")]
        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }
        
        [HttpGet]
        [Route("forgotpassword")]
        [Authorize]
        public ActionResult ForgotPassword()
        {
            return View();
        }
    }
}
