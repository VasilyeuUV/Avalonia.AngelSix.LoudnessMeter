namespace test.Interfaces;

public static class InterfaceImplementer
{
    public static T CreateInstance<T>(object values) where T : class
    {
        var sourceType = values.GetType();
        var targetType = typeof(T);
        var constructor = targetType.GetConstructors().First();
        var parameters = constructor.GetParameters()
                                    .Select(p => sourceType.GetProperty(p.Name).GetValue(values))
                                    .ToArray();
        return (T)constructor.Invoke(parameters);
    }
}
