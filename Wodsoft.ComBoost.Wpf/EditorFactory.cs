using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Wpf.Editor;

namespace Wodsoft.ComBoost.Wpf
{
    public static class EditorFactory
    {
        private static Dictionary<string, Type> _CustomStringEditor;
        private static Dictionary<CustomDataType, Type> _CustomTypeEditor;

        static EditorFactory()
        {
            _CustomStringEditor = new Dictionary<string, Type>();
            _CustomTypeEditor = new Dictionary<CustomDataType, Type>();

            AddEditor<BooleanEditor>(CustomDataType.Boolean);
            AddEditor<CurrencyEditor>(CustomDataType.Currency);
            AddEditor<DateEditor>(CustomDataType.Date);
            AddEditor<DateTimeEditor>(CustomDataType.DateTime);
            AddEditor<DefaultEditor>(CustomDataType.Default);
            AddEditor<EmailAddressEditor>(CustomDataType.EmailAddress);
            AddEditor<DefaultEditor>(CustomDataType.File);
            AddEditor<HtmlEditor>(CustomDataType.Html);
            AddEditor<DefaultEditor>(CustomDataType.Image);
            AddEditor<ImageUrlEditor>(CustomDataType.ImageUrl);
            AddEditor<IntegerEditor>(CustomDataType.Integer);
            AddEditor<MultilineTextEditor>(CustomDataType.MultilineText);
            AddEditor<NumberEditor>(CustomDataType.Number);
            AddEditor<PasswordEditor>(CustomDataType.Password);
            AddEditor<PhoneNumberEditor>(CustomDataType.PhoneNumber);
            AddEditor<SexEditor>(CustomDataType.Sex);
            AddEditor<TextEditor>(CustomDataType.Text);
            AddEditor<TimeEditor>(CustomDataType.Time);
            AddEditor<UrlEditor>(CustomDataType.Url);

            AddEditor<DefaultEditor>("Collection");
            AddEditor<Editor.EntityEditor>("Entity");
            AddEditor<DefaultEditor>("Enum");
        }

        public static void AddEditor<TEditor>(string type) where TEditor : EditorBase, new()
        {
            if (_CustomStringEditor.ContainsKey(type))
                _CustomStringEditor[type] = typeof(TEditor);
            else
                _CustomStringEditor.Add(type, typeof(TEditor));
        }

        public static void AddEditor<TEditor>(CustomDataType type) where TEditor : EditorBase, new()
        {
            if (_CustomTypeEditor.ContainsKey(type))
                _CustomTypeEditor[type] = typeof(TEditor);
            else
                _CustomTypeEditor.Add(type, typeof(TEditor));
        }

        public static EditorBase GetEditor(string type)
        {
            if (!_CustomStringEditor.ContainsKey(type))
                return null;
            return (EditorBase)Activator.CreateInstance(_CustomStringEditor[type]);
        }

        public static EditorBase GetEditor(CustomDataType type)
        {
            if (!_CustomTypeEditor.ContainsKey(type))
                return null;
            return (EditorBase)Activator.CreateInstance(_CustomTypeEditor[type]);
        }
    }
}
