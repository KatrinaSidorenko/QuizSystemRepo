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
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }
        public int AttemptId { get; set; }
    }
}
