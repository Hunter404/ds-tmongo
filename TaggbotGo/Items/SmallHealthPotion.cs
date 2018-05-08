using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaggbotGo.Taggmons;

namespace TaggbotGo.Items
{
	public class SmallHealthPotion : Item
	{
		public const string Uid = "SmallHealthPotion";

		public override string Name { get; } = "Small Health Potion";

		public override string Description { get; } = "Refills 25% of taggmon's health.";

		public override string ItemId => Uid;

		public override int MaxStack { get; } = 10;

		public override bool Use(Player activator)
		{
			return false;
		}

		public override bool Use(Player activator, Taggmon target)
		{
			if (Math.Abs(target.Health - target.MaxHealth) < float.Epsilon) return false;

			target.Health += target.MaxHealth * 0.25f;

			return true;
		}
	}
}
