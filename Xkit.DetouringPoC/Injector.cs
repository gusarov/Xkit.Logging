using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

public class Injector
{
#if TODO
	private class InjectorStorage<T>
	{
		public static List<Func<T>> TargetsStorage = new List<Func<T>>();
	}

	public static DateTime GetUtcNowReplacement()
	{
		return InjectorStorage<DateTime>.TargetsStorage[5]();
	}

	static Delegate CreateDelegate(MethodInfo methodInfo, object target)
	{
		Func<Type[], Type> getType;
		var isAction = methodInfo.ReturnType.Equals((typeof(void)));
		var types = methodInfo.GetParameters().Select(p => p.ParameterType);

		if (isAction)
		{
			getType = Expression.GetActionType;
		}
		else
		{
			getType = Expression.GetFuncType;
			types = types.Concat(new[] { methodInfo.ReturnType });
		}

		if (methodInfo.IsStatic)
		{
			return Delegate.CreateDelegate(getType(types.ToArray()), methodInfo);
		}

		return Delegate.CreateDelegate(getType(types.ToArray()), target, methodInfo.Name);
	}
#endif

	public static void Replace<T>(Expression<Func<T>> originalCall, Func<T> replacementCall)
	{
		var originalMethod = GetOriginalMethod(originalCall);
		var replacementMethod = replacementCall.Method;

#if TODO
		if (!replacementMethod.IsStatic) // TODO also - if any other signature missmatch
		{
			// have to wrap it! Otherwise implicit this will be for a wrong instance or will be empty

			int index;
			lock (InjectorStorage<T>.TargetsStorage)
			{
				index = InjectorStorage<T>.TargetsStorage.Count;
				InjectorStorage<T>.TargetsStorage.Add(replacementCall);
			}

			// the signature of dynamic method must match to original method signature, but body must call replacement and adjust the signature to it
			var d = new DynamicMethod("", typeof(T), Array.Empty<Type>(), true);
			var gen = d.GetILGenerator();

			var fi = typeof(InjectorStorage<T>).GetField("TargetsStorage");
			gen.Emit(OpCodes.Ldsfld, fi); // storage list
			gen.Emit(OpCodes.Ldc_I4, index); // hadcoded index in global memory
			gen.EmitCall(OpCodes.Callvirt, typeof(List<T>).GetMethod("get_Item"), null);
			// gen.EmitCall(OpCodes.Callvirt, typeof(Func<T>).GetMethod("Invoke"), null);
			gen.Emit(OpCodes.Ret);
			var del = d.CreateDelegate(CreateDelegate(originalMethod, null).GetType());
			replacementMethod = del.Method;
		}

		/*
		var replacementMethod = replacementCall.Target == null
			? replacementCall.Method // static - no need to wrap into binded delegate
			: Delegate.CreateDelegate(typeof(Func<T>), replacementCall.Target, replacementCall.Method).Method; // create unbound delegate to wrap binded instance method and pass "first argument" aka "implicit this" of instance
		*/
#endif
		Install(originalMethod, replacementMethod);
	}

	static MethodInfo GetOriginalMethod<T>(Expression<Func<T>> originalCall)
	{
		var expression = originalCall.Body;
		if (expression is MethodCallExpression mce) // methods
		{
			return mce.Method;
		}
		else if (expression is MemberExpression me) // e.g. property getter
		{
			switch (me.Member.MemberType)
			{
				case MemberTypes.Property:
					var pi = (PropertyInfo)me.Member;
					var mi = pi.GetAccessors().FirstOrDefault(); // todo support replace getter or setter separately, but this signature for Func<> assumes getters only
					return mi;
				default:
					throw new NotSupportedException(me.Member.MemberType.ToString());
			}
		}
		else
		{
			throw new NotSupportedException(expression.GetType().FullName);
		}
	}

	internal static void Install<TFrom, TTo>(int funcNum)
	{
		var originalMethod = typeof(TFrom).GetMethod("TargetMethod" + funcNum, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
		var methodToInject = typeof(TTo).GetMethod("InjectionMethod" + funcNum, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
		Install(originalMethod, methodToInject);
	}

	internal static unsafe void Install(MethodInfo methodOriginal, MethodInfo methodReplacement)
	{
		RuntimeHelpers.PrepareMethod(methodOriginal.MethodHandle);
		RuntimeHelpers.PrepareMethod(methodReplacement.MethodHandle);

		if (IntPtr.Size == 4)
		{
			int* tar = (int*)methodOriginal.MethodHandle.Value.ToPointer() + 2;
			int* inj = (int*)methodReplacement.MethodHandle.Value.ToPointer() + 2;
#if DEBUG
			// Console.WriteLine("\nVersion x86 Debug\n");

			byte* injInst = (byte*)*inj;
			byte* tarInst = (byte*)*tar;

			int* injSrc = (int*)(injInst + 1);
			int* tarSrc = (int*)(tarInst + 1);

			*tarSrc = (((int)injInst + 5) + *injSrc) - ((int)tarInst + 5);
#else
                // Console.WriteLine("\nVersion x86 Release\n");
                *tar = *inj;
#endif
		}
		else
		{

			long* tar = (long*)methodOriginal.MethodHandle.Value.ToPointer() + 1;
			long* inj = (long*)methodReplacement.MethodHandle.Value.ToPointer() + 1;
#if DEBUG
			// Console.WriteLine("\nVersion x64 Debug\n");
			byte* injInst = (byte*)*inj;
			byte* tarInst = (byte*)*tar;


			int* injSrc = (int*)(injInst + 1);
			int* tarSrc = (int*)(tarInst + 1);

			*tarSrc = (((int)injInst + 5) + *injSrc) - ((int)tarInst + 5);
#else
                // Console.WriteLine("\nVersion x64 Release\n");
                *tar = *inj;
#endif
		}
	}
}
