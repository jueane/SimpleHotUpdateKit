using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

public static class AssemblyExtension
{
    public static void Call(this Assembly assembly, string className, string methodName)
    {
        if (assembly == null)
        {
            throw new Exception($"{assembly.FullName} is null");
        }

        Type entryType = assembly.GetType(className);
        if (entryType == null)
            throw new Exception($"{className} not found");
        MethodInfo method = entryType.GetMethod(methodName);
        if (method == null)
            throw new Exception($"{methodName} not found");
        method.Invoke(null, null);
        Debug.Log($"Called method {className}.{method.DeclaringType.FullName}.");
    }

    public static IEnumerator Initialize(this Assembly assembly, string className)
    {
        if (assembly == null)
        {
            throw new Exception($"{assembly.FullName} is null");
        }

        Type entryType = assembly.GetType(className);
        if (entryType == null)
            throw new Exception($"{className} not found");
        if (Activator.CreateInstance(entryType) is IAssemblyInitializer initializer)
        {
            Debug.Log($"Calling method {className}.{nameof(initializer.Initialize)}");
            yield return initializer.Initialize();
        }
        else
        {
            throw new Exception($"{assembly}.{className} not implements {nameof(IAssemblyInitializer)}");
        }
    }
}
