using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using Wodsoft.ComBoost.Business.Controls.EditorItems;

namespace Wodsoft.ComBoost.Business.Controls
{
    public class DefaultEditorItemFactory : EditorItemFactory
    {
        public WorkFrame Frame { get; set; }

        public sealed override EditorItem GetEditorItem(PropertyInfo property)
        {
            CustomDataType type = CustomDataType.Default;
            string custom = null;
            var att = property.GetCustomAttributes(typeof(CustomDataTypeAttribute), true).FirstOrDefault() as CustomDataTypeAttribute;
            if (att != null)
            {
                type = att.Type;
                custom = att.Custom;
            }
            else
            {
                if (property.PropertyType == typeof(DateTime))
                    type = CustomDataType.Date;
                if (property.PropertyType == typeof(TimeSpan))
                    type = CustomDataType.Time;
                else if (property.PropertyType == typeof(bool))
                    type = CustomDataType.Boolean;
                else if (property.PropertyType == typeof(short) || property.PropertyType == typeof(int) || property.PropertyType == typeof(long))
                    type = CustomDataType.Integer;
                else if (property.PropertyType == typeof(float) || property.PropertyType == typeof(double))
                    type = CustomDataType.Number;
                else if (property.PropertyType == typeof(decimal))
                    type = CustomDataType.Currency;
                else if (property.PropertyType.IsEnum)
                {
                    type = CustomDataType.Other;
                    custom = "Enum";
                }
                else if (property.PropertyType.IsGenericType)
                {
                    type = CustomDataType.Other;
                    custom = "Collection";
                }
                else if (typeof(EntityBase).IsAssignableFrom(property.PropertyType))
                {
                    type = CustomDataType.Other;
                    custom = "Entity";
                }
            }
            return GetEditorItem(type, custom, property);
        }

        protected virtual EditorItem GetEditorItem(CustomDataType type, string custom, PropertyInfo property)
        {
            RequiredAttribute required = property.GetCustomAttributes(typeof(RequiredAttribute), true).FirstOrDefault() as RequiredAttribute;
            EditorItem item;
            switch (type)
            {
                case CustomDataType.Boolean:
                    item = new BoolEditor(Frame);
                    break;
                case CustomDataType.Currency:
                    item = new CurrencyEditor(Frame);
                    break;
                case CustomDataType.Date:
                    item = new DateEditor(Frame);
                    break;
                case CustomDataType.DateTime:
                    item = new DateTimeEditor(Frame);
                    break;
                case CustomDataType.Default:
                    item = new DefaultEditor(Frame);
                    if (required != null)
                        ((DefaultEditor)item).IsAllowdEmpty = required.AllowEmptyStrings;
                    break;
                case CustomDataType.EmailAddress:
                    item = new EmailAddressEditor(Frame);
                    break;
                case CustomDataType.Html:
                    item = new HtmlEditor(Frame);
                    break;
                case CustomDataType.ImageUrl:
                    item = new ImageUrlEditor(Frame);
                    break;
                case CustomDataType.Image:
                    item = new ImageEditor(Frame);
                    break;
                case CustomDataType.Integer:
                    item = new IntegerEditor(Frame);
                    break;
                case CustomDataType.MultilineText:
                    item = new MultilineTextEditor(Frame);
                    if (required != null)
                        ((MultilineTextEditor)item).IsAllowdEmpty = required.AllowEmptyStrings;
                    break;
                case CustomDataType.Number:
                    item = new NumberEditor(Frame);
                    break;
                case CustomDataType.Password:
                    item = new PasswordEditor(Frame);
                    break;
                case CustomDataType.PhoneNumber:
                    item = new PhoneNumberEditor(Frame);
                    break;
                case CustomDataType.Sex:
                    item = new SexEditor(Frame);
                    break;
                case CustomDataType.Text:
                    item = new DefaultEditor(Frame);
                    break;
                case CustomDataType.Time:
                    item = new TimeEditor(Frame);
                    break;
                case CustomDataType.Url:
                    item = new UrlEditor(Frame);
                    break;
                default:
                    switch (custom)
                    {
                        case "Enum":
                            item = GetEnumEditorItem(property.PropertyType);
                            break;
                        case "Entity":
                            item = new EntityEditor(Frame, property.PropertyType);
                            break;
                        case "Collection":
                            item = new CollectionEditor(Frame, property.PropertyType.GetGenericArguments()[0]);
                            break;
                        default:
                            throw new NotSupportedException("不支持自定义类型编辑器。");
                    }
                    break;
            }
            if (required != null)
                item.IsRequired = true;
            return item;
        }

        private EditorItem GetEnumEditorItem(Type enumType)
        {
            return new ComboBoxEditor(Frame, Enum.GetValues(enumType).Cast<object>().ToArray());
        }
    }
}
