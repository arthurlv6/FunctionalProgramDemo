using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionalProgramDemo
{
    public class CouponDBContext: DbContext
    {
        public CouponDBContext(DbContextOptions<CouponDBContext> options) : base(options)
        {
            
        }
        public virtual DbSet<Coupon> Coupons { get; set; }
        public virtual DbSet<Redemption> Redemptions { get; set; }
        public virtual DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Redemption>()
                .HasIndex(r => new { r.UserId, r.CouponId });

            modelBuilder.Entity<User>()
                .HasIndex(u => new { u.Name });

            modelBuilder.Entity<Coupon>()
                .HasIndex(c => new { c.Title });
        }
    }
}
