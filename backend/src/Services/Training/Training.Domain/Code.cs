using Domain;
using System.Xml.Linq;

namespace Training.Domain
{
    public class Code : ValueObject<Code>
    {
        public string Name { get; }
        public int Sequence { get; }

        public Code(string name, int sequence)
        {
            Contracts.Require(!string.IsNullOrEmpty(name), "The name cannot be null or empty");
            Contracts.Require(sequence >= 1, "The sequence must be a positive number");
            Contracts.Require(sequence <= 999, "The sequence cannot be bigger than 100");

            Name = name.Length > 11 ? name.Substring(0, 11) : name;     // Name wont hinder the creation of Code
            Sequence = sequence;
        }

        public Code(string value)
        {
            Contracts.Require(!string.IsNullOrEmpty(value), "A code cannot be empty");
            Contracts.Require(value.Length <= 15, "A code cannot have more than 15 characters");

            var parts = value.Split('-');
            Contracts.Require(parts.Length == 2, "A code must be in the format 'Name-Sequence'");

            Name = parts[0];
            Contracts.Require(!string.IsNullOrEmpty(Name), "The name in the code cannot be null or empty");
            Contracts.Require(Name.All(char.IsLetterOrDigit), "The name can only contain letters and digits");

            Sequence = int.Parse(parts[1]);
            Contracts.Require(Sequence >= 1, "The sequence must be a positive number");
            Contracts.Require(Sequence <= 999, "The sequence cannot be bigger than 100");
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
            yield return Sequence;
        }

        public override string ToString()
        {
            return $"{Name}-{Sequence}";
        }

        public static implicit operator string(Code code) => code.ToString();

        public static implicit operator Code(string value) => new Code(value);
    }
}