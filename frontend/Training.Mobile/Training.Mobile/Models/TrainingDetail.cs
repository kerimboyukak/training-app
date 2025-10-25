using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Training.Mobile.Models
{
    public class TrainingDetail
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int MaximumCapacity { get; set; }
        public Room? Room { get; set; }
        public Coach? Coach { get; set; }
        public IList<Participation> Participations { get; set; } = new List<Participation>();
        public string TimeWindow { get; set; } = default!;
        public bool IsFull  { get; set; }
        public bool IsUserRegistered { get; set; }
    }
}
