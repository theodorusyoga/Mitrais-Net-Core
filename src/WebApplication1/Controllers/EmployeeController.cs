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
            int id = GetUserIdFromJWT();

            var result = _context.Pictures.Where(p => p.OwnerID == id).ToList();
            var obj = new List<object>();
            foreach (var item in result)
            {
                obj.Add(new
                {
                    id = item.ID,
                    src = item.Source,
                    desc = item.Description,
                    likes = item.Likes,
                    comments_amt = _context.Comments.Where(p => p.PictureID == item.ID).Count(),
                    liked = _context.UserLikes.Where(p => p.PictureID == item.ID && p.UserID == id).Count() > 0
                });
            }
            return obj;
        }

        [AllowCrossSite]
        [HttpPost("/api/picture")]
        [Authorize(Policy = "User")]
        public object GetPicture(Picture picture)
        {
            int id = GetUserIdFromJWT();
            var item = _context.Pictures.Where(p => p.ID == picture.ID).Single();

            var comments = _context.Comments.Where(p => p.PictureID == item.ID).OrderByDescending(p => p.Time);
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
                liked = _context.UserLikes.Where(p => p.PictureID == item.ID && p.UserID == id).Count() > 0,
                comments_amt = _context.Comments.Where(p => p.PictureID == item.ID).Count(),
                comments = comments_result
            };

        }


        [AllowCrossSite]
        [HttpPost("/api/pictures/like")]
        [Authorize(Policy = "User")]
        public object LikePictures(UserLike like)
        {
            int id = GetUserIdFromJWT();
            var select = _context.Pictures.Where(p => p.ID == like.PictureID).Single();
            select.Likes += 1;
            like.UserID = id;
            _context.UserLikes.Add(like);
            _context.SaveChanges();
            return new
            {
                status = 0
            };
        }

        [AllowCrossSite]
        [HttpPost("/api/pictures/unlike")]
        [Authorize(Policy = "User")]
        public object UnlikePictures(UserLike like)
        {
            int id = GetUserIdFromJWT();
            var select = _context.Pictures.Where(p => p.ID == like.PictureID).Single();
            select.Likes -= 1;

            var userlike = _context.UserLikes.Where(p => p.UserID == id && p.PictureID == like.PictureID).Single();
            _context.UserLikes.Remove(userlike);
            _context.SaveChanges();

            return new
            {
                status = 0
            };
        }

        [AllowCrossSite]
        [HttpPost("/api/comments")]
        [Authorize(Policy = "User")]
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

        [AllowCrossSite]
        [HttpPost("/api/comments/delete")]
        [Authorize(Policy = "User")]
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

        [AllowCrossSite]
        [HttpPost("/api/comments/update")]
        [Authorize(Policy = "User")]
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

        [AllowCrossSite]
        [HttpPost("/api/comments/create")]
        [Authorize(Policy = "User")]
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

        [AllowCrossSite]
        [HttpGet("/blog/getposts")]
        [Authorize(Policy = "User")]
        public object GetPosts()
        {
            var posts = _context.Posts.OrderByDescending(p => p.ID);
            var result = new List<object>();
            foreach (var post in posts)
            {
                var obj = new
                {
                    id = post.ID,
                    title = post.Title,
                    content = post.Content,
                    media = post.Media,
                    tags = post.Tags,
                    time = post.Time.ToString("dd MMMM yyyy HH:mm:ss")
                };
                result.Add(obj);
            }
            return result;
        }

        [AllowCrossSite]
        [HttpPost("/blog/get")]
        [Authorize(Policy = "User")]
        public object GetSinglePost(Post data)
        {
            var post = _context.Posts.Where(p => p.ID == data.ID).Single();

            return new
            {
                id = post.ID,
                title = post.Title,
                content = post.Content,
                media = post.Media,
                tags = post.Tags,
                time = post.Time.ToString("dd MMMM yyyy HH:mm:ss")
            };
        }

        [AllowCrossSite]
        [HttpPost("/blog/create")]
        [Authorize(Policy = "User")]
        public object CreatePost(Post post)
        {
            post.Time = DateTime.Now;
            _context.Posts.Add(post);
            _context.SaveChanges();
            return new
            {
                status = 0,
                id = post.ID,
                title = post.Title,
                content = post.Content,
                media = post.Media,
                tags = post.Tags,
                time = post.Time.ToString("dd MMMM yyyy HH:mm:ss")
            };
        }

        private int GetUserIdFromJWT()
        {
            StringValues auth = string.Empty;
            if (this.HttpContext.Request.Headers.TryGetValue("Authorization", out auth))
            {
                string token = auth.ToString().Replace("Bearer", "").Trim();
                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
                int id = Convert.ToInt32(jwt.Claims.Where(p => p.Type == "nameid").Single().Value);
                return id;
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
