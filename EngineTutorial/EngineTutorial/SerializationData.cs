using System.Collections.Generic;
using System.Xml;
using System;
using System.Reflection;

namespace Innovation
{
    // Info about Services
    struct ServiceData
    {
        public string Type;
        public bool IsService;
    }

    // Provides link between Component and Serializer classes. A component
    // adds data and keys to this class to simplify serialization
    public class SerializationData
    {
        // Stores data
        // Public so the Serializer can save the data it holds
        public Dictionary<string, object> Data = new Dictionary<string, object>();

        Serializer serializer;
        XmlWriter writer;

        public void AddData(string Key, object Data)
        {
            this.Data.Add(Key, Data);
        }

        public T GetData<T>(string Key)
        {
            object val = this.Data[Key];
            return (T)val;
        }

        public bool ContainsData(string Key)
        {
            return Data.ContainsKey(Key);
        }

        public XmlWriter Writer
        {
            get { return writer; }
        }

        public SerializationData(Serializer Serializer, XmlWriter Writer)
        {
            serializer = Serializer;
            writer = Writer;
        }

        // Tell serializer we will be using the specified type
        public void AddDependency(Type type)
        {
            serializer.Dependency(type);
        }

        // Get the Type specified through the serializer, so the
        // right assembly will be used.
        public Type GetTypeFromDependency(string type)
        {
            return Assembly.Load(serializer.DependencyMap[type]).GetType(type);
        }
    }
}