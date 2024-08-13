using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Wodsoft.ComBoost.Grpc
{
    public class DomainGrpcArgument<T1>
    {
        public DomainGrpcArgument() { }

        public DomainGrpcArgument(T1 arg1)
        {
            Argument1 = arg1;
        }

        [DataMember(Order = 1)]
        public T1? Argument1 { get; set; }
    }

    public class DomainGrpcArgument<T1, T2> : DomainGrpcArgument<T1>
    {
        public DomainGrpcArgument() { }

        public DomainGrpcArgument(T1 arg1, T2 arg2) : base(arg1)
        {
            Argument2 = arg2;
        }

        [DataMember(Order = 2)]
        public T2? Argument2 { get; set; }
    }

    public class DomainGrpcArgument<T1, T2, T3> : DomainGrpcArgument<T1, T2>
    {
        public DomainGrpcArgument() { }

        public DomainGrpcArgument(T1 arg1, T2 arg2, T3 arg3) : base(arg1, arg2)
        {
            Argument3 = arg3;
        }

        [DataMember(Order = 3)]
        public T3? Argument3 { get; set; }
    }

    public class DomainGrpcArgument<T1, T2, T3, T4> : DomainGrpcArgument<T1, T2, T3>
    {
        public DomainGrpcArgument() { }

        public DomainGrpcArgument(T1 arg1, T2 arg2, T3 arg3, T4 arg4) : base(arg1, arg2, arg3)
        {
            Argument4 = arg4;
        }

        [DataMember(Order = 4)]
        public T4? Argument4 { get; set; }
    }

    public class DomainGrpcArgument<T1, T2, T3, T4, T5> : DomainGrpcArgument<T1, T2, T3, T4>
    {
        public DomainGrpcArgument() { }

        public DomainGrpcArgument(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) : base(arg1, arg2, arg3, arg4)
        {
            Argument5 = arg5;
        }

        [DataMember(Order = 5)]
        public T5? Argument5 { get; set; }
    }

    public class DomainGrpcArgument<T1, T2, T3, T4, T5, T6> : DomainGrpcArgument<T1, T2, T3, T4, T5>
    {
        public DomainGrpcArgument() { }

        public DomainGrpcArgument(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) : base(arg1, arg2, arg3, arg4, arg5)
        {
            Argument6 = arg6;
        }

        [DataMember(Order = 6)]
        public T6? Argument6 { get; set; }
    }

    public class DomainGrpcArgument<T1, T2, T3, T4, T5, T6, T7> : DomainGrpcArgument<T1, T2, T3, T4, T5, T6>
    {
        public DomainGrpcArgument() { }

        public DomainGrpcArgument(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) : base(arg1, arg2, arg3, arg4, arg5, arg6)
        {
            Argument7 = arg7;
        }

        [DataMember(Order = 7)]
        public T7? Argument7 { get; set; }
    }

    public class DomainGrpcArgument<T1, T2, T3, T4, T5, T6, T7, T8> : DomainGrpcArgument<T1, T2, T3, T4, T5, T6, T7>
    {
        public DomainGrpcArgument() { }

        public DomainGrpcArgument(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8) : base(arg1, arg2, arg3, arg4, arg5, arg6, arg7)
        {
            Argument8 = arg8;
        }

        [DataMember(Order = 8)]
        public T8? Argument8 { get; set; }
    }

    public class DomainGrpcArgument<T1, T2, T3, T4, T5, T6, T7, T8, T9> : DomainGrpcArgument<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        public DomainGrpcArgument() { }

        public DomainGrpcArgument(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9) : base(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8)
        {
            Argument9 = arg9;
        }

        [DataMember(Order = 9)]
        public T9? Argument9 { get; set; }
    }

    public class DomainGrpcArgument<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : DomainGrpcArgument<T1, T2, T3, T4, T5, T6, T7, T8, T9>
    {
        public DomainGrpcArgument() { }

        public DomainGrpcArgument(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10) : base(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9)
        {
            Argument10 = arg10;
        }

        [DataMember(Order = 10)]
        public T10? Argument10 { get; set; }
    }

    public class DomainGrpcArgument<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : DomainGrpcArgument<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
    {
        public DomainGrpcArgument() { }

        public DomainGrpcArgument(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
            : base(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10)
        {
            Argument11 = arg11;
        }

        [DataMember(Order = 11)]
        public T11? Argument11 { get; set; }
    }

    public class DomainGrpcArgument<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : DomainGrpcArgument<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
    {
        public DomainGrpcArgument() { }

        public DomainGrpcArgument(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
            : base(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11)
        {
            Argument12 = arg12;
        }

        [DataMember(Order = 12)]
        public T12? Argument12 { get; set; }
    }

    public class DomainGrpcArgument<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : DomainGrpcArgument<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
    {
        public DomainGrpcArgument() { }

        public DomainGrpcArgument(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
            : base(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12)
        {
            Argument13 = arg13;
        }

        [DataMember(Order = 13)]
        public T13? Argument13 { get; set; }
    }

    public class DomainGrpcArgument<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : DomainGrpcArgument<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>
    {
        public DomainGrpcArgument() { }

        public DomainGrpcArgument(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
            : base(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13)
        {
            Argument14 = arg14;
        }

        [DataMember(Order = 14)]
        public T14? Argument14 { get; set; }
    }

    public class DomainGrpcArgument<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : DomainGrpcArgument<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>
    {
        public DomainGrpcArgument() { }

        public DomainGrpcArgument(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
            : base(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14)
        {
            Argument15 = arg15;
        }

        [DataMember(Order = 15)]
        public T15? Argument15 { get; set; }
    }

    public class DomainGrpcArgumentHelper
    {
        public static Type GetArgumentType(params Type[] types)
        {
            switch (types.Length)
            {
                case 1:
                    return typeof(DomainGrpcArgument<>).MakeGenericType(types);
                case 2:
                    return typeof(DomainGrpcArgument<,>).MakeGenericType(types);
                case 3:
                    return typeof(DomainGrpcArgument<,,>).MakeGenericType(types);
                case 4:
                    return typeof(DomainGrpcArgument<,,,>).MakeGenericType(types);
                case 5:
                    return typeof(DomainGrpcArgument<,,,,>).MakeGenericType(types);
                case 6:
                    return typeof(DomainGrpcArgument<,,,,,>).MakeGenericType(types);
                case 7:
                    return typeof(DomainGrpcArgument<,,,,,,>).MakeGenericType(types);
                case 8:
                    return typeof(DomainGrpcArgument<,,,,,,,>).MakeGenericType(types);
                case 9:
                    return typeof(DomainGrpcArgument<,,,,,,,,>).MakeGenericType(types);
                case 10:
                    return typeof(DomainGrpcArgument<,,,,,,,,,>).MakeGenericType(types);
                case 11:
                    return typeof(DomainGrpcArgument<,,,,,,,,,,>).MakeGenericType(types);
                case 12:
                    return typeof(DomainGrpcArgument<,,,,,,,,,,,>).MakeGenericType(types);
                case 13:
                    return typeof(DomainGrpcArgument<,,,,,,,,,,,,>).MakeGenericType(types);
                case 14:
                    return typeof(DomainGrpcArgument<,,,,,,,,,,,,,>).MakeGenericType(types);
                case 15:
                    return typeof(DomainGrpcArgument<,,,,,,,,,,,,,,>).MakeGenericType(types);
                default:
                    throw new ArgumentOutOfRangeException(nameof(types), "Argument count must more than zero and less than or equal 15.");
            }
        }
    }
}
