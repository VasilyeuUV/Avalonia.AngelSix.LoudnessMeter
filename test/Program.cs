

using System.Net.NetworkInformation;
using System.Runtime.Intrinsics.X86;
using System;
using AutoMapper;
using Castle.DynamicProxy;
using Newtonsoft.Json;
using test.Interfaces;
using test.Models;
using test.Variants;
using System.Reflection;
using System.Reflection.Emit;
using System.Diagnostics.Metrics;

internal class Program
{
    private static void Main(string[] args)
    {
        //// - Автогенерация классов
        //DynamicClassCreatorVar1.StartDynamicCreateCurrentClassByInterface();
        //DynamicClassCreatorVar2.StartVar2();
        //DynamicClassCreatorVar3.StartVar3();


        //string json = @"{
        //    ""Key"": 1,
        //    ""Description"": ""Sample description"",
        //    ""Type"": ""Sample type"",
        //    ""Height"": 10.5
        //}";

        //CommonControl commonControl = JsonConvert.DeserializeObject<CommonControl>(json);

        //// Initialize AutoMapper with the profile
        //var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        //var mapper = config.CreateMapper();

        //// Map CommonControl to an anonymous type
        //var anonType = new
        //{
        //    Key = (int?)null,
        //    Type = (string)null
        //};
        //var mappedObject = mapper.Map(commonControl, anonType);

        //// Create an instance of ICurrentControl using the mapped object
        //ICurrentControl currentControl = InterfaceImplementer.CreateInstance<ICurrentControl>(mappedObject);

        //// Output the mapped object to verify
        //Console.WriteLine($"Key: {currentControl.Key}, Type: {currentControl.Type}");


        //StartVar5();
        //StartVar6();


        ////##################################################################################################################
        //// ВАРИАНТ 1
        //// Configure AutoMapper
        //var config = new MapperConfiguration(cfg =>
        //{
        //    cfg.CreateMap<MyClass, IMyInterface>()
        //        .ForMember(dest => dest.Property1, opt => opt.MapFrom(src => src.Property1))
        //        .ForMember(dest => dest.Property2, opt => opt.MapFrom(src => src.Property2))
        //        .ForMember(dest => dest.Property3, opt => opt.MapFrom(src => src.Property3))
        //        ;
        //});

        //IMapper mapper = config.CreateMapper();

        //// Usage
        //MyClass sourceObject = new MyClass { Property1 = "Value1", Property2 = 42 };
        //IMyInterface destination = mapper.Map<IMyInterface>(sourceObject);


        ////##################################################################################################################
        //// ВАРИАНТ 2
        //// Configure AutoMapper
        //var config = new MapperConfiguration(cfg =>
        //{
        //    cfg.CreateMap<MyClass, IMyInterface>()
        //        .ConstructUsing(src => new MyInterfaceImpl { Property1 = src.Property1, Property2 = src.Property2 });
        //});

        //IMapper mapper = config.CreateMapper();

        //// Usage
        //MyClass sourceObject = new MyClass { Property1 = "Value1", Property2 = 42 };
        //IMyInterface destination = mapper.Map<IMyInterface>(sourceObject);


        ////##################################################################################################################
        //// ВАРИАНТ 3
        //// Configure AutoMapper
        //var config = new MapperConfiguration(cfg =>
        //{
        //    cfg.CreateMap<MyClass, object>().ConvertUsing(new MyClassToInterfaceConverter());
        //});

        //IMapper mapper = config.CreateMapper();

        //// Usage with different interfaces
        //MyClass sourceObject = new MyClass { Property1 = "Value1", Property2 = 42, DateProperty = DateTime.Now };

        //IInterface1 destination1 = mapper.Map<IInterface1>(sourceObject);
        //Console.WriteLine($"IInterface1 - Property1: {destination1.Property1}");

        //IInterface2 destination2 = mapper.Map<IInterface2>(sourceObject);
        //Console.WriteLine($"IInterface2 - Property2: {destination2.Property2}");



        ////##################################################################################################################
        //// ВАРИАНТ 4
        //// Configure AutoMapper
        //var config = new MapperConfiguration(cfg =>
        //{
        //    cfg.CreateMap<MyClass, object>().ConvertUsing(new MyClassToInterfaceConverter());
        //});

        //IMapper mapper = config.CreateMapper();

        //// Usage with different interfaces
        //MyClass sourceObject = new MyClass { Property1 = "Value1", Property2 = 42, DateProperty = DateTime.Now };

        //IInterface1 destination1 = mapper.Map<IInterface1>(sourceObject);
        //Console.WriteLine($"IInterface1 - Property1: {destination1.Property1}");

        //IInterface2 destination2 = mapper.Map<IInterface2>(sourceObject);
        //Console.WriteLine($"IInterface2 - Property2: {destination2.Property2}");


        ////##################################################################################################################
        //// ВАРИАНТ 5
        //// Configure AutoMapper
        //var config = new MapperConfiguration(cfg =>
        //{
        //    // Configure mapping for IInterface1
        //    cfg.CreateMap<MyClass, IInterface1>()
        //       .ForMember(dest => dest.Property1, opt => opt.MapFrom(src => src.Property1));

        //    // Configure mapping for IInterface2
        //    cfg.CreateMap<MyClass, IInterface2>()
        //       .ForMember(dest => dest.Property2, opt => opt.MapFrom(src => src.Property2));
        //});

        //IMapper mapper = config.CreateMapper();

        //// Usage with different interfaces
        //MyClass sourceObject = new MyClass { Property1 = "Value1", Property2 = 42, DateProperty = DateTime.Now };

        //IInterface1 destination1 = mapper.Map<IInterface1>(sourceObject);
        //Console.WriteLine($"IInterface1 - Property1: {destination1.Property1}");

        //IInterface2 destination2 = mapper.Map<IInterface2>(sourceObject);
        //Console.WriteLine($"IInterface2 - Property2: {destination2.Property2}");




        //##################################################################################################################
        // ВАРИАНТ 6
        // Configure AutoMapper
        // Configure AutoMapper
        var config = new MapperConfiguration(cfg =>
        {
            // Configure mapping for IInterface1
            cfg.CreateMap<MyClass, IInterface1>().ConstructUsing((src, context) => MapToInterface<IInterface1>(src));

            // Configure mapping for IInterface2
            cfg.CreateMap<MyClass, IInterface2>().ConstructUsing((src, context) => MapToInterface<IInterface2>(src));
        });

        IMapper mapper = config.CreateMapper();

        // Usage with different interfaces
        MyClass sourceObject = new MyClass { Property1 = "Value1", Property2 = 42, DateProperty = DateTime.Now };

        IInterface1 destination1 = mapper.Map<IInterface1>(sourceObject);
        Console.WriteLine($"IInterface1 - Property1: {destination1.Property1}");

        IInterface2 destination2 = mapper.Map<IInterface2>(sourceObject);
        Console.WriteLine($"IInterface2 - Property2: {destination2.Property2}");
    }


