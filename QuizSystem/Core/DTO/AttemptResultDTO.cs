using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class AttemptResultDTO
    {
        public int TestId { get; set; }
        public int TakedTestUserId { get; set; }
        public int AttemptId { get; set; }
        public int SharedTestId { get; set; }
        public List<AnswerResultDTO> Answers { get; set; }
    }
}
