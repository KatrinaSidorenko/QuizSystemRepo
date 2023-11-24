﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    public  class AttemptHistoryDTO
    {
        public int AttemptId { get; set; }
        public double Points { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int SharedTestId { get; set; }
        public int RightAnswersAmount { get; set; }
        public int TestId { get; set; }
        public int UserId { get; set; }
        public double Accuracy { get; set; }
    }
}
