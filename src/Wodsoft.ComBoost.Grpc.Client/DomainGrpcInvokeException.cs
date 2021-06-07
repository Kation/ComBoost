using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Grpc.Client
{
    public class DomainGrpcInvokeException : Exception
    {
        public DomainGrpcInvokeException(DomainGrpcException exception) : base(exception.Message, exception.InnerException == null ? null : new DomainGrpcInvokeException(exception.InnerException))
        {
            Source = exception.Source;
            Title = exception.Title;
            StackTrace = exception.StackTrace;
        }

        public string Title { get; }

        public new string StackTrace { get; }
    }
}
