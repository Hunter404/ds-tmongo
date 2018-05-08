using System;
using System.Collections.Generic;
using System.Data.SQLite;
using TaggbotGo.Taggmons;

namespace TaggbotGo.Items
{
	public abstract class Item
	{
		private const string TableName = "inventory_item";
		private const string PrimaryKeyName = "id";

		protected static readonly string _namespace;
		protected static readonly Dictionary<string, Type> _metadata;

		public long Id { get; private set; }

		public long InventoryId { get; set; }

		public abstract string Name { get; }

		public abstract string Description { get; }

		public abstract string ItemId { get; }

		public int Stack
		{
			get => _stack;
			set
			{
				if (value > MaxStack) throw new Exception("New stack is too big");

				_dirty = true;
				_stack = value;
			}
		}

		public abstract int MaxStack { get; }

		private bool _dirty = true;
		private bool _exists;

		private int _stack;

		static Item()
		{
			_metadata = new Dictionary<string, Type>();
			_namespace = typeof(Item).Namespace;
		}

		public virtual bool Drop(Player activator)
		{
			return false;
			// Drop on world
		}

		public virtual bool Use(Player activator)
		{
			return false;
		}

		public virtual bool Use(Player activator, Taggmon target)
		{
			return false;
		}

		public void Save(long inventoryId)
		{
			if (inventoryId == 0) throw new Exception("Unknown ID 0");

			if (!_dirty) return;

			using (var conn = new SQLiteConnection(Program.ConnectionString))
			{
				conn.Open();

				var cmd = conn.CreateCommand();
				cmd.CommandText = _exists
					? $"UPDATE {TableName} SET stack=@stack, fk_inventory_id=@inventoryId WHERE {PrimaryKeyName} = @{PrimaryKeyName}"
					: $"INSERT INTO {TableName} (fk_inventory_id, item_id, stack) VALUES (@inventoryId, @itemId, @stack)";
				cmd.Parameters.Add(new SQLiteParameter("id", Id));
				cmd.Parameters.Add(new SQLiteParameter("inventoryId", inventoryId));
				cmd.Parameters.Add(new SQLiteParameter("itemId", ItemId));
				cmd.Parameters.Add(new SQLiteParameter("stack", Stack));

				cmd.ExecuteNonQuery();

				if (!_exists) Id = conn.LastInsertRowId;

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
		}

		public static Item CreateFromItemId(int id, string itemId, int stack = 1)
		{
			Type type;

			if (!_metadata.ContainsKey(itemId))
			{
				type = Type.GetType(_namespace + "." + itemId);

				_metadata[itemId] = type ?? throw new Exception($"Type of {itemId} couldn't be found.");
			}
			else
			{
				type = _metadata[itemId];
			}

			var item = (Item) Activator.CreateInstance(type);
			item.Id = id;
			item.Stack = Math.Min(stack, item.MaxStack);

			return item;
		}

		public static List<Item> FindInventoryItems(long inventoryId)
		{
			var items = new List<Item>();

			using (var conn = new SQLiteConnection(Program.ConnectionString))
			{
				conn.Open();

				var cmd = conn.CreateCommand();
				cmd.CommandText = "SELECT * FROM inventory_item WHERE fk_inventory_id = @inventoryId";
				cmd.Parameters.Add(new SQLiteParameter("inventoryId", inventoryId));

				var reader = cmd.ExecuteReader();

				while (reader.Read())
				{
					var id = 0;
					var itemId = "";
					var stack = 1;

					for (var i = 0; i < reader.FieldCount; i++)
					{
						switch (reader.GetName(i))
						{
							case "id":
								id = reader.GetInt32(i);
								break;
							case "fk_inventory_id":
								// stack = reader.GetInt32(i);
								break;
							case "item_id":
								itemId = reader.GetString(i);
								break;
							case "stack":
								stack = reader.GetInt32(i);
								break;
						}
					}

					var item = CreateFromItemId(id, itemId, stack);
					item._dirty = false;

					items.Add(item);
				}

				reader.Close();
				conn.Close();
			}

			return items;
		}
	}
}