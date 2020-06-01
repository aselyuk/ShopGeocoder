using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopGeocoder.Data;
using ShopGeocoder.Models;

namespace ShopGeocoder.Controllers
{
    public class ApiKeysController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ApiKeysController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ApiKeys
        public async Task<IActionResult> Index()
        {
            return View(await _context.ApiKeys.ToListAsync());
        }

        // GET: ApiKeys/Error
        public IActionResult Error()
        {
            return View();
        }

        // GET: ApiKeys/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apiKey = await _context.ApiKeys
                .FirstOrDefaultAsync(m => m.Guid == id);
            if (apiKey == null)
            {
                return NotFound();
            }

            return View(apiKey);
        }

        // GET: ApiKeys/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ApiKeys/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Guid,Email,Key,Description")] ApiKey apiKey)
        {
            if (ModelState.IsValid)
            {
                apiKey.Guid = Guid.NewGuid();
                _context.Add(apiKey);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(apiKey);
        }

        // GET: ApiKeys/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apiKey = await _context.ApiKeys.FindAsync(id);
            if (apiKey == null)
            {
                return NotFound();
            }
            return View(apiKey);
        }

        // POST: ApiKeys/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Guid,Email,Key,Description,Default")] ApiKey apiKey)
        {
            if (id != apiKey.Guid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(apiKey);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApiKeyExists(apiKey.Guid))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(apiKey);
        }

        // GET: ApiKeys/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apiKey = await _context.ApiKeys
                .FirstOrDefaultAsync(m => m.Guid == id);
            if (apiKey == null)
            {
                return NotFound();
            }

            return View(apiKey);
        }

        // POST: ApiKeys/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var apiKey = await _context.ApiKeys.FindAsync(id);
            _context.ApiKeys.Remove(apiKey);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApiKeyExists(Guid id)
        {
            return _context.ApiKeys.Any(e => e.Guid == id);
        }
    }
}
