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
			StringBuilder result = new StringBuilder();
			Tree rootTree = ParseAndCreateGitTree();
			rootTree.AppendToBuilder(result);

			File.WriteAllText(outputFilePath, result.ToString());

#if DEBUG
			Console.WriteLine(result);
#endif
		}

		private Tree ParseAndCreateGitTree()
		{
			SyntaxTree tree = CSharpSyntaxTree.ParseText(input);
			CompilationUnitSyntax root = tree.GetRoot() as CompilationUnitSyntax;
			Tree rootGitTree = new Tree("");
			foreach (SyntaxNode node in root.Members) {
				CreateTree(node, rootGitTree);
			}
			return rootGitTree;
		}

		private void CreateTree(SyntaxNode node, Tree currentRoot)
		{
			if (node is NamespaceDeclarationSyntax) {
				NamespaceDeclarationSyntax namespaceNode = node as NamespaceDeclarationSyntax;
				// FIXME Namespace will be able to appeare twice in the root tree.
				Tree nameSpaceRoot = new Tree(NAMESPACE_ROOT_NAME);
				Tree nameSpaceTree = new Tree(namespaceNode.Name.ToString());
				nameSpaceRoot.AppendObject(nameSpaceTree);

				foreach (MemberDeclarationSyntax member in namespaceNode.Members) {
					CreateTree(member, nameSpaceTree);
				}
				currentRoot.AppendObject(nameSpaceRoot);
			} else if (node is ClassDeclarationSyntax) {
				ClassDeclarationSyntax classNode = node as ClassDeclarationSyntax;
				Tree classRootTree = new Tree(CLASS_ROOT_NAME);

				classRootTree.AppendObject(ClassDeclaration(classNode, classRootTree));
				currentRoot.AppendObject(classRootTree);
			}
		}

		private GitObject ClassDeclaration(ClassDeclarationSyntax classNode, Tree classRootTree)
		{
			Tree classTree = new Tree(classNode.Identifier.ToString());

			classTree.AppendObject(FieldDeclarations(classNode.Members.OfType<FieldDeclarationSyntax>()));
			classTree.AppendObject(PropertyDecrarations(classNode.Members.OfType<PropertyDeclarationSyntax>()));
			classTree.AppendObject(MethodDeclarations(classNode.Members.OfType<MethodDeclarationSyntax>()));

			List<ClassDeclarationSyntax> innerClassNodes = classNode.Members.OfType<ClassDeclarationSyntax>().ToList();
			if (innerClassNodes.Count > 0) {
				Tree innerClassRootTree = new Tree(CLASS_ROOT_NAME);
				foreach (ClassDeclarationSyntax innerClass in innerClassNodes) {
					innerClassRootTree.AppendObject(ClassDeclaration(innerClass, innerClassRootTree));
				}
				classTree.AppendObject(innerClassRootTree);
			}
			return classTree;
		}

		private GitObject MethodDeclarations(IEnumerable<MethodDeclarationSyntax> nodes)
		{
			Tree methodRootTree = new Tree(METHOD_ROOT_NAME);
			foreach (MethodDeclarationSyntax node in nodes) {
				string  methodString = node.Identifier + "(";
				methodString += string.Join(",", node.ParameterList.Parameters.Select(p => p.Type.ToString()));
				methodString += ")";
				Tree methodTree = new Tree(methodString);
				string _text = node.GetText().ToString().Trim('\n');
				methodTree.AppendObject(new Blob(BODY, _text));

				// FIXME parameters should be stored as a blob.
				//result.AppendLine(START_TREE + PARAMETERS);
				//Tree parameterList = new Tree(PARAMETERS);
				//StringBuilder parameterList = new StringBuilder();
				//foreach (ParameterSyntax parameter in node.ParameterList.Parameters) {
				//	parameterList.AppendLine(parameter.Type.ToString() + " " + parameter.Identifier);
				//}
				//result.AppendLine(parameterList.ToString());
				//result.AppendLine(END_TREE + PARAMETERS);
				//result.AppendLine(END_TREE + methodString);
				methodRootTree.AppendObject(methodTree);
			}
			return methodRootTree;
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

