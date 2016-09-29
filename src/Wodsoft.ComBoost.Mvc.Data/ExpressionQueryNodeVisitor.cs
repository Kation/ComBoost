using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Reflection;

namespace Wodsoft.ComBoost.Mvc
{
    public class ExpressionQueryNodeVisitor : QueryNodeVisitor<Expression>
    {
        public override Expression Visit(AllNode nodeIn)
        {
            var source = nodeIn.Source.Accept(this);
            var parameter = nodeIn.Body.Accept(this);
            var type = Type.GetType(nodeIn.Source.ItemType.FullName());
            var method = typeof(Queryable).GetTypeInfo().GetMethod("All");
            return Expression.Call(method, source, parameter);
        }

        public override Expression Visit(AnyNode nodeIn)
        {
            var source = nodeIn.Source.Accept(this);
            var parameter = nodeIn.Body.Accept(this);
            var type = Type.GetType(nodeIn.Source.ItemType.FullName());
            var method = typeof(Queryable).GetTypeInfo().GetMethod("Any", new Type[] { typeof(IQueryable<>).MakeGenericType(type), typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(type, typeof(bool))) });
            return Expression.Call(method, source, parameter);
        }

        public override Expression Visit(BinaryOperatorNode nodeIn)
        {
            var left = nodeIn.Left.Accept(this);
            var right = nodeIn.Right.Accept(this);
            switch (nodeIn.OperatorKind)
            {
                case BinaryOperatorKind.Add:
                    return Expression.Add(left, right);
                case BinaryOperatorKind.And:
                    return Expression.And(left, right);
                case BinaryOperatorKind.Divide:
                    return Expression.Divide(left, right);
                case BinaryOperatorKind.Equal:
                    return Expression.Equal(left, right);
                case BinaryOperatorKind.GreaterThan:
                    return Expression.GreaterThan(left, right);
                case BinaryOperatorKind.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(left, right);
                case BinaryOperatorKind.Has:
                    return Expression.AndAssign(left, right);
                case BinaryOperatorKind.LessThan:
                    return Expression.LessThan(left, right);
                case BinaryOperatorKind.LessThanOrEqual:
                    return Expression.LessThanOrEqual(left, right);
                case BinaryOperatorKind.Modulo:
                    return Expression.Modulo(left, right);
                case BinaryOperatorKind.Multiply:
                    return Expression.Multiply(left, right);
                case BinaryOperatorKind.NotEqual:
                    return Expression.NotEqual(left, right);
                case BinaryOperatorKind.Or:
                    return Expression.Or(left, right);
                case BinaryOperatorKind.Subtract:
                    return Expression.Subtract(left, right);
                default:
                    throw new NotSupportedException();
            }
        }

        public override Expression Visit(CollectionComplexNode nodeIn)
        {
            Expression source = nodeIn.Source.Accept(this);
            return Expression.Property(source, nodeIn.Property.Name);
        }

        public override Expression Visit(CollectionFunctionCallNode nodeIn)
        {
            //Expression source = nodeIn.Source.Accept(this);
            //var parameters = nodeIn.Parameters.Select(t => t.Accept(this)).ToArray();
            //nodeIn.Functions.Select(t => t.FullName());
            throw new NotSupportedException();
        }

        public override Expression Visit(CollectionNavigationNode nodeIn)
        {
            Expression source = nodeIn.Source.Accept(this);
            return Expression.Property(source, nodeIn.NavigationProperty.Name);
        }

        public override Expression Visit(CollectionOpenPropertyAccessNode nodeIn)
        {
            Expression source = nodeIn.Source.Accept(this);
            return Expression.Property(source, nodeIn.Name);
        }

        public override Expression Visit(CollectionPropertyAccessNode nodeIn)
        {
            return base.Visit(nodeIn);
        }

        public override Expression Visit(CollectionResourceCastNode nodeIn)
        {
            throw new NotSupportedException();
        }

        public override Expression Visit(CollectionResourceFunctionCallNode nodeIn)
        {
            throw new NotSupportedException();
        }

        public override Expression Visit(ConstantNode nodeIn)
        {
            return Expression.Constant(nodeIn.Value);
        }

        public override Expression Visit(ConvertNode nodeIn)
        {
            var type = Type.GetType(nodeIn.TypeReference.FullName());
            Expression source = nodeIn.Source.Accept(this);
            return Expression.Convert(source, type);
        }

        public override Expression Visit(CountNode nodeIn)
        {
            Expression source = nodeIn.Source.Accept(this);
            var type = Type.GetType(nodeIn.TypeReference.AsCollection().CollectionDefinition().ElementType.FullName());
            var method = typeof(Queryable).GetTypeInfo().GetMethod("Count", new Type[] { typeof(IQueryable<>).MakeGenericType(type) });
            return Expression.Call(method, source);
        }

        public override Expression Visit(NamedFunctionParameterNode nodeIn)
        {
            Expression source = nodeIn.Value.Accept(this);
            return source;
        }

        public override Expression Visit(NonResourceRangeVariableReferenceNode nodeIn)
        {
            throw new NotSupportedException();
        }

        public override Expression Visit(ParameterAliasNode nodeIn)
        {
            var type = Type.GetType(nodeIn.TypeReference.FullName());
            return Expression.Parameter(type, nodeIn.Alias);
        }

        public override Expression Visit(ResourceRangeVariableReferenceNode nodeIn)
        {
            throw new NotSupportedException();
        }

        public override Expression Visit(SearchTermNode nodeIn)
        {
            throw new NotSupportedException();
        }

        public override Expression Visit(SingleComplexNode nodeIn)
        {
            throw new NotSupportedException();
        }

        public override Expression Visit(SingleNavigationNode nodeIn)
        {
            Expression source = nodeIn.Source.Accept(this);
            return Expression.Property(source, nodeIn.NavigationProperty.Name);
        }

        public override Expression Visit(SingleResourceCastNode nodeIn)
        {
            throw new NotSupportedException();
        }

        public override Expression Visit(SingleResourceFunctionCallNode nodeIn)
        {
            throw new NotSupportedException();
        }

        public override Expression Visit(SingleValueFunctionCallNode nodeIn)
        {
            return base.Visit(nodeIn);
        }

        public override Expression Visit(SingleValueOpenPropertyAccessNode nodeIn)
        {
            Expression source = nodeIn.Source.Accept(this);
            return Expression.Property(source, nodeIn.Name);
        }

        public override Expression Visit(SingleValuePropertyAccessNode nodeIn)
        {
            Expression source = nodeIn.Source.Accept(this);
            return Expression.Property(source, nodeIn.Property.Name);
        }

        public override Expression Visit(UnaryOperatorNode nodeIn)
        {
            Expression source = nodeIn.Operand.Accept(this);
            switch (nodeIn.OperatorKind)
            {
                case UnaryOperatorKind.Negate:
                    return Expression.Negate(source);
                case UnaryOperatorKind.Not:
                    return Expression.Not(source);
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
