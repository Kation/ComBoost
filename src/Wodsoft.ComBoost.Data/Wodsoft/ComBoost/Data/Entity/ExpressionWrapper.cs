using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Reflection;

namespace Wodsoft.ComBoost.Data.Entity
{
    public class ExpressionWrapper<T, M> : ExpressionVisitor
    {
        public ExpressionWrapper()
        {
            _Parameter = Expression.Parameter(typeof(M));
        }

        private ParameterExpression _Parameter;

        protected override Expression VisitLambda<F>(Expression<F> node)
        {
            var lambdaType = typeof(F);
            if (lambdaType.IsConstructedGenericType && lambdaType.GetGenericTypeDefinition() == typeof(Func<,>) && lambdaType.GenericTypeArguments[0] == typeof(T) && lambdaType.GenericTypeArguments[1] == typeof(bool))
            {
                Expression<Func<T, bool>> expression = (Expression<Func<T, bool>>)(object)node;
                return Expression.Lambda<Func<M, bool>>(Visit(expression.Body), _Parameter);
            }
            return base.VisitLambda<F>(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(Queryable))
            {
                var method = node.Method.GetGenericMethodDefinition().MakeGenericMethod(node.Method.GetGenericArguments().Select(t =>
                t == typeof(T) ? typeof(M) : t).ToArray());
                return Expression.Call(method, node.Arguments.Select(t => Visit(t)));
            }
            return base.VisitMethodCall(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression.Type == typeof(T))
                return Expression.MakeMemberAccess(Visit(node.Expression), typeof(M).GetMember(node.Member.Name, BindingFlags.Instance | BindingFlags.Public)[0]);
            return base.VisitMember(node);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (node.Type == typeof(T))
                return _Parameter;
            return base.VisitParameter(node);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value != null && node.Type != node.Value)
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
}
