using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Picture
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }
        public int Likes { get; set; }
    }
}
