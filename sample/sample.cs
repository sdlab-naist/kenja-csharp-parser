using System;
using System.Collections;
using System.Linq;
using System.Text;

class Program
{
	string hoge = "aa";
	string hoge2 = "aa";

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

	public int j_;
	public int j {
		get {
			return j_;
		}
		set {
			j_ = value;
		}
	}

	var x = 11;

	// class Program2
	// 	{
	// 		public int num = 1;

	// 		void Hoge(double hoge1, int hoge2, string hoge3)
	// 		{
	// 			Console.WriteLine(hoge4);
	// 		}
	// 	}
	// }
}

// namespace HelloWorld
// {
// 	class Program2
// 	{
// 		public int num = 1;

// 		void Hoge(double hoge1, int hoge2, string hoge3)
// 		{
// 			Console.WriteLine(hoge4);
// 		}
// 	}
// }
