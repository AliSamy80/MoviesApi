namespace MoviesApi.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Movie, MovieDetailsDTO>();
            CreateMap<CreateMovieDTO, Movie>()
                .ForMember(src => src.Poster, opt => opt.Ignore());

        }
    }
}
