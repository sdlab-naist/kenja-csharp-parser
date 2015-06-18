using System.Collections.Generic;
using System.Text;

namespace KenjaParser
{
    /// <summary>
    /// A class which represents a Tree of Git.
    /// This class provided interface to make tree file for kenja.
    /// If you want to make a root tree, provide empty string to Name property.
    /// </summary>
    public class Tree : GitObject
    {
        private const string START_TREE = "[TS] ";
        private const string END_TREE = "[TE] ";
        private List<GitObject> objects = new List<GitObject>();

        public int Length { get { return objects.Count; } }
        
        /// <param name="name">
        /// Name of a tree. For a root tree, empty string should be provided.
        /// </param>
        public Tree(string name) : base(name){
        }

        public override void AppendToBuilder(StringBuilder builder)
        {
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

