using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Metadata;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Wodsoft.ComBoost.Wpf.Editor
{
    [TemplatePart(Name = "PART_Selector", Type = typeof(Selector))]
    [TemplatePart(Name = "PART_Add", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Remove", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Clear", Type = typeof(Button))]
    public class CollectionEditor : EditorBase
    {
        static CollectionEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CollectionEditor), new FrameworkPropertyMetadata(typeof(CollectionEditor)));
        }

        private Selector _Selector;
        private Button _Add, _Remove, _Clear;
        private List<IEntity> _OriginList, _AddList, _RemoveList;
        private ObservableCollection<IEntity> _CurrentList;
        private dynamic _EntityQueryable;

        public override void OnApplyTemplate()
        {
            _Selector = (Selector)GetTemplateChild("PART_Selector");
            _Add = (Button)GetTemplateChild("PART_Add");
            _Remove = (Button)GetTemplateChild("PART_Remove");
            _Clear = (Button)GetTemplateChild("PART_Clear");

            _Add.Click += _Add_Click;
            _Remove.Click += _Remove_Click;
            _Clear.Click += _Clear_Click;

            _Remove.SetBinding(UIElement.IsEnabledProperty, new Binding("SelectedItem")
            {
                Source = _Selector,
                Converter = new IsNullToBooleanValueConverter(),
                ConverterParameter = false
            });
        }

        public override void EndInit()
        {
            base.EndInit();
            _OriginList = ((IEnumerable)OriginValue).Cast<IEntity>().ToList();
            _CurrentList = new ObservableCollection<IEntity>(_OriginList);
            _AddList = new List<IEntity>();
            _RemoveList = new List<IEntity>();
            CurrentValue = _CurrentList;
            Editor.Saving += Editor_Saving;
            IEntityContextBuilder builder = EntityResolver.Current.Resolve<IEntityContextBuilder>();
            _EntityQueryable = builder.GetContext(Metadata.ClrType.GetGenericArguments()[0]);
        }

        protected override bool ValidateCore()
        {
            return true;
        }

        void _Clear_Click(object sender, RoutedEventArgs e)
        {
            _AddList.Clear();
            _RemoveList.Clear();
            _RemoveList.AddRange(_OriginList);
        }

        void _Remove_Click(object sender, RoutedEventArgs e)
        {
            IEntity entity = (IEntity)_Selector.SelectedItem;
            if (_OriginList.Contains(entity))
                _RemoveList.Add(entity);
            else
                _AddList.Remove(entity);
            _CurrentList.Remove(entity);
        }

        async void _Add_Click(object sender, RoutedEventArgs e)
        {
            dynamic controller = EntityRouter.Routers.GetController(EntityAnalyzer.GetMetadata(Metadata.ClrType.GetGenericArguments()[0]));
            EntityMultipleSelector selector = (EntityMultipleSelector)await controller.GetMultipleSelector();
            if (selector.ShowDialog() == true)
            {
                var items = selector.SelectedEntity.Except(_CurrentList).ToArray();
                foreach (var item in items)
                {
                    if (_OriginList.Contains(item))
                        _RemoveList.Remove(item);
                    else
                        _AddList.Add(item);
                    _CurrentList.Add(item);
                }
            }
        }

        void Editor_Saving(object sender, RoutedEventArgs e)
        {
            dynamic list = OriginValue;
            foreach (var item in _AddList)
                list.Add(Convert.ChangeType(item, Metadata.ClrType));
            foreach (var item in _RemoveList)
                list.Remove(Convert.ChangeType(item, Metadata.ClrType));
        }
    }
}
