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
        private WrappedAsyncQueryProvider _provider;

        public WrappedAsyncExpressionVisitor(WrappedAsyncQueryProvider provider)
        {
            _provider = provider;
            _root = provider.SourceExpression;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(Wodsoft.ComBoost.Data.Linq.QueryableExtensions))
            {
                switch (node.Method.Name)
                {
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
                                Expression.Call(method, Visit(node.Arguments[0]), node.Arguments[1], node.Arguments[2])
                                : Expression.Call(method, Visit(node.Arguments[0]), node.Arguments[1], node.Arguments[2], node.Arguments[3]);
                        }
                        else
                        {
                            var method = MapMethod(node.Method);
                            return node.Method.GetParameters().Length == 4 ?
                                Expression.Call(method, Visit(node.Arguments[0]), node.Arguments[1], node.Arguments[2], node.Arguments[3])
                                : Expression.Call(method, Visit(node.Arguments[0]), node.Arguments[1], node.Arguments[2], node.Arguments[3], node.Arguments[4]);
                        }
                    #endregion
                    #region Include
                    case "Include":
                        {
                            string path;
                            if (node.Method.GetGenericArguments().Length == 2)
                            {
                                if (!TryParsePath(((LambdaExpression)((UnaryExpression)node.Arguments[1]).Operand).Body, out path))
                                    throw new ArgumentException("Invalid navigation path.");
                            }
                            else
                            {
                                path = (string)((ConstantExpression)node.Arguments[1]).Value;
                            }
                            var queryable = _provider.SourceProvider.CreateQuery(Visit(node.Arguments[0]));
                            queryable = (IQueryable)queryable.GetType().GetMethod("Include").Invoke(queryable, new object[] { path });
                            return queryable.Expression;
                        }
                    #endregion
                    default:
                        throw new NotSupportedException($"Can not support method \"{node.Method.DeclaringType}.{node.Method.Name}\".");
                }
            }
            return base.VisitMethodCall(node);
        }

        private static ConcurrentDictionary<MethodInfo, MethodInfo> _Mapping = new ConcurrentDictionary<MethodInfo, MethodInfo>();
        private static MethodInfo MapMethod(MethodInfo value)
        {
            return _Mapping.GetOrAdd(value, method =>
            {
                if (method.DeclaringType == typeof(Wodsoft.ComBoost.Data.Linq.QueryableExtensions))
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
                            return typeof(Queryable).GetTypeInfo().GetDeclaredMethods(method.Name).First(t => t.IsGenericMethod && t.GetParameters()[1].ParameterType == method.GetParameters()[1].ParameterType).MakeGenericMethod(method.GetGenericArguments());
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
                }
                else
                    return null;
            });
        }

        private bool TryParsePath(Expression expression, out string path)
        {
            path = null;
            var withoutConvert = RemoveConvert(expression); // Removes boxing
            var memberExpression = withoutConvert as MemberExpression;
            var callExpression = withoutConvert as MethodCallExpression;

            if (memberExpression != null)
            {
                var thisPart = memberExpression.Member.Name;
                string parentPart;
                if (!TryParsePath(memberExpression.Expression, out parentPart))
                {
                    return false;
                }
                path = parentPart == null ? thisPart : (parentPart + "." + thisPart);
            }
            else if (callExpression != null)
            {
                if (callExpression.Method.Name == "Select"
                    && callExpression.Arguments.Count == 2)
                {
                    string parentPart;
                    if (!TryParsePath(callExpression.Arguments[0], out parentPart))
                    {
                        return false;
                    }
                    if (parentPart != null)
                    {
                        var subExpression = callExpression.Arguments[1] as LambdaExpression;
                        if (subExpression != null)
                        {
                            string thisPart;
                            if (!TryParsePath(subExpression.Body, out thisPart))
                            {
                                return false;
                            }
                            if (thisPart != null)
                            {
                                path = parentPart + "." + thisPart;
                                return true;
                            }
                        }
                    }
                }
                return false;
            }

            return true;
        }

        private Expression RemoveConvert(Expression expression)
        {
            while (expression.NodeType == ExpressionType.Convert
                   || expression.NodeType == ExpressionType.ConvertChecked)
            {
                expression = ((UnaryExpression)expression).Operand;
            }

            return expression;
        }
    }
}
