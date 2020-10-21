using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FunctionalProgramDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CouponController : Controller
    {
        private readonly CouponRepo couponRepo;

        public CouponController(CouponRepo couponRepo)
        {
            this.couponRepo = couponRepo;
        }
        [HttpGet("GetAllAvailableCoupons")]
        public IActionResult GetAsync(int couponQuantity = 100)
        {
            var result = Maybe.Ok(couponQuantity)
                .Railway(q => CheckQuantity(q.Value))
                .Railway(q => couponRepo.GetAllAvaliableAsync<CouponModel>(q.Value).Result);
            
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return BadRequest(result.Error);
        }
        private Maybe<int> CheckQuantity(int couponQuantity)
        {
            if (couponQuantity > 500 || couponQuantity < 100) return Maybe.Fail<int>("couponQuantity should be 100-500.");
            return Maybe.Ok<int>(couponQuantity);
        }
    }
}
