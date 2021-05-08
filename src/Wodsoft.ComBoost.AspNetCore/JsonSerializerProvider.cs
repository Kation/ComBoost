﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.AspNetCore
{
    /// <summary>
    /// Json序列化提供器。
    /// </summary>
    public class JsonSerializerProvider : ISerializerProvider
    {
        public JsonSerializerProvider() : this(new System.Text.Json.JsonSerializerOptions()) { }

        public JsonSerializerProvider(System.Text.Json.JsonSerializerOptions settings)
        {
            Settings = settings;
        }

        /// <summary>
        /// 获取Json序列化设置。
        /// </summary>
        public System.Text.Json.JsonSerializerOptions Settings { get; private set; }

        /// <summary>
        /// 获取序列化器。
        /// </summary>
        /// <param name="type">类型。</param>
        /// <returns>返回序列化器。</returns>
        public ISerializer GetSerializer(Type type)
        {
            return new JsonSerializer(type, Settings);
        }
    }

    /// <summary>
    /// Json序列化器。
    /// </summary>
    public class JsonSerializer : ISerializer
    {
        public JsonSerializer(Type type, System.Text.Json.JsonSerializerOptions settings)
        {
            Type = type;
            Settings = settings;
        }

        /// <summary>
        /// 获取类型。
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// 获取Json序列化设置。
        /// </summary>
        public System.Text.Json.JsonSerializerOptions Settings { get; private set; }

        /// <summary>
        /// 反序列化。
        /// </summary>
        /// <param name="stream">流。</param>
        /// <returns>返回反序列化后的对象。</returns>
        public object Deserialize(Stream stream)
        {
            var reader = new StreamReader(stream);
            var value = reader.ReadToEnd();
            return System.Text.Json.JsonSerializer.Deserialize(value, Type);
        }

        /// <summary>
        /// 序列化。
        /// </summary>
        /// <param name="stream">流。</param>
        /// <param name="value">要序列化的对象。</param>
        public void Serialize(Stream stream, object value)
        {
            var text = System.Text.Json.JsonSerializer.Serialize(value, Type, Settings);
            var writer = new StreamWriter(stream);
            writer.Write(text);
            writer.Close();
        }
    }
}
