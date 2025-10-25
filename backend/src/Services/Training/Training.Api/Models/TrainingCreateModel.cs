namespace Training.Api.Models
{
    public class TrainingCreateModel
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int MaximumCapacity { get; set; }
        public string RoomCode { get; set; } = string.Empty;
        public string CoachId { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
