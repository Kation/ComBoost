using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

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
            foreach (var property in this.GetType().GetProperties())
            {
                if (data.ContainsKey(property.Name))
                    continue;
                if (property.GetGetMethod() != null && property.GetSetMethod() != null)
                    if (property.PropertyType.IsValueType)
                        data.Add(property.Name, Activator.CreateInstance(property.PropertyType));
                    else
                        data.Add(property.Name, null);
            }
        }

        /// <summary>
        /// Get the value of property.
        /// </summary>
        /// <param name="propertyName">Name of property.</param>
        /// <returns></returns>
        protected object GetValue(string propertyName)
        {
            if (!data.ContainsKey(propertyName))
                throw new ArgumentException("Property \"" + propertyName + "\" doesn't exists.");
            object value = data[propertyName];
            if (value == null && this.GetType().GetProperty(propertyName).PropertyType.IsValueType)
                return Activator.CreateInstance(this.GetType().GetProperty(propertyName).PropertyType);
            return value;
        }

        /// <summary>
        /// Get a property is setting value enabled.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected virtual bool CanSetValue(string propertyName)
        {
            return true;
        }

        /// <summary>
        /// Set the value of property.
        /// </summary>
        /// <param name="propertyName">Name of property.</param>
        /// <param name="value">Value.</param>
        protected void SetValue(string propertyName, object value)
        {
            if (!data.ContainsKey(propertyName))
                throw new ArgumentException("Property \"" + propertyName + "\" doesn't exists.");
            if (!CanSetValue(propertyName))
                throw new InvalidOperationException("Property \"" + propertyName + "\" can not modify.");
            OnPropertyChanging(propertyName);
            data[propertyName] = value;
            OnPropertyChanged(propertyName);
        }

        /// <summary>
        /// Set the value of property without notify.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        protected void SetValueWithoutNotify(string propertyName, object value)
        {
            if (!data.ContainsKey(propertyName))
                throw new ArgumentException("Property \"" + propertyName + "\" doesn't exists.");
            data[propertyName] = value;
        }

        /// <summary>
        /// Trigger event when property changed.
        /// </summary>
        /// <param name="propertyName">Name of property.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Trigger event when property changing.
        /// </summary>
        /// <param name="propertyName">Name of property.</param>
        protected void OnPropertyChanging(string propertyName)
        {
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
