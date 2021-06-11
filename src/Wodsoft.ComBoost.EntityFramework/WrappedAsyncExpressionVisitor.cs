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
                            return node.Method.GetParameters().Length == 1 ?
                                Expression.Call(method, Visit(node.Arguments[0]))
                                : Expression.Call(method, Visit(node.Arguments[0]), node.Arguments[1]);
                        }
                    case "OrderBy":
                    case "OrderByDescending":
                        {
                            var method = MapMethod(node.Method);
                            return node.Method.GetParameters().Length == 2 ?
                                Expression.Call(method, Visit(node.Arguments[0]), node.Arguments[1])
                                : Expression.Call(method, Visit(node.Arguments[0]), node.Arguments[1], node.Arguments[2]);
                        }
                    case "ThenBy":
                    case "ThenByDescending":
                        {
                            var method = MapMethod(node.Method);
                            return node.Method.GetParameters().Length == 2 ?
                                Expression.Call(method, Visit(node.Arguments[0]), node.Arguments[1])
                                : Expression.Call(method, Visit(node.Arguments[0]), node.Arguments[1], node.Arguments[2]);
                        }
                    case "Select":
                        {
                            var method = MapMethod(node.Method);
                            return Expression.Call(method, Visit(node.Arguments[0]), node.Arguments[1]);
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
                            return node.Method.GetParameters().Length == 2 ?
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
                    case "SingleAsync":
                    case "SingleOrDefaultAsync":
                        {
                            var method = MapMethod(node.Method);
                            return node.Method.GetParameters().Length == 2 ?
                                Expression.Call(method, Visit(node.Arguments[0]), node.Arguments[1])
                                : Expression.Call(method, Visit(node.Arguments[0]), node.Arguments[1], node.Arguments[2]);
                        }
                    case "AverageAsync":
                    case "SumAsync":
                        {
                            var method = MapMethod(node.Method);
                            return node.Method.GetParameters().Length == 2 ?
                                Expression.Call(method, Visit(node.Arguments[0]), node.Arguments[1])
                                : Expression.Call(method, Visit(node.Arguments[0]), node.Arguments[1], node.Arguments[2]);
                        }
                    case "MaxAsync":
                    case "MinAsync":
                        {
                            var method = MapMethod(node.Method);
                            return node.Method.GetParameters().Length == 2 ?
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
                        if (node.Method.GetGenericArguments().Length == 2)
                        {
                            var method = MapMethod(node.Method);
                            return node.Method.GetParameters().Length == 3 ?
                                Expression.Call(method, Visit(node.Arguments[0]), Expression.Constant(((LambdaExpression)((UnaryExpression)node.Arguments[1]).Operand).Compile(), node.Arguments[1].Type.GenericTypeArguments[0]), node.Arguments[2])
                                : Expression.Call(method, Visit(node.Arguments[0]), Expression.Constant(((LambdaExpression)((UnaryExpression)node.Arguments[1]).Operand).Compile(), node.Arguments[1].Type.GenericTypeArguments[0]), node.Arguments[2], node.Arguments[3]);
                        }
                        else
                        {
                            var method = MapMethod(node.Method);
                            return node.Method.GetParameters().Length == 4 ?
                                Expression.Call(method, Visit(node.Arguments[0]), Expression.Constant(((LambdaExpression)((UnaryExpression)node.Arguments[1]).Operand).Compile(), node.Arguments[1].Type.GenericTypeArguments[0]), Expression.Constant(((LambdaExpression)((UnaryExpression)node.Arguments[2]).Operand).Compile(), node.Arguments[2].Type.GenericTypeArguments[0]), node.Arguments[3])
                                : Expression.Call(method, Visit(node.Arguments[0]), Expression.Constant(((LambdaExpression)((UnaryExpression)node.Arguments[1]).Operand).Compile(), node.Arguments[1].Type.GenericTypeArguments[0]), Expression.Constant(((LambdaExpression)((UnaryExpression)node.Arguments[2]).Operand).Compile(), node.Arguments[2].Type.GenericTypeArguments[0]), node.Arguments[3], node.Arguments[4]);
                        }
                    #endregion
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
                switch (method.Name)
                {
                    #region QueryMethod
                    case "Distinct":
                    case "OrderBy":
                    case "OrderByDescending":
                    case "ThenBy":
                    case "ThenByDescending":
                        return typeof(Queryable).GetMember(method.Name).Cast<MethodInfo>().Where(t => t.GetParameters().Length == method.GetParameters().Length).First().MakeGenericMethod(method.GetGenericArguments());
                    case "Select":
                    case "SkipWhile":
                    case "TakeWhile":
                    case "Where":
                        return typeof(Queryable).GetMember(method.Name).Cast<MethodInfo>().Where(t => t.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericArguments().Length == method.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericArguments().Length).First().MakeGenericMethod(method.GetGenericArguments());
                    case "Skip":
                    case "SkipLast":
                    case "Take":
                    case "TakeLast":
                        return typeof(Queryable).GetMethod(method.Name).MakeGenericMethod(method.GetGenericArguments());
                    #endregion
                    #region ReadMethod
                    case "AllAsync":
                    case "AnyAsync":
                    case "CountAsync":
                    case "LongCountAsync":
                    case "FirstAsync":
                    case "FirstOrDefaultAsync":
                    case "SingleAsync":
                    case "SingleOrDefaultAsync":
                        return typeof(System.Data.Entity.QueryableExtensions).GetMember(method.Name).Cast<MethodInfo>().Where(t => t.GetParameters().Length == method.GetParameters().Length && t.GetParameters()[t.GetParameters().Length - 1].ParameterType == typeof(CancellationToken)).First().MakeGenericMethod(method.GetGenericArguments()[0]);
                    case "AverageAsync":
                    case "SumAsync":
                        return (method.GetParameters().Length == 2 ?
                            typeof(System.Data.Entity.QueryableExtensions).GetMember(method.Name).Cast<MethodInfo>().Where(t => !t.IsGenericMethodDefinition && t.GetParameters().Length == 2 && t.GetParameters()[0].ParameterType.GetGenericArguments()[0] == method.GetParameters()[0].ParameterType.GetGenericArguments()[0]).First()
                            : typeof(System.Data.Entity.QueryableExtensions).GetMember(method.Name).Cast<MethodInfo>().Where(t => t.IsGenericMethodDefinition && t.GetParameters().Length == 3 && t.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericArguments()[1] == method.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericArguments()[1]).First().MakeGenericMethod(method.GetGenericArguments()));
                    case "MaxAsync":
                    case "MinAsync":
                        return (method.GetParameters().Length == 2 ?
                            typeof(System.Data.Entity.QueryableExtensions).GetMember(method.Name).Cast<MethodInfo>().Where(t => t.GetParameters().Length == 2).First().MakeGenericMethod(method.ReturnType.GetGenericArguments()[0])
                            : typeof(System.Data.Entity.QueryableExtensions).GetMember(method.Name).Cast<MethodInfo>().Where(t => t.GetParameters().Length == 3).First().MakeGenericMethod(method.GetGenericArguments()[0], method.ReturnType.GetGenericArguments()[0]));
                    case "ToArrayAsync":
                    case "ToListAsync":
                        return typeof(System.Data.Entity.QueryableExtensions).GetMember(method.Name).Cast<MethodInfo>().Where(t => t.IsGenericMethod && t.GetParameters().Length == method.GetParameters().Length).First().MakeGenericMethod(method.GetGenericArguments());
                    case "ToDictionaryAsync":
                        return typeof(System.Data.Entity.QueryableExtensions).GetMember(method.Name).Cast<MethodInfo>().Where(t => t.GetGenericArguments().Length == method.GetGenericArguments().Length && t.GetParameters().Length == method.GetParameters().Length).First().MakeGenericMethod(method.GetGenericArguments());
                    #endregion
                    default:
                        return null;
                }
            });
        }
    }
}
