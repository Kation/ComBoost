using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Wodsoft.ComBoost.Wpf.Editor
{
    [TemplatePart(Name = "PART_TextBox", Type = typeof(TextBox))]
    public class EntityEditor : EditorBase
    {
        static EntityEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EntityEditor), new FrameworkPropertyMetadata(typeof(EntityEditor)));
        }

        public EntityEditor()
        {
            SelectCommand = new EntityCommand(SelectEntity);
            ClearCommand = new EntityCommand(SelectEntity);
        }

        protected TextBox TextBox { get; private set; }

        public override void OnApplyTemplate()
        {
            TextBox = (TextBox)GetTemplateChild("PART_TextBox");
            TextBox.SetBinding(TextBox.TextProperty, new Binding { Source = this, Path = new PropertyPath(CurrentValueProperty), Mode = BindingMode.OneWay });
        }

        private async void SelectEntity(IEntity entity)
        {
            dynamic controller = EntityRouter.Routers.GetController(Metadata.ClrType);
            EntitySelector selector = await controller.GetSelector();
            if (selector.ShowDialog() == true)
            {
                CurrentValue = selector.SelectedEntity;
            }
        }

        private void ClearEntity(IEntity entity)
        {
            CurrentValue = null;
            IsChanged = OriginValue == null;
        }
        public ICommand SelectCommand { get { return (ICommand)GetValue(SelectCommandProperty); } protected set { SetValue(SelectCommandPropertyKey, value); } }
        protected static readonly DependencyPropertyKey SelectCommandPropertyKey = DependencyProperty.RegisterReadOnly("SelectCommand", typeof(ICommand), typeof(EntityEditor), new PropertyMetadata());
        public static readonly DependencyProperty SelectCommandProperty = SelectCommandPropertyKey.DependencyProperty;

        public ICommand ClearCommand { get { return (ICommand)GetValue(ClearCommandProperty); } protected set { SetValue(ClearCommandPropertyKey, value); } }
        protected static readonly DependencyPropertyKey ClearCommandPropertyKey = DependencyProperty.RegisterReadOnly("ClearCommand", typeof(ICommand), typeof(EntityEditor), new PropertyMetadata());
        public static readonly DependencyProperty ClearCommandProperty = ClearCommandPropertyKey.DependencyProperty;
    }
}
