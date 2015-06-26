using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace KenjaParser
{
	public static class Helper
	{
		public static string ToModifierTypeString(this ParameterSyntax parameter)
		{
			string str = "";
			if (parameter.Modifiers.Count != 0) {
				str += parameter.Modifiers + " ";
			}
			str += parameter.Type.ToString();
			return str;
		}
	}
}

