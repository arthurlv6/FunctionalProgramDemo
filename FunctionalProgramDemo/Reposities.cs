using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FunctionalProgramDemo
{
    public class BaseRepo
    {
        protected readonly CouponDBContext _dBContext;
        protected readonly IMapper _mapper;
        public BaseRepo(CouponDBContext dBContext, IMapper mapper)
        {
            _dBContext = dBContext;
            _mapper = mapper;
        }
        public async Task<Maybe<M>> GetOneAsync<T, M>(int id) where T : BaseEntity where M : BaseModel
        {
            try
            {
                var requirement = await _dBContext.Set<T>()
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(d => d.Id == id);
                if (requirement != null)
                    return Maybe.Ok(requirement.ToModel<M>(_mapper));
                else
                    return Maybe.Fail<M>("Not found coupon by id.");
            }
            catch (Exception ex)
            {
                return Maybe.Fail<M>("GetOneAsync failed", ex);
            }
        }
        public async Task UpdateAsync<T, M>(M m) where T : BaseEntity where M : BaseModel
        {
            try
            {
                var entity = _dBContext.Set<T>().First(d => d.Id == m.Id);
                _mapper.Map(m, entity);
                await _dBContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
    public class CouponRepo:BaseRepo
    {
        public CouponRepo(CouponDBContext _dBContext, IMapper _mapper) : base(_dBContext, _mapper)
        {
        }
        public async Task<Maybe<IEnumerable<M>>> GetAllAvaliableAsync<M>(int quantity=100) where M : BaseModel
        {
            try
            {
                //if(quantity>500 || quantity<100) return Maybe.Fail<IEnumerable<M>>("Quantity should be between 100 to 500.");
                var data = await _dBContext.Set<Coupon>().Where(d => d.Redemptions.Count() < d.MaxAllUser).Take(quantity)
                            .Select(d => d.ToModel<M>(_mapper))
                            .ToListAsync();
                return Maybe.Ok<IEnumerable<M>>(data);
            }
            catch (Exception ex)
            {
                return Maybe.Fail<IEnumerable<M>>("Failed to search avaliable coupons", ex);
            }
        }

        public async Task<Maybe<CouponModel>> GetCouponByTitleAsync(string couponTitle)
        {
            try
            {
                var coupon = await _dBContext.Set<Coupon>().FirstOrDefaultAsync(d => d.Title == couponTitle);
                if (coupon == null) return Maybe.Fail<CouponModel>("coupon was not found.");
                return Maybe.Ok<CouponModel>(_mapper.Map<CouponModel>(coupon));
            }
            catch (Exception ex)
            {
                return Maybe.Fail<CouponModel>("FirstOrDefaultAsync failed", ex);
            }
           
        }

        public async Task<Maybe<bool>> IsNotAvailabeByTitleAsync(string couponTitle)
        {
            try
            {
                var utc = DateTime.UtcNow;
                return Maybe.Ok(!await _dBContext.Set<Coupon>().Where(d => d.StartDate < utc && d.EndDate > utc).AnyAsync(d => d.Title == couponTitle));
            }
            catch (Exception ex)
            {
                return Maybe.Fail<bool>("IsNotAvailabeByTitleAsync failed.",ex);
            }
        }
    }
    public class UserRepo : BaseRepo
    {
        public UserRepo(CouponDBContext _dBContext, IMapper _mapper) : base(_dBContext, _mapper)
        {
        }
        public async Task<Maybe<bool>> IsNotAvailabeByNameAsync(string name)
        {
            try
            {
                return Maybe.Ok( !await _dBContext.Set<User>().AnyAsync(d => d.Name == name));
            }
            catch (Exception ex)
            {
                return Maybe.Fail<bool>("IsNotAvailabeByTitleAsync failed.", ex);
            }
        }
        public async Task<Maybe<UserModel>> GetUserByNameAsync(string name)
        {
            try
            {
                var user = await _dBContext.Set<User>().FirstOrDefaultAsync(d => d.Name == name);
                if (user == null) return Maybe.Fail<UserModel>("Not found by the user name.");
                return Maybe.Ok( _mapper.Map<UserModel>(user));
            }
            catch (Exception ex)
            {
                return Maybe.Fail<UserModel>("FirstOrDefaultAsync failed.", ex);
            }
        }
    }
    public class RedemptionRepo : BaseRepo
    {
        public RedemptionRepo(CouponDBContext _dBContext, IMapper _mapper) : base(_dBContext, _mapper)
        {
        }
        public virtual async Task<Maybe<bool>> IsOverPerUserLimitAsync(int userId,int couponId,int maxPerUser)
        {
            try
            {
                var count = await _dBContext.Set<Redemption>().CountAsync(d => d.UserId == userId && d.CouponId == couponId);
                if (count < maxPerUser) return Maybe.Ok(false);
                return Maybe.Ok(true);
            }
            catch (Exception ex)
            {
                return Maybe.Fail<bool>("FirstOrDefaultAsync failed.", ex);
            }
        }
        public virtual async Task<Maybe<bool>> IsOverAllUserLimitAsync(int couponId,int maxAllUser)
        {
            try
            {
                var count = await _dBContext.Set<Redemption>().CountAsync(d => d.CouponId == couponId);
                if (count < maxAllUser) return Maybe.Ok(false);
                return Maybe.Ok(true);
            }
            catch (Exception ex)
            {
                return Maybe.Fail<bool>("FirstOrDefaultAsync failed.", ex);
            }
            
        }
        public virtual async Task<Maybe<RedemptionModel>> SaveRedeemCouponAsync(Redemption redemption)
        {
            try
            {
                await _dBContext.Set<Redemption>().AddAsync(redemption);
                await _dBContext.SaveChangesAsync();
                return Maybe.Ok(_mapper.Map<RedemptionModel>(redemption));
            }
            catch (Exception ex)
            {
                return Maybe.Fail<RedemptionModel>("SaveRedeemCouponAsync failed", ex);
            }
        }
        public virtual async Task<Maybe<List<RedemptionModel>>> GetUserReportAsync(int userId)
        {
            try
            {
                return Maybe.Ok(await _dBContext.Set<Redemption>().Include(d=>d.Coupon).Include(d=>d.User).Where(d => d.UserId == userId).Select(d => d.ToModel<RedemptionModel>(_mapper)).ToListAsync());
            }
            catch (Exception ex)
            {
                return Maybe.Fail<List<RedemptionModel>>("GetUserReportAsync failed",ex);
            }
        }
    }
}
