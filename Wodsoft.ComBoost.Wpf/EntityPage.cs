using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Wodsoft.ComBoost.Wpf
{
    public abstract class EntityPage : Page
    {
        protected EntityPage(EntityController controller)
        {
            if (controller == null)
                throw new ArgumentNullException("controller");
            Controller = controller;
        }

        public EntityController Controller { get; private set; }
    }
}
