using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Wodsoft.ComBoost.Business.Controls
{
    public abstract class EditorItemFactory
    {
        public abstract EditorItem GetEditorItem(PropertyInfo property);
    }
}
