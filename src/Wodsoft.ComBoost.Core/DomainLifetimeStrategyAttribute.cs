using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class DomainLifetimeStrategyAttribute : Attribute
    {
        public DomainLifetimeStrategyAttribute(DomainLifetimeStrategy strategy)
        {
            Strategy = strategy;
        }

        public DomainLifetimeStrategy Strategy { get; }
    }

    public enum DomainLifetimeStrategy
    {
        Scope = 0,
        Transient = 1
    }
}
