using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Wodsoft.ComBoost.Wpf
{
    public class EditorPresenter : ContentControl
    {
        public System.Data.Entity.Metadata.PropertyMetadata Metadata { get { return (System.Data.Entity.Metadata.PropertyMetadata)GetValue(MetadataProperty); } set { SetValue(MetadataProperty, value); } }
        public static readonly DependencyProperty MetadataProperty = DependencyProperty.Register("Metadata", typeof(System.Data.Entity.Metadata.PropertyMetadata), typeof(EditorPresenter));

        public IEntity Entity { get { return (IEntity)GetValue(EntityProperty); } set { SetValue(EntityProperty, value); } }
        public static readonly DependencyProperty EntityProperty = DependencyProperty.Register("Entity", typeof(IEntity), typeof(EditorPresenter));

        public object Editor { get { return GetValue(EditorProperty); } set { SetValue(EditorProperty, value); } }
        public static readonly DependencyProperty EditorProperty = DependencyProperty.Register("Editor", typeof(object), typeof(EditorPresenter));

        public override void EndInit()
        {
            EditorBase editor;
            if (Metadata == null || Entity == null || Editor == null)
                return;
            if (Metadata.Type == CustomDataType.Other)
                editor = EditorFactory.GetEditor(Metadata.CustomType);
            else
                editor = EditorFactory.GetEditor(Metadata.Type);
            editor.BeginInit();
            editor.Metadata = Metadata;
            editor.Editor = (EntityEditor)Editor;
            editor.OriginValue = Metadata.Property.GetValue(Entity);
            editor.EndInit();
            Content = editor;
            base.EndInit();
        }
    }
}
