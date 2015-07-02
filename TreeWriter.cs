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
	public class TreeWriter
	{
		private const string NAMESPACE_ROOT_NAME = "[NS]";
		private const string FIELD_ROOT_NAME = "[FE]";
		private const string PROPERTY_ROOT_NAME = "[PR]";
		private const string CONSTRUCTOR_ROOT_NAME = "[CS]";
		private const string METHOD_ROOT_NAME = "[MT]";
		private const string CLASS_ROOT_NAME = "[CN]";
		private const string INTERFACE_ROOT_NAME = "[IN]";

		private const string BODY = "body";
		private const string PARAMETERS = "parameters";

		private	string input;

		public TreeWriter(string input)
		{
			this.input = input;
		}

		public void Write(string outputFilePath)
		{
			StringBuilder treeTextBuilder = new StringBuilder();
			Tree rootTree = ParseAndCreateGitTree();
			rootTree.AppendToBuilder(treeTextBuilder);

			File.WriteAllText(outputFilePath, treeTextBuilder.ToString());

#if DEBUG
			Console.WriteLine(treeTextBuilder);
#endif
		}

		private Tree ParseAndCreateGitTree()
		{
			SyntaxTree tree = CSharpSyntaxTree.ParseText(input);
			CompilationUnitSyntax root = tree.GetRoot() as CompilationUnitSyntax;

			return NameSpaceDeclaration(root);
		}

		/// <summary>
		/// CompilationUnitSyntax is also regarded as a NameSpaceDeclaration.
		/// </summary>
		private Tree NameSpaceDeclaration(SyntaxNode node)
		{
			SyntaxList<MemberDeclarationSyntax> members;
			Tree nameSpaceRoot;
			GetNameSpaceRootAndNodeMembers(node, out members, out nameSpaceRoot);

			Tree classRoot = new Tree(CLASS_ROOT_NAME);
			Tree innerNameSpaceRoot = new Tree(NAMESPACE_ROOT_NAME);

			foreach (MemberDeclarationSyntax member in members) {
				if (member is ClassDeclarationSyntax) {
					var classDecl = member as ClassDeclarationSyntax;
					classRoot.AppendObject(ClassDeclaration(classDecl, false));
				} else if (member is NamespaceDeclarationSyntax) {
					innerNameSpaceRoot.AppendObject(NameSpaceDeclaration(member));
				}
			}

			nameSpaceRoot.AppendObject(classRoot);
			nameSpaceRoot.AppendObject(innerNameSpaceRoot);

			return nameSpaceRoot;
		}

		private void GetNameSpaceRootAndNodeMembers(SyntaxNode node, out SyntaxList<MemberDeclarationSyntax> members, out Tree nameSpaceRoot)
		{
			if (node is NamespaceDeclarationSyntax) {
				var n = node as NamespaceDeclarationSyntax;
				members = n.Members;
				nameSpaceRoot = new Tree(n.Name.ToString());
			} else {
				var n = node as CompilationUnitSyntax;
				members = n.Members;
				nameSpaceRoot = new Tree("");
			}
		}

		private Tree ClassDeclaration(ClassDeclarationSyntax classNode)
		{
			Tree classTree = new Tree(classNode.Identifier.ToString());

			classTree.AppendObject(FieldDeclarations(classNode.Members.OfType<FieldDeclarationSyntax>()));
			classTree.AppendObject(PropertyDecrarations(classNode.Members.OfType<PropertyDeclarationSyntax>()));
			classTree.AppendObject(MethodDeclarations(classNode.Members.OfType<MethodDeclarationSyntax>()));
			classTree.AppendObject(ConstructorDeclarations(classNode.Members.OfType<ConstructorDeclarationSyntax>()));

			List<ClassDeclarationSyntax> innerClassNodes = classNode.Members.OfType<ClassDeclarationSyntax>().ToList();
			if (innerClassNodes.Count > 0) {
				Tree innerClassRootTree = new Tree(CLASS_ROOT_NAME);
				foreach (ClassDeclarationSyntax innerClass in innerClassNodes) {
					innerClassRootTree.AppendObject(ClassDeclaration(innerClass, true));
				}
				classTree.AppendObject(innerClassRootTree);
			}
			return classTree;
		}

		private Tree ClassDeclaration(ClassDeclarationSyntax classNode, bool isInnerClass)
		{
			Tree classTree = ClassDeclaration(classNode);
			Blob modifiers = new Blob("modifiers", GetClassModifierBody(classNode, isInnerClass));
			classTree.AppendObject(modifiers);

			return classTree;
		}

		private string GetClassModifierBody(ClassDeclarationSyntax classNode, bool isInnerClass)
		{
			String[] accessLevelModifiers = {"public", "private", "internal", "protected"};
			var modifiers = classNode.Modifiers.Select(m => m.ToString()).ToList();
			if (!modifiers.Intersect(accessLevelModifiers).Any())
			{
				if(isInnerClass)
					modifiers.Add("private");
				else
					modifiers.Add("internal");
			}
			modifiers.Sort();
			return string.Join("\n", modifiers);
		}

		private GitObject MethodDeclarations(IEnumerable<MethodDeclarationSyntax> nodes)
		{
			Tree methodRootTree = new Tree(METHOD_ROOT_NAME);
			foreach (MethodDeclarationSyntax node in nodes) {
				CreateBodyParametersBlob(methodRootTree, node, node.Identifier.ToString());
			}
			return methodRootTree;
		}

		private GitObject ConstructorDeclarations(IEnumerable<ConstructorDeclarationSyntax> nodes)
		{
			Tree constructorRootTree = new Tree(CONSTRUCTOR_ROOT_NAME);
			foreach (ConstructorDeclarationSyntax node in nodes) {
				CreateBodyParametersBlob(constructorRootTree, node, node.Identifier.ToString());
			}
			return constructorRootTree;
		}

		private void CreateBodyParametersBlob(Tree tree, BaseMethodDeclarationSyntax node, string identifier)
		{
			string  methodString = identifier + "(";
			methodString += string.Join(",", node.ParameterList.Parameters.Select(p => p.ToModifierTypeString()));
			methodString += ")";
			Tree methodTree = new Tree(methodString);
			if ( node.Body != null)
			{
				string _text = node.Body.GetText().ToString().Trim('\n');
				methodTree.AppendObject(new Blob(BODY, _text));
			}

			StringBuilder parameterStringBuilder = new StringBuilder();
			foreach (ParameterSyntax parameter in node.ParameterList.Parameters) {
				parameterStringBuilder.AppendLine(parameter.ToModifierTypeString() + " " + parameter.Identifier);
			}
			string parameterString = parameterStringBuilder.ToString().Trim('\n');
			methodTree.AppendObject(new Blob(PARAMETERS, parameterString.ToString()));

			tree.AppendObject(methodTree);
		}

		private GitObject PropertyDecrarations(IEnumerable<PropertyDeclarationSyntax> nodes)
		{
			Tree propertyRootTree = new Tree(PROPERTY_ROOT_NAME);
			foreach (PropertyDeclarationSyntax node in nodes) {
				string _text = node.GetText().ToString().Trim('\n');
				Blob prop = new Blob(node.Identifier.ToString(), _text);
				propertyRootTree.AppendObject(prop);
			}
			return propertyRootTree;
		}

		private GitObject FieldDeclarations(IEnumerable<FieldDeclarationSyntax> nodes)
		{
			Tree fieldRootTree = new Tree(FIELD_ROOT_NAME);
			foreach (FieldDeclarationSyntax node in nodes) {
				foreach (VariableDeclaratorSyntax variables in node.Declaration.Variables) {
					Blob field = new Blob(variables.Identifier.ToString(), "");
					fieldRootTree.AppendObject(field);
				}
			}
			return fieldRootTree;
		}
	}
}

