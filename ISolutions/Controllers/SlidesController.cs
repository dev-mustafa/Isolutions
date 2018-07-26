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
    public class SlidesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ImageUploader uploader = new ImageUploader();

        // GET: Slides
        public async Task<ActionResult> Index()
        {
            var slides = db.Slides.Include(s => s.Category);
            return View(await slides.ToListAsync());
        }

        // GET: Slides/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Slide slide = await db.Slides.FindAsync(id);
            if (slide == null)
            {
                return HttpNotFound();
            }
            return View(slide);
        }

        // GET: Slides/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name");
            return View();
        }

        // POST: Slides/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Title,Content,Image,CategoryId,TitleColor,ContentColor")] Slide slide)
        {
            if (ModelState.IsValid)
            {
                if (Request.Files[0].FileName != "")
                    uploader.Upload(Request.Files[0], Server.MapPath("~\\Images\\Slides\\" + Request.Files[0].FileName));
                db.Slides.Add(slide);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", slide.CategoryId);
            return View(slide);
        }

        // GET: Slides/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Slide slide = await db.Slides.FindAsync(id);
            if (slide == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", slide.CategoryId);
            return View(slide);
        }

        // POST: Slides/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Title,Content,Image,CategoryId,TitleColor,ContentColor")] Slide slide)
        {
            if (ModelState.IsValid)
            {
                if (Request.Files[0].FileName != "")
                    uploader.Upload(Request.Files[0], Server.MapPath("~\\Images\\Slides\\" + Request.Files[0].FileName));
                db.Entry(slide).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", slide.CategoryId);
            return View(slide);
        }

        // GET: Slides/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Slide slide = await db.Slides.FindAsync(id);
            if (slide == null)
            {
                return HttpNotFound();
            }
            return View(slide);
        }

        // POST: Slides/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Slide slide = await db.Slides.FindAsync(id);
            db.Slides.Remove(slide);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
