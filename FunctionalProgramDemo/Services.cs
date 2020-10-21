using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace FunctionalProgramDemo
{
    public enum CanRedeem
    {
        YouCan,
        CouponNotExist,
        OverUserLimit,
        OverAllLimit
    }
    public class UserService
    {
        private readonly CouponRepo couponRepo;
        private readonly UserRepo userRepo;
        private readonly RedemptionRepo redemptionRepo;

        public UserService(CouponRepo couponRepo, UserRepo userRepo, RedemptionRepo redemptionRepo)
        {
            this.couponRepo = couponRepo;
            this.userRepo = userRepo;
            this.redemptionRepo = redemptionRepo;
        }
        public async Task<Maybe<UserModel>> GetUserByNameAsync(string name)
        {
            return await userRepo.GetUserByNameAsync(name);
        }

        public async Task<Maybe<string>> CanRedeemAsync(int couponId,int userId)
        {
            return (await couponRepo.GetOneAsync<Coupon, CouponModel>(couponId))
                .Railway(coupon =>
                {
                    if (redemptionRepo.IsOverPerUserLimitAsync(userId, couponId, coupon.Value.MaxPerUser).Result.Value)
                        return Maybe.Fail<int>("IsOverPerUserLimitAsync");
                    return Maybe.Ok(coupon.Value.MaxAllUser);
                })
                .Railway(maxCoupon => {
                    if ((redemptionRepo.IsOverAllUserLimitAsync(couponId, maxCoupon.Value)).Result.Value)
                        return Maybe.Fail<string>("IsOverAllUserLimitAsync");
                    return Maybe.Ok<string>("Yes, you can");
                });
        }
        private async Task<Maybe<RedemptionModel>> SaveRedeemCouponAsync(RedemptionModel redemptionModel)
        {
            return await redemptionRepo.SaveRedeemCouponAsync(new Redemption() { CouponId = redemptionModel.CouponId, UserId = redemptionModel.UserId, OperationDate = DateTime.UtcNow });
        }

        public Maybe<RedemptionModel> RedeemCoupon(RedeemCouponModel redeemCouponModel)
        {
            return  Maybe.Ok(redeemCouponModel)//create maybe object
                .Railway(m => GetUserByNameAsync(m.Value.UserName).Result)//get user
                .Railway(u =>
                {
                    var coupon = couponRepo.GetCouponByTitleAsync(redeemCouponModel.CouponTitle).Result;
                    if (coupon.IsFailure)
                        return Maybe.Fail<RedemptionModel>(coupon.Error);
                    return Maybe.Ok(new RedemptionModel { UserId = u.Value.Id, CouponId = coupon.Value.Id });
                })//get coupon and wrap it into model
                .Railway(m =>
                {
                    var can = CanRedeemAsync(m.Value.CouponId, m.Value.UserId).Result;
                    if (can.IsSuccess)
                        return Maybe.Ok<RedemptionModel>(SaveRedeemCouponAsync(m.Value).Result.Value);
                    else
                        return Maybe.Fail<RedemptionModel>(can.Error);
                });//save redemption
        }
        public async Task<Maybe<List<RedemptionModel>>> UserRedemptionReport(string userName="")
        {
            return (await GetUserByNameAsync(userName))// get user
               .Railway(d => redemptionRepo.GetUserReportAsync(d.Value.Id).Result);// get report
        }
        public async Task<Maybe<string>> CheckIfUserCanRedeemAsync(string couponTitle = "", string userName = "")
        {
            return (await GetUserByNameAsync(userName)) //get user
                .Railway((u) =>
                {
                    var coupon = couponRepo.GetCouponByTitleAsync(couponTitle).Result;
                    if (coupon.IsFailure)
                        return Maybe.Fail<RedemptionModel>(coupon.Exception.Message);
                    return Maybe.Ok(new RedemptionModel { UserId = u.Value.Id, CouponId = coupon.Value.Id });
                }) // get coupon and wrap it into model
                .Railway(m => CanRedeemAsync(m.Value.CouponId, m.Value.UserId).Result);//check
        }
    }
}
