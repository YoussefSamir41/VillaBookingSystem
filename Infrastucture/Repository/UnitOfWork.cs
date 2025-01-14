using Application.Common.Interfaces;
using Domain.Entities;
using Infrastucture.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastucture.Repository
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly ApplicationDbcontext dbcontext; 
       public IVillaRepository Villa { get; private set; }
        public IVillaNumberRepository VillaNumber { get ; private set ; }
        public IAmenityRepository Amenity { get; private set ; }

        public IBookingRepository Booking { get; private set; }

        public IApplicationUserRepository Application { get; private set; }

        public UnitOfWork(ApplicationDbcontext dbcontext)
        {
            this.dbcontext = dbcontext;
            Villa = new VillaRepository(dbcontext);
            VillaNumber = new VillaNumberRepository(dbcontext);
            Amenity = new AmenityRepository(dbcontext);
            Booking = new BookingRepository(dbcontext);
            Application = new ApplicationUserRepository(dbcontext);
        }

      
    }
}
