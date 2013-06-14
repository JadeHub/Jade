using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;

namespace JadeControls.Workspace
{
    internal class AddProjectFileViewModel : ViewModelBase, IDataErrorInfo
    {
        private string _location;
        private string _fileNameText;

        public AddProjectFileViewModel()
        {
            _fileNameText = "";
        }

        #region Public Properties

        public string Location
        {
            get { return _location; }
            set 
            {
                if (value != _location)
                {
                    _location = value;
                    OnPropertyChanged("Location");
                }
            }
        }

        public string FileNameText
        {
            get { return _fileNameText; }
            set
            {
                if (value != _fileNameText)
                {
                    _fileNameText = value;
                    OnPropertyChanged("FileNameText");
                }
            }
        }

        public override string DisplayName { get { return ""; } }

        public IEnumerable<string> Paths
        {
            get
            {
                if (!IsValid) return null;

                List<string> result = new List<string>();

                if(_fileNameText.Contains('\"'))
                {
                    string[] names = _fileNameText.Split(new Char[] {'\"'}, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string name in names)
                    {
                        result.Add(System.IO.Path.Combine(new string[] { _location, name }));
                    }
                }
                else
                {
                    result.Add(System.IO.Path.Combine(new string[] { _location, _fileNameText }));
                }

                return result;
            }
        }

        public bool IsValid
        {
            get
            {
                return !string.IsNullOrEmpty(_fileNameText);
            }
        }

        #endregion

        #region IDataErrorInfo Members

        string IDataErrorInfo.Error 
        { 
            get 
            { 
                return null; 
            } 
        }

        string IDataErrorInfo.this[string propertyName]
        {
            get 
            {
                string error = null;
                if (propertyName == "FileNameText" && _fileNameText.Length == 0)
                    error = "File names field cannot be empty.";
                
                OnPropertyChanged("IsValid");
                CommandManager.InvalidateRequerySuggested();
                return error;
            }
        }
                
        #endregion // IDataErrorInfo Members
    }
}
