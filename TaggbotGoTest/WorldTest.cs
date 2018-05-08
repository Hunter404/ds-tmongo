using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaggbotGo;
using TaggbotGo.World;

namespace TaggbotGoTest
{
	[TestClass]
	public class WorldTest
	{
		private Region _region;

		[TestInitialize]
		public void CreateWorld()
		{
			_region = new Region(2);
		}

		[TestMethod]
		public void SetBlockTest()
		{
			_region.SetBlock(555, 555, 'C');
		}

		[TestMethod]
		public void GetBlockTest()
		{
			SetBlockTest();

			Assert.IsTrue(_region.GetBlock(555, 555) == 'C');
		}

		[TestMethod]
		public void CreateChunkTest()
		{
			Assert.IsNotNull(_region.CreateChunk(555, 555));
		}

		[TestMethod]
		public void RenderChunksTest()
		{
			var originX = 32;
			var originY = 0;

			var width = 16;
			var height = 16;

			GenerateTestData(_region, originX, originY, width, height);

			var res = _region.RenderChunks(originX, originY, width, height);

			Assert.IsTrue(res.GetLength(0) == width);
			Assert.IsTrue(res.GetLength(1) == height);

			for (var x = 0; x < width; x++)
			{
				for (var y = 0; y < height; y++)
				{
					if (res[x, y] != '#') Assert.Fail("Expected #");

					Debug.Write(res[x, y]);
				}

				Debug.WriteLine("");
			}
		}

		[TestMethod]
		public void RenderPerlinTest()
		{
			var originX = 0;
			var originY = 0;

			var width = 4096;
			var height = 4096;

			var res = _region.RenderChunks(originX, originY, width, height, 8);

			var fullPath = Path.GetFullPath("C:\\Users\\frelar\\Desktop");

			res.Save(Path.Combine(fullPath, "out.png"), ImageFormat.Png);
			System.Diagnostics.Process.Start(@"C:\\Users\\frelar\\Desktop\\out.png");
		}

		public static void GenerateTestData(Region region, int startX, int startY, int width, int height)
		{
			for (var x = startX; x < startX + width; x++)
			{
				for (var y = startY; y < startY + height; y++)
				{
					region.SetBlock(x, y, '#');
				}
			}
		}
	}
}
