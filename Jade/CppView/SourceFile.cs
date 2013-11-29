﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CppView
{
    using JadeUtils.IO;

    public interface ICodeFile
    {
        FilePath Path { get; }
    }

    public interface ISourceFile : ICodeFile
    {        
        LibClang.TranslationUnit TranslationUnit { get; set; }
    }

    public class SourceFile : ISourceFile
    {
        private FilePath _path;
        private LibClang.TranslationUnit _tu; 

        public SourceFile(string path)
        {
            _path = JadeUtils.IO.FilePath.Make(path);            
        }

        public SourceFile(FilePath path)
        {
            _path = path;
        }

        #region Properties

        public JadeUtils.IO.FilePath Path
        {
            get { return _path; }
        }

        public LibClang.TranslationUnit TranslationUnit 
        {
            get 
            {
                return _tu;
            }

            set
            {
                _tu = value;
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
