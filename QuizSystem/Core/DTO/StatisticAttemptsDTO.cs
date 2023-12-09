using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class StatisticAttemptsDTO
    {
        public int AmountOfAttempts { get; set; }
        public double AverageTime { get; set; }
        public double AverageMark { get; set; }
        public DateTime FirstAttemptDate { get; set; }
        public DateTime LastAttemptDate { get; set; }
       // public double Accuracy { get; set; }    
    }
}
