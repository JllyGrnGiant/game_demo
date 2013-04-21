using System.Collections.Generic;
using System.Xml;
using System;
using System.Reflection;

namespace Innovation
{
    public class Serializer
    {
        // Keeps track of what Assembly Types are located in. The Type is the Key and the
        // Assembly is the value
        public Dictionary<string, string> DependencyMap = new Dictionary<string, string>();

        // Uses dependency map to write dependencies to xml file
        public void WriteDependencies(XmlWriter Writer)
        {
            Writer.WriteStartElement("Dependencies");

            // Temp list used to keep track of dependencies we need to write
            List<string> assemblies = new List<string>();

            // Foreach Type, add its Assembly to the assembly list if it hasn't been added
            foreach (string assembly in DependencyMap.Values)
                if (!assemblies.Contains(assembly))
                    assemblies.Add(assembly);

            // Write each assembly to the file
            foreach (string assembly in assemblies)
            {
                Writer.WriteStartElement("Assembly");
                Writer.WriteAttributeString("Name", assembly);

                // Write each type in the Assembly as a child node
                foreach (string type in DependencyMap.Keys)
                {
                    if (DependencyMap[type] == assembly)
                    {
                        Writer.WriteStartElement("Type");
                        Writer.WriteAttributeString("Name", type);
                        Writer.WriteEndElement();
                    }
                }

                Writer.WriteEndElement();
            }
        }

        // Add type to the dependency map
        public void Dependency(Type type)
        {
            string name = type.FullName;
            string assembly = type.Assembly.FullName;

            if (!DependencyMap.ContainsKey(name))
                DependencyMap.Add(name, assembly);
        }

        // Load back the assemblies used from file
        public void PopulateAssemblies(XmlNode DependenciesRoot)
        {
            // For each node, add the type to the list.
            // Attribute 0 = type name, 1 = assembly name
            foreach (XmlNode Node in DependenciesRoot.ChildNodes)
                foreach (XmlNode child in Node.ChildNodes)
                    DependencyMap.Add(child.Attributes[0].Value, Node.Attributes[0].Value);
        }

        public void ClearDependencies()
        {
            DependencyMap.Clear();
        }

        public void WriteGameScreens(XmlWriter Writer)
        {
            Writer.WriteStartElement("GameScreens");

            foreach (GameScreen screen in Engine.GameScreens)
            {
                // The background screen is auto created. No need to serialize it
                if (screen != Engine.BackgroundScreen)
                {
                    Writer.WriteStartElement("GameScreen");
                    Writer.WriteAttributeString("Name", screen.Name);
                    Writer.WriteAttributeString("Type", screen.GetType().FullName);

                    Dependency(screen.GetType());

                    if (screen.BlocksInput) { Writer.WriteElementString("BlocksInput", null); }
                    if (screen.OverrideInputBlocked) { Writer.WriteElementString("OverrideInputBlocked", null); }
                    if (screen.BlocksUpdate) { Writer.WriteElementString("BlocksUpdate", null); }
                    if (screen.OverrideUpdateBlocked) { Writer.WriteElementString("OverrideUpdateBlocked", null); }
                    if (screen.BlocksDraw) { Writer.WriteElementString("BlocksDraw", null); }
                    if (screen.OverrideDrawBlocked) { Writer.WriteElementString("OverrideDrawBlocked", null); }
                    if (screen == Engine.DefaultScreen) { Writer.WriteElementString("DefaultScreen", null); }

                    Writer.WriteEndElement();
                }
            }

            Writer.WriteEndElement();
        }

