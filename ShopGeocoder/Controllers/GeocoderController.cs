using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopGeocoder.Data;

namespace ShopGeocoder.Controllers
{
    public class GeocoderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private string ApiKey { get; set; }

        public GeocoderController(ApplicationDbContext context)
        {
            _context = context;

            var key = _context.ApiKeys.Where(w => w.Default).FirstOrDefault();

            ApiKey = key != null ? key.Key : "";
        }

        // GET: GeocoderController
        public async Task<IActionResult> Index()
        {
            if (ApiKey.Equals(string.Empty))
                return RedirectToAction("Error", "ApiKeys");

            ViewData["ApiKey"] = ApiKey;            
            return View(await _context.Shops.Take(100).ToListAsync());
        }

        [HttpPost]
        public async Task<JsonResult> UpdateCoords(Guid id)
        {
            if (id != null)
            {
                try
                {
                    var shop = _context.Shops
                        .Where(w => w.Guid == id)
                        .FirstOrDefaultAsync().Result;

                    if (shop != null)
                    {
                        var lat = Convert.ToDouble(Request.Form["lat"].FirstOrDefault().Replace(".", ","));
                        var lon = Convert.ToDouble(Request.Form["lon"].FirstOrDefault().Replace(".", ","));
                        shop.Latitude = lat;
                        shop.Longitude = lon;
                        await _context.SaveChangesAsync();
                    }
                    return Json(shop);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShopExists(id))
                    {
                        return Json(null);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            else
            {
                return Json(null);
            }
        }

        private bool ShopExists(Guid id)
        {
            return _context.Shops.Any(e => e.Guid == id);
        }

        // GET: GeocoderController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: GeocoderController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: GeocoderController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: GeocoderController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: GeocoderController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: GeocoderController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: GeocoderController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
