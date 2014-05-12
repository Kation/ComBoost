using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Wodsoft.ComBoost.Business.Controls
{
    [ContentProperty("Content")]
    public abstract class EditorItem : Control, IDisposable
    {
        static EditorItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EditorItem), new FrameworkPropertyMetadata(typeof(EditorItem)));
        }

        public EditorItem(WorkFrame frame)
        {
            IsChanged = false;
            Initialized = false;
            Frame = frame;
        }

        public WorkFrame Frame { get; private set; }

        new internal bool Initialized { get; set; }
        internal string Title { get; set; }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(EditorItem), new PropertyMetadata(OnValueChanged));
        public object Value { get { return GetValue(ValueProperty); } set { SetValue(ValueProperty, value); } }

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(object), typeof(EditorItem));
        public object Content { get { return GetValue(ContentProperty); } set { SetValue(ContentProperty, value); } }

        private bool _IsRequired;
        public bool IsRequired { get { return _IsRequired; } set { _IsRequired = value; OnValueChanged(this, new DependencyPropertyChangedEventArgs()); } }

        public bool IsChanged { get; protected set; }

        public EditorPropertySetter CustomSetter { get; set; }

        public event EditorValueChanged ValueChanged;

        public virtual bool ValidateData()
        {
            if (IsRequired && Value == null)
                return false;
            return true;
        }

        private static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            EditorItem host = (EditorItem)sender;
            if (!host.ValidateData())
                host.BorderBrush = Brushes.Red;
            else
                host.BorderBrush = Brushes.Transparent;
            if (host.Initialized)
                host.IsChanged = true;
            if (host.ValueChanged != null)
                host.ValueChanged(host);
        }

        public virtual void Dispose()
        {
            
        }

        public virtual void UpdateValue()
        {

        }
    }

    public delegate void EditorValueChanged(EditorItem item);

    public delegate void EditorPropertySetter(object entity, EditorItem editorItem, PropertyInfo property);
}
