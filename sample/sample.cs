using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace Namespace1
{
	namespace InnerNamespace1
	{
		class Sample
		{
			string hoge;

			public Sample(ref string hoge, out int integer)
			{
				this.hoge = hoge;
			}
		}
	}

	class Sample
	{
		public string hoge = "aa";
		private string fuga = "aa";

		public string piyo {
			get {return fuga;}
		}

		class InnerClass
		{
			public InnerClass()
			{
				Console.WriteLine("Inner Class");
			}
		}

		void SampleMain(string[] args)
		{
			var a = 10;
			Console.WriteLine("Hello, World!");
		}

		void Hoge(double p1, int p2)
		{
			double i = p1 + (double)p2;
			Console.WriteLine(i);
		}
	}
}

class Class1
{
	int hoge;
	private int Hoge()
	{
		return hoge;
	}
}

namespace Namespace2
{
	class Sample
	{
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
	}
}

class Class2
{
	float fuga;
	private float Fuga()
	{
		return fuga;
	}
}
