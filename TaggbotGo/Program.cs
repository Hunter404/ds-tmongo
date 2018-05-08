using System.IO;
using System.Reflection;
using MathNet.Numerics.LinearAlgebra.Single;

namespace TaggbotGo
{
	public class Program
	{
		public static string ConnectionString => _connectionString = _connectionString ?? $"Data Source =\"{Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..\\Database.sqlite"))}\";AutoVacuum=None";

		private static string _connectionString;

		private static void Main(string[] args)
		{

		}
	}
}
