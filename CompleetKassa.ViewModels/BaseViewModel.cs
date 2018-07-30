using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CompleetKassa.ViewModels.Commands;

namespace CompleetKassa.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected virtual void SetProperty<T>(ref T member, T val, [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(member, val)) return;

            member = val;
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

		public BaseViewModel (string name, string color,string imageloc)
		{
			Name = name;
            Color = color;
            ImageLoc = imageloc;
		}

		public ICommand OnClosePageCommand { get; set; }

		public string Name { get; set; }
        public string Color { get; set; }

        public string ImageLoc{ get; set; }
    }
}
