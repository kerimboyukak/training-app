using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Training.Api.Models;
using Training.AppLogic;
using Training.Domain;

namespace Training.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingsController : ControllerBase
    {
        private readonly ITrainingRepository _trainingRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly ICoachService _coachService;
        private readonly IApprenticeService _apprenticeService;
        private readonly IMapper _mapper;

        public TrainingsController(ITrainingRepository trainingRepository, IRoomRepository roomRepository,IApprenticeService apprenticeService, ICoachService coachService, IMapper mapper)
        {
            _trainingRepository = trainingRepository;
            _roomRepository = roomRepository;
            _apprenticeService = apprenticeService;
            _coachService = coachService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var trainings = await _trainingRepository.GetAllAsync();
            var trainingDetailModels = new List<TrainingDetailModel>();

            foreach (Domain.Training training in trainings)
            {
                TrainingDetailModel model = _mapper.Map<TrainingDetailModel>(training);
                trainingDetailModels.Add(model);
            }
            return Ok(trainingDetailModels);
        }
        [HttpGet("rooms")]
        public async Task<IActionResult> GetAllRooms()
        {
            var rooms = await _roomRepository.GetAllAsync();
            var roomDetailModels = rooms.Select(room => new RoomDetailModel
            {
                RoomCode = room.RoomCode.ToString(),
                Name = room.Name
            }).ToList();

            return Ok(roomDetailModels);
        }

        [HttpGet("future")]
        public async Task<IActionResult> GetFutureTrainings()
        {
            var futureTrainings = await _trainingRepository.GetFutureTrainingsAsync();
            var futureTrainingDetailModels = new List<TrainingDetailModel>();

            foreach (Domain.Training training in futureTrainings)
            {
                TrainingDetailModel model = _mapper.Map<TrainingDetailModel>(training);
                futureTrainingDetailModels.Add(model);
            }
            return Ok(futureTrainingDetailModels);
        }
        [HttpGet("past")]
        public async Task<IActionResult> GetPastTrainings()
        {
            var pastTrainings = await _trainingRepository.GetPastTrainingsAsync();
            var pastTrainingDetailModels = new List<TrainingDetailModel>();

            foreach (Domain.Training training in pastTrainings)
            {
                TrainingDetailModel model = _mapper.Map<TrainingDetailModel>(training);
                pastTrainingDetailModels.Add(model);
            }
            return Ok(pastTrainingDetailModels);
        }


        [HttpGet("{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            Domain.Training? training = await _trainingRepository.GetByCodeAsync(new Code(code));
            if (training is null)
            {
                return NotFound();
            }
            return training is null ? NotFound() : Ok(_mapper.Map<TrainingDetailModel>(training));
        }

        [HttpGet("{code}/participation/{id}")]
        public async Task<IActionResult> GetParticipation(string code, string id)
        {
            var training = await _trainingRepository.GetByCodeAsync(new Code(code));
            if (training is null)
            {
                return NotFound();
            }

            var participation = training.Participations.FirstOrDefault(p => p.ApprenticeId == id);
            return participation is null ? NotFound() : Ok(_mapper.Map<ParticipationDetailModel>(participation));
        }

        [HttpPost]
        [Authorize(policy: "write")]
        public async Task<IActionResult> CreateTraining([FromBody] TrainingCreateModel model)
        {
            var timeWindow = new TimeWindow(model.StartTime, model.EndTime);
            Domain.Training training = await _coachService.CreateTraining(model.Name, model.Description, model.MaximumCapacity, model.RoomCode, model.CoachId, timeWindow);
            var outputModel = _mapper.Map<TrainingDetailModel>(training);

            return CreatedAtAction(nameof(GetByCode), new { code = outputModel.Code }, outputModel);

        }

        [HttpPost("{code}/register/{id}")]
        [Authorize(policy: "write")]
        public async Task<IActionResult> RegisterApprentice(string code, string id)
        {
            var apprentice = await _apprenticeService.RegisterApprentice(code, id);
            return CreatedAtAction(nameof(GetByCode), new { code = code }, apprentice);

        }

        [HttpPost("{code}/registerExternal")]
        [Authorize(policy: "write")]
        public async Task<IActionResult> RegisterExternalApprentice(string code, [FromBody] ApprenticeCreateModel model)
        {
            var apprentice = await _apprenticeService.RegisterExternalApprentice(code, model.FirstName, model.LastName, model.Company);
            return CreatedAtAction(nameof(GetByCode), new { code = code }, apprentice);

        }

        [HttpPost("{code}/finish/{id}")]
        [Authorize(policy: "write")]
        public async Task<IActionResult> FinishParticipation(string code, string id)
        {
            var participation = await _apprenticeService.FinishParticipation(code, id);
            var participationModel = _mapper.Map<ParticipationDetailModel>(participation);
            return CreatedAtAction(nameof(GetParticipation), new { code = code, id = id }, participationModel);
        }
    }
}
