using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaggbotGo.Taggmons.Buffs
{
	public class Poison : Buff
	{
		private float _damage;

		public Poison(float damage, int maxTicks) : base(maxTicks)
		{
			_damage = damage;
		}

		public override void Update(Taggmon taggmon)
		{
			base.Update(taggmon);

			taggmon.Health -= _damage;
		}
	}
}
