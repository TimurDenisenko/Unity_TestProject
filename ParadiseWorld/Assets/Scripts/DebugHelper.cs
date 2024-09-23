using System;
using System.Reflection;

namespace Assets.Scripts
{
    public class DebugHelper
    {
        public static void ClearLog()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            Type type = assembly.GetType("UnityEditor.LogEntries");
            MethodInfo method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
        }
    }
}
