using Domain;

namespace Training.Domain
{
    public class Training : Entity
    {
        private readonly List<Participation> _participations;
        public Code TrainingCode { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int MaximumCapacity { get; private set; }
        public Code RoomCode { get; private set; }
        public string CoachId { get; private set; }
        public TimeWindow TimeWindow { get; private set; }
        public Room? Room { get; private set; }
        public Coach? Coach { get; private set; }
        public IReadOnlyList<Participation> Participations => _participations;

        private Training() // EF 
        {
            _participations = new List<Participation>();
        }

        private Training(string name, string description, int maximumCapacity, Code roomCode, string coachId, TimeWindow timeWindow, int sequence)
        {
            Contracts.Require(maximumCapacity >= 2, "Maximum capacity has to be minimum 2");
            Contracts.Require(name.All(char.IsLetterOrDigit), "The name of the training can only contain letters and digits");
            Contracts.Require(!string.IsNullOrEmpty(name), "The name of a training cannot be empty");
            Contracts.Require(!string.IsNullOrEmpty(description), "The description of a training cannot be empty");
            Contracts.Require(!string.IsNullOrEmpty(coachId), "The coach of a training cannot be empty");

            Name = name;
            Description = description;
            MaximumCapacity = maximumCapacity;
            _participations = new List<Participation>();
            RoomCode = roomCode;
            CoachId = coachId;
            TimeWindow = timeWindow;
            TrainingCode = new Code(Name, sequence);
        }

        public static Training CreateNew(string name, string description, int maximumCapacity, Code roomCode, string coachId, TimeWindow timeWindow, int sequence)
        {

            return new Training(name, description, maximumCapacity, roomCode, coachId, timeWindow, sequence);

        }

        public void Register(Apprentice apprentice)
        {
            Contracts.Require(_participations.Count < MaximumCapacity, "Maximum capacity reached.");
            Contracts.Require(!_participations.Any(p => p.ApprenticeId == apprentice.Id), "Apprentice already registered.");
            Contracts.Require(DateTime.Now <= TimeWindow.Start.AddMinutes(15), "Registration is closed. You can only register up to 15 minutes after the training has started.");

            var participation = Participation.CreateNew(TrainingCode.ToString(), apprentice.Id);
            _participations.Add(participation);
        }
        
        public void FinishParticipation(string apprenticeId)
        {
            var participation = _participations.FirstOrDefault(p => p.ApprenticeId == apprenticeId);
            Contracts.Require(participation is not null, "Apprentice is not registered for this training.");

            participation?.Finish();
        }

        protected override IEnumerable<object> GetIdComponents()
        {
            yield return TrainingCode;
        }
    }
}
