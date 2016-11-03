﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SmarterBalanced.SampleItems.Core.Repos;
using SmarterBalanced.SampleItems.Core.Repos.Models;


namespace SmarterBalanced.SampleItems.Web.Controllers
{
    public class GlobalAccessibilityController : Controller
    {
        private IGlobalAccessibilityRepo repo;

        public GlobalAccessibilityController(IGlobalAccessibilityRepo globalAccessibilityRepo)
        {
            repo = globalAccessibilityRepo;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            string cookieName = repo.GetSettings().SettingsConfig.AccessibilityCookie;
            var iSSAP = Request.Cookies[cookieName];
            GlobalAccessibilityViewModel viewmodel = repo.GetGlobalAccessibilityViewModel(iSSAP);

            return View(viewmodel);
        }

        [HttpPost]
        public IActionResult Index(GlobalAccessibilityViewModel model)
        {
            string cookieName = repo.GetSettings().SettingsConfig.AccessibilityCookie;
            string iSSAPCode = repo.GetISSAPCode(model);
            Response.Cookies.Append(cookieName, iSSAPCode);

            return RedirectToAction("Index");
        }


        public IActionResult Reset()
        {
            string cookieName = repo.GetSettings().SettingsConfig.AccessibilityCookie;
            Response.Cookies.Delete(cookieName);

            return RedirectToAction("Index");
        }

    }

}
