using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FunctionalProgramDemo
{
    public static class DbInitializer
    {
        public static async System.Threading.Tasks.Task InitializeAsync(CouponDBContext context)
        {
            context.Database.EnsureCreated();

            if (!context.Coupons.Any())
            {
                List<Coupon> coupons = new List<Coupon>();
                for (int i = 0; i < 50000; i++)
                {
                    coupons.Add(new Coupon { Title = $"Coupon {i}", MaxPerUser=10, MaxAllUser= 20000, StartDate=DateTime.UtcNow, EndDate=DateTime.MaxValue });
                }
                await context.Coupons.AddRangeAsync(coupons);
            }
            if (!context.Users.Any())
            {
                context.Users.Add(new User { Name = "Arthur" });
                context.Users.Add(new User { Name = "Jason" });
                context.Users.Add(new User { Name = "Selina" });
            }
               await context.SaveChangesAsync();
        }
    }
}
