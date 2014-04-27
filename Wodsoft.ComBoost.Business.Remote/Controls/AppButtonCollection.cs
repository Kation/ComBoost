using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Wodsoft.ComBoost.Business.Controls
{
    public sealed class AppButtonCollection : ObservableCollection<AppButton>
    {
        protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

            base.OnCollectionChanged(e);
        }
    }
}
