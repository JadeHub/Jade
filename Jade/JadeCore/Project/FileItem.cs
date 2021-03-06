﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JadeUtils.IO;

namespace JadeCore.Project
{
    public class FileItem : IFileItem
    {
        #region Data

        private JadeUtils.IO.IFileHandle _file;
        private ItemType _type;

        #endregion

        #region Constructor

        public FileItem(JadeUtils.IO.IFileHandle file)
        {
            _file = file;
            SetFileType();
        }

        private void SetFileType()
        {
            string ext = _file.Path.Extention.ToLower();
            if (ext == ".c" || ext == ".cc" || ext == ".cpp")
            {
                _type = ItemType.CppSourceFile;
            }
            else if (ext == ".h")
            {
                _type = ItemType.CppHeaderFile;
            }
            else
            {
                _type = ItemType.Unknown;
            }
        }

        #endregion

        #region Public Properties

        public string ItemName { get { return Path.Str; } }
        public ItemType Type { get { return _type; } }
        public FilePath Path { get { return _file.Path; } }
        public JadeUtils.IO.IFileHandle Handle { get { return _file; } }

        #endregion

        public override string ToString()
        {
            return _file.ToString();
        }
    }
}
