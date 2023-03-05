using System;

/*
Injector.Replace(() => Target.TargetMethod2(), () => "HAHA!!");
var targetInstance = new Target();
targetInstance.Test();
Injector.Install<Target, Replacement>(1);
Injector.Install<Target, Replacement>(2);
Injector.Install<Target, Replacement>(3);
Injector.Install<Target, Replacement>(4);
targetInstance.Test();
*/


Injector.Replace(() => DateTime.UtcNow, () => new DateTime(2000, 1, 2, 3, 4, 5));
Console.WriteLine(DateTime.UtcNow);


public class Target
{
	public void Test()
	{
		// TargetMethod1();
		Console.WriteLine(TargetMethod2());
		// TargetMethod3("Test");
		// TargetMethod4();
	}

	private void TargetMethod1()
	{
		Console.WriteLine("Target.TargetMethod1()");

	}

	public static string TargetMethod2()
	{
		Console.WriteLine("Target.TargetMethod2()");
		return "Not injected 2";
	}

	public void TargetMethod3(string text)
	{
		Console.WriteLine("Target.TargetMethod3(" + text + ")");
	}

	private void TargetMethod4()
	{
		Console.WriteLine("Target.TargetMethod4()");
	}
}

class Replacement
{
	private void InjectionMethod1()
	{
		Console.WriteLine("Injection.InjectionMethod1");
	}

	private string InjectionMethod2()
	{
		Console.WriteLine("Injection.InjectionMethod2");
		return "Injected 2";
	}

	private void InjectionMethod3(string text)
	{
		Console.WriteLine("Injection.InjectionMethod3 " + text);
	}

	private void InjectionMethod4()
	{
		Console.WriteLine("Injection.InjectionMethod4");
		// System.Diagnostics.Process.Start("calc");
	}
}
