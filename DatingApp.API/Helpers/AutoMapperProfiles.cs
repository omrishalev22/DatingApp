using System;
using System.Linq;
using AutoMapper;
using DatingApp.API.Dtos;
using DatingApp.API.Models;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User,UserForListDto>().ForMember(dest => dest.PhotoUrl, opt => {
                opt.MapFrom(src => src.Photos.FirstOrDefault(item=>item.isMain).Url);
            })
            .ForMember(dest => dest.Age, opt => {
                opt.ResolveUsing(src => src.DateOfBirth.CalculateAge());
            });
            CreateMap<User,UserForDetailsDto>()
            .ForMember(dest => dest.Age, opt => {
                opt.ResolveUsing(src => src.DateOfBirth.CalculateAge());
            });;
            CreateMap<Photo,PhotoForDetailedDto>();
        }
    }
}