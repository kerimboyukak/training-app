using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Training.Mobile.Models
{
    public class TrainingCreateModel
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int MaximumCapacity { get; set; }
        public string RoomCode { get; set; } = default!;
        public string CoachId { get; set; } = default!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
