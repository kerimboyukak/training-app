using Domain;

namespace Training.Domain
{
    public class Participation : Entity
    {
        public Code TrainingCode { get; private set; }
        public string ApprenticeId { get; private set; }
        public bool IsFinished { get; private set; }
        public Apprentice Apprentice { get; private set; } = null!;
        public Training Training { get; private set; } = null!;

        private Participation() // EF Core requires a parameterless constructor
        {
        }

        private Participation(string trainingCode, string apprenticeId)
        {
            TrainingCode = new Code(trainingCode);
            ApprenticeId = apprenticeId;
            IsFinished = false;
        }

        public static Participation CreateNew(string trainingCode, string apprenticeId)
        {
            return new Participation(trainingCode, apprenticeId);
        }

        public void Finish()
        {
            Contracts.Require(!IsFinished, "Participation is already finished.");
            IsFinished = true;
        }

        protected override IEnumerable<object> GetIdComponents()
        {
            yield return TrainingCode;
            yield return ApprenticeId;
        }
    }
}