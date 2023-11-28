using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class QuestionStatDTO
    {
        public string Description { get; set; }
        public QuestionType Type { get; set; }
        public int Point { get; set; }
        public int QuestionId { get; set; }
        //public double GetMaxPointProcent { get; set; }
        public List<AnswerStatDTO> Answers { get; set; }
    }
}
