using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
	public static class InputReader
	{
		public static IEnumerable<string> Read<TProgram>()
		{
			using (Stream? stream = typeof(TProgram).Assembly.GetManifestResourceStream($"{typeof(TProgram).Namespace}.Input.txt"))
			{
				using (StreamReader sr = new StreamReader(stream!))
				{
					string? line;
					do
					{
						line = sr.ReadLine();
						if (line != null)
							yield return line;
					}
					while (line != null);
				}
			}
		}
	}
}
