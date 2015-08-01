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
			CreateParser(args).Parse();
		}

		static bool CheckArgs(string[] args)
		{
			if (args.Length != 1 && args.Length != 2) {
				Console.WriteLine("Usage (for a blob): kenja-csharp-parser.exe <output dir path>");
				Console.WriteLine("Usage (for blos): kenja-csharp-parser.exe <src repository path> <output dir path>");
				return false;
			}
			return true;
		}

		static Parser CreateParser(string[] args)
		{
			Parser parser;

			if (args.Length == 1) {
				parser = new Parser(args);
			} else {
				parser = new ParserFromBlobs(args);
			}

			return parser;
		}

		////////////////////

		protected string[] args;

		public Parser(string[] args)
		{
			this.args = args;
		}

		public virtual void Parse()
		{
			if (!CheckArgs()) {return;}

			TreeWriter treeWrite = new TreeWriter(GetSrc());
			treeWrite.Write(args[0]);
		}

		protected virtual bool CheckArgs()
		{
			string outputDirectory = Path.GetDirectoryName(args[0]);
			return CheckDirectoryExists(outputDirectory);
		}

		protected bool CheckDirectoryExists(string directoryPath)
		{
			if (!Directory.Exists(directoryPath)) {
				try {
					Directory.CreateDirectory(directoryPath);
				} catch {
					Console.WriteLine("could not create directory " + directoryPath);
					return false;
				}
			}
			return true;
		}

		private string GetSrc()
		{
			StringBuilder inputFileSrc;
#if DEBUG
			StreamReader sr = new StreamReader(Console.ReadLine());
			inputFileSrc = new StringBuilder(sr.ReadToEnd());
#else
			inputFileSrc = new StringBuilder();
			string line = "";
			while ((line = Console.ReadLine()) != null)
			{
				inputFileSrc.AppendLine(line);
			}
#endif
			return inputFileSrc.ToString();
		}
	}
}
