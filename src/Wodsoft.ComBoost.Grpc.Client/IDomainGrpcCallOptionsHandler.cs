using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Grpc.Client
{
    public interface IDomainGrpcCallOptionsHandler
    {
        void Handle(Type service, ref CallOptions callOptions);
    }
}
