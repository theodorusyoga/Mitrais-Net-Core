using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class UserLike
    {
        [Key]
        public int LikeID { get; set; }
        public int UserID { get; set; }
        public int PictureID { get; set; }
        public User User { get; set; }
        public Picture Picture { get; set; }
    }
}
