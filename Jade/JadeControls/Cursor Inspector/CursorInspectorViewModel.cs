using LibClang;

namespace JadeControls.CursorInspector
{
    public class CursorInspectorPaneViewModel : JadeControls.Docking.ToolPaneViewModel
    {
        private CursorViewModel _cursor;

        public CursorInspectorPaneViewModel()
        {
            Title = "Cursor Inspector";
            ContentId = "CursorInspectorPaneViewModel";
        }

        public void SetCursor(Cursor c)
        {
            _cursor = new CursorViewModel(c);
            OnPropertyChanged("CurrentCursor");
        }

        public CursorViewModel CurrentCursor
        {
            get { return _cursor; }
        }
    }
}
