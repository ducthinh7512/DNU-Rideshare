using DNUrideshare.Models;
using Microsoft.AspNetCore.Mvc;

namespace DNUrideshare.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                // Logic xác thực đơn giản (có thể thay bằng kiểm tra database)
                if (model.UserType == "Customer" && model.Username == "customer" && model.Password == "123456")
                {
                    return RedirectToAction("CustomerDashboard");
                }
                else if (model.UserType == "Admin" && model.Username == "admin" && model.Password == "admin123")
                {
                    return RedirectToAction("AdminDashboard");
                }
                else if (model.UserType == "Driver" && model.Username == "driver" && model.Password == "driver123")
                {
                    return RedirectToAction("DriverDashboard");
                }
                else
                {
                    ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                }
            }
            return View(model);
        }
        public IActionResult Logout()
        {
            return RedirectToAction("Login"); // Chuyển hướng về trang đăng nhập
        }
        public IActionResult CustomerDashboard()
        {
            return View();
        }

        public IActionResult AdminDashboard()
        {
            return View();
        }

        public IActionResult DriverDashboard()
        {
            return View();
        }
    }
}