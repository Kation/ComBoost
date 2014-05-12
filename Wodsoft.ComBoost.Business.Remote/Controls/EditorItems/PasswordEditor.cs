using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace Wodsoft.ComBoost.Business.Controls.EditorItems
{
    public class PasswordEditor : EditorItem
    {
        private PasswordBox password;

        public PasswordEditor(WorkFrame frame)
            : base(frame)
        {
            CustomSetter = new EditorPropertySetter(SetPassword);

            password = new PasswordBox();
            password.PasswordChanged += password_PasswordChanged;
            Content = password;
        }

        private void password_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (password.Password != "********")
            {
                Value = password.Password;
                IsChanged = true;
            }
        }

        protected override void OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.Name == "Value")
            {
                if (Value.GetType() == typeof(byte[]))
                {
                    password.Password = "********";
                    IsChanged = false;
                }
            }
            base.OnPropertyChanged(e);
        }

        private void SetPassword(object entity, EditorItem editorItem, PropertyInfo property)
        {
            if (entity.GetType().GetInterfaces().Count(t => t == typeof(IPassword)) == 0)
                property.SetValue(entity, editorItem.Value, null);
            else
                typeof(IPassword).GetMethod("SetPassword").Invoke(entity, new object[] { editorItem.Value });
        }
    }
}
