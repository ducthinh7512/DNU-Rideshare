using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace DNUrideshare.Controllers
{
    public class DriverController : Controller
    {
        public static List<Trip> _trips = new List<Trip>(); // Danh sách tạm để lưu chuyến đi
        public IActionResult AccountInfo() { return View(); }
        public IActionResult EditAccount() { return View(); }
        public IActionResult Index() => View();
        public IActionResult Trips()
        {
            var trips = _trips; // Lấy danh sách chuyến đi
            return View(trips); // Sử dụng view của tài xế
        }
        public IActionResult ManageTrips() => View(_trips);
        public IActionResult PostTrip() => View();
        [HttpPost]
        public IActionResult UpdateAccount(string Name, string Email, string Phone, string VehiclePlate, string VehicleType)
        {
            if (string.IsNullOrWhiteSpace(Name) || Name.Length > 50)
            {
                ViewBag.Error = "Tên không được để trống và không vượt quá 50 ký tự.";
                return View("EditAccount");
            }

            if (string.IsNullOrWhiteSpace(Email) || !new EmailAddressAttribute().IsValid(Email))
            {
                ViewBag.Error = "Email không hợp lệ.";
                return View("EditAccount");
            }

            if (string.IsNullOrWhiteSpace(Phone) || !Regex.IsMatch(Phone, @"^[0-9]{10}$"))
            {
                ViewBag.Error = "Số điện thoại phải là 10 chữ số.";
                return View("EditAccount");
            }

            if (string.IsNullOrWhiteSpace(VehiclePlate) || VehiclePlate.Length > 10)
            {
                ViewBag.Error = "Biển số xe không hợp lệ hoặc vượt quá 10 ký tự.";
                return View("EditAccount");
            }

            if (string.IsNullOrWhiteSpace(VehicleType) || VehicleType.Length > 50)
            {
                ViewBag.Error = "Loại xe không hợp lệ hoặc vượt quá 50 ký tự.";
                return View("EditAccount");
            }

            TempData["SuccessMessage"] = "Thông tin đã được cập nhật thành công!";
            return RedirectToAction("AccountInfo");
        }
        [HttpPost]
        public IActionResult PostTrip(string origin, string customOrigin, string destination, string customDestination, 
            string departureTime, int availableSeats, int pricePerSeat, string vehicle, string routeDescription, 
            string notes, bool instantBooking)
        {
            string finalOrigin = string.IsNullOrEmpty(customOrigin) ? origin : customOrigin;
            string finalDestination = string.IsNullOrEmpty(customDestination) ? destination : customDestination;

            var trip = new Trip
            {
                Origin = finalOrigin,
                Destination = finalDestination,
                DepartureTime = DateTime.Parse(departureTime),
                AvailableSeats = availableSeats,
                PricePerSeat = pricePerSeat,
                Vehicle = vehicle,
                RouteDescription = routeDescription,
                Notes = notes,
                InstantBooking = instantBooking,
                DriverId = "driver1"
            };

            _trips.Add(trip);

            ViewBag.Message = $"Đăng chuyến đi thành công: {finalOrigin} -> {finalDestination} vào {departureTime}";
            ViewBag.Origin = finalOrigin;
            ViewBag.Destination = finalDestination;
            ViewBag.DepartureTime = DateTime.Parse(departureTime).ToString("dd/MM/yyyy HH:mm");
            ViewBag.PricePerSeat = pricePerSeat;
            return View("PostTripSuccess");
        }
    }

    public class Trip
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime DepartureTime { get; set; }
        public int AvailableSeats { get; set; }
        public int PricePerSeat { get; set; }
        public string Vehicle { get; set; }
        public string RouteDescription { get; set; }
        public string Notes { get; set; }
        public bool InstantBooking { get; set; }
        public string DriverId { get; set; }
    }
}