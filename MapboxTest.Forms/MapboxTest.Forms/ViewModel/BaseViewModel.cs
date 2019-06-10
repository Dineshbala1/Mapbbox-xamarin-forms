using System.ComponentModel;
using System.Runtime.CompilerServices;
using MapboxTest.Forms.Service;
using Xamarin.Forms;

namespace MapboxTest.Forms.ViewModel
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        private bool _isBusy;
        private string _pageTitle;
        public event PropertyChangedEventHandler PropertyChanged;

        protected BaseViewModel()
        {
            PopupService = DependencyService.Get<IPopupService>();
            MapBoxOfflineService = DependencyService.Get<IMapboxOfflineService>();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        public string PageTitle
        {
            get => _pageTitle;
            set
            {
                _pageTitle = value;
                OnPropertyChanged();
            }
        }

        public IPopupService PopupService { get; }

        public IMapboxOfflineService MapBoxOfflineService { get; }
    }
}