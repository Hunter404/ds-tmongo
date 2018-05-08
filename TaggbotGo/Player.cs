using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Numerics;
using TaggbotGo.Items;
using TaggbotGo.Taggmons;

namespace TaggbotGo
{
	public class Player
	{
		private const string TableName = "players";
		private const string PrimaryKeyName = "id";

		public const int MaxTaggmons = 6;

		public string Name { get; }

		public string Id { get; }

		public long InventoryId => _inventory?.Id ?? _inventoryId;

		public Status Status { get; set; }

		public int Money { get; set; }

		public Vector2 Position { get; set; }

		public Inventory Inventory => _inventory = _inventory ?? Inventory.Find(InventoryId) ?? Inventory.Create();

		public List<Taggmon> Taggmons => _taggmons = _taggmons ?? Taggmon.FindTaggmons(Id);

		private Inventory _inventory;
		private List<Taggmon> _taggmons;

		// Can we save?
		private bool _dirty = true;
		private bool _exists;

		private int _inventoryId;

		protected Player(string name, string id, Vector2 position)
		{
			Name = name;
			Id = id;
			Position = position;
		}

		public void Save()
		{
			if (!_dirty) return;
			
			// Inventory gets loaded/created
			if (!_exists && Inventory == null) throw new NullReferenceException("Inventory failed to create automaticly");

			using (var conn = new SQLiteConnection(Program.ConnectionString))
			{
				conn.Open();

				var cmd = conn.CreateCommand();
				cmd.CommandText = _exists
					? $"UPDATE {TableName} SET name = @name, x = @x, y = @y, fk_inventory_id = @inventoryId WHERE {PrimaryKeyName} = @{PrimaryKeyName}"
					: $"INSERT INTO {TableName} ({PrimaryKeyName}, name, x, y, fk_inventory_id) VALUES (@{PrimaryKeyName}, @name, @x, @y, @inventoryId)";
				cmd.Parameters.Add(new SQLiteParameter(PrimaryKeyName, Id));
				cmd.Parameters.Add(new SQLiteParameter("name", Name));
				cmd.Parameters.Add(new SQLiteParameter("x", (int) Position.X));
				cmd.Parameters.Add(new SQLiteParameter("y", (int) Position.Y));
				cmd.Parameters.Add(new SQLiteParameter("inventoryId", InventoryId));

				cmd.ExecuteNonQuery();

				conn.Close();
			}

			_dirty = false;
			_exists = true;
		}

		public void Remove()
		{
			using (var conn = new SQLiteConnection(Program.ConnectionString))
			{
				conn.Open();

				var cmd = conn.CreateCommand();
				cmd.CommandText = $"DELETE FROM {TableName} WHERE {PrimaryKeyName} = @{PrimaryKeyName}";
				cmd.Parameters.Add(new SQLiteParameter(PrimaryKeyName, Id));

				cmd.ExecuteNonQuery();

				conn.Close();
			}

			Inventory.Find(_inventoryId)?.Remove();
		}

		public static Player Create(string playerId, string name, Vector2 position)
		{
			var player = new Player(name, playerId, position);
			player.Save();

			return player;
		}

		public static Player Find(string playerId)
		{
			var player = (Player) null;

			using (var conn = new SQLiteConnection(Program.ConnectionString))
			{
				conn.Open();

				var cmd = conn.CreateCommand();
				cmd.CommandText = $"SELECT * FROM {TableName} WHERE {PrimaryKeyName} = @{PrimaryKeyName} LIMIT 1";
				cmd.Parameters.Add(new SQLiteParameter(PrimaryKeyName, playerId));

				var reader = cmd.ExecuteReader();

				if (reader.Read())
				{
					var name = "";
					var id = "";
					var x = 0f;
					var y = 0f;
					var inventoryId = 0;

					for (var i = 0; i < reader.FieldCount; i++)
					{
						switch (reader.GetName(i))
						{
							case "id":
								id = reader.GetString(i);
								break;
							case "name":
								name = reader.GetString(i);
								break;
							case "x":
								x = reader.GetFloat(i);
								break;
							case "y":
								y = reader.GetFloat(i);
								break;
							case "fk_inventory_id":
								inventoryId = reader.GetInt32(i);
								break;
						}
					}

					player = new Player(name, id, new Vector2(x, y));
					player._inventoryId = inventoryId;
					player._dirty = false;
					player._exists = true;
				}

				reader.Close();
				conn.Close();
			}

			return player;
		}
	}

	public enum Status
	{
		Idle,
		Moving,
		Battle,
	}
}