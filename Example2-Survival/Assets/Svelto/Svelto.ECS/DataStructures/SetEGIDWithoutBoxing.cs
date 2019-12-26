using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Svelto.ECS.Internal
{
    static class SetEGIDWithoutBoxing<T> where T : struct, IEntityStruct
    {
        internal delegate void ActionCast(ref T target, EGID egid);

        public static readonly ActionCast SetIDWithoutBoxing = MakeSetter();

        static ActionCast MakeSetter()
        {
            if (EntityBuilder<T>.HAS_EGID)
            {
#if !ENABLE_IL2CPP                
                Type myTypeA = typeof(T);
                PropertyInfo myFieldInfo = myTypeA.GetProperty("ID");

                ParameterExpression targetExp = Expression.Parameter(typeof(T).MakeByRefType(), "target");
                ParameterExpression valueExp = Expression.Parameter(typeof(EGID), "value");
                MemberExpression fieldExp = Expression.Property(targetExp, myFieldInfo);
                BinaryExpression assignExp = Expression.Assign(fieldExp, valueExp);

                var setter = Expression.Lambda<ActionCast>(assignExp, targetExp, valueExp).Compile();

                return setter;
#else        
                return new ActionCast((ref T target, EGID value) => { ((INeedEGID) target).ID = value; });
#endif
            }

            return null;
        }

        public static void Warmup()
        {         
        }
    }
}