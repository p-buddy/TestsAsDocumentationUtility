using System;

namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    public readonly struct Without<TRequiredType>
    {
        public Type[] Types { get; }

        private Without(params Type[] types)
        {
            Types = types;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <returns></returns>
        public static Without<TRequiredType> This<T1>() where T1 : TRequiredType => new Without<TRequiredType>(typeof(T1));
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <returns></returns>
        public static Without<TRequiredType> These<T1, T2>() where T1 : TRequiredType where T2 : TRequiredType =>
            new Without<TRequiredType>(typeof(T1), typeof(T2));
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <returns></returns>
        public static Without<TRequiredType> These<T1, T2, T3>() where T1 : TRequiredType 
                                                                 where T2 : TRequiredType
                                                                 where T3 : TRequiredType =>
            new Without<TRequiredType> (typeof(T1), typeof(T2), typeof(T3));
    }
}