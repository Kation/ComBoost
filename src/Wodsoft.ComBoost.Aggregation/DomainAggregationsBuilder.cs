using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Wodsoft.ComBoost.Aggregation.Test")]
namespace Wodsoft.ComBoost.Aggregation
{
    public class DomainAggregationsBuilder
    {
        internal static ModuleBuilder Module;

        internal static List<IDomainAggregationsBuilderExtension> Extensions = new List<IDomainAggregationsBuilderExtension>();

        static DomainAggregationsBuilder()
        {
            AssemblyBuilder assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Wodsoft.ComBoost.Aggregation.Dynamic"), AssemblyBuilderAccess.Run);
            Module = assembly.DefineDynamicModule("default");
        }

        public static void AddExtension<TExtension>()
            where TExtension : IDomainAggregationsBuilderExtension, new()
        {
            if (Extensions.Any(t => t is TExtension))
                return;
            Extensions.Add(new TExtension());
        }
    }

    public class DomainAggregationsBuilder<T> : DomainAggregationsBuilder
    {
        static DomainAggregationsBuilder()
        {
            var type = typeof(T);
            if (type.IsValueType)
            {
                HasAggregation = false;
                return;
            }
            var aggregationProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(t => t.CanRead && t.GetGetMethod().IsVirtual && t.GetCustomAttribute<AggregateAttribute>() != null).ToList();
            if (aggregationProperties.Count > 0)
            {
                HasAggregation = true;
            }
            var subProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(t => t.PropertyType != typeof(string) && t.PropertyType != typeof(object) && t.CanRead && t.GetGetMethod().IsVirtual && t.GetCustomAttribute<IgnoreAggregateAttribute>() == null).ToList();
            if (subProperties.Count > 0)
            {
                List<PropertyInfo> properties = new List<PropertyInfo>();
                foreach (var property in subProperties)
                {
                    var has = (bool)typeof(DomainAggregationsBuilder<>).MakeGenericType(property.PropertyType).GetProperty(nameof(HasAggregation)).GetValue(null);
                    if (has)
                        properties.Add(property);
                }
                subProperties = properties;
                if (properties.Count > 0)
                    HasAggregation = true;
            }
            if (HasAggregation)
                BuildType(aggregationProperties, subProperties);
        }

        private static readonly MethodInfo _UnwrapMethodInfo = typeof(TaskExtensions).GetMethods().First(t => t.Name == nameof(TaskExtensions.Unwrap) && t.IsGenericMethodDefinition);

        private static void BuildType(List<PropertyInfo> aggregationProperties, List<PropertyInfo> subProperties)
        {
            var type = typeof(T);
            var typeBuilder = Module.DefineType("Wodsoft.ComBoost.Aggregation.Dynamic." + type.Name + "_" + type.GetHashCode(), TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed, type);
            typeBuilder.AddInterfaceImplementation(typeof(IDomainAggregation));
            var aggregateAsyncMethod = typeBuilder.DefineMethod(nameof(IDomainAggregation.AggregateAsync), MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final, typeof(Task), new Type[] { typeof(IDomainAggregator) });
            //typeBuilder.DefineMethodOverride(aggregateAsyncMethod, typeof(IDomainAggregation).GetMethod(nameof(IDomainAggregation.AggregateAsync)));
            var aggregateAsyncILGenerator = aggregateAsyncMethod.GetILGenerator();
            aggregateAsyncILGenerator.Emit(OpCodes.Ldstr, $"Begain aggregate {type.Name}.");
            aggregateAsyncILGenerator.Emit(OpCodes.Call, typeof(Debug).GetMethod("WriteLine", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null));

            var constructor = typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard, new Type[] { type });
            constructor.DefineParameter(1, ParameterAttributes.None, "value");
            var constructorILGenerator = constructor.GetILGenerator();
            foreach (var property in type.GetProperties().Where(t => t.CanRead && t.CanWrite && t.GetIndexParameters().Length == 0))
            {
                constructorILGenerator.Emit(OpCodes.Ldarg_0);
                constructorILGenerator.Emit(OpCodes.Ldarg_1);
                constructorILGenerator.Emit(property.GetMethod.IsFinal ? OpCodes.Call : OpCodes.Callvirt, property.GetMethod);
                constructorILGenerator.Emit(property.SetMethod.IsFinal ? OpCodes.Call : OpCodes.Callvirt, property.SetMethod);
            }
            constructorILGenerator.Emit(OpCodes.Ret);

