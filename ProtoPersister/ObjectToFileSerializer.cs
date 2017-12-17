using ProtoBuf;
using ProtoBuf.Meta;
using System;
using System.IO;
using System.Linq;

namespace Proto
{
    internal sealed class ObjectToFileSerializer
    {
        private RuntimeTypeModel _runtimeTypeModel;

        public string Serialize(object objectToSerialize, string filePath)
        {
            EnsureRuntimeTypeModelCreated(objectToSerialize.GetType());
           
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
            {
                _runtimeTypeModel.Serialize(fs, objectToSerialize);
            }
            
            return "";
        }

        public object Deserialize(string pathToFile, Type objectType)
        {
            EnsureRuntimeTypeModelCreated(objectType);
            using (var reader = new FileStream(pathToFile, FileMode.Open, FileAccess.Read))
            {
                return _runtimeTypeModel.Deserialize(reader, null, objectType);
            }
        }
        
        public T DeepClone<T>(object objectToClone)
        {
            if(objectToClone == null)
            {
                return default(T);
            }

            EnsureRuntimeTypeModelCreated(objectToClone.GetType());
            return (T)_runtimeTypeModel.DeepClone(objectToClone);
        }

        public static RuntimeTypeModel BuildRuntimeType(Type type)
        {
            var model = RuntimeTypeModel.Create();

            BuildTypeRecursive(type, model);

            // Compile model in place for better performance, .Compile() can be used if all types are known beforehand
            model.CompileInPlace();

            return model;
        }

        private void EnsureRuntimeTypeModelCreated(Type objectType)
        {
            if (_runtimeTypeModel == null)
            {
                _runtimeTypeModel = BuildRuntimeType(objectType);
            }
        }

        private static void BuildTypeRecursive(Type type, RuntimeTypeModel model)
        {
            if(AlreadyContainsType(type, model))
            {
                return;
            }

            var metaType = model.Add(type, false);
            metaType.AsReferenceDefault = true;
            metaType.UseConstructor = false;
            metaType.EnumPassthru = true;

            // Add contract for all the serializable fields
            var serializableFields = type
                .GetProperties()
                .Where(fi => !Attribute.IsDefined(fi, typeof(NonSerializedAttribute)) && (fi.PropertyType.IsPublic || fi.PropertyType.IsNestedPublic))
                .OrderBy(fi => fi.Name)  // it's important to keep the same fields order in all the AppDomains
                .Select((fi, index) => new { info = fi, index }).ToArray();
            foreach (var field in serializableFields)
            {
                if (field.info.PropertyType.IsValueType || 
                    field.info.PropertyType.IsPrimitive || 
                    field.info.PropertyType.IsSerializable)
                {
                    var metaField = metaType.AddField(field.index + 1, field.info.Name);
                    metaField.AsReference = !field.info.PropertyType.IsValueType;       // cyclic references support
                    metaField.DynamicType = field.info.PropertyType == typeof(object);  // any type support
                }

                if (!field.info.PropertyType.IsSimpleType() 
                     || field.info.PropertyType.IsNested)
                {
                    var genericEnumerables = ReflectionHelper.GetGenericIEnumerables(field.info.PropertyType);
                    if(genericEnumerables.Any())
                    {
                        foreach(var item in genericEnumerables)
                        {
                            BuildTypeRecursive(item, model);
                        }
                    }
                    else
                    {
                        BuildTypeRecursive(field.info.PropertyType, model);
                    }
                }
            }
        }

        private static bool AlreadyContainsType(Type type, RuntimeTypeModel model)
        {
            foreach(var t in model.GetTypes())
            {
                var knownType = ((MetaType)t).Type;
                if (knownType == type)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
