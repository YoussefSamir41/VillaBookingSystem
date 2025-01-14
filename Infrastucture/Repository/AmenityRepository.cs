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
    public class AmenityRepository : GenericRepository<Amenity>, IAmenityRepository
    {
        private readonly ApplicationDbcontext dbcontext;

        public AmenityRepository(ApplicationDbcontext dbcontext) : base(dbcontext)
        {
            this.dbcontext = dbcontext;
        }

        public void Save()
        {
            dbcontext.SaveChanges();
        }

        public void Update(Amenity entity)
        {
            dbcontext.Amenties.Update(entity);
        }

       
    }
}
