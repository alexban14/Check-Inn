using Check_Inn.Areas.Dashboard.ViewModels;
using Check_Inn.Services;
using Check_Inn.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Check_Inn.Areas.Dashboard.Controllers
{
    public class RolesController : Controller
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

        public RolesController()
        {
        }

        public RolesController(CheckInnUserManager userManager, CheckInnSignInManager signInManager, CheckInnRoleManager roleManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
        }



        // GET: Dashboard/Roles
        public ActionResult Index(string searchTerm, int? page)
        {
            int recordSize = 10;
            page = page ?? 1;

            RolesListingModel model = new RolesListingModel();

            model.SearchTerm = searchTerm;

            model.Roles = SearchRoles(searchTerm, page.Value, recordSize);
            
            var totalRecords = SearchRolesCount(searchTerm);

            model.Pager = new Pager(totalRecords, page, recordSize);

            return View(model);
        }
        private IEnumerable<IdentityRole> SearchRoles(string searchTerm, int? page, int? recordSize)
        {
            var roles = RoleManager.Roles.AsQueryable();

            if(!string.IsNullOrEmpty(searchTerm))
            {
                roles = roles.Where(a => a.Name.ToLower().Contains(searchTerm.ToLower()) );
            }

            var skip = (page - 1) * recordSize;

            return roles
                .OrderBy(x => x.Name)
                .Skip((int)skip)
                .Take((int)recordSize)
                .ToList();
        }

        public int SearchRolesCount(string searchTerm)
        {
            var roles = RoleManager.Roles.AsQueryable();

            if(!string.IsNullOrEmpty(searchTerm))
            {
                roles = roles.Where(a => a.Name.ToLower().Contains(searchTerm.ToLower()) );
            }


            return roles.Count();

        }

        // GET: Dashboard/Accomodations/Create
        public async Task<ActionResult> Action(string ID)
        {
            RolesActionModel model = new RolesActionModel();

            if(!string.IsNullOrEmpty(ID))
            {
                IdentityRole role = await RoleManager.FindByIdAsync(ID);

                model.ID = role.Id;
                model.Name = role.Name;
            }

            return View("Action", model);
        }

        // POST: Dashboard/Accomodations/Create
        [HttpPost]
        public async Task<JsonResult> Action(IdentityRole model)
        {
            JsonResult json = new JsonResult();
            IdentityResult result;

            if (!string.IsNullOrEmpty(model.Id))
            {
                IdentityRole role = await RoleManager.FindByIdAsync(model.Id);

                role.Name = model.Name;

                result = await RoleManager.UpdateAsync(role);
            }
            else
            {
                IdentityRole role = new IdentityRole();

                role.Name = model.Name;

                result = await RoleManager.CreateAsync(role);
            }

            json.Data = new { Success = result.Succeeded, Message = string.Join(", ", result.Errors) };

            return json;
        }

        // GET: Dashboard/Accomodations/Delete/5
        public async Task<ActionResult> Delete(string ID)
        {
            RolesActionModel model = new RolesActionModel();

            IdentityRole role = await RoleManager.FindByIdAsync(ID);

            model.ID = role.Id;
            model.Name = role.Name;

            return View("Delete", model);
        }


        //POST: Dashboard/Accomodations/Delete/5
        [HttpPost]
        public async Task<JsonResult> Delete(IdentityRole model)
        {
            JsonResult json = new JsonResult();
            IdentityResult result;

            if (!string.IsNullOrEmpty(model.Id))
            {
                IdentityRole role = await RoleManager.FindByIdAsync(model.Id);

                result = await RoleManager.DeleteAsync(role);

                json.Data = new { Success = result.Succeeded, Message = string.Join(", ", result.Errors) };
            }
            else
            {
                json.Data = new { Success = false, Message = "Invalid role" };
            }

            return json;
        }
    }
}