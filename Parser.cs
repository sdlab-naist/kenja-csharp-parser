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

			StreamReader sr = new StreamReader(args[0]);
			string inputFileString = sr.ReadToEnd();
			string outputDirectory = args[1];

			TreeWriter treeWrite = new TreeWriter(inputFileString);

			string filePath = Path.Combine(outputDirectory, "hoge");
			treeWrite.Write(filePath);
		}

		static bool CheckArgs(string[] args)
		{
			if (args.Length != 2) {
				Console.WriteLine("Input Error");
				Console.WriteLine("Input <Path of Input File> <Path of Output Directory>");
				return false;
			}

			string inputFile = args[0];
			string outputDirectory = args[1];

			if (!File.Exists(inputFile)) {
				Console.WriteLine(inputFile + " does not exists.");
				return false;
			}

			if (!Directory.Exists(outputDirectory)) {
				try {
					Directory.CreateDirectory(outputDirectory);
				}
				catch {
					Console.WriteLine("Could not create directory " + outputDirectory);
				}
			}
			return true;
		}
	}
}
