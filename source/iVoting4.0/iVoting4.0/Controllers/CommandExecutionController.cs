using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace iVoting.Controllers
{
    public class CommandExecutionController : Controller
    {
		[HttpGet]
		public ActionResult ExecuteConsole(string id)
        {
			this.ExecuteCommand(id);
            return View();
        }

		private void ExecuteCommand(string argument)
		{
			ProcessStartInfo ProcessInfo;
			Process Process;
			var commandLocation = ConfigurationManager.AppSettings["ConsoleCommand"];

			ProcessInfo = new ProcessStartInfo("cmd.exe", "/C " + Server.MapPath(commandLocation.ToString()) + " " + argument);
			ProcessInfo.CreateNoWindow = false;
			ProcessInfo.UseShellExecute = true;

			Process = Process.Start(ProcessInfo);
		}
    }
}