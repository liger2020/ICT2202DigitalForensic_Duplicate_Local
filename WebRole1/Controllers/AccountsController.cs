using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebRole1.Models;

namespace WebRole1.Controllers
{
    public class AccountsController : Controller
    {
        private ICT2202DFEntities db = new ICT2202DFEntities();

        private SHA256 sha256 = SHA256.Create();

        /// <summary>
        /// GET: Accounts
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View(db.Accounts.ToList());
        }

        /// <summary>
        /// Accounts/Details/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        /// <summary>
        /// GET: Accounts/Login
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// POST: Accounts/Login (Action to Validate the Username and Password)
        /// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        /// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include = "Username, Password")] Account account)
        {
            if (ModelState.IsValid)
            {
                //Need to change this into LINQ Query (Need to Santize the Input to prevent SQL Injection)
                var verify = db.Accounts.SqlQuery("SELECT * FROM Account WHERE Username = @p0 AND Password = @p1;", account.Username, account.Password);
                if (verify.Count() != 0) //If failed Verfication
                {
                    List<Account> user = verify.ToList();
                    Session["Username"] = user[0].Username;
                    Session["First_Name"] = user[0].First_Name;
                }
                else
                {
                    return View(); //Need to understand how does (Return View() work)
                }
            }
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// GET: Accounts/Create
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// POST: Accounts/Create (Create Account for user)
        /// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        /// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idAccount,Username,Password,First_Name,Last_Name")] Account account)
        {
            if (ModelState.IsValid)
            {
                db.Accounts.Add(account);
                db.SaveChanges();
                return RedirectToAction("Login");
            }

            return View(account);
        }

        /// <summary>
        /// GET: Accounts/Edit/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idAccount,Username,Password,First_Name,Last_Name")] Account account)
        {
            if (ModelState.IsValid)
            {
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(account);
        }

        /// <summary>
        /// GET: Accounts/Delete/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        /// <summary>
        /// POST: Accounts/Delete/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Account account = db.Accounts.Find(id);
            db.Accounts.Remove(account);
            db.SaveChanges();
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

        /// <summary>
        /// Get IP of Computer
        /// </summary>
        /// <returns></returns>
        public string GetIP()
        {
            string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            return ip;
        }

        /// <summary>
        /// Password Hash Function
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private byte[] Hash_function(string password)
        {
            Byte[] byteArray = Encoding.UTF8.GetBytes(password);
            using (MemoryStream stream = new MemoryStream(byteArray))
            {
                return sha256.ComputeHash(stream);
            }
        }

        /// <summary>
        /// Change Byte to String (Hash)
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string BytesToString(byte[] bytes)
        {
            string result = "";
            foreach (byte b in bytes) result += b.ToString("X2");
            return result;
        }
    }
}
