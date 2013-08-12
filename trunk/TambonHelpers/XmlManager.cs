using System;
using System.IO;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace De.AHoerstemeier.Tambon
{
    /// <summary>
    /// Serializes and deserializes an instance of an entity to XML using either XmlSerializer
    /// </summary>
    public static class XmlManager
    {
        /// <summary>
        /// Serializes an Entity to Xml using the <see cref="System.Xml.Serialization.XmlSerializer"/>.
        /// </summary>
        /// <typeparam name="T">Type of the Entity.</typeparam>
        /// <param name="entity">the Entity to serialize.</param>
        /// <param name="extraTypes">A Type array of additional object types to serialize. <see cref="System.Type"/></param>
        /// <returns>the Xml of the Entity as <see cref="System.String"/>.</returns>
        public static String EntityToXml<T>(T entity, params Type[] extraTypes)
        {
            MemoryStream stream = null;
            TextWriter writer = null;
            try
            {
                stream = new MemoryStream(); // read xml in memory
                writer = new StreamWriter(stream, Encoding.UTF8);
                // get serialise object
                XmlSerializer serializer = new XmlSerializer(typeof(T), extraTypes);
                serializer.Serialize(writer, entity); // read object
                var count = (Int32)stream.Length; // saves object in memory stream
                Byte[] arr = new Byte[count];
                stream.Seek(0, SeekOrigin.Begin);
                // copy stream contents in byte array
                stream.Read(arr, 0, count);
                UTF8Encoding utf = new UTF8Encoding(); // convert byte array to string
                //return utf.GetString(arr).Trim().Replace("\n", "").Replace("\r", "");
                return utf.GetString(arr);
            }
            catch
            {
                return String.Empty;
            }
            finally
            {
                if ( writer != null )
                    writer.Close();  // Also closes the underlying stream!
                //if ( stream != null )
                //    stream.Dispose();
            }
        }

        /// <summary>
        /// Deserialize an Xml stream to the corresponding Entity T using the <see cref="System.Xml.Serialization.XmlSerializer"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the <paramref name="xmlStream"/> can not be deserialized to a valid object of type <typeparamref name="T"/>.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="xmlStream"/> or <paramref name="serializer"/> is null.</exception>
        /// <typeparam name="T">Type of the Entity</typeparam>
        /// <param name="xmlStream">The xml <see cref="System.IO.Stream"/> to deserialize</param>
        /// <param name="serializer">The XmlSerializer that is being used for deserialization.</param>
        /// <returns>The Entity of Type T</returns>
        public static T XmlToEntity<T>(Stream xmlStream, XmlSerializer serializer)
        {
            if ( xmlStream == null )
            {
                throw new ArgumentNullException("xmlStream");
            }
            if ( serializer == null )
            {
                throw new ArgumentNullException("serializer");
            }

            XmlTextReader reader = null;
            try
            {
                // serialise to object
                //XmlSerializer serializer = new XmlSerializer(typeof(T), extraTypes);
                reader = new XmlTextReader(xmlStream); // create reader
                // convert reader to object
                return (T)serializer.Deserialize(reader);
            }
            finally
            {
                if ( reader != null )
                    reader.Close();
            }
        }
    }
}