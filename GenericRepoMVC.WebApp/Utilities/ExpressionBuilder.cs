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
    }
}
