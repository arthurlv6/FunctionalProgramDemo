using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FunctionalProgramDemo
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public T ToModel<T>(IMapper mapper) where T : class
        {
            return mapper.Map<T>(this);
        }
    }
    public class Coupon : BaseEntity
    {
        [MaxLength(200)]
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int MaxPerUser { get; set; }
        public int MaxAllUser { get; set; }
        public IList<Redemption> Redemptions { get; set; }

    }
    public class Redemption : BaseEntity
    {
        public int UserId { get; set; }
        public int CouponId { get; set; }
        public User User { get; set; }
        public Coupon Coupon { get; set; }
        public DateTime OperationDate { get; set; }
    }
    public class User : BaseEntity
    {
        [MaxLength(200)]
        public string Name { get; set; }
        public IList<Redemption> Redemptions { get; set; }
    }
}
