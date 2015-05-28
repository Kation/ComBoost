using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace System.ComponentModel
{
    /// <summary>
    /// Notify object.
    /// </summary>
    public abstract class NotifyBase : INotifyPropertyChanged, INotifyPropertyChanging
    {
        private Dictionary<string, object> data;

        /// <summary>
        /// Initialize notify object.
        /// </summary>
        public NotifyBase()
        {
            data = new Dictionary<string, object>();
        }

        /// <summary>
        /// Get the value of property.
        /// </summary>
        /// <param name="propertyName">Name of property.</param>
        /// <returns></returns>
        protected object GetValue([CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
                throw new ArgumentNullException("propertyName");
            object value = null;
            if (data.ContainsKey(propertyName))
                value = data[propertyName];
            if (value == null && this.GetType().GetProperty(propertyName).PropertyType.IsValueType)
                return Activator.CreateInstance(this.GetType().GetProperty(propertyName).PropertyType);
            return value;
        }

        /// <summary>
        /// Get a property is setting value enabled.
        /// </summary>
        /// <param name="propertyName">Name of property.</param>
        /// <returns>true if property can be set.</returns>
        protected virtual bool CanSetValue(string propertyName)
        {
            return true;
        }

        /// <summary>
        /// Set the value of property.
        /// </summary>
        /// <param name="propertyName">Name of property.</param>
        /// <param name="value">Value.</param>
        protected void SetValue(object value, [CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
                throw new ArgumentNullException("propertyName");
            if (!CanSetValue(propertyName))
                throw new InvalidOperationException("Property \"" + propertyName + "\" can not modify.");
            OnPropertyChanging(propertyName);
            if (!data.ContainsKey(propertyName))
                data.Add(propertyName, value);
            else
                data[propertyName] = value;
            OnPropertyChanged(propertyName);
        }

        /// <summary>
        /// Set the value of property without notify.
        /// </summary>
        /// <param name="propertyName">Name of property.</param>
        /// <param name="value">Value.</param>
        protected void SetValueWithoutNotify(object value, [CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
                throw new ArgumentNullException("propertyName");
            if (!data.ContainsKey(propertyName))
                data.Add(propertyName, value);
            else
                data[propertyName] = value;
        }

        /// <summary>
        /// Trigger event when property changed.
        /// </summary>
        /// <param name="propertyName">Name of property.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            if (propertyName == null)
                throw new ArgumentNullException("propertyName");
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Trigger event when property changing.
        /// </summary>
        /// <param name="propertyName">Name of property.</param>
        protected void OnPropertyChanging(string propertyName)
        {
            if (propertyName == null)
                throw new ArgumentNullException("propertyName");
            if (PropertyChanging != null)
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
        }

        /// <summary>
        /// Property changed event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Property changing event.
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging;
    }
}
