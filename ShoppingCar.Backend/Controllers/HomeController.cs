namespace ShoppingCar.Backend.Controllers
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Domain.Models;
    using Helpers;
    using Models;

    public class HomeController : Controller
    {
        private LocalDataContext db = new LocalDataContext();


        [Authorize(Roles = "Customer")]
        public async Task<ActionResult> ShowCar()
        {
            var customer = await db.Customers.
                Where(c => c.Email.ToLower().Equals(User.Identity.Name.ToLower())).
                FirstOrDefaultAsync();

            if (customer == null)
            {
                return HttpNotFound();
            }

            var saleDetailTmps = await db.SaleDetailTmps.
                Include(sdt => sdt.Product).
                Include(sdt => sdt.Customer).
                Where(sdt => sdt.CustomerId == customer.CustomerId).
                OrderBy(sdt => sdt.Product.Name).
                ToListAsync();
            return View(saleDetailTmps);
        }

        [Authorize(Roles = "Customer")]
        public async Task<ActionResult> AddToCar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var product = await this.db.Products.FindAsync(id);

            if (product == null)
            {
                return HttpNotFound();
            }

            var customer = await db.Customers.
                Where(c => c.Email.ToLower().Equals(User.Identity.Name.ToLower())).
                FirstOrDefaultAsync();

            if (customer == null)
            {
                return HttpNotFound();
            }

            var saleDetailTmp = await db.SaleDetailTmps.
                Where(sdt => sdt.CustomerId == customer.CustomerId && 
                             sdt.ProductId == product.ProductId).
                FirstOrDefaultAsync();

            if (saleDetailTmp == null)
            {
                saleDetailTmp = new SaleDetailTmp
                {
                    CustomerId = customer.CustomerId,
                    Name = product.Name,
                    PercentDiscount = 0,
                    Price = product.Price,
                    ProductId = product.ProductId,
                    Quantity = 1,
                };

                db.SaleDetailTmps.Add(saleDetailTmp);
            }
            else
            {
                saleDetailTmp.Quantity++;
                db.Entry(saleDetailTmp).State = EntityState.Modified;
            }

            if (saleDetailTmp.Quantity >= product.QuantityDiscount)
            {
                saleDetailTmp.PercentDiscount = product.PercentDiscount;
            }

            await db.SaveChangesAsync();
            return RedirectToAction("Index2");
        }

        public async Task<ActionResult> Index2()
        {
            return View(await this.db.Products.ToListAsync());
        }

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
        public async Task<ActionResult> Register(CustomerView view)
        {
            if (ModelState.IsValid)
            {
                var response = UsersHelper.CreateUserASP(view.Email, "Customer", view.Password);
                if (response)
                {
                    var pic = string.Empty;
                    var folder = "~/Content/Customers";

                    if (view.ImageFile != null)
                    {
                        pic = FilesHelper.UploadPhoto(view.ImageFile, folder);
                        pic = $"{folder}/{pic}";
                    }

                    var customer = this.ToCustomer(view, pic);
                    db.Customers.Add(customer);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, "Este correo ya está siendo utilizado por otro cliente.");
            }

            ViewBag.CountryId = new SelectList(await this.GetCountries(), "CountryId", "Name", view.CountryId);
            ViewBag.CityId = new SelectList(await this.GetCities(view.CountryId), "CityId", "Name", view.CityId);
            return View(view);
        }

        private Customer ToCustomer(CustomerView view, string pic)
        {
            return new Customer
            {
                Address = view.Address,
                City = view.City,
                CityId = view.CityId,
                Confirm = view.Confirm,
                CustomerId = view.CustomerId,
                Email = view.Email,
                FirstNames = view.FirstNames,
                ImagePath = pic,
                LastNames = view.LastNames,
                Password = view.Password,
                Phone = view.Phone,
            };
        }

        public JsonResult GetCitiesJson(int countryId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var cities = db.Cities.Where(c => c.CountryId == countryId).OrderBy(c => c.Name);
            return Json(cities);
        }
    }
}