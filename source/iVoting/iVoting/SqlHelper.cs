using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using iVoting.Models;

namespace iVoting
{
	public static class SqlHelper
	{
		private static string connectionString = ConfigurationManager.ConnectionStrings["VotingConnection"].ConnectionString;

		public static VotingModel GetFirstVoteToday(Gender gender)
		{
			var sql = string.Format(@"Select top 1 * from VotingModels Where Gender={0} and VotingDate=#{1}#", (int)gender, DateTime.Today.ToString("MM/dd/yyyy"));
			var dt = GetDataTable(sql);
			var model = GetVotingById(int.Parse(dt.Rows[0]["ID"].ToString()));

			return model;
		}

		public static bool HasMaleRecord()
		{
			var sql = string.Format(@"Select top 1 * from VotingModels Where Gender=0 and VotingDate=#{0}#", DateTime.Today.ToString("MM/dd/yyyy"));
			var dt = GetDataTable(sql);

			return dt.Rows.Count == 1;
		}

		public static bool HasFemaleRecord()
		{
            var sql = string.Format(@"Select * from VotingModels Where Gender=1 and VotingDate=#{0}#", DateTime.Today.ToString("MM/dd/yyyy"));
			var dt = GetDataTable(sql);

			return dt.Rows.Count == 1;
		}

		public static void InsertVoting(VotingModel votingModel)
		{
			var sql = string.Format(@"Insert into VotingModels (Gender, Emotion, Exciting, Happy, Sad, Upset, VotingDate)  values({0},{1},{2},{3},{4},{5},'{6}')",
				(int)votingModel.Gender,
				(int)votingModel.Emotion,
				votingModel.Exciting,
				votingModel.Happy,
				votingModel.Sad,
				votingModel.Upset,
				votingModel.VotingDate);

			bool result = ExecuteSql(sql);
		}

		public static void UpdateVotingById(VotingModel votingModel)
		{
			var sql = string.Format(@"Update VotingModels set Exciting={0}, Happy={1}, Sad={2}, Upset={3} where ID={4}",
				votingModel.Exciting,
				votingModel.Happy,
				votingModel.Sad,
				votingModel.Upset,
				votingModel.ID);

			bool result = ExecuteSql(sql);
		}
		public static IEnumerable<VotingModel> GetVotings()
		{
			var models = new List<VotingModel>();
			var sql = @"Select * from VotingModels  order by VotingDate desc, Gender";
			var dt = GetDataTable(sql);

			foreach (DataRow row in dt.Rows)
			{
				models.Add(new VotingModel
				{
					ID = int.Parse(row["ID"].ToString()),
					Exciting = int.Parse(row["Exciting"].ToString()),
					Gender = (Gender)Enum.Parse(typeof(Gender), row["Gender"].ToString()),
					Happy = int.Parse(row["Happy"].ToString()),
					Sad = int.Parse(row["Sad"].ToString()),
					Upset = int.Parse(row["Upset"].ToString()),
					VotingDate = DateTime.Parse(row["VotingDate"].ToString()),
				});
			}

			return models;
		}

		public static VotingModel GetVotingById(int id)
		{
			var model = new VotingModel();
			var sql = string.Format(@"Select top 1 * from VotingModels  where ID={0}", id);
			var dt = GetDataTable(sql);

			foreach (DataRow row in dt.Rows)
			{
				model.ID = int.Parse(row["ID"].ToString());
				model.Exciting = int.Parse(row["Exciting"].ToString());
				model.Gender = (Gender)Enum.Parse(typeof(Gender), row["Gender"].ToString());
				model.Happy = int.Parse(row["Happy"].ToString());
				model.Sad = int.Parse(row["Sad"].ToString());
				model.Upset = int.Parse(row["Upset"].ToString());
				model.VotingDate = DateTime.Parse(row["VotingDate"].ToString());
			}

			return model;
		}

		private static DataTable GetDataTable(string sql)
		{
			var dt = new DataTable();
			using (OleDbConnection connection = new OleDbConnection(connectionString))
			{
				using (OleDbCommand command = new OleDbCommand(sql, connection))
				{
					using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
					{
						connection.Open();
						adapter.Fill(dt);
						connection.Close();
					}
				}
			}

			return dt;
		}

		private static bool ExecuteSql(string sql)
		{
			int effectedRows = 0;
			using (OleDbConnection connection = new OleDbConnection(connectionString))
			{
				using (OleDbCommand command = new OleDbCommand(sql, connection))
				{
					connection.Open();
					effectedRows = command.ExecuteNonQuery();
					connection.Close();
				}
			}

			return effectedRows == 1;
		}
	}
}