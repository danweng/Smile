using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using iVoting.Models;
using PagedList;

namespace iVoting.Controllers
{
	public class VotingController : Controller
	{
		private VotingDBContext db = new VotingDBContext();

		// GET: /Voting/
		public ActionResult Index(int? page)
		{
			var votings = db.Votings.OrderByDescending(a => a.VotingDate).ThenBy(a => a.Gender).ToList(); //SqlHelper.GetVotings().ToList();
			var pageNumber = page ?? 1;
			var onePageOfVotings = votings.ToPagedList(pageNumber, 20);

			return View(onePageOfVotings);
		}

		// GET: /Voting/ThanksForVotiing
		public ActionResult ThanksForVoting()
		{
			return View();
		}

		// GET: /Voting/CommandExecution
		public ActionResult CommandExecution()
		{
			return View();
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
		public ActionResult Create([Bind(Include = "ID,Gender,Emotion")] VotingModel votingModel)
		{
			votingModel.VotingDate = DateTime.Today;
			if (ModelState.IsValid)
			{
				switch (votingModel.Emotion)
				{
					case Emotion.好生氣:
						votingModel.Upset++;
						break;
					case Emotion.好開心:
						votingModel.Happy++;
						break;
					case Emotion.很興奮:
						votingModel.Exciting++;
						break;
					case Emotion.很難過:
						votingModel.Sad++;
						break;
				}

				bool isFirstVoteToday = this.IsFirstVoteToday(votingModel);

				if (isFirstVoteToday)
				{
					//// 新增今天的第一筆投票
					this.InsertVoting(votingModel);
				}
				else
				{
					//// 更新當日的票數
					var record = db.Votings.FirstOrDefault(a => a.Gender == votingModel.Gender && a.VotingDate == DateTime.Today);
					record.Upset = record.Upset + votingModel.Upset;
					record.Happy = record.Happy + votingModel.Happy;
					record.Exciting = record.Exciting + votingModel.Exciting;
					record.Sad = record.Sad + votingModel.Sad;
					db.Votings.Attach(record);
					db.Entry(record).State = EntityState.Modified;
					db.SaveChanges();
					//SqlHelper.UpdateVotingById(record);
				}

				return View();
			}

			return View(votingModel);
		}

		/// <summary>
		/// 是否是今天第一次投票
		/// </summary>
		/// <param name="votingModel">The voting model.</param>
		/// <returns>True / False</returns>
		private bool IsFirstVoteToday(VotingModel votingModel)
		{
			var hasMaleRecord = db.Votings.Where(a => a.Gender == Gender.男生 && votingModel.VotingDate == a.VotingDate).Any();
			var hasFemaleRecord = db.Votings.Where(a => a.Gender == Gender.女生 && votingModel.VotingDate == a.VotingDate).Any();
			//var hasMaleRecord = SqlHelper.HasMaleRecord();
			//var hasFemaleRecord = SqlHelper.HasFemaleRecord();

			if (hasMaleRecord == false && votingModel.Gender == Gender.男生)
			{
				return true;
			}

			if (hasFemaleRecord == false && votingModel.Gender == Gender.女生)
			{
				return true;
			}

			return false;
		}

		// GET: /Voting/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			VotingModel votingmodel = db.Votings.Find(id); //SqlHelper.GetVotingById((int)id);
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
		public ActionResult Edit([Bind(Include = "ID,Gender,Exciting,Happy,Sad,Upset,VotingDate")] VotingModel votingmodel)
		{
			if (ModelState.IsValid)
			{
				db.Entry(votingmodel).State = EntityState.Modified;
				db.SaveChanges();
				//SqlHelper.UpdateVotingById(votingmodel);
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
			VotingModel votingmodel = db.Votings.Find(id); //SqlHelper.GetVotingById((int)id);
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
			VotingModel votingmodel = db.Votings.Find(id); //SqlHelper.GetVotingById((int)id);
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

		private void InsertVoting(VotingModel votingModel)
		{
			db.Votings.Add(votingModel);
			db.SaveChanges();
			//SqlHelper.InsertVoting(votingModel);
		}
	}
}
