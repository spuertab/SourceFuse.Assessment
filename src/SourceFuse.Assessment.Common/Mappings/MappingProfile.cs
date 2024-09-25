using SourceFuse.Assessment.Common.Resources.Entities;
using SourceFuse.Assessment.Common.Models;
using AutoMapper;

namespace SourceFuse.Assessment.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Song, SongModel>().ReverseMap();
        }
    }
}
