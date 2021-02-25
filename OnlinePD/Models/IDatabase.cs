using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlinePD.Controllers.HandHistory;

namespace OnlinePD.Models
{
    public interface IDatabase<T>
    {
        IList<T> Get(string user);
        void Put(string user, IList<T> data);
    }

    public class HandDatabase: IDatabase<Hand>
    {
        private IDictionary<string, List<Hand>> allHandHistories = new Dictionary<string, List<Hand>>();

        public IList<Hand> Get(string user)
        {
            if (allHandHistories.ContainsKey(user)) return this.allHandHistories[user];

            throw new Exception("User has no hands");
        }
        public void Put(string user, IList<Hand> handHistories)
        {
            if (allHandHistories.ContainsKey(user)) allHandHistories[user].AddRange(handHistories);
            else allHandHistories[user] = handHistories.ToList();
        }

    }
}
