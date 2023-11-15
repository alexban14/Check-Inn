using Check_Inn.Areas.Dashboard.ViewModels;
using Check_Inn.Entities;
using Check_Inn.Services;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

            //model.Users = usersService.SearchUser(searchTerm, AccomodationPackageID, page, recordSize);
            //model.Roles = accomodationPackagesService.GetAllAcomodationPackages();

            int totalRecords = 0; //accomodationsService.SearchAccomodationCount(searchTerm, AccomodationPackageID);

            model.Pager = new Check_Inn.ViewModels.Pager(totalRecords, page, recordSize);

            return View(model);
        }

        // GET: Dashboard/Accomodations/Create
        public ActionResult Action(int? ID)
        {
            AccomodationActionModel model = new AccomodationActionModel();

            model.AccomodationPackages = accomodationPackagesService.GetAllAcomodationPackages();

            if(ID > 0)
            {
                Accomodation accomodation = accomodationsService.GetAccomodationByID(ID.Value);

                model.ID = accomodation.ID;
                model.AccomodationPackageID = accomodation.AccomodationPackageID;
                model.Name = accomodation.Name;
                model.Description = accomodation.Description;
            }

            return View("Action", model);
        }

        // POST: Dashboard/Accomodations/Create
        [HttpPost]
        public JsonResult Action(Accomodation model)
        {
            JsonResult json = new JsonResult();
            bool result;

            Console.WriteLine("AccomodationPackageID: {0}, AccomodationName: {1}", model.AccomodationPackageID, model.Name);

            if (model.ID > 0)
            {
                Accomodation accomodation = accomodationsService.GetAccomodationByID(model.ID);
                accomodation.AccomodationPackageID = model.AccomodationPackageID;
                accomodation.Name = model.Name;
                accomodation.Description = model.Description;

                result = accomodationsService.UpdateAccomodation(accomodation);
            }
            else
            {
                Accomodation accomodation = new Accomodation();

                accomodation.AccomodationPackageID = model.AccomodationPackageID;
                accomodation.Name = model.Name;
                accomodation.Description = model.Description;

                result = accomodationsService.SaveAccomodation(accomodation);

            }


            if (result)
            {
                json.Data = new { Success = true };
            }
            else
            {
                json.Data = new { Success = false, Message = "Unable to perform action on Accomodation" };
            }

            return json;
        }

        // GET: Dashboard/Accomodations/Delete/5
        public ActionResult Delete(int ID)
        {
            AccomodationActionModel model = new AccomodationActionModel();

            Accomodation accomodation = accomodationsService.GetAccomodationByID(ID);   

            model.ID = accomodation.ID;
            model.Name = accomodation.Name;

            return View("Delete", model);
        }

        // POST: Dashboard/Accomodations/Delete/5
        //[HttpPost]
        //public ActionResult Delete(Accomodation model)
    }
}