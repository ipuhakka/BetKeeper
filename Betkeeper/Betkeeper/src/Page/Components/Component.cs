﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Betkeeper.Page.Components
{
    public enum ComponentType
    {
        Button,

        /// <summary>
        /// Field not containing preliminary data
        /// </summary>
        Field,

        Table,

        Container,

        Tab,

        /// <summary>
        /// Label without attached field
        /// </summary>
        Label
    }

    [Serializable]
    public class Component
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ComponentType ComponentType { get; }

        public string ComponentKey { get; set; }

        public Component(ComponentType componentType, string componentKey = null)
        {
            ComponentType = componentType;
            ComponentKey = componentKey;
        }

        /// <summary>
        /// Clones a component.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toClone"></param>
        /// <returns></returns>
        public static T CloneComponent<T>(T toClone)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, toClone);

                stream.Seek(0, SeekOrigin.Begin);

                return (T)formatter.Deserialize(stream);
            }
        }
    }
}
