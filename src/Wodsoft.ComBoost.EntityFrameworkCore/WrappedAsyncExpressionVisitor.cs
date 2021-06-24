using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Wodsoft.ComBoost.Data.Entity
{
    public class WrappedAsyncExpressionVisitor : ExpressionVisitor
    {
        private Expression _root;

        public WrappedAsyncExpressionVisitor(Expression root)
        {
            _root = root;
        }

        private static readonly MethodInfo _ToArrayAsync = typeof(EntityFrameworkQueryableExtensions).GetMethod("ToArrayAsync");

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(AsyncQueryable))
            {
                switch (node.Method.Name)
                {
                    #region QueryMethod
                    case "Distinct":
                        {
                            var method = MapMethod(node.Method);
                            return method.GetParameters().Length == 1 ?
                                Expression.Call(method, Visit(node.Arguments[0]))
                                : Expression.Call(method, Visit(node.Arguments[0]), node.Arguments[1]);
                        }
                    case "OrderBy":
                    case "OrderByDescending":
                        {
                            var method = MapMethod(node.Method);
                            return method.GetParameters().Length == 2 ?
                                Expression.Call(method, Visit(node.Arguments[0]), node.Arguments[1])
                                : Expression.Call(method, Visit(node.Arguments[0]), node.Arguments[1], node.Arguments[2]);
                        }
                    case "ThenBy":
                    case "ThenByDescending":
                        {
                            var method = MapMethod(node.Method);
                            return method.GetParameters().Length == 2 ?
                                Expression.Call(method, Visit(node.Arguments[0]), node.Arguments[1])
                                : Expression.Call(method, Visit(node.Arguments[0]), node.Arguments[1], node.Arguments[2]);
                        }
                    case "Select":
                        {
                            var method = MapMethod(node.Method);
                            return Expression.Call(method, Visit(node.Arguments[0]), node.Arguments[1]);
                        }
                    case "Reverse":
                        {
                            var method = MapMethod(node.Method);
                            return Expression.Call(method, Visit(node.Arguments[0]));
                        }
                    case "Skip":
                    case "SkipLast":
                    case "Take":
                    case "TakeLast":
                        {
                            var method = MapMethod(node.Method);
                            return Expression.Call(method, Visit(node.Arguments[0]), node.Arguments[1]);
                        }
                    case "SkipWhile":
                    case "TakeWhile":
                    case "Where":
                        {
                            var method = MapMethod(node.Method);
                            return method.GetParameters().Length == 2 ?
                                Expression.Call(method, Visit(node.Arguments[0]), node.Arguments[1])
                                : Expression.Call(method, Visit(node.Arguments[0]), node.Arguments[1], node.Arguments[2]);
                        }
                    #endregion
                    #region ReadMethod
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
                            var method = MapMethod(node.Method);
                            return method.GetParameters().Length == 2 ?
                                Expression.Call(method, Visit(node.Arguments[0]), node.Arguments[1])
                                : Expression.Call(method, Visit(node.Arguments[0]), node.Arguments[1], node.Arguments[2]);
                        }
                    case "AverageAsync":
                    case "SumAsync":
                        {
                            var method = MapMethod(node.Method);
                            return method.GetParameters().Length == 2 ?
                                Expression.Call(method, Visit(node.Arguments[0]), node.Arguments[1])
                                : Expression.Call(method, Visit(node.Arguments[0]), node.Arguments[1], node.Arguments[2]);
                        }
                    case "MaxAsync":
                    case "MinAsync":
                        {
                            var method = MapMethod(node.Method);
                            return method.GetParameters().Length == 2 ?
                                Expression.Call(method, Visit(node.Arguments[0]), node.Arguments[1])
                                : Expression.Call(method, Visit(node.Arguments[0]), node.Arguments[1], node.Arguments[2]);
                        }
                    case "ToArrayAsync":
                    case "ToListAsync":
                        {
                            var method = MapMethod(node.Method);
                            return Expression.Call(method, Visit(node.Arguments[0]), node.Arguments[1]);
                        }
                    case "ToDictionaryAsync":
                        {
                            var method = MapMethod(node.Method);
                            if (node.Method.GetGenericArguments().Length == 2)
                            {
                                return method.GetParameters().Length == 3 ?
                                    Expression.Call(method, Visit(node.Arguments[0]), Expression.Constant(((LambdaExpression)((UnaryExpression)node.Arguments[1]).Operand).Compile(), node.Arguments[1].Type.GenericTypeArguments[0]), node.Arguments[2])
                                    : Expression.Call(method, Visit(node.Arguments[0]), Expression.Constant(((LambdaExpression)((UnaryExpression)node.Arguments[1]).Operand).Compile(), node.Arguments[1].Type.GenericTypeArguments[0]), node.Arguments[2], node.Arguments[3]);
                            }
                            else
                            {
                                return method.GetParameters().Length == 4 ?
                                    Expression.Call(method, Visit(node.Arguments[0]), Expression.Constant(((LambdaExpression)((UnaryExpression)node.Arguments[1]).Operand).Compile(), node.Arguments[1].Type.GenericTypeArguments[0]), Expression.Constant(((LambdaExpression)((UnaryExpression)node.Arguments[2]).Operand).Compile(), node.Arguments[2].Type.GenericTypeArguments[0]), node.Arguments[3])
                                    : Expression.Call(method, Visit(node.Arguments[0]), Expression.Constant(((LambdaExpression)((UnaryExpression)node.Arguments[1]).Operand).Compile(), node.Arguments[1].Type.GenericTypeArguments[0]), Expression.Constant(((LambdaExpression)((UnaryExpression)node.Arguments[2]).Operand).Compile(), node.Arguments[2].Type.GenericTypeArguments[0]), node.Arguments[3], node.Arguments[4]);
                            }
                        }
                    #endregion
                    default:
                        throw new NotSupportedException($"Can not support method \"{node.Method.DeclaringType}.{node.Method.Name}\".");
                }
            }
            else if (node.Method.DeclaringType == typeof(EntityContextExtensions))
            {
                switch (node.Method.Name)
                {
                    case "Include":
                    case "ThenInclude":
                        {
                            var method = MapMethod(node.Method);
                            return Expression.Call(method, Visit(node.Arguments[0]), node.Arguments[1]);
                        }
                    default:
                        throw new NotSupportedException($"Can not support method \"{node.Method.DeclaringType}.{node.Method.Name}\".");
                }
            }
            return base.VisitMethodCall(node);
        }

        protected override Expression VisitExtension(Expression node)
        {
            if (node is WrappedAsyncQueryExpression)
                return _root;
            return base.VisitExtension(node);
        }

        private static ConcurrentDictionary<MethodInfo, MethodInfo> _Mapping = new ConcurrentDictionary<MethodInfo, MethodInfo>();
        private static MethodInfo MapMethod(MethodInfo value)
        {
            return _Mapping.GetOrAdd(value, method =>
            {
                if (method.DeclaringType == typeof(AsyncQueryable))
                {
                    switch (method.Name)
                    {
                        #region QueryMethod
                        case "Distinct":
                            return (method.GetParameters().Length == 1 ?
                                typeof(Queryable).GetMethod(method.Name, 1, new Type[] { typeof(IQueryable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)) })
                                : typeof(Queryable).GetMethod(method.Name, 1, new Type[] { typeof(IQueryable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)), typeof(IEqualityComparer<>).MakeGenericType(Type.MakeGenericMethodParameter(0)) }))
                                .MakeGenericMethod(method.GetGenericArguments()[0]);
                        case "OrderBy":
                        case "OrderByDescending":
                            return (method.GetParameters().Length == 2 ?
                                typeof(Queryable).GetMethod(method.Name, 2, new Type[] { typeof(IQueryable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)), typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(Type.MakeGenericMethodParameter(0), Type.MakeGenericMethodParameter(1))) })
                                : typeof(Queryable).GetMethod(method.Name, 2, new Type[] { typeof(IQueryable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)), typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(Type.MakeGenericMethodParameter(0), Type.MakeGenericMethodParameter(1)), typeof(IComparer<>).MakeGenericType(Type.MakeGenericMethodParameter(1))) }))
                                .MakeGenericMethod(method.GetGenericArguments());
                        case "ThenBy":
                        case "ThenByDescending":
                            return (method.GetParameters().Length == 2 ?
                                typeof(Queryable).GetMethod(method.Name, 2, new Type[] { typeof(IOrderedQueryable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)), typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(Type.MakeGenericMethodParameter(0), Type.MakeGenericMethodParameter(1))) })
                                : typeof(Queryable).GetMethod(method.Name, 2, new Type[] { typeof(IOrderedQueryable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)), typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(Type.MakeGenericMethodParameter(0), Type.MakeGenericMethodParameter(1)), typeof(IComparer<>).MakeGenericType(Type.MakeGenericMethodParameter(1))) }))
                                .MakeGenericMethod(method.GetGenericArguments());
                        case "Select":
                            return (method.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericArguments().Length == 2 ?
                                typeof(Queryable).GetMethod("Select", 2, new Type[] { typeof(IQueryable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)), typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(Type.MakeGenericMethodParameter(0), Type.MakeGenericMethodParameter(1))) })
                                : typeof(Queryable).GetMethod("Select", 2, new Type[] { typeof(IQueryable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)), typeof(Expression<>).MakeGenericType(typeof(Func<,,>).MakeGenericType(Type.MakeGenericMethodParameter(0), typeof(int), Type.MakeGenericMethodParameter(1))) }))
                                .MakeGenericMethod(method.GetGenericArguments());
                        case "Reverse":
                        case "Skip":
                        case "SkipLast":
                        case "Take":
                        case "TakeLast":
                            return typeof(Queryable).GetMethod(method.Name).MakeGenericMethod(method.GetGenericArguments());
                        case "SkipWhile":
                        case "TakeWhile":
                        case "Where":
                            return (method.GetParameters().Length == 2 ?
                                typeof(Queryable).GetMethod(method.Name, 1, new Type[] { typeof(IQueryable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)), typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(Type.MakeGenericMethodParameter(0), typeof(bool))) })
                                : typeof(Queryable).GetMethod(method.Name, 1, new Type[] { typeof(IQueryable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)), typeof(Expression<>).MakeGenericType(typeof(Func<,,>).MakeGenericType(Type.MakeGenericMethodParameter(0), typeof(int), typeof(bool))) }))
                                .MakeGenericMethod(method.GetGenericArguments());
                        #endregion
                        #region ReadMethod
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
                            return (method.GetParameters().Length == 2 ?
                                typeof(EntityFrameworkQueryableExtensions).GetMethod(method.Name, 1, new Type[] { typeof(IQueryable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)), typeof(CancellationToken) })
                                : typeof(EntityFrameworkQueryableExtensions).GetMethod(method.Name, 1, new Type[] { typeof(IQueryable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)), typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(Type.MakeGenericMethodParameter(0), typeof(bool))), typeof(CancellationToken) }))
                                .MakeGenericMethod(method.GetGenericArguments());
                        case "AverageAsync":
                        case "SumAsync":
                            return (method.GetParameters().Length == 2 ?
                                typeof(EntityFrameworkQueryableExtensions).GetMethod(method.Name, new Type[] { typeof(IQueryable<>).MakeGenericType(method.GetParameters()[0].ParameterType.GetGenericArguments()[0]), typeof(CancellationToken) })
                                : typeof(EntityFrameworkQueryableExtensions).GetMethod(method.Name, 1, new Type[] { typeof(IQueryable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)), typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(Type.MakeGenericMethodParameter(0), method.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericArguments()[1])), typeof(CancellationToken) }).MakeGenericMethod(method.GetGenericArguments()[0]));
                        case "MaxAsync":
                        case "MinAsync":
                            return (method.GetParameters().Length == 2 ?
                                typeof(EntityFrameworkQueryableExtensions).GetMethod(method.Name, 1, new Type[] { typeof(IQueryable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)), typeof(CancellationToken) }).MakeGenericMethod(method.ReturnType.GetGenericArguments()[0])
                                : typeof(EntityFrameworkQueryableExtensions).GetMethod(method.Name, 2, new Type[] { typeof(IQueryable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)), typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(Type.MakeGenericMethodParameter(0), Type.MakeGenericMethodParameter(1))), typeof(CancellationToken) }).MakeGenericMethod(method.GetGenericArguments()[0], method.ReturnType.GetGenericArguments()[0]));
                        case "ToArrayAsync":
                        case "ToListAsync":
                            return typeof(EntityFrameworkQueryableExtensions).GetMethod(method.Name).MakeGenericMethod(method.GetGenericArguments());
                        case "ToDictionaryAsync":
                            if (method.GetGenericArguments().Length == 2)
                            {
                                var type1 = Type.MakeGenericMethodParameter(0);
                                var type2 = Type.MakeGenericMethodParameter(1);
                                return (method.GetParameters().Length == 3 ?
                                    typeof(EntityFrameworkQueryableExtensions).GetMethod(method.Name, 2, new Type[] { typeof(IQueryable<>).MakeGenericType(type1), typeof(Func<,>).MakeGenericType(type1, type2), typeof(CancellationToken) })
                                    : typeof(EntityFrameworkQueryableExtensions).GetMethod(method.Name, 2, new Type[] { typeof(IQueryable<>).MakeGenericType(type1), typeof(Func<,>).MakeGenericType(type1, type2), typeof(IEqualityComparer<>).MakeGenericType(type2), typeof(CancellationToken) }))
                                    .MakeGenericMethod(method.GetGenericArguments());
                            }
                            else
                            {
                                var type1 = Type.MakeGenericMethodParameter(0);
                                var type2 = Type.MakeGenericMethodParameter(1);
                                var type3 = Type.MakeGenericMethodParameter(2);
                                return (method.GetParameters().Length == 4 ?
                                    typeof(EntityFrameworkQueryableExtensions).GetMethod(method.Name, 3, new Type[] { typeof(IQueryable<>).MakeGenericType(type1), typeof(Func<,>).MakeGenericType(type1, type2), typeof(Func<,>).MakeGenericType(type1, type3), typeof(CancellationToken) })
                                    : typeof(EntityFrameworkQueryableExtensions).GetMethod(method.Name, 3, new Type[] { typeof(IQueryable<>).MakeGenericType(type1), typeof(Func<,>).MakeGenericType(type1, type2), typeof(Func<,>).MakeGenericType(type1, type3), typeof(IEqualityComparer<>).MakeGenericType(type2), typeof(CancellationToken) }))
                                    .MakeGenericMethod(method.GetGenericArguments());
                            }
                        #endregion
                        default:
                            return null;
                    }
                }
                else if (method.DeclaringType == typeof(EntityContextExtensions))
                {
                    switch (method.Name)
                    {
                        case "Include":
                            return (method.GetGenericArguments().Length == 1 ?
                                typeof(EntityFrameworkQueryableExtensions).GetMethod(method.Name, 1, new Type[] { typeof(IQueryable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)), typeof(string) })
                                : typeof(EntityFrameworkQueryableExtensions).GetMethod(method.Name, 2, new Type[] { typeof(IQueryable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)), typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(Type.MakeGenericMethodParameter(0), Type.MakeGenericMethodParameter(1))) })
                                ).MakeGenericMethod(method.GetGenericArguments());
                        case "ThenInclude":
                            return (method.GetParameters()[0].ParameterType.GetGenericArguments()[1] == method.GetGenericArguments()[1] ?
                                typeof(EntityFrameworkQueryableExtensions).GetMethod(method.Name, 3, new Type[] { typeof(IIncludableQueryable<,>).MakeGenericType(Type.MakeGenericMethodParameter(0), Type.MakeGenericMethodParameter(1)), typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(Type.MakeGenericMethodParameter(1), Type.MakeGenericMethodParameter(2))) })
                                : typeof(EntityFrameworkQueryableExtensions).GetMethod(method.Name, 3, new Type[] { typeof(IIncludableQueryable<,>).MakeGenericType(Type.MakeGenericMethodParameter(0), typeof(IEnumerable<>).MakeGenericType(Type.MakeGenericMethodParameter(1))), typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(Type.MakeGenericMethodParameter(2), Type.MakeGenericMethodParameter(3))) })
                                ).MakeGenericMethod(method.GetGenericArguments());
                        default:
                            return null;
                    }
                }
                else
                    return null;
            });
        }
    }
}
