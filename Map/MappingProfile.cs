using System.Collections.Generic;
using AutoMapper;
using DataSystem.Models;
using DataSystem.Models.ViewModels;
using DataSystem.Models.ViewModels.chart;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Add as many of these lines as you need to map your objects
        CreateMap<TblIycf, IycfDto>().ForMember(vm => vm.CauseShortName, map => map.MapFrom(s => s.Iycf.CauseShortName));
        CreateMap<TblOtp, SamoutDto>().ForMember(vm => vm.AgeGroup, map => map.MapFrom(s => s.Otp.AgeGroup));

        CreateMap<TblOtptfu, SaminDto>().ForMember(vm => vm.AgeGroup, map => map.MapFrom(s => s.Otptfu.AgeGroup));

        CreateMap<TblMam, mamVM>().ForMember(vm => vm.AgeGroup, map => map.MapFrom(s => s.Mam.AgeGroup));

        CreateMap<TblFstock, fstockViewModel>().ForMember(vm => vm.Item, map => map.MapFrom(s => s.Stock.Item));

        CreateMap<TblStockOtp, stockoutDto>()
        .ForMember(vm => vm.Item, map => map.MapFrom(s => s.Sstockotp.Item))
        .ForMember(vm => vm.StockId, map => map.MapFrom(s => s.SstockotpId));

        CreateMap<TblStockIpt, stockinDto>()
        .ForMember(vm => vm.Item, map => map.MapFrom(s => s.Sstock.Item));


    }
}