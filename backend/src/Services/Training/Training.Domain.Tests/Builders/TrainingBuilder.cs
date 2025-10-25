using System.Reflection;
using Test;

namespace Training.Domain.Tests.Builders
{
    public class TrainingBuilder : BuilderBase<Training>
    {
        public TrainingBuilder()
        {
            Item = Training.CreateNew(
                Random.Shared.NextString(10, true),
                Random.Shared.NextString(),
                Random.Shared.Next(1, 100),
                new Code(Random.Shared.NextString(10, true), Random.Shared.Next(1, 10)),
                Random.Shared.NextString(),
                new TimeWindow(DateTime.Now, DateTime.Now.AddHours(1)),
                Random.Shared.Next(1, 10)
            );
        }
        public TrainingBuilder WithTrainingCode(Code trainingCode)
        {
            SetProperty(t => t.TrainingCode, trainingCode);
            return this;
        }
    }
}