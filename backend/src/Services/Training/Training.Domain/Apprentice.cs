using Domain;

namespace Training.Domain
{
    public class Apprentice : Entity
    {
        public string Id { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Company { get; private set; }
        public int Xp { get; private set; }

        private Apprentice(string id, string firstName, string lastName, string company)
        {
            Contracts.Require(!string.IsNullOrEmpty(id), "The identifier of a apprentice cannot be empty");  // EmployeeNumber: when a new apprentice is created, Id must match the employee in the HR domain
            Contracts.Require(!string.IsNullOrEmpty(firstName), "The last name of a apprentice cannot be empty");
            Contracts.Require(!string.IsNullOrEmpty(lastName), "The first name of a apprentice cannot be empty");
            Contracts.Require(!string.IsNullOrEmpty(company), "The company of a apprentice cannot be empty");

            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Company = company;
            Xp = 0;
        }
        public static Apprentice CreateNew(string id, string firstName, string lastName, string company)
        {
            return new Apprentice(id, firstName, lastName, company);
        }

        public void AddXp(int xp)
        {
            Contracts.Require(xp >= 0, "XP cannot be negative");
            Xp += xp;
        }

        protected override IEnumerable<object> GetIdComponents()
        {
            yield return Id;
        }
    }
}
