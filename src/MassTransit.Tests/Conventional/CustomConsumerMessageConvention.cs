namespace MassTransit.Tests.Conventional
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Metadata;


    class CustomConsumerMessageConvention<T> :
        IConsumerMessageConvention
        where T : class
    {
        public IEnumerable<IMessageInterfaceType> GetMessageTypes()
        {
            var typeInfo = typeof(T).GetTypeInfo();
            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(IHandler<>))
            {
                var interfaceType = new CustomConsumerInterfaceType(typeInfo.GetGenericArguments()[0], typeof(T));
                if (TypeMetadataCache.IsValidMessageType(interfaceType.MessageType))
                    yield return interfaceType;
            }

            IEnumerable<CustomConsumerInterfaceType> types = typeof(T).GetInterfaces()
                .Where(x => x.GetTypeInfo().IsGenericType)
                .Where(x => x.GetTypeInfo().GetGenericTypeDefinition() == typeof(IHandler<>))
                .Select(x => new CustomConsumerInterfaceType(x.GetTypeInfo().GetGenericArguments()[0], typeof(T)))
                .Where(x => TypeMetadataCache.IsValidMessageType(x.MessageType));

            foreach (CustomConsumerInterfaceType type in types)
                yield return type;
        }
    }
}
