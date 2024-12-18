using System.Linq;
using THEBADDEST.DatabaseModule;
using UnityEngine;


namespace THEBADDEST.LeaderBoardModule
{


	public class LeaderBoardPanel : MonoBehaviour
	{

		[SerializeField] string userName="Umair";
		[SerializeField] Transform contentHolder;
		[SerializeField] LeaderBoardItem userLeaderBoardItem;
		
		void Start()
		{
			var leaderBoardTable = DatabaseServiceLocator.DatabaseService().GetTable<LeaderBoardTable>();
			var leaderBoardRecords = leaderBoardTable.GetSortedLeaderBoard(10);
			for (int i = 0; i < leaderBoardRecords.Count; i++)
			{
				var leaderBoardRecord = leaderBoardRecords[i];
				var leaderBoardItem   = Instantiate(Resources.Load<LeaderBoardItem>("LeaderBoardItem"), contentHolder, false);
				leaderBoardItem.SetData(leaderBoardRecord,i+1);
			}

			
			var userLeaderBoardRecord   = leaderBoardTable.GetByKey(x => x.userName, userName);
			var userPosition = leaderBoardRecords.IndexOf(userLeaderBoardRecord) + 1;
			userLeaderBoardItem.SetData(userLeaderBoardRecord,userPosition);
		}
		
	}


}