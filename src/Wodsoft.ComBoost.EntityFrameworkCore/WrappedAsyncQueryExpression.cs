using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Wodsoft.ComBoost.Data.Entity
{
    public class WrappedAsyncQueryExpression : Expression
    {
        public WrappedAsyncQueryExpression(Type elementType)
        {
            Type = typeof(IQueryable<>).MakeGenericType(elementType);
        }

        public override Type Type { get; }

        public override ExpressionType NodeType => ExpressionType.Extension;

        public override bool CanReduce => false;
    }
}
