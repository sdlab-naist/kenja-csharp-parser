using System.Text;

namespace KenjaParser
{
    abstract public class GitObject
    {
        public string name { get; set; }

        public GitObject(string name)
        {
            this.name = name;
        }

        abstract public void AppendToBuilder(StringBuilder builder);
    }
}
