using Microsoft.AspNetCore.Mvc;
using DNUrideshare.Controllers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace DNUrideshare.Controllers
{
    public class CustomerController : Controller
    {
        private static List<Booking> _bookings = new List<Booking>(); // Danh sách tạm để lưu booking

        public IActionResult Index() => View();
        public IActionResult EditAccount() { return View(); }
        public IActionResult AccountInfo() { return View(); }
        public IActionResult Trips()
        {
            
            {
                var trips = DriverController._trips;
                return View(trips);
            }
        }
        public IActionResult ManageTrips()
        {
            var bookings = _bookings;
            return View(bookings);
        }
        public IActionResult ConfirmBooking(int tripId)
        {
            Console.WriteLine($"ConfirmBooking called with tripId: {tripId}");
            var trip = DriverController._trips.FirstOrDefault(t => t.GetHashCode() == tripId); // Giả sử dùng hashcode làm ID tạm
            if (trip == null)
            {
                Console.WriteLine("Trip not found for tripId: " + tripId);
                return RedirectToAction("Trips", new { error = "Chuyến đi không tồn tại." });
            }
            return View(trip); // Truyền trực tiếp trip làm model
        }

        [HttpPost]
        public IActionResult UpdateAccount(string Name, string Email, string Phone)
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

            TempData["SuccessMessage"] = "Thông tin đã được cập nhật thành công!";
            return RedirectToAction("AccountInfo");
        }
        [HttpPost]
        public IActionResult ConfirmBooking(int tripId, string action)
        {
            Console.WriteLine($"ConfirmBooking POST called with tripId: {tripId}, action: {action}");
            var trip = DriverController._trips.FirstOrDefault(t => t.GetHashCode() == tripId);
            if (trip == null)
            {
                Console.WriteLine("Trip not found for tripId: " + tripId);
                return RedirectToAction("Trips", new { error = "Chuyến đi không tồn tại." });
            }

            if (action == "confirm")
            {
                if (trip.AvailableSeats <= 0)
                {
                    Console.WriteLine("No available seats for tripId: " + tripId);
                    return RedirectToAction("Trips", new { error = "Không còn chỗ trống cho chuyến đi này." });
                }

                var booking = new Booking
                {
                    TripId = trip.GetHashCode(),
                    Trip = trip,
                    BookingTime = DateTime.Now,
                    CustomerId = "customer1" // Giả định ID khách
                };
                _bookings.Add(booking);

                // Giảm số chỗ trống
                trip.AvailableSeats -= 1;

                // Thông báo cho tài xế (giả lập)
                NotifyDriver(trip.DriverId, $"Khách hàng đã đặt chuyến: {trip.Origin} -> {trip.Destination}");

                Console.WriteLine("Booking successful for tripId: " + tripId);
                return RedirectToAction("ManageTrips");
            }
            Console.WriteLine("Booking canceled for tripId: " + tripId);
            return RedirectToAction("Trips");
        }

        [HttpPost]
        public IActionResult CancelBooking(int bookingId)
        {
            Console.WriteLine($"CancelBooking called with bookingId: {bookingId}");
            var booking = _bookings.FirstOrDefault(b => b.GetHashCode() == bookingId);
            if (booking == null)
            {
                Console.WriteLine("Booking not found for bookingId: " + bookingId);
                return NotFound();
            }

            var timeDiff = (booking.Trip.DepartureTime - DateTime.Now).TotalHours;
            if (timeDiff < 24)
            {
                Console.WriteLine("Cancellation not allowed, less than 24 hours remaining");
                ViewBag.Error = "Không thể hủy chuyến vì còn dưới 24 giờ trước giờ khởi hành.";
                return View("ManageTrips", _bookings);
            }

            _bookings.Remove(booking);
            booking.Trip.AvailableSeats += 1; // Trả lại chỗ trống
            NotifyDriver(booking.Trip.DriverId, $"Chuyến đi {booking.Trip.Origin} -> {booking.Trip.Destination} đã bị hủy bởi khách.");
            Console.WriteLine("Booking canceled successfully for bookingId: " + bookingId);
            return RedirectToAction("ManageTrips");
        }

        private void NotifyDriver(string driverId, string message)
        {
            // Giả lập thông báo (thay bằng email, notification hệ thống trong thực tế)
            Console.WriteLine($"Thông báo cho tài xế {driverId}: {message}");
        }
    }

    public class Booking
    {
        public int TripId { get; set; }
        public Trip Trip { get; set; }
        public DateTime BookingTime { get; set; }
        public string CustomerId { get; set; }
    }
}