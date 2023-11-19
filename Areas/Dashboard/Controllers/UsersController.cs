using Check_Inn.Areas.Dashboard.ViewModels;
using Check_Inn.Entities;
using Check_Inn.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Check_Inn.Areas.Dashboard.Controllers
{
    public class UsersController : Controller
    {
        private CheckInnUserManager _userManager;
        private CheckInnSignInManager _signInManager;
        private CheckInnRoleManager _roleManager;

        public CheckInnSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<CheckInnSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public CheckInnUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<CheckInnUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public CheckInnRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().GetUserManager<CheckInnRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        public UsersController()
        {
        }

        public UsersController(CheckInnUserManager userManager, CheckInnSignInManager signInManager, CheckInnRoleManager roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        // GET: Dashboard/Users
        public ActionResult Index(string searchTerm, string roleID, int? page)
        {
            int recordSize = 3;
            page = page ?? 1;

            UserListingModel model = new UserListingModel();

            model.SearchTerm = searchTerm;
            model.RoleID = roleID;
            model.Roles = RoleManager.Roles.ToList();

            model.Users = this.SearchUsers(searchTerm, roleID, page, recordSize);

            int totalRecords = this.SearchUserCount(searchTerm, roleID);

            model.Pager = new Check_Inn.ViewModels.Pager(totalRecords, page, recordSize);

            return View(model);
        }

        private IEnumerable<User> SearchUsers(string searchTerm, string roleID, int? page, int? recordSize)
        {
            var users = UserManager.Users.AsQueryable();

            if(!string.IsNullOrEmpty(searchTerm))
            {
                users = users.Where(a => a.Email.ToLower().Contains(searchTerm.ToLower()) );
            }

            if(!string.IsNullOrEmpty(roleID))
            {
                //users = users.Where(a => a.Email.ToLower().Contains(searchTerm.ToLower()) );
            }

            var skip = (page - 1) * recordSize;

            return users
                .OrderBy(x => x.Email)
                .Skip((int)skip)
                .Take((int)recordSize)
                .ToList();
        }

        public int SearchUserCount(string searchTerm, string roleID)
        {
            var users = UserManager.Users.AsQueryable();

            if(!string.IsNullOrEmpty(searchTerm))
            {
                users = users.Where(a => a.Email.ToLower().Contains(searchTerm.ToLower()) );
            }

            if(!string.IsNullOrEmpty(roleID))
            {
                //users = users.Where(a => a.Email.ToLower().Contains(searchTerm.ToLower()) );
            }

            return users.Count();
        }

        // GET: Dashboard/Users/Create
        public async Task<ActionResult> Action(string ID)
        {
            UserActionModel model = new UserActionModel();

            if(!string.IsNullOrEmpty(ID))
            {
                var user = await UserManager.FindByIdAsync(ID);

                model.ID = user.Id;
                model.FullName = user.FullName;
                model.Email = user.Email;
                model.Username = user.UserName;
                model.Country = user.Country;
                model.City = user.City;
                model.Address = user.Address;

            }

            return View("Action", model);
        }

        // POST: Dashboard/Users/Create
        [HttpPost]
        public async Task<JsonResult> Action(User model)
        {
            JsonResult json = new JsonResult();
            IdentityResult result;

            if (!string.IsNullOrEmpty(model.Id))
            {
                var user = await UserManager.FindByIdAsync(model.Id);

                user.FullName = model.FullName;
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.Country = model.Country;
                user.City = model.City;
                user.Address = model.Address;

                result = await UserManager.UpdateAsync(user);
            }
            else
            {
                User user = new User();

                user.FullName = model.FullName;
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.Country = model.Country;
                user.City = model.City;
                user.Address = model.Address;

                result = await UserManager.CreateAsync(user);
            }

            json.Data = new { Success = result.Succeeded, Message = result.Errors };

            return json;
        }

        // GET: Dashboard/Users/Delete/5
        public async Task<ActionResult> Delete(string ID)
        {
            UserActionModel model = new UserActionModel();

            User user = await UserManager.FindByIdAsync(ID);

            model.ID = user.Id;
            model.FullName = user.FullName;

            return View("Delete", model);
        }


        //POST: Dashboard/Users/Delete/5
        [HttpPost]
        public async Task<JsonResult> Delete(User model)
        {
            JsonResult json = new JsonResult();
            IdentityResult result;

            if (!string.IsNullOrEmpty(model.Id))
            {
                var user = await UserManager.FindByIdAsync(model.Id);

                result = await UserManager.DeleteAsync(user);

                json.Data = new { Success = result.Succeeded, Message = string.Join(", ", result.Errors) };
            }
            else
            {
                json.Data = new { Success = false, Message = "Invalid user" };
            }

            return json;
        }

        //GET: Dashboard/Users/UserRoles
        [HttpGet]
        public async Task<ActionResult> UserRoles(string ID)
        {
            User user = await UserManager.FindByIdAsync(ID);
            UserRolesModel model = new UserRolesModel();
            IEnumerable<string> userRoleIDs = user.Roles.Select(x => x.RoleId).ToList();

            model.UserID = user.Id;
            model.UserName = user.UserName;
            model.UserRoles = RoleManager.Roles.Where(x => userRoleIDs.Contains(x.Id)).ToList();
            model.Roles = RoleManager.Roles.Where(x => !userRoleIDs.Contains(x.Id)).ToList();

            return View("UserRoles", model);
        }

        //POST: DashBoard/Users/AssingUserRole
        [HttpPost]
        public async Task<JsonResult> UserRoleOperation(string userID, string roleID, bool toDelete = false)
        {
            JsonResult json = new JsonResult();

            User user = await UserManager.FindByIdAsync(userID);
            IdentityRole role = await RoleManager.FindByIdAsync(roleID);

            if (user != null && role != null)
            {
                IdentityResult result;
                if (!toDelete)
                {
                    result = await UserManager.AddToRoleAsync(userID, role.Name);
                }
                else
                {
                    result = await UserManager.RemoveFromRoleAsync(userID, role.Name);
                }

                json.Data = new { Success = result.Succeeded, Message = string.Join(", ", result.Errors) };
            }
            else
            {
                json.Data = new { Success = false, Message = "Invalid operation" };
            }

            return json;
        }
    }
}