using AutoMapper;
using HotelListing.IRepository;
using HotelListing.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CountryController> _logger;
        private readonly IMapper _mapper;

        public CountryController(IUnitOfWork unitOfWork,
            ILogger<CountryController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
        [ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCountries()
        {
            try
            {
                var countries = await _unitOfWork.Countries.GetAll();
                var res = _mapper.Map<List<CountryDTO>>(countries);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex , $"{"Somethings Went Wrong in the " + nameof(GetCountries)}");
                return StatusCode(500,"Internal Server Error. Please Try Again Later.");
            }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
        [ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCountryById(int id)
        {
            try
            {
                var country = await _unitOfWork.Countries.Get(x=>x.Id == id,
                    new List<string>() { "Hotels" });
                var res = _mapper.Map<CountryDTO>(country);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{"Somethings Went Wrong in the " + nameof(GetCountryById)}");
                return StatusCode(500, "Internal Server Error. Please Try Again Later.");
            }
        }
    }
}
