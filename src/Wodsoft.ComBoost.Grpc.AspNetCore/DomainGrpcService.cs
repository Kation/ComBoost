using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Wodsoft.ComBoost.Grpc.AspNetCore
{
    public class DomainGrpcService
    {
        private static readonly AssemblyBuilder _AssemblyBuilder;
        private static readonly ModuleBuilder _ModuleBuilder;

        static DomainGrpcService()
        {
            _AssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Wodsoft.ComBoost.Grpc.AspNetCore.Dynamic"), AssemblyBuilderAccess.Run);
            _ModuleBuilder = _AssemblyBuilder.DefineDynamicModule("Default");
        }

        public static Assembly GetAssembly()
        {
            return _AssemblyBuilder;
        }

        public static TypeBuilder CreateType(string name, Type parentType)
        {
            return _ModuleBuilder.DefineType("Wodsoft.ComBoost.Grpc.Services." + name, TypeAttributes.Public | TypeAttributes.Class, parentType);
        }
    }
}
