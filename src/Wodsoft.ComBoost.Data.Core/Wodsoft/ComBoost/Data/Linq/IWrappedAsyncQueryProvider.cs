using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Data.Linq
{
    public interface IWrappedAsyncQueryProvider : IQueryProvider
    {
        Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken token);
    }
}
