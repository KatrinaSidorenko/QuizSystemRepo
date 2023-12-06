using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class SharedTestStatDTO
    {
        public int TakenCountByUsers { get; set; }
        public double AvgPoints { get; set; }
        public double PassedUsersProcent { get; set; }
        public int AmmountOfAttempts { get; set; }
        public List<UserStatDTO> UsersStat { get; set; }
        public List<QuestionStatDTO> QuestionsStat { get; set; }
    }
}
