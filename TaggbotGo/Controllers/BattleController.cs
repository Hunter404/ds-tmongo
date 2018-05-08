using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaggbotGo.Items;
using TaggbotGo.Taggmons;
using TaggbotGo.Taggmons.Attacks;

namespace TaggbotGo.Controllers
{
	public class BattlePlayerModel
	{
		public Player Player { get; set; }
		public Taggmon Taggmon { get; set; }
	}

	public class BattleController
	{
		public List<BattlePlayerModel> Players { get; set; }

		public BattlePlayerModel CurrentPlayer => Players[_turn];

		private int _turn;

		public void Init()
		{
			OnPreviousPlayerDone();
		}

		public void OnPlayerUseItem(Item item)
		{
			OnPreviousPlayerDone();
		}

		public void OnPlayerAttack(Attack attack)
		{
			OnPreviousPlayerDone();
		}

		public void OnPreviousPlayerDone()
		{
			_turn++;

			if (_turn > Players.Count) _turn = 0;

			var player = Players[_turn];

			Debug.WriteLine($"{player.Player.Name}'s turn");
		}
	}
}
