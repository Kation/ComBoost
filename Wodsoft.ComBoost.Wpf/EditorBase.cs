using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Wodsoft.ComBoost.Wpf
{
    public abstract class EditorBase : Control
    {
        public object OriginValue
        {
            get { return GetValue(OriginValueProperty); }
            set
            {
                if (_Init)
                    SetValue(OriginValuePropertyKey, value);
            }
        }
        protected static readonly DependencyPropertyKey OriginValuePropertyKey = DependencyProperty.RegisterReadOnly("OriginValue", typeof(object), typeof(EditorBase), new PropertyMetadata());
        public static readonly DependencyProperty OriginValueProperty = OriginValuePropertyKey.DependencyProperty;

        public object CurrentValue { get { return GetValue(CurrentValueProperty); } set { SetValue(CurrentValueProperty, value); } }
        protected static readonly DependencyProperty CurrentValueProperty = DependencyProperty.Register("CurrentValue", typeof(object), typeof(EditorBase), new PropertyMetadata(CurrentValueChanged));
        private static void CurrentValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EditorBase editor = (EditorBase)d;
            editor.ValidateCore();
            if (!editor._Init)
                editor.IsChanged = true;
        }

        public System.Data.Entity.Metadata.PropertyMetadata Metadata { get { return (System.Data.Entity.Metadata.PropertyMetadata)GetValue(MetadataProperty); } set { SetValue(MetadataProperty, value); } }
        public static readonly DependencyProperty MetadataProperty = DependencyProperty.Register("Metadata", typeof(System.Data.Entity.Metadata.PropertyMetadata), typeof(EditorBase));

        public bool IsChanged { get { return (bool)GetValue(IsChangedProperty); } protected set { SetValue(IsChangedPropertyKey, value); } }
        protected static readonly DependencyPropertyKey IsChangedPropertyKey = DependencyProperty.RegisterReadOnly("IsChanged", typeof(bool), typeof(EditorBase), new PropertyMetadata(false));
        public static readonly DependencyProperty IsChangedProperty = IsChangedPropertyKey.DependencyProperty;

        public bool HasError { get { return (bool)GetValue(HasErrorProperty); } protected set { SetValue(HasErrorPropertyKey, value); } }
        protected static readonly DependencyPropertyKey HasErrorPropertyKey = DependencyProperty.RegisterReadOnly("HasError", typeof(bool), typeof(EditorBase), new PropertyMetadata(false));
        public static readonly DependencyProperty HasErrorProperty = HasErrorPropertyKey.DependencyProperty;

        public EntityEditor Editor { get { return (EntityEditor)GetValue(EditorProperty); } set { SetValue(EditorProperty, value); } }
        public static readonly DependencyProperty EditorProperty = DependencyProperty.Register("Editor", typeof(EntityEditor), typeof(EditorBase), new PropertyMetadata());

        public string ErrorMessage { get { return (string)GetValue(ErrorMessageProperty); } protected set { SetValue(ErrorMessagePropertyKey, value); } }
        protected static readonly DependencyPropertyKey ErrorMessagePropertyKey = DependencyProperty.RegisterReadOnly("ErrorMessage", typeof(string), typeof(EditorBase), new PropertyMetadata());
        public static readonly DependencyProperty ErrorMessageProperty = ErrorMessagePropertyKey.DependencyProperty;

        private bool _Init;
        public override void BeginInit()
        {
            base.BeginInit();
            _Init = true;
        }

        public override void EndInit()
        {
            if (Metadata == null)
                throw new InvalidOperationException("Metadata is null.");
            if (Editor == null)
                throw new InvalidOperationException("Editor is null.");
            Editor.PreviewSave += Editor_PreviewSave;
            Editor.Saving += Editor_Saving;
            CurrentValue = OriginValue;
            IsChanged = false;
            _Init = false;
            ValidateCore();
            base.EndInit();
        }

        private void Editor_Saving(object sender, RoutedEventArgs e)
        {
            if (IsChanged)
                Metadata.Property.SetValue(Editor.Model.Item, CurrentValue);
        }

        private void Editor_PreviewSave(object sender, RoutedEventArgs e)
        {
            if (HasError)
                e.Handled = true;
        }

        protected virtual bool ValidateCore()
        {
            foreach (var att in Metadata.Property.GetCustomAttributes(true).OfType<ValidationAttribute>())
            {
                try
                {
                    att.Validate(CurrentValue, Metadata.Name);
                }
                catch (Exception ex)
                {
                    HasError = true;
                    ErrorMessage = ex.Message;
                    return false;
                }
            }
            HasError = false;
            ErrorMessage = null;
            return true;
        }

        public bool Validate(ValidationContext validationContext)
        {
            return ValidateCore();
        }

    }
}
