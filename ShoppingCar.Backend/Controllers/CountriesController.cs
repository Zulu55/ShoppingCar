using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ShoppingCar.Backend.Models;
using ShoppingCar.Domain.Models;

namespace ShoppingCar.Backend.Controllers
{
    public class CountriesController : Controller
    {
        private LocalDataContext db = new LocalDataContext();

        public async Task<ActionResult> DeleteCity(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var city = await db.Cities.FindAsync(id);

            if (city == null)
            {
                return HttpNotFound();
            }

            db.Cities.Remove(city);
            await db.SaveChangesAsync();
            return RedirectToAction($"Details/{city.CountryId}");
        }


        public async Task<ActionResult> EditCity(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var city = await db.Cities.FindAsync(id);

            if (city == null)
            {
                return HttpNotFound();
            }

            ViewBag.CountryId = new SelectList(db.Countries.OrderBy(c => c.Name), "CountryId", "Name", city.CountryId);
            return View(city);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditCity(City city)
        {
            if (ModelState.IsValid)
            {
                db.Entry(city).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction($"Details/{city.CountryId}");
            }

            ViewBag.CountryId = new SelectList(db.Countries.OrderBy(c => c.Name), "CountryId", "Name", city.CountryId);
            return View(city);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddCity(City city)
        {
            if (ModelState.IsValid)
            {
                db.Cities.Add(city);
                await db.SaveChangesAsync();
                return RedirectToAction($"Details/{city.CountryId}");
            }

            return View(city);
        }

        public async Task<ActionResult> AddCity(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var country = await db.Countries.FindAsync(id);

            if (country == null)
            {
                return HttpNotFound();
            }

            var city = new City { CountryId = country.CountryId, };
            return View(city);
        }

        public async Task<ActionResult> Index()
        {
            return View(await db.Countries.OrderBy(c => c.Name).ToListAsync());
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var country = await db.Countries.FindAsync(id);

            if (country == null)
            {
                return HttpNotFound();
            }

            return View(country);
        }

        // GET: Countries/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Countries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "CountryId,Name")] Country country)
        {
            if (ModelState.IsValid)
            {
                db.Countries.Add(country);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(country);
        }

        // GET: Countries/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Country country = await db.Countries.FindAsync(id);
            if (country == null)
            {
                return HttpNotFound();
            }
            return View(country);
        }

        // POST: Countries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "CountryId,Name")] Country country)
        {
            if (ModelState.IsValid)
            {
                db.Entry(country).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(country);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var country = await db.Countries.FindAsync(id);

            if (country == null)
            {
                return HttpNotFound();
            }

            db.Cities.RemoveRange(country.Cities);
            db.Countries.Remove(country);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
