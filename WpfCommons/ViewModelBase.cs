namespace WpfCommons
{
    public class ViewModelBase : NotifyObject
    {
        private bool _isLoading;

        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }

            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged(() => IsLoading);
                }
            }
        }

        private string _isLoadingInfoText;

        public string IsLoadingInfoText
        {
            get
            {
                return _isLoadingInfoText;
            }

            set
            {
                if (_isLoadingInfoText != value)
                {
                    _isLoadingInfoText = value;
                    OnPropertyChanged(() => IsLoadingInfoText);
                }
            }
        }
    }
}
