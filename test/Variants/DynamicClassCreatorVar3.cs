using System.Reflection;
using System.Reflection.Emit;
using test.Interfaces;

namespace test.Variants
{
    internal class DynamicClassCreatorVar3
    {
        internal static void StartVar3()
        {
            Type interfaceType = typeof(IControl); // Replace with any interface type
            Type dynamicType = DynamicClassCreatorVar3.CreateDynamicClass(interfaceType);
            var instance = Activator.CreateInstance(dynamicType);

            // Set properties through reflection
            SetProperty(dynamicType, instance, "Key", 123);
            SetProperty(dynamicType, instance, "Description", "Test Description");
            SetProperty(dynamicType, instance, "Type", "Test Type");
            SetProperty(dynamicType, instance, "Height", 45.6);

            // Get properties through reflection
            foreach (var property in interfaceType.GetProperties())
            {
                Console.WriteLine($"{property.Name}: {dynamicType.GetProperty(property.Name)?.GetValue(instance)}");
            }
        }

        private static void SetProperty(Type type, object instance, string propertyName, object value)
        {
            type.GetProperty(propertyName)?.SetValue(instance, value);
        }



        public static Type CreateDynamicClass(Type interfaceType)
        {
            if (!interfaceType.IsInterface || !interfaceType.Name.StartsWith("I"))
                throw new ArgumentException("The provided type must be an interface and its name must start with 'I'.", nameof(interfaceType));

            string assemblyName = "DynamicAssembly";
            string moduleName = "DynamicModule";
            string typeName = interfaceType.Name.Substring(1); // Remove the first letter "I"

            AssemblyName aName = new AssemblyName(assemblyName);
            AssemblyBuilder ab = AssemblyBuilder.DefineDynamicAssembly(aName, AssemblyBuilderAccess.RunAndCollect);
            ModuleBuilder mb = ab.DefineDynamicModule(moduleName);

            TypeBuilder tb = mb.DefineType(typeName, TypeAttributes.Public);
            tb.AddInterfaceImplementation(interfaceType);

            foreach (var property in interfaceType.GetProperties())
            {
                FieldBuilder fieldBuilder = tb.DefineField("_" + property.Name, property.PropertyType, FieldAttributes.Private);
                DefineProperty(tb, property, fieldBuilder);
            }

            return tb.CreateType();
        }

        private static void DefineProperty(TypeBuilder tb, PropertyInfo propertyInfo, FieldBuilder fieldBuilder)
        {
            string propertyName = propertyInfo.Name;
            Type propertyType = propertyInfo.PropertyType;

            PropertyBuilder pb = tb.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);

            // Define getter method
            MethodBuilder getMethodBuilder = tb.DefineMethod("get_" + propertyName,
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.Virtual | MethodAttributes.HideBySig,
                propertyType, Type.EmptyTypes);

            ILGenerator getIL = getMethodBuilder.GetILGenerator();
            getIL.Emit(OpCodes.Ldarg_0);
            getIL.Emit(OpCodes.Ldfld, fieldBuilder);
            getIL.Emit(OpCodes.Ret);
            pb.SetGetMethod(getMethodBuilder);

            // Define setter method
            MethodBuilder setMethodBuilder = tb.DefineMethod("set_" + propertyName,
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                null, new[] { propertyType });

            ILGenerator setIL = setMethodBuilder.GetILGenerator();
            setIL.Emit(OpCodes.Ldarg_0);
            setIL.Emit(OpCodes.Ldarg_1);
            setIL.Emit(OpCodes.Stfld, fieldBuilder);
            setIL.Emit(OpCodes.Ret);
            pb.SetSetMethod(setMethodBuilder);

            // Implement the interface method if it exists
            if (propertyInfo.GetGetMethod() != null)
            {
                tb.DefineMethodOverride(getMethodBuilder, propertyInfo.GetGetMethod());
            }
        }
    }
}
