using Domain;

namespace Training.Domain
{
    public class TimeWindow : ValueObject<TimeWindow>
    {
        public DateTime Start { get; }
        public DateTime End { get; }

        public TimeWindow(DateTime start, DateTime end)
        {
            Contracts.Require(end > start, "End time must come after start time.");

            Start = start;
            End = end;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Start;
            yield return End;
        }

        public bool Overlaps(TimeWindow other)
        {
            return Start < other.End && End > other.Start;
        }

        public override string ToString()
        {
            return $"{Start:dd/MM/yyyy HH:mm} - {End:dd/MM/yyyy HH:mm}";
        }

        public static implicit operator string(TimeWindow timeWindow) => timeWindow.ToString();
    }
}