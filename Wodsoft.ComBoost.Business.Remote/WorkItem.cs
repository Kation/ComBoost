using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Wodsoft.ComBoost.Business.Controls;

namespace Wodsoft.ComBoost.Business
{
    public class WorkItem
    {
        public WorkItem()
        {
            IconSize = new Size(64, 64);
        }

        public string Name { get; set; }

        public Visual Icon { get; set; }

        public string Description { get; set; }

        public int Order { get; set; }

        public bool SmallSize { get; set; }

        public string[] Roles { get; set; }

        public GetWorkPage GetWorkPage { get; set; }

        public Size IconSize { get; set; }
    }

    public delegate WorkPage GetWorkPage();
}
