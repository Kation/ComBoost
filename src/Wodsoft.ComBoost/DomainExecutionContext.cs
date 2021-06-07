using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public class DomainExecutionContext : IDomainExecutionContext
    {
        public DomainExecutionContext(IDomainService domainService, IDomainContext domainContext, MethodInfo method)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            if (domainContext == null)
                throw new ArgumentNullException(nameof(domainContext));
            if (method == null)
                throw new ArgumentNullException(nameof(method));
            _DomainService = domainService;
            _Context = domainContext;
            _Method = method;
            _Parameters = _Method.GetParameters();
            _ParameterValues = GetParameters();
        }

        public DomainExecutionContext(IDomainService domainService, IDomainContext domainContext, MethodInfo method, object[] parameters)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            if (domainContext == null)
                throw new ArgumentNullException(nameof(domainContext));
            if (method == null)
                throw new ArgumentNullException(nameof(method));
            _DomainService = domainService;
            _Context = domainContext;
            _Method = method;
            _ParameterValues = parameters;
        }

        private IDomainService _DomainService;
        public IDomainService DomainService
        {
            get
            {
                return _DomainService;
            }
        }

        private IDomainContext _Context;
        public IDomainContext DomainContext { get { return _Context; } }

        private MethodInfo _Method;
        public MethodInfo DomainMethod { get { return _Method; } }

        private object[] _ParameterValues;
        public object[] ParameterValues { get { return _ParameterValues; } }

        private ParameterInfo[] _Parameters;
        public object GetParameterValue(ParameterInfo parameter)
        {
            int index = Array.IndexOf(_Parameters, parameter);
            if (index == -1)
                throw new ArgumentException("该参数不属于当前执行上下文。", nameof(parameter));
            return _ParameterValues[index];
        }

        private object[] GetParameters()
        {
            return _Parameters.Select(t =>
            {
                var from = t.GetCustomAttribute<FromAttribute>();
                if (from == null)
                    throw new NotSupportedException(string.Format("不能解析的参数，{0}的{1}。", _Method.DeclaringType.FullName, t.Name));
                var value = from.GetValue(DomainContext, t);
                if (value == null && t.HasDefaultValue)
                    value = t.DefaultValue;
                return value;
            }).ToArray();
        }

        public void Done()
        {
            IsCompleted = true;
        }

        public void Done(object result)
        {
            Result = result;
            IsCompleted = true;
        }

        public object Result { get; set; }

        public bool IsAborted { get { return DomainContext.ServiceAborted.IsCancellationRequested; } }

        public bool IsCompleted { get; private set; }
    }
}
