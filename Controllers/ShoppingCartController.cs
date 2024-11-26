using KantoShop.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KantoShop.Controllers
{
    public class ShoppingCartController : Controller
    {
        ShopKantoEntities db = new ShopKantoEntities();

        // GET: ShoppingCart, chuẩn bị dữ liệu cho View
        public ActionResult ShowCart()
        {
            if (Session["Cart"] == null)
                return View("ShowCart");
            Cart _cart = Session["Cart"] as Cart;
            return View(_cart);

        }

        // Tạo mới giỏ hàng, nguồn được lấy từ Session
        public Cart GetCart()
        {
            Cart cart = Session["Cart"] as Cart;
            if (cart == null || Session["Cart"] == null)
            {
                cart = new Cart();
                Session["Cart"] = cart;
            }
            return cart;
        }


        // Các phương thức cho đặt hàng thành công
        public ActionResult CheckOut(FormCollection form)
        {
            try
            {
                // Lấy giỏ hàng từ session
                Cart cart = Session["Cart"] as Cart;
                if (cart == null || cart.Items.Count() == 0)
                {
                    return Content("Giỏ hàng của bạn trống!");
                }

                // Tạo mới đối tượng đơn hàng
                var customerId = int.Parse(form["CodeCustomer"]);
                var customer = db.Customers.SingleOrDefault(c => c.CustomerID == customerId);
                if (customer == null)
                {
                    return Content("Khách hàng không tồn tại.");
                }

                Order newOrder = new Order
                {
                    CustomerID = customer.CustomerID,
                    OrderDate = DateTime.Now,
                    Status = "Chờ xử lý", // trạng thái mặc định
                    TotalAmount = cart.GetTotalMoney()
                };

                // Thêm đơn hàng vào cơ sở dữ liệu
                db.Orders.Add(newOrder);
                db.SaveChanges(); // Lưu đơn hàng vào cơ sở dữ liệu

                // Lưu chi tiết đơn hàng (OrderDetails)
                foreach (var item in cart.Items)
                {
                    OrderDetail orderDetail = new OrderDetail
                    {
                        OrderID = newOrder.OrderID,
                        ProductID = item._product.ProductID,
                        Quantity = item._quantity,
                        UnitPrice = item._product.Price
                    };
                    db.OrderDetails.Add(orderDetail); // Thêm vào chi tiết đơn hàng
                }

                // Lưu thay đổi trong bảng OrderDetails
                db.SaveChanges();

                // Xóa giỏ hàng sau khi đặt hàng thành công
                cart.ClearCart();

                // Chuyển hướng đến trang thành công
                return RedirectToAction("CheckOut_Success", "ShoppingCart");
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return Content("Có sai sót! Xin kiểm tra lại thông tin. Lỗi: " + ex.Message);
            }
        }

        // Tính tổng tiền đơn hàng
        public PartialViewResult BagCart()
        {
            decimal total_money_item = 0;
            Cart cart = Session["Cart"] as Cart;
            if (cart != null)
                total_money_item = cart.GetTotalMoney();
            ViewBag.TotalCart = total_money_item;
            return PartialView("BagCart");
        }

        // Giỏ hàng của người dùng (lưu trong Session hoặc trong DB)
        private List<CartItem> GetCartItems()
        {
            var cart = Session["Cart"] as List<CartItem>;
            if (cart == null)
            {
                cart = new List<CartItem>();
                Session["Cart"] = cart;
            }
            return cart;
        }

        // ShoppingCartController.cs
        [HttpPost]
        public ActionResult AddToCart(int id, int quantity)
        {
            // Kiểm tra giá trị của id và quantity
            if (id <= 0 || quantity <= 0)
            {
                return RedirectToAction("ShowCart", "ShoppingCart"); // Trở lại giỏ hàng nếu có lỗi
            }

            var product = db.Products.SingleOrDefault(p => p.ProductID == id);
            if (product != null)
            {
                // Thêm sản phẩm vào giỏ hàng
                Cart cart = GetCart();  // Lấy giỏ hàng hiện tại từ Session
                cart.AddProductToCart(product, quantity);
            }

            return RedirectToAction("ShowCart", "ShoppingCart"); // Chuyển hướng đến giỏ hàng sau khi thêm sản phẩm
        }



        public ActionResult Update_Cart_Quantity(FormCollection form)
        {
            Cart cart = Session["Cart"] as Cart;
            int id_pro = int.Parse(Request.Form["idPro"]);
            int _quantity = int.Parse(Request.Form["carQuantity"]);
            cart.UpdateProductQuantity(id_pro, _quantity);

            return RedirectToAction("ShowCart", "ShoppingCart");

        }

        [HttpPost]
        public ActionResult RemoveCart(int id)
        {
            Cart cart = Session["Cart"] as Cart;
            cart.RemoveProductFromCart(id);

            return RedirectToAction("ShowCart", "ShoppingCart");

        }
    }
}