    // Helper method to dynamically map MyClass to interfaces
    private static TInterface MapToInterface<TInterface>(MyClass source)
    {
        Type dynamicType = CreateDynamicType<TInterface>();
        var dynamicObject = Activator.CreateInstance(dynamicType);

        PropertyInfo[] sourceProperties = typeof(MyClass).GetProperties();
        PropertyInfo[] dynamicProperties = dynamicType.GetProperties();

        foreach (var dynamicProp in dynamicProperties)
        {
            var sourceProp = Array.Find(sourceProperties, prop => prop.Name == dynamicProp.Name);
            if (sourceProp != null && dynamicProp.CanWrite)
            {
                dynamicProp.SetValue(dynamicObject, sourceProp.GetValue(source));
            }
        }

        return (TInterface)dynamicObject;
    }


    // Helper method to create a dynamic type that implements the given interface
    private static Type CreateDynamicType<TInterface>()
    {
        TypeBuilder typeBuilder = GetTypeBuilder<TInterface>();

        // Define a field in the dynamic type to store the class Type name
        FieldBuilder typeField = typeBuilder.DefineField("ClassName", typeof(string), FieldAttributes.Public);

        ConstructorBuilder constructor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { typeof(string) });
        ILGenerator constructorIL = constructor.GetILGenerator();
        constructorIL.Emit(OpCodes.Ldarg_0);
        constructorIL.Emit(OpCodes.Ldarg_1);
        constructorIL.Emit(OpCodes.Stfld, typeField);
        constructorIL.Emit(OpCodes.Ret);

