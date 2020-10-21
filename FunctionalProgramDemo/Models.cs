using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FunctionalProgramDemo
{
    public abstract class BaseModel
    {
        [Key]
        public int Id { get; set; }
    }
    public class CouponModel : BaseModel
    {
        [MaxLength(200)]
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int MaxPerUser { get; set; }
        public int MaxAllUser { get; set; }

    }
    public class RedemptionModel : BaseModel
    {
        public int UserId { get; set; }
        public int CouponId { get; set; }
        public string UserName { get; set; }
        public string CouponTitle { get; set; }
        public DateTime OperationDate { get; set; }
    }
    public class UserModel : BaseModel
    {
        [MaxLength(200)]
        public string Name { get; set; }
        public IList<Redemption> Redemptions { get; set; }
    }
    public class PaginationModel
    {
        public int Page { get; set; } = 1;
        public int QuantityPerPage { get; set; } = 10;
    }
    public class RedeemCouponModel
    {
        public string UserName { get; set; }
        public string CouponTitle { get; set; }
        public int uniqueId { get; set; }
    }
}
