using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Training.Api.Models;
using Training.AppLogic;
using Training.Domain;

namespace Training.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoachesController : ControllerBase
    {
        private readonly ICoachRepository _coachRepository;
        private readonly IMapper _mapper;
        public CoachesController(ICoachRepository coachRepository, IMapper mapper)
        {
            _coachRepository = coachRepository;
            _mapper = mapper;
        }

        [HttpGet("{number}")]
        public async Task<IActionResult> GetByNumber(string number)
        {
            Coach? coach = await _coachRepository.GetByIdAsync(number);
            return coach is null ? NotFound() : Ok(_mapper.Map<CoachDetailModel>(coach));
        }
    }
}
