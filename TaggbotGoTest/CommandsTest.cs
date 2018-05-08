using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaggbotGo.Commands;

namespace TaggbotGoTest
{
	[TestClass]
	public class CommandsTest
	{
		[TestMethod]
		public void HelpTest()
		{
			Command.Execute("help");
		}
	}
}
