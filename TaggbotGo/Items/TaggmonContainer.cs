using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaggbotGo.Items
{
	public class TaggmonContainer : Item
	{
		public const string Uid = "TaggmonContainer";

		public override string Name => "Taggmon Container";

		public override string Description => "Taggmon Container or TGC is a empty container for a taggmon.";

		public override string ItemId => Uid;

		public override int MaxStack => 5;

		public override bool Drop(Player activator)
		{
			// Put item in world chest
			return false;
		}

		public override bool Use(Player activator)
		{
			// Use in combat? Throw.
			Debug.WriteLine($"Consumed 1 {Name}");

			activator.Inventory.Consume(this);

			return true;
		}
	}
}
