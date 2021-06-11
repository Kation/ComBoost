using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Data.Entity
{
    public class WrappedAsyncQueryProvider : IAsyncQueryProvider
    {
        public WrappedAsyncQueryProvider(IQueryProvider queryProvider, Expression sourceExpression)
        {
            SourceProvider = queryProvider ?? throw new ArgumentNullException(nameof(queryProvider));
            SourceExpression = sourceExpression;
        }

        public IQueryProvider SourceProvider { get; }

        public Expression SourceExpression { get; }

        public IAsyncQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new WrappedAsyncQueryable<TElement>(expression, this);
        }

        public ValueTask<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken token)
        {
            expression = new WrappedAsyncExpressionVisitor(SourceExpression).Visit(expression);
            if (expression is MethodCallExpression methodExpression)
            {
                switch (methodExpression.Method.Name)
                {
                    case "AllAsync":
                    case "AnyAsync":
                    case "CountAsync":
                    case "LongCountAsync":
                    case "FirstAsync":
                    case "FirstOrDefaultAsync":
                    case "LastAsync":
                    case "LastOrDefaultAsync":
                    case "SingleAsync":
                    case "SingleOrDefaultAsync":
                        {
                            List<object> parameters = new List<object> { SourceProvider.CreateQuery(methodExpression.Arguments[0]) };
                            if (methodExpression.Method.GetParameters().Length == 2)
                                parameters.Add(((ConstantExpression)methodExpression.Arguments[1]).Value);
                            else
                            {
                                parameters.Add(((UnaryExpression)methodExpression.Arguments[1]).Operand);
                                parameters.Add(((ConstantExpression)methodExpression.Arguments[2]).Value);
                            }
                            return new ValueTask<TResult>((Task<TResult>)methodExpression.Method.Invoke(null, parameters.ToArray()));
                        }
                    case "AverageAsync":
                    case "MaxAsync":
                    case "MinAsync":
                    case "SumAsync":
                        {
                            List<object> parameters = new List<object> { SourceProvider.CreateQuery(methodExpression.Arguments[0]) };
                            if (methodExpression.Method.GetParameters().Length == 2)
                                parameters.Add(((ConstantExpression)methodExpression.Arguments[1]).Value);
                            else
                            {
                                parameters.Add(((UnaryExpression)methodExpression.Arguments[1]).Operand);
                                parameters.Add(((ConstantExpression)methodExpression.Arguments[2]).Value);
                            }
                            return new ValueTask<TResult>((Task<TResult>)methodExpression.Method.Invoke(null, parameters.ToArray()));
                        }
                    case "ToArrayAsync":
                    case "ToListAsync":
                        return new ValueTask<TResult>((Task<TResult>)methodExpression.Method.Invoke(null, new object[] { SourceProvider.CreateQuery(methodExpression.Arguments[0]), ((ConstantExpression)methodExpression.Arguments[1]).Value }));
                    case "ToDictionaryAsync":
                        {
                            List<object> parameters = new List<object> { SourceProvider.CreateQuery(methodExpression.Arguments[0]) };
                            for (int i = 1; i < methodExpression.Arguments.Count; i++)
                            {
                                parameters.Add(((ConstantExpression)methodExpression.Arguments[i]).Value);
                            }
                            return new ValueTask<TResult>((Task<TResult>)methodExpression.Method.Invoke(null, parameters.ToArray()));
                        }
                }
            }
            //if (expression.Type == typeof(Task<TResult>))
            //{
            //    return new ValueTask<TResult>((Task<TResult>)Expression.Lambda(expression).Compile().DynamicInvoke());
            //}
            return new ValueTask<TResult>(((Microsoft.EntityFrameworkCore.Query.IAsyncQueryProvider)SourceProvider).ExecuteAsync<Task<TResult>>(expression, token));
        }
    }
}
