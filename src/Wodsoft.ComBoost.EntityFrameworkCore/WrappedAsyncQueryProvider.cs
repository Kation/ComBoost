using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Linq;

namespace Wodsoft.ComBoost.Data.Entity
{
    public class WrappedAsyncQueryProvider : IQueryProvider, IWrappedAsyncQueryProvider, IAsyncQueryProvider
    {
        public WrappedAsyncQueryProvider(IQueryProvider queryProvider, Expression sourceExpression)
        {
            SourceProvider = queryProvider ?? throw new ArgumentNullException(nameof(queryProvider));
            SourceExpression = sourceExpression;
        }

        public IQueryProvider SourceProvider { get; }

        public Expression SourceExpression { get; }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new WrappedAsyncQueryable<TElement>(expression, this);
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return (IQueryable)Activator.CreateInstance(typeof(WrappedAsyncQueryable<>).MakeGenericType(expression.Type.GetGenericArguments()[0]), expression, this)!;
        }

        public object? Execute(Expression expression)
        {
            expression = new WrappedAsyncExpressionVisitor().Visit(expression);
            return SourceProvider.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            expression = new WrappedAsyncExpressionVisitor().Visit(expression);
            return SourceProvider.Execute<TResult>(expression);
        }

        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken token)
        {
            //expression = new WrappedAsyncExpressionVisitor(SourceExpression).Visit(expression);
            if (expression is MethodCallExpression methodExpression && methodExpression.Method.DeclaringType == typeof(QueryableExtensions))
            {
                var visitor = new WrappedAsyncExpressionVisitor();
                expression = visitor.Visit(expression);
                methodExpression = (MethodCallExpression)expression;
                List<object?> parameters = new List<object?> { SourceProvider.CreateQuery(methodExpression.Arguments[0]) };
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
                            if (methodExpression.Method.GetParameters().Length == 2)
                                parameters.Add(((ConstantExpression)methodExpression.Arguments[1]).Value);
                            else
                            {
                                parameters.Add(((UnaryExpression)methodExpression.Arguments[1]).Operand);
                                parameters.Add(((ConstantExpression)methodExpression.Arguments[2]).Value);
                            }
                            break;
                        }
                    case "AverageAsync":
                    case "MaxAsync":
                    case "MinAsync":
                    case "SumAsync":
                        {
                            if (methodExpression.Method.GetParameters().Length == 2)
                                parameters.Add(((ConstantExpression)methodExpression.Arguments[1]).Value);
                            else
                            {
                                parameters.Add(((UnaryExpression)methodExpression.Arguments[1]).Operand);
                                parameters.Add(((ConstantExpression)methodExpression.Arguments[2]).Value);
                            }
                            break;
                        }
                    case "ToArrayAsync":
                    case "ToListAsync":
                        parameters.Add(((ConstantExpression)methodExpression.Arguments[1]).Value);
                        break;
                    case "ToDictionaryAsync":
                        {
                            for (int i = 1; i < methodExpression.Arguments.Count; i++)
                            {
                                parameters.Add(((ConstantExpression)methodExpression.Arguments[i]).Value);
                            }
                            break;
                        }
                    default:
                        throw new NotSupportedException("不支持此方法。");
                }
                return (Task<TResult>)GetFunc(methodExpression.Method)(parameters);
            }
            return ((Microsoft.EntityFrameworkCore.Query.IAsyncQueryProvider)SourceProvider).ExecuteAsync<Task<TResult>>(expression, token);
        }

        private static ConcurrentDictionary<MethodInfo, Func<List<object?>, Task>> _func = new ConcurrentDictionary<MethodInfo, Func<List<object?>, Task>>();
        private static Func<List<object?>, Task> GetFunc(MethodInfo method)
        {
            return _func.GetOrAdd(method, _ =>
            {
                ParameterExpression args = Expression.Parameter(typeof(List<object?>), "args");
                List<Expression> parameters = new List<Expression>();
                int i = 0;
                foreach (var p in method.GetParameters())
                {
                    parameters.Add(Expression.Convert(Expression.Property(args, args.Type.GetProperty("Item")!, Expression.Constant(i)), p.ParameterType));
                    i++;
                }
                return Expression.Lambda<Func<List<object?>, Task>>(Expression.Call(method, parameters), args).Compile();
            });
        }

        TResult IAsyncQueryProvider.ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            return ((IAsyncQueryProvider)SourceProvider).ExecuteAsync<TResult>(new WrappedAsyncExpressionVisitor().Visit(expression), cancellationToken);
        }
    }
}
