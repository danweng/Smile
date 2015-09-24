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
        public ActionResult Create([Bind(Include = "ID,Gender,Emotion")] VotingModel votingmodel)
        {
            votingmodel.Today = DateTime.Today;
            if (ModelState.IsValid)
            {
                switch (votingmodel.Emotion)
                {
                    case Emotion.好生氣:
                        votingmodel.Upset++;
                        break;
                    case Emotion.好開心:
                        votingmodel.Happy++;
                        break;
                    case Emotion.很興奮:
                        votingmodel.Exciting++;
                        break;
                    case Emotion.很難過:
                        votingmodel.Sad++;
                        break;
                }

                var hasMaleRecord = db.Votings.Where(a => a.Gender == Gender.男生 && votingmodel.Today == DateTime.Today).Any();
                var hasFemaleRecord = db.Votings.Where(a => a.Gender == Gender.女生 && votingmodel.Today == DateTime.Today).Any();
                if (hasMaleRecord ==false && votingmodel.Gender == Gender.男生)
                {
                    db.Votings.Add(votingmodel);
                    db.SaveChanges();
                }
                else if (hasFemaleRecord == false && votingmodel.Gender == Gender.女生)
                {
                    db.Votings.Add(votingmodel);
                    db.SaveChanges();
                }
                else
                {
                    var record = db.Votings.First(a=>a.Gender == votingmodel.Gender && a.Today == DateTime.Today);
                    record.Upset = record.Upset + votingmodel.Upset;
                    record.Happy = record.Happy + votingmodel.Happy;
                    record.Exciting = record.Exciting + votingmodel.Exciting;
                    record.Sad = record.Sad + votingmodel.Sad;
                    db.Votings.Attach(record);
                    db.Entry(record).State = EntityState.Modified;
                    db.SaveChanges();
                }

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
        public ActionResult Edit([Bind(Include = "ID,Gender,Exciting,Happy,Sad,Upset")] VotingModel votingmodel)
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
