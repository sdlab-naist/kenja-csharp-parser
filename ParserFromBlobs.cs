using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NGit;
using NGit.Storage.File;

namespace KenjaParser
{
	class ParserFromBlobs : Parser
	{
		private Repository repo;

		public ParserFromBlobs(string[] args) : base(args) {}

		public override void Parse()
		{
			if (!CheckArgs()) {return;}

#if DEBUG
			StreamReader sr = new StreamReader(Console.ReadLine());
			string line = "";
			while ((line = sr.ReadLine()) != null) {
				if (!string.IsNullOrEmpty(line)) {
					TreeWriter treeWrite = new TreeWriter(GetSrcFromBlobID(line));
					treeWrite.Write(Path.Combine(args[1], line));
				}
			}
#else
			string line = "";
			while ((line = Console.ReadLine()) != null)
			{
				if (!string.IsNullOrEmpty(line)) {
					TreeWriter treeWrite = new TreeWriter(GetSrcFromBlobID(line));
					treeWrite.Write(Path.Combine(args[1], line));
				}
			}
#endif
		}

		protected override bool CheckArgs()
		{
			string outputDirectory = args[1];
			if (!CheckDirectoryExists(outputDirectory)) {return false;}

			string repoPath = Path.Combine(args[0], ".git");
			if (!CheckGitRepoExists(repoPath)) {return false;}
			repo = new FileRepository(repoPath);

			return true;
		}

		private bool CheckGitRepoExists(string repoPath)
		{
			return Directory.Exists(repoPath);
		}

		private string GetSrcFromBlobID(string blobID)
		{
			ObjectLoader loader = repo.Open(ObjectId.FromString(blobID));
			byte[] bytes = loader.GetCachedBytes();
			return System.Text.Encoding.UTF8.GetString(bytes);
		}
	}
}
