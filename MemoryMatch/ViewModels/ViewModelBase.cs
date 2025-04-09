using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MemoryMatch.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            bool valueChanged = !Equals(field, value);
            
            if (valueChanged)
            {
                field = value;
                OnPropertyChanged(propertyName);
            }
            
            return valueChanged;
        }
    }
} 