        // Implement properties of the interface in the dynamic type
        foreach (var property in typeof(MyClass).GetProperties())
        {
            CreateProperty(typeBuilder, property.Name, property.PropertyType);
        }

        return typeBuilder.CreateType();
    }


    // Helper method to create a type builder for the given interface
    private static TypeBuilder GetTypeBuilder<TInterface>()
    {
        var typeSignature = "DynamicImplementation_" + typeof(TInterface).Name;
        var assemblyName = new AssemblyName(typeSignature);
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(Guid.NewGuid().ToString()), AssemblyBuilderAccess.Run);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
        var typeBuilder = moduleBuilder.DefineType(typeSignature,
                        TypeAttributes.Public |
                        TypeAttributes.Class |
                        TypeAttributes.AutoClass |
                        TypeAttributes.AnsiClass |
                        TypeAttributes.BeforeFieldInit |
                        TypeAttributes.AutoLayout,
                        null);

        typeBuilder.AddInterfaceImplementation(typeof(TInterface));

        return typeBuilder;
    }


    // Helper method to create a property in the dynamic type
    private static void CreateProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType)
    {
        var fieldBuilder = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

        var propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
        var getMethodBuilder = typeBuilder.DefineMethod("get_" + propertyName,
                            MethodAttributes.Public |
                            MethodAttributes.SpecialName |
                            MethodAttributes.HideBySig,
                            propertyType,
                            Type.EmptyTypes);
        var getIl = getMethodBuilder.GetILGenerator();

        getIl.Emit(OpCodes.Ldarg_0);
        getIl.Emit(OpCodes.Ldfld, fieldBuilder);
        getIl.Emit(OpCodes.Ret);

        var setMethodBuilder = typeBuilder.DefineMethod("set_" + propertyName,
                            MethodAttributes.Public |
                            MethodAttributes.SpecialName |
                            MethodAttributes.HideBySig,
                            null,
                            new Type[] { propertyType });
        var setIl = setMethodBuilder.GetILGenerator();

        setIl.Emit(OpCodes.Ldarg_0);
        setIl.Emit(OpCodes.Ldarg_1);
        setIl.Emit(OpCodes.Stfld, fieldBuilder);
        setIl.Emit(OpCodes.Ret);

        propertyBuilder.SetGetMethod(getMethodBuilder);
        propertyBuilder.SetSetMethod(setMethodBuilder);
    }

}

//    Option 1: Using AutoMapper's ForMember Method
//You can use AutoMapper's ForMember method to explicitly define how properties should be mapped from the source object to the interface.

public class MyClass
{
    public string Property1 { get; set; }
    public int Property2 { get; set; }
    public DateTime DateProperty { get; set; } // Additional property not in some interfaces
}

public interface IInterface1
{
    string Property1 { get; }
}

public interface IInterface2
{
    int Property2 { get; }
}

























//// Implementation class for the interface
//public class MyInterfaceImpl : IMyInterface
//{
//    public string Property1 { get; set; }
//    public int Property2 { get; set; }
//}

//// Converter class to dynamically map MyClass to any interface with matching properties
//public class MyClassToInterfaceConverter : ITypeConverter<MyClass, object>
//{
//    public object Convert(MyClass source, object destination, ResolutionContext context)
//    {
//        Type destinationType = context.DestinationType;

//        if (!destinationType.IsInterface)
//        {
//            throw new ArgumentException("Destination type must be an interface.");
//        }

//        // Create a dynamic proxy object implementing the destination interface
//        object dest = Activator.CreateInstance(destinationType);

//        // Get properties of MyClass
//        PropertyInfo[] sourceProperties = typeof(MyClass).GetProperties();

//        // Get properties of the destination interface
//        PropertyInfo[] destinationProperties = destinationType.GetProperties();

