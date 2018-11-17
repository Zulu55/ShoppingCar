using ShoppingCar.Backend.Models;
using ShoppingCar.Domain.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ShoppingCar.Backend.Controllers
{
    public class HomeController : Controller
    {
        private LocalDataContext db = new LocalDataContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public async Task<ActionResult> Register()
        {
            ViewBag.CountryId = new SelectList(await this.GetCountries(), "CountryId", "Name");
            ViewBag.CityId = new SelectList(await this.GetCities(0), "CityId", "Name");
            return View();
        }

        private async Task<List<City>> GetCities(int countryId)
        {
            var cities = await db.Cities.OrderBy(c => c.Name).Where(c => c.CountryId == countryId).ToListAsync();
            cities.Insert(0, new City
            {
                CityId = 0,
                Name = "[Seleccione una ciudad...]"
            });

            return cities;
        }

        private async Task<List<Country>> GetCountries()
        {
            var countries = await db.Countries.OrderBy(c => c.Name).ToListAsync();
            countries.Insert(0, new Country
            {
                CountryId = 0,
                Name = "[Seleccione un país...]"
            });

            return countries;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Customers.Add(customer);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CityId = new SelectList(db.Cities, "CityId", "Name", customer.CityId);
            return View(customer);
        }
    }
}