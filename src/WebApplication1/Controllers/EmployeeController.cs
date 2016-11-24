using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;
using Microsoft.AspNetCore.Cors;
using System.IdentityModel.Tokens.Jwt;

namespace WebApplication1.Controllers
{
    public class EmployeeController : Controller
    {
        private ReduxDbContext _context;

        public EmployeeController(ReduxDbContext context)
        {
            _context = context;
        }

        [HttpGet("/todo")]
        public object GetTodos()
        {
            var result = _context.Todos.ToList();
            var obj = new List<object>();
            foreach (var item in result)
            {
                obj.Add(new
                {
                    id = item.ID,
                    text = item.Text
                });
            }
            return obj;
        }

        [HttpGet("/api")]
        public object GetAll()
        {
            var result = _context.Employees.ToList();
            var obj = new List<object>();
            foreach (var item in result)
            {
                obj.Add(new
                {
                    id = item.ID,
                    firstname = item.FirstName,
                    midname = item.MidName,
                    surname = item.LastName,
                    gender = item.Gender,
                    birth = item.BirthDate.ToString("MM/dd/yyyy")
                });
            }
            return obj;
        }


        [HttpPost("/get")]
        public object GetEmp(Employee emp)
        {
            return _context.Employees.Where(p => p.ID == emp.ID).Single();
        }

        [HttpPost("/delete")]
        public object DeleteEmp(Employee emp)
        {
            var select = _context.Employees.Where(p => p.ID == emp.ID).Single();
            _context.Employees.Remove(select);
            _context.SaveChanges();
            return new
            {
                status = 0
            };
        }

        [HttpPost("/create")]
        public object CreateEmp(Employee emp)
        {
            _context.Employees.Add(emp);
            _context.SaveChanges();
            return new
            {
                id = emp.ID,
                status = 0
            };
        }

        [HttpPost("/update")]
        public object UpdateEmp(Employee emp)
        {
            var select = _context.Employees.Where(p => p.ID == emp.ID).Single();
            select.BirthDate = emp.BirthDate;
            select.FirstName = emp.FirstName;
            select.Gender = emp.Gender;
            select.LastName = emp.LastName;
            select.MidName = emp.MidName;

            _context.SaveChanges();
            return new
            {
                id = emp.ID,
                status = 0
            };
        }

        [AllowCrossSite]
        [HttpGet("/api/pictures")]
        [Authorize(Policy = "User")]

        public object GetPictures()
        {
            DecryptJWT();

            var result = _context.Pictures.ToList();
            var obj = new List<object>();
            foreach (var item in result)
            {
                obj.Add(new
                {
                    id = item.ID,
                    src = item.Source,
                    desc = item.Description,
                    likes = item.Likes,
                    comments_amt = _context.Comments.Where(p => p.PictureID == item.ID).Count()
                });
            }
            return obj;
        }

        [HttpPost("/api/picture")]
        public object GetPicture(Picture picture)
        {
            var item = _context.Pictures.Where(p => p.ID == picture.ID).Single();

            var comments = _context.Comments.Where(p => p.PictureID == item.ID);
            var comments_result = new List<object>();
            foreach (var comment in comments)
            {
                comments_result.Add(new
                {
                    id = comment.ID,
                    name = comment.Name,
                    text = comment.Text,
                    pictureid = comment.PictureID,
                    time = comment.Time
                });
            }

            return new
            {
                id = item.ID,
                src = item.Source,
                desc = item.Description,
                likes = item.Likes,
                comments_amt = _context.Comments.Where(p => p.PictureID == item.ID).Count(),
                comments = comments_result
            };

        }

        [HttpPost("/api/pictures/like")]
        public object LikePictures(Picture picture)
        {
            var select = _context.Pictures.Where(p => p.ID == picture.ID).Single();
            select.Likes += 1;
            _context.SaveChanges();
            return new
            {
                status = 0
            };
        }

        [HttpPost("/api/comments")]
        public object GetComments(Picture picture)
        {
            var select = _context.Comments.Where(p => p.PictureID == picture.ID);
            var result = new List<object>();
            foreach (var item in select)
            {
                result.Add(new
                {
                    id = item.ID,
                    name = item.Name,
                    text = item.Text,
                    pictureid = item.PictureID,
                    time = item.Time
                });
            }
            return result;
        }

        [HttpPost("/api/comments/delete")]
        public object DeleteComment(Comment comment)
        {
            var select = _context.Comments.Where(p => p.ID == comment.ID).Single();
            _context.Comments.Remove(select);
            _context.SaveChanges();
            return new
            {
                status = 0
            };
        }

        [HttpPost("/api/comments/update")]
        public object UpdateComment(Comment comment)
        {
            var select = _context.Comments.Where(p => p.ID == comment.ID).Single();
            select.Text = comment.Text;
            _context.SaveChanges();
            return new
            {
                status = 0
            };
        }

        [HttpPost("/api/comments/create")]
        public object CreateComment(Comment comment)
        {
            comment.Time = DateTime.Now;
            _context.Comments.Add(comment);
            _context.SaveChanges();
            return new
            {
                id = comment.ID,
                text = comment.Text,
                name = comment.Name,
                time = comment.Time,
                status = 0
            };
        }

        private int DecryptJWT()
        {
            StringValues auth = string.Empty;
            if (this.HttpContext.Request.Headers.TryGetValue("Authorization", out auth))
            {
                string token = auth.ToString().Replace("Bearer", "").Trim();
                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

            }
            return 0;
        }



        public class AllowCrossSiteAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext context)
            {
                context.HttpContext.Response.Headers.Add(new KeyValuePair<string, StringValues>("Access-Control-Allow-Headers", "Authorization"));
                context.HttpContext.Response.Headers.Add(new KeyValuePair<string, StringValues>("Access-Control-Allow-Methods", "GET,PUT,POST,DELETE,OPTIONS"));
                // context.HttpContext.Response.Headers.Add(new KeyValuePair<string, StringValues>("Access-Control-Allow-Origin", "http://localhost:8080"));
                base.OnActionExecuting(context);
            }
        }
    }
}
