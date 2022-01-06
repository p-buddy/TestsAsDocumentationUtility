using System;

namespace pbuddy.TypeUtility.RuntimeScripts
{
    public static class GenericMethod
    {
        public static readonly Type[] Arguments =
        {
            typeof(GenericMethodArgument0),
            typeof(GenericMethodArgument1),
            typeof(GenericMethodArgument2),
            typeof(GenericMethodArgument3),
            typeof(GenericMethodArgument4),
            typeof(GenericMethodArgument5),
        };
    }
    
    public struct GenericMethodArgument0 : IGenericMethodArgument
    {
    }
    
    public struct GenericMethodArgument1 : IGenericMethodArgument
    {
    }
    
    public struct GenericMethodArgument2 : IGenericMethodArgument
    {
    }
    
    public struct GenericMethodArgument3 : IGenericMethodArgument
    {
    }
    
    public struct GenericMethodArgument4 : IGenericMethodArgument
    {
    }
    
    public struct GenericMethodArgument5 : IGenericMethodArgument
    {
    }
}