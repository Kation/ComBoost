using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Wpf.Editor
{
    public static class EditorFactory
    {
        private static Dictionary<string, Type> _CustomStringEditor;
        private static Dictionary<CustomDataType, Type> _CustomTypeEditor;

        static EditorFactory()
        {
            _CustomStringEditor = new Dictionary<string, Type>();
            _CustomTypeEditor = new Dictionary<CustomDataType, Type>();

            AddEditor<DefaultEditor>(CustomDataType.Boolean);
            AddEditor<DefaultEditor>(CustomDataType.Currency);
            AddEditor<DefaultEditor>(CustomDataType.Date);
            AddEditor<DefaultEditor>(CustomDataType.DateTime);
            AddEditor<DefaultEditor>(CustomDataType.Default);
            AddEditor<DefaultEditor>(CustomDataType.EmailAddress);
            AddEditor<DefaultEditor>(CustomDataType.File);
            AddEditor<DefaultEditor>(CustomDataType.Html);
            AddEditor<DefaultEditor>(CustomDataType.Image);
            AddEditor<DefaultEditor>(CustomDataType.ImageUrl);
            AddEditor<DefaultEditor>(CustomDataType.Integer);
            AddEditor<DefaultEditor>(CustomDataType.MultilineText);
            AddEditor<DefaultEditor>(CustomDataType.Number);
            AddEditor<DefaultEditor>(CustomDataType.Password);
            AddEditor<DefaultEditor>(CustomDataType.PhoneNumber);
            AddEditor<DefaultEditor>(CustomDataType.Sex);
            AddEditor<DefaultEditor>(CustomDataType.Text);
            AddEditor<DefaultEditor>(CustomDataType.Time);
            AddEditor<DefaultEditor>(CustomDataType.Url);

            AddEditor<DefaultEditor>("Collection");
            AddEditor<DefaultEditor>("Entity");
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
