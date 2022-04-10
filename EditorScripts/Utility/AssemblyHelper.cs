using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    public static class AssemblyHelper
    {
        private const BindingFlags PermissiveFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static | 
                                                     BindingFlags.DeclaredOnly | BindingFlags.NonPublic;
        public static Assembly[] GetDependentAssemblies(this Assembly assembly)
        {
            bool IsDependencyAssembly(AssemblyName assemblyRef) => assemblyRef.Name == assembly.GetName().Name;

            bool DoesReferenceDependency(Assembly otherAssembly)
            { 
                return otherAssembly.GetReferencedAssemblies().ToList().Any(IsDependencyAssembly);
            }
            
            return AppDomain.CurrentDomain.GetAssemblies()
                            .ToList()
                            .Where(DoesReferenceDependency)
                            .ToArray();
        }
        
        public static List<MemberInfo> GetAllMembersWithAndWithout(this Assembly assembly,
                                                                   in With<Attribute> with,
                                                                   in Without<Attribute> without)
        {
            var members = new List<MemberInfo>();

            foreach (Type type in assembly.GetTypes())
            {
                members.AddIfMemberSatisfies(type, in with, in without);
                foreach (MemberInfo memberInfo in type.GetMembers(PermissiveFlags))
                {
                    members.AddIfMemberSatisfies(memberInfo, in with, in without);
                }
            }
            
            return members;
        }
        
        private static void AddIfMemberSatisfies(this List<MemberInfo> list, MemberInfo memberInfo, in With<Attribute> with, in Without<Attribute> without)
        {
            if (list.Contains(memberInfo)) return;
            
            foreach (var attr in with.Types)
            {
                if (memberInfo.GetCustomAttribute(attr) is null) return;
            }
                    
            foreach (var attr in without.Types)
            {
                if (memberInfo.GetCustomAttribute(attr) != null) return;
            }
                    
            list.Add(memberInfo);
        }
    }
}