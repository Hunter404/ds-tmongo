namespace TaggbotGo.Taggmons.Attacks
{
	public abstract class Attack
	{
		public abstract string Name { get; set; }

		public abstract int Cost { get; set; }

		public abstract void Perform(Taggmon caller, Taggmon target);
	}
}