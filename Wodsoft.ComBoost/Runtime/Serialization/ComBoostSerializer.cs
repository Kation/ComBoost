using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Runtime.Serialization
{
    /// <summary>
    /// Comboost serializer.
    /// </summary>
    public class ComBoostSerializer
    {
        private object _Lock;
        private List<Type> _TypeStorage;
        private static Hashtable _TypeProperty;
        private static Hashtable _PropertyGetLambda;
        private static Hashtable _PropertySetLambda;

        static ComBoostSerializer()
        {
            _TypeProperty = new Hashtable();
            _PropertyGetLambda = new Hashtable();
            _PropertySetLambda = new Hashtable();
        }

        /// <summary>
        /// Initialize comboost serialzer.
        /// </summary>
        public ComBoostSerializer()
        {
            _Lock = new object();
            _TypeStorage = new List<Type>();
            _TypeStorage.Add(null);
            IsTypeExtensionEnabled = false;
        }

        /// <summary>
        /// Get or set the type extension enabled.
        /// </summary>
        public bool IsTypeExtensionEnabled { get; set; }

        /// <summary>
        /// Serialize an object.
        /// </summary>
        /// <typeparam name="T">Type of object.</typeparam>
        /// <param name="stream">Data stream.</param>
        /// <param name="obj">Object to serialize.</param>
        public void Serialize<T>(Stream stream, T obj)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (obj == null)
                throw new ArgumentNullException("obj");
            Monitor.Enter(_Lock);
            try
            {
                if (IsTypeExtensionEnabled)
                {
                    stream.Position += 4;
                    var position = stream.Position;
                    SerializeObject(stream, typeof(T), obj);
                    int length = (int)(stream.Position - position);
                    stream.Position = position -= 4;
                    var data = BitConverter.GetBytes(length);
                    stream.Write(data, 0, data.Length);
                    stream.Seek(length, SeekOrigin.Current);

                    stream.WriteByte((byte)(_TypeStorage.Count - 1));
                    for (int i = 1; i < _TypeStorage.Count; i++)
                    {
                        Type type = _TypeStorage[i];
                        if (type.IsGenericType)
                        {
                            var typeData = Encoding.UTF8.GetBytes(ConvertTypeToString(type.GetGenericTypeDefinition()));
                            var typeLength = BitConverter.GetBytes((ushort)typeData.Length);
                            stream.Write(typeLength, 0, typeLength.Length);
                            stream.Write(typeData, 0, typeData.Length);
                            foreach (var parameter in type.GetGenericArguments().Select(t => GetTypeIndex(t)))
                                stream.WriteByte(parameter);
                        }
                        else
                        {
                            var typeData = Encoding.UTF8.GetBytes(ConvertTypeToString(_TypeStorage[i]));
                            var typeLength = BitConverter.GetBytes((ushort)typeData.Length);
                            stream.Write(typeLength, 0, typeLength.Length);
                            stream.Write(typeData, 0, typeData.Length);
                        }
                    }
                }
                else
                    SerializeObject(stream, typeof(T), obj);
            }
            finally
            {
                _TypeStorage.Clear();
                _TypeStorage.Add(null);
                Monitor.Exit(_Lock);
            }
        }

        /// <summary>
        /// Deserialize an object.
        /// </summary>
        /// <typeparam name="T">Type of object.</typeparam>
        /// <param name="stream">Data stream.</param>
        /// <returns></returns>
        public T Deserialize<T>(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            Monitor.Enter(_Lock);
            try
            {
                if (IsTypeExtensionEnabled)
                {
                    var dataLengthData = new byte[sizeof(int)];
                    stream.Read(dataLengthData, 0, dataLengthData.Length);
                    var dataLength = BitConverter.ToInt32(dataLengthData, 0);
                    long dataPosition = stream.Position;
                    stream.Seek(dataLength, SeekOrigin.Current);
                    int typeLength = stream.ReadByte();
                    for (int i = 0; i < typeLength; i++)
                    {
                        byte[] lengthData = new byte[2];
                        stream.Read(lengthData, 0, 2);
                        ushort length = BitConverter.ToUInt16(lengthData, 0);
                        byte[] data = new byte[length];
                        stream.Read(data, 0, length);
                        string typeName = Encoding.UTF8.GetString(data);
                        Type type = Type.GetType(typeName, true, false);
                        if (type.IsGenericTypeDefinition)
                            type = type.MakeGenericType(((TypeInfo)type).GenericTypeParameters.Select(t => GetType((byte)stream.ReadByte())).ToArray());
                        _TypeStorage.Add(type);
                    }
                    var end = stream.Position;
                    stream.Position = dataPosition;
                    T result = (T)DeserializeObject(stream, typeof(T));
                    stream.Position = end;
                    return result;
                }
                else
                {
                    T result = (T)DeserializeObject(stream, typeof(T));
                    return result;
                }
            }
            finally
            {
                _TypeStorage.Clear();
                _TypeStorage.Add(null);
                Monitor.Exit(_Lock);
            }
        }

        /// <summary>
        /// Serialize object.
        /// </summary>
        /// <param name="stream">Data stream.</param>
        /// <param name="type">Type of object.</param>
        /// <param name="obj">Object to serialize.</param>
        protected virtual void SerializeObject(Stream stream, Type type, object obj)
        {
            if (obj is IList)
            {
                //if (type.GetConstructor(new Type[0]) == null)
                //    throw new NotSupportedException("Type of \"" + type.FullName + "\" not support.");
                //stream.WriteByte(GetTypeIndex(type));
                var ilist = type.GetInterface("IList`1");
                if (ilist == null)
                    throw new NotSupportedException("Type of \"" + type.FullName + "\" not support.");
                Type targetType = ilist.GetGenericArguments()[0];
                IList list = (IList)obj;
                var lengthData = BitConverter.GetBytes(list.Count);
                stream.Write(lengthData, 0, lengthData.Length);
                for (int i = 0; i < list.Count; i++)
                {
                    object value = list[i];
                    if (value == null)
                    {
                        stream.WriteByte(0);
                        continue;
                    }
                    stream.WriteByte(1);
                    //stream.WriteByte(GetTypeIndex(value.GetType()));
                    SerializeValue(stream, targetType, value);
                }
            }
            else if (obj is Array)
            {
                Array array = (Array)obj;
                if (array.Rank > 255)
                    throw new NotSupportedException("Rank of array more than 255.");
                stream.WriteByte((byte)array.Rank);
                for (int i = 0; i < array.Rank; i++)
                {
                    var min = BitConverter.GetBytes(array.GetLowerBound(i));
                    var max = BitConverter.GetBytes(array.GetUpperBound(i));
                    stream.Write(min, 0, min.Length);
                    stream.Write(max, 0, max.Length);
                }
                SerializeArray(stream, array, new int[0]);
            }
            else
            {
                //if (type.GetConstructor(new Type[0]) == null)
                //    throw new NotSupportedException("Type \"" + type.FullName + "\" not support without no parameter constructor.");
                //stream.WriteByte(GetTypeIndex(type));
                long position = stream.Position;
                byte count = 0;
                byte index = 0;
                stream.Position++;
                PropertyInfo[] properties;
                if (!_TypeProperty.ContainsKey(type))
                {
                    properties = type.GetProperties().Where(t => t.CanWrite && t.CanRead).ToArray();
                    _TypeProperty.Add(type, properties);
                }
                else
                    properties = (PropertyInfo[])_TypeProperty[type];
                foreach (var property in obj.GetType().GetProperties().Where(t => t.CanWrite && t.CanRead))
                {
                    object value;
                    if (!_PropertyGetLambda.ContainsKey(property))
                    {
                        var parameterExpression = Expression.Parameter(typeof(object));
                        var convertExpression = Expression.Convert(parameterExpression, type);
                        var propertyExpression = Expression.Property(convertExpression, property);
                        convertExpression = Expression.Convert(propertyExpression, typeof(object));
                        var lambda = Expression.Lambda<Func<object, object>>(convertExpression, parameterExpression).Compile();
                        _PropertyGetLambda.Add(property, lambda);
                        value = lambda(obj);
                    }
                    else
                    {
                        value = ((Func<object, object>)_PropertyGetLambda[property])(obj);
                    }
                    //object value = property.GetValue(obj);
                    if (value == null)
                    {
                        index++;
                        continue;
                    }
                    stream.WriteByte((byte)index);
                    SerializeValue(stream, property.PropertyType, value);
                    count++;
                    index++;
                }
                long position2 = stream.Position;
                stream.Position = position;
                stream.WriteByte(count);
                stream.Position = position2;
            }
        }

        /// <summary>
        /// Deserialize object.
        /// </summary>
        /// <param name="stream">Data stream.</param>
        /// <param name="type">Type of object.</param>
        /// <returns></returns>
        protected virtual object DeserializeObject(Stream stream, Type type)
        {
            if (typeof(IList).IsAssignableFrom(type))
            {
                //var targetType = GetType((byte)stream.ReadByte());
                //if (targetType.GetConstructor(new Type[0]) == null)
                //    throw new NotSupportedException("Type of \"" + targetType.FullName + "\" not support.");
                //if (type.GetConstructor(new Type[0]) == null)
                //    throw new NotSupportedException("Type of \"" + type.FullName + "\" not support.");
                var ilist = type.GetInterface("IList`1");
                if (ilist == null)
                    throw new NotSupportedException("Type of \"" + type.FullName + "\" not support.");
                var valueType = ilist.GetGenericArguments()[0];
                IList list = (IList)Activator.CreateInstance(type);
                var lenghtData = new byte[sizeof(int)];
                stream.Read(lenghtData, 0, lenghtData.Length);
                var length = BitConverter.ToInt32(lenghtData, 0);
                for (int i = 0; i < length; i++)
                {
                    if (stream.ReadByte() == 0)
                    {
                        list.Add(null);
                        continue;
                    }
                    //byte targetTypeIndex = (byte)stream.ReadByte();
                    //if (targetTypeIndex == 0)
                    //{
                    //    list.Add(null);
                    //    continue;
                    //}
                    //targetType = GetType(targetTypeIndex);
                    list.Add(DeserializeValue(stream, valueType));
                }
                return list;
            }
            else if (type.IsSubclassOf(typeof(Array)))
            {
                int rank = stream.ReadByte();
                List<object> parameters = new List<object>();
                for (int i = 0; i < rank; i++)
                {
                    var minData = new byte[sizeof(int)];
                    var maxData = new byte[sizeof(int)];
                    stream.Read(minData, 0, minData.Length);
                    stream.Read(maxData, 0, maxData.Length);
                    var min = BitConverter.ToInt32(minData, 0);
                    var max = BitConverter.ToInt32(maxData, 0);
                    parameters.Add(min);
                    parameters.Add(max);
                }
                Array array = (Array)Activator.CreateInstance(type, parameters.ToArray());
                DeserializeArray(stream, array, new int[0]);
                return array;
            }
            else
            {
                //type = GetType((byte)stream.ReadByte());
                object item = Activator.CreateInstance(type);
                int count = stream.ReadByte();
                PropertyInfo[] properties;
                if (!_TypeProperty.ContainsKey(type))
                {
                    properties = type.GetProperties().Where(t => t.CanWrite && t.CanRead).ToArray();
                    _TypeProperty.Add(type, properties);
                }
                else
                    properties = (PropertyInfo[])_TypeProperty[type];
                for (int i = 0; i < count; i++)
                {
                    var index = stream.ReadByte();
                    var property = properties[index];
                    if (!_PropertySetLambda.ContainsKey(property))
                    {
                        var objParameter = Expression.Parameter(typeof(object));
                        var valueParameter = Expression.Parameter(typeof(object));
                        var objConverterParameter = Expression.Convert(objParameter, type);
                        var valueConverterParameter = Expression.Convert(valueParameter, property.PropertyType);
                        var expression = Expression.Call(objConverterParameter, property.GetSetMethod(), valueConverterParameter);
                        var lambda = Expression.Lambda<Action<object, object>>(expression, objParameter, valueParameter).Compile();
                        _PropertySetLambda.Add(property, lambda);
                        lambda(item, DeserializeValue(stream, property.PropertyType));
                    }
                    else
                    {
                        ((Action<object, object>)_PropertySetLambda[property])(item, DeserializeValue(stream, property.PropertyType));
                    }
                    //property.SetValue(item, DeserializeValue(stream, property.PropertyType));
                }
                return item;
            }
        }

        /// <summary>
        /// Serialize value.
        /// </summary>
        /// <param name="stream">Data stream.</param>
        /// <param name="type">Type of value.</param>
        /// <param name="value">Value to serialize.</param>
        protected virtual void SerializeValue(Stream stream, Type type, object value)
        {
            if (value is ValueType)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    value = type.GetProperty("Value").GetValue(value);
                if (value is bool)
                {
                    stream.WriteByte((bool)value ? (byte)1 : (byte)0);
                }
                else if (value is byte)
                {
                    stream.WriteByte((byte)value);
                }
                else if (value is sbyte)
                {
                    stream.WriteByte((byte)((sbyte)value + 127));
                }
                else if (value is short)
                {
                    var data = BitConverter.GetBytes((short)value);
                    stream.Write(data, 0, data.Length);
                }
                else if (value is ushort)
                {
                    var data = BitConverter.GetBytes((ushort)value);
                    stream.Write(data, 0, data.Length);
                }
                else if (value is int)
                {
                    var data = BitConverter.GetBytes((int)value);
                    stream.Write(data, 0, data.Length);
                }
                else if (value is uint)
                {
                    var data = BitConverter.GetBytes((uint)value);
                    stream.Write(data, 0, data.Length);
                }
                else if (value is long)
                {
                    var data = BitConverter.GetBytes((long)value);
                    stream.Write(data, 0, data.Length);
                }
                else if (value is ulong)
                {
                    var data = BitConverter.GetBytes((ulong)value);
                    stream.Write(data, 0, data.Length);
                }
                else if (value is float)
                {
                    var data = BitConverter.GetBytes((float)value);
                    stream.Write(data, 0, data.Length);
                }
                else if (value is double)
                {
                    var data = BitConverter.GetBytes((double)value);
                    stream.Write(data, 0, data.Length);
                }
                else if (value is decimal)
                {
                    var datas = decimal.GetBits((decimal)value);
                    stream.WriteByte((byte)datas.Length);
                    for (int i = 0; i < datas.Length; i++)
                    {
                        var data = BitConverter.GetBytes(datas[i]);
                        stream.Write(data, 0, data.Length);
                    }
                }
                else if (value is char)
                {
                    var data = BitConverter.GetBytes((char)value);
                    stream.Write(data, 0, data.Length);
                }
                else if (value is Guid)
                {
                    var data = ((Guid)value).ToByteArray();
                    stream.Write(data, 0, data.Length);
                }
                else if (value is DateTime)
                {
                    var data = BitConverter.GetBytes(((DateTime)value).Ticks);
                    stream.Write(data, 0, data.Length);
                }
                else if (value is DateTimeOffset)
                {
                    var data = ((DateTimeOffset)value);
                    SerializeValue(stream, typeof(DateTime), data.DateTime);
                    SerializeValue(stream, typeof(TimeSpan), data.Offset);
                }
                else if (value is TimeSpan)
                {
                    var data = BitConverter.GetBytes(((TimeSpan)value).Ticks);
                    stream.Write(data, 0, data.Length);
                }
                else
                {
                    throw new NotSupportedException("Type of \"" + type.Name + "\" not support.");
                }
            }
            else if (value is string)
            {
                var data = Encoding.UTF8.GetBytes((string)value);
                var length = BitConverter.GetBytes(data.Length);
                stream.Write(length, 0, length.Length);
                stream.Write(data, 0, data.Length);
            }
            else
            {
                SerializeObject(stream, type, value);
            }
        }

        /// <summary>
        /// Deserialize value.
        /// </summary>
        /// <param name="stream">Data stream.</param>
        /// <param name="type">Type of value.</param>
        /// <returns></returns>
        protected virtual object DeserializeValue(Stream stream, Type type)
        {
            if (type.IsValueType)
            {
                if (type == typeof(bool))
                {
                    return stream.ReadByte() == 1;
                }
                else if (type == typeof(byte))
                {
                    return stream.ReadByte();
                }
                else if (type == typeof(sbyte))
                {
                    return (sbyte)(stream.ReadByte() - 127);
                }
                else if (type == typeof(short))
                {
                    var data = new byte[sizeof(short)];
                    stream.Read(data, 0, data.Length);
                    return BitConverter.ToInt16(data, 0);
                }
                else if (type == typeof(ushort))
                {
                    var data = new byte[sizeof(ushort)];
                    stream.Read(data, 0, data.Length);
                    return BitConverter.ToUInt16(data, 0);
                }
                else if (type == typeof(int))
                {
                    var data = new byte[sizeof(int)];
                    stream.Read(data, 0, data.Length);
                    return BitConverter.ToInt32(data, 0);
                }
                else if (type == typeof(uint))
                {
                    var data = new byte[sizeof(uint)];
                    stream.Read(data, 0, data.Length);
                    return BitConverter.ToUInt32(data, 0);
                }
                else if (type == typeof(long))
                {
                    var data = new byte[sizeof(long)];
                    stream.Read(data, 0, data.Length);
                    return BitConverter.ToInt64(data, 0);
                }
                else if (type == typeof(ulong))
                {
                    var data = new byte[sizeof(ulong)];
                    stream.Read(data, 0, data.Length);
                    return BitConverter.ToUInt64(data, 0);
                }
                else if (type == typeof(float))
                {
                    var data = new byte[sizeof(float)];
                    stream.Read(data, 0, data.Length);
                    return BitConverter.ToSingle(data, 0);
                }
                else if (type == typeof(double))
                {
                    var data = new byte[sizeof(double)];
                    stream.Read(data, 0, data.Length);
                    return BitConverter.ToDouble(data, 0);
                }
                else if (type == typeof(decimal))
                {
                    var length = stream.ReadByte();
                    var d = new int[length];
                    for (int i = 0; i < length; i++)
                    {
                        var data = new byte[sizeof(int)];
                        stream.Read(data, 0, data.Length);
                        d[i] = BitConverter.ToInt32(data, 0);
                    }
                    return new decimal(d);
                }
                else if (type == typeof(char))
                {
                    var data = new byte[sizeof(char)];
                    stream.Read(data, 0, data.Length);
                    return BitConverter.ToChar(data, 0);
                }
                else if (type == typeof(Guid))
                {
                    var data = new byte[16];
                    stream.Read(data, 0, data.Length);
                    return new Guid(data);
                }
                else if (type == typeof(DateTime))
                {
                    var data = new byte[sizeof(long)];
                    stream.Read(data, 0, data.Length);
                    return new DateTime(BitConverter.ToInt64(data, 0));
                }
                else if (type == typeof(DateTimeOffset))
                {
                    var data = new byte[sizeof(long)];
                    stream.Read(data, 0, data.Length);
                    var datetime = new DateTime(BitConverter.ToInt64(data, 0));
                    stream.Read(data, 0, data.Length);
                    var timespan = new TimeSpan(BitConverter.ToInt64(data, 0));
                    return new DateTimeOffset(datetime, timespan);
                }
                else if (type == typeof(TimeSpan))
                {
                    var data = new byte[sizeof(long)];
                    stream.Read(data, 0, data.Length);
                    return TimeSpan.FromTicks(BitConverter.ToInt64(data, 0));
                }
                else
                {
                    throw new NotSupportedException("Type of \"" + type.Name + "\" not support.");
                }
            }
            else if (type == typeof(string))
            {
                var lengthData = new byte[sizeof(int)];
                stream.Read(lengthData, 0, lengthData.Length);
                var length = BitConverter.ToInt32(lengthData, 0);
                var data = new byte[length];
                stream.Read(data, 0, data.Length);
                return Encoding.UTF8.GetString(data);
            }
            else
            {
                return DeserializeObject(stream, type);
            }
        }

        /// <summary>
        /// Get type index.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <returns></returns>
        protected byte GetTypeIndex(Type type)
        {
            type = ConvertTypeToType(type);
            if (!_TypeStorage.Contains(type))
            {
                if (type.IsGenericType)
                    foreach (var argument in type.GetGenericArguments())
                    {
                        GetTypeIndex(argument);
                    }
                _TypeStorage.Add(type);
            }
            return (byte)_TypeStorage.IndexOf(type);
        }

        /// <summary>
        /// Get type from index.
        /// </summary>
        /// <param name="index">Index of type.</param>
        /// <returns></returns>
        protected Type GetType(int index)
        {
            return GetType((byte)index);
        }

        /// <summary>
        /// Get type from index.
        /// </summary>
        /// <param name="index">Index of type.</param>
        /// <returns></returns>
        protected Type GetType(byte index)
        {
            return _TypeStorage[index];
        }

        private void SerializeArray(Stream stream, Array array, int[] rank)
        {
            Type type = array.GetType().GetElementType();
            var min = array.GetLowerBound(rank.Length);
            var max = array.GetUpperBound(rank.Length);
            int[] target = new int[rank.Length + 1];
            rank.CopyTo(target, 0);
            for (int i = min; i <= max; i++)
            {
                if (array.Rank > target.Length)
                {
                    SerializeArray(stream, array, target);
                }
                else
                {
                    target[rank.Length] = i;
                    var value = array.GetValue(target);
                    SerializeValue(stream, type, value);
                }
            }
        }

        private void DeserializeArray(Stream stream, Array array, int[] rank)
        {
            Type type = array.GetType().GetElementType();
            var min = array.GetLowerBound(rank.Length);
            var max = array.GetUpperBound(rank.Length);
            int[] target = new int[rank.Length + 1];
            rank.CopyTo(target, 0);
            for (int i = min; i <= max; i++)
            {
                if (array.Rank > target.Length)
                {
                    DeserializeArray(stream, array, target);
                }
                else
                {
                    target[rank.Length] = i;
                    array.SetValue(DeserializeValue(stream, type), target);
                }
            }
        }

        /// <summary>
        /// Convert type to type.
        /// </summary>
        /// <param name="type">Type to convert.</param>
        /// <returns></returns>
        protected virtual Type ConvertTypeToType(Type type)
        {
            return type;
        }

        /// <summary>
        /// Convert string to type.
        /// </summary>
        /// <param name="typeName">Name of type.</param>
        /// <returns></returns>
        protected virtual Type ConvertTypeFromString(string typeName)
        {
            return Type.GetType(typeName);
        }

        /// <summary>
        /// Convert type to string.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <returns></returns>
        protected virtual string ConvertTypeToString(Type type)
        {
            return type.AssemblyQualifiedName;
        }
    }
}
