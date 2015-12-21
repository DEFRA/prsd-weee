namespace EA.Weee.Xml.Converter
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;
    using System.Xml.Serialization;

    /// <summary>
    /// Traverses an object graph to find all string properties with attributes identifying them with the XML
    /// data type of "Token". Any such properties are tokenised according to the rules defined by the XML 
    /// standard for collapsing whitespace.
    /// </summary>
    internal class WhiteSpaceCollapser : IWhiteSpaceCollapser
    {
        public void Collapse(object @object)
        {
            List<object> processedObjects = new List<object>();

            Process(@object, processedObjects);
        }

        private void Process(object @object, List<object> processedObjects)
        {
            if (@object == null)
            {
                // Don't process null objects.
                return;
            }

            if (!@object.GetType().IsDefined(typeof(XmlTypeAttribute)))
            {
                // Only process objects which are mapped to XML types.
                return;
            }

            if (processedObjects.Contains(@object))
            {
                // Don't process object's we're already processed, otherwise we could get a cyclic dependency.
                return;
            }

            processedObjects.Add(@object);

            foreach (PropertyInfo property in @object.GetType().GetProperties())
            {
                if (property.PropertyType == typeof(string))
                {
                    ProcessStringProperty(@object, property);
                }
                else if (property.PropertyType == typeof(string[]))
                {
                    ProcessArrayOfStringProperty(@object, property);
                }
                else if (property.PropertyType.IsArray)
                {
                    Array values = (Array)property.GetValue(@object);
                    foreach (object value in values)
                    {
                        Process(value, processedObjects);
                    }
                }
                else
                {
                    Process(property.GetValue(@object), processedObjects);
                }
            }
        }

        private void ProcessArrayOfStringProperty(object @object, PropertyInfo property)
        {
            IEnumerable<XmlArrayItemAttribute> xmlArrayItemAttributes = property.GetCustomAttributes<XmlArrayItemAttribute>();
            foreach (XmlArrayItemAttribute xmlArrayItemAttribute in xmlArrayItemAttributes)
            {
                if (xmlArrayItemAttribute != null)
                {
                    if (xmlArrayItemAttribute.DataType == "token")
                    {
                        string[] values = (string[])property.GetValue(@object);
                        if (values != null)
                        {
                            for (int index = 0; index < values.Length; ++index)
                            {
                                string value = values[index];
                                value = CollapseWhiteSpace(value);
                                values[index] = value;
                            }
                            property.SetValue(@object, values);
                        }
                    }
                }
            }
        }

        private void ProcessStringProperty(object @object, PropertyInfo property)
        {
            IEnumerable<XmlElementAttribute> xmlElementAttributes = property.GetCustomAttributes<XmlElementAttribute>();
            foreach (XmlElementAttribute xmlElementAttribute in xmlElementAttributes)
            {
                if (xmlElementAttribute != null)
                {
                    if (xmlElementAttribute.DataType == "token")
                    {
                        string value = (string)property.GetValue(@object);
                        value = CollapseWhiteSpace(value);
                        property.SetValue(@object, value);
                    }
                }
            }
        }

        /// <summary>
        /// See: https://en.wikipedia.org/wiki/Whitespace_character
        /// </summary>
        private readonly List<char> whiteSpaceChars = new List<char>()
        {
            (char)9,
            (char)10,
            (char)11,
            (char)12,
            (char)13,
            (char)32,
            (char)133,
            (char)160,
            (char)8192,
            (char)8193,
            (char)8194,
            (char)8195,
            (char)8196,
            (char)8197,
            (char)8198,
            (char)8199,
            (char)8200,
            (char)8201,
            (char)8202,
            (char)8232,
            (char)8233,
            (char)8239,
            (char)8287,
            (char)12288,
            (char)6158,
            (char)8203,
            (char)8204,
            (char)8205,
            (char)8288,
            (char)65279,
        };

        /// <summary>
        /// This method performs the same processing as is done by the XML validator for a value
        /// with the restriction whiteSpace="collapse".
        /// It replaces all white space characters with spaces, then removes contiguous runs of spaces
        /// and any leading or trailing spaces.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string CollapseWhiteSpace(string value)
        {
            if (value == null)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();

            bool previousCharacterWasWhiteSpace = false;
            foreach (char c in value)
            {
                if (whiteSpaceChars.Contains(c))
                {
                    if (!previousCharacterWasWhiteSpace)
                    {
                        sb.Append(' ');
                        previousCharacterWasWhiteSpace = true;
                    }
                }
                else
                {
                    sb.Append(c);
                    previousCharacterWasWhiteSpace = false;
                }
            }

            return sb.ToString().Trim();
        }
    }
}