            var tasksVariable = aggregateAsyncILGenerator.DeclareLocal(typeof(List<Task>));
            aggregateAsyncILGenerator.Emit(OpCodes.Newobj, typeof(List<Task>).GetConstructor(Array.Empty<Type>()));
            aggregateAsyncILGenerator.Emit(OpCodes.Stloc, tasksVariable);

            foreach (var property in aggregationProperties)
            {
                var aggregateAttribute = property.GetCustomAttribute<AggregateAttribute>();
                if (aggregateAttribute.IsSelfIgnored)
                {
                    var overrideProperty = typeBuilder.DefineProperty(property.Name, property.Attributes, property.PropertyType, null);

                    var getMethod = property.GetGetMethod();
                    var overrideGetMethod = typeBuilder.DefineMethod(getMethod.Name, MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.SpecialName);
                    overrideGetMethod.SetReturnType(getMethod.ReturnType);
                    var overrideGetILGenerator = overrideGetMethod.GetILGenerator();
                    overrideGetILGenerator.Emit(OpCodes.Ldarg_0);
                    overrideGetILGenerator.Emit(OpCodes.Call, getMethod);
                    overrideGetILGenerator.Emit(OpCodes.Ret);

                    var setMethod = property.GetSetMethod();
                    var overrideSetMethod = typeBuilder.DefineMethod(setMethod.Name, MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.SpecialName);
                    overrideSetMethod.SetParameters(property.PropertyType);
                    var overrideSetILGenerator = overrideSetMethod.GetILGenerator();
                    overrideSetILGenerator.Emit(OpCodes.Ldarg_0);
                    overrideSetILGenerator.Emit(OpCodes.Ldarg_1);
                    overrideSetILGenerator.Emit(OpCodes.Call, setMethod);
                    overrideSetILGenerator.Emit(OpCodes.Ret);

                    //overrideMethod.SetCustomAttribute
                    foreach (var extension in Extensions)
                    {
                        foreach (var att in extension.CreateIgnoredPropertyAttribute())
                        {
                            overrideProperty.SetCustomAttribute(att);
                        }
                    }
                    //typeBuilder.DefineMethodOverride(overrideMethod, getMethod);
                    overrideProperty.SetGetMethod(overrideGetMethod);
                    overrideProperty.SetSetMethod(overrideSetMethod);
                }

                Type underlyingType = Nullable.GetUnderlyingType(property.PropertyType);
                var keyVariable = aggregateAsyncILGenerator.DeclareLocal(property.PropertyType);
                var endLabel = aggregateAsyncILGenerator.DefineLabel();
                //Read property
                aggregateAsyncILGenerator.Emit(OpCodes.Ldarg_0);
                aggregateAsyncILGenerator.Emit(OpCodes.Callvirt, property.GetGetMethod());
                aggregateAsyncILGenerator.Emit(OpCodes.Stloc, keyVariable);
                if (property.PropertyType.IsClass || underlyingType != null)
                {
                    //Check if key property has value
                    if (property.PropertyType.IsValueType)
                    {
                        aggregateAsyncILGenerator.Emit(OpCodes.Ldloca, keyVariable);
                        aggregateAsyncILGenerator.Emit(OpCodes.Call, property.PropertyType.GetProperty("HasValue").GetMethod);
                    }
                    else
                        aggregateAsyncILGenerator.Emit(OpCodes.Ldloc, keyVariable);
                    aggregateAsyncILGenerator.Emit(OpCodes.Ldstr, "No need to aggregate.");
                    aggregateAsyncILGenerator.Emit(OpCodes.Call, typeof(Debug).GetMethod("WriteLine", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null));
                    aggregateAsyncILGenerator.Emit(OpCodes.Brfalse, endLabel);
                }
                //if has value
                //tasks.Add(...);
                aggregateAsyncILGenerator.Emit(OpCodes.Ldloc, tasksVariable);
                var getAggregationMethod = typeof(IDomainAggregator).GetMethod(nameof(IDomainAggregator.GetAggregationAsync)).MakeGenericMethod(aggregateAttribute.AggregationType, underlyingType ?? property.PropertyType);
                //aggregator.GetAggregation<T,TKey>(keyVariable)
                aggregateAsyncILGenerator.Emit(OpCodes.Ldarg_1);
                if (underlyingType != null)
                {
                    aggregateAsyncILGenerator.Emit(OpCodes.Ldloca, keyVariable);
                    aggregateAsyncILGenerator.Emit(OpCodes.Call, property.PropertyType.GetProperty("Value").GetMethod);
                }
                else
                    aggregateAsyncILGenerator.Emit(OpCodes.Ldloc, keyVariable);
                aggregateAsyncILGenerator.Emit(OpCodes.Callvirt, getAggregationMethod);

                //Define ContinueWith method
                var continueMethod = typeBuilder.DefineMethod("continue_" + property.Name, MethodAttributes.Private | MethodAttributes.Final | MethodAttributes.NewSlot, null, new Type[] { getAggregationMethod.ReturnType });
                var continueILGenerator = continueMethod.GetILGenerator();
                var continueNotFaultLabel = continueILGenerator.DefineLabel();
                continueILGenerator.Emit(OpCodes.Ldarg_1);
                continueILGenerator.Emit(OpCodes.Call, typeof(Task).GetProperty(nameof(Task.IsFaulted)).GetMethod);
                continueILGenerator.Emit(OpCodes.Brfalse, continueNotFaultLabel);
                continueILGenerator.Emit(OpCodes.Ldarg_1);
                continueILGenerator.Emit(OpCodes.Call, typeof(Task).GetProperty(nameof(Task.Exception)).GetMethod);
                continueILGenerator.Emit(OpCodes.Call, typeof(System.Runtime.ExceptionServices.ExceptionDispatchInfo).GetMethod("Capture", BindingFlags.Public | BindingFlags.Static));
                continueILGenerator.Emit(OpCodes.Call, typeof(System.Runtime.ExceptionServices.ExceptionDispatchInfo).GetMethod("Throw", BindingFlags.Public | BindingFlags.Instance));
                continueILGenerator.MarkLabel(continueNotFaultLabel);


                var ignoreAttribute = property.GetCustomAttribute<IgnoreAggregateAttribute>();
                Type nestType;
                if (aggregateAttribute.AggregationType == type)
                    nestType = typeBuilder;
                else
                    nestType = (Type)typeof(DomainAggregationsBuilder<>).MakeGenericType(aggregateAttribute.AggregationType).GetProperty(nameof(AggregationType), BindingFlags.Public | BindingFlags.Static).GetValue(null);
                if (ignoreAttribute == null && nestType != null)
                {
                    //var returnMethod = typeBuilder.DefineMethod("return_" + property.Name, MethodAttributes.Private | MethodAttributes.Final | MethodAttributes.NewSlot, aggregateAttribute.AggregationType, new Type[] { typeof(Task), typeof(object) });
                    //var returnILGenerator = returnMethod.GetILGenerator();
                    //var returnNotFaultLabel = returnILGenerator.DefineLabel();
                    //returnILGenerator.Emit(OpCodes.Ldarg_1);
                    //returnILGenerator.Emit(OpCodes.Call, typeof(Task).GetProperty(nameof(Task.IsFaulted)).GetMethod);
                    //returnILGenerator.Emit(OpCodes.Brfalse, returnNotFaultLabel);
                    //returnILGenerator.Emit(OpCodes.Ldarg_1);
                    //returnILGenerator.Emit(OpCodes.Call, typeof(Task).GetProperty(nameof(Task.Exception)).GetMethod);
                    //returnILGenerator.Emit(OpCodes.Call, typeof(System.Runtime.ExceptionServices.ExceptionDispatchInfo).GetMethod("Capture", BindingFlags.Public | BindingFlags.Static));
                    //returnILGenerator.Emit(OpCodes.Call, typeof(System.Runtime.ExceptionServices.ExceptionDispatchInfo).GetMethod("Throw", BindingFlags.Public | BindingFlags.Instance));
                    //returnILGenerator.MarkLabel(returnNotFaultLabel);
                    //returnILGenerator.Emit(OpCodes.Ldstr, "Nest aggregate completed. Return previous aggregation object.");
                    //returnILGenerator.Emit(OpCodes.Call, typeof(Debug).GetMethod("WriteLine", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null));
                    //returnILGenerator.Emit(OpCodes.Ldarg_2);
                    //returnILGenerator.Emit(OpCodes.Castclass, aggregateAttribute.AggregationType);
                    //returnILGenerator.Emit(OpCodes.Ret);

                    var nestMethod = typeBuilder.DefineMethod("nest_" + property.Name, MethodAttributes.Private | MethodAttributes.Final | MethodAttributes.NewSlot, getAggregationMethod.ReturnType, new Type[] { getAggregationMethod.ReturnType, typeof(object) });
                    //.ContinueWith(nest_..., aggregator).Unwrap()
                    aggregateAsyncILGenerator.Emit(OpCodes.Ldarg_0);
                    aggregateAsyncILGenerator.Emit(OpCodes.Ldftn, nestMethod);
                    aggregateAsyncILGenerator.Emit(OpCodes.Newobj, typeof(Func<,,>).MakeGenericType(getAggregationMethod.ReturnType, typeof(object), getAggregationMethod.ReturnType).GetConstructors()[0]);
                    aggregateAsyncILGenerator.Emit(OpCodes.Ldarg_1);
                    aggregateAsyncILGenerator.Emit(OpCodes.Call, getAggregationMethod.ReturnType.GetTypeInfo().GetDeclaredMethods(nameof(Task.ContinueWith)).First(t => t.IsGenericMethodDefinition && t.GetParameters().Length == 2 && t.GetParameters()[1].ParameterType == typeof(object)).MakeGenericMethod(getAggregationMethod.ReturnType));
                    aggregateAsyncILGenerator.Emit(OpCodes.Call, _UnwrapMethodInfo.MakeGenericMethod(aggregateAttribute.AggregationType));

                    var nestILGenerator = nestMethod.GetILGenerator();
                    //var valueVariable = task.Result;
                    var nestValueVariable = nestILGenerator.DeclareLocal(aggregateAttribute.AggregationType);
                    var nestNotFaultLabel = nestILGenerator.DefineLabel();
                    nestILGenerator.Emit(OpCodes.Ldarg_1);
                    nestILGenerator.Emit(OpCodes.Call, typeof(Task).GetProperty(nameof(Task.IsFaulted)).GetMethod);
                    nestILGenerator.Emit(OpCodes.Brfalse, nestNotFaultLabel);
                    nestILGenerator.Emit(OpCodes.Ldarg_1);
                    nestILGenerator.Emit(OpCodes.Call, typeof(Task).GetProperty(nameof(Task.Exception)).GetMethod);
                    nestILGenerator.Emit(OpCodes.Call, typeof(System.Runtime.ExceptionServices.ExceptionDispatchInfo).GetMethod("Capture", BindingFlags.Public | BindingFlags.Static));
                    nestILGenerator.Emit(OpCodes.Call, typeof(System.Runtime.ExceptionServices.ExceptionDispatchInfo).GetMethod("Throw", BindingFlags.Public | BindingFlags.Instance));
                    nestILGenerator.MarkLabel(nestNotFaultLabel);
                    nestILGenerator.Emit(OpCodes.Ldarg_1);
                    nestILGenerator.Emit(OpCodes.Call, getAggregationMethod.ReturnType.GetProperty(nameof(Task<object>.Result)).GetMethod);
                    nestILGenerator.Emit(OpCodes.Dup);
                    nestILGenerator.Emit(OpCodes.Stloc, nestValueVariable);
                    //if (valueVariable == null)
                    //    return task;
                    var nestHasValueLabel = nestILGenerator.DefineLabel();
                    nestILGenerator.Emit(OpCodes.Brtrue, nestHasValueLabel);
                    nestILGenerator.Emit(OpCodes.Ldstr, "No value of aggregation.");
                    nestILGenerator.Emit(OpCodes.Call, typeof(Debug).GetMethod("WriteLine", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null));
                    nestILGenerator.Emit(OpCodes.Ldarg_1);
                    nestILGenerator.Emit(OpCodes.Ret);
                    nestILGenerator.MarkLabel(nestHasValueLabel);
                    //else
                    //    return aggregator.AggregateAsync(valueVariable)
                    nestILGenerator.Emit(OpCodes.Ldstr, "Begin aggregate nest aggregation object.");
                    nestILGenerator.Emit(OpCodes.Call, typeof(Debug).GetMethod("WriteLine", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null));
                    nestILGenerator.Emit(OpCodes.Ldarg_2);
                    nestILGenerator.Emit(OpCodes.Castclass, typeof(IDomainAggregator));
                    nestILGenerator.Emit(OpCodes.Ldloc, nestValueVariable);
                    nestILGenerator.Emit(OpCodes.Callvirt, typeof(IDomainAggregator).GetTypeInfo().GetDeclaredMethods(nameof(IDomainAggregator.AggregateAsync)).First(t => t.IsGenericMethod).MakeGenericMethod(aggregateAttribute.AggregationType));
                    ////      .ContinueWith(return_, valueVariable);
                    //nestILGenerator.Emit(OpCodes.Ldarg_0);
                    //nestILGenerator.Emit(OpCodes.Ldftn, returnMethod);
                    //nestILGenerator.Emit(OpCodes.Newobj, typeof(Func<,,>).MakeGenericType(typeof(Task), typeof(object), aggregateAttribute.AggregationType).GetConstructors()[0]);
                    //nestILGenerator.Emit(OpCodes.Ldloc, nestValueVariable);
                    //nestILGenerator.Emit(OpCodes.Call, typeof(Task).GetTypeInfo().GetDeclaredMethods(nameof(Task.ContinueWith)).First(t => t.IsGenericMethodDefinition && t.GetParameters().Length == 2 && t.GetParameters()[1].ParameterType == typeof(object)).MakeGenericMethod(aggregateAttribute.AggregationType));
                    nestILGenerator.Emit(OpCodes.Ret);
                }
                //.ContinueWith(continue_...)
                aggregateAsyncILGenerator.Emit(OpCodes.Ldarg_0);
                aggregateAsyncILGenerator.Emit(OpCodes.Ldftn, continueMethod);
                aggregateAsyncILGenerator.Emit(OpCodes.Newobj, typeof(Action<>).MakeGenericType(getAggregationMethod.ReturnType).GetConstructors()[0]);
                aggregateAsyncILGenerator.Emit(OpCodes.Call, getAggregationMethod.ReturnType.GetTypeInfo().GetDeclaredMethods(nameof(Task.ContinueWith)).First(t => t.GetParameters().Length == 1 && t.ReturnType == typeof(Task)));
                aggregateAsyncILGenerator.Emit(OpCodes.Call, typeof(List<Task>).GetMethod(nameof(List<Task>.Add)));


                //var valueVariable = task.Result;
                var continueValueVariable = continueILGenerator.DeclareLocal(aggregateAttribute.AggregationType);
                continueILGenerator.Emit(OpCodes.Ldstr, "Aggregate completed.");
                continueILGenerator.Emit(OpCodes.Call, typeof(Debug).GetMethod("WriteLine", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null));
                continueILGenerator.Emit(OpCodes.Ldarg_1);
                continueILGenerator.Emit(OpCodes.Call, getAggregationMethod.ReturnType.GetProperty(nameof(Task<object>.Result)).GetMethod);
                continueILGenerator.Emit(OpCodes.Dup);
                continueILGenerator.Emit(OpCodes.Stloc, continueValueVariable);
                //if (valueVariable == null)
                //    return;
                var continueHasValueLabel = continueILGenerator.DefineLabel();
                continueILGenerator.Emit(OpCodes.Brtrue, continueHasValueLabel);
                continueILGenerator.Emit(OpCodes.Ret);
                continueILGenerator.MarkLabel(continueHasValueLabel);

                if (aggregateAttribute.IsExposed)
                {
                    //Define expose properties of aggregation type
                    foreach (var exposeProperty in aggregateAttribute.AggregationType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(t => t.CanRead))
                    {
                        if (type.GetProperty(exposeProperty.Name) != null)
                            continue;
                        var aggregationField = typeBuilder.DefineField("_" + aggregateAttribute.AggregationName + exposeProperty.Name, exposeProperty.PropertyType, FieldAttributes.Public);
                        var aggregationProperty = typeBuilder.DefineProperty(aggregateAttribute.AggregationName + exposeProperty.Name, PropertyAttributes.None, exposeProperty.PropertyType, Array.Empty<Type>());
                        var aggregationGetMethod = typeBuilder.DefineMethod("get_" + aggregateAttribute.AggregationName + exposeProperty.Name, MethodAttributes.HideBySig | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.Final, exposeProperty.PropertyType, Array.Empty<Type>());
                        var aggregationGetMethodILGenerator = aggregationGetMethod.GetILGenerator();
                        //return this.aggregationField;
                        aggregationGetMethodILGenerator.Emit(OpCodes.Ldarg_0);
                        aggregationGetMethodILGenerator.Emit(OpCodes.Ldfld, aggregationField);
                        aggregationGetMethodILGenerator.Emit(OpCodes.Ret);
                        aggregationProperty.SetGetMethod(aggregationGetMethod);

                        //this._aggregationField = valueVariable.exposeProperty;
                        continueILGenerator.Emit(OpCodes.Ldarg_0);
                        continueILGenerator.Emit(OpCodes.Ldloc, continueValueVariable);
                        continueILGenerator.Emit(OpCodes.Callvirt, exposeProperty.GetMethod);
                        continueILGenerator.Emit(OpCodes.Stfld, aggregationField);
                    }
                }
                else
                {
                    //Define get property for aggregation type
                    var aggregationField = typeBuilder.DefineField("_" + aggregateAttribute.AggregationName, nestType ?? aggregateAttribute.AggregationType, FieldAttributes.Public);
                    var aggregationProperty = typeBuilder.DefineProperty(aggregateAttribute.AggregationName, PropertyAttributes.None, nestType ?? aggregateAttribute.AggregationType, Array.Empty<Type>());
                    var aggregationGetMethod = typeBuilder.DefineMethod("get_" + aggregateAttribute.AggregationName, MethodAttributes.HideBySig | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.Final, nestType ?? aggregateAttribute.AggregationType, Array.Empty<Type>());
                    var aggregationGetMethodILGenerator = aggregationGetMethod.GetILGenerator();
                    //return this.aggregationField;
                    aggregationGetMethodILGenerator.Emit(OpCodes.Ldarg_0);
                    aggregationGetMethodILGenerator.Emit(OpCodes.Ldfld, aggregationField);
                    aggregationGetMethodILGenerator.Emit(OpCodes.Ret);
                    aggregationProperty.SetGetMethod(aggregationGetMethod);

                    //this._aggregationField = valueVariable;
                    continueILGenerator.Emit(OpCodes.Ldarg_0);
                    continueILGenerator.Emit(OpCodes.Ldloc, continueValueVariable);
                    continueILGenerator.Emit(OpCodes.Stfld, aggregationField);
                }

                //return;
                continueILGenerator.Emit(OpCodes.Ret);

                aggregateAsyncILGenerator.MarkLabel(endLabel);

                var finalLabel = aggregateAsyncILGenerator.DefineLabel();
                var falseLabel = aggregateAsyncILGenerator.DefineLabel();
                //if (tasksVariable.Count != 0)
                aggregateAsyncILGenerator.Emit(OpCodes.Ldloc, tasksVariable);
                aggregateAsyncILGenerator.Emit(OpCodes.Call, typeof(List<Task>).GetProperty(nameof(List<Task>.Count)).GetMethod);
                aggregateAsyncILGenerator.Emit(OpCodes.Brfalse, falseLabel);
                //    return Task.WhenAll(tasksVariable);
                aggregateAsyncILGenerator.Emit(OpCodes.Ldstr, $"Waiting aggregate {type.Name} to completed.");
                aggregateAsyncILGenerator.Emit(OpCodes.Call, typeof(Debug).GetMethod("WriteLine", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null));
                aggregateAsyncILGenerator.Emit(OpCodes.Ldloc, tasksVariable);
                aggregateAsyncILGenerator.Emit(OpCodes.Call, typeof(Task).GetMethod(nameof(Task.WhenAll), BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(IEnumerable<>).MakeGenericType(typeof(Task)) }, null));
                aggregateAsyncILGenerator.Emit(OpCodes.Br, finalLabel);

                //else
                //    return Task.CompletedTask;
                aggregateAsyncILGenerator.MarkLabel(falseLabel);
                aggregateAsyncILGenerator.Emit(OpCodes.Ldstr, $"End aggregate {type.Name}. Nothing need to aggregate.");
                aggregateAsyncILGenerator.Emit(OpCodes.Call, typeof(Debug).GetMethod("WriteLine", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null));
                aggregateAsyncILGenerator.Emit(OpCodes.Call, typeof(Task).GetProperty(nameof(Task.CompletedTask)).GetMethod);
                aggregateAsyncILGenerator.MarkLabel(finalLabel);
                aggregateAsyncILGenerator.Emit(OpCodes.Ret);
            }

            AggregationType = typeBuilder.CreateTypeInfo().AsType();
            Constructor = AggregationType.GetConstructor(new Type[] { type });
        }

        public static bool HasAggregation { get; private set; }

        public static Type AggregationType { get; private set; }

        internal static ConstructorInfo Constructor { get; private set; }
    }
}
