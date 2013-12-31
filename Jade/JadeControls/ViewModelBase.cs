using System;
using System.ComponentModel;

namespace JadeControls
{
    public abstract class NotifyPropertyChanged : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
    /*
    public abstract class DockingToolViewModel : JadeControls.NotifyPropertyChanged
    {
        abstract public string DisplayName { get; }
    }
     */ 
}
