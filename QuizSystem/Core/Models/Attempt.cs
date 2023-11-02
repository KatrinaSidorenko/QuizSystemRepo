using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Attempt
    {
        public int AttemptId { get; set; }
        public int Points { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int SharedTestId { get; set; }
        public int RightAnswersAmount { get; set; }
        public int TestId { get; set; }
        public int UserId { get; set; }
    }
}
