using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Training.Mobile.Models
{
    public class Participation
    {
        public string TrainingCode { get; set; } = string.Empty;
        public Apprentice? Apprentice { get; set; }
        public bool IsFinished { get; set; }
    }
}
