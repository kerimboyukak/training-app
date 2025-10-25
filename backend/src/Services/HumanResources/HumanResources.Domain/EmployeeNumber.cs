using Domain;

namespace HumanResources.Domain
{
    public class EmployeeNumber : ValueObject<EmployeeNumber>
    {
        public int Year { get; }
        public int Month { get; }
        public int Day { get; }
        public int Sequence { get; }

        public EmployeeNumber(DateTime startDate, int sequence)
        {
            Contracts.Require(sequence >= 1, "The sequence in the employee number must be a positive number");

            Year = startDate.Year;
            Month = startDate.Month;
            Day = startDate.Day;
            Sequence = sequence;
        }
        public EmployeeNumber(string value)
        {
            Contracts.Require(!string.IsNullOrEmpty(value), "An employee number cannot be empty");
            Contracts.Require(value.Length == 11, "An employee number must have exactly 11 characters");
            Contracts.Require(value.All(c => char.IsDigit(c)), "An employee number can only contain digits");

            Year = int.Parse(value.Substring(0, 4));
            Contracts.Require(Year > 0, "The first 4 digits of an employee number must be a valid year");

            Month = int.Parse(value.Substring(4, 2));
            Contracts.Require(Month >= 1 && Month <= 12, "Digits 5 and 6 of an employee number must be a valid month");

            Day = int.Parse(value.Substring(6, 2));
            Contracts.Require(Day >= 1 && Day <= 31, "Digits 7 and 8 of an employee number must be a valid day");

            Sequence = int.Parse(value.Substring(8, 3));
            Contracts.Require(Sequence >= 1, "The sequence in the employee number must be a positive number");
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Year;
            yield return Month;
            yield return Day;
            yield return Sequence;
        }

        public override string ToString()
        {
            return $"{Year:0000}{Month:00}{Day:00}{Sequence:000}";  // placeholder 0 means that the number will be padded with zeros when its needed
        }

        // 	Implicit Conversion from EmployeeNumber to string: The ToString method of EmployeeNumber is called implicitly when you assign an EmployeeNumber object to a string variable.
        public static implicit operator string(EmployeeNumber number) => number.ToString();

        //  Implicit Conversion from string to EmployeeNumber: The constructor EmployeeNumber(string value) is called implicitly when you assign a string to an EmployeeNumber variable.
        public static implicit operator EmployeeNumber(string value) => new EmployeeNumber(value);
    }
}
