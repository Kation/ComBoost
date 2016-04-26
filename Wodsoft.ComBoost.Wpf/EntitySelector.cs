using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Wodsoft.ComBoost.Wpf
{
    public class EntitySelector : EntityViewer
    {
        private ListView _ItemsView;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _ItemsView = (ListView)GetTemplateChild("PART_ItemsView");
            _ItemsView.SelectionMode = SelectionMode.Single;
        }

        public IEntity SelectedEntity { get { return (IEntity)_ItemsView.SelectedItem; } }
    }
}
