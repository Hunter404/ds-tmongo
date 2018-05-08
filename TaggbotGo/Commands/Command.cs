using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace TaggbotGo.Commands
{
	public abstract class Command
	{
		private static Dictionary<string, Command> _commands;

		public abstract string Text { get; }
		
		public abstract string Description { get; }

		public abstract void Run();

		public static void Execute(string command)
		{
			if (_commands == null) _commands = GetAllCommands();

			Debug.WriteLine(_commands.Count);

			if (!_commands.ContainsKey(command)) return;

			_commands[command].Run();
		}

		private static Dictionary<string, Command> GetAllCommands()
		{
			return Assembly
				.GetExecutingAssembly()
				.GetTypes()
				.Where(x => typeof(Command).IsAssignableFrom(x) && !x.IsAbstract)
				.Select(x => Activator.CreateInstance(x) as Command)
				.Where(x => x != null)
				.ToDictionary(x => x.Text, x => x);
		}
	}
}
