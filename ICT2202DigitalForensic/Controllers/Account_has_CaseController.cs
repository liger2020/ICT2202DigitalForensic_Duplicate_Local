using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ICT2202DigitalForensic.Models;

namespace ICT2202DigitalForensic.Controllers
{
    public class Account_has_CaseController : Controller
    {
        private ICT2202ProjectEntities db = new ICT2202ProjectEntities();

        // GET: Account_has_Case
        public ActionResult Index()
        {
            var account_has_Case = db.Account_has_Case.Include(a => a.Account).Include(a => a.Case_Number);
            return View(account_has_Case.ToList());
        }

        // GET: Account_has_Case/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account_has_Case account_has_Case = db.Account_has_Case.Find(id);
            if (account_has_Case == null)
            {
                return HttpNotFound();
            }
            return View(account_has_Case);
        }

        // GET: Account_has_Case/Create
        public ActionResult Create()
        {
            ViewBag.Account_idAccount = new SelectList(db.Accounts, "idAccount", "Username");
            ViewBag.Case_idCase = new SelectList(db.Case_Number, "idCase", "Name");
            return View();
        }

        // POST: Account_has_Case/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Account_idAccount,Case_idCase,Last_modified,Last_accessed")] Account_has_Case account_has_Case)
        {
            if (ModelState.IsValid)
            {
                db.Account_has_Case.Add(account_has_Case);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Account_idAccount = new SelectList(db.Accounts, "idAccount", "Username", account_has_Case.Account_idAccount);
            ViewBag.Case_idCase = new SelectList(db.Case_Number, "idCase", "Name", account_has_Case.Case_idCase);
            return View(account_has_Case);
        }

        // GET: Account_has_Case/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account_has_Case account_has_Case = db.Account_has_Case.Find(id);
            if (account_has_Case == null)
            {
                return HttpNotFound();
            }
            ViewBag.Account_idAccount = new SelectList(db.Accounts, "idAccount", "Username", account_has_Case.Account_idAccount);
            ViewBag.Case_idCase = new SelectList(db.Case_Number, "idCase", "Name", account_has_Case.Case_idCase);
            return View(account_has_Case);
        }

        // POST: Account_has_Case/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Account_idAccount,Case_idCase,Last_modified,Last_accessed")] Account_has_Case account_has_Case)
        {
            if (ModelState.IsValid)
            {
                db.Entry(account_has_Case).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Account_idAccount = new SelectList(db.Accounts, "idAccount", "Username", account_has_Case.Account_idAccount);
            ViewBag.Case_idCase = new SelectList(db.Case_Number, "idCase", "Name", account_has_Case.Case_idCase);
            return View(account_has_Case);
        }

        // GET: Account_has_Case/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account_has_Case account_has_Case = db.Account_has_Case.Find(id);
            if (account_has_Case == null)
            {
                return HttpNotFound();
            }
            return View(account_has_Case);
        }

        // POST: Account_has_Case/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Account_has_Case account_has_Case = db.Account_has_Case.Find(id);
            db.Account_has_Case.Remove(account_has_Case);
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
    }
}
