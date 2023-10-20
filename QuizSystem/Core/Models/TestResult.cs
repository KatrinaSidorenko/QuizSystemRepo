using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class TestResult
    {
        public int TestResultId { get; set; }
        public int Points { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TestId { get; set; }
        public int UserId { get; set; }
    }
}
