﻿namespace BlingMeMVC.Controllers
{
    using System.Data;
    using System.Linq;
    using System.Web.Mvc;

    using BlingMeMVC.Models.ViewModels;
    using BlingMe.Domain.EF;
    using BlingMe.Domain.Entities;


    public class HomeController : Controller
    {
        private UnitOfWork uow = new UnitOfWork();

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {

            var repo = uow.GetRepository<Bracelet>();
            var loggedOnUserId = GetLoggedOnUserId();

            var modelBracelet = repo.Get(filter: b => b.Owner == loggedOnUserId
                && b.Type == BraceletType.Person).Single();


            return RedirectToRoute("Bracelet", new { id = modelBracelet.ID });
            
            /*
            var mock = new Mocks();
            var braceletId = (from b in mock.Bracelets
                              where b.Owner == User.Identity.Name.Replace("AVELO\\", string.Empty)
                              select b.ID).FirstOrDefault();

            // get the user's own bracelet number from EF and redirect to it
            // User.Identity.Name gives you their Avelo name so this is easy enough
            return RedirectToRoute("Bracelet", new { id = braceletId });
             */
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Bracelet(int id)
        {
            
            var repo = uow.GetRepository<Bracelet>();
            var charmsRepo = uow.GetRepository<Charm>();

            var modelBracelet = repo.Get(filter: b => b.ID == id).FirstOrDefault();

            // Dirty hack - investigate this
            var charms = charmsRepo.Get(filter: c => c.ParentID == modelBracelet.ID);
            modelBracelet.Charms = charms.ToList();


            return View(new BraceletView(modelBracelet, GetLoggedOnUserId()));
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Bracelet(int id, string query)
        {
            var repo = uow.GetRepository<Bracelet>();
            var modelBracelet = repo.Get(filter: b => b.ID.ToString() == query).FirstOrDefault();

            /*
            var mock = new Mocks();
            var modelBracelet = (from b in mock.Bracelets where b.ID.ToString() == query select b).FirstOrDefault();
            */

            return RedirectToRoute("Bracelet", new { id = modelBracelet.ID });
        }

        [AllowAnonymous]
        public ActionResult _Search()
        {
            var repo = uow.GetRepository<Bracelet>();
            var model = (from b in repo.Get() select b).ToList();

            return PartialView(model);
        }


        private string GetLoggedOnUserId()
        {
            return User.Identity.Name.Replace("AVELO\\", string.Empty);
        }
    }
}
