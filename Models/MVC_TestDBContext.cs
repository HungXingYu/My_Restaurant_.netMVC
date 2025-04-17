using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace My_Restaurant.Models
{
    public partial class MVC_TestDBContext : DbContext
    {
        public MVC_TestDBContext()
            : base("name=MVC_TestDB")
        {
        }

        public virtual DbSet<Restaurant> Restaurants { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
