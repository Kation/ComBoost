using System;
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
                definition[0] = _M;
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
            else if (node.Method.DeclaringType == typeof(QueryableExtensions) && node.Method.Name == "Wrap")
            {
                Expression expression = node.Arguments[0];
                if (expression.NodeType == ExpressionType.Convert)
                    expression = ((UnaryExpression)expression).Operand;
                return expression;
            }
            return base.VisitMethodCall(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (typeof(IEntity).IsAssignableFrom(node.Expression.Type) && node.Expression.Type.GetTypeInfo().IsInterface)
            {
                var type = EntityDescriptor.GetMetadata(node.Expression.Type).Type;
                return Expression.MakeMemberAccess(Visit(node.Expression), type.GetMember(node.Member.Name, BindingFlags.Instance | BindingFlags.Public)[0]);
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
