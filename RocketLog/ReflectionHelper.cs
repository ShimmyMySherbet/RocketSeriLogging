using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace RocketLog
{
    public static class ReflectionHelper
    {
        public static HarmonyMethod FindHMethod<ClassType>(string Name) => new HarmonyMethod(FindMethod<ClassType>(Name));

        public static HarmonyMethod FindHMethod<ClassType, Arg1>(string Name) => new HarmonyMethod(FindMethod<ClassType, Arg1>(Name));

        public static HarmonyMethod FindHMethod<ClassType, Arg1, Arg2>(string Name) => new HarmonyMethod(FindMethod<ClassType, Arg1, Arg2>(Name));

        public static HarmonyMethod FindHMethod<ClassType, Arg1, Arg2, Arg3>(string Name) => new HarmonyMethod(FindMethod<ClassType, Arg1, Arg2, Arg3>(Name));

        public static MethodInfo FindMethod(string Name, Type ClassType)
        {
            return ClassType.GetMethod(Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
        }

        public static MethodInfo FindMethod<Arg1>(string Name, Type ClassType)
        {
            return FindMatch(ClassType, Name, typeof(Arg1));
        }

        public static MethodInfo FindMethod<Arg1, Arg2>(string Name, Type ClassType)
        {
            return FindMatch(ClassType, Name, typeof(Arg1), typeof(Arg2));
        }
        public static MethodInfo FindMethod<Arg1, Arg2, Arg3>(string Name, Type ClassType)
        {
            return FindMatch(ClassType, Name, typeof(Arg1), typeof(Arg2), typeof(Arg3));
        }

        public static MethodInfo FindMethod<ClassType>(string Name)
        {
            return typeof(ClassType).GetMethod(Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
        }

        public static MethodInfo FindMethod<ClassType, Arg1>(string Name)
        {
            return FindMatch(typeof(ClassType), Name, typeof(Arg1));

            //return typeof(ClassType).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).First((x) =>
            //{
            //    var Types = x.GetParameters();
            //    System.Console.WriteLine("test " + x.Name + $" {string.Join(", ", Types.Select(d => d.ParameterType.Name))}");
            //    System.Console.WriteLine("for  " + Name + $" {typeof(Arg1).Name}");
            //    if (!string.Equals(Name, x.Name, System.StringComparison.InvariantCultureIgnoreCase)) return false;
            //    if (Types.Length != 1) return false;
            //    if (Types[0].ParameterType != typeof(Arg1)) return false;
            //    return true;
        }

        public static MethodInfo FindMethod<ClassType, Arg1, Arg2>(string Name)
        {
            return FindMatch(typeof(ClassType), Name, typeof(Arg1), typeof(Arg2));

            //System.Console.WriteLine($"looking for {Name}");
            //return typeof(ClassType).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).First((x) =>
            //{
            //    var Types = x.GetParameters();
            //    System.Console.WriteLine("test " + x.Name + $" {string.Join(", ", Types.Select(d => d.ParameterType.Name))}");
            //    System.Console.WriteLine("for  " + Name + $" {typeof(Arg1).Name}, {typeof(Arg2).Name}");
            //    if (Types.Length != 2) return false;
            //    if (!string.Equals(Name, x.Name, System.StringComparison.InvariantCultureIgnoreCase)) return false;
            //    if (Types[0].ParameterType != typeof(Arg1)) return false;
            //    if (Types[1].ParameterType != typeof(Arg2)) return false;
            //    return true;
            //});
        }

        public static MethodInfo FindMethod<ClassType, Arg1, Arg2, Arg3>(string Name)
        {
            return FindMatch(typeof(ClassType), Name, typeof(Arg1), typeof(Arg2), typeof(Arg3));

            //return typeof(ClassType).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).First((x) =>
            //{
            //    var Types = x.GetParameters();
            //    System.Console.WriteLine("test " + x.Name + $" {string.Join(", ", Types.Select(d => d.ParameterType.Name))}");
            //    System.Console.WriteLine("for  " + Name + $" {typeof(Arg1).Name}, {typeof(Arg2).Name}, {typeof(Arg3).Name}");
            //    if (Types.Length != 3) return false;
            //    if (!string.Equals(Name, x.Name, System.StringComparison.InvariantCultureIgnoreCase)) return false;
            //    if (Types[0].ParameterType != typeof(Arg1)) return false;
            //    if (Types[1].ParameterType != typeof(Arg2)) return false;
            //    if (Types[2].ParameterType != typeof(Arg3)) return false;
            //    return true;
            //});
        }

        private static MethodInfo FindMatch(Type ClassType, string MethodName, params Type[] ParameterTypes)
        {
            return ClassType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).First((x) =>
            {
                var Types = x.GetParameters();
                if (Types.Length != ParameterTypes.Length) return false;
                if (!string.Equals(MethodName, x.Name, System.StringComparison.InvariantCultureIgnoreCase)) return false;
                for (int i = 0; i < ParameterTypes.Length; i++)
                {
                    if (ParameterTypes[i] != Types[i].ParameterType)
                    {
                        return false;
                    }
                }
                return true;
            });
        }
    }
}