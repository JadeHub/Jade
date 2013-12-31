using System.Windows.Media;

namespace JadeControls.Docking
{
    public class PaneViewModel : NotifyPropertyChanged
    {
        #region Data

        private string _title;
        private string _contentId;
        private bool _selected;
        private ImageSource _iconSource;

        #endregion

        public PaneViewModel()
        {
        }

        #region Properties

        public string Title
        {
            get { return _title; }
            set
            {
                if (value != _title)
                {
                    _title = value;
                    OnPropertyChanged("Title");
                }
            }
        }

        public string ContentId
        {
            get { return _contentId; }
            set
            {
                if (_contentId != value)
                {
                    _contentId = value;
                    OnPropertyChanged("ContentId");
                }
            }
        }

        public bool IsSelected
        {
            get { return _selected; }
            set
            {
                if (_selected != value)
                {
                    _selected = value;
                    OnPropertyChanged("IsSelected");
                }
            }
        }

        public ImageSource IconSource
        {
            get { return _iconSource; }
            set
            {
                if (_iconSource != value)
                {
                    _iconSource = value;
                    OnPropertyChanged("IconSource");
                }
            }
        }

        #endregion
    }
}
