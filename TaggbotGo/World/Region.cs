using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using LibNoise;
using LibNoise.Primitive;

namespace TaggbotGo.World
{
	public class Region
	{
		public const int ChunkSize = 32;

		public int Seed { get; }

		public int RegionSize { get; }

		public Dictionary<Tuple<int, int>, Chunk> Chunks { get; set; }

		private readonly ImprovedPerlin _perlin;

		public Region(int seed, int size = 4096)
		{
			Seed = seed;
			RegionSize = size;

			Chunks = new Dictionary<Tuple<int, int>, Chunk>();

			_perlin = new ImprovedPerlin(Seed, NoiseQuality.Best);
		}

		public Tuple<int, int> GetChunkPos(int x, int y)
		{
			var chunkPosX = Convert.ToInt32(x / ChunkSize) * ChunkSize;
			var chunkPosY = Convert.ToInt32(y / ChunkSize) * ChunkSize;
			return new Tuple<int, int>(chunkPosX, chunkPosY);
		}

		public void SetBlock(int x, int y, char block)
		{
			if (x < 0 || y < 0 || x > RegionSize || y > RegionSize) return;

			var chunk = CreateChunk(x, y);

			var localX = x - chunk.X;
			var localY = y - chunk.Y;

			chunk.Tiles[localX, localY] = block;
		}

		public char GetBlock(int x, int y)
		{
			if (x < 0 || y < 0 || x > RegionSize || y > RegionSize) return '\0';

			var chunk = CreateChunk(x, y);

			var localX = x - chunk.X;
			var localY = y - chunk.Y;

			var block = chunk.Tiles[localX, localY];

			if (block != '\0') return block;

			var perlin = GetPerlin(x, y);

			// Water
			if (perlin < -0.14f) return 'W';
			if (perlin < -0.05f) return 'S';
			if (perlin < 0.9f) return 'G';

			return 'R';
		}

		public float GetPerlin(int x, int y)
		{
			var scale = (new Vector2(x, y) - new Vector2(RegionSize / 2f, RegionSize / 2f)).Length() / RegionSize * 2f;

			// QUAD MACHINE
			scale = scale * scale * scale;
			scale = Math.Min(Math.Max(scale, 0f), 1f);

			var major = _perlin.GetValue(x / 1024f, 0f, y / 1024f);

			var normal = _perlin.GetValue(x / 128f, 0f, y / 128f) * 0.4f;

			var minor = _perlin.GetValue(x / 32f, 0f, y / 32f) * 0.1f;

			return (major + normal + minor) - scale;
		}

		public Chunk CreateChunk(int x, int y) => CreateChunk(GetChunkPos(x, y));

		public Chunk CreateChunk(Tuple<int, int> pos)
		{
			if (Chunks.ContainsKey(pos)) return Chunks[pos];

			var chunk = new Chunk(pos.Item1, pos.Item2, ChunkSize);

			Chunks[pos] = chunk;

			return chunk;
		}

		public char[,] RenderChunks(int originX, int originY, int width, int height)
		{
			var result = new char[width, height];

			for (var x = 0; x < width; x ++)
			{
				for (var y = 0; y < height; y ++)
				{
					result[x, y] = GetBlock(originX + x, originY + y);
				}
			}

			return result;
		}

		public Bitmap RenderChunks(int originX, int originY, int width, int height, int zoom)
		{
			var bitmap = new Bitmap(width / zoom, height / zoom);

			for (var x = 0; x < width; x += zoom)
			{
				for (var y = 0; y < height; y += zoom)
				{
					var color = Color.Black;
					switch (GetBlock(originX + x, originY + y))
					{
						case 'W':
							color = Color.DeepSkyBlue;
							break;
						case 'S':
							color = Color.Yellow;
							break;
						case 'G':
							color = Color.GreenYellow;
							break;
						case 'R':
							color = Color.Gray;
							break;
					}
/*
					var perlin = GetPerlin(originX + x, originY + y);
					var b = (int) (Math.Min(Math.Max(perlin, 0), 1) * 255f);

					var color = Color.FromArgb(255, b, b, b);
*/

					bitmap.SetPixel(x / zoom, y / zoom, color);
				}
			}

			return bitmap;
		}
	}
}
 