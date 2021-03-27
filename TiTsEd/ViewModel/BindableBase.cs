﻿using System.Collections.Generic;
using System.ComponentModel;
using TiTsEd.Model;

namespace TiTsEd.ViewModel
{
    public abstract class BindableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (null != PropertyChanged) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnSavePropertyChanged(propertyName);
            return true;
        }

        protected bool SetProperty<T>(ref T storage, T value, string propertyName = null)
        {
            return SetField(ref storage, value, propertyName);
        }

        public virtual bool SetValue(AmfObject obj, object key, object value, string propertyName = null)
        {
            if (AreSame(obj[key], value)) return false;

            obj[key] = value;
            OnSavePropertyChanged(propertyName);
            return true;
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { SetField(ref name, value, "Name"); }
        }

        static bool AreSame(object x, object y)
        {
            if (x == null) return (y == null);
            if (y == null) return false;
            return object.Equals(x, y);
        }

        protected virtual void OnSavePropertyChanged(string propertyName = null)
        {
            OnPropertyChanged(propertyName);
            VM.Instance.NotifySaveRequiredChanged(true);
        }
    }
}
