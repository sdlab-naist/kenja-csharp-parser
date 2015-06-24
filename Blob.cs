using System.Text;

namespace KenjaParser
{
	public class Blob : GitObject
	{
		private const string BLOB = "[BN] ";
		private const string BLOB_LINEINFO = "[BI] ";

		protected string body;

		public Blob(string name, string body) : base(name)
		{
			body.TrimEnd('\n');
			this.body = body;
		}

		public override void AppendToBuilder(StringBuilder builder)
		{
			builder.AppendLine(BLOB + name);

			int lines = body.Split('\n').Length;
			builder.AppendLine(BLOB_LINEINFO + lines);

			builder.Append(body);
			builder.AppendLine();
		}
	}
}

