using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaggbotGo;
using TaggbotGo.Controllers;
using TaggbotGo.Taggmons;

namespace TaggbotGoTest
{
	[TestClass]
	public class BattleTest
	{
		[TestMethod]
		public void TestThing()
		{
			var battle = new BattleController();

			battle.Players = new List<BattlePlayerModel>();

			battle.Players.Add(new BattlePlayerModel
			{
				Player = Player.Create("0000", "Hunter404", new Vector2(0, 0)),
				Taggmon = new Taggmon(1, 100)
			});
			battle.Players.Add(new BattlePlayerModel
			{
				Player = Player.Create("0001", "Douche", new Vector2(0, 0)),
				Taggmon = new Taggmon(1, 100)
			});

			battle.Init();

			battle.OnPlayerAttack(new );
		}
	}
}
