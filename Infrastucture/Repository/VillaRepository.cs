using Application.Common.Interfaces;
using Domain.Entities;
using Infrastucture.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastucture.Repository
{
    public class VillaRepository : GenericRepository<Villa>, IVillaRepository
    {
        private readonly ApplicationDbcontext dbcontext;

        public VillaRepository(ApplicationDbcontext dbcontext) :base(dbcontext)
        {
            this.dbcontext = dbcontext;
        }
        
        public void Save()
        {
            dbcontext.SaveChanges();
        }

        public void Update(Villa entity)
        {
            dbcontext.Update(entity);
        }
    }
}
