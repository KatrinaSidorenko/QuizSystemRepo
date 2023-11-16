using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Enums
{
    public  static class SortingDictionnary
    {
        public static  Dictionary<SortingParam, (string prop, string order)> SortingValues = new()
        {
            {SortingParam.Name, ("test_name", "asc") },
            {SortingParam.NameDesc, ("test_name", "desc") },
            {SortingParam.Date, ("date_of_creation", "asc") },
            {SortingParam.DateDesc, ("date_of_creation", "desc") },
            {SortingParam.StartDate, ("start_date", "asc") },
            {SortingParam.StartDateDesc, ("start_date", "desc") },
            {SortingParam.EndDate, ("end_date", "asc") },
            {SortingParam.EndDateDesc, ("end_date", "desc") },
            {SortingParam.AttemptCount, ("attempt_count", "asc") },
            {SortingParam.AttemptCountDesc, ("attempt_count", "desc") },
            {SortingParam.AttemptDuration, ("attempt_duration", "asc") },
            {SortingParam.AttemptDurationDesc, ("attempt_duration", "desc") },
            {SortingParam.Points, ("points", "asc") },
            {SortingParam.PointsDesc, ("points", "desc") },
             {SortingParam.RightAnswers, ("right_answers_amount", "asc") },
            {SortingParam.RightAnswersDesc, ("right_answers_amount", "desc") }
        };
    }
}
