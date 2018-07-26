using ISolutions.Portal.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ISolutions.Portal.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public async Task<ActionResult> Index()
        {
            var categories = await db.Categories.Where(c => c.IsReady).ToListAsync();
            ViewBag.category = new SelectList(categories, "Id", "Name");
            ViewBag.city = new SelectList(await db.Cities.Where(c => c.IsReady).ToListAsync(), "Id", "Name");
            var latest = new List<TrainingEvent>();
            var events = await db.Events.ToListAsync();
            var date = DateTime.Now.Date;
            foreach (var category in categories)
            {
                latest.AddRange(events.Where(e => e.Course.CategoryId == category.Id && e.Date >= date).OrderBy(e => e.Date).Take(3).ToArray());
            }
            ViewBag.latest = latest.OrderBy(e=>e.Date).ToList();
            var months = new List<SelectListItem>();
            for (int i = 1; i < 13; i++)
            {
                months.Add(new SelectListItem() { Text = System.Globalization.DateTimeFormatInfo.CurrentInfo.GetMonthName(i), Value = i.ToString() });
            }
            ViewBag.month = new SelectList(months, "Value", "Text");
            return View(await db.Slides.ToListAsync());
        }
        public ActionResult About()
        {
            //ParseCourses();
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [HttpPost]
        public ActionResult Send(string name, string email, string subject, string text)
        {

            MailMessage mail = new MailMessage();

            SmtpClient smtpServer = new SmtpClient(Properties.Settings.Default.smtpServer, Properties.Settings.Default.Port);
            smtpServer.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.Email, Properties.Settings.Default.Password);
            var valid = Regex.IsMatch(email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            if (valid)
            {
                mail.From = new MailAddress(email);
                mail.To.Add(Properties.Settings.Default.Email);
            }
            else
            {
                return Content("Please enter a vaild email address");
            }
            mail.Subject = subject;
            mail.Body = "Message from " + name + Environment.NewLine;
            mail.Body += text;
            try
            {
                smtpServer.Send(mail);
                return Content("Message sent successfully");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
        private DataSet GetDate(string fileLocation)
        {
            DataSet ds = new DataSet();
            string excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";

            //Create Connection to Excel work book and add oledb namespace
            using (var excelConnection = new OleDbConnection(excelConnectionString))
            {
                excelConnection.Open();
                var dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                var excelConnection1 = new OleDbConnection(excelConnectionString);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ds.Tables.Add(dt.Rows[i]["TABLE_NAME"].ToString().Replace("$", ""));
                    using (var dataAdapter = new OleDbDataAdapter(string.Format("Select * from [{0}];", dt.Rows[i]["TABLE_NAME"]), excelConnection1))
                    {
                        dataAdapter.Fill(ds.Tables[i]);
                    }
                }
            }
            return ds;
        }
        public void ParseCourses()
        {
            var courses = db.Courses.ToList();
            var cities = db.Cities.ToList();
            foreach (DataTable table in GetDate(@"G:\Documents\Requirements\Isolutions\2015 Training Plan (2).xlsx").Tables)
            {
                foreach (DataRow row in table.Rows)
                {
                    var code = int.Parse(row[0].ToString());
                    var date = Convert.ToDateTime(row[2].ToString().Remove(3, 3));
                    var TEvenet = new TrainingEvent()
                    {
                        CityId = cities.Single(c => c.Name == row[4].ToString()).Id,
                        CourseId = courses.Single(c => c.Code == code).Id,
                        Date = date,
                        IsReady = true,
                        Duration = int.Parse(row[3].ToString().Substring(0, 1))
                    };
                    db.Events.Add(TEvenet);
                }
            }
            //db.SaveChanges();
        }
    }
}