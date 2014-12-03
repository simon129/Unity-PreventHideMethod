using UnityEngine;
using System;
using System.Reflection;

public class PreventHideMonoSingletonAssetModificationProcessor : UnityEditor.AssetModificationProcessor
{
	public static string[] OnWillSaveAssets(string[] paths)
	{
		foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
		{
			if (assembly.FullName.StartsWith("Assembly-"))
			{
				foreach (var type in assembly.GetTypes())
				{
					Type singletonBaseType;
					if (IsMonoSingleton(type, out singletonBaseType))
					{
						CheckMethodFromMonoSingleton("Awake", type, singletonBaseType);
						CheckMethodFromMonoSingleton("OnDestroy", type, singletonBaseType);
					}
				}
			}
		}

		return paths;
	}

	private static bool IsMonoSingleton(Type t, out Type baseType)
	{
		if (t.BaseType != null)
		{
			t = t.BaseType;
			if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(MonoSingleton<>))
			{
				baseType = t.GetGenericArguments()[0];
				return true;
			}
			else
			{
				return IsMonoSingleton(t, out baseType);
			}
		}
		else
		{
			baseType = t;
			return false;
		}
	}

	private static void CheckMethodFromMonoSingleton(string methodString, Type type, Type singletonBaseType)
	{
		BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		MethodInfo method = type.GetMethod(methodString, bindingFlags);

		Type baseDeclaringType = typeof(MonoSingleton<>).MakeGenericType(singletonBaseType);

		if (method.GetBaseDefinition().DeclaringType != baseDeclaringType)
		{
			Debug.LogError(string.Format("{0} declare its function {1} on {2}, it should be declared on {3}",
							type.Name,
							method.Name,
							method.GetBaseDefinition().DeclaringType,
							baseDeclaringType));


			// try to skip on "new" method, but failed
			// http://msdn.microsoft.com/en-us/library/system.reflection.methodbase.ishidebysig%28v=vs.110%29.aspx
			//foreach (var m in type.GetMethods(bindingFlags))
			//{
			//    if (m.Name == methodString)
			//        Debug.Log((m.Attributes & MethodAttributes.VtableLayoutMask) == MethodAttributes.NewSlot);
			//}
		}
	}
}