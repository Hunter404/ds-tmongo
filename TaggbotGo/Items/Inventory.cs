using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Numerics;

namespace TaggbotGo.Items
{
	public class Inventory
	{
		private const string TableName = "inventory";
		private const string PrimaryKeyName = "id";

		public long Id { get; private set; }

		public Vector2 Position { get; set; }

		public int Slots { get; }

		public List<Item> Items { get; private set; }

		private bool _dirty = true;
		private bool _exists;

		public Inventory()
		{
			Slots = 6;

			Items = new List<Item>();
		}

		public bool AddItem(Item item)
		{
			if (Items.Count >= Slots) return false;

			var stackTarget = Items.Where(x => x.GetType() == item.GetType()).FirstOrDefault(i => i.Stack < i.MaxStack);
			if (stackTarget != null)
			{
				Stack(item, stackTarget);
			}
			else
			{
				Items.Add(item);
			}

			item.InventoryId = Id;

			return true;
		}

		public void RemoveItem(Item item)
		{
			Items.Remove(item);
			item.Remove();
		}

		public void Consume(Item item)
		{
			item.Stack--;

			if (item.Stack > 1) return;

			Items.Remove(item);
		}

		private void Stack(Item source, Item target)
		{
			var availableOnTarget = target.MaxStack - target.Stack;
			var amountToStack = Math.Min(availableOnTarget, source.Stack);

			target.Stack += amountToStack;
			source.Stack -= amountToStack;

			if (target.Stack == target.MaxStack && source.Stack > 0)
			{
				AddItem(source);
			}
		}

		public void Save()
		{
			if (_dirty)
			{
				using (var conn = new SQLiteConnection(Program.ConnectionString))
				{
					conn.Open();

					var cmd = conn.CreateCommand();
					cmd.CommandText = _exists
						? $"UPDATE {TableName} SET x = @x, y = @y WHERE {PrimaryKeyName} = @{PrimaryKeyName}"
						: $"INSERT INTO {TableName} (x, y) VALUES (@x, @y)";
					cmd.Parameters.Add(new SQLiteParameter("id", Id));
					cmd.Parameters.Add(new SQLiteParameter("x", (int) Position.X));
					cmd.Parameters.Add(new SQLiteParameter("y", (int) Position.Y));

					cmd.ExecuteNonQuery();

					if (!_exists) Id = conn.LastInsertRowId;

					conn.Close();
				}

				_dirty = false;
				_exists = true;
			}

			foreach (var item in Items)
			{
				item.Save(Id);
			}
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
		}

		public static Inventory Create()
		{
			var inventory = new Inventory();
			inventory.Save();

			return inventory;
		}

		public static Inventory Find(long inventoryId)
		{
			var inventory = (Inventory) null;

			using (var conn = new SQLiteConnection(Program.ConnectionString))
			{
				conn.Open();

				var cmd = conn.CreateCommand();
				cmd.CommandText = "SELECT * FROM inventory WHERE id = @inventoryId";
				cmd.Parameters.Add(new SQLiteParameter("inventoryId", inventoryId));

				var reader = cmd.ExecuteReader();

				while (reader.Read())
				{
					var id = 0;
					var x = 0;
					var y = 0;

					for (var i = 0; i < reader.FieldCount; i++)
					{
						switch (reader.GetName(i))
						{
							case "id":
								id = reader.GetInt32(i);
								break;
							case "x":
								x = reader.GetInt32(i);
								break;
							case "y":
								y = reader.GetInt32(i);
								break;
						}
					}

					inventory = new Inventory
					{
						Id = id,
						Position = new Vector2(x, y),
						_dirty = false,
						_exists = true
					};

				}

				reader.Close();
				conn.Close();
			}

			if (inventory != null && inventory.Id != 0)
				inventory.Items = Item.FindInventoryItems(inventoryId);

			return inventory;
		}
	}
}