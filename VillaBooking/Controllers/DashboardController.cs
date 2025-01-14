using Application.Common.Interfaces;
using Application.Common.Utility;
using Microsoft.AspNetCore.Mvc;
using VillaBooking.ViewModels;

namespace VillaBooking.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        static int previousMonth = DateTime.Now.Month == 1 ? 12 : DateTime.Now.Month - 1;
        readonly DateTime previousMonthStartDate = new(DateTime.Now.Year, previousMonth, 1);
        readonly DateTime currentMonthStartDate = new(DateTime.Now.Year, DateTime.Now.Month, 1);

        public DashboardController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetTotalBookingRadialChartData()
        {




            // Fetch all bookings with status other than Pending and Cancelled
            var totalBookings = unitOfWork.Booking.GetAll(u => u.Status != SD.StatusPending
                                                             && u.Status != SD.StatusCancelled);

            // Count bookings for the current month
            var countByCurrentMonth = totalBookings.Count(u => u.BookingDate >= currentMonthStartDate
                                                             && u.BookingDate <= DateTime.Now);

            // Count bookings for the previous month
            var countByPreviousMonth = totalBookings.Count(u => u.BookingDate >= previousMonthStartDate
                                                              && u.BookingDate < currentMonthStartDate);

            RedirecrtBarchartVM redirecrtBarchartVM = new();

            int increaseDeacreaseRatio = 100;
            if (countByPreviousMonth != 0)
            {
                increaseDeacreaseRatio = Convert.ToInt32(
                    (countByCurrentMonth - countByPreviousMonth) / countByPreviousMonth * 100);
            }
            redirecrtBarchartVM.TotalCount = totalBookings.Count();
            redirecrtBarchartVM.IncreasDeacreasAmount = countByCurrentMonth;
            redirecrtBarchartVM.HasRatioIncrease = currentMonthStartDate > previousMonthStartDate;
            redirecrtBarchartVM.Series = new int[] { increaseDeacreaseRatio };


            return Json(redirecrtBarchartVM);
        }



        public async Task<IActionResult> GetRegisteredUserChartData()
        {
            var totalUsers = unitOfWork.Application.GetAll();
            var countByCurrentMonth = totalUsers.Count(u => u.CreatedAt >= currentMonthStartDate
                                                             && u.CreatedAt <= DateTime.Now);
            var countByPreviousMonth = totalUsers.Count(u => u.CreatedAt >= previousMonthStartDate
                                                              && u.CreatedAt < currentMonthStartDate);
            RedirecrtBarchartVM redirecrtBarchartVM = new();

            int increaseDeacreaseRatio = 100;
            if (countByPreviousMonth != 0)
            {
                increaseDeacreaseRatio = Convert.ToInt32(
                    (countByCurrentMonth - countByPreviousMonth) / countByPreviousMonth * 100);
            }
            redirecrtBarchartVM.TotalCount = totalUsers.Count();
            redirecrtBarchartVM.IncreasDeacreasAmount = countByCurrentMonth;
            redirecrtBarchartVM.HasRatioIncrease = currentMonthStartDate > previousMonthStartDate;
            redirecrtBarchartVM.Series = new int[] { increaseDeacreaseRatio };
            return Json(redirecrtBarchartVM);
        }


        public async Task<IActionResult> GetRevenueChartData()
        {
            var currentMonthStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var previousMonthStartDate = currentMonthStartDate.AddMonths(-1);

            var totalBookings = unitOfWork.Booking.GetAll(u => u.Status != SD.StatusPending && u.Status != SD.StatusCancelled);

            if (totalBookings == null || !totalBookings.Any())
            {
                return Json(new { error = "No bookings found." });
            }

            var totalRevenue = totalBookings.Sum(u => u.TotalCost);

            var countByCurrentMonth = totalBookings
                .Where(u => u.BookingDate >= currentMonthStartDate && u.BookingDate <= DateTime.Now)
                .Sum(u => u.TotalCost);

            var countByPreviousMonth = totalBookings
                .Where(u => u.BookingDate >= previousMonthStartDate && u.BookingDate < currentMonthStartDate)
                .Sum(u => u.TotalCost);

            int increaseDeacreaseRatio = 0;
            if (countByPreviousMonth != 0)
            {
                increaseDeacreaseRatio = Convert.ToInt32(
                    ((countByCurrentMonth - countByPreviousMonth) / countByPreviousMonth) * 100);
            }

            // Assuming you fetch these values from your data source
            totalRevenue = 5000;
            var hasRatioIncrease = true; // Change this based on your logic
            var currentRevenue = 2360; // Change this based on your logic
            var series = new int[] { 65 }; // This could represent your chart data

            // Return the JSON object with the expected structure
            return Json(new
            {
                totalCount = totalRevenue,
                HasRatioIncrease = hasRatioIncrease,
                Series = series,
                CurrentRevenue = currentRevenue // Add this if required in the front-end logic
            });
        }

        public async Task<IActionResult> GetBookingPieChartData()
        {
            var totalBookings = unitOfWork.Booking.GetAll(
                 u => u.BookingDate >= DateTime.Now.AddDays(-30)
                 && u.Status != SD.StatusPending
                 && u.Status != SD.StatusCancelled
            );

            var CustomerWithoneBooking = totalBookings.GroupBy(b => b.UserId).Where(x => x.Count() == 1).Select(x => x.Key).ToList();

            int bookingsByNewCustomer = CustomerWithoneBooking.Count();
            int bookingByReturnedCustomer = totalBookings.Count() - bookingsByNewCustomer;

            PieChartVM pieChartVM = new PieChartVM()
            {
                labels = new string[] { "New Customer Bookings", "Returning Customer Bookings" },
                series = new int[] { 25, 23 }
            };

            return Json(pieChartVM);  // Ensure this is the correct format
        }



        public async Task<IActionResult> GetMemberAndBookingLineChartData()
        {
            var bookingData = unitOfWork.Booking.GetAll(u => u.BookingDate >= DateTime.Now.AddDays(-30)
               && u.BookingDate.Date <= DateTime.Now).GroupBy(b => b.BookingDate.Date)
               .Select(u => new
               {
                   DateTime = u.Key,
                   NewBookingCount = u.Count(),
               });


            var customerData = unitOfWork.Application.GetAll(u => u.CreatedAt >= DateTime.Now.AddDays(-30)
              && u.CreatedAt.Date <= DateTime.Now).GroupBy(b => b.CreatedAt.Date)
              .Select(u => new
              {
                  DateTime = u.Key,
                  NewCustomerCount = u.Count(),
              });


            var leftjoin = bookingData.GroupJoin(customerData, booking => booking.DateTime, customer
                => customer.DateTime, (booking, customer) => new
                {
                    booking.DateTime,
                    booking.NewBookingCount,
                    NewCustomerCount = customer.Select(x => x.NewCustomerCount).FirstOrDefault()
                });



            var rightjoin = customerData.GroupJoin(bookingData, customer => customer.DateTime, customer
                => customer.DateTime, (customer, booking) => new
                {
                    customer.DateTime,
                    NewBookingCount = booking.Select(x => x.NewBookingCount).FirstOrDefault(),
                    customer.NewCustomerCount
                });

            var mergeData = leftjoin.Union(rightjoin).OrderBy(x => x.DateTime).ToList();


            var newBookingData = mergeData.Select(x => x.NewBookingCount).ToArray();
            var newCustomerData = mergeData.Select(x => x.NewCustomerCount).ToArray();
            var categories = mergeData.Select(x => x.DateTime.ToString("MM/dd/yyyy")).ToArray();



            List<ChartData> chartDataList = new()
      {
         new ChartData
         {
             Name = "New Bookings" ,
             Data = newBookingData
         } ,
         new ChartData
         {
             Name = "New Members" ,
             Data= newCustomerData

         } ,
      };

            LineChartVM lineChartVM = new LineChartVM()
            {
                Categories = categories,
                Series = chartDataList

            };


            return Json(lineChartVM);
        }








    }
}
