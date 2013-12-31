
namespace JadeControls.Docking
{
    public class ToolPaneViewModel : PaneViewModel
    {
        #region Data

        private bool _visible;

        #endregion

        public ToolPaneViewModel()
        {
            IsVisible = true;
        }

        #region Properties

        public bool IsVisible
        {
            get { return _visible; }
            set
            {
                if (_visible != value)
                {
                    _visible = value;
                    OnPropertyChanged("IsVisible");
                }
            }
        }

        #endregion
    }
}
