using AutoMapper;
using HotelListing.IRepository;
using HotelListing.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CountryController> _logger;
        private readonly IMapper _mapper;

        public HotelController(IUnitOfWork unitOfWork,
            ILogger<CountryController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(statusCode:StatusCodes.Status200OK)]
        [ProducesResponseType(statusCode:StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetHotels()
        {
            try
            {
                var hotels = await _unitOfWork.Hotels.GetAll();
                var res = _mapper.Map<List<HotelDTO>>(hotels);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{"Somethings Went Wrong in the " + nameof(GetHotels)}");
                return StatusCode(500, "Internal Server Error. Please Try Again Later.");
            }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
        [ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetHotelsById(int id)
        {
            try
            {
                var hotel = await _unitOfWork.Hotels.Get(x => x.Id == id,
                    new List<string>() { "Country" });
                var res = _mapper.Map<HotelDTO>(hotel);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{"Somethings Went Wrong in the " + nameof(GetHotelsById)}");
                return StatusCode(500, "Internal Server Error. Please Try Again Later.");
            }
        }
    }
}
