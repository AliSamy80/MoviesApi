using MoviesApi.Helpers;
using MoviesApi.Services;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMoviesService _moviesService;
        private readonly IGenresService _genresService;
        private readonly IMapper _mapper;


        private new List<string> allowedExtensions = new List<string>() { ".jpg" , ".png" };
        private long maxAllowedPosterSize = 1048576;


        public MoviesController(IMoviesService moviesService, IGenresService genresService,  IMapper mapper)
        {
            _moviesService = moviesService;
            _genresService = genresService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var movies = await _moviesService.GetAll();
            // To Do => Map Movies to DTO   
            var data = _mapper.Map<IEnumerable<MovieDetailsDTO>>(movies);
            return Ok(data);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var movie = await _moviesService.GetById(id);
            if (movie == null)
                return NotFound("There Is No Movie With This ID");

            var dto = _mapper.Map<MovieDetailsDTO>(movie);
            return Ok(dto);
        }

        [HttpGet("GetByGenreId/{genreId:int}")]
        public async Task<IActionResult> GetByGenreIdAsync(byte genreId)
        {
            var movies = await _moviesService.GetAll(genreId);
            var data = _mapper.Map<IEnumerable<MovieDetailsDTO>>(movies);
            return Ok(data);
        }



        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm] CreateMovieDTO dTO)
        {
            if (dTO.Poster == null)
            {
                return BadRequest("Poster is required");
            }
            if (!allowedExtensions.Contains(Path.GetExtension(dTO.Poster.FileName).ToLower()))
            {
                return BadRequest("Only .jpg and .png Extensions are Allowed");
            }
            if (dTO.Poster.Length > maxAllowedPosterSize)
            {
                return BadRequest("Max Allowed Size For Poster 1 MB");
            }

            var isValidGenre = await _genresService.isValidGenre(dTO.GenreId);

            if (!isValidGenre)
            {
                return BadRequest("Invalid Genre Id");
            }
            using var dataStream = new MemoryStream();

            await dTO.Poster.CopyToAsync(dataStream);

            var movie = _mapper.Map<Movie>(dTO);

            movie.Poster = dataStream.ToArray();

            await _moviesService.Add(movie);

            return Ok(movie);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> EditAsync([FromRoute] int id,[FromForm] CreateMovieDTO dTO)
        {
            var movie = await _moviesService.GetById(id);

            if (movie == null)
            {
                return NotFound("There Is No Movie With This ID");
            }

            var isValidGenre = await _genresService.isValidGenre(dTO.GenreId);

            if (!isValidGenre)
            {
                return BadRequest("Invalid Genre Id");
            }
            if(dTO.Poster != null)
            {
                if (!allowedExtensions.Contains(Path.GetExtension(dTO.Poster.FileName).ToLower()))
                {
                    return BadRequest("Only .jpg and .png Extensions are Allowed");
                }
                if (dTO.Poster.Length > maxAllowedPosterSize)
                {
                    return BadRequest("Max Allowed Size For Poster 1 MB");
                }
                using var dataStream = new MemoryStream();

                await dTO.Poster.CopyToAsync(dataStream);

                movie.Poster = dataStream.ToArray();
            }

            movie.Title = dTO.Title;
            movie.Year = dTO.Year;
            movie.Rate = dTO.Rate;
            movie.StoreLine = dTO.StoreLine;
            movie.GenreId = dTO.GenreId;

            _moviesService.Update(movie);
            return Ok(movie);
        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {

            var movie = await _moviesService.GetById(id);

            if (movie == null)
                return NotFound($"There Is No Movie With Id = {id} To Remove");

            _moviesService.Delete(movie);
            return Ok(movie);
        }
    }
}