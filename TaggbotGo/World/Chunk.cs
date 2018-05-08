namespace TaggbotGo.World
{
	public class Chunk
	{
		public int X;
		public int Y;

		public string PlayerId { get; set; }

		public char[,] Tiles;

		public Chunk(int x, int y, int size)
		{
			X = x;
			Y = y;

			Tiles = new char[size, size];
		}

		public Player GetOwner()
		{
			if (PlayerId == null) return null;
			return Player.Find(PlayerId);
		}

		public char[] Serialize()
		{
			var lx = Tiles.GetLength(0);
			var ly = Tiles.GetLength(1);

			var serialized = new char[lx * ly];
			
			for (var i = 0; i < lx; i++)
			{
				for (var j = 0; j < ly; j++)
				{
					serialized[(j * lx) + i] = Tiles[i, j];
				}
			}

			return serialized;
		}

		public void DeSerialize(char[] data)
		{
			var lx = Tiles.GetLength(0);
			var ly = Tiles.GetLength(1);

			for (var i = 0; i < lx; i++)
			{
				for (var j = 0; j < ly; j++)
				{
					Tiles[i, j] = data[(j * lx) + i];
				}
			}
		}
	}
}