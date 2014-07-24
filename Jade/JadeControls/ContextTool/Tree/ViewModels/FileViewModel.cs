using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using JadeUtils.IO;
using LibClang;
using JadeCore.CppSymbols;

namespace JadeControls.ContextTool
{
    public class FileViewModel : TreeItemBase
    {
        private FilePath _path;
                
        public FileViewModel(FilePath path, TranslationUnit tu)
            :base(null, path.FileName)
        {
            _path = path;

            tu.Cursor.VisitChildren(delegate(Cursor cursor, Cursor p)
                                    {
                                        if(CanDisplayCursor(cursor))
                                        {
                                            AddCursor(cursor);
                                        }
                                        return Cursor.ChildVisitResult.Continue;
                                    });
            OnPropertyChanged("Children");
        }

        private bool CanDisplayCursor(Cursor c)
        {
            if(!Factory.CanMakeTreeItem(c))
            { 
                return false; 
            }

            if(c.Location.File == null ||
                _path != FilePath.Make(c.Location.File.Name))
            {
                return false;
            }

            return true;
        }

        private void AddCursor(Cursor c)
        {
            ITreeItem item = Factory.MakeTreeItem(this, c);
            if(item == null) return;
            Children.Add(item);
        }

        public override string TypeChar { get { return ""; } }
    }
}
