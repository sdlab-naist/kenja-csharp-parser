using System.Collections.Generic;
using System.Text;

namespace KenjaParser
{
    public class Tree : GitObject
    {
        private const string START_TREE = "[TS] ";
        private const string END_TREE = "[TE] ";
        private List<GitObject> objects = new List<GitObject>();
        
        public Tree(string name) : base(name){
        }

        public override void AppendToBuilder(StringBuilder builder)
        {
            // TODO add document comment
            if(name != ""){
                builder.AppendLine(START_TREE + name);
            }

            objects.Sort((a, b) => a.name.CompareTo(b.name));
            foreach (GitObject obj in objects)
            {
                obj.AppendToBuilder(builder);
            }

            if(name != ""){
                builder.AppendLine(END_TREE + name);
            }
        }
        
        public void AppendObject(GitObject obj){
            objects.Add(obj);
        }
    }
}