//        // Map properties based on name match
//        foreach (var destProp in destinationProperties)
//        {
//            var sourceProp = sourceProperties.FirstOrDefault(prop => prop.Name == destProp.Name);
//            if (sourceProp != null && destProp.CanWrite)
//            {
//                destProp.SetValue(dest, sourceProp.GetValue(source));
//            }
//        }

//        return dest;
//    }
//}


//// Converter class to dynamically map MyClass to any interface with matching properties
//public class MyClassToInterfaceConverter : ITypeConverter<MyClass, object>
//{
//    public object Convert(MyClass source, object destination, ResolutionContext context)
//    {
//        Type destinationType = typeof(object);

//        if (!destinationType.IsInterface)
//        {
//            throw new ArgumentException("Destination type must be an interface.");
//        }

//        // Create a dynamic proxy object implementing the destination interface
//        object dest = Activator.CreateInstance(destinationType);

//        // Get properties of MyClass
//        PropertyInfo[] sourceProperties = typeof(MyClass).GetProperties();

//        // Get properties of the destination interface
//        PropertyInfo[] destinationProperties = destinationType.GetProperties();

//        // Map properties based on name match
//        foreach (var destProp in destinationProperties)
//        {
//            var sourceProp = sourceProperties.FirstOrDefault(prop => prop.Name == destProp.Name);
//            if (sourceProp != null && destProp.CanWrite)
//            {
//                destProp.SetValue(dest, sourceProp.GetValue(source));
//            }
//        }

//        return dest;
//    }
//}


//// Custom resolver to dynamically map MyClass to interfaces
//public class DynamicInterfaceMappingResolver : ITypeConverter<MyClass, object>
//{
//    public object Convert(MyClass source, object destination, ResolutionContext context)
//    {
//        Type destinationType = context.DestinationType;

//        var sourceMember = typeof(MyClass).GetMember(memberOptions.DestinationMember.Name);
//        var sourceProp = typeof(MyClass).GetProperty(memberOptions.DestinationMember.Name);
//        var sourceType = sourceProp?.PropertyType;

//        // Create an instance of a dynamic proxy object that implements the destination interface
//        object dest = Activator.CreateInstance(destinationType);

//        // Get properties of MyClass
//        PropertyInfo[] sourceProperties = typeof(MyClass).GetProperties();

//        // Get properties of the destination interface
//        PropertyInfo[] destinationProperties = destinationType.GetProperties();

//        // Map properties based on name match
//        foreach (var destProp in destinationProperties)
//        {
//            var sourceProp = sourceProperties.FirstOrDefault(prop => prop.Name == destProp.Name);
//            if (sourceProp != null && destProp.CanWrite)
//            {
//                destProp.SetValue(dest, sourceProp.GetValue(source));
//            }
//        }

//        return dest;
//    }
//}











//private static void StartVar6()
//{
//    string json = @"{
//        ""Key"": 1,
//        ""Description"": ""Sample description"",
//        ""Type"": ""Sample type"",
//        ""Height"": 10.5
//    }";

//    CommonControl commonControl = JsonConvert.DeserializeObject<CommonControl>(json);


//    Type interfaceType = typeof(ICurrentControl); // Replace with any interface type
//    Type dynamicType = DynamicClassCreatorVar3.CreateDynamicClass(interfaceType);

//    var config = new MapperConfiguration(cfg => cfg.CreateMap<CommonControl, ICurrentControl>().As(dynamicType));
//    // Initialize AutoMapper with the profile
//    //var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
//    var mapper = config.CreateMapper();

//    // Map CommonControl to ICurrentControl using the dynamic proxy factory
//    //ICurrentControl currentControl = mapper.Map<ICurrentControl>(commonControl);
//    ICurrentControl currentControl = mapper.Map<ICurrentControl>(commonControl);

//    // Output the mapped object to verify
//    Console.WriteLine($"Key: {currentControl.Key}, Type: {currentControl.Type}");
//}



//private static void StartVar5()
//{
//    string json = @"{
//        ""Key"": 1,
//        ""Description"": ""Sample description"",
//        ""Type"": ""Sample type"",
//        ""Height"": 10.5
//    }";

