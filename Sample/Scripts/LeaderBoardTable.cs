using System;
using System.Collections.Generic;
using UnityEngine;
using THEBADDEST.DatabaseModule;


namespace THEBADDEST.LeaderBoardModule
{


    [Serializable]
    public class LeaderBoardRecord
    {
        public string userName;
        public int score;
    }
    
    [CreateAssetMenu(menuName = "Database/Tables/LeaderBoardTable", fileName = "LeaderBoardTable")]
    public class LeaderBoardTable : Table<LeaderBoardRecord>
    {

        public IList<LeaderBoardRecord> GetSortedLeaderBoard(int upto)
        {
            var list = new List<LeaderBoardRecord>();
            for (int i = 0; i < Entries.Count; i++)
            {
                list.Add(Entries[i]);
            }
            list.Sort((a, b) => b.score.CompareTo(a.score));
            return list.GetRange(0, Math.Min(upto, list.Count));
        }

    }
    
}
