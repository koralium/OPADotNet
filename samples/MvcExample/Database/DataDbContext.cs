using Microsoft.EntityFrameworkCore;
using MvcExample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcExample.Database
{
    public class DataDbContext : DbContext
    {
        public DbSet<DataModel> Data { get; set; }

        public DataDbContext(DbContextOptions<DataDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }
    }
}
