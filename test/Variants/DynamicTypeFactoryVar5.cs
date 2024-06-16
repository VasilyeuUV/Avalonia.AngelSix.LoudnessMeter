using System.Reflection;
using System.Reflection.Emit;
using test.Interfaces;

namespace test.Variants
{
    internal static class DynamicTypeFactoryVar5
    {
        public static Type CreateDynamicImplementationType()
        {
            // Define the properties of the interface dynamically
            var properties = new Dictionary<string, Type>
        {
            { nameof(ICurrentControl.Key), typeof(int?) },
            { nameof(ICurrentControl.Type), typeof(string) }
        };

            // Create the dynamic type
            var typeBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("DynamicAssembly"), AssemblyBuilderAccess.Run)
                                             .DefineDynamicModule("DynamicModule")
                                             .DefineType("DynamicCurrentControl", TypeAttributes.Public);

            foreach (var property in properties)
            {
                var fieldBuilder = typeBuilder.DefineField("_" + property.Key.ToLower(), property.Value, FieldAttributes.Private);
                var propertyBuilder = typeBuilder.DefineProperty(property.Key, PropertyAttributes.None, property.Value, Type.EmptyTypes);

                var getMethodBuilder = typeBuilder.DefineMethod("get_" + property.Key, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, property.Value, Type.EmptyTypes);
                var getIl = getMethodBuilder.GetILGenerator();
                getIl.Emit(OpCodes.Ldarg_0);
                getIl.Emit(OpCodes.Ldfld, fieldBuilder);
                getIl.Emit(OpCodes.Ret);

                var setMethodBuilder = typeBuilder.DefineMethod("set_" + property.Key, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, new Type[] { property.Value });
                var setIl = setMethodBuilder.GetILGenerator();
                setIl.Emit(OpCodes.Ldarg_0);
                setIl.Emit(OpCodes.Ldarg_1);
                setIl.Emit(OpCodes.Stfld, fieldBuilder);
                setIl.Emit(OpCodes.Ret);

                propertyBuilder.SetGetMethod(getMethodBuilder);
                propertyBuilder.SetSetMethod(setMethodBuilder);
            }

            var createdType = typeBuilder.CreateType();

            return createdType;
        }
    }
}
