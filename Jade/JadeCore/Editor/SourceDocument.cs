﻿using JadeUtils.IO;
using System;
using System.Threading;
using System.IO;
	
namespace JadeCore.Editor
{
    
        
	public interface ISourceDocument : JadeCore.IEditorDoc
	{
//	    event EventHandler OnIndexUpdated;
	//    CppCodeBrowser.IProjectIndex ProjectIndex { get; }
	}
                
	public class SourceDocument : ISourceDocument
	{
	    #region Data
	
	    private IEditorController _controller;
	    private ITextDocument _document;
        private Project.IProject _project;
        private DocChangeTracker _changeTracker;
	        
	    #endregion
	
	    #region Constructor
	
	    public SourceDocument(IEditorController controller, ITextDocument document, Project.IProject proj)
	    {
	        _controller = controller;
	        _document = document;
            _project = proj;
            _changeTracker = new DocChangeTracker(this, _project.IndexBuilder);
	    }
	
	    #endregion
	
	    #region IEditorDoc implementation
		
	    public void Save()
	    {
	        _document.Save(_document.File);
	    }
	    
        public Project.IProject Project
        {
            get { return _project; }
        }

	    public ITextDocument TextDocument { get { return _document; } }
	        
	    public string Name
	    {
	        get { return _document.Name; }
	    }
	
	    public IFileHandle File
	    {
	        get { return _document.File; }
	    }
	
	    public bool Modified
	    {
	        get
	        {
	            return _document.Modified;
	        }            
	    }
	
	    #endregion
	
	    #region IEditorSourceDocument implementation
	/*
	    public CppCodeBrowser.IProjectIndex ProjectIndex
	    {
	        get { return _indexBuilder.Index; }
	    }
	*/
	    #endregion
	}
}