        public void SerializeObject(XmlWriter Writer, object Input, string InstanceName)
        {
            if (Input == null)
                return;

            if (InstanceName != null)
            {
                Writer.WriteStartElement("Field");
                Writer.WriteAttributeString("Name", InstanceName);
            }

            Type t = Input.GetType();

            // Add type's assembly to dependencies if necessary
            Dependency(t);

            // If we have a value type, we can save it directly
            if (t == typeof(short) ||
                t == typeof(long) ||
                t == typeof(float) ||
                t == typeof(decimal) ||
                t == typeof(double) ||
                t == typeof(ulong) ||
                t == typeof(uint) ||
                t == typeof(ushort) ||
                t == typeof(sbyte) ||
                t == typeof(int) ||
                t == typeof(byte) ||
                t == typeof(char) ||
                t == typeof(string) ||
                t == typeof(bool))
            {
                // Write type of value then value
                Writer.WriteAttributeString("Type", t.FullName);
                Writer.WriteValue(Input.ToString());
            }
            else
            {
                // If it's not a value type, we need to break it down
                Writer.WriteStartElement(Input.GetType().FullName);
                
                // Serialize all fields recursively
                foreach (FieldInfo info in Input.GetType().GetFields())
                    SerializeObject(Writer, info.GetValue(Input), info.Name);

                Writer.WriteEndElement();
            }

            // Write the end if this is not a one line value
            if (InstanceName != null)
                Writer.WriteEndElement();
        }

        // Deserialize component defined in the ComponentNode
        public Component Deserialize(XmlNode ComponentNode)
        {
            // Find type from component nodes name
            Assembly a = Assembly.Load(DependencyMap[ComponentNode.LocalName]);
            Type t = a.GetType(ComponentNode.LocalName);

            // Create an instance of the type and a SerializationData object
            Component component = (Component)Activator.CreateInstance(t);
            SerializationData data = new SerializationData(this, null);

            // For each field defined, get its value and add the field to SerializationData
            foreach (XmlNode child in ComponentNode.ChildNodes)
            {
                // Make name and object values, and get
                // the name from the 0 attribute.
                string name = child.Attributes[0].Value;
                object value = null;

                // If the field node contains text only, it's a value type
                // and we can set the object directly
                if (child.ChildNodes[0].NodeType == XmlNodeType.Text)
                    value = parse(child);
                // Otherwise we need to recreate a more complex object from the data
                else if (child.ChildNodes[0].NodeType == XmlNodeType.Element)
                    value = parseTree(child.FirstChild);

                // Save the field to the SerializationData
                data.AddData(name, value);
            }

            // Tell the component to load from the data
            component.RecieveSerializationData(data);

            return component;
        }

        // Returns an object from an XmlNode that contains a value type
        object parse(XmlNode value)
        {
            // Get type being parsed
            Assembly a = Assembly.Load(DependencyMap[value.Attributes["Type"].InnerText]);
            Type t = a.GetType(value.Attributes["Type"].InnerText);

            // If it is a string, we can return it how it is
            if (t == typeof(string))
                return value.InnerText;

            // Otherwise, it can be parsed using the "Parse()" method all value
            // typese have, invoked using reflection
            MethodInfo m = t.GetMethod("Parse", new Type[] { typeof(string) });

            // Return the value "Parse()" returns, using the node text as the argument
            return m.Invoke(null, new object[] { value.InnerText });
        }

        // Returns an object constructed from a tree of XmlNodes
        object parseTree(XmlNode root)
        {
            // Get type to be built
            Assembly a = Assembly.Load(DependencyMap[root.Name]);
            Type t = a.GetType(root.Name);

            // Create an instance of the type
            object instance = Activator.CreateInstance(t);

            // For each field in the node's children
            foreach (XmlNode member in root.ChildNodes)
            {
                // Get the info
                FieldInfo fInfo = t.GetField(member.Attributes["Name"].Value);

                // If the node contains a value type, set the value directly
                if (member.ChildNodes[0].NodeType == XmlNodeType.Text)
                    fInfo.SetValue(instance, parse(member));
                // Otherwise we need to parse it as a tree
                else
                    fInfo.SetValue(instance, parseTree(member));
            }

            return instance;
        }

        // Serialize the SerializationData to file
        public void Serialize(XmlWriter Writer, SerializationData Input)
        {
            foreach (KeyValuePair<string, object> pair in Input.Data)
            {
                Writer.WriteStartElement("Field");
                Writer.WriteAttributeString("Name", pair.Key);

                // We won't have an instance name here because
                // it is already in the Name attribute
                SerializeObject(Writer, pair.Value, null);

                Writer.WriteEndElement();
            }
        }
    }
}