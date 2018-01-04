using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LinqToSql.DynamicLambda
{
    public class ExpressionBuilder
    {
        private static readonly MethodInfo ContainsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
        private static readonly MethodInfo StartsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
        private static readonly MethodInfo EndsWithMethod = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });

        // Receive a collection of query filters and returns a lambda expression
        public static Expression<Func<T, bool>> GetExpression<T>(ICollection<QueryFilter> filters)
        {
            Expression exp = null;
            
            //Creates the parameter ex  the p in (p => p.something)
            var param = Expression.Parameter(typeof(T), "p");

            if (filters.Count == 0)
                return null;

            // Find out how many terms it has to build and join them if it is necessary 
            if (filters.Count != 1)
            {
                if (filters.Count == 2)
                    exp = GetExpression(param, filters.First(), filters.ElementAt(1));
                else
                {
                    while (filters.Count > 0)
                    {
                        var f1 = filters.First();
                        var f2 = filters.ElementAt(1);

                        exp = exp == null
                            ? GetExpression(param, f1, f2)
                            : Expression.AndAlso(exp, GetExpression(param, f1, f2));

                        filters.Remove(f1);
                        filters.Remove(f2);

                        if (filters.Count != 1) continue;

                        exp = Expression.AndAlso(exp, GetExpression(param, filters.First()));

                        filters.Remove(filters.First());
                    }
                }
            }
            else
                exp = GetExpression(param, filters.First());

            return Expression.Lambda<Func<T, bool>>(exp, param);
        }

        // Creates each lambda term 
        private static Expression GetExpression(Expression param, QueryFilter queryFilter)
        {
            // Creates the member. It is the part of field to be evaluated like (p => p.name)
            Expression member = queryFilter.PropertyName.Split('.')
                .Aggregate(param, Expression.PropertyOrField);

            // Creates the value that the member will be evaluated 
            ConstantExpression constant = Expression.Constant(queryFilter.Value);

            // Form the expression term like p.Name == "Ferrari"
            switch (queryFilter.Operator)
            {
                case Operator.Equals:
                    return Expression.Equal(member, constant);

                case Operator.Contains:
                    return Expression.Call(member, ContainsMethod, constant);

                case Operator.GreaterThan:
                    return Expression.GreaterThan(member, constant);

                case Operator.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(member, constant);

                case Operator.LessThan:
                    return Expression.LessThan(member, constant);

                case Operator.LessThanOrEqualTo:
                    return Expression.LessThanOrEqual(member, constant);

                case Operator.StartsWith:
                    return Expression.Call(member, StartsWithMethod, constant);

                case Operator.EndsWith:
                    return Expression.Call(member, EndsWithMethod, constant);
            }

            return null;
        }

        // Join to terms if need it 
        private static BinaryExpression GetExpression(Expression param, QueryFilter filter1, QueryFilter filter2)
        {
            Expression result1 = GetExpression(param, filter1);
            Expression result2 = GetExpression(param, filter2);
            return Expression.AndAlso(result1, result2);
        }
    }
}
