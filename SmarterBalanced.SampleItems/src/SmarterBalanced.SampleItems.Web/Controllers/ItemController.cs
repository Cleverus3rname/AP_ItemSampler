using CoreFtp;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SmarterBalanced.SampleItems.Core.Repos;
using SmarterBalanced.SampleItems.Core.Repos.Models;
using SmarterBalanced.SampleItems.Dal.Configurations.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SmarterBalanced.SampleItems.Web.Controllers
{
    [Route("Item")]
    public class ItemController : Controller
    {
        private readonly IItemViewRepo repo;
        private readonly AppSettings appSettings;
        private readonly ILogger logger;

        public ItemController(IItemViewRepo itemViewRepo, AppSettings settings, ILoggerFactory loggerFactory)
        {
            repo = itemViewRepo;
            appSettings = settings;
            logger = loggerFactory.CreateLogger<ItemController>();
        }

        // GET: /<controller>/
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Index()
        {
            logger.LogDebug($"{nameof(Index)} redirect to itemssearch");

            return RedirectToActionPermanent("Index", "itemsSearch");
        }

        /// <summary>
        /// Converts a base64 encoded, serialized JSON string to
        /// a dictionary representing user accessibility preferences.
        /// </summary>
        private Dictionary<string, string> DecodeCookie(string base64Cookie)
        {
            try
            {
                byte[] data = Convert.FromBase64String(base64Cookie);
                string utf8Cookie = Encoding.UTF8.GetString(data);

                var cookiePreferences =
                    JsonConvert.DeserializeObject<Dictionary<string, string>>(utf8Cookie) ?? new Dictionary<string, string>();
                return cookiePreferences;
            }
            catch (Exception e)
            {
                logger.LogInformation(
                    "Unable to deserialize user accessibility options from cookie. Reason: " + e.Message);
                return new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// Returns an ItemDigest given a bankKey and itemKey, setting
        /// ISAAP based on URL or cookie if URL ISAAP not specified.
        /// </summary>
        /// <param name="bankKey"></param>
        /// <param name="itemKey"></param>
        /// <param name="iSAAP"></param>
        [HttpGet("Details")]
        public IActionResult Details(int? bankKey, int? itemKey, string iSAAP)
        {
            if (!bankKey.HasValue || !itemKey.HasValue)
            {
                logger.LogWarning($"{nameof(Details)} null param(s) for {bankKey} {itemKey}");
                return BadRequest();
            }

            string cookieName = appSettings.SettingsConfig.AccessibilityCookie;
            string cookieString = Request?.Cookies[cookieName] ?? string.Empty;
            var cookiePreferences = DecodeCookie(cookieString);

            string[] isaapCodes = string.IsNullOrEmpty(iSAAP) ? new string[0] : iSAAP.Split(';');

            var itemViewModel = repo.GetItemViewModel(bankKey.Value, itemKey.Value, isaapCodes, cookiePreferences);
            if (itemViewModel == null)
            {
                logger.LogWarning($"{nameof(Details)} invalid item {bankKey} {itemKey}");
                return BadRequest();
            }

            return View(itemViewModel);
        }

        [HttpGet("Braille")]
        public async Task<ActionResult> Braille(int? bankKey, int? itemKey, string brailleCode)
        {
            if(!bankKey.HasValue || !itemKey.HasValue || string.IsNullOrEmpty(brailleCode))
            {
                return BadRequest();
            }

            var fileName = repo.GenerateBrailleZipName(itemKey.Value, brailleCode);
            try
            {
                var ftpReadStream = await repo.GetItemBrailleZip(
                    bankKey.Value, 
                    itemKey.Value, 
                    brailleCode);

                Response.Cookies.Append("brailleDLstarted", "1", new Microsoft.AspNetCore.Http.CookieOptions {
                    Path = "/",
                    HttpOnly = false,
                    Secure = false,
                    Expires = DateTimeOffset.Now.AddSeconds(10)
                }
                    
                    );
                return File(ftpReadStream, "application/zip", fileName);
            } catch(Exception e)
            {
                logger.LogError($"{nameof(Braille)} failed to load braille for {itemKey.Value}, message {e.Message}");

                return BadRequest();
            }
        }

        [HttpGet("AboutThisItemViewModel")]
        [EnableCors("AllowAllOrigins")]
        public IActionResult AboutThisItemViewModel(int? bankKey, int? itemKey)
        {
            if(!bankKey.HasValue || !itemKey.HasValue)
            {
                return BadRequest();
            }

            var aboutThis = repo.GetAboutThisItemViewModel(bankKey.Value, itemKey.Value);

            return Json(aboutThis);
        }

   
        [HttpGet("ItemAccessibility")]
        public IActionResult AccessibilityResourceGroupIsaap(int? bankKey, int? itemKey, string[] isaapCodes = default(string[]))
        {
            if (!bankKey.HasValue || !itemKey.HasValue)
            {
                return BadRequest();
            }

            var accResourceGroup = repo.GetAccessibilityResourceGroup(bankKey.Value, itemKey.Value, isaapCodes);

            return Json(accResourceGroup);
        }
    }

}
