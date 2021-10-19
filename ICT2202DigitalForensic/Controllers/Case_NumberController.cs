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
    public class Case_NumberController : Controller
    {
        private ICT2202ProjectEntities db = new ICT2202ProjectEntities();

        // GET: Case_Number
        public ActionResult Index()
        {
            return View(db.Case_Number.ToList());
        }

        // GET: Case_Number/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Case_Number case_Number = db.Case_Number.Find(id);
            if (case_Number == null)
            {
                return HttpNotFound();
            }
            return View(case_Number);
        }

        // GET: Case_Number/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Case_Number/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idCase,Name,Hash")] Case_Number case_Number)
        {
            if (ModelState.IsValid)
            {
                db.Case_Number.Add(case_Number);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(case_Number);
        }

        // GET: Case_Number/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Case_Number case_Number = db.Case_Number.Find(id);
            if (case_Number == null)
            {
                return HttpNotFound();
            }
            return View(case_Number);
        }

        // POST: Case_Number/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idCase,Name,Hash")] Case_Number case_Number)
        {
            if (ModelState.IsValid)
            {
                db.Entry(case_Number).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(case_Number);
        }

        // GET: Case_Number/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Case_Number case_Number = db.Case_Number.Find(id);
            if (case_Number == null)
            {
                return HttpNotFound();
            }
            return View(case_Number);
        }

        // POST: Case_Number/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Case_Number case_Number = db.Case_Number.Find(id);
            db.Case_Number.Remove(case_Number);
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
