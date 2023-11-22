using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Enums
{
    public static class FilterDictionary
    {
        public static Dictionary<FilterParam, (int start, int end)> FilterParamDict = new()
        {
            {FilterParam.from0to25, (0,25) },
            {FilterParam.from25to50, (25, 50) },
            {FilterParam.from50to75, (50, 75) },
            {FilterParam.from75to100, (75, 100) },
            {FilterParam.from0to100, (0, 100) }
        };
    }
}
