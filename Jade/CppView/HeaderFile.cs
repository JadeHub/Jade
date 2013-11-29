using System;
using System.Collections.Generic;
using System.Linq;

namespace CppView
{
    using JadeUtils.IO;

    public interface IHeaderFile : ICodeFile
    {
        void MentionedIn(LibClang.TranslationUnit tu);
        LibClang.TranslationUnit DefaultTranslationUnit { get; }        
    }

    public class HeaderFile : IHeaderFile
    {
        private FilePath _path;
        private HashSet<LibClang.TranslationUnit> _tus;
                
        public HeaderFile(FilePath path)
        {
            _path = path;
            _tus = new HashSet<LibClang.TranslationUnit>();
        }

        public void MentionedIn(LibClang.TranslationUnit tu)
        {
            _tus.Add(tu);
        }

        #region Properties

        public JadeUtils.IO.FilePath Path
        {
            get { return _path; }
        }

        public LibClang.TranslationUnit DefaultTranslationUnit 
        { 
            get 
            {
                if (_tus.Count > 0)
                {
                    return _tus.First();
                }
                return null;
            }
        }

        #endregion

        public override string ToString()
        {
            return _path.ToString();
        }

        public override bool Equals(object obj)
        {
            return _path.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _path.GetHashCode();
        }
    }
}
