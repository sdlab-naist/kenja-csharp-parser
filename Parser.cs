using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace KenjaParser
{
	class Parser
	{
		static void Main(string[] args)
		{
			if (!CheckArgs(args)) {return;}

#if DEBUG
			StreamReader sr = new StreamReader(Console.ReadLine());
			string inputFileSrc = sr.ReadToEnd();
#else
			string inputFileSrc = "";
			string line = "";
			while ((line = Console.ReadLine()) != null)
			{
				inputFileSrc += line;
			}
#endif

			TreeWriter treeWrite = new TreeWriter(inputFileSrc);
			treeWrite.Write(args[0]);
		}

		static bool CheckArgs(string[] args)
		{
			if (args.Length != 1) {
				Console.WriteLine("please input output dir path");
				return false;
			}

			string outputFilePath = args[0];
			string outputDirectory = Path.GetDirectoryName(outputFilePath);
			if (!Directory.Exists(outputDirectory)) {
				try {
					Directory.CreateDirectory(outputDirectory);
				}
				catch {
					Console.WriteLine("could not create directory " + outputDirectory);
					return false;
				}
			}
			return true;
		}
	}
}
