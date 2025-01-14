using Application.Common.Interfaces;
using Application.Common.Utility;
using Domain.Entities;
using Infrastucture.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastucture.Repository
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        private readonly ApplicationDbcontext dbcontext;

        public BookingRepository(ApplicationDbcontext dbcontext) : base(dbcontext)
        {
            this.dbcontext = dbcontext;
        }

        public void Save()
        {
            dbcontext.SaveChanges();
        }

        public void Update(Booking entity)
        {
            dbcontext.Update(entity);
        }

        public void UpdateStatues(int bookingId, string bookingstatus)
        {
            var bookingFromDb = dbcontext.Bookings.FirstOrDefault(m => m.Id == bookingId); 
            if (bookingFromDb != null)
            {
                bookingFromDb.Status = bookingstatus;
                if (bookingstatus == SD.StatusCheckedIn)
                {
                    bookingFromDb.ActualCheckInDate = DateTime.Now;
                }
                if(bookingstatus==SD.StatusCompleted)
                    
                {
                    bookingFromDb.ActualCheckInDate= DateTime.Now;
                }
            }
        }

        public void UpdateStripPaymentId(int bookingId, string seassionId, string paymentIntendId)
        {
            var bookingFromDb = dbcontext.Bookings.FirstOrDefault(m => m.Id == bookingId);
            if (bookingFromDb != null)
            {
                if (string.IsNullOrEmpty(seassionId))
                {
                    bookingFromDb.StripeSessionId = seassionId;
                }
                if (string.IsNullOrEmpty(paymentIntendId))
                {
                    bookingFromDb.StripePaymentIntentId = paymentIntendId;
                    bookingFromDb.PaymentDate=DateTime.Now;
                    bookingFromDb.IsPaymentSuccessful = true;
                }
            }
        }
    }
}
