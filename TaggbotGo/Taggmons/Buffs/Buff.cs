namespace TaggbotGo.Taggmons.Buffs
{
	public class Buff
	{
		public float MaxHealth { get; set; }
		public float MaxActionPoints { get; set; }
		
		public float Attack { get; set; }
		public float SpecialAttack { get; set; }
		
		public float Defense { get; set; }
		public float SpecialDefence { get; set; }

		public float Speed { get; set; }

		public int MaxTicks { get; set; }
		public int Ticks { get; set; }

		public Buff(int maxTicks)
		{
			MaxTicks = maxTicks;
		}

		public Buff(float maxHealth, float maxActionPoints, float attack, float specialAttack, float defense, float specialDefence, float speed, int maxTicks)
		{
			MaxHealth = maxHealth;
			MaxActionPoints = maxActionPoints;
			Attack = attack;
			SpecialAttack = specialAttack;
			Defense = defense;
			SpecialDefence = specialDefence;
			Speed = speed;
			MaxTicks = maxTicks;
		}

		public virtual void Spawn(Taggmon taggmon)
		{
			Ticks = MaxTicks;
		}

		public virtual void Update(Taggmon taggmon)
		{
			Ticks--;
		}

		public virtual void Remove(Taggmon tagmon)
		{

		}
	}
}
