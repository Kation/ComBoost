using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.Runtime.Serialization.Formatters
{
    /// <summary>
    /// Serializer of Linq Expression.
    /// </summary>
    public class ExpressionSerializer
    {
        private SerializerHeader<ParameterExpression> _Parameters;
        private SerializerHeader<Type> _Types;

        private Type ReadType(Stream stream)
        {
            return _Types.GetHeader(stream.ReadByte());
        }

        private Type ReadTypeCore(Stream stream)
        {
            byte[] lengthData = new byte[2];
            stream.Read(lengthData, 0, 2);
            ushort length = BitConverter.ToUInt16(lengthData, 0);
            byte[] nameData = new byte[length];
            stream.Read(nameData, 0, length);
            string name = Encoding.UTF8.GetString(nameData, 0, length);
            Type type = Type.GetType(name, true);
            if (type.IsGenericTypeDefinition)
            {
                Type[] parameters = new Type[stream.ReadByte()];
                for (int i = 0; i < parameters.Length; i++)
                    parameters[i] = ReadTypeCore(stream);
                type = type.MakeGenericType(parameters);
            }
            return type;
        }

        private void WriteType(Stream stream, Type type)
        {
            stream.WriteByte((byte)_Types.GetIndex(type));
        }

        private void WriteTypeCore(Stream stream, Type type)
        {
            string name;
            if (type.IsGenericType)
            {
                name = type.GetGenericTypeDefinition().AssemblyQualifiedName;
                byte[] nameData = Encoding.UTF8.GetBytes(name);
                byte[] length = BitConverter.GetBytes((ushort)nameData.Length);
                stream.Write(length, 0, 2);
                stream.Write(nameData, 0, nameData.Length);
                stream.WriteByte((byte)type.GenericTypeArguments.Length);
                foreach (var item in type.GenericTypeArguments)
                    WriteTypeCore(stream, item);
            }
            else
            {
                name = type.AssemblyQualifiedName;
                byte[] nameData = Encoding.UTF8.GetBytes(name);
                byte[] length = BitConverter.GetBytes((ushort)nameData.Length);
                stream.Write(length, 0, 2);
                stream.Write(nameData, 0, nameData.Length);
            }
        }

        #region Deserialize

        private IQueryProvider _Queryable;
        /// <summary>
        /// Deserialize expression from stream to build a queryalbe.
        /// </summary>
        /// <param name="stream">Stream data.</param>
        /// <param name="queryableProvider">Queryable object.</param>
        /// <returns></returns>
        public IQueryable Deserialize(Stream stream, IQueryProvider queryableProvider)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            _Parameters = new SerializerHeader<ParameterExpression>();
            _Types = new SerializerHeader<Type>();
            _Queryable = queryableProvider;

            int count = stream.ReadByte();
            for (int i = 0; i < count; i++)
            {
                Type type = ReadTypeCore(stream);
                _Types.AddHeader(type);
            }

            count = stream.ReadByte();
            for (int i = 0; i < count; i++)
            {
                //Read Expression Type
                Type type = _Types.GetHeader(stream.ReadByte());

                //Read Member Name
                int length = stream.ReadByte();
                byte[] nameData = new byte[length];
                stream.Read(nameData, 0, length);
                string name = Encoding.UTF8.GetString(nameData, 0, length);

                ParameterExpression parameter = Expression.Parameter(type, name);
                _Parameters.AddHeader(parameter);
            }

            Expression expression = Deserialize(stream);

            _Parameters = null;
            _Types = null;
            _Queryable = null;

            return queryableProvider.CreateQuery(expression);
        }

        /// <summary>
        /// Deserialize expression from stream to build a queryalbe.
        /// </summary>
        /// <param name="stream">Stream data.</param>
        /// <param name="queryableProvider">Query single object.</param>
        /// <returns></returns>
        public object DeserializeSingle(Stream stream, IQueryProvider queryableProvider)
        {
                        if (stream == null)
                throw new ArgumentNullException("stream");

            _Parameters = new SerializerHeader<ParameterExpression>();
            _Types = new SerializerHeader<Type>();
            _Queryable = queryableProvider;

            int count = stream.ReadByte();
            for (int i = 0; i < count; i++)
            {
                Type type = ReadTypeCore(stream);
                _Types.AddHeader(type);
            }

            count = stream.ReadByte();
            for (int i = 0; i < count; i++)
            {
                //Read Expression Type
                Type type = _Types.GetHeader(stream.ReadByte());

                //Read Member Name
                int length = stream.ReadByte();
                byte[] nameData = new byte[length];
                stream.Read(nameData, 0, length);
                string name = Encoding.UTF8.GetString(nameData, 0, length);

                ParameterExpression parameter = Expression.Parameter(type, name);
                _Parameters.AddHeader(parameter);
            }

            Expression expression = Deserialize(stream);

            _Parameters = null;
            _Types = null;
            _Queryable = null;

            return queryableProvider.Execute(expression);
        }

        private Expression Deserialize(Stream stream)
        {
            Type type = _Types.GetHeader(stream.ReadByte());
            if (typeof(BinaryExpression).IsAssignableFrom(type))
                return DeserializeBinary(stream);
            else if (typeof(ConstantExpression).IsAssignableFrom(type))
                return DeserializeConstant(stream);
            else if (typeof(LambdaExpression).IsAssignableFrom(type))
                return DeserializeLambda(stream);
            else if (typeof(MemberExpression).IsAssignableFrom(type))
                return DeserializeMember(stream);
            else if (typeof(MethodCallExpression).IsAssignableFrom(type))
                return DeserializeMethodCall(stream);
            else if (typeof(ParameterExpression).IsAssignableFrom(type))
                return _Parameters.GetHeader(stream.ReadByte());
            else if (typeof(UnaryExpression).IsAssignableFrom(type))
                return DeserializeUnary(stream);
            else
                throw new NotSupportedException();
        }

        private BinaryExpression DeserializeBinary(Stream stream)
        {
            ExpressionType nodeType = (ExpressionType)stream.ReadByte();

            //Type type = ReadType(stream);
            Expression left = Deserialize(stream);

            //type = ReadType(stream);
            Expression right = Deserialize(stream);

            switch (nodeType)
            {
                case ExpressionType.Add:
                    return Expression.And(left, right);
                case ExpressionType.AddAssign:
                    return Expression.AddAssign(left, right);
                case ExpressionType.AddAssignChecked:
                    return Expression.AddAssignChecked(left, right);
                case ExpressionType.AddChecked:
                    return Expression.AddChecked(left, right);
                case ExpressionType.AndAlso:
                    return Expression.AndAlso(left, right);
                case ExpressionType.Assign:
                    return Expression.Assign(left, right);
                case ExpressionType.ArrayIndex:
                    return Expression.ArrayIndex(left, right);
                case ExpressionType.Coalesce:
                    return Expression.Coalesce(left, right);
                case ExpressionType.Divide:
                    return Expression.Divide(left, right);
                case ExpressionType.DivideAssign:
                    return Expression.DivideAssign(left, right);
                case ExpressionType.Equal:
                    return Expression.Equal(left, right);
                case ExpressionType.ExclusiveOr:
                    return Expression.ExclusiveOr(left, right);
                case ExpressionType.ExclusiveOrAssign:
                    return Expression.ExclusiveOrAssign(left, right);
                case ExpressionType.GreaterThan:
                    return Expression.GreaterThan(left, right);
                case ExpressionType.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(left, right);
                case ExpressionType.LeftShift:
                    return Expression.LeftShift(left, right);
                case ExpressionType.LeftShiftAssign:
                    return Expression.LeftShiftAssign(left, right);
                case ExpressionType.LessThan:
                    return Expression.LessThan(left, right);
                case ExpressionType.LessThanOrEqual:
                    return Expression.LessThanOrEqual(left, right);
                case ExpressionType.Modulo:
                    return Expression.Modulo(left, right);
                case ExpressionType.ModuloAssign:
                    return Expression.ModuloAssign(left, right);
                case ExpressionType.Multiply:
                    return Expression.Multiply(left, right);
                case ExpressionType.MultiplyAssign:
                    return Expression.MultiplyAssign(left, right);
                case ExpressionType.MultiplyAssignChecked:
                    return Expression.MultiplyAssignChecked(left, right);
                case ExpressionType.MultiplyChecked:
                    return Expression.MultiplyChecked(left, right);
                case ExpressionType.NotEqual:
                    return Expression.NotEqual(left, right);
                case ExpressionType.Or:
                    return Expression.Or(left, right);
                case ExpressionType.OrAssign:
                    return Expression.OrAssign(left, right);
                case ExpressionType.OrElse:
                    return Expression.OrElse(left, right);
                case ExpressionType.Power:
                    return Expression.Power(left, right);
                case ExpressionType.PowerAssign:
                    return Expression.PowerAssign(left, right);
                case ExpressionType.RightShift:
                    return Expression.RightShift(left, right);
                case ExpressionType.RightShiftAssign:
                    return Expression.RightShiftAssign(left, right);
                case ExpressionType.Subtract:
                    return Expression.Subtract(left, right);
                case ExpressionType.SubtractAssign:
                    return Expression.SubtractAssign(left, right);
                case ExpressionType.SubtractAssignChecked:
                    return Expression.SubtractAssignChecked(left, right);
                case ExpressionType.SubtractChecked:
                    return Expression.SubtractChecked(left, right);
                default:
                    throw new NotSupportedException();
            }
        }

        private ConstantExpression DeserializeConstant(Stream stream)
        {
            bool isNull = stream.ReadByte() == 0;
            if (isNull)
                return Expression.Constant(null);

            bool root = stream.ReadByte() == 1;
            if (root)
                return Expression.Constant(_Queryable);

            Type type = ReadType(stream);

            byte[] lengthData = new byte[4];
            stream.Read(lengthData, 0, 4);
            int length = BitConverter.ToInt32(lengthData, 0);

            byte[] data = new byte[length];
            stream.Read(data, 0, length);
            MemoryStream ms = new MemoryStream(data);
            ms.Position = 0;

            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter serializer = new Binary.BinaryFormatter();
            object obj = serializer.Deserialize(ms);
            return Expression.Constant(obj, type);
        }

        private LambdaExpression DeserializeLambda(Stream stream)
        {
            //Read Lambda Delegate Type
            Type type = ReadType(stream);

            //Read TailCall
            bool tailCall = stream.ReadByte() == 1;

            //Read Parameter Count
            int parameterCount = stream.ReadByte();
            ParameterExpression[] parameters = new ParameterExpression[parameterCount];

            //Write Parameters
            for (int i = 0; i < parameterCount; i++)
                parameters[i] = _Parameters.GetHeader(stream.ReadByte());// DeserializeParameter(stream);

            //Read Lambda Expression Type
            //Type bodyType = ReadType(stream);

            //Read Lambda Expression
            Expression body = Deserialize(stream);

            return Expression.Lambda(type, body, tailCall, parameters);
        }

        private MemberExpression DeserializeMember(Stream stream)
        {
            //Read Expression Type
            //Type type = ReadType(stream);

            //Read Expression
            Expression expression = Deserialize(stream);

            //Read Member Name
            int length = stream.ReadByte();
            byte[] nameData = new byte[length];
            stream.Read(nameData, 0, length);
            string name = Encoding.UTF8.GetString(nameData, 0, length);

            return Expression.PropertyOrField(expression, name);
        }

        private MethodCallExpression DeserializeMethodCall(Stream stream)
        {
            //Read Object Expression
            Expression expression = null;
            if (stream.ReadByte() == 1)
            {
                //Type type = ReadType(stream);
                expression = Deserialize(stream);
            }

            //Read Method Declaring Type
            Type declaringType = ReadType(stream);

            //Read Method Name
            int length = stream.ReadByte();
            byte[] nameData = new byte[length];
            stream.Read(nameData, 0, length);
            string name = Encoding.UTF8.GetString(nameData, 0, length);

            //Get Method
            var methods = declaringType.GetMethods().Where(t => t.Name == name).ToArray();
            MethodInfo method = null;
            if (methods.Length == 1)
                method = methods[0];
            else
            {
                int parameterLength = stream.ReadByte();
                if (methods.Count(t => t.GetParameters().Length == parameterLength) == 1)
                {
                    method = methods.Single(t => t.GetParameters().Length == parameterLength);
                }
                else
                {
                    Type[] parameters = new Type[parameterLength];
                    for (int i = 0; i < parameterLength; i++)
                    {
                        parameters[i] = ReadType(stream);
                    }
                    foreach (var item in methods.Where(t => t.GetParameters().Length == parameterLength))
                    {
                        var p = item.GetParameters();
                        bool success = true;
                        for (int i = 0; i < parameterLength; i++)
                        {
                            if (p[i].ParameterType.Name != parameters[i].Name)
                            {
                                success = false;
                                break;
                            }
                        }
                        if (success)
                        {
                            method = item;
                            break;
                        }
                    }
                }
            }

            //Read Method Generic
            if (method.IsGenericMethodDefinition)
            {
                int l = stream.ReadByte();
                Type[] gp = new Type[l];
                for (int i = 0; i < l; i++)
                    gp[i] = ReadType(stream);
                method = method.MakeGenericMethod(gp);
            }

            //Read Parameters
            int argumentLength = stream.ReadByte();
            Expression[] arguments = new Expression[argumentLength];
            for (int i = 0; i < argumentLength; i++)
            {
                //Type aType = ReadType(stream);
                arguments[i] = Deserialize(stream);
            }

            if (expression == null)
                return Expression.Call(method, arguments);
            else
                return Expression.Call(expression, method, arguments);
        }

        //private ParameterExpression DeserializeParameter(Stream stream)
        //{
        //    //Read Expression Type
        //    Type type = ReadType(stream);

        //    //Read Member Name
        //    int length = stream.ReadByte();
        //    byte[] nameData = new byte[length];
        //    stream.Read(nameData, 0, length);
        //    string name = Encoding.UTF8.GetString(nameData, 0, length);

        //    return Expression.Parameter(type, name);
        //}

        private UnaryExpression DeserializeUnary(Stream stream)
        {
            ExpressionType nodeType = (ExpressionType)stream.ReadByte();

            Type expType = ReadType(stream);

            //Type type = ReadType(stream);
            Expression expression = Deserialize(stream);

            switch (nodeType)
            {
                case ExpressionType.Convert:
                    return Expression.Convert(expression, expType);
                case ExpressionType.ConvertChecked:
                    return Expression.ConvertChecked(expression, expType);
                case ExpressionType.Decrement:
                    return Expression.Decrement(expression);
                case ExpressionType.Increment:
                    return Expression.Increment(expression);
                case ExpressionType.IsFalse:
                    return Expression.IsFalse(expression);
                case ExpressionType.IsTrue:
                    return Expression.IsTrue(expression);
                case ExpressionType.Negate:
                    return Expression.Negate(expression);
                case ExpressionType.NegateChecked:
                    return Expression.NegateChecked(expression);
                case ExpressionType.Not:
                    return Expression.Not(expression);
                case ExpressionType.OnesComplement:
                    return Expression.OnesComplement(expression);
                case ExpressionType.PostDecrementAssign:
                    return Expression.PostDecrementAssign(expression);
                case ExpressionType.PreDecrementAssign:
                    return Expression.PreDecrementAssign(expression);
                case ExpressionType.Quote:
                    return Expression.Quote(expression);
                case ExpressionType.Throw:
                    return Expression.Throw(expression);
                case ExpressionType.TypeAs:
                    return Expression.TypeAs(expression, expType);
                case ExpressionType.UnaryPlus:
                    return Expression.UnaryPlus(expression);
                case ExpressionType.Unbox:
                    return Expression.Unbox(expression, expType);
                default:
                    throw new NotSupportedException();
            }
        }

        #endregion

        #region Serialize

        /// <summary>
        /// Serialize IQueryable's expression to stream.
        /// </summary>
        /// <param name="stream">Operation stream.</param>
        /// <param name="queryable">IQueryable object.</param>
        public void Serialize(Stream stream, IQueryable queryable)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (queryable == null)
                throw new ArgumentNullException("queryable");
            _Parameters = new SerializerHeader<ParameterExpression>();
            _Types = new SerializerHeader<Type>();

            _Types.AddHeader(queryable.Expression.GetType());

            MemoryStream ms = new MemoryStream();
            ms.WriteByte(0);
            Serialize(ms, queryable.Expression);

            stream.WriteByte((byte)_Types.Count);
            for (int i = 0; i < _Types.Count; i++)
                WriteTypeCore(stream, _Types.GetHeader(i));

            stream.WriteByte((byte)_Parameters.Count);
            for (int i = 0; i < _Parameters.Count; i++)
            {
                ParameterExpression parameter = _Parameters.GetHeader(i);

                //Write Type
                stream.WriteByte((byte)_Types.GetIndex(parameter.Type));

                //Write Member Name Length
                byte[] nameData = Encoding.UTF8.GetBytes(parameter.Name);
                stream.WriteByte((byte)nameData.Length);

                //Write Member Name
                stream.Write(nameData, 0, nameData.Length);
            }
            ms.Position = 0;
            ms.CopyTo(stream);
            ms.Dispose();

            _Parameters = null;
            _Types = null;
        }

        private void Serialize(Stream stream, Expression expression)
        {
            if (expression is BinaryExpression)
                Serialize(stream, (BinaryExpression)expression);
            else if (expression is ConstantExpression)
                Serialize(stream, (ConstantExpression)expression);
            else if (expression is LambdaExpression)
                Serialize(stream, (LambdaExpression)expression);
            else if (expression is MemberExpression)
                Serialize(stream, (MemberExpression)expression);
            else if (expression is MethodCallExpression)
                Serialize(stream, (MethodCallExpression)expression);
            else if (expression is ParameterExpression)
                Serialize(stream, (ParameterExpression)expression);
            else if (expression is UnaryExpression)
                Serialize(stream, (UnaryExpression)expression);
            else
                throw new NotSupportedException();
        }

        private void Serialize(Stream stream, BinaryExpression expression)
        {
            stream.WriteByte((byte)expression.NodeType);

            Type type = expression.Left.GetType();
            WriteType(stream, type);
            Serialize(stream, expression.Left);

            type = expression.Right.GetType();
            WriteType(stream, type);
            Serialize(stream, expression.Right);
        }

        private void Serialize(Stream stream, ConstantExpression expression)
        {
            if (expression.Value == null)
            {
                stream.WriteByte(0);
                return;
            }
            stream.WriteByte(1);
            if (expression.Value is IQueryable)
            {
                stream.WriteByte(1);
            }
            else
            {
                stream.WriteByte(0);
                WriteType(stream, expression.Value.GetType());

                MemoryStream ms = new MemoryStream();
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new Binary.BinaryFormatter();
                formatter.Serialize(ms, expression.Value);

                byte[] msl = BitConverter.GetBytes((int)ms.Length);
                stream.Write(msl, 0, msl.Length);

                byte[] data = ms.ToArray();
                stream.Write(data, 0, data.Length);
            }
        }

        private void Serialize(Stream stream, LambdaExpression expression)
        {
            //Write Lambda Delegate Type
            WriteType(stream, expression.Type);

            //Write TailCall
            stream.WriteByte(expression.TailCall ? (byte)1 : (byte)0);

            //Write Parameter Count
            stream.WriteByte((byte)expression.Parameters.Count);

            //Write Parameters
            for (int i = 0; i < expression.Parameters.Count; i++)
                Serialize(stream, expression.Parameters[i]);

            //Write Lambda Expression Type
            WriteType(stream, expression.Body.GetType());

            //Write Lambda Expression
            Serialize(stream, expression.Body);
        }

        private void Serialize(Stream stream, MemberExpression expression)
        {
            //Write Expression Type
            WriteType(stream, expression.Expression.GetType());

            //Write Expression
            Serialize(stream, expression.Expression);

            //Write Member Name Length
            byte[] nameData = Encoding.UTF8.GetBytes(expression.Member.Name);
            stream.WriteByte((byte)nameData.Length);

            //Write Member Name
            stream.Write(nameData, 0, nameData.Length);
        }

        private void Serialize(Stream stream, MethodCallExpression expression)
        {
            //Write Object Expression
            if (expression.Object == null)
                stream.WriteByte(0);
            else
            {
                stream.WriteByte(1);
                WriteType(stream, expression.Object.GetType());
                Serialize(stream, expression.Object);
            }

            //Write Method Declaring Type
            WriteType(stream, expression.Method.DeclaringType);

            //Write Method Name
            byte[] nameData = Encoding.UTF8.GetBytes(expression.Method.Name);
            stream.WriteByte((byte)nameData.Length); //max 256 byte for name data
            stream.Write(nameData, 0, nameData.Length);

            //Write Multi Method
            var methods = expression.Method.DeclaringType.GetMethods().Where(t => t.Name == expression.Method.Name).ToArray();
            if (methods.Length != 0)
            {
                //Write Method Parameters
                var parameters = expression.Method.GetParameters();
                stream.WriteByte((byte)parameters.Length);
                if (methods.Count(t => t.GetParameters().Length == parameters.Length) != 1)
                {
                    for (int i = 0; i < parameters.Length; i++)
                        WriteType(stream, parameters[i].ParameterType);
                }
            }

            //Write Method Generic
            if (expression.Method.IsGenericMethod)
            {
                var types = expression.Method.GetGenericArguments();
                stream.WriteByte((byte)types.Length);
                foreach (var t in types)
                    WriteType(stream, t);
            }

            //Write Arguments
            stream.WriteByte((byte)expression.Arguments.Count);
            foreach (var item in expression.Arguments)
            {
                WriteType(stream, item.GetType());
                Serialize(stream, item);
            }
        }

        private void Serialize(Stream stream, ParameterExpression expression)
        {
            _Types.GetIndex(expression.Type);
            stream.WriteByte((byte)_Parameters.GetIndex(expression));
        }

        private void Serialize(Stream stream, UnaryExpression expression)
        {
            stream.WriteByte((byte)expression.NodeType);

            WriteType(stream, expression.Type);

            Type type = expression.Operand.GetType();
            WriteType(stream, type);
            Serialize(stream, expression.Operand);
        }

        #endregion
    }
}
