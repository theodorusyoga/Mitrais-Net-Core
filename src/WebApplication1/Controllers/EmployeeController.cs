using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [AllowCrossSite]
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

        [HttpGet("/api/pictures")]
        public object GetPictures()
        {
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

            return new
            {
                id = item.ID,
                src = item.Source,
                desc = item.Description,
                likes = item.Likes,
                comments_amt = _context.Comments.Where(p => p.PictureID == item.ID).Count()

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
    }



    public class AllowCrossSiteAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Response.Headers.Add(new KeyValuePair<string, StringValues>("Access-Control-Allow-Origin", "*"));
            context.HttpContext.Response.Headers.Add(new KeyValuePair<string, StringValues>("Access-Control-Allow-Headers", "Content-Type"));
            base.OnActionExecuting(context);
        }
    }
}
