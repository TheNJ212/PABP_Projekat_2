using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PABP_Projekat_2.Context;
using PABP_Projekat_2.Models;
using PABP_Projekat_2.Extensions;

namespace PABP_Projekat_2.Controllers
{
    public class ProductsController : Controller
    {
        private readonly NorthwindContext _context;

        public ProductsController(NorthwindContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index(int? pageNumber)
        {
            int pageSize = 5;
            var products = _context.Products.AsNoTracking().Include(p => p.Category).Include(p => p.Supplier);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "CompanyName");
            return View(await products.ToPagedListAsync(pageNumber ?? 1, pageSize));
        }



        //public async Task<IActionResult> Filter()
        //{

        //    var Customers = await _context.Customers.AsNoTracking().ToListAsync();
        //    ViewData["CustomerId"] = new SelectList(Customers, "CustomerId", "CompanyName");
        //    TempData["showEmpty"] = false;
        //    return View(new List<OrderDetail>());
        //}

        public async Task<IActionResult> Filter(string CustomerId)
        {
            TempData["showEmpty"] = false;
            ViewData["CustomerId"] = new SelectList(await _context.Customers.AsNoTracking().ToListAsync(), "CustomerId", "CompanyName", CustomerId);
            if (!string.IsNullOrEmpty(CustomerId))
            {
                ViewData["OrderId"] = new SelectList(await _context.Orders.AsNoTracking().Where(x => x.CustomerId == CustomerId).ToListAsync(), "OrderId", "OrderId");
                TempData["showEmpty"] = true;
            }
            return View("Filter", new List<OrderDetail>());
        }


        public async Task<IActionResult> Filter2(int OrderId, string CustomerId)
        {
            var orders = await _context.Orders.AsNoTracking().FirstOrDefaultAsync(x => x.OrderId == OrderId);
            var orderDetails = await _context.OrderDetails.AsNoTracking().Include(x => x.Product).ThenInclude(x=>x.Supplier).Include(x=>x.Product).ThenInclude(x=>x.Category).Where(x => x.OrderId == OrderId).ToListAsync();
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CompanyName", CustomerId);
            ViewData["OrderId"] = new SelectList(_context.Orders.AsNoTracking().Where(x => x.CustomerId == CustomerId), "OrderId", "OrderId", OrderId);
            TempData["showEmpty"] = false;

            return View("Filter", orderDetails);
        }

        public async Task<IActionResult> ProductsCountedAndGroup(int? pageNumber)
        {
            int pageSize = 10;
            var orderDetails = await _context.OrderDetails.AsNoTracking().Include(x => x.Order).Include(x => x.Product).ThenInclude(x => x.Supplier).ToListAsync();
            var grouped = orderDetails.GroupBy(x => new { x.OrderId, x.Product.SupplierId }).Select(g =>
               new OrderSupplierProductCount()
               {
                   Order = g.FirstOrDefault().Order,
                   ProductCount = g.Count(),
                   Supplier = g.FirstOrDefault().Product.Supplier
               }).OrderByDescending(x => x.ProductCount).ToPagedList(pageNumber ?? 1, pageSize);


            return View("Group", grouped);
        }



        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create(int supplierId)
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            var supplier = _context.Suppliers.FirstOrDefault(x => x.SupplierId == supplierId);
            if (supplier is not null)
            {
                ViewData["SupplierId"] = new SelectList(new [] { supplier }, "SupplierId", "CompanyName",supplier.SupplierId);
            }
            else
            {
                ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "CompanyName");
            }
            
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductName,SupplierId,CategoryId,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                TempData["success"] = "Product added successfully!";
                return RedirectToAction(nameof(Create), new { supplierId = product.SupplierId });
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "CompanyName", product.SupplierId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "CompanyName", product.SupplierId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,SupplierId,CategoryId,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "CompanyName", product.SupplierId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
