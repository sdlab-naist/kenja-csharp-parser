using System;
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

		private const string FIELD_ROOT_NAME = "[FE]";
		private const string CONSTRUCTOR_ROOT_NAME = "[CS] ";

		private const string CLASS_ROOT_NAME = "[CN] ";
		private const string INTERFACE_ROOT_NAME = "[IN] ";

		public TreeWriter()
		{
			Write();
		}

		private void Write()
		{
			SyntaxTree tree = CSharpSyntaxTree.ParseText(
				@"
					using System;
					using System.Collections;
					using System.Linq;
					using System.Text;

					namespace HelloWorld
					{
						class Program
						{
							string _hoge = ""aa"";

							static void Main(string[] args)
							{
								var a = 10;
								Console.WriteLine(""Hello, World!"");
							}

							void Hoge(double hoge1, int hoge2)
							{
								int hoge3 = hoge1 + hoge2;
								Console.WriteLine(hoge3);
							}

							public int i {get;set;}
							var i = 11;

						}
					}");

			CompilationUnitSyntax _root = tree.GetRoot() as CompilationUnitSyntax;
			foreach (SyntaxNode _node in _root.Members) {
				if (_node is NamespaceDeclarationSyntax) {
					NamespaceDeclarationSyntax _namespaceNode = _node as NamespaceDeclarationSyntax;
					foreach (BaseTypeDeclarationSyntax _namepsaceMember in _namespaceNode.Members)
					{
						if (_namepsaceMember is ClassDeclarationSyntax)
						{
							ClassDeclarationSyntax _classNode = _namepsaceMember as ClassDeclarationSyntax;
							foreach (var _classMember in _classNode.Members)
							{
								if (_classMember is PropertyDeclarationSyntax) {PropertyDecraration((PropertyDeclarationSyntax)_classMember);}
								if (_classMember is MethodDeclarationSyntax) {MethodDeclaration((MethodDeclarationSyntax)_classMember);}
								if (_classMember is FieldDeclarationSyntax) {FieldDeclaration((FieldDeclarationSyntax)_classMember);}
							}
						}
					}
				}
			}

			Console.Read();
		}

		private void PropertyDecraration(PropertyDeclarationSyntax _node)
		{
			Console.WriteLine("Property:: " + _node.GetText());
		}

		private void MethodDeclaration(MethodDeclarationSyntax _node)
		{
			foreach (SyntaxTree _tree in _node.ChildNodes()) {
				Console.WriteLine(_tree);
				Console.WriteLine("----");
			}

			Console.WriteLine("Method:: " + _node.GetText());
			foreach (SyntaxToken _methodModifier in _node.Modifiers)
			{
				Console.WriteLine("Modifier:: " + _methodModifier.Text);
			}
			Console.WriteLine("Identifier:: " + _node.ReturnType);
			Console.WriteLine("Identifier:: " + _node.Identifier);
			foreach (ParameterSyntax _parameter in _node.ParameterList.Parameters)
			{
				Console.WriteLine("Parameter Type:: " + _parameter.Type.ToString());
				Console.WriteLine("Parameter Identifier:: " + _parameter.Identifier);
			}
		}

		private void FieldDeclaration(FieldDeclarationSyntax _node)
		{
			Console.WriteLine("Variable:: " + _node.GetText());
		}
	}
}

