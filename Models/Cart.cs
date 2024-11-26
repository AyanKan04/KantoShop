using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KantoShop.Models
{
    // Mô tả một mục sản phẩm trong giỏ hàng
    public class CartItem
    {
        // Sản phẩm và số lượng của sản phẩm trong giỏ hàng
        public Product _product { get; set; }
        public int _quantity { get; set; }
    }

    // Mô tả giỏ hàng gồm các CartItem
    public class Cart
    {
        // Danh sách các sản phẩm trong giỏ hàng
        private List<CartItem> items = new List<CartItem>();

        // Thuộc tính để lấy danh sách các CartItem
        public IEnumerable<CartItem> Items
        {
            get { return items; }
        }

        // Phương thức thêm sản phẩm vào giỏ hàng
        public void AddProductToCart(Product product, int quantity = 1)
        {
            if (product == null || quantity <= 0)
                return; // Không thêm sản phẩm nếu sản phẩm không hợp lệ hoặc số lượng <= 0

            // Kiểm tra xem sản phẩm đã có trong giỏ chưa
            var item = items.FirstOrDefault(s => s._product.ProductID == product.ProductID);

            if (item == null)
            {
                // Nếu sản phẩm chưa có trong giỏ, thêm mới
                items.Add(new CartItem { _product = product, _quantity = quantity });
            }
            else
            {
                // Nếu sản phẩm đã có, cộng thêm số lượng
                item._quantity += quantity;
            }
        }

        // Phương thức tính tổng số lượng sản phẩm trong giỏ hàng
        public int GetTotalQuantity()
        {
            return items.Sum(s => s._quantity );
        }

        // Phương thức tính tổng số tiền của giỏ hàng
        public decimal GetTotalMoney()
        {
            var total = items.Sum(s => s._quantity * s._product.Price);
            // Tính tổng tiền của giỏ hàng
            return items.Sum(s => s._quantity * s._product.Price);
        }

        // Phương thức cập nhật lại số lượng sản phẩm trong giỏ hàng
        public void UpdateProductQuantity(int productId, int newQuantity)
        {
            var item = items.Find(s => s._product.ProductID == productId);
            if (item != null)
                item._quantity = newQuantity;

        }

        // Phương thức xóa sản phẩm khỏi giỏ hàng
        public void RemoveProductFromCart(int productId)
        {
            items.RemoveAll(s => s._product.ProductID == productId);
        }

        // Phương thức xóa tất cả sản phẩm trong giỏ hàng (sau khi thanh toán)
        public void ClearCart()
        {
            items.Clear();
        }
    }

}