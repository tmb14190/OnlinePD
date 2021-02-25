using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;

namespace OnlinePD.Controllers.HandHistory
{
    public class HandHistoryController : Controller
    {
        private readonly HandHistoryService handHistoryService = new HandHistoryService();

        public IActionResult UploadHandHistoryFileToDatabase(string user, string filepath)
        {
            try
            {
                this.handHistoryService.UploadHandHistoryFileToDatabase(user, filepath);
            }
            catch
            {
                return StatusCode(500, (new { status = "Failed", message = "Failed to upload to database" })); // Internal Server Error = 500
            }

            return Ok(new { status = "Success", message = "Successfully uploaded hand histories" }); // Ok = 200
        }

        public IActionResult GetHandsByUser(string user)
        {
            try
            {
                IList<Hand> hands = handHistoryService.GetHandsByUser(user);
                return Ok(new { status = "Success", body = hands });
            }
            catch
            {
                return StatusCode(404, (new { status = "Failed", message = "User has no hands" }));
            }
        }

/*      public async Task<IActionResult> ResultsGraph(string user, string returnurl = null)
        {

        }

        public async Task<IActionResult> GetStatistics(string user, List<string> stats, )
        {

        }*/
    }
}