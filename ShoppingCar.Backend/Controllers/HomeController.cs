namespace ShoppingCar.Backend.Controllers
{
    using System;
    using System.Collections;
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
        public ActionResult Confirm()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Confirm(Sale sale)
        {
            if (ModelState.IsValid)
            {
                var customer = await db.Customers.Where(c => c.Email.ToLower().Equals(User.Identity.Name.ToLower())).FirstOrDefaultAsync();
                if (customer == null)
                {
                    return HttpNotFound();
                }

                var saleDetailTmps = await db.SaleDetailTmps.Where(s => s.CustomerId == customer.CustomerId).ToListAsync();
                if (saleDetailTmps.Count != 0)
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            sale.CustomerId = customer.CustomerId;
                            sale.DateSale = DateTime.Now;
                            db.Sales.Add(sale);
                            await db.SaveChangesAsync();

                            foreach (var saleDetailTmp in saleDetailTmps)
                            {
                                var saleDetail = new SaleDetail
                                {
                                     Name = saleDetailTmp.Name,
                                     PercentDiscount = saleDetailTmp.PercentDiscount,
                                     Price = saleDetailTmp.Price,
                                     ProductId = saleDetailTmp.ProductId,
                                     Quantity = saleDetailTmp.Quantity,
                                     SaleId = sale.SaleId,
                                };

                                db.SaleDetails.Add(saleDetail);

                                var product = await db.Products.FindAsync(saleDetailTmp.ProductId);
                                if (product != null)
                                {
                                    product.Stock -= saleDetailTmp.Quantity;
                                }

                                db.Entry(product).State = EntityState.Modified;
                            }

                            db.SaleDetailTmps.RemoveRange(saleDetailTmps);
                            await db.SaveChangesAsync();
                            transaction.Commit();
                            return RedirectToAction("ShowCar");
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            ModelState.AddModelError(string.Empty, $"Error: {ex.Message}");
                            return View(sale);
                        }
                    }
                }

                ModelState.AddModelError(string.Empty, "Debes de agregar algunos artículos antes de confirmar la compra.");
            }

            return View(sale);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var saleDetailTmp = await this.db.SaleDetailTmps.FindAsync(id);

            if (saleDetailTmp == null)
            {
                return HttpNotFound();
            }

            db.SaleDetailTmps.Remove(saleDetailTmp);
            await db.SaveChangesAsync();
            return RedirectToAction("ShowCar");
        }

        [HttpPost]
        public async Task<ActionResult> Edit(SaleDetailTmp saleDetailTmp)
        {
            if (ModelState.IsValid)
            {
                var product = await db.Products.FindAsync(saleDetailTmp.ProductId);
                if (product == null)
                {
                    return HttpNotFound();
                }

                if (saleDetailTmp.Quantity >= product.QuantityDiscount)
                {
                    saleDetailTmp.PercentDiscount = product.PercentDiscount;
                }
                else
                {
                    saleDetailTmp.PercentDiscount = 0;
                }

                db.Entry(saleDetailTmp).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("ShowCar");
            }

            return View(saleDetailTmp);
        }

        [Authorize(Roles = "Customer")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var saleDetailTmp = await this.db.SaleDetailTmps.FindAsync(id);

            if (saleDetailTmp == null)
            {
                return HttpNotFound();
            }

            return View(saleDetailTmp);
        }

        [Authorize(Roles = "Customer")]
        public async Task<ActionResult> AddNewToCar()
        {
            ViewBag.ProductId = new SelectList(await this.GetProducts(), "ProductId", "Name");
            var view = new SaleDetailTmp { Quantity = 1 };
            return View(view);
        }

        [HttpPost]
        public async Task<ActionResult> AddNewToCar(SaleDetailTmp saleDetailTmp)
        {
            if (ModelState.IsValid)
            {
                var customer = await db.Customers.
                    Where(c => c.Email.ToLower().Equals(User.Identity.Name.ToLower())).
                    FirstOrDefaultAsync();

                if (customer == null)
                {
                    return HttpNotFound();
                }

                var saleDetailTmpOld = await db.SaleDetailTmps.
                    Where(sdt => sdt.CustomerId == customer.CustomerId && 
                                 sdt.ProductId == saleDetailTmp.ProductId).
                    FirstOrDefaultAsync();
                var product = await db.Products.FindAsync(saleDetailTmp.ProductId);

                if (saleDetailTmpOld == null)
                {
                    saleDetailTmp.CustomerId = customer.CustomerId;
                    saleDetailTmp.Name = product.Name;
                    saleDetailTmp.Price = product.Price;
                    if (saleDetailTmp.Quantity >= product.QuantityDiscount)
                    {
                        saleDetailTmp.PercentDiscount = product.PercentDiscount;
                    }

                    db.SaleDetailTmps.Add(saleDetailTmp);
                }
                else
                {
                    saleDetailTmpOld.Quantity += saleDetailTmp.Quantity;
                    if (saleDetailTmpOld.Quantity >= product.QuantityDiscount)
                    {
                        saleDetailTmpOld.PercentDiscount = product.PercentDiscount;
                    }

                    db.Entry(saleDetailTmpOld).State = EntityState.Modified;
                }

                await db.SaveChangesAsync();
                return RedirectToAction("ShowCar");
            }

            ViewBag.ProductId = new SelectList(await this.GetProducts(), "ProductId", "Name");
            return View(saleDetailTmp);
        }

        private async Task<List<Product>> GetProducts()
        {
            var products = await db.Products.OrderBy(p => p.Name).ToListAsync();
            products.Insert(0, new Product
            {
                ProductId = 0,
                Name = "[Seleccione un producto...]",
            });

            return products;
        }

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