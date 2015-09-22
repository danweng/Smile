using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using iVoting.Models;

namespace iVoting.Controllers
{
    public class VotingController : Controller
    {
        private VotingDBContext db = new VotingDBContext();

        // GET: /Voting/
        public ActionResult Index()
        {
            return View(db.Votings.ToList());
        }

        // GET: /Voting/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VotingModel votingmodel = db.Votings.Find(id);
            if (votingmodel == null)
            {
                return HttpNotFound();
            }
            return View(votingmodel);
        }

        // GET: /Voting/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Voting/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,Gender,Exciting,Happy,Sad,Upset")] VotingModel votingmodel)
        {
            if (ModelState.IsValid)
            {
                db.Votings.Add(votingmodel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(votingmodel);
        }

        // GET: /Voting/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VotingModel votingmodel = db.Votings.Find(id);
            if (votingmodel == null)
            {
                return HttpNotFound();
            }
            return View(votingmodel);
        }

        // POST: /Voting/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,Gender,Exciting,Happy,Sad,Upset")] VotingModel votingmodel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(votingmodel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(votingmodel);
        }

        // GET: /Voting/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VotingModel votingmodel = db.Votings.Find(id);
            if (votingmodel == null)
            {
                return HttpNotFound();
            }
            return View(votingmodel);
        }

        // POST: /Voting/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            VotingModel votingmodel = db.Votings.Find(id);
            db.Votings.Remove(votingmodel);
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
