using GenericRepoMVC.Domain.Models;
using GenericRepoMVC.ViewModels;
using System.Linq.Expressions;

namespace GenericRepoMVC.WebApp.Utilities
{
    public static class ExpressionBuilder
    {
        public static Expression<Func<T, bool>> CreateFilter<T, TRequest>(TRequest request) 
        {
            var parameterExp = Expression.Parameter(typeof(T), "p");
            var expressions = new List<Expression>();
            
            //const for FROM TO convension
            const string PROP_DT_FROM_NAME = "From";
            const string PROP_DT_TO_NAME = "To";

            foreach (var prop in typeof(TRequest).GetProperties())
            {
                var value = prop.GetValue(request);
                if (value == null || (value is string str && string.IsNullOrEmpty(str))) 
                {
                    continue;
                }

                MemberExpression? propertyExp = null;
                var valueExp = Expression.Constant(value);

                if (prop.PropertyType == typeof(string))
                {
                    propertyExp = Expression.Property(parameterExp, prop.Name);
                    expressions.Add(Expression.Call(propertyExp!, "Contains", null, valueExp));
                }
                // FROM TO convension
                else if (prop.Name.EndsWith(PROP_DT_FROM_NAME))
                {
                    propertyExp = Expression.Property(parameterExp, prop.Name.Replace(PROP_DT_FROM_NAME, ""));
                    expressions.Add(Expression.GreaterThanOrEqual(propertyExp, valueExp));
                }
                else if (prop.Name.EndsWith(PROP_DT_TO_NAME))
                {
                    propertyExp = Expression.Property(parameterExp, prop.Name.Replace(PROP_DT_TO_NAME, ""));
                    expressions.Add(Expression.LessThanOrEqual(propertyExp, valueExp));
                }
                //Normal type
                else
                {
                    propertyExp = Expression.Property(parameterExp, prop.Name);
                    expressions.Add(Expression.Equal(propertyExp!, valueExp));
                }
            }

            Expression body = expressions.Count > 0
                ? expressions.Aggregate(Expression.AndAlso)
                : Expression.Constant(true);

            var lambda = Expression.Lambda<Func<T, bool>>(body, parameterExp);
            return lambda;
        }

        public static Func<IQueryable<T>, IOrderedQueryable<T>>? CreateOrderBy<T, TRequest>(TRequest? orderByRequest)
        {
            if (orderByRequest == null || areAllPropertiesDefault(orderByRequest))
            {
                return null;
            }

            var sortedRequestDict = getPropertiesNameInOrder(orderByRequest);

            IOrderedQueryable<T> ApplyOrdering(IQueryable<T> query, string propertyName, bool isFirst)
            {
                var parameter = Expression.Parameter(typeof(T), "p");
                var property = Expression.Property(parameter, propertyName);
                var lambda = Expression.Lambda<Func<T, object>>(Expression.Convert(property, typeof(object)), parameter);

                if (isFirst)
                {
                    return query.OrderBy(lambda);
                }
                else
                {
                    return ((IOrderedQueryable<T>)query).ThenBy(lambda);
                }
            }

            return query =>
            {
                IOrderedQueryable<T>? orderedQuery = null;

                foreach (var propertyName in sortedRequestDict)
                {
                    if (orderedQuery == null)
                    {
                        orderedQuery = ApplyOrdering(query, propertyName, true);
                    }
                    else
                    {
                        orderedQuery = ApplyOrdering(orderedQuery, propertyName, false);
                    }
                }

                return orderedQuery!;
            };
        }

        private static bool areAllPropertiesDefault<TRequest>(TRequest orderByRequest)
        {
            var props = orderByRequest!.GetType().GetProperties();
            foreach (var prop in props)
            {
                var value = prop.GetValue(orderByRequest);
                var defaultValue = GetDefaultValue(prop.PropertyType);

                if (!Equals(value, defaultValue))
                {
                    return false;
                }
            }
            return true;
        }

        private static object? GetDefaultValue(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        private static IEnumerable<string> getPropertiesNameInOrder<TRequest>(TRequest orderByRequest)
        {
            // 0 0 1 2 
            //id fname lname date
            var props = orderByRequest!.GetType().GetProperties(); //prop names 4 orderby
            var orderPriorityPairs = new Dictionary<string, int>();
            foreach (var prop in props)
            {
                var value = (int)prop.GetValue(orderByRequest)!;
                if (value != 0)
                {
                    orderPriorityPairs.Add(prop.Name.Replace("OrderBy", ""), value);
                }
            }
            var sortedDict = orderPriorityPairs
                .OrderBy(e => e.Value)
                .ToDictionary();
            return sortedDict.Keys.ToArray();
        }
    }
}
