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
        private ProjectParseThreads _parseThread;
        private Project.IProject _project;
	        
	    #endregion
	
	    #region Constructor
	
	    public SourceDocument(IEditorController controller, ITextDocument document, Project.IProject proj)
	    {
	        _controller = controller;
	        _document = document;
            _project = proj;
            _parseThread = new ProjectParseThreads(proj, proj.IndexBuilder, _controller, delegate(FilePath fp) { });
	        _parseThread.Run = true;
	        _controller.ActiveDocumentChanged += OnActiveDocumentChanged;
	    }
	
	    void OnActiveDocumentChanged(IEditorDoc newValue, IEditorDoc oldValue)
	    {

	        //_parseThread.HighPriority = args.Document == this;
	        //_controller.ActiveDocumentChanged = OnActiveDocumentChanged;
	    }
	
	    public void Dispose()
	    {
	        _parseThread.Dispose();
	    }
	
	    #endregion
	
	    #region IEditorDoc implementation
	
	    /*public event EventHandler OnIndexUpdated;
	
	    private void RaiseOnIndexUpdated()
	    {
	        EventHandler handler = OnIndexUpdated;
	        if (handler != null)
	            handler(this, EventArgs.Empty);
	    }*/
	
	    public event EventHandler OnClosing;
	
	    private void RaiseOnClosing()
	    {
	        EventHandler handler = OnClosing;
	        if (handler != null)
	            handler(this, EventArgs.Empty);
	    }
	                
	    public void Close()
	    {
	        RaiseOnClosing();
	    }
	
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
