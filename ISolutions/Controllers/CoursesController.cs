using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ISolutions.Portal.Models;
using ISolutions.Portal.Helpers;

namespace ISolutions.Portal.Controllers
{
    [Authorize(Users = "Admin")]
    public class CoursesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ImageUploader uploader = new ImageUploader();

        // GET: Courses
        [AllowAnonymous]
        public async Task<ActionResult> Index(string category = "")
        {
            var date = DateTime.Now.Date;
            ViewBag.category = category != "" ? category : "0";
            ViewBag.Categories = await db.Categories.Where(c => c.IsReady).ToListAsync();
            ViewBag.Events = await db.Events.Include(e => e.Course).Where(e => e.Date >= date).ToListAsync();

            return View();
        }
        [AllowAnonymous]
        public async Task<ActionResult> _GetCourses(int category, string city, string month)
        {
            var courses = db.Events.Include(e => e.City).Include(c => c.Course);
            if (category != 0)
                courses = courses.Where(c => c.Course.CategoryId == category);
            if (city != "")
            {
                var City = int.Parse(city);
                courses = courses.Where(c => c.CityId == City);
            }
            if (month != "")
            {
                var Month = int.Parse(month);
                courses = courses.Where(c => c.Date.Month == Month && c.Date.Year == DateTime.Now.Year);
            }

            return View(await courses.ToListAsync());
        }
        [AllowAnonymous]
        public async Task<ActionResult> Search(string category = "", string city = "", string month = "")
        {
            ViewBag.category = category != "" ? category : "0";
            var categories = await db.Categories.Where(c => c.IsReady).ToListAsync();
            categories.Insert(0, new Category() { Name = "All", Id = 0 });
            ViewBag.Categories = categories;
            ViewBag.Cities = new SelectList(await db.Cities.Where(c => c.IsReady).ToListAsync(), "Id", "Name", city);


            var months = new List<SelectListItem>();
            for (int i = 1; i < 13; i++)
            {
                months.Add(new SelectListItem() { Text = System.Globalization.DateTimeFormatInfo.CurrentInfo.GetMonthName(i), Value = i.ToString() });
            }
            ViewBag.Months = new SelectList(months, "Value", "Text", month);

            return View();
        }

        //GET: Courses
        public async Task<ActionResult> List()
        {

            var courses = db.Courses.Include(c => c.Category);
            return View(await courses.ToListAsync());
        }

        // GET: Courses/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = await db.Courses.FindAsync(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // GET: Courses/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name");
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Code,Title,Image,CategoryId")] Course course)
        {
            if (ModelState.IsValid)
            {
                if(db.Courses.Any(c=>c.Code == course.Code))
                {
                    ModelState.AddModelError("Code", "Code already exist");
                    ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", course.CategoryId);
                    return View(course);
                }
                if (db.Courses.Any(c => c.Title == course.Title))
                {
                    ModelState.AddModelError("Title", "Title already exist");
                    ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", course.CategoryId);
                    return View(course);
                }
                db.Courses.Add(course);
                if (Request.Files["img"].FileName != "")
                    uploader.Upload(Request.Files["img"], Server.MapPath("~\\Images\\Courses\\" + Request.Files["img"].FileName));
                await db.SaveChangesAsync();
                if (Request.Files["outline"].FileName != "")
                    uploader.Upload(Request.Files["outline"], Server.MapPath("~\\Images\\Courses\\" + course.Id + ".pdf"));

                return RedirectToAction("List");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", course.CategoryId);
            return View(course);
        }

        // GET: Courses/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = await db.Courses.FindAsync(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", course.CategoryId);
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Code,Title,Image,CategoryId")] Course course)
        {
            if (ModelState.IsValid)
            {              
                if (Request.Files["img"].FileName != "")
                    uploader.Upload(Request.Files["img"], Server.MapPath("~\\Images\\Courses\\" + Request.Files["img"].FileName));
                if (Request.Files["outline"].FileName != "")
                    uploader.Upload(Request.Files["outline"], Server.MapPath("~\\Images\\Courses\\" + course.Id + ".pdf"));

                db.Entry(course).State = EntityState.Modified;

                await db.SaveChangesAsync();
                return RedirectToAction("List");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", course.CategoryId);
            return View(course);
        }

        // GET: Courses/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = await db.Courses.FindAsync(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Course course = await db.Courses.FindAsync(id);
            db.Courses.Remove(course);
            await db.SaveChangesAsync();
            return RedirectToAction("List");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
