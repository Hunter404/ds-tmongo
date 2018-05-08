using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompareKvp = System.Collections.Generic.KeyValuePair<TaggbotGo.Taggmons.ElementType, TaggbotGo.Taggmons.ElementType>;
using Compare = System.Collections.Generic.Dictionary<System.Collections.Generic.KeyValuePair<TaggbotGo.Taggmons.ElementType, TaggbotGo.Taggmons.ElementType>, float>;

namespace TaggbotGo.Taggmons
{
	public static class ElementHelper
	{
		private static Compare _compare;

		static ElementHelper()
		{

			Populate();
		}

		private static void Populate()
		{
			if (_compare != null) return;
			_compare = new Compare();

			// Normal
			Add(ElementType.Normal, ElementType.Metal, Strength.Bad);
			Add(ElementType.Normal, ElementType.Ghost, Strength.VeryBad);

			// Fire
			Add(ElementType.Fire, ElementType.Fire, Strength.VeryBad);
			Add(ElementType.Fire, ElementType.Water, Strength.VeryBad);
			Add(ElementType.Fire, ElementType.Grass, Strength.VeryGood);
			Add(ElementType.Fire, ElementType.Cold, Strength.VeryGood);

			// Water
			Add(ElementType.Water, ElementType.Fire, Strength.VeryGood);
			Add(ElementType.Water, ElementType.Water, Strength.VeryBad);
			Add(ElementType.Water, ElementType.Grass, Strength.VeryBad);
			Add(ElementType.Water, ElementType.Cold, Strength.VeryGood);

			// Grass
			Add(ElementType.Grass, ElementType.Fire, Strength.VeryBad);
			Add(ElementType.Grass, ElementType.Water, Strength.VeryGood);
			Add(ElementType.Grass, ElementType.Grass, Strength.VeryBad);
			Add(ElementType.Grass, ElementType.Cold, Strength.VeryBad);

			// Cold
			Add(ElementType.Cold, ElementType.Fire, Strength.VeryBad);
			Add(ElementType.Cold, ElementType.Water, Strength.Bad);
			Add(ElementType.Cold, ElementType.Grass, Strength.Good);
			Add(ElementType.Cold, ElementType.Flying, Strength.Good);
			Add(ElementType.Cold, ElementType.Metal, Strength.VeryBad);

			// Psionic
			Add(ElementType.Psionic, ElementType.Venomous, Strength.VeryGood);
			Add(ElementType.Psionic, ElementType.Psionic, Strength.Bad);

			// Venomous
			Add(ElementType.Venomous, ElementType.Grass, Strength.Good);
			Add(ElementType.Venomous, ElementType.Venomous, Strength.Bad);
			Add(ElementType.Venomous, ElementType.Psionic, Strength.VeryBad);

			// Metal
			Add(ElementType.Metal, ElementType.Fire, Strength.Bad);
			Add(ElementType.Metal, ElementType.Water, Strength.Bad);
			Add(ElementType.Metal, ElementType.Cold, Strength.VeryGood);
			Add(ElementType.Metal, ElementType.Metal, Strength.VeryBad);

			// Ghost
			Add(ElementType.Ghost, ElementType.Normal, Strength.VeryBad);
			Add(ElementType.Ghost, ElementType.Ghost, Strength.VeryGood);
			Add(ElementType.Ghost, ElementType.Venomous, Strength.Bad);

			// Flying
			Add(ElementType.Flying, ElementType.Grass, Strength.VeryGood);
			Add(ElementType.Flying, ElementType.Metal, Strength.VeryBad);
		}

		private static void Add(ElementType a, ElementType b, Strength value)
		{
			if (value == Strength.Normal) return;

			const float veryBad = 0.6f;
			const float bad = 0.8f;
			const float @default = 1f;
			const float good = 1.2f;
			const float veryGood = 1.4f;

			var effect = @default;
			var inversedEffect = @default;
			switch (value)
			{
				case Strength.VeryBad:
					effect = veryBad;
					inversedEffect = veryGood;
					break;
				case Strength.Bad:
					effect = bad;
					inversedEffect = good;
					break;
				case Strength.Good:
					effect = good;
					inversedEffect = bad;
					break;
				case Strength.VeryGood:
					effect = veryGood;
					inversedEffect = veryBad;
					break;
			}

			_compare[new CompareKvp(a, b)] = effect;
			_compare[new CompareKvp(b, a)] = inversedEffect;
		}

		public static float Compare(ElementType a, ElementType b)
		{
			var key = new CompareKvp(a, b);
			if (_compare.ContainsKey(key))
				return _compare[key];

			return 1;
		}

		public enum Strength
		{
			VeryBad,
			Bad,
			Normal,
			Good,
			VeryGood,
		}
	}

	public enum ElementType
	{
		Normal,
		Water,
		Fire,
		Grass,
		Cold,
		Psionic,
		Venomous,
		Metal,
		Ghost,
		Flying,
	}
}
