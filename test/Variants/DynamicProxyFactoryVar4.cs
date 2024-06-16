using ImpromptuInterface;
using test.Interfaces;
using test.Models;

namespace test.Variants
{
    //public static class DynamicProxyFactory
    //{
    //    internal static ICurrentControl CreateProxy(CommonControl source)
    //    {
    //        // Map CommonControl to a dynamic object
    //        dynamic proxy = new ImpromptuDictionary();

    //        // Assign properties from CommonControl
    //        proxy.Key = source.Key;
    //        proxy.Type = source.Type;

    //        // Return the dynamic proxy as ICurrentControl
    //        return Impromptu.ActLike<ICurrentControl>(proxy);
    //    }
    //}

    public static class DynamicProxyFactory
    {
        internal static ICurrentControl CreateProxy(CommonControl source)
        {
            // Use Impromptu to create a dynamic implementation of ICurrentControl
            var proxy = new
            {
                Key = source.Key,
                Type = source.Type
            };

            return Impromptu.ActLike<ICurrentControl>(proxy);
        }
    }
}
