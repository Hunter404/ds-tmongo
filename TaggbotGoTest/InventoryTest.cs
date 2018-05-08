using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaggbotGo;
using TaggbotGo.Items;

namespace TaggbotGoTest
{
	[TestClass]
	public class InventoryTest
	{
		[TestInitialize]
		public void InitalizeTest()
		{
			Debug.WriteLine(Program.ConnectionString);

			Player.Create("0000", "Hunter404", new Vector2(1, 1));
		}

		[TestCleanup]
		public void CleanupTest()
		{
			Player.Find("0000")?.Remove();
		}

		[TestMethod]
		public void LargeStackTest()
		{
			try
			{
				// ReSharper disable once ObjectCreationAsStatement
				new TaggmonContainer
				{
					Stack = 10
				};

				Assert.Fail("Stack is too big but didn't fail.");
			}
			catch (Exception)
			{
				// ignored
			}
		}

		[TestMethod]
		public void GetPlayerInventoryTest()
		{
			var player = Player.Find("0000");

			Assert.IsNotNull(player.Inventory);
		}

		[TestMethod]
		public void InventoryManipulationTest()
		{
			var player = Player.Find("0000");

			for (var i = player.Inventory.Items.Count - 1; i >= 0; i--)
			{
				var item = player.Inventory.Items[i];

				player.Inventory.RemoveItem(item);
			}

			player.Inventory.AddItem(new TaggmonContainer
			{
				Stack = 5
			});

			player.Inventory.AddItem(new TaggmonContainer
			{
				Stack = 5
			});

			Assert.IsNotNull(player.Inventory?.Items);
			
			Assert.AreEqual(5, player.Inventory.Items[0].Stack);
			Assert.AreEqual(5, player.Inventory.Items[1].Stack);

			player.Inventory.Save();

			player = Player.Find("0000");

			Assert.AreEqual(5, player.Inventory.Items[0].Stack);
			Assert.AreEqual(5, player.Inventory.Items[1].Stack);

			player.Inventory.RemoveItem(player.Inventory.Items[0]);
			player.Inventory.RemoveItem(player.Inventory.Items[0]);

			player.Save();

			player = Player.Find("0000");

			Assert.AreEqual(0, player.Inventory.Items.Count);
		}
	}
}
