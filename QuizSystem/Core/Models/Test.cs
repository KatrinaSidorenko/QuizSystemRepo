using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Test
    {
        public int TestId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Visibility Visibility { get; set; }
        public DateTime DateOfCreation { get; set; }
        public int UserId { get; set; }
    }
}
