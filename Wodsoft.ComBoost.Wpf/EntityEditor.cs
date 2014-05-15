﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Wodsoft.ComBoost.Wpf
{
    [TemplatePart(Name = "PART_Save", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Cancel", Type = typeof(Button))]
    public class EntityEditor : EntityPage
    {
        static EntityEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EntityEditor), new FrameworkPropertyMetadata(typeof(EntityEditor)));
        }

        public EntityEditor(EntityController controller)
            : base(controller)
        {
            Loaded += EntityEditor_Loaded;
            Unloaded += EntityEditor_Unloaded;
        }

        private void EntityEditor_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void EntityEditor_Loaded(object sender, RoutedEventArgs e)
        {
            if (NavigationService == null)
                throw new NotSupportedException("EntityEditor must place in a navigator.");
            if (Model.Item.Index == default(Guid))
                Title = "创建" + Model.Metadata.Name;
            else
                Title = "编辑" + Model.Metadata.Name;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Button save = (Button)GetTemplateChild("PART_Save");
            save.Click += Save_Click;
            Button cancel = (Button)GetTemplateChild("PART_Cancel");
            cancel.Click += Cancel_Click;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Cancel();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        public EntityEditModel Model { get { return (EntityEditModel)GetValue(ModelProperty); } set { SetValue(ModelProperty, value); } }
        public static readonly DependencyProperty ModelProperty = DependencyProperty.Register("Model", typeof(EntityEditModel), typeof(EntityEditor));

        public event RoutedEventHandler PreviewSave { add { AddHandler(PreviewSaveEvent, value); } remove { RemoveHandler(PreviewSaveEvent, value); } }
        public static readonly RoutedEvent PreviewSaveEvent = EventManager.RegisterRoutedEvent("PreviewSave", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(EntityEditor));

        public event RoutedEventHandler Saving { add { AddHandler(SavingEvent, value); } remove { RemoveHandler(SavingEvent, value); } }
        public static readonly RoutedEvent SavingEvent = EventManager.RegisterRoutedEvent("Saving", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(EntityEditor));

        public event RoutedEventHandler Saved { add { AddHandler(SavedEvent, value); } remove { RemoveHandler(SavedEvent, value); } }
        public static readonly RoutedEvent SavedEvent = EventManager.RegisterRoutedEvent("Saved", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(EntityEditor));

        public event RoutedEventHandler Canceled { add { AddHandler(CanceledEvent, value); } remove { RemoveHandler(CanceledEvent, value); } }
        public static readonly RoutedEvent CanceledEvent = EventManager.RegisterRoutedEvent("Canceled", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(EntityEditor));

        public void Save()
        {
            RoutedEventArgs pe = new RoutedEventArgs(PreviewSaveEvent, this);
            RaiseEvent(pe);
            if (pe.Handled)
                return;
            RoutedEventArgs ie = new RoutedEventArgs(SavingEvent, this);
            RaiseEvent(ie);
            RoutedEventArgs de = new RoutedEventArgs(SavedEvent, this);
            RaiseEvent(de);
        }

        public void Cancel()
        {
            RoutedEventArgs e = new RoutedEventArgs(CanceledEvent, this);
            RaiseEvent(e);
            NavigationService.GoBack();
        }
    }
}
