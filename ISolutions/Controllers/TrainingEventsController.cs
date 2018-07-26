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

namespace ISolutions.Portal.Controllers
{
    [Authorize(Users = "Admin")]
    public class TrainingEventsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TrainingEvents
        public async Task<ActionResult> Index()
        {
            var events = db.Events.Include(t => t.City).Include(t => t.Course);
            return View(await events.ToListAsync());
        }
        // GET: TrainingEvents
        public async Task<ActionResult> List()
        {
            var events = db.Events.Include(t => t.City).Include(t => t.Course);
            return View(await events.ToListAsync());
        }

        // GET: TrainingEvents/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingEvent trainingEvent = await db.Events.FindAsync(id);
            if (trainingEvent == null)
            {
                return HttpNotFound();
            }
            return View(trainingEvent);
        }

        // GET: TrainingEvents/Create
        public ActionResult Create()
        {
            ViewBag.Category = new SelectList(db.Categories, "Id", "Name");
            ViewBag.CityId = new SelectList(db.Cities, "Id", "Name");
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Title");
            return View();
        }

        // POST: TrainingEvents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Duration,Date,IsReady,CityId,CourseId")] TrainingEvent trainingEvent)
        {
            if (ModelState.IsValid)
            {
                db.Events.Add(trainingEvent);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CityId = new SelectList(db.Cities, "Id", "Name", trainingEvent.CityId);
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Title", trainingEvent.CourseId);
            return View(trainingEvent);
        }

        // GET: TrainingEvents/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingEvent trainingEvent = await db.Events.FindAsync(id);
            if (trainingEvent == null)
            {
                return HttpNotFound();
            }
            ViewBag.Category = new SelectList(db.Categories, "Id", "Name", trainingEvent.Course.CategoryId);
            ViewBag.CityId = new SelectList(db.Cities, "Id", "Name", trainingEvent.CityId);
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Title", trainingEvent.CourseId);
            return View(trainingEvent);
        }

        // POST: TrainingEvents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Duration,Date,IsReady,CityId,CourseId")] TrainingEvent trainingEvent)
        {
            if (ModelState.IsValid)
            {
                db.Entry(trainingEvent).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CityId = new SelectList(db.Cities, "Id", "Name", trainingEvent.CityId);
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Title", trainingEvent.CourseId);
            return View(trainingEvent);
        }

        // GET: TrainingEvents/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingEvent trainingEvent = await db.Events.FindAsync(id);
            if (trainingEvent == null)
            {
                return HttpNotFound();
            }
            return View(trainingEvent);
        }

        // POST: TrainingEvents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            TrainingEvent trainingEvent = await db.Events.FindAsync(id);
            db.Events.Remove(trainingEvent);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public ActionResult _courses(int categoryId = 0)
        {
            var courses = categoryId > 0 ? db.Courses.Where(c => c.CategoryId == categoryId) : db.Courses.AsQueryable();
            ViewBag.CourseId = new SelectList(courses, "Id", "Title");
            return View();
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
