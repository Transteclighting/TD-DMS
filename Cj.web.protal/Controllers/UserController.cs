using System;
using System.Linq;
using System.Web.Mvc;
using Cj.web.Models;
using Cj.web.protal.Models;
using System.Web.Security;
using System.Web;
using Cj.web.protal.App_Start;
using System.Collections.Generic;

namespace Cj.web.Controllers
{
    public class UserController : Controller
    {
        private ProjectDb db = new ProjectDb();
        public ActionResult Login(string returnUrl)
        {
            if (Session["LoggedUserName"] != null && Session["LogedUserPassword"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User aUser, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                User logedUser = db.Database.SqlQuery<User>(@"select * from t_User Where UserName='" + aUser.UserName.Trim() + "'").FirstOrDefault();
                if (logedUser != null)
                {
                    PdsaHash mph2 = new PdsaHash(PdsaHash.PDSAHashType.MD5);
                    string pass = mph2.CreateHash(aUser.Password, logedUser.Salt);
                    if (logedUser.Password == pass)
                    {
                        FormsAuthentication.SetAuthCookie(logedUser.UserName, false);
                        var authTicket = new FormsAuthenticationTicket(1, logedUser.UserName, DateTime.Now, DateTime.Now.AddMinutes(20), false, logedUser.UserFullName);
                        string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                        var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                        HttpContext.Response.Cookies.Add(authCookie);
                        Session["LoggedUserName"] = logedUser.UserName;
                        Session["LogedUserPassword"] = logedUser.Password;
                        Session["LoggedUserFullName"] = logedUser.UserFullName;
                        Session["UserId"] = logedUser.UserId;
                        Session["Permissions"]  = getUserPermission(logedUser.UserId);
                        if (logedUser.EmployeeID !=null)
                        {
                            Session["UserImg"] = getUserImage(logedUser.EmployeeID.ToString());
                        }
                        if (!string.IsNullOrWhiteSpace(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        return RedirectToAction("Index", "Home");
                    }
                }
                ModelState.AddModelError("LoginFaild", "The user name or password is incorrect");
                return View();
            }
            return View();
        }
        public ActionResult Logout()
        {
            Session["LoggedUserName"] = null;
            Session["LogedUserPassword"] = null;
            Session["LoggedUserFullName"] = null;
            Session["UserId"] = null;
            Session["UserImg"] = null;
            return RedirectToAction("Login", "User");
        }
        public string getUserImage(string empId)
        {
            Byte[] imgByte = db.Database.SqlQuery<Byte[]>(@"Select EmployeePhoto from t_Employee Where EmployeeID =" + empId).FirstOrDefault();
            string base64String = Convert.ToBase64String(imgByte, 0, imgByte.Length);
            return "data:image/png;base64," + base64String;
        }

        public List<string> getUserPermission(int userId)
        {
            var qury = @"select PermissionKey from t_UserPermission where UserID=" + userId;
            List<string> permission = db.Database.SqlQuery<string>(qury).ToList();
            return permission;
        }
        [AuthenticationFilter]
        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePassword aChangePassword)
        {
            if (ModelState.IsValid)
            {
                var loggedUser =(from a in db.Users
                            where a.UserName == aChangePassword.UserName
                            select a).FirstOrDefault();

                PdsaHash mph2 = new PdsaHash(PdsaHash.PDSAHashType.MD5);
                string pass = mph2.CreateHash(aChangePassword.OldPassword, loggedUser.Salt);
                if (String.Compare(loggedUser.Password, pass, StringComparison.Ordinal) == 0)
                {
                    if (String.Compare(aChangePassword.NewPassword, aChangePassword.ConfirmPassword, StringComparison.Ordinal) == 0)
                    {
                        try
                        {
                            var userId = (int)Session["UserId"];
                            mph2 = new PdsaHash(PdsaHash.PDSAHashType.MD5);
                            string salt = mph2.CreateSalt();
                            string password = mph2.CreateHash(aChangePassword.ConfirmPassword, salt);
                            db.Database.ExecuteSqlCommand("Update t_User Set Password='" + password + "',Salt='" + salt + "' Where UserId='" + userId + "' and UserName= '" + aChangePassword.UserName + "' ");
                            db.SaveChanges();
                            TempData["ValidationStatus"] = 1;
                            TempData["ValidationMsg"] = "You've Successfully Updated Password.";

                        }
                        catch (Exception)
                        {
                            TempData["ValidationStatus"] = 2;
                            TempData["ValidationMsg"] = "Failed To Updated Password.Please Try Again.";
                        }

                    }
                    else
                    {
                        ModelState.AddModelError("LoginFaild", "New Password And Confirm Password Are Not Match");
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError("IncorrectOldPass", "Old Password Is Incorrect");
                    return View();
                }

            }

            return RedirectToAction("Index", "Home");
        }
    }
}
