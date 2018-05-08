using System;
using System.Diagnostics;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaggbotGo;
using TaggbotGo.Items;

namespace TaggbotGoTest
{
	[TestClass]
	public class PlayerTest
	{
		[TestInitialize]
		public void InitializeTest()
		{
			Player.Create("0000", "Hunter404", new Vector2(1, 1));
		}

		[TestCleanup]
		public void CleanupTest()
		{
			Player.Find("0000").Remove();
		}

		[TestMethod]
		public void GetPlayerTest()
		{
			var player = Player.Find("0000");

			Assert.IsNotNull(player);
		}

		[TestMethod]
		public void SavePlayerTest()
		{
			var player = Player.Find("0000");

			player.Save();

			// Check if there's 2
		}

		[TestMethod]
		public void SpawnPlayerTest()
		{
			var player = Player.Find("0000");

			Assert.AreEqual("Hunter404", player.Name);
			Assert.AreEqual("0000", player.Id);
			Assert.AreEqual(new Vector2(1, 1), player.Position);
		}

		[TestMethod]
		public void MovePlayerTest()
		{
			var player = Player.Find("0000");
			
			Assert.AreEqual(new Vector2(1, 1), player.Position);
		}
	}
}
