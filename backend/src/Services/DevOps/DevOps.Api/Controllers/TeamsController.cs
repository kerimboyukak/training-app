using AutoMapper;
using DevOps.Api.Models;
using DevOps.AppLogic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevOps.Api.Controllers
{ 
    [ApiController]
    [Route("[controller]")]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamService _teamService;
        private readonly ITeamRepository _teamRepository;
        private readonly IMapper _mapper;
        public TeamsController(ITeamService teamService, ITeamRepository teamRepository, IMapper mapper)
        {
            _teamService = teamService;
            _teamRepository = teamRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var teams = await _teamRepository.GetAllAsync();
            var teamDetailModels = new List<TeamDetailModel>();

            foreach (Team team in teams)
            {
                TeamDetailModel model = _mapper.Map<TeamDetailModel>(team);
                teamDetailModels.Add(model);
            }
            return Ok(teamDetailModels);
        }

        [HttpPost("{id}/assemble")]
        [Authorize(policy: "write")]    // securing the action method -> enforcing the "write" policy
        public async Task<IActionResult> AssembleTeam(Guid id, TeamAssembleInputModel model)
        {
            Team? team = await _teamRepository.GetByIdAsync(id);
            if (team is null)
            {
                return NotFound();
            }
            await _teamService.AssembleDevelopersAsyncFor(team, model.RequiredNumberOfDevelopers);
            return Ok();
        }
    }
}