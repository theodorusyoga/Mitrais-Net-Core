using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Comment
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public int PictureID { get; set; }
        public Picture Picture { get; set; }
    }
}
