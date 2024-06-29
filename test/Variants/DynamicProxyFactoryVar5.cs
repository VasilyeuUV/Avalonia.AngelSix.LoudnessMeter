using ImpromptuInterface;
using test.Interfaces;
using test.Models;

public static class DynamicProxyFactoryVar5
{
    internal static ICurrentControl CreateProxy(CommonControl source)
    {
        // Create dynamic proxy with ImpromptuInterface
        var proxy = new
        {
            Key = source.Key,
            Type = source.Type
        };

        return Impromptu.ActLike<ICurrentControl>(proxy);
    }
}
