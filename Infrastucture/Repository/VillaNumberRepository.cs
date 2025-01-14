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
    public class VillaNumberRepository : GenericRepository<VillaNumber>, IVillaNumberRepository
    {

        private readonly ApplicationDbcontext dbcontext;

        public VillaNumberRepository(ApplicationDbcontext dbcontext) : base(dbcontext)
        {
            this.dbcontext = dbcontext;
        }
        public void Save()
        {
            dbcontext.SaveChanges();
        }

        public void Update(VillaNumber entity)
        {
            dbcontext.Update(entity);
        }

      
    }
}
