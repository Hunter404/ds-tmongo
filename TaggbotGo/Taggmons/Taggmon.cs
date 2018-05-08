using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using TaggbotGo.Items;
using TaggbotGo.Taggmons.Attacks;
using TaggbotGo.Taggmons.Buffs;

namespace TaggbotGo.Taggmons
{
	public class Taggmon
	{
		public const int MaxLevel = 100;

		public int Id { get; } = 1;
		public int PlayerId { get; }

		public string Name { get; set; }
		public string TaggmonId { get; set; }

		public int Level => (int) Math.Min(Math.Max(Math.Floor(Math.Sqrt(Experience / 100f)), 1), 100);
		public int Experience { get; private set; }

		public float LevelFraction => Level / (float) MaxLevel;

		public float Health { get; set; }
		public float MaxHealth => _maxHealth.GetStat(LevelFraction);

		public float ActionPoints { get; set; }
		public float MaxActionPoints => _maxActionPoints.GetStat(LevelFraction);

		public float Attack => _attack.GetStat(LevelFraction);
		public float SpecialAttack => _specialAttack.GetStat(LevelFraction);

		public float Defense => _defense.GetStat(LevelFraction);
		public float SpecialDefence => _specialDefense.GetStat(LevelFraction);

		public float Speed => _speed.GetStat(LevelFraction);

		public List<Attack> Attacks { get; set; }
		public List<Buff> Buffs { get; set; }

		private Stat _specialDefense;
		private Stat _defense;
		private Stat _specialAttack;
		private Stat _attack;
		private Stat _maxActionPoints;
		private Stat _maxHealth;
		private Stat _speed;

		protected Taggmon(int id, string playerId, string name, string type, string taggmonId, int experience, int health, int actionPoints)
		{
			
		}

#if DEBUG
		public Taggmon(int minLevel, int maxLevel)
		{
			_specialDefense = new Stat(minLevel, maxLevel);
			_defense = new Stat(minLevel, maxLevel);
			_specialAttack = new Stat(minLevel, maxLevel);
			_attack = new Stat(minLevel, maxLevel);
			_maxActionPoints = new Stat(minLevel, maxLevel);
			_maxHealth = new Stat(minLevel, maxLevel);
			_speed = new Stat(minLevel, maxLevel);
		}
#endif

		public void TakeDamage(float damage, ElementType damageType)
		{

		}

		public void AddExperience(int xp)
		{
			var lvl = Level;

			Experience += xp;

			if (Level > lvl)
			{
				// LevelUp
			}
		}

		public float GetMaxHealthAfterBuffs()
		{
			var maxHealth = MaxHealth;

			return maxHealth + Buffs.Sum(x => maxHealth * x.MaxHealth);
		}

		public float GetMaxActionPointsAfterBuffs()
		{
			var maxAp = MaxActionPoints;

			return maxAp + Buffs.Sum(x => maxAp * x.MaxActionPoints);
		}

		public float GetAttackAfterBuffs()
		{
			var attack = Attack;

			return attack + Buffs.Sum(x => attack * x.Attack);
		}

		public float GetSpecialAttackAfterBuffs()
		{
			var specialAttack = SpecialAttack;

			return specialAttack + Buffs.Sum(x => specialAttack * x.SpecialAttack);
		}

		public float GetDefenseAfterBuffs()
		{
			var defense = Defense;

			return defense + Buffs.Sum(x => defense * x.Defense);
		}

		public float GetSpecialDefenseAfterBuffs()
		{
			var specialDefense = SpecialDefence;

			return specialDefense + Buffs.Sum(x => specialDefense * x.SpecialDefence);
		}

		public void AddBuff(Buff buff)
		{
			Buffs.Add(buff);
		}

		public static List<Taggmon> FindTaggmons(string playerId)
		{
			var taggmons = new List<Taggmon>();

			using (var conn = new SQLiteConnection(Program.ConnectionString))
			{
				conn.Open();

				var cmd = conn.CreateCommand();
				cmd.CommandText =
@"SELECT
	player_taggmons.id,
	player_taggmons.name AS `custom_name`,
	player_taggmons.fk_taggmon_id,
	taggmons.taggmon_id,
	taggmons.name,
	taggmons.type,
	player_taggmons.health,
	player_taggmons.action_points,
	player_taggmons.experience
FROM player_taggmons
LEFT OUTER JOIN taggmons ON taggmons.id = player_taggmons.fk_taggmon_id
WHERE player_taggmons.fk_player_id = @playerId";
				cmd.Parameters.Add(new SQLiteParameter("playerId", playerId));

				var reader = cmd.ExecuteReader();

				while (reader.Read())
				{
					var id = 0;
					var name = (string) null;
					var experience = 0;
					var actionPoints = 0;
					var taggmonId = (string) null;
					var type = (string) null;
					var health = 0;

					for (var i = 0; i < reader.FieldCount; i++)
					{
						switch (reader.GetName(i))
						{
							case "id":
								id = reader.GetInt32(i);
								break;
							case "name":
								name = name ?? reader.GetString(i);
								break;
							case "custom_name":
								var customName = reader.GetString(i);

								if (!string.IsNullOrEmpty(customName))
									name = customName;
								break;
							case "taggmon_id":
								taggmonId = reader.GetString(i);
								break;
							case "type":
								type = reader.GetString(i);
								break;
							case "experience":
								experience = reader.GetInt32(i);
								break;
							case "action_points":
								actionPoints = reader.GetInt32(i);
								break;
							case "health":
								health = reader.GetInt32(i);
								break;
						}
					}

					taggmons.Add(new Taggmon(id, name, playerId, type, taggmonId, experience, health, actionPoints));
				}

				reader.Close();
				conn.Close();
			}

			return taggmons;
		}
	}

	public class Stat
	{
		private readonly int _minValue;
		private readonly int _maxValue;

		public Stat(int minValue, int maxValue)
		{
			_minValue = minValue;
			_maxValue = maxValue;
		}

		public float GetStat(float fraction)
		{
			return Lerp(_minValue, _maxValue, fraction);
		}

		private float Lerp(int v0, int v1, float t)
		{
			return v0 + t * (v1 - v0);
		}
	}
}