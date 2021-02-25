using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlinePD.Models;

namespace OnlinePD.Controllers.HandHistory
{
    public class HandHistoryService
    {

        private readonly HandDatabase Db = new HandDatabase();

        public void UploadHandHistoryFileToDatabase(string user, string filepath)
        {

            string rawLines = File.ReadAllText(filepath);
            IList<string> rawHands = rawLines.Split("\r\n\r\n\r\n"); // split at double line break
            IList<Hand> hands = rawHands.Select(hand => Hand.Parse(hand.Split("\n"))).ToList(); // split hand history into a string array for each line and construct Hand method

            // Upload to database
            this.Db.Put(user, hands);

            // Delete temporary file now that it is uploaded to database
            //File.Delete(filepath);
        }

        public IList<Hand> GetHandsByUser(string user)
        {
            return Db.Get(user);
        }

    }
}
