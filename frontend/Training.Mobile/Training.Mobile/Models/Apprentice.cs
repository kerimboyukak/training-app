using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Training.Mobile.Models
{
    public class Apprentice
    {
        public string Id { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Company { get; set; } = default!;
        public int Xp { get; set; } = default!;
    }
}
