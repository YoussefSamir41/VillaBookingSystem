using Application.Common.Interfaces;
using Application.Common.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VillaBooking.Models;
using VillaBooking.ViewModels;

namespace VillaBooking.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }


        public IActionResult Index()
        {
            HomeVM homeVM = new HomeVM()
            {
                VillaList = unitOfWork.Villa.GetAll(includeProperties: "VillaAmenity"),
                Nights = 1,
                CheckInDate = DateOnly.FromDateTime(DateTime.Now),



            };
            return View(homeVM);
        }


        [HttpPost]
        public IActionResult Index(HomeVM homeVM)
        {
            homeVM.VillaList = unitOfWork.Villa.GetAll(includeProperties: "VillaAmenity");

            foreach (var villa in homeVM.VillaList)
            {
                if (villa.Id % 2 == 0)
                {
                    villa.IsAvailable = false;
                }

            }



            return View(homeVM);
        }

        public IActionResult GetVillaBydate(int nights, DateOnly checkindate)
        {
            var villalist = unitOfWork.Villa.GetAll(includeProperties: "VillaAmenity").ToList();
            var villaNumberList = unitOfWork.VillaNumber.GetAll().ToList();
            var bookecdVillas = unitOfWork.Booking.GetAll(u => u.Status == SD.StatusApproved
            || u.Status == SD.StatusCheckedIn).ToList();
            foreach (var villa in villalist)
            {
                int roomAvailabe = SD.VillaRoomsAvailable_Count
                     (villa.Id, villaNumberList, checkindate, nights, bookecdVillas);

                villa.IsAvailable = roomAvailabe > 0 ? true : false;


            }
            HomeVM homeVM = new()
            {
                CheckInDate = checkindate,
                VillaList = villalist,
                Nights = nights
            };
            return PartialView("VillaList", homeVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}