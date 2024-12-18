using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace THEBADDEST.LeaderBoardModule
{


	public class LeaderBoardItem : MonoBehaviour
	{

		[SerializeField] TextMeshProUGUI position;
		[SerializeField] TextMeshProUGUI userName;
		[SerializeField] TextMeshProUGUI score;
		[SerializeField] Image           backgroundImage;
		[SerializeField] Color           firstColor, secondColor, thirdColor, normalColor;

		public void SetData(LeaderBoardRecord leaderBoardRecord, int position)
		{
			this.position.text    = position.ToString();
			this.userName.text    = leaderBoardRecord.userName;
			this.score.text       = leaderBoardRecord.score.ToString();
			backgroundImage.color = position == 1 ? firstColor : position == 2 ? secondColor : position == 3 ? thirdColor : normalColor;
		}

	}


}