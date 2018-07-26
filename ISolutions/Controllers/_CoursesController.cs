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

namespace ISolutions.Controllers
{
    [Authorize(Users = "Admin")]
    public class _CoursesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [AllowAnonymous]
        // GET: Courses
        public async Task<ActionResult> Index(string category = "", string city = "", string month = "")
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
        [AllowAnonymous]
        //public async Task<ActionResult> _GetCourses(int category, string city, string month)
        //{
        //    var courses = db.Courses.Include(c => c.Category).Include(c => c.City);
        //    if (category != 0)
        //        courses = courses.Where(c => c.CategoryId == category);
        //    if (city != "")
        //    {
        //        var City = int.Parse(city);
        //        courses = courses.Where(c => c.CityId == City);
        //    }
        //    if (month != "")
        //    {
        //        var Month = int.Parse(month);
        //        courses = courses.Where(c => c.Date.Month == Month && c.Date.Year == DateTime.Now.Year);
        //    }

        //    return View(await courses.ToListAsync());
        //}
        // GET: Courses
        //public async Task<ActionResult> List()
        //{

        //    var courses = db.Courses.Include(c => c.Category).Include(c => c.City);
        //    return View(await courses.ToListAsync());
        //}

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
            ViewBag.CityId = new SelectList(db.Cities, "Id", "Name");
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Code,Title,Duration,Date,Description,CityId,CategoryId,IsReady")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Courses.Add(course);
                await db.SaveChangesAsync();
                return RedirectToAction("List");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", course.CategoryId);
            //ViewBag.CityId = new SelectList(db.Cities, "Id", "Name", course.CityId);
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
            //ViewBag.CityId = new SelectList(db.Cities, "Id", "Name", course.CityId);
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Code,Title,Duration,Date,Description,CityId,CategoryId,IsReady")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Entry(course).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("List");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", course.CategoryId);
           // ViewBag.CityId = new SelectList(db.Cities, "Id", "Name", course.CityId);
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
