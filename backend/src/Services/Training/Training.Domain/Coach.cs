using Domain;

namespace Training.Domain
{
    public class Coach : Entity
    {
        public string Id { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        protected override IEnumerable<object> GetIdComponents()
        {
            yield return Id;
        }

        private Coach(string id, string firstName, string lastName)
        {
            Contracts.Require(!string.IsNullOrEmpty(id), "The identifier of a coach cannot be empty");
            Contracts.Require(!string.IsNullOrEmpty(firstName), "The last name of a coach cannot be empty");
            Contracts.Require(!string.IsNullOrEmpty(lastName), "The first name of a coach cannot be empty");

            Id = id;
            FirstName = firstName;
            LastName = lastName;
        }

        public static Coach CreateNew(string id, string firstName, string lastName)
        {
            return new Coach(id, firstName, lastName);
        }
    }
}
