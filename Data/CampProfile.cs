using AutoMapper;
using CoreApiFundamentals.Models;
using CoreCodeCamp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApiFundamentals.Data
{
    public class CampProfile : Profile
    {
        public CampProfile()
        {
            CreateMap<Camp, CampModel>()
                .ForMember(x => x.Venue, y => y.MapFrom(z => z.Location.VenueName)).ReverseMap();
            CreateMap<Talk, TalkModel>().ReverseMap(); ;
            CreateMap<Speaker, SpeakerModel>().ReverseMap();
        }
    }
}
