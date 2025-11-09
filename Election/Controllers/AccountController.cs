using Election.Authorization;
using Election.Dto;
using Election.Models;
using Election.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Election.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        #region SignIn

        // GET: Account
        [Authorized]
        public ActionResult Index()
        {
            return AdminProfile();
        }
        [Authorized]
        private ActionResult AdminProfile()
        {
            var adminId = (int)TempData["PersonId"];
            using (var dbContext = new ElectionsDbEntities())
            {
                var result = dbContext.Users.Where(x => x.Id == adminId).Select(x => new SignInVM()
                {
                    UserName = x.UserName,
                    Email = x.EmployeeName,

                }).FirstOrDefault(); 
                return View("ProfileAdmin", result);

            }

        }
        [Authorized]

        public ActionResult EditAdminProfile()
        {
            var adminId = (int)TempData["PersonId"];
            using (var dbContext = new ElectionsDbEntities())
            {
                var result = dbContext.Users.Where(x => x.Id == adminId).Select(x => new SignInVM()
                {
                    UserName = x.UserName,
                    Email = x.EmployeeName,

                }).FirstOrDefault();

                return View("EditAdminProfile", result);
            }
        }

        [HttpPost]
        public ActionResult EditAdminProfile(SignInVM editAdminVM)
        {
            if (ModelState.IsValid)
            {
                using (var dbContext = new ElectionsDbEntities())
                {
                    var adminId = (int)TempData["UserId"];

                    var person = dbContext.Users.Where(x => x.Id == adminId).FirstOrDefault();

                    person.Password = Security.Encrypt(editAdminVM.Password);
                    person.UserName = editAdminVM.Email;
                    person.UserName = editAdminVM.UserName;

                    dbContext.SaveChanges();

                }


                TempData["success"] = "تم حفط البيانات بنجاح";
                return RedirectToAction("Index", "Dashboard");
            }


            TempData["warning"] = "برجاء ادخال البيانات المطلوبة";

            return RedirectToAction("Index", "Dashboard");
        }
        public ActionResult SignIn()
        {
            var pass = Security.Encrypt("Ybewfc1654#GFaqw");
            using (var dbContext = new ElectionsDbEntities())
            {
                var user = dbContext.Users.FirstOrDefault(x=>x.UserName=="Admin").Password ;
                var usewwr = dbContext.Users.FirstOrDefault(x => x.UserName == "Admin").UserName ;
                var pass11 = Security.Decrypt(user);

            }
            return View(new SignInVM());
        }

        [HttpPost]
        public ActionResult SignIn(SignInVM VM)
        {

            VTSAuth auth = new VTSAuth();

            using (var dbContext = new ElectionsDbEntities())
            {
                if (string.IsNullOrEmpty(VM.Password))
                {
                    TempData["warning"] = "تأكد من ادخال كلمة المرور";
                    return View(VM);
                }
                if (string.IsNullOrEmpty(VM.UserName))
                {
                    TempData["warning"] = "تأكد من ادخال اسم المستخدم ";
                    return View(VM);
                }
                var pass = Security.Encrypt(VM.Password);

                var user = dbContext.Users.FirstOrDefault(x =>x.UserName == VM.UserName && x.Password == pass);

                if (user == null)
                {
                    TempData["warning"] = "بيانات الدخول غير صحيحة !! ";
                    return View(VM);
                }
                TempData["success"] = "تم تسجيل الدخول بنجاح";



                var Data = new UserInfo()
                {
                    FirstName = user.EmployeeName,
                    UserId = user.Id,
                    RoleId=user.Id
                };
                auth.SaveToCookies(Data);
                return RedirectToAction("Index", "Home");

            }


        }


        #endregion

        #region SignOut
        public ActionResult SignOut()
        {
            VTSAuth auth = new VTSAuth();
            auth.LoadDataFromCookies();
            auth.ClearCookies();
            return Redirect(Url.Action("SignIn", "Account"));
        }
        #endregion
    }
}