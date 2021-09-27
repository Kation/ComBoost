﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Test
{
    public class GreeterService : DomainService
    {
        public Task<HelloResponse> SayHi([FromValue] HelloRequest request)
        {
            return Task.FromResult(new HelloResponse { Answer = $"Hi {request.Name}." });
        }

        public Task<string> Hello([FromValue(false)] string text)
        {
            Debug.WriteLine("Receive hello: " + text);
            return Task.FromResult("Hi.");
        }
    }
}