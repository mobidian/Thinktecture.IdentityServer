using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Thinktecture.IdentityServer.Models;
using Thinktecture.IdentityServer.Repositories;
using Thinktecture.IdentityServer.Web.ViewModels.Administration;

namespace Thinktecture.IdentityServer.Web.Controllers
{
    public class UserManagementController : Controller
    {
        [Import]
        public IUserRepository UserRepository { get; set; }

        [Import]
        public IRoleRepository RoleRepository { get; set; }

        public UserManagementController()
        {
            Container.Current.SatisfyImportsOnce(this);            
        }

        public ActionResult Index()
        {
            List<UserModel> users = new List<UserModel>();
            foreach(User user in UserRepository.GetUsers())
                users.Add(new UserModel() { UserName = user.UserName, Roles = user.Roles});

            UsersModel usersModel = new UsersModel() { Users = users };

            return View(usersModel);
        }

        public ActionResult Add()
        {
            ViewBag.Roles = RoleRepository.GetRoles();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(UserModel userModel)
        {
            try
            {
                UserRepository.Add(new User()
                                   {
                                       UserName = userModel.UserName,
                                       Password = userModel.Password,
                                       Email = userModel.Email,
                                       SecurityQuestion = userModel.SecurityQuestion,
                                       SecurityAnswer = userModel.SecurityAnswer,
                                       Roles = userModel.Roles
                                   });
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.Roles = RoleRepository.GetRoles();
                return View(userModel);
            }
        }

        public ActionResult Delete(string userName)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string userName, FormCollection collection)
        {
            try
            {
                UserRepository.Delete(userName);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException.Message);
                return View();
            }
        }

        public ActionResult Edit(string userName)
        {
            ViewBag.Roles = RoleRepository.GetRoles();

            User user = UserRepository.GetUser(userName);

            UserModel userModel = new UserModel()
                                      {
                                          UserName = user.UserName,
                                          Email = user.Email,
                                          SecurityQuestion = user.SecurityQuestion,
                                          SecurityAnswer = user.SecurityAnswer,
                                          Roles = user.Roles
                                      };

            return View(userModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserModel userModel)
        {
            try
            {
                UserRepository.Edit(new User()
                {
                    UserName = userModel.UserName,
                    Password = userModel.Password,
                    Email = userModel.Email,
                    SecurityQuestion = userModel.SecurityQuestion,
                    SecurityAnswer = userModel.SecurityAnswer,
                    Roles = userModel.Roles
                });
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.Roles = RoleRepository.GetRoles();
                return View(userModel);
            }
        }
    }
}
