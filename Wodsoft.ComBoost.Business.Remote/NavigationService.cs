using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wodsoft.ComBoost.Business.Controls;

namespace Wodsoft.ComBoost.Business
{
    public class NavigationService
    {
        private WorkFrame Frame;
        private Stack<WorkPage> History;

        internal NavigationService(WorkFrame frame)
        {
            Frame = frame;
            History = new Stack<WorkPage>();
        }

        public void NavigateTo(WorkPage page)
        {
            WorkPage old = Frame.Content;
            Frame.Content = page;
            if (old != null)
            {
                History.Push(old);
                old.BaseNavigateFrom(page);                
            }
            if (page != null)
                page.BaseNavigateTo(old);
            if (Navigated != null)
                Navigated(old, page);
        }

        public bool CanGoBack
        {
            get
            {
                return History.Count > 0;
            }
        }

        public void GoBack()
        {
            if (!CanGoBack)
            {
                NavigateTo(null);
                return;
            }
            WorkPage old = Frame.Content;
            Frame.Content = History.Pop();
            if (old !=null)
                old.BaseNavigateFrom(Frame.Content);
            Frame.Content.BaseNavigateTo(old);
            if (Navigated != null)
                Navigated(old, Frame.Content);
        }

        public event NavigatedEventHandler Navigated;
    }
}
