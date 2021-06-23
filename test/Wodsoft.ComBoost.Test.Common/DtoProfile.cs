using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Wodsoft.ComBoost.Test.Entities;
using Wodsoft.ComBoost.Test.Models;

namespace Wodsoft.ComBoost.Test
{
    public class DtoProfile : Profile
    {
        public DtoProfile()
        {
            CreateMap<UserEntity, UserDto>()
                .ForMember(t => t.Password, options => options.Ignore());
            CreateMap<UserDto, UserEntity>()
                .ForMember(t => t.Password, options => options.Ignore())
                .ForMember(t => t.CreationDate, options => options.Ignore())
                .ForMember(t => t.ModificationDate, options => options.Ignore())
                .AfterMap((dto, entity) =>
                {
                    if (!string.IsNullOrEmpty(dto.Password))
                        entity.SetPassword(dto.Password);
                });
        }
    }
}
