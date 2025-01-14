using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Utility
{
    public static class SD
    {
        public const string RoleCustomer = "Customer";
        public const string RoleAdmin = "Admin";
        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusCheckedIn = "CheckedIn";
        public const string StatusCompleted = "Completed";
        public const string StatusCancelled = "Cancelled";
        public const string StatusRefunded = "Refunded";


        public static int VillaRoomsAvailable_Count(int villaId,
                      List<VillaNumber> villaNumberList, DateOnly checkInDate, int nights,
                      List<Booking> bookings)
        {
            List<int> bookingInDate = new();
            int finalAvailableRoomForAllNights = int.MaxValue;
            var roomsInVilla = villaNumberList.Where(x => x.VillaId == villaId).Count();

            for (int i = 0; i < nights; i++)
            {
                // Convert checkInDate (DateOnly) to DateTime for comparison
                var checkInDateTime = checkInDate.ToDateTime(new TimeOnly(0, 0));

                var villasBooked = bookings.Where(u => u.CheckInDate <= checkInDateTime.AddDays(i)
                    && u.CheckOutDate > checkInDateTime.AddDays(i) && u.VillaId == villaId);

                foreach (var booking in villasBooked)
                {
                    if (!bookingInDate.Contains(booking.Id))
                    {
                        bookingInDate.Add(booking.Id);
                    }
                }

                var totalAvailableRooms = roomsInVilla - bookingInDate.Count;
                if (totalAvailableRooms == 0)
                {
                    return 0;
                }
                else
                {
                    if (finalAvailableRoomForAllNights > totalAvailableRooms)
                    {
                        finalAvailableRoomForAllNights = totalAvailableRooms;
                    }
                }
            }

            return finalAvailableRoomForAllNights;
        }



    }
}
