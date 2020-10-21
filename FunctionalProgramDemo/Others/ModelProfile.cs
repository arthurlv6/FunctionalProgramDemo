using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FunctionalProgramDemo
{
    public class ModelProfile : Profile
    {
        public ModelProfile()
        {
            CreateMap<Coupon, CouponModel>().ReverseMap();
            CreateMap<Redemption, RedemptionModel>()
                .ForMember(d => d.CouponTitle, o => o.MapFrom(s => s.Coupon.Title))
                .ForMember(d => d.UserName, o => o.MapFrom(s => s.User.Name))
                .ReverseMap();
            CreateMap<User, UserModel>().ReverseMap();
        }
    }
}
