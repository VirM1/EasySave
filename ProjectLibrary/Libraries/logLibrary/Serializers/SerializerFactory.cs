using System.Linq;
using System;
using System.Collections.Generic;

namespace ProjectLibrary.LogLibrary.Serializers
{
    class SerializerFactory
    {
        private static readonly IEnumerable<ISerializer> _availableSerializers = new ISerializer[]
                {
            new XmlLogSerializer(),
            new JsonLogSerializer(),
        };

        public static ISerializer GetSerializer(LogExtension extension)
        {
            var result = _availableSerializers.FirstOrDefault(x => x.LogExtension == extension);
            return result ?? new JsonLogSerializer();
        }
    }
}
