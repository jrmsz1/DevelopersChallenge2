using AutoMapper;
using SERVICESAPI.DTOs;
using SERVICESAPI.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SERVICESAPI
{
    public class OFXProfile : Profile
    {

        public OFXProfile()
        {
            CreateMap<OFX_STMTTRN, OFX_STMTTRN_DTO>()
                .ForMember(dest => dest.TIPOTRN, opt => opt.MapFrom(src => src.TRNTYPE))
                .ForMember(dest => dest.DATATRN, opt => opt.MapFrom(src => src.DTPOSTED))
                .ForMember(dest => dest.DATATRN_TMZ, opt => opt.MapFrom(src => src.DTPOSTED_TMZ))
                .ForMember(dest => dest.VALOR, opt => opt.MapFrom(src => src.TRNAMT))
                .ForMember(dest => dest.DESCRICAO, opt => opt.MapFrom(src => src.MEMO))
                .ForMember(dest => dest.DATA_RECONCILIACAO, opt => opt.MapFrom(src => src.DT_RECONCILIATION)).ReverseMap();


        }
    }
}
