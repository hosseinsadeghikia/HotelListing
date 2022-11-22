using AutoMapper;
using HotelListing.Core.DTOs;
using HotelListing.Core.IRepository;
using HotelListing.Core.Models;
using HotelListing.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HotelController> _logger;
        private readonly IMapper _mapper;

        public HotelController(IUnitOfWork unitOfWork,
            ILogger<HotelController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
        [ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetHotels()
        {
            var hotels = await _unitOfWork.Hotels.GetAll();
            var res = _mapper.Map<List<HotelDTO>>(hotels);
            return Ok(res);
        }

        [HttpGet]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
        [ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
        [Route("GetHotelsPagedList")]
        public async Task<IActionResult> GetHotelsPagedList([FromQuery] RequestParams requestParams)
        {
            var hotels = await _unitOfWork.Hotels.GetAllPagedList(requestParams);
            var res = _mapper.Map<List<HotelDTO>>(hotels);
            return Ok(res);
        }

        [HttpGet("{id:int}", Name = "GetHotelsById")]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
        [ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetHotelsById(int id)
        {
            var hotel = await _unitOfWork.Hotels.Get(x => x.Id == id,
                    include: x => x.Include(q => q.Country));
            var res = _mapper.Map<HotelDTO>(hotel);
            return Ok(res);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("CreateHotel")]
        [ProducesResponseType(statusCode: StatusCodes.Status400BadRequest)]
        [ProducesResponseType(statusCode: StatusCodes.Status201Created)]
        [ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateHotel([FromBody] CreateHotelDTO hotelDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Invalid POST attempt in {nameof(CreateHotel)}");
                return BadRequest(ModelState);
            }

            var hotel = _mapper.Map<Hotel>(hotelDto);
            await _unitOfWork.Hotels.Insert(hotel);
            await _unitOfWork.SaveAsync();

            return CreatedAtRoute("GetHotelsById",
                new { id = hotel.Id }, hotel);
        }

        [Authorize]
        [HttpPut("{id:int}")]
        [ProducesResponseType(statusCode: StatusCodes.Status400BadRequest)]
        [ProducesResponseType(statusCode: StatusCodes.Status204NoContent)]
        [ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateHotel(int id, [FromBody] UpdateHotelDTO hotelDto)
        {
            if (!ModelState.IsValid || id < 1)
            {
                _logger.LogError($"Invalid POST attempt in {nameof(UpdateHotel)}");
                return BadRequest(ModelState);
            }

            var hotel = await _unitOfWork.Hotels.Get(x => x.Id == id);

            if (hotel == null)
            {
                _logger.LogError($"Invalid POST attempt in {nameof(UpdateHotel)}");
                return BadRequest("Submitted data is invalid");
            }

            _mapper.Map(hotelDto, hotel);
            _unitOfWork.Hotels.Update(hotel);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(statusCode: StatusCodes.Status400BadRequest)]
        [ProducesResponseType(statusCode: StatusCodes.Status204NoContent)]
        [ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            if (!ModelState.IsValid || id < 1)
            {
                _logger.LogError($"Invalid Delete attempt in {nameof(DeleteHotel)}");
                return BadRequest(ModelState);
            }

            var hotel = await _unitOfWork.Hotels.Get(x => x.Id == id);

            if (hotel == null)
            {
                _logger.LogError($"Invalid Delete attempt in {nameof(DeleteHotel)}");
                return BadRequest("Submitted data is invalid");
            }

            await _unitOfWork.Hotels.Delete(id);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }
    }
}
