using DevOps.Domain;
using Domain;

public class Team : Entity
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }

    private readonly List<Developer> _developers;
    public IReadOnlyList<Developer> Developers => _developers;  //other classes can only read the list of developers, they can't add or remove developers from a team via the property

    private Team(Guid id, string name)
    {
        Id = id;
        Name = name;
        _developers = new List<Developer>();
    }

    public static Team CreateNew(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ContractException("The name of a team cannot be empty");
        }

        Guid id = Guid.NewGuid();
        if (id == Guid.Empty)
        {
            throw new ContractException("The identifier of a team cannot be empty");
        }

        return new Team(id, name);
    }

    public void Join(Developer developer)
    {
        if (developer.TeamId == Id)
        {
            throw new ContractException("The developer is already in this team");
        }

        if (developer.TeamId != null)
        {
            throw new ContractException("The developer is already in another team");
        }

        developer.TeamId = Id;
        _developers.Add(developer);
    }

    protected override IEnumerable<object> GetIdComponents()
    {
        yield return Id;    // 2 Teams are considered equal if their Ids are equal
    }
}