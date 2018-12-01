namespace ShoppingCar.Backend.Controllers
{
    using System;
    using System.Data;
    using System.Data.Entity;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Domain.Models;
    using Models;
    using PagedList;

    [Authorize(Roles = "Admin")]
    public class SalesController : Controller
    {
        private LocalDataContext db = new LocalDataContext();

        public async Task<ActionResult> Deliver(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var sale = await db.Sales.FindAsync(id);

            if (sale == null)
            {
                return HttpNotFound();
            }

            if (sale.IsDeliveried)
            {
                ViewBag.ErrorMessage = "Esta venta ya fue despachada";
                return RedirectToAction($"Error/{id}");
            }

            sale.IsDeliveried = true;
            sale.DateDeliveried = DateTime.UtcNow;
            db.Entry(sale).State = EntityState.Modified;
            await db.SaveChangesAsync();

            return RedirectToAction($"Details/{id}");
        }

        public ActionResult Error(int id)
        {
            return View(id);
        }

        public ActionResult Index(int? page = null)
        {
            page = (page ?? 1);
            var sales = db.Sales.Include(s => s.Customer);
            return View(sales.OrderBy(s => s.IsDeliveried)
                .ThenByDescending(s => s.DateSale)
                .ToPagedList((int)page, 10));
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sale sale = await db.Sales.FindAsync(id);
            if (sale == null)
            {
                return HttpNotFound();
            }
            return View(sale);
        }

        // GET: Sales/Create
        public ActionResult Create()
        {
            ViewBag.CustomerId = new SelectList(db.Customers, "CustomerId", "FirstNames");
            return View();
        }

        // POST: Sales/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "SaleId,CustomerId,DateSale,Remarks,IsDeliveried,DateDeliveried")] Sale sale)
        {
            if (ModelState.IsValid)
            {
                db.Sales.Add(sale);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerId = new SelectList(db.Customers, "CustomerId", "FirstNames", sale.CustomerId);
            return View(sale);
        }

        // GET: Sales/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sale sale = await db.Sales.FindAsync(id);
            if (sale == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerId = new SelectList(db.Customers, "CustomerId", "FirstNames", sale.CustomerId);
            return View(sale);
        }

        // POST: Sales/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "SaleId,CustomerId,DateSale,Remarks,IsDeliveried,DateDeliveried")] Sale sale)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sale).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerId = new SelectList(db.Customers, "CustomerId", "FirstNames", sale.CustomerId);
            return View(sale);
        }

        // GET: Sales/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sale sale = await db.Sales.FindAsync(id);
            if (sale == null)
            {
                return HttpNotFound();
            }
            return View(sale);
        }

        // POST: Sales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Sale sale = await db.Sales.FindAsync(id);
            db.Sales.Remove(sale);
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
