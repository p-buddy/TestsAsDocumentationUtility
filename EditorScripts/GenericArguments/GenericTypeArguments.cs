using System;

namespace pbuddy.TypeUtility.RuntimeScripts
{
    public static class GenericType
    {
        public static readonly Type[] Arguments =
        {
            typeof(GenericTypeArgument0),
            typeof(GenericTypeArgument1),
            typeof(GenericTypeArgument2),
            typeof(GenericTypeArgument3),
            typeof(GenericTypeArgument4),
            typeof(GenericTypeArgument5),
        };
    }
    
    public struct GenericTypeArgument0 : IGenericTypeArgument
    {
    }
    
    public struct GenericTypeArgument1 : IGenericTypeArgument
    {
    }
    
    public struct GenericTypeArgument2 : IGenericTypeArgument
    {
    }
    
    public struct GenericTypeArgument3 : IGenericTypeArgument
    {
    }
    
    public struct GenericTypeArgument4 : IGenericTypeArgument
    {
    }
    
    public struct GenericTypeArgument5 : IGenericTypeArgument
    {
    }
}