﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Reflection;
using Wodsoft.ComBoost.Data.Entity.Metadata;

namespace Wodsoft.ComBoost.Data.Entity
{

    public class ExpressionWrapper : ExpressionVisitor
    {
        private static readonly MethodInfo _WrapMethod = typeof(QueryableExtensions).GetMethod("Wrap", new Type[] { typeof(object) });
        private Type _T, _M;

        public ExpressionWrapper(Type target, Type mapped)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (mapped == null)
                throw new ArgumentNullException(nameof(mapped));
            _T = target;
            _M = mapped;
            _Parameter = Expression.Parameter(mapped);
        }

        private ParameterExpression _Parameter;

        protected override Expression VisitLambda<F>(Expression<F> node)
        {
            var lambdaType = typeof(F);
            if (lambdaType.IsConstructedGenericType && lambdaType.GetGenericTypeDefinition() == typeof(Func<,>) && lambdaType.GenericTypeArguments[0] == _T)
            {
                var definition = lambdaType.GetGenericArguments();
                for (int i = 0; i < definition.Length; i++)
                {
                    if (typeof(IEntity).IsAssignableFrom(definition[i]))
                        if (definition[i] == _T)
                            definition[i] = _M;
                        else
                            definition[i] = EntityDescriptor.GetMetadata(definition[i]).Type;
                }
                lambdaType = lambdaType.GetGenericTypeDefinition().MakeGenericType(definition);
                return Expression.Lambda(lambdaType, Visit(node.Body), _Parameter);
            }
            return base.VisitLambda<F>(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(Queryable))
            {
                var method = node.Method.GetGenericMethodDefinition().MakeGenericMethod(node.Method.GetGenericArguments().Select(t =>
                t == _T ? _M : t).ToArray());
                return Expression.Call(method, node.Arguments.Select(t => Visit(t)));
            }
            else if (node.Method == _WrapMethod)
            {
                Expression expression = node.Arguments[0];
                if (expression.NodeType == ExpressionType.Convert)
                    expression = ((UnaryExpression)expression).Operand;
                return Visit(expression);
            }
            else if (node.Method.IsGenericMethod)
            {
                var args = node.Method.GetGenericArguments();
                for (int i = 0; i < args.Length; i++)
                {
                    if (typeof(IEntity).IsAssignableFrom(args[i]))
                        if (args[i] == _T)
                            args[i] = _M;
                        else
                            args[i] = EntityDescriptor.GetMetadata(args[i]).Type;
                }
                var method = node.Method.GetGenericMethodDefinition().MakeGenericMethod(args);
                if (node.Object == null)
                    return Expression.Call(method, node.Arguments.Select(t => Visit(t)));
                else
                    return Expression.Call(Visit(node.Object), method, node.Arguments.Select(t => Visit(t)));
            }
            return base.VisitMethodCall(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            //静态成员直接返回
            if (node.Expression == null)
                return base.VisitMember(node);
            if (typeof(IEntity).IsAssignableFrom(node.Expression.Type) && node.Expression.Type.GetTypeInfo().IsInterface)
            {
                Type type;
                if (node.Expression.Type == _T)
                    type = _M;
                else
                    type = EntityDescriptor.GetMetadata(node.Expression.Type).Type;
                return Expression.MakeMemberAccess(Visit(node.Expression), type.GetMember(node.Member.Name, BindingFlags.Instance | BindingFlags.Public)[0]);
            }
            else if (node.Expression is ConstantExpression && node.Expression.Type.Name.Contains("<>"))
            {
                var value = Expression.Lambda<Func<object>>(Expression.Convert(node, typeof(object))).Compile()();
                return Expression.Constant(value);
            }
            else if (node.Expression is MemberExpression && ((MemberExpression)node.Expression).Member.DeclaringType.Name.Contains("<>"))
            {
                var value = Expression.Lambda<Func<object>>(Expression.Convert(node, typeof(object))).Compile()();
                return Expression.Constant(value);
            }
            return base.VisitMember(node);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (node.Type == _T)
                return _Parameter;
            return base.VisitParameter(node);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value != null && node.Type != node.Value.GetType())
                return Expression.Constant(node.Value);
            return base.VisitConstant(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node.NodeType == ExpressionType.Equal)
            {
                Expression left = Visit(node.Left);
                Expression right = Visit(node.Right);
                return Expression.Equal(left, right);
            }
            else if (node.NodeType == ExpressionType.NotEqual)
            {
                Expression left = Visit(node.Left);
                Expression right = Visit(node.Right);
                return Expression.NotEqual(left, right);
            }
            return base.VisitBinary(node);
        }
    }

    public class ExpressionWrapper<T, M> : ExpressionWrapper
    {
        public ExpressionWrapper() : base(typeof(T), typeof(M))
        { }
    }
}
