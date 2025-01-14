using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IBookingRepository : IGenericRepository<Booking>
    {
        void Update(Booking booking);
        void Save();
        void UpdateStatues(int bookingId , string orderStatus);
        void UpdateStripPaymentId(int bookingId , string seassionId , string paymentIntendId);
    }
}
