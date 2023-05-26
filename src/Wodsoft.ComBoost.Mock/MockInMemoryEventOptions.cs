using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Wodsoft.ComBoost.Mock
{
    public class MockInMemoryEventOptions
    {
        public object InstanceKey { get; set; } = "Default";

        private string? _groupName;
        public string GroupName { get => _groupName ?? Assembly.GetEntryAssembly().FullName; set => _groupName = value; }

        public bool ThrowExceptionForMustHandleEventWhenNull { get; set; } = false;

        public bool IsAsyncEvent { get; set; } = false;
    }
}
