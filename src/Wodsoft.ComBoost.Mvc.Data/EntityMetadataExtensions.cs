using System;
using System.Linq;
using Wodsoft.ComBoost.Data.Entity.Metadata;

namespace Wodsoft.ComBoost.Mvc
{
    public static class EntityMetadataExtensions
    {
        //public static IEdmModel GetEdmModel(this IEntityMetadata metadata)
        //{
        //    EdmModel model = metadata.DataBag.EdmModel;
        //    if (model == null)
        //    {
        //        model = new EdmModel();
        //        var entityType = new EdmEntityType(metadata.Type.Namespace, metadata.Type.Name);
        //        var keyProperty = entityType.AddStructuralProperty(metadata.KeyProperty.ClrName, GetEdmTypeKind(metadata.KeyProperty.ClrType));
        //        entityType.AddKeys(keyProperty);
        //        //entityType.AddKeys()
        //        foreach (var property in metadata.SearchProperties)
        //        {
        //            if (property.CustomType == "Entity")
        //            {
        //                var navEntityMetadata = EntityDescriptor.GetMetadata(property.ClrType);
        //                var navEntityType = new EdmEntityType(navEntityMetadata.Type.Namespace, navEntityMetadata.Type.Name);
        //                navEntityType.AddStructuralProperty(navEntityMetadata.KeyProperty.ClrName, GetEdmTypeKind(navEntityMetadata.KeyProperty.ClrType));
        //                navEntityType.AddStructuralProperty(navEntityMetadata.DisplayProperty.ClrName, GetEdmTypeKind(navEntityMetadata.DisplayProperty.ClrType));
        //                model.AddElement(navEntityType);
        //                //EdmTypeReference
        //                //entityType.AddStructuralProperty(property.ClrName, navEntityType.);
        //            }
        //            else if (property.CustomType == "Enum")
        //            {
        //                var enumType = model.SchemaElements.Count(t => t.Namespace == property.ClrType.Namespace && t.Name == property.ClrType.Name);

        //            }
        //            else
        //                entityType.AddStructuralProperty(property.ClrName, GetEdmTypeKind(property.ClrType));
        //        }
        //    }
        //    return model;
        //}

        //private static EdmPrimitiveTypeKind GetEdmTypeKind(Type type)
        //{
        //    if (type == typeof(string))
        //        return EdmPrimitiveTypeKind.String;
        //    else if (type == typeof(bool))
        //        return EdmPrimitiveTypeKind.Boolean;
        //    else if (type == typeof(sbyte))
        //        return EdmPrimitiveTypeKind.SByte;
        //    else if (type == typeof(byte))
        //        return EdmPrimitiveTypeKind.Byte;
        //    else if (type == typeof(short))
        //        return EdmPrimitiveTypeKind.Int16;
        //    else if (type == typeof(int))
        //        return EdmPrimitiveTypeKind.Int32;
        //    else if (type == typeof(long))
        //        return EdmPrimitiveTypeKind.Int64;
        //    else if (type == typeof(float))
        //        return EdmPrimitiveTypeKind.Single;
        //    else if (type == typeof(double))
        //        return EdmPrimitiveTypeKind.Double;
        //    else if (type == typeof(decimal))
        //        return EdmPrimitiveTypeKind.Decimal;
        //    else if (type == typeof(DateTime))
        //        return EdmPrimitiveTypeKind.Date;
        //    else if (type == typeof(TimeSpan))
        //        return EdmPrimitiveTypeKind.TimeOfDay;
        //    else if (type == typeof(DateTimeOffset))
        //        return EdmPrimitiveTypeKind.DateTimeOffset;
        //    else if (type == typeof(Guid))
        //        return EdmPrimitiveTypeKind.Guid;
        //    else
        //        throw new NotSupportedException("不支持的类型。");
        //}
    }
}
