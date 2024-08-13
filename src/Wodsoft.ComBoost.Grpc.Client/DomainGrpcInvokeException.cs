using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Grpc.Client
{
    public class DomainGrpcInvokeException : Exception, IDomainRpcException
    {
        public DomainGrpcInvokeException(DomainGrpcException exception) : base(exception.Message, exception.InnerException == null ? null : new DomainGrpcInvokeException(exception.InnerException))
        {
            Source = exception.Source;
            Title = exception.Title;
            RemoteStackTrace = exception.StackTrace;
        }

        public string Title { get; }

        public string RemoteStackTrace { get; }

        string IDomainRpcException.StackTrace => RemoteStackTrace;

        IDomainRpcException? IDomainRpcException.InnerException => (DomainGrpcInvokeException?)InnerException;
    }
}
