using DevOps.AppLogic;
using DevOps.Domain;

internal class TeamService : ITeamService
{
    private readonly IDeveloperRepository _developerRepository;
    public TeamService(IDeveloperRepository developerRepository)
    {
        _developerRepository = developerRepository;
    }

    public async Task AssembleDevelopersAsyncFor(Team team, int requiredNumberOfDevelopers)
    {
        IReadOnlyList<Developer> availableDevelopers = await _developerRepository.FindDevelopersWithoutATeamAsync();

        // Enough developers available / Not enough developers available
        int developersToSelect = Math.Min(requiredNumberOfDevelopers, availableDevelopers.Count);

        Random random = new Random();
        List<Developer> selectedDevelopers = availableDevelopers
            .OrderBy(_ => random.Next())
            .Take(developersToSelect) 
            .ToList();      // Take a random selection of developers from the available developers

        foreach (Developer developer in selectedDevelopers)
        {
            team.Join(developer);
        }

        await _developerRepository.CommitTrackedChangesAsync();
    }
}