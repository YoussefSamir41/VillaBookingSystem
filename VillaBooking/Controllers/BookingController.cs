using Application.Common.Interfaces;
using Application.Common.Utility;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace VillaBooking.Controllers
{
    [Route("booking")]
    public class BookingController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public BookingController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: /booking/index
        [HttpGet("index")]
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        // GET: /booking/finalize
        [HttpGet("finalize")]
        [Authorize]
        public IActionResult FinalizeBooking(int villaid, DateTime checkindate, int nights)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ApplicationUser user = unitOfWork.Application.Get(u => u.Id == userId);

            var v = unitOfWork.Villa.Get(u => u.Id == villaid, includeProperties: "VillaAmenity");
            Booking booking = new Booking()
            {
                Id = villaid,
                Villa = v,
                CheckInDate = checkindate,
                Nights = nights,
                CheckOutDate = checkindate.AddDays(nights),
                UserId = userId,
                Phone = user.PhoneNumber,
                Email = user.Email,
                Name = user.Name
            };
            booking.TotalCost = (double)(booking.Villa.Price * nights);
            return View(booking);
        }

        // POST: /booking/finalize
        [HttpPost("finalize")]
        [Authorize]
        public IActionResult FinalizeBooking(Booking booking)
        {
            // Get villa details and calculate total cost
            var villa = unitOfWork.Villa.Get(u => u.Id == booking.VillaId);
            booking.TotalCost = (double)villa.Price * booking.Nights;
            booking.Status = SD.StatusPending;
            booking.BookingDate = DateTime.Now;

            // Add the booking to the database
            unitOfWork.Booking.Add(booking);
            unitOfWork.Booking.Save();

            var domain = Request.Scheme + "://" + Request.Host.Value + "/";

            // Create Stripe session options
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{domain}booking/BookingConfirmation?bookingId={Uri.EscapeDataString(booking.Id.ToString())}",
                CancelUrl = $"{domain}booking/FinalizeBooking?villaid={Uri.EscapeDataString(booking.VillaId.ToString())}&checkindate={Uri.EscapeDataString(booking.CheckInDate.ToString("yyyy-MM-dd"))}&nights={Uri.EscapeDataString(booking.Nights.ToString())}",
            };

            // Add the line item for Stripe checkout
            options.LineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(booking.TotalCost * 100), // Stripe expects the amount in cents
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = villa.Name,
                        // Optionally add an image for the product
                        // Images = new List<string> { domain + villa.ImageUrl }
                    },
                },
                Quantity = 1
            });

            // Create Stripe session
            var sessionService = new Stripe.Checkout.SessionService();
            Stripe.Checkout.Session session = sessionService.Create(options);

            // Redirect user to Stripe Checkout page
            return Redirect(session.Url);
        }


        [HttpGet("BookingConfirmation")]
        [Authorize]
        public IActionResult BookingConfirmation(int bookingId)
        {
            // Get the booking from the database with associated User and Villa details
            Booking bookingFromDb = unitOfWork.Booking.Get(
                u => u.Id == bookingId,
                includeProperties: "User,Villa");

            if (bookingFromDb != null && bookingFromDb.Status == SD.StatusPending)
            {
                var service = new Stripe.Checkout.SessionService();

                // Make sure StripeSessionId is available in the bookingFromDb object
                if (!string.IsNullOrEmpty(bookingFromDb.StripeSessionId))
                {
                    try
                    {
                        // Retrieve the Stripe session using the stored session ID
                        Stripe.Checkout.Session session = service.Get(bookingFromDb.StripeSessionId);

                        // Check the payment status
                        if (session.PaymentStatus == "paid") // Ensure the payment status is correct
                        {
                            // Update the booking status and payment details in the database
                            unitOfWork.Booking.UpdateStatues(bookingFromDb.Id, SD.StatusApproved);
                            unitOfWork.Booking.UpdateStripPaymentId(bookingFromDb.Id, session.Id, session.PaymentIntentId);
                            unitOfWork.Booking.Save();
                        }
                    }
                    catch (StripeException ex)
                    {
                        // Handle Stripe exception
                        Console.WriteLine($"Error retrieving Stripe session: {ex.Message}");
                    }
                }
            }

            // Return the booking confirmation view with the booking details
            return View(bookingFromDb); // Passing the whole booking object to the view
        }

        #region Api Calls
        [HttpGet("getall")]
        [Authorize]
        public IActionResult GetAll()
        {
            // Get the current user's ID (handle null case)
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Get the status from the query parameter if provided
            var status = Request.Query["status"].ToString();

            // Ensure bookings query works with or without the userId and status
            IEnumerable<Booking> bookings;

            if (User.IsInRole(SD.RoleAdmin))
            {
                if (string.IsNullOrEmpty(status))
                {
                    bookings = unitOfWork.Booking.GetAll(includeProperties: "User,Villa");
                }
                else
                {
                    bookings = unitOfWork.Booking.GetAll(b => b.Status == status, includeProperties: "User,Villa");
                }
            }
            else
            {
                if (string.IsNullOrEmpty(status))
                {
                    bookings = unitOfWork.Booking.GetAll(u => u.UserId == userId, includeProperties: "User,Villa");
                }
                else
                {
                    bookings = unitOfWork.Booking.GetAll(
                        u => u.UserId == userId && u.Status == status,
                        includeProperties: "User,Villa"
                    );
                }
            }

            // Project data ensuring null checks for User properties
            var result = bookings.Select(b => new
            {
                b.Id,
                Name = b.User?.Name ?? "N/A",
                Phone = b.User?.PhoneNumber ?? "N/A", // Ensure this field is named 'Phone'
                Email = b.User?.Email ?? "N/A",
                b.Status,
                b.CheckInDate,
                b.Nights,
                b.TotalCost
            });

            return Json(new { data = result });
        }


        [HttpGet("BookingDetails")]
        [Authorize]
        public IActionResult BookingDetails(int bookingId)
        {
            var bookings = unitOfWork.Booking.Get(u => u.Id == bookingId, includeProperties: "User,Villa");
            return View(bookings);
        }





        #endregion
    }

}