//    CommonControl commonControl = JsonConvert.DeserializeObject<CommonControl>(json);

//    // Initialize AutoMapper with the profile
//    var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfileVar5>());
//    var mapper = config.CreateMapper();

//    // Map CommonControl to ICurrentControl using the dynamically created type
//    ICurrentControl currentControl = mapper.Map<ICurrentControl>(commonControl);

//    // Output the mapped object to verify
//    Console.WriteLine($"Key: {currentControl.Key}, Type: {currentControl.Type}");
//}



//public class MappingProfile : Profile
//{
//    public MappingProfile()
//    {
//        Type interfaceType = typeof(ICurrentControl); // Replace with any interface type
//        Type dynamicType = DynamicClassCreatorVar3.CreateDynamicClass(interfaceType);

//        CreateMap<CommonControl, ICurrentControl>()
//            //.ConvertUsing(src => DynamicProxyFactory.CreateProxy(src))
//            .As(dynamicType);
//    }
//}



//public static class DynamicProxyFactory
//{
//    private static readonly ProxyGenerator _proxyGenerator = new ProxyGenerator();

//    internal static ICurrentControl CreateProxy(CommonControl source)
//    {
//        var proxy = _proxyGenerator.CreateInterfaceProxyWithoutTarget<ICurrentControl>(new ProxyInterceptor(source));
//        return proxy;
//    }

//    private class ProxyInterceptor : IInterceptor
//    {
//        private readonly CommonControl _source;

//        public ProxyInterceptor(CommonControl source)
//        {
//            _source = source;
//        }

//        public void Intercept(IInvocation invocation)
//        {
//            var propertyName = invocation.Method.Name.Substring(4); // remove "get_"
//            var propertyInfo = typeof(CommonControl).GetProperty(propertyName);
//            if (propertyInfo != null)
//            {
//                invocation.ReturnValue = propertyInfo.GetValue(_source);
//            }
//            else
//            {
//                throw new ArgumentException($"Property '{propertyName}' not found on '{typeof(CommonControl).Name}'.");
//            }
//        }
//    }
//}




/*
using AutoMapper;
using System;
using System.Reflection;

public class MyClass
{
    public string Property1 { get; set; }
    public int Property2 { get; set; }
    public DateTime DateProperty { get; set; } // Additional property not in some interfaces
}

public interface IInterface1
{
    string Property1 { get; set; }
}

public interface IInterface2
{
    int Property2 { get; set; }
}

public class Program
{
    public static void Main()
    {
        // Configure AutoMapper
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<MyClass, object>().ConvertUsing(new DynamicInterfaceMappingResolver());
        });

        IMapper mapper = config.CreateMapper();

        // Usage with different interfaces
        MyClass sourceObject = new MyClass { Property1 = "Value1", Property2 = 42, DateProperty = DateTime.Now };

        IInterface1 destination1 = mapper.Map<IInterface1>(sourceObject);
        Console.WriteLine($"IInterface1 - Property1: {destination1.Property1}");

        IInterface2 destination2 = mapper.Map<IInterface2>(sourceObject);
        Console.WriteLine($"IInterface2 - Property2: {destination2.Property2}");
    }
}

// Custom resolver to dynamically map MyClass to interfaces
public class DynamicInterfaceMappingResolver : ITypeConverter<MyClass, object>
{
    public object Convert(MyClass source, object destination, ResolutionContext context)
    {
        Type destinationType = context.DestinationType;

        // Create an instance of a dynamic proxy object that implements the destination interface
        object dest = Activator.CreateInstance(destinationType);

        // Get properties of MyClass
        PropertyInfo[] sourceProperties = typeof(MyClass).GetProperties();

        // Get properties of the destination interface
        PropertyInfo[] destinationProperties = destinationType.GetProperties();

        // Map properties based on name match
        foreach (var destProp in destinationProperties)
        {
            var sourceProp = sourceProperties.FirstOrDefault(prop => prop.Name == destProp.Name);
            if (sourceProp != null && destProp.CanWrite)
            {
                destProp.SetValue(dest, sourceProp.GetValue(source));
            }
        }

        return dest;
    }
} 
 */