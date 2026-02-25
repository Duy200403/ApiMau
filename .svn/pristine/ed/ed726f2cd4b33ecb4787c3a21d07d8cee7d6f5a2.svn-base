using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ApiWebsite.Helper
{
    public static class ReflectionQueryable
    {
        private static readonly MethodInfo OrderByMethod =
            typeof(Queryable).GetMethods()
                .Where(method => method.Name == "OrderBy")
                .Where(method => method.GetParameters().Length == 2)
                .Single();

        private static readonly MethodInfo OrderByMethodDescending =
            typeof(Queryable).GetMethods()
                .Where(method => method.Name == "OrderByDescending")
                .Where(method => method.GetParameters().Length == 2)
                .Single();


        private static readonly MethodInfo ThenByMethod =
            typeof(Queryable).GetMethods()
                .Where(method => method.Name == "ThenBy")
                .Where(method => method.GetParameters().Length == 2)
                .Single();

        private static readonly MethodInfo ThenByMethodDescending =
            typeof(Queryable).GetMethods()
                .Where(method => method.Name == "ThenByDescending")
                .Where(method => method.GetParameters().Length == 2)
                .Single();


        public static IQueryable<TSource> OrderByProperty<TSource>(this IQueryable<TSource> source, string propertyName, SortDirection direction = SortDirection.ASC)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(TSource), "Post");
            Expression orderByProperty = Expression.Property(parameter, (propertyName ?? "Id"));
            if (direction == SortDirection.DESC)
            {
                LambdaExpression lambda = Expression.Lambda(orderByProperty, new[] { parameter });
                MethodInfo genericMethod = OrderByMethodDescending.MakeGenericMethod(new[] { typeof(TSource), orderByProperty.Type });
                object ret = genericMethod.Invoke(null, new object[] { source, lambda });
                return (IQueryable<TSource>)ret;
            }
            else
            {
                LambdaExpression lambda = Expression.Lambda(orderByProperty, new[] { parameter });
                MethodInfo genericMethod = OrderByMethod.MakeGenericMethod(new[] { typeof(TSource), orderByProperty.Type });
                object ret = genericMethod.Invoke(null, new object[] { source, lambda });
                return (IQueryable<TSource>)ret;
            }
        }



        public static IQueryable<TSource> ThenByProperty<TSource>(this IQueryable<TSource> source, string propertyName, string direction)
        {
            if (!string.IsNullOrWhiteSpace(propertyName) && !string.IsNullOrWhiteSpace(direction))
            {
                ParameterExpression parameter = Expression.Parameter(typeof(TSource), "post");
                Expression orderByProperty = Expression.Property(parameter, propertyName);
                if ("DESC" == direction)
                {
                    LambdaExpression lambda = Expression.Lambda(orderByProperty, new[] { parameter });
                    MethodInfo method = ThenByMethodDescending;
                    MethodInfo genericMethod = method.MakeGenericMethod
                        (new[] { typeof(TSource), orderByProperty.Type });
                    object ret = genericMethod.Invoke(null, new object[] { source, lambda });
                    return (IQueryable<TSource>)ret;
                }
                else
                {
                    LambdaExpression lambda = Expression.Lambda(orderByProperty, new[] { parameter });
                    MethodInfo method = ThenByMethod;
                    MethodInfo genericMethod = method.MakeGenericMethod
                        (new[] { typeof(TSource), orderByProperty.Type });
                    object ret = genericMethod.Invoke(null, new object[] { source, lambda });
                    return (IQueryable<TSource>)ret;
                }
            }
            else
            {
                return source;
            }
        }
    }
}