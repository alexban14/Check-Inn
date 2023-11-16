using Check_Inn.Areas.Dashboard.ViewModels;
using Check_Inn.Entities;
using Check_Inn.Services;
using Microsoft.AspNet.Identity;
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

        public UsersController()
        {
        }

        public UsersController(CheckInnUserManager userManager, CheckInnSignInManager signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Dashboard/Users
        public ActionResult Index(string searchTerm, string roleID, int? page)
        {
            int recordSize = 3;
            page = page ?? 1;

            UserListingModel model = new UserListingModel();

            model.SearchTerm = searchTerm;
            model.RoleID = roleID;

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

        // GET: Dashboard/Accomodations/Create
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

        // POST: Dashboard/Accomodations/Create
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

        /*
        // GET: Dashboard/Accomodations/Delete/5
        public ActionResult Delete(int ID)
        {
            AccomodationActionModel model = new AccomodationActionModel();

            Accomodation accomodation = accomodationsService.GetAccomodationByID(ID);   

            model.ID = accomodation.ID;
            model.Name = accomodation.Name;

            return View("Delete", model);
        }

        */

        // POST: Dashboard/Accomodations/Delete/5
        //[HttpPost]
        //public ActionResult Delete(Accomodation model)
    }
}