using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using KantoShop.Models;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Security.Cryptography;
using System.Text;

namespace KantoShop.Controllers
{
    public class AccountController : Controller
    {
        // Trang đăng nhập (GET)
        public ActionResult Index()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login");
            }

            int userId = (int)Session["UserId"];

            using (var dbContext = new ShopKantoContext())
            {
                var user = dbContext.AdminUsers
                    .Where(u => u.ID == userId)
                    .Select(u => new Account
                    {
                        Username = u.UserName,
                        FullName = u.FullName,
                        Email = u.Email,
                        PhoneNumber = u.PhoneNumber,
                        Address = u.Address,
                        Avatar = u.Avatar,
                        BackgroundImage = u.BackgroundImage
                    })
                    .FirstOrDefault();

                return View(user);
            }
        }

        [HttpPost]
        public ActionResult Index(Account model)
        {
            if (ModelState.IsValid)
            {
                int userId = (int)Session["UserId"];

                using (var dbContext = new ShopKantoContext())
                {
                    var user = dbContext.AdminUsers.FirstOrDefault(u => u.ID == userId);
                    if (user != null)
                    {
                        user.FullName = model.FullName;
                        user.Email = model.Email;
                        user.PhoneNumber = model.PhoneNumber;
                        user.Address = model.Address;
                        user.Avatar = model.Avatar;
                        user.BackgroundImage = model.BackgroundImage;

                        dbContext.SaveChanges();
                    }
                }

                // Sau khi lưu, chuyển hướng lại trang index
                return RedirectToAction("Index");
            }

            return View(model);
        }


        // Trang đăng nhập (GET)
        public ActionResult Login()
        {
            // Kiểm tra nếu người dùng đã đăng nhập, chuyển hướng đến trang chủ
            if (Session["UserId"] != null)
            {
                return RedirectToAction("ProductList", "Products");
            }

            return View();
        }

        // Xử lý đăng nhập (POST)
        [HttpPost]
        public async Task<ActionResult> Login(Account model)
        {
            if (ModelState.IsValid)
            {
                using (var dbContext = new ShopKantoContext())
                {
                    var user = dbContext.AdminUsers
                        .FirstOrDefault(u => u.UserName == model.Username);

                    if (user != null && VerifyPassword(model.Password, user.PasswordUser))
                    {
                        // Đăng nhập thành công, lưu thông tin người dùng vào session
                        Session["UserId"] = user.ID;
                        Session["Username"] = user.UserName;

                        // Thiết lập tên người dùng cho User.Identity (nếu bạn cần)
                        FormsAuthentication.SetAuthCookie(user.UserName, false); // Nếu dùng FormsAuthentication

                        // Chuyển hướng đến trang sản phẩm (hoặc trang yêu cầu ban đầu)
                        return RedirectToAction("ProductList", "Products");
                    }
                }

                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
            }

            return View(model);
        }

        // Hàm băm mật khẩu
        private bool VerifyPassword(string enteredPassword, string storedPassword)
        {
            // Băm mật khẩu người dùng nhập và so sánh với mật khẩu đã lưu
            var enteredHash = HashPassword(enteredPassword);
            return enteredHash == storedPassword;
        }

        // Trang đăng ký (GET)
        public ActionResult Register()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Register(Account model)
        {
            // Kiểm tra mật khẩu và xác nhận mật khẩu
            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Mật khẩu xác nhận không khớp.");
                return View(model);
            }

            using (var dbContext = new ShopKantoContext())
            {
                // Kiểm tra tên đăng nhập đã tồn tại chưa
                var existingUser = dbContext.AdminUsers
                    .FirstOrDefault(u => u.UserName == model.Username);

                if (existingUser != null)
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại.");
                    return View(model);
                }

                // Mã hóa mật khẩu trước khi lưu vào cơ sở dữ liệu
                var hashedPassword = HashPassword(model.Password);

                // Tạo đối tượng AdminUser mới
                var newUser = new AdminUser
                {
                    UserName = model.Username,
                    FullName = model.FullName,
                    Email = model.Email,
                    PasswordUser = hashedPassword, // Lưu mật khẩu đã băm
                    Status = "Active", // Trạng thái mặc định
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    Avatar = model.Avatar,
                    BackgroundImage = model.BackgroundImage
                };

                // Thêm người dùng vào cơ sở dữ liệu và lưu thay đổi
                dbContext.AdminUsers.Add(newUser);

                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    // Hiển thị lỗi nếu có
                    ModelState.AddModelError("", "Có lỗi khi lưu thông tin người dùng: " + ex.Message);
                    return View(model);
                }
            }

            // Sau khi đăng ký thành công, chuyển hướng tới trang đăng nhập
            return RedirectToAction("Login");
        }



        // Hàm băm mật khẩu
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashBytes);
            }
        }



        // Đăng xuất
        public ActionResult Logout()
        {
            // Xóa thông tin đăng nhập khỏi Session
            Session.Clear();
            FormsAuthentication.SignOut(); // Nếu bạn sử dụng Forms Authentication

            // Chuyển hướng về trang đăng nhập
            return RedirectToAction("Login", "Account");
        }
    }
}
