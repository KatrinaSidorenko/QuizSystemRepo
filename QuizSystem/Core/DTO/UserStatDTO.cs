using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class UserStatDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int AttemptsAmount { get; set; }
        public double BestResult { get; set; }
        public double WorstResult { get; set; }
        public double AvgResult { get; set; }
    }
}
