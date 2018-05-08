using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace TaggbotGo.Commands
{
	public class Help : Command
	{
		private static string _helpText;
		
		public override string Text => "help";

		public override string Description => "Prints help";

		public override void Run()
		{
			if (_helpText == null)
			{
				var commands = Assembly
					.GetExecutingAssembly()
					.GetTypes()
					.Where(x => typeof(Command).IsAssignableFrom(x) && !x.IsAbstract)
					.Select(x => Activator.CreateInstance(x) as Command)
					.Where(x => x != null)
					.Select(x => $"{x.Text} - {x.Description}")
					.Aggregate((x, y) => $"{x}\n{y}");

				_helpText = "Available commands:\n" + commands;
			}

			Debug.WriteLine(_helpText);
		}
	}
}
