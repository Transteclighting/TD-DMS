using System;
using System.Linq;
using System.Web.Mvc;
using DealerManagementSystem.Models;

namespace DealerManagementSystem.Controllers
{
    public class UserController : Controller
    {
        private ProjectDb db = new ProjectDb();
        // GET: /User/

        public ActionResult Login()
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
        public ActionResult Login(DmsUser aUser)
        {

            if (ModelState.IsValid)
            {
                var sql = @"select * from t_DMSUser Where UserName='" + aUser.UserName.Trim() + "' and IsActive=1";
                DmsUser logedUser = db.Database.SqlQuery<DmsUser>(sql).FirstOrDefault();
                if (logedUser != null)
                {
                    int? empId = null;
                    if (logedUser.Role == "Manager")
                    {
                        User user = db.Database.SqlQuery<User>(@"select Password,Salt,EmployeeID from t_User Where UserName='" + aUser.UserName.Trim() + "'").FirstOrDefault();
                        if (user != null)
                        {
                            logedUser.Password = user.Password;
                            logedUser.Salt = user.Salt;
                            empId = user.EmployeeID;

                        }
                    }

                    if (logedUser.Role == "Admin")
                    {
                        User user = db.Database.SqlQuery<User>(@"select Password,Salt,EmployeeID from t_User Where UserName='" + aUser.UserName.Trim() + "'").FirstOrDefault();
                        if (user != null)
                        {
                            logedUser.Password = user.Password;
                            logedUser.Salt = user.Salt;
                            empId = user.EmployeeID;

                        }
                    }
                    PdsaHash mph2 = new PdsaHash(PdsaHash.PDSAHashType.MD5);
                    string pass = mph2.CreateHash(aUser.Password, logedUser.Salt);
                    if (logedUser.Password == pass)
                    {
                        Session["LoggedUserName"] = logedUser.UserName;
                        Session["LogedUserPassword"] = logedUser.Password;
                        Session["LoggedUserFullName"] = logedUser.UserFullName;
                        Session["CustomerId"] = logedUser.CustomerId;
                        Session["UserId"] = logedUser.UserId;
                        // var outlet = db.DmsOutlet.First(a => a.CustomerId == logedUser.CustomerId);
                        var customer = db.Customers.First(a => a.CustomerId == logedUser.CustomerId);
                        if (customer != null)
                        {
                            Session["ParentCustomerId"] = customer.ParentCustomerId;
                        }
                        //if (outlet != null)
                        //{
                        //    Session["OutletName"] = outlet.OutletName;
                        //}
                        Session["UserType"] = logedUser.Role;
                        Session["EmpId"] = empId;
                        return RedirectToAction("Welcome");
                    }
                }
                ModelState.AddModelError("LoginFaild", "The user name or password is incorrect");
                return View();
            }
            return View();
        }
        public ActionResult Welcome()
        {
            if (Session["LoggedUserName"] != null && Session["LogedUserPassword"] != null)
            {
                if (Session["UserType"].ToString() == "Admin")
                {
                    return RedirectToAction("Dashboard", "Home");
                }
                else
                    return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        public ActionResult ChangePassword()
        {
            if (Session["LoggedUserName"] != null && Session["LogedUserPassword"] != null)
            {
                return View();
            }
            return RedirectToAction("Login");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePassword aChangePassword)
        {
            if (ModelState.IsValid)
            {
                var loggedUser = (from a in db.DmsUsers
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
                            db.Database.ExecuteSqlCommand("Update t_DMSUser Set Password='" + password + "',Salt='" + salt + "' Where UserId='" + userId + "' and UserName= '" + aChangePassword.UserName + "' ");
                            db.SaveChanges();
                            TempData["ChangePassStatus"] = 1;
                            TempData["ChangePassMsg"] = "You've Successfully Updated Password.";

                        }
                        catch (Exception)
                        {
                            TempData["ChangePassStatus"] = 2;
                            TempData["ChangePassMsg"] = "Failed To Updated Password.Please Try Again.";
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
