using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc.Converter;

namespace System.Web.Mvc
{
    public static class EntityValueConverter
    {
        private static Dictionary<Type, TypeConverter> _Items;

        static EntityValueConverter()
        {
            _Items = new Dictionary<Type, TypeConverter>();
            AddConverter<Boolean>(new BooleanConverter());
            AddConverter<Byte>(new ByteConverter());
            AddConverter<DateTime>(new DateTimeConverter());
            AddConverter<Decimal>(new DecimalConverter());
            AddConverter<Double>(new DoubleConverter());
            AddConverter<IEntity>(new EntityConverter());
            AddConverter<Enum>(new System.Web.Mvc.Converter.EnumConverter());
            AddConverter<Int16>(new Int16Converter());
            AddConverter<Int32>(new Int32Converter());
            AddConverter<Int64>(new Int64Converter());
            AddConverter<SByte>(new SByteConverter());
            AddConverter<Single>(new SingleConverter());
            AddConverter<String>(new StringConverter());
            AddConverter<TimeSpan>(new TimeSpanConverter());
            AddConverter<UInt16>(new UInt16Converter());
            AddConverter<UInt32>(new UInt32Converter());
            AddConverter<UInt64>(new UInt64Converter());
        }

        public static void AddConverter<T>(TypeConverter converter)
        {
            if (converter == null)
                throw new ArgumentNullException("converter");
            AddConverter(typeof(T), converter);
        }

        public static void AddConverter(Type type, TypeConverter converter)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (converter == null)
                throw new ArgumentNullException("converter");
            if (!converter.CanConvertFrom(typeof(string)))
                throw new ArgumentException("Can not converter value from string.");
            if (_Items.ContainsKey(type))
                _Items[type] = converter;
            else
                _Items.Add(type, converter);
        }

        public static void RemoveConverter<T>()
        {
            RemoveConverter(typeof(T));
        }

        public static void RemoveConverter(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (_Items.ContainsKey(type))
                _Items.Remove(type);
        }

        public static TypeConverter GetConverter<T>()
        {
            return GetConverter(typeof(T));
        }

        public static TypeConverter GetConverter(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            foreach (var t in type.GetInterfaces())
            {
                if (_Items.ContainsKey(t))
                    return _Items[t];
                if (t.IsGenericType)
                    if (_Items.ContainsKey(t.GetGenericTypeDefinition()))
                        return _Items[t.GetGenericTypeDefinition()];
            }
            while (!_Items.ContainsKey(type))
            {
                if (type.IsAbstract || type == typeof(object))
                    return null;
                type = type.BaseType;
                if (type.IsGenericType)
                    if (_Items.ContainsKey(type.GetGenericTypeDefinition()))
                        return _Items[type.GetGenericTypeDefinition()];
            }
            return _Items[type];
        }
    }
}
