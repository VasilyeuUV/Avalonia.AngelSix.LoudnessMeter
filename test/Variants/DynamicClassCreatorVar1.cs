using System.Reflection;
using System.Reflection.Emit;
using test.Interfaces;

namespace test.Variants
{
    internal class DynamicClassCreatorVar1
    {
        internal static void StartDynamicCreateCurrentClassByInterface()
        {

            Type dynamicType = DynamicClassCreatorVar1.CreateDynamicControlClass();
            var instance = Activator.CreateInstance(dynamicType);

            // Example of setting properties through reflection
            PropertyInfo keyProperty = dynamicType.GetProperty("Key");
            PropertyInfo descriptionProperty = dynamicType.GetProperty("Description");
            PropertyInfo typeProperty = dynamicType.GetProperty("Type");
            PropertyInfo heightProperty = dynamicType.GetProperty("Height");

            keyProperty.SetValue(instance, 123);
            descriptionProperty.SetValue(instance, "Test Description");
            typeProperty.SetValue(instance, "Test Type");
            heightProperty.SetValue(instance, 45.6);

            Console.WriteLine("Key: " + keyProperty.GetValue(instance));
            Console.WriteLine("Description: " + descriptionProperty.GetValue(instance));
            Console.WriteLine("Type: " + typeProperty.GetValue(instance));
            Console.WriteLine("Height: " + heightProperty.GetValue(instance));
        }


        public static Type CreateDynamicControlClass()
        {
            string assemblyName = "DynamicAssembly";
            string moduleName = "DynamicModule";
            string typeName = "DynamicControl";

            AssemblyName aName = new AssemblyName(assemblyName);
            AssemblyBuilder ab = AssemblyBuilder.DefineDynamicAssembly(aName, AssemblyBuilderAccess.RunAndCollect);
            ModuleBuilder mb = ab.DefineDynamicModule(moduleName);

            TypeBuilder tb = mb.DefineType(typeName, TypeAttributes.Public);
            tb.AddInterfaceImplementation(typeof(IControl));

            // Define fields for properties
            FieldBuilder keyField = tb.DefineField("_key", typeof(int?), FieldAttributes.Private);
            FieldBuilder descriptionField = tb.DefineField("_description", typeof(string), FieldAttributes.Private);
            FieldBuilder typeField = tb.DefineField("_type", typeof(string), FieldAttributes.Private);
            FieldBuilder heightField = tb.DefineField("_height", typeof(double?), FieldAttributes.Private);

            // Define Key property
            DefineProperty(tb, "Key", typeof(int?), keyField);

            // Define Description property
            DefineProperty(tb, "Description", typeof(string), descriptionField);

            // Define Type property
            DefineProperty(tb, "Type", typeof(string), typeField);

            // Define Height property
            DefineProperty(tb, "Height", typeof(double?), heightField);

            Type dynamicType = tb.CreateType();

            return dynamicType;
        }

        private static void DefineProperty(TypeBuilder tb, string propertyName, Type propertyType, FieldBuilder fieldBuilder)
        {
            PropertyBuilder pb = tb.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);

            // Define getter method
            MethodBuilder getMethodBuilder = tb.DefineMethod(
                "get_" + propertyName,
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.Virtual | MethodAttributes.HideBySig,
                propertyType,
                Type.EmptyTypes);

            ILGenerator getIL = getMethodBuilder.GetILGenerator();
            getIL.Emit(OpCodes.Ldarg_0);
            getIL.Emit(OpCodes.Ldfld, fieldBuilder);
            getIL.Emit(OpCodes.Ret);

            pb.SetGetMethod(getMethodBuilder);

            // Define setter method
            MethodBuilder setMethodBuilder = tb.DefineMethod(
                "set_" + propertyName,
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                null,
                new Type[] { propertyType });

            ILGenerator setIL = setMethodBuilder.GetILGenerator();
            setIL.Emit(OpCodes.Ldarg_0);
            setIL.Emit(OpCodes.Ldarg_1);
            setIL.Emit(OpCodes.Stfld, fieldBuilder);
            setIL.Emit(OpCodes.Ret);

            pb.SetSetMethod(setMethodBuilder);

            // Implement the interface method
            MethodInfo interfaceGetMethod = typeof(IControl).GetMethod("get_" + propertyName);
            tb.DefineMethodOverride(getMethodBuilder, interfaceGetMethod);
        }
    }
}
