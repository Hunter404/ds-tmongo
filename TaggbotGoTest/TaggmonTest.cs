using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaggbotGo.Taggmons;

namespace TaggbotGoTest
{
	[TestClass]
	public class TaggmonTest
	{
		private Taggmon _taggmon;

		[TestInitialize]
		public void TaggmonInitialize()
		{
			_taggmon = new Taggmon(1, 100);
		}

		[TestMethod]
		public void SpawnTaggmon()
		{

		}

		[TestMethod]
		public void ExperienceTest()
		{
			_taggmon.AddExperience(1000); // 1k

			Assert.AreEqual(3, _taggmon.Level);

			_taggmon.AddExperience(9000); // 10k

			Assert.AreEqual(10, _taggmon.Level);

			_taggmon.AddExperience(90000); // 100k

			Assert.AreEqual(31, _taggmon.Level);

			_taggmon.AddExperience(900000); // 1m

			Assert.AreEqual(100, _taggmon.Level);
		}

		[TestMethod]
		public void TypesDefenseDamageCsv()
		{
			var enumNames = Enum.GetNames(typeof(ElementType));
			var enumTypes = Enum.GetValues(typeof(ElementType)).Cast<ElementType>().ToArray();

			var content = "," + string.Join(",", enumNames) + "\n";

			for (var i = 0; i < enumNames.Length; i++)
			{
				var type = enumTypes[i];

				content += enumNames[i] + ",";

				var damages = new float[enumNames.Length];

				for (var j = 0; j < enumNames.Length; j++)
				{
					var type2 = enumTypes[j];

					damages[j] = ElementHelper.Compare(type, type2);
				}

				content += damages.Select(x => "\"" + x + "\"").Aggregate((a, b) => $"{a},{b}") + "\n";
			}

			Debug.WriteLine(content);
		}
	}
}
