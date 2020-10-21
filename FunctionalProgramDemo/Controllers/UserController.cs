using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FunctionalProgramDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly UserService userService;
        public UserController(UserService userServer)
        {
            this.userService = userServer;
        }
        [HttpGet("CanRedeem")]
        public async Task<IActionResult> CheckIfUserCanRedeemAsync(string couponTitle = "", string userName="" )
        {
            var result = await userService.CheckIfUserCanRedeemAsync(couponTitle, userName);
            if (result.IsSuccess)
                return Ok(result.Value);
            return BadRequest(result.Error);
        }


        [HttpGet("UserRedemptionReport")]
        public async Task<IActionResult> GetReportAsync(string userName = "")
        {
            var result = await userService.UserRedemptionReport(userName);
            if (result.IsSuccess)
                return Ok(result.Value);
            return BadRequest(result.Error);
        }
        [HttpPost("RedeemCoupon")]
        public IActionResult Post([FromBody] RedeemCouponModel  redeemCouponModel)
        {
            var result = userService.RedeemCoupon(redeemCouponModel);
            if (result.IsSuccess)
                return Ok(result.Value);
            return BadRequest(result.Error);
        }
    }
}
