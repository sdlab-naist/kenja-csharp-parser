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
		private const string BLOB = "[BN] ";
		private const string BLOB_LINEINFO = "[BI] ";

		private const string TREE = "[TN] ";
		private const string START_TREE = "[TS] ";
		private const string END_TREE = "[TE] ";

		private const string NAMESPACE_ROOT_NAME = "[NS]";
		private const string FIELD_ROOT_NAME = "[FE]";
		private const string PROPERTY_ROOT_NAME = "[PR]";
		private const string CONSTRUCTOR_ROOT_NAME = "[CS]";
		private const string METHOD_ROOT_NAME = "[MT]";
		private const string CLASS_ROOT_NAME = "[CN]";
		private const string INTERFACE_ROOT_NAME = "[IN]";

		private const string BODY = "body";
		private const string PARAMETERS = "parameters";

		private StringBuilder result;
		private	string input;

		public TreeWriter(string input)
		{
			result = new StringBuilder();
			this.input = input;
			CreateResult();
		}

		public void Write(string outputFilePath)
		{
			File.WriteAllText(outputFilePath, result.ToString());
		}

		private void CreateResult()
		{
			SyntaxTree tree = CSharpSyntaxTree.ParseText(input);
			CompilationUnitSyntax root = tree.GetRoot() as CompilationUnitSyntax;
			foreach (SyntaxNode node in root.Members) {
				CreateTree(node);
			}

#if DEBUG
			Console.WriteLine(result);
#endif
		}

		private void CreateTree(SyntaxNode node)
		{
			if (node is NamespaceDeclarationSyntax) {
				NamespaceDeclarationSyntax namespaceNode = node as NamespaceDeclarationSyntax;
				result.AppendLine(START_TREE + NAMESPACE_ROOT_NAME);
				result.AppendLine(START_TREE + namespaceNode.Name);

				foreach (MemberDeclarationSyntax member in namespaceNode.Members) {
					CreateTree(member);
				}

				result.AppendLine(END_TREE + namespaceNode.Name);
				result.AppendLine(END_TREE + NAMESPACE_ROOT_NAME);
			} else if (node is ClassDeclarationSyntax) {
				ClassDeclarationSyntax classNode = node as ClassDeclarationSyntax;
				result.AppendLine(START_TREE + CLASS_ROOT_NAME);
				ClassDeclaration(classNode);
				result.AppendLine(END_TREE + CLASS_ROOT_NAME);
			}
		}

		private void ClassDeclaration(ClassDeclarationSyntax classNode)
		{
			result.AppendLine(START_TREE + classNode.Identifier);

			FieldDeclarations(classNode.Members.OfType<FieldDeclarationSyntax>());
			PropertyDecrarations(classNode.Members.OfType<PropertyDeclarationSyntax>());
			MethodDeclarations(classNode.Members.OfType<MethodDeclarationSyntax>());

			List<ClassDeclarationSyntax> innerClassNodes = classNode.Members.OfType<ClassDeclarationSyntax>().ToList();
			if (innerClassNodes.Count > 0) {
				result.AppendLine(START_TREE + CLASS_ROOT_NAME);
				foreach (ClassDeclarationSyntax innerClass in innerClassNodes) {
					ClassDeclaration(innerClass);
				}
				result.AppendLine(END_TREE + CLASS_ROOT_NAME);
			}

			result.AppendLine(END_TREE + classNode.Identifier);
		}

		private void MethodDeclarations(IEnumerable<MethodDeclarationSyntax> nodes)
		{
			result.AppendLine(START_TREE + METHOD_ROOT_NAME);
			foreach (MethodDeclarationSyntax node in nodes) {
				string  methodString = node.Identifier + "(";
				methodString += string.Join(",", node.ParameterList.Parameters.Select(p => p.Type.ToString()));
				methodString += ")";
				result.AppendLine(START_TREE + methodString);
				result.AppendLine(BLOB + BODY);
				result.AppendLine(BLOB_LINEINFO + node.GetText().Lines.Count);
				string _text = node.GetText().ToString().Trim('\n');
				result.AppendLine(_text);
				result.AppendLine(START_TREE + PARAMETERS);
				StringBuilder parameterList = new StringBuilder();
				foreach (ParameterSyntax parameter in node.ParameterList.Parameters) {
					parameterList.AppendLine(parameter.Type.ToString() + " " + parameter.Identifier);
				}
				result.AppendLine(parameterList.ToString());
				result.AppendLine(END_TREE + PARAMETERS);
				result.AppendLine(END_TREE + methodString);
			}
			result.AppendLine(END_TREE + METHOD_ROOT_NAME);
		}

		private void PropertyDecrarations(IEnumerable<PropertyDeclarationSyntax> nodes)
		{
			result.AppendLine(START_TREE + PROPERTY_ROOT_NAME);
			foreach (PropertyDeclarationSyntax node in nodes) {
				result.AppendLine(BLOB + node.Identifier);
				result.AppendLine(BLOB_LINEINFO + node.GetText().Lines.Count);
				string _text = node.GetText().ToString().Trim('\n');
				result.AppendLine(_text);
			}
			result.AppendLine(END_TREE + PROPERTY_ROOT_NAME);
		}

		private void FieldDeclarations(IEnumerable<FieldDeclarationSyntax> nodes)
		{
			result.AppendLine(START_TREE + FIELD_ROOT_NAME);
			foreach (FieldDeclarationSyntax node in nodes) {
				foreach (VariableDeclaratorSyntax variables in node.Declaration.Variables) {
					result.AppendLine(BLOB + variables.Identifier);
					result.AppendLine(BLOB_LINEINFO + "0");
				}
			}
			result.AppendLine(END_TREE + FIELD_ROOT_NAME);
		}
	}
}

