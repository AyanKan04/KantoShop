using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using KantoShop.Models;
using PagedList;

namespace KantoShop.Controllers
{
    public class ProductsController : Controller
    {
        private ShopKantoEntities db = new ShopKantoEntities();



        // GET: Products/ProductList
        public ActionResult ProductList()
        {
            // Lấy danh sách sản phẩm kèm theo danh mục (Category)
            var products = db.Products.Include(p => p.Category).ToList();

            // Trả về view với danh sách sản phẩm
            return View(products);
        }

        // Các phương thức còn lại giữ nguyên (Index, Details, Create, Edit, Delete)
        public ActionResult Index(string SearchString, string priceRange, string size, string productType, string sortOrder)
        {
            // Đảm bảo rằng ViewBag.ProductTypes được khởi tạo với danh sách các loại sản phẩm
            ViewBag.ProductTypes = new SelectList(db.Categories, "CategoryID", "CategoryName");

            var products = db.Products.Include(p => p.Category).AsQueryable();

            // Tìm kiếm theo tên sản phẩm
            if (!String.IsNullOrEmpty(SearchString))
            {
                var searchPattern = @"\b" + Regex.Escape(SearchString.Trim()) + @"\b"; // Regex for exact word match
                products = products.ToList()  // Execute query and load into memory
                    .Where(s => Regex.IsMatch(s.ProductName.ToLower(), searchPattern.ToLower())) // Apply regex in memory
                    .AsQueryable();  // Convert back to IQueryable
            }

            // Lọc theo giá
            if (!string.IsNullOrEmpty(priceRange) && priceRange != "0")
            {
                decimal? minPrice = 0;
                decimal? maxPrice = decimal.MaxValue;

                switch (priceRange)
                {
                    case "1": // Dưới 100,000 VND
                        maxPrice = 100000;
                        break;
                    case "2": // Từ 100,000 - 500,000 VND
                        minPrice = 100000;
                        maxPrice = 500000;
                        break;
                    case "3": // Trên 500,000 VND
                        minPrice = 500000;
                        break;
                }

                products = products.Where(p => p.Price >= minPrice && p.Price <= maxPrice);
            }

            // Lọc theo kích thước
            if (!string.IsNullOrEmpty(size) && size != "0")
            {
                products = products.Where(p => p.Size == size);
            }

            // Lọc theo loại sản phẩm (danh mục)
            if (!string.IsNullOrEmpty(productType) && productType != "0")
            {
                int categoryId = int.Parse(productType);
                products = products.Where(p => p.CategoryID == categoryId);
            }

            // Sắp xếp theo giá
            if (sortOrder == "asc")
            {
                products = products.OrderBy(p => p.Price);
            }
            else if (sortOrder == "desc")
            {
                products = products.OrderByDescending(p => p.Price);
            }

            // Trả về các sản phẩm đã lọc và sắp xếp
            return View(products.ToList());
        }



        // GET: Products/Details/{id}
        public ActionResult Details(int? id)
        {
            // Kiểm tra id có hợp lệ không
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Lấy sản phẩm chính từ ProductID
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound(); // Nếu không tìm thấy sản phẩm
            }

            // Lấy tất cả sản phẩm cùng CategoryID (trừ sản phẩm hiện tại)
            var relatedProducts = db.Products
                .Where(p => p.CategoryID == product.CategoryID && p.ProductID != product.ProductID)
                .Take(4)  // Giới hạn số lượng sản phẩm liên quan để giảm tải cho server (Tùy chọn)
                .ToList();

            // Truyền danh sách sản phẩm liên quan vào ViewBag
            ViewBag.RelatedProducts = relatedProducts;

            // Trả về view với sản phẩm chính
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,CategoryID,ProductName,Price,QuantityInStock,Size,Color,Description,ImageUrl,CreatedAt")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,CategoryID,ProductName,Price,QuantityInStock,Size,Color,Description,ImageUrl,CreatedAt")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
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
