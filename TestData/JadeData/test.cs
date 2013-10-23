using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;

namespace JadeGui.ViewModels
{
    using JadeData;

   
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IJadeViewModel
    {
        #region Data

        private JadeCore.IWorkspaceManager _workspaceManager;
        private Commands.NewWorkspace _newWorkspaceCmd;
        private Commands.OpenWorkspace _openWorkspaceCmd;
        private Commands.SaveWorkspace _saveWorkspaceCmd;
        private Commands.SaveAsWorkspace _saveAsWorkspaceCmd;
        private Commands.CloseWorkspace _closeWorkspaceCmd;

        private ICommand _exitCommand;

        #endregion

        public JadeViewModel()
        {
            _workspaceManager = new WorkspaceManager();
            _workspaceManager.WorkspaceOpened += delegate { OnPropertyChanged("Workspace"); };

            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();
            _newWorkspaceCmd = new Commands.NewWorkspace(this);
            _openWorkspaceCmd = new Commands.OpenWorkspace(this);
            _saveWorkspaceCmd = new Commands.SaveWorkspace(this);
            _saveAsWorkspaceCmd = new Commands.SaveAsWorkspace(this);
            _closeWorkspaceCmd = new Commands.CloseWorkspace(this);
        }

        #region EditorControl

        private JadeControls.EditorControl.ViewModel.EditorControlViewModel _editorModel;

        public JadeCore.ViewModels.IEditorViewModel Editor
        {
            get { return _editorModel; }
        }

        #endregion

        #region RequestClose [event]

        /// <summary>
        /// Raised to request ui to close
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

        #region Workspace Tree

        public JadeCore.ViewModels.IWorkspaceViewModel Workspace
        {
            get { return _workspaceManager.ViewModel; }
        }

        #endregion

        #region Commands

        public ICommand CloseWorkspaceCommand { get { return _closeWorkspaceCmd.Command; } }
        public ICommand NewWorkspaceCommand { get { return _newWorkspaceCmd.Command; } }
        public ICommand OpenWorkspaceCommand { get { return _openWorkspaceCmd.Command;}}
        public ICommand SaveWorkspaceCommand { get { return _saveWorkspaceCmd.Command; } }
        public ICommand SaveAsWorkspaceCommand { get { return _saveAsWorkspaceCmd.Command; } }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(delegate { OnRequestClose(); });
                }
                return _exitCommand;
            }
        }

        #endregion

        #region public Methods

        public bool OnExit()
        {
            if (_workspaceManager.RequiresSave)
            {
                return _workspaceManager.SaveOrDiscardWorkspace();
            }
            return true;
        }
       
        #endregion

        public JadeCore.IWorkspaceManager WorkspaceManager
        {
            get
            {
                return _workspaceManager;
            }
        }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class SaveWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path = _viewModel.Workspace.Path;

            if(path == null || path.Length == 0)
            {
                if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    return;                
            }
            _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.RequiresSave; } }
    }

    internal class SaveAsWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveAsWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path;

            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
            }
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.WorkspaceOpen; } }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class OpenWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command 
        { 
            get 
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            } 
        }

        public OpenWorkspace(JadeCore.ViewModels.IJadeViewModel vm)        
        {
            _viewModel = vm;     
        }

        private void OnCommand()
        {
            string path;
            if (JadeCore.GuiUtils.PromptOpenFile(".jws", "Jade Workspace files (.jws)|*.jws", true, out path) == false)
            {
                return;
            }

            try
            {
                _viewModel.WorkspaceManager.OpenWorkspace(path);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening workspace. " + e.ToString());
            }            
        }

        private bool CanDoCommand { get { return true; } }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class NewWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public NewWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string name;

            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                try
                {
                    _viewModel.WorkspaceManager.NewWorkspace(name, "");
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        private bool CanDoCommand 
        { 
            get { return true; } 
        }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class CloseWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public CloseWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            _viewModel.WorkspaceManager.CloseWorkspace();            
        }

        private bool CanDoCommand
        {
            get
            {
                return _viewModel.WorkspaceManager.WorkspaceOpen;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JadeData.Persistence.Workspace
{
    public static class Reader
    {
        private static JadeData.Project.IProject MakeProject(string workspaceDir, ProjectType xml)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                path = System.IO.Path.Combine(workspaceDir, path);
            }
            return Persistence.Project.Reader.Read(path);
        }

        private static JadeData.Workspace.IFolder MakeFolder(string workspaceDir, FolderType xml)
        {
            JadeData.Workspace.IFolder result = new JadeData.Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(workspaceDir, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(workspaceDir, p));
            }
            return result;
        }

        static public JadeData.Workspace.IWorkspace Read(string path)
        {
            WorkspaceType xml;
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceType));
            TextReader tr = new StreamReader(path);
            try
            {                
                xml = (WorkspaceType)serializer.Deserialize(tr);                
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }

            JadeData.Workspace.IWorkspace result = new JadeData.Workspace.Workspace(xml.Name, path);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result.Directory, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(result.Directory, p));
            }

            return result;
        }
    }

    public static class Writer
    {
        static private ProjectType MakeProject(JadeData.Workspace.ProjectItem proj, string workspaceDir)
        {
            ProjectType result = new ProjectType();

            string absPath = proj.Path;

            if (System.IO.Path.IsPathRooted(absPath) == false)
            {
                absPath = System.IO.Path.Combine(workspaceDir, absPath);
            }

            result.Path = proj.Path;
            Persistence.Project.Writer.Write(proj, absPath);
            return result;
        }

        static private FolderType MakeFolder(JadeData.Workspace.IFolder folder, string workspaceDir)
        {
            FolderType result = new FolderType();
            result.Name = folder.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for(int i=0;i<projTemps.Count;i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            return result;
        }

        static private string GetProjectPath(JadeData.Workspace.IWorkspace workspace, JadeData.Project.IProject proj)
        {
            return "";
        }

        static public string Write(JadeData.Workspace.IWorkspace workspace, string path)
        {
            string workspaceDir = System.IO.Path.GetDirectoryName(path);

            JadeData.Workspace.IFolder folder = workspace;
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for (int i = 0; i < projTemps.Count; i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            System.Xml.XmlDocument doc = new XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            System.IO.TextWriter stream = new System.IO.StreamWriter(path); 
            try
            {                    
                serializer.Serialize(stream, result);
                stream.Close();
                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;

namespace JadeGui.ViewModels
{
    using JadeData;

   
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IJadeViewModel
    {
        #region Data

        private JadeCore.IWorkspaceManager _workspaceManager;
        private Commands.NewWorkspace _newWorkspaceCmd;
        private Commands.OpenWorkspace _openWorkspaceCmd;
        private Commands.SaveWorkspace _saveWorkspaceCmd;
        private Commands.SaveAsWorkspace _saveAsWorkspaceCmd;
        private Commands.CloseWorkspace _closeWorkspaceCmd;

        private ICommand _exitCommand;

        #endregion

        public JadeViewModel()
        {
            _workspaceManager = new WorkspaceManager();
            _workspaceManager.WorkspaceOpened += delegate { OnPropertyChanged("Workspace"); };

            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();
            _newWorkspaceCmd = new Commands.NewWorkspace(this);
            _openWorkspaceCmd = new Commands.OpenWorkspace(this);
            _saveWorkspaceCmd = new Commands.SaveWorkspace(this);
            _saveAsWorkspaceCmd = new Commands.SaveAsWorkspace(this);
            _closeWorkspaceCmd = new Commands.CloseWorkspace(this);
        }

        #region EditorControl

        private JadeControls.EditorControl.ViewModel.EditorControlViewModel _editorModel;

        public JadeCore.ViewModels.IEditorViewModel Editor
        {
            get { return _editorModel; }
        }

        #endregion

        #region RequestClose [event]

        /// <summary>
        /// Raised to request ui to close
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

        #region Workspace Tree

        public JadeCore.ViewModels.IWorkspaceViewModel Workspace
        {
            get { return _workspaceManager.ViewModel; }
        }

        #endregion

        #region Commands

        public ICommand CloseWorkspaceCommand { get { return _closeWorkspaceCmd.Command; } }
        public ICommand NewWorkspaceCommand { get { return _newWorkspaceCmd.Command; } }
        public ICommand OpenWorkspaceCommand { get { return _openWorkspaceCmd.Command;}}
        public ICommand SaveWorkspaceCommand { get { return _saveWorkspaceCmd.Command; } }
        public ICommand SaveAsWorkspaceCommand { get { return _saveAsWorkspaceCmd.Command; } }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(delegate { OnRequestClose(); });
                }
                return _exitCommand;
            }
        }

        #endregion

        #region public Methods

        public bool OnExit()
        {
            if (_workspaceManager.RequiresSave)
            {
                return _workspaceManager.SaveOrDiscardWorkspace();
            }
            return true;
        }
       
        #endregion

        public JadeCore.IWorkspaceManager WorkspaceManager
        {
            get
            {
                return _workspaceManager;
            }
        }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class SaveWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path = _viewModel.Workspace.Path;

            if(path == null || path.Length == 0)
            {
                if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    return;                
            }
            _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.RequiresSave; } }
    }

    internal class SaveAsWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveAsWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path;

            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
            }
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.WorkspaceOpen; } }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class OpenWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command 
        { 
            get 
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            } 
        }

        public OpenWorkspace(JadeCore.ViewModels.IJadeViewModel vm)        
        {
            _viewModel = vm;     
        }

        private void OnCommand()
        {
            string path;
            if (JadeCore.GuiUtils.PromptOpenFile(".jws", "Jade Workspace files (.jws)|*.jws", true, out path) == false)
            {
                return;
            }

            try
            {
                _viewModel.WorkspaceManager.OpenWorkspace(path);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening workspace. " + e.ToString());
            }            
        }

        private bool CanDoCommand { get { return true; } }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class NewWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public NewWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string name;

            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                try
                {
                    _viewModel.WorkspaceManager.NewWorkspace(name, "");
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        private bool CanDoCommand 
        { 
            get { return true; } 
        }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class CloseWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public CloseWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            _viewModel.WorkspaceManager.CloseWorkspace();            
        }

        private bool CanDoCommand
        {
            get
            {
                return _viewModel.WorkspaceManager.WorkspaceOpen;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JadeData.Persistence.Workspace
{
    public static class Reader
    {
        private static JadeData.Project.IProject MakeProject(string workspaceDir, ProjectType xml)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                path = System.IO.Path.Combine(workspaceDir, path);
            }
            return Persistence.Project.Reader.Read(path);
        }

        private static JadeData.Workspace.IFolder MakeFolder(string workspaceDir, FolderType xml)
        {
            JadeData.Workspace.IFolder result = new JadeData.Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(workspaceDir, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(workspaceDir, p));
            }
            return result;
        }

        static public JadeData.Workspace.IWorkspace Read(string path)
        {
            WorkspaceType xml;
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceType));
            TextReader tr = new StreamReader(path);
            try
            {                
                xml = (WorkspaceType)serializer.Deserialize(tr);                
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }

            JadeData.Workspace.IWorkspace result = new JadeData.Workspace.Workspace(xml.Name, path);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result.Directory, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(result.Directory, p));
            }

            return result;
        }
    }

    public static class Writer
    {
        static private ProjectType MakeProject(JadeData.Workspace.ProjectItem proj, string workspaceDir)
        {
            ProjectType result = new ProjectType();

            string absPath = proj.Path;

            if (System.IO.Path.IsPathRooted(absPath) == false)
            {
                absPath = System.IO.Path.Combine(workspaceDir, absPath);
            }

            result.Path = proj.Path;
            Persistence.Project.Writer.Write(proj, absPath);
            return result;
        }

        static private FolderType MakeFolder(JadeData.Workspace.IFolder folder, string workspaceDir)
        {
            FolderType result = new FolderType();
            result.Name = folder.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for(int i=0;i<projTemps.Count;i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            return result;
        }

        static private string GetProjectPath(JadeData.Workspace.IWorkspace workspace, JadeData.Project.IProject proj)
        {
            return "";
        }

        static public string Write(JadeData.Workspace.IWorkspace workspace, string path)
        {
            string workspaceDir = System.IO.Path.GetDirectoryName(path);

            JadeData.Workspace.IFolder folder = workspace;
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for (int i = 0; i < projTemps.Count; i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            System.Xml.XmlDocument doc = new XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            System.IO.TextWriter stream = new System.IO.StreamWriter(path); 
            try
            {                    
                serializer.Serialize(stream, result);
                stream.Close();
                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;

namespace JadeGui.ViewModels
{
    using JadeData;

   
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IJadeViewModel
    {
        #region Data

        private JadeCore.IWorkspaceManager _workspaceManager;
        private Commands.NewWorkspace _newWorkspaceCmd;
        private Commands.OpenWorkspace _openWorkspaceCmd;
        private Commands.SaveWorkspace _saveWorkspaceCmd;
        private Commands.SaveAsWorkspace _saveAsWorkspaceCmd;
        private Commands.CloseWorkspace _closeWorkspaceCmd;

        private ICommand _exitCommand;

        #endregion

        public JadeViewModel()
        {
            _workspaceManager = new WorkspaceManager();
            _workspaceManager.WorkspaceOpened += delegate { OnPropertyChanged("Workspace"); };

            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();
            _newWorkspaceCmd = new Commands.NewWorkspace(this);
            _openWorkspaceCmd = new Commands.OpenWorkspace(this);
            _saveWorkspaceCmd = new Commands.SaveWorkspace(this);
            _saveAsWorkspaceCmd = new Commands.SaveAsWorkspace(this);
            _closeWorkspaceCmd = new Commands.CloseWorkspace(this);
        }

        #region EditorControl

        private JadeControls.EditorControl.ViewModel.EditorControlViewModel _editorModel;

        public JadeCore.ViewModels.IEditorViewModel Editor
        {
            get { return _editorModel; }
        }

        #endregion

        #region RequestClose [event]

        /// <summary>
        /// Raised to request ui to close
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

        #region Workspace Tree

        public JadeCore.ViewModels.IWorkspaceViewModel Workspace
        {
            get { return _workspaceManager.ViewModel; }
        }

        #endregion

        #region Commands

        public ICommand CloseWorkspaceCommand { get { return _closeWorkspaceCmd.Command; } }
        public ICommand NewWorkspaceCommand { get { return _newWorkspaceCmd.Command; } }
        public ICommand OpenWorkspaceCommand { get { return _openWorkspaceCmd.Command;}}
        public ICommand SaveWorkspaceCommand { get { return _saveWorkspaceCmd.Command; } }
        public ICommand SaveAsWorkspaceCommand { get { return _saveAsWorkspaceCmd.Command; } }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(delegate { OnRequestClose(); });
                }
                return _exitCommand;
            }
        }

        #endregion

        #region public Methods

        public bool OnExit()
        {
            if (_workspaceManager.RequiresSave)
            {
                return _workspaceManager.SaveOrDiscardWorkspace();
            }
            return true;
        }
       
        #endregion

        public JadeCore.IWorkspaceManager WorkspaceManager
        {
            get
            {
                return _workspaceManager;
            }
        }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class SaveWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path = _viewModel.Workspace.Path;

            if(path == null || path.Length == 0)
            {
                if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    return;                
            }
            _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.RequiresSave; } }
    }

    internal class SaveAsWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveAsWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path;

            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
            }
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.WorkspaceOpen; } }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class OpenWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command 
        { 
            get 
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            } 
        }

        public OpenWorkspace(JadeCore.ViewModels.IJadeViewModel vm)        
        {
            _viewModel = vm;     
        }

        private void OnCommand()
        {
            string path;
            if (JadeCore.GuiUtils.PromptOpenFile(".jws", "Jade Workspace files (.jws)|*.jws", true, out path) == false)
            {
                return;
            }

            try
            {
                _viewModel.WorkspaceManager.OpenWorkspace(path);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening workspace. " + e.ToString());
            }            
        }

        private bool CanDoCommand { get { return true; } }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class NewWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public NewWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string name;

            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                try
                {
                    _viewModel.WorkspaceManager.NewWorkspace(name, "");
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        private bool CanDoCommand 
        { 
            get { return true; } 
        }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class CloseWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public CloseWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            _viewModel.WorkspaceManager.CloseWorkspace();            
        }

        private bool CanDoCommand
        {
            get
            {
                return _viewModel.WorkspaceManager.WorkspaceOpen;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JadeData.Persistence.Workspace
{
    public static class Reader
    {
        private static JadeData.Project.IProject MakeProject(string workspaceDir, ProjectType xml)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                path = System.IO.Path.Combine(workspaceDir, path);
            }
            return Persistence.Project.Reader.Read(path);
        }

        private static JadeData.Workspace.IFolder MakeFolder(string workspaceDir, FolderType xml)
        {
            JadeData.Workspace.IFolder result = new JadeData.Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(workspaceDir, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(workspaceDir, p));
            }
            return result;
        }

        static public JadeData.Workspace.IWorkspace Read(string path)
        {
            WorkspaceType xml;
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceType));
            TextReader tr = new StreamReader(path);
            try
            {                
                xml = (WorkspaceType)serializer.Deserialize(tr);                
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }

            JadeData.Workspace.IWorkspace result = new JadeData.Workspace.Workspace(xml.Name, path);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result.Directory, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(result.Directory, p));
            }

            return result;
        }
    }

    public static class Writer
    {
        static private ProjectType MakeProject(JadeData.Workspace.ProjectItem proj, string workspaceDir)
        {
            ProjectType result = new ProjectType();

            string absPath = proj.Path;

            if (System.IO.Path.IsPathRooted(absPath) == false)
            {
                absPath = System.IO.Path.Combine(workspaceDir, absPath);
            }

            result.Path = proj.Path;
            Persistence.Project.Writer.Write(proj, absPath);
            return result;
        }

        static private FolderType MakeFolder(JadeData.Workspace.IFolder folder, string workspaceDir)
        {
            FolderType result = new FolderType();
            result.Name = folder.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for(int i=0;i<projTemps.Count;i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            return result;
        }

        static private string GetProjectPath(JadeData.Workspace.IWorkspace workspace, JadeData.Project.IProject proj)
        {
            return "";
        }

        static public string Write(JadeData.Workspace.IWorkspace workspace, string path)
        {
            string workspaceDir = System.IO.Path.GetDirectoryName(path);

            JadeData.Workspace.IFolder folder = workspace;
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for (int i = 0; i < projTemps.Count; i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            System.Xml.XmlDocument doc = new XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            System.IO.TextWriter stream = new System.IO.StreamWriter(path); 
            try
            {                    
                serializer.Serialize(stream, result);
                stream.Close();
                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;

namespace JadeGui.ViewModels
{
    using JadeData;

   
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IJadeViewModel
    {
        #region Data

        private JadeCore.IWorkspaceManager _workspaceManager;
        private Commands.NewWorkspace _newWorkspaceCmd;
        private Commands.OpenWorkspace _openWorkspaceCmd;
        private Commands.SaveWorkspace _saveWorkspaceCmd;
        private Commands.SaveAsWorkspace _saveAsWorkspaceCmd;
        private Commands.CloseWorkspace _closeWorkspaceCmd;

        private ICommand _exitCommand;

        #endregion

        public JadeViewModel()
        {
            _workspaceManager = new WorkspaceManager();
            _workspaceManager.WorkspaceOpened += delegate { OnPropertyChanged("Workspace"); };

            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();
            _newWorkspaceCmd = new Commands.NewWorkspace(this);
            _openWorkspaceCmd = new Commands.OpenWorkspace(this);
            _saveWorkspaceCmd = new Commands.SaveWorkspace(this);
            _saveAsWorkspaceCmd = new Commands.SaveAsWorkspace(this);
            _closeWorkspaceCmd = new Commands.CloseWorkspace(this);
        }

        #region EditorControl

        private JadeControls.EditorControl.ViewModel.EditorControlViewModel _editorModel;

        public JadeCore.ViewModels.IEditorViewModel Editor
        {
            get { return _editorModel; }
        }

        #endregion

        #region RequestClose [event]

        /// <summary>
        /// Raised to request ui to close
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

        #region Workspace Tree

        public JadeCore.ViewModels.IWorkspaceViewModel Workspace
        {
            get { return _workspaceManager.ViewModel; }
        }

        #endregion

        #region Commands

        public ICommand CloseWorkspaceCommand { get { return _closeWorkspaceCmd.Command; } }
        public ICommand NewWorkspaceCommand { get { return _newWorkspaceCmd.Command; } }
        public ICommand OpenWorkspaceCommand { get { return _openWorkspaceCmd.Command;}}
        public ICommand SaveWorkspaceCommand { get { return _saveWorkspaceCmd.Command; } }
        public ICommand SaveAsWorkspaceCommand { get { return _saveAsWorkspaceCmd.Command; } }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(delegate { OnRequestClose(); });
                }
                return _exitCommand;
            }
        }

        #endregion

        #region public Methods

        public bool OnExit()
        {
            if (_workspaceManager.RequiresSave)
            {
                return _workspaceManager.SaveOrDiscardWorkspace();
            }
            return true;
        }
       
        #endregion

        public JadeCore.IWorkspaceManager WorkspaceManager
        {
            get
            {
                return _workspaceManager;
            }
        }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class SaveWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path = _viewModel.Workspace.Path;

            if(path == null || path.Length == 0)
            {
                if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    return;                
            }
            _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.RequiresSave; } }
    }

    internal class SaveAsWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveAsWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path;

            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
            }
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.WorkspaceOpen; } }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class OpenWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command 
        { 
            get 
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            } 
        }

        public OpenWorkspace(JadeCore.ViewModels.IJadeViewModel vm)        
        {
            _viewModel = vm;     
        }

        private void OnCommand()
        {
            string path;
            if (JadeCore.GuiUtils.PromptOpenFile(".jws", "Jade Workspace files (.jws)|*.jws", true, out path) == false)
            {
                return;
            }

            try
            {
                _viewModel.WorkspaceManager.OpenWorkspace(path);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening workspace. " + e.ToString());
            }            
        }

        private bool CanDoCommand { get { return true; } }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class NewWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public NewWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string name;

            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                try
                {
                    _viewModel.WorkspaceManager.NewWorkspace(name, "");
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        private bool CanDoCommand 
        { 
            get { return true; } 
        }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class CloseWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public CloseWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            _viewModel.WorkspaceManager.CloseWorkspace();            
        }

        private bool CanDoCommand
        {
            get
            {
                return _viewModel.WorkspaceManager.WorkspaceOpen;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JadeData.Persistence.Workspace
{
    public static class Reader
    {
        private static JadeData.Project.IProject MakeProject(string workspaceDir, ProjectType xml)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                path = System.IO.Path.Combine(workspaceDir, path);
            }
            return Persistence.Project.Reader.Read(path);
        }

        private static JadeData.Workspace.IFolder MakeFolder(string workspaceDir, FolderType xml)
        {
            JadeData.Workspace.IFolder result = new JadeData.Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(workspaceDir, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(workspaceDir, p));
            }
            return result;
        }

        static public JadeData.Workspace.IWorkspace Read(string path)
        {
            WorkspaceType xml;
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceType));
            TextReader tr = new StreamReader(path);
            try
            {                
                xml = (WorkspaceType)serializer.Deserialize(tr);                
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }

            JadeData.Workspace.IWorkspace result = new JadeData.Workspace.Workspace(xml.Name, path);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result.Directory, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(result.Directory, p));
            }

            return result;
        }
    }

    public static class Writer
    {
        static private ProjectType MakeProject(JadeData.Workspace.ProjectItem proj, string workspaceDir)
        {
            ProjectType result = new ProjectType();

            string absPath = proj.Path;

            if (System.IO.Path.IsPathRooted(absPath) == false)
            {
                absPath = System.IO.Path.Combine(workspaceDir, absPath);
            }

            result.Path = proj.Path;
            Persistence.Project.Writer.Write(proj, absPath);
            return result;
        }

        static private FolderType MakeFolder(JadeData.Workspace.IFolder folder, string workspaceDir)
        {
            FolderType result = new FolderType();
            result.Name = folder.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for(int i=0;i<projTemps.Count;i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            return result;
        }

        static private string GetProjectPath(JadeData.Workspace.IWorkspace workspace, JadeData.Project.IProject proj)
        {
            return "";
        }

        static public string Write(JadeData.Workspace.IWorkspace workspace, string path)
        {
            string workspaceDir = System.IO.Path.GetDirectoryName(path);

            JadeData.Workspace.IFolder folder = workspace;
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for (int i = 0; i < projTemps.Count; i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            System.Xml.XmlDocument doc = new XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            System.IO.TextWriter stream = new System.IO.StreamWriter(path); 
            try
            {                    
                serializer.Serialize(stream, result);
                stream.Close();
                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;

namespace JadeGui.ViewModels
{
    using JadeData;

   
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IJadeViewModel
    {
        #region Data

        private JadeCore.IWorkspaceManager _workspaceManager;
        private Commands.NewWorkspace _newWorkspaceCmd;
        private Commands.OpenWorkspace _openWorkspaceCmd;
        private Commands.SaveWorkspace _saveWorkspaceCmd;
        private Commands.SaveAsWorkspace _saveAsWorkspaceCmd;
        private Commands.CloseWorkspace _closeWorkspaceCmd;

        private ICommand _exitCommand;

        #endregion

        public JadeViewModel()
        {
            _workspaceManager = new WorkspaceManager();
            _workspaceManager.WorkspaceOpened += delegate { OnPropertyChanged("Workspace"); };

            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();
            _newWorkspaceCmd = new Commands.NewWorkspace(this);
            _openWorkspaceCmd = new Commands.OpenWorkspace(this);
            _saveWorkspaceCmd = new Commands.SaveWorkspace(this);
            _saveAsWorkspaceCmd = new Commands.SaveAsWorkspace(this);
            _closeWorkspaceCmd = new Commands.CloseWorkspace(this);
        }

        #region EditorControl

        private JadeControls.EditorControl.ViewModel.EditorControlViewModel _editorModel;

        public JadeCore.ViewModels.IEditorViewModel Editor
        {
            get { return _editorModel; }
        }

        #endregion

        #region RequestClose [event]

        /// <summary>
        /// Raised to request ui to close
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

        #region Workspace Tree

        public JadeCore.ViewModels.IWorkspaceViewModel Workspace
        {
            get { return _workspaceManager.ViewModel; }
        }

        #endregion

        #region Commands

        public ICommand CloseWorkspaceCommand { get { return _closeWorkspaceCmd.Command; } }
        public ICommand NewWorkspaceCommand { get { return _newWorkspaceCmd.Command; } }
        public ICommand OpenWorkspaceCommand { get { return _openWorkspaceCmd.Command;}}
        public ICommand SaveWorkspaceCommand { get { return _saveWorkspaceCmd.Command; } }
        public ICommand SaveAsWorkspaceCommand { get { return _saveAsWorkspaceCmd.Command; } }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(delegate { OnRequestClose(); });
                }
                return _exitCommand;
            }
        }

        #endregion

        #region public Methods

        public bool OnExit()
        {
            if (_workspaceManager.RequiresSave)
            {
                return _workspaceManager.SaveOrDiscardWorkspace();
            }
            return true;
        }
       
        #endregion

        public JadeCore.IWorkspaceManager WorkspaceManager
        {
            get
            {
                return _workspaceManager;
            }
        }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class SaveWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path = _viewModel.Workspace.Path;

            if(path == null || path.Length == 0)
            {
                if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    return;                
            }
            _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.RequiresSave; } }
    }

    internal class SaveAsWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveAsWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path;

            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
            }
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.WorkspaceOpen; } }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class OpenWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command 
        { 
            get 
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            } 
        }

        public OpenWorkspace(JadeCore.ViewModels.IJadeViewModel vm)        
        {
            _viewModel = vm;     
        }

        private void OnCommand()
        {
            string path;
            if (JadeCore.GuiUtils.PromptOpenFile(".jws", "Jade Workspace files (.jws)|*.jws", true, out path) == false)
            {
                return;
            }

            try
            {
                _viewModel.WorkspaceManager.OpenWorkspace(path);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening workspace. " + e.ToString());
            }            
        }

        private bool CanDoCommand { get { return true; } }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class NewWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public NewWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string name;

            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                try
                {
                    _viewModel.WorkspaceManager.NewWorkspace(name, "");
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        private bool CanDoCommand 
        { 
            get { return true; } 
        }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class CloseWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public CloseWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            _viewModel.WorkspaceManager.CloseWorkspace();            
        }

        private bool CanDoCommand
        {
            get
            {
                return _viewModel.WorkspaceManager.WorkspaceOpen;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JadeData.Persistence.Workspace
{
    public static class Reader
    {
        private static JadeData.Project.IProject MakeProject(string workspaceDir, ProjectType xml)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                path = System.IO.Path.Combine(workspaceDir, path);
            }
            return Persistence.Project.Reader.Read(path);
        }

        private static JadeData.Workspace.IFolder MakeFolder(string workspaceDir, FolderType xml)
        {
            JadeData.Workspace.IFolder result = new JadeData.Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(workspaceDir, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(workspaceDir, p));
            }
            return result;
        }

        static public JadeData.Workspace.IWorkspace Read(string path)
        {
            WorkspaceType xml;
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceType));
            TextReader tr = new StreamReader(path);
            try
            {                
                xml = (WorkspaceType)serializer.Deserialize(tr);                
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }

            JadeData.Workspace.IWorkspace result = new JadeData.Workspace.Workspace(xml.Name, path);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result.Directory, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(result.Directory, p));
            }

            return result;
        }
    }

    public static class Writer
    {
        static private ProjectType MakeProject(JadeData.Workspace.ProjectItem proj, string workspaceDir)
        {
            ProjectType result = new ProjectType();

            string absPath = proj.Path;

            if (System.IO.Path.IsPathRooted(absPath) == false)
            {
                absPath = System.IO.Path.Combine(workspaceDir, absPath);
            }

            result.Path = proj.Path;
            Persistence.Project.Writer.Write(proj, absPath);
            return result;
        }

        static private FolderType MakeFolder(JadeData.Workspace.IFolder folder, string workspaceDir)
        {
            FolderType result = new FolderType();
            result.Name = folder.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for(int i=0;i<projTemps.Count;i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            return result;
        }

        static private string GetProjectPath(JadeData.Workspace.IWorkspace workspace, JadeData.Project.IProject proj)
        {
            return "";
        }

        static public string Write(JadeData.Workspace.IWorkspace workspace, string path)
        {
            string workspaceDir = System.IO.Path.GetDirectoryName(path);

            JadeData.Workspace.IFolder folder = workspace;
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for (int i = 0; i < projTemps.Count; i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            System.Xml.XmlDocument doc = new XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            System.IO.TextWriter stream = new System.IO.StreamWriter(path); 
            try
            {                    
                serializer.Serialize(stream, result);
                stream.Close();
                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;

namespace JadeGui.ViewModels
{
    using JadeData;

   
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IJadeViewModel
    {
        #region Data

        private JadeCore.IWorkspaceManager _workspaceManager;
        private Commands.NewWorkspace _newWorkspaceCmd;
        private Commands.OpenWorkspace _openWorkspaceCmd;
        private Commands.SaveWorkspace _saveWorkspaceCmd;
        private Commands.SaveAsWorkspace _saveAsWorkspaceCmd;
        private Commands.CloseWorkspace _closeWorkspaceCmd;

        private ICommand _exitCommand;

        #endregion

        public JadeViewModel()
        {
            _workspaceManager = new WorkspaceManager();
            _workspaceManager.WorkspaceOpened += delegate { OnPropertyChanged("Workspace"); };

            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();
            _newWorkspaceCmd = new Commands.NewWorkspace(this);
            _openWorkspaceCmd = new Commands.OpenWorkspace(this);
            _saveWorkspaceCmd = new Commands.SaveWorkspace(this);
            _saveAsWorkspaceCmd = new Commands.SaveAsWorkspace(this);
            _closeWorkspaceCmd = new Commands.CloseWorkspace(this);
        }

        #region EditorControl

        private JadeControls.EditorControl.ViewModel.EditorControlViewModel _editorModel;

        public JadeCore.ViewModels.IEditorViewModel Editor
        {
            get { return _editorModel; }
        }

        #endregion

        #region RequestClose [event]

        /// <summary>
        /// Raised to request ui to close
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

        #region Workspace Tree

        public JadeCore.ViewModels.IWorkspaceViewModel Workspace
        {
            get { return _workspaceManager.ViewModel; }
        }

        #endregion

        #region Commands

        public ICommand CloseWorkspaceCommand { get { return _closeWorkspaceCmd.Command; } }
        public ICommand NewWorkspaceCommand { get { return _newWorkspaceCmd.Command; } }
        public ICommand OpenWorkspaceCommand { get { return _openWorkspaceCmd.Command;}}
        public ICommand SaveWorkspaceCommand { get { return _saveWorkspaceCmd.Command; } }
        public ICommand SaveAsWorkspaceCommand { get { return _saveAsWorkspaceCmd.Command; } }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(delegate { OnRequestClose(); });
                }
                return _exitCommand;
            }
        }

        #endregion

        #region public Methods

        public bool OnExit()
        {
            if (_workspaceManager.RequiresSave)
            {
                return _workspaceManager.SaveOrDiscardWorkspace();
            }
            return true;
        }
       
        #endregion

        public JadeCore.IWorkspaceManager WorkspaceManager
        {
            get
            {
                return _workspaceManager;
            }
        }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class SaveWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path = _viewModel.Workspace.Path;

            if(path == null || path.Length == 0)
            {
                if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    return;                
            }
            _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.RequiresSave; } }
    }

    internal class SaveAsWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveAsWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path;

            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
            }
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.WorkspaceOpen; } }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class OpenWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command 
        { 
            get 
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            } 
        }

        public OpenWorkspace(JadeCore.ViewModels.IJadeViewModel vm)        
        {
            _viewModel = vm;     
        }

        private void OnCommand()
        {
            string path;
            if (JadeCore.GuiUtils.PromptOpenFile(".jws", "Jade Workspace files (.jws)|*.jws", true, out path) == false)
            {
                return;
            }

            try
            {
                _viewModel.WorkspaceManager.OpenWorkspace(path);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening workspace. " + e.ToString());
            }            
        }

        private bool CanDoCommand { get { return true; } }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class NewWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public NewWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string name;

            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                try
                {
                    _viewModel.WorkspaceManager.NewWorkspace(name, "");
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        private bool CanDoCommand 
        { 
            get { return true; } 
        }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class CloseWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public CloseWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            _viewModel.WorkspaceManager.CloseWorkspace();            
        }

        private bool CanDoCommand
        {
            get
            {
                return _viewModel.WorkspaceManager.WorkspaceOpen;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JadeData.Persistence.Workspace
{
    public static class Reader
    {
        private static JadeData.Project.IProject MakeProject(string workspaceDir, ProjectType xml)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                path = System.IO.Path.Combine(workspaceDir, path);
            }
            return Persistence.Project.Reader.Read(path);
        }

        private static JadeData.Workspace.IFolder MakeFolder(string workspaceDir, FolderType xml)
        {
            JadeData.Workspace.IFolder result = new JadeData.Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(workspaceDir, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(workspaceDir, p));
            }
            return result;
        }

        static public JadeData.Workspace.IWorkspace Read(string path)
        {
            WorkspaceType xml;
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceType));
            TextReader tr = new StreamReader(path);
            try
            {                
                xml = (WorkspaceType)serializer.Deserialize(tr);                
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }

            JadeData.Workspace.IWorkspace result = new JadeData.Workspace.Workspace(xml.Name, path);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result.Directory, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(result.Directory, p));
            }

            return result;
        }
    }

    public static class Writer
    {
        static private ProjectType MakeProject(JadeData.Workspace.ProjectItem proj, string workspaceDir)
        {
            ProjectType result = new ProjectType();

            string absPath = proj.Path;

            if (System.IO.Path.IsPathRooted(absPath) == false)
            {
                absPath = System.IO.Path.Combine(workspaceDir, absPath);
            }

            result.Path = proj.Path;
            Persistence.Project.Writer.Write(proj, absPath);
            return result;
        }

        static private FolderType MakeFolder(JadeData.Workspace.IFolder folder, string workspaceDir)
        {
            FolderType result = new FolderType();
            result.Name = folder.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for(int i=0;i<projTemps.Count;i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            return result;
        }

        static private string GetProjectPath(JadeData.Workspace.IWorkspace workspace, JadeData.Project.IProject proj)
        {
            return "";
        }

        static public string Write(JadeData.Workspace.IWorkspace workspace, string path)
        {
            string workspaceDir = System.IO.Path.GetDirectoryName(path);

            JadeData.Workspace.IFolder folder = workspace;
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for (int i = 0; i < projTemps.Count; i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            System.Xml.XmlDocument doc = new XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            System.IO.TextWriter stream = new System.IO.StreamWriter(path); 
            try
            {                    
                serializer.Serialize(stream, result);
                stream.Close();
                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;

namespace JadeGui.ViewModels
{
    using JadeData;

   
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IJadeViewModel
    {
        #region Data

        private JadeCore.IWorkspaceManager _workspaceManager;
        private Commands.NewWorkspace _newWorkspaceCmd;
        private Commands.OpenWorkspace _openWorkspaceCmd;
        private Commands.SaveWorkspace _saveWorkspaceCmd;
        private Commands.SaveAsWorkspace _saveAsWorkspaceCmd;
        private Commands.CloseWorkspace _closeWorkspaceCmd;

        private ICommand _exitCommand;

        #endregion

        public JadeViewModel()
        {
            _workspaceManager = new WorkspaceManager();
            _workspaceManager.WorkspaceOpened += delegate { OnPropertyChanged("Workspace"); };

            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();
            _newWorkspaceCmd = new Commands.NewWorkspace(this);
            _openWorkspaceCmd = new Commands.OpenWorkspace(this);
            _saveWorkspaceCmd = new Commands.SaveWorkspace(this);
            _saveAsWorkspaceCmd = new Commands.SaveAsWorkspace(this);
            _closeWorkspaceCmd = new Commands.CloseWorkspace(this);
        }

        #region EditorControl

        private JadeControls.EditorControl.ViewModel.EditorControlViewModel _editorModel;

        public JadeCore.ViewModels.IEditorViewModel Editor
        {
            get { return _editorModel; }
        }

        #endregion

        #region RequestClose [event]

        /// <summary>
        /// Raised to request ui to close
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

        #region Workspace Tree

        public JadeCore.ViewModels.IWorkspaceViewModel Workspace
        {
            get { return _workspaceManager.ViewModel; }
        }

        #endregion

        #region Commands

        public ICommand CloseWorkspaceCommand { get { return _closeWorkspaceCmd.Command; } }
        public ICommand NewWorkspaceCommand { get { return _newWorkspaceCmd.Command; } }
        public ICommand OpenWorkspaceCommand { get { return _openWorkspaceCmd.Command;}}
        public ICommand SaveWorkspaceCommand { get { return _saveWorkspaceCmd.Command; } }
        public ICommand SaveAsWorkspaceCommand { get { return _saveAsWorkspaceCmd.Command; } }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(delegate { OnRequestClose(); });
                }
                return _exitCommand;
            }
        }

        #endregion

        #region public Methods

        public bool OnExit()
        {
            if (_workspaceManager.RequiresSave)
            {
                return _workspaceManager.SaveOrDiscardWorkspace();
            }
            return true;
        }
       
        #endregion

        public JadeCore.IWorkspaceManager WorkspaceManager
        {
            get
            {
                return _workspaceManager;
            }
        }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class SaveWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path = _viewModel.Workspace.Path;

            if(path == null || path.Length == 0)
            {
                if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    return;                
            }
            _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.RequiresSave; } }
    }

    internal class SaveAsWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveAsWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path;

            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
            }
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.WorkspaceOpen; } }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class OpenWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command 
        { 
            get 
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            } 
        }

        public OpenWorkspace(JadeCore.ViewModels.IJadeViewModel vm)        
        {
            _viewModel = vm;     
        }

        private void OnCommand()
        {
            string path;
            if (JadeCore.GuiUtils.PromptOpenFile(".jws", "Jade Workspace files (.jws)|*.jws", true, out path) == false)
            {
                return;
            }

            try
            {
                _viewModel.WorkspaceManager.OpenWorkspace(path);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening workspace. " + e.ToString());
            }            
        }

        private bool CanDoCommand { get { return true; } }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class NewWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public NewWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string name;

            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                try
                {
                    _viewModel.WorkspaceManager.NewWorkspace(name, "");
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        private bool CanDoCommand 
        { 
            get { return true; } 
        }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class CloseWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public CloseWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            _viewModel.WorkspaceManager.CloseWorkspace();            
        }

        private bool CanDoCommand
        {
            get
            {
                return _viewModel.WorkspaceManager.WorkspaceOpen;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JadeData.Persistence.Workspace
{
    public static class Reader
    {
        private static JadeData.Project.IProject MakeProject(string workspaceDir, ProjectType xml)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                path = System.IO.Path.Combine(workspaceDir, path);
            }
            return Persistence.Project.Reader.Read(path);
        }

        private static JadeData.Workspace.IFolder MakeFolder(string workspaceDir, FolderType xml)
        {
            JadeData.Workspace.IFolder result = new JadeData.Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(workspaceDir, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(workspaceDir, p));
            }
            return result;
        }

        static public JadeData.Workspace.IWorkspace Read(string path)
        {
            WorkspaceType xml;
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceType));
            TextReader tr = new StreamReader(path);
            try
            {                
                xml = (WorkspaceType)serializer.Deserialize(tr);                
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }

            JadeData.Workspace.IWorkspace result = new JadeData.Workspace.Workspace(xml.Name, path);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result.Directory, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(result.Directory, p));
            }

            return result;
        }
    }

    public static class Writer
    {
        static private ProjectType MakeProject(JadeData.Workspace.ProjectItem proj, string workspaceDir)
        {
            ProjectType result = new ProjectType();

            string absPath = proj.Path;

            if (System.IO.Path.IsPathRooted(absPath) == false)
            {
                absPath = System.IO.Path.Combine(workspaceDir, absPath);
            }

            result.Path = proj.Path;
            Persistence.Project.Writer.Write(proj, absPath);
            return result;
        }

        static private FolderType MakeFolder(JadeData.Workspace.IFolder folder, string workspaceDir)
        {
            FolderType result = new FolderType();
            result.Name = folder.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for(int i=0;i<projTemps.Count;i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            return result;
        }

        static private string GetProjectPath(JadeData.Workspace.IWorkspace workspace, JadeData.Project.IProject proj)
        {
            return "";
        }

        static public string Write(JadeData.Workspace.IWorkspace workspace, string path)
        {
            string workspaceDir = System.IO.Path.GetDirectoryName(path);

            JadeData.Workspace.IFolder folder = workspace;
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for (int i = 0; i < projTemps.Count; i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            System.Xml.XmlDocument doc = new XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            System.IO.TextWriter stream = new System.IO.StreamWriter(path); 
            try
            {                    
                serializer.Serialize(stream, result);
                stream.Close();
                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;

namespace JadeGui.ViewModels
{
    using JadeData;

   
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IJadeViewModel
    {
        #region Data

        private JadeCore.IWorkspaceManager _workspaceManager;
        private Commands.NewWorkspace _newWorkspaceCmd;
        private Commands.OpenWorkspace _openWorkspaceCmd;
        private Commands.SaveWorkspace _saveWorkspaceCmd;
        private Commands.SaveAsWorkspace _saveAsWorkspaceCmd;
        private Commands.CloseWorkspace _closeWorkspaceCmd;

        private ICommand _exitCommand;

        #endregion

        public JadeViewModel()
        {
            _workspaceManager = new WorkspaceManager();
            _workspaceManager.WorkspaceOpened += delegate { OnPropertyChanged("Workspace"); };

            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();
            _newWorkspaceCmd = new Commands.NewWorkspace(this);
            _openWorkspaceCmd = new Commands.OpenWorkspace(this);
            _saveWorkspaceCmd = new Commands.SaveWorkspace(this);
            _saveAsWorkspaceCmd = new Commands.SaveAsWorkspace(this);
            _closeWorkspaceCmd = new Commands.CloseWorkspace(this);
        }

        #region EditorControl

        private JadeControls.EditorControl.ViewModel.EditorControlViewModel _editorModel;

        public JadeCore.ViewModels.IEditorViewModel Editor
        {
            get { return _editorModel; }
        }

        #endregion

        #region RequestClose [event]

        /// <summary>
        /// Raised to request ui to close
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

        #region Workspace Tree

        public JadeCore.ViewModels.IWorkspaceViewModel Workspace
        {
            get { return _workspaceManager.ViewModel; }
        }

        #endregion

        #region Commands

        public ICommand CloseWorkspaceCommand { get { return _closeWorkspaceCmd.Command; } }
        public ICommand NewWorkspaceCommand { get { return _newWorkspaceCmd.Command; } }
        public ICommand OpenWorkspaceCommand { get { return _openWorkspaceCmd.Command;}}
        public ICommand SaveWorkspaceCommand { get { return _saveWorkspaceCmd.Command; } }
        public ICommand SaveAsWorkspaceCommand { get { return _saveAsWorkspaceCmd.Command; } }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(delegate { OnRequestClose(); });
                }
                return _exitCommand;
            }
        }

        #endregion

        #region public Methods

        public bool OnExit()
        {
            if (_workspaceManager.RequiresSave)
            {
                return _workspaceManager.SaveOrDiscardWorkspace();
            }
            return true;
        }
       
        #endregion

        public JadeCore.IWorkspaceManager WorkspaceManager
        {
            get
            {
                return _workspaceManager;
            }
        }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class SaveWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path = _viewModel.Workspace.Path;

            if(path == null || path.Length == 0)
            {
                if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    return;                
            }
            _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.RequiresSave; } }
    }

    internal class SaveAsWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveAsWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path;

            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
            }
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.WorkspaceOpen; } }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class OpenWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command 
        { 
            get 
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            } 
        }

        public OpenWorkspace(JadeCore.ViewModels.IJadeViewModel vm)        
        {
            _viewModel = vm;     
        }

        private void OnCommand()
        {
            string path;
            if (JadeCore.GuiUtils.PromptOpenFile(".jws", "Jade Workspace files (.jws)|*.jws", true, out path) == false)
            {
                return;
            }

            try
            {
                _viewModel.WorkspaceManager.OpenWorkspace(path);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening workspace. " + e.ToString());
            }            
        }

        private bool CanDoCommand { get { return true; } }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class NewWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public NewWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string name;

            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                try
                {
                    _viewModel.WorkspaceManager.NewWorkspace(name, "");
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        private bool CanDoCommand 
        { 
            get { return true; } 
        }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class CloseWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public CloseWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            _viewModel.WorkspaceManager.CloseWorkspace();            
        }

        private bool CanDoCommand
        {
            get
            {
                return _viewModel.WorkspaceManager.WorkspaceOpen;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JadeData.Persistence.Workspace
{
    public static class Reader
    {
        private static JadeData.Project.IProject MakeProject(string workspaceDir, ProjectType xml)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                path = System.IO.Path.Combine(workspaceDir, path);
            }
            return Persistence.Project.Reader.Read(path);
        }

        private static JadeData.Workspace.IFolder MakeFolder(string workspaceDir, FolderType xml)
        {
            JadeData.Workspace.IFolder result = new JadeData.Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(workspaceDir, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(workspaceDir, p));
            }
            return result;
        }

        static public JadeData.Workspace.IWorkspace Read(string path)
        {
            WorkspaceType xml;
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceType));
            TextReader tr = new StreamReader(path);
            try
            {                
                xml = (WorkspaceType)serializer.Deserialize(tr);                
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }

            JadeData.Workspace.IWorkspace result = new JadeData.Workspace.Workspace(xml.Name, path);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result.Directory, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(result.Directory, p));
            }

            return result;
        }
    }

    public static class Writer
    {
        static private ProjectType MakeProject(JadeData.Workspace.ProjectItem proj, string workspaceDir)
        {
            ProjectType result = new ProjectType();

            string absPath = proj.Path;

            if (System.IO.Path.IsPathRooted(absPath) == false)
            {
                absPath = System.IO.Path.Combine(workspaceDir, absPath);
            }

            result.Path = proj.Path;
            Persistence.Project.Writer.Write(proj, absPath);
            return result;
        }

        static private FolderType MakeFolder(JadeData.Workspace.IFolder folder, string workspaceDir)
        {
            FolderType result = new FolderType();
            result.Name = folder.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for(int i=0;i<projTemps.Count;i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            return result;
        }

        static private string GetProjectPath(JadeData.Workspace.IWorkspace workspace, JadeData.Project.IProject proj)
        {
            return "";
        }

        static public string Write(JadeData.Workspace.IWorkspace workspace, string path)
        {
            string workspaceDir = System.IO.Path.GetDirectoryName(path);

            JadeData.Workspace.IFolder folder = workspace;
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for (int i = 0; i < projTemps.Count; i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            System.Xml.XmlDocument doc = new XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            System.IO.TextWriter stream = new System.IO.StreamWriter(path); 
            try
            {                    
                serializer.Serialize(stream, result);
                stream.Close();
                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;

namespace JadeGui.ViewModels
{
    using JadeData;

   
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IJadeViewModel
    {
        #region Data

        private JadeCore.IWorkspaceManager _workspaceManager;
        private Commands.NewWorkspace _newWorkspaceCmd;
        private Commands.OpenWorkspace _openWorkspaceCmd;
        private Commands.SaveWorkspace _saveWorkspaceCmd;
        private Commands.SaveAsWorkspace _saveAsWorkspaceCmd;
        private Commands.CloseWorkspace _closeWorkspaceCmd;

        private ICommand _exitCommand;

        #endregion

        public JadeViewModel()
        {
            _workspaceManager = new WorkspaceManager();
            _workspaceManager.WorkspaceOpened += delegate { OnPropertyChanged("Workspace"); };

            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();
            _newWorkspaceCmd = new Commands.NewWorkspace(this);
            _openWorkspaceCmd = new Commands.OpenWorkspace(this);
            _saveWorkspaceCmd = new Commands.SaveWorkspace(this);
            _saveAsWorkspaceCmd = new Commands.SaveAsWorkspace(this);
            _closeWorkspaceCmd = new Commands.CloseWorkspace(this);
        }

        #region EditorControl

        private JadeControls.EditorControl.ViewModel.EditorControlViewModel _editorModel;

        public JadeCore.ViewModels.IEditorViewModel Editor
        {
            get { return _editorModel; }
        }

        #endregion

        #region RequestClose [event]

        /// <summary>
        /// Raised to request ui to close
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

        #region Workspace Tree

        public JadeCore.ViewModels.IWorkspaceViewModel Workspace
        {
            get { return _workspaceManager.ViewModel; }
        }

        #endregion

        #region Commands

        public ICommand CloseWorkspaceCommand { get { return _closeWorkspaceCmd.Command; } }
        public ICommand NewWorkspaceCommand { get { return _newWorkspaceCmd.Command; } }
        public ICommand OpenWorkspaceCommand { get { return _openWorkspaceCmd.Command;}}
        public ICommand SaveWorkspaceCommand { get { return _saveWorkspaceCmd.Command; } }
        public ICommand SaveAsWorkspaceCommand { get { return _saveAsWorkspaceCmd.Command; } }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(delegate { OnRequestClose(); });
                }
                return _exitCommand;
            }
        }

        #endregion

        #region public Methods

        public bool OnExit()
        {
            if (_workspaceManager.RequiresSave)
            {
                return _workspaceManager.SaveOrDiscardWorkspace();
            }
            return true;
        }
       
        #endregion

        public JadeCore.IWorkspaceManager WorkspaceManager
        {
            get
            {
                return _workspaceManager;
            }
        }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class SaveWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path = _viewModel.Workspace.Path;

            if(path == null || path.Length == 0)
            {
                if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    return;                
            }
            _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.RequiresSave; } }
    }

    internal class SaveAsWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveAsWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path;

            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
            }
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.WorkspaceOpen; } }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class OpenWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command 
        { 
            get 
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            } 
        }

        public OpenWorkspace(JadeCore.ViewModels.IJadeViewModel vm)        
        {
            _viewModel = vm;     
        }

        private void OnCommand()
        {
            string path;
            if (JadeCore.GuiUtils.PromptOpenFile(".jws", "Jade Workspace files (.jws)|*.jws", true, out path) == false)
            {
                return;
            }

            try
            {
                _viewModel.WorkspaceManager.OpenWorkspace(path);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening workspace. " + e.ToString());
            }            
        }

        private bool CanDoCommand { get { return true; } }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class NewWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public NewWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string name;

            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                try
                {
                    _viewModel.WorkspaceManager.NewWorkspace(name, "");
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        private bool CanDoCommand 
        { 
            get { return true; } 
        }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class CloseWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public CloseWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            _viewModel.WorkspaceManager.CloseWorkspace();            
        }

        private bool CanDoCommand
        {
            get
            {
                return _viewModel.WorkspaceManager.WorkspaceOpen;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JadeData.Persistence.Workspace
{
    public static class Reader
    {
        private static JadeData.Project.IProject MakeProject(string workspaceDir, ProjectType xml)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                path = System.IO.Path.Combine(workspaceDir, path);
            }
            return Persistence.Project.Reader.Read(path);
        }

        private static JadeData.Workspace.IFolder MakeFolder(string workspaceDir, FolderType xml)
        {
            JadeData.Workspace.IFolder result = new JadeData.Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(workspaceDir, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(workspaceDir, p));
            }
            return result;
        }

        static public JadeData.Workspace.IWorkspace Read(string path)
        {
            WorkspaceType xml;
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceType));
            TextReader tr = new StreamReader(path);
            try
            {                
                xml = (WorkspaceType)serializer.Deserialize(tr);                
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }

            JadeData.Workspace.IWorkspace result = new JadeData.Workspace.Workspace(xml.Name, path);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result.Directory, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(result.Directory, p));
            }

            return result;
        }
    }

    public static class Writer
    {
        static private ProjectType MakeProject(JadeData.Workspace.ProjectItem proj, string workspaceDir)
        {
            ProjectType result = new ProjectType();

            string absPath = proj.Path;

            if (System.IO.Path.IsPathRooted(absPath) == false)
            {
                absPath = System.IO.Path.Combine(workspaceDir, absPath);
            }

            result.Path = proj.Path;
            Persistence.Project.Writer.Write(proj, absPath);
            return result;
        }

        static private FolderType MakeFolder(JadeData.Workspace.IFolder folder, string workspaceDir)
        {
            FolderType result = new FolderType();
            result.Name = folder.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for(int i=0;i<projTemps.Count;i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            return result;
        }

        static private string GetProjectPath(JadeData.Workspace.IWorkspace workspace, JadeData.Project.IProject proj)
        {
            return "";
        }

        static public string Write(JadeData.Workspace.IWorkspace workspace, string path)
        {
            string workspaceDir = System.IO.Path.GetDirectoryName(path);

            JadeData.Workspace.IFolder folder = workspace;
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for (int i = 0; i < projTemps.Count; i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            System.Xml.XmlDocument doc = new XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            System.IO.TextWriter stream = new System.IO.StreamWriter(path); 
            try
            {                    
                serializer.Serialize(stream, result);
                stream.Close();
                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;

namespace JadeGui.ViewModels
{
    using JadeData;

   
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IJadeViewModel
    {
        #region Data

        private JadeCore.IWorkspaceManager _workspaceManager;
        private Commands.NewWorkspace _newWorkspaceCmd;
        private Commands.OpenWorkspace _openWorkspaceCmd;
        private Commands.SaveWorkspace _saveWorkspaceCmd;
        private Commands.SaveAsWorkspace _saveAsWorkspaceCmd;
        private Commands.CloseWorkspace _closeWorkspaceCmd;

        private ICommand _exitCommand;

        #endregion

        public JadeViewModel()
        {
            _workspaceManager = new WorkspaceManager();
            _workspaceManager.WorkspaceOpened += delegate { OnPropertyChanged("Workspace"); };

            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();
            _newWorkspaceCmd = new Commands.NewWorkspace(this);
            _openWorkspaceCmd = new Commands.OpenWorkspace(this);
            _saveWorkspaceCmd = new Commands.SaveWorkspace(this);
            _saveAsWorkspaceCmd = new Commands.SaveAsWorkspace(this);
            _closeWorkspaceCmd = new Commands.CloseWorkspace(this);
        }

        #region EditorControl

        private JadeControls.EditorControl.ViewModel.EditorControlViewModel _editorModel;

        public JadeCore.ViewModels.IEditorViewModel Editor
        {
            get { return _editorModel; }
        }

        #endregion

        #region RequestClose [event]

        /// <summary>
        /// Raised to request ui to close
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

        #region Workspace Tree

        public JadeCore.ViewModels.IWorkspaceViewModel Workspace
        {
            get { return _workspaceManager.ViewModel; }
        }

        #endregion

        #region Commands

        public ICommand CloseWorkspaceCommand { get { return _closeWorkspaceCmd.Command; } }
        public ICommand NewWorkspaceCommand { get { return _newWorkspaceCmd.Command; } }
        public ICommand OpenWorkspaceCommand { get { return _openWorkspaceCmd.Command;}}
        public ICommand SaveWorkspaceCommand { get { return _saveWorkspaceCmd.Command; } }
        public ICommand SaveAsWorkspaceCommand { get { return _saveAsWorkspaceCmd.Command; } }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(delegate { OnRequestClose(); });
                }
                return _exitCommand;
            }
        }

        #endregion

        #region public Methods

        public bool OnExit()
        {
            if (_workspaceManager.RequiresSave)
            {
                return _workspaceManager.SaveOrDiscardWorkspace();
            }
            return true;
        }
       
        #endregion

        public JadeCore.IWorkspaceManager WorkspaceManager
        {
            get
            {
                return _workspaceManager;
            }
        }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class SaveWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path = _viewModel.Workspace.Path;

            if(path == null || path.Length == 0)
            {
                if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    return;                
            }
            _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.RequiresSave; } }
    }

    internal class SaveAsWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveAsWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path;

            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
            }
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.WorkspaceOpen; } }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class OpenWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command 
        { 
            get 
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            } 
        }

        public OpenWorkspace(JadeCore.ViewModels.IJadeViewModel vm)        
        {
            _viewModel = vm;     
        }

        private void OnCommand()
        {
            string path;
            if (JadeCore.GuiUtils.PromptOpenFile(".jws", "Jade Workspace files (.jws)|*.jws", true, out path) == false)
            {
                return;
            }

            try
            {
                _viewModel.WorkspaceManager.OpenWorkspace(path);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening workspace. " + e.ToString());
            }            
        }

        private bool CanDoCommand { get { return true; } }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class NewWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public NewWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string name;

            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                try
                {
                    _viewModel.WorkspaceManager.NewWorkspace(name, "");
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        private bool CanDoCommand 
        { 
            get { return true; } 
        }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class CloseWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public CloseWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            _viewModel.WorkspaceManager.CloseWorkspace();            
        }

        private bool CanDoCommand
        {
            get
            {
                return _viewModel.WorkspaceManager.WorkspaceOpen;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JadeData.Persistence.Workspace
{
    public static class Reader
    {
        private static JadeData.Project.IProject MakeProject(string workspaceDir, ProjectType xml)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                path = System.IO.Path.Combine(workspaceDir, path);
            }
            return Persistence.Project.Reader.Read(path);
        }

        private static JadeData.Workspace.IFolder MakeFolder(string workspaceDir, FolderType xml)
        {
            JadeData.Workspace.IFolder result = new JadeData.Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(workspaceDir, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(workspaceDir, p));
            }
            return result;
        }

        static public JadeData.Workspace.IWorkspace Read(string path)
        {
            WorkspaceType xml;
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceType));
            TextReader tr = new StreamReader(path);
            try
            {                
                xml = (WorkspaceType)serializer.Deserialize(tr);                
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }

            JadeData.Workspace.IWorkspace result = new JadeData.Workspace.Workspace(xml.Name, path);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result.Directory, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(result.Directory, p));
            }

            return result;
        }
    }

    public static class Writer
    {
        static private ProjectType MakeProject(JadeData.Workspace.ProjectItem proj, string workspaceDir)
        {
            ProjectType result = new ProjectType();

            string absPath = proj.Path;

            if (System.IO.Path.IsPathRooted(absPath) == false)
            {
                absPath = System.IO.Path.Combine(workspaceDir, absPath);
            }

            result.Path = proj.Path;
            Persistence.Project.Writer.Write(proj, absPath);
            return result;
        }

        static private FolderType MakeFolder(JadeData.Workspace.IFolder folder, string workspaceDir)
        {
            FolderType result = new FolderType();
            result.Name = folder.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for(int i=0;i<projTemps.Count;i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            return result;
        }

        static private string GetProjectPath(JadeData.Workspace.IWorkspace workspace, JadeData.Project.IProject proj)
        {
            return "";
        }

        static public string Write(JadeData.Workspace.IWorkspace workspace, string path)
        {
            string workspaceDir = System.IO.Path.GetDirectoryName(path);

            JadeData.Workspace.IFolder folder = workspace;
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for (int i = 0; i < projTemps.Count; i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            System.Xml.XmlDocument doc = new XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            System.IO.TextWriter stream = new System.IO.StreamWriter(path); 
            try
            {                    
                serializer.Serialize(stream, result);
                stream.Close();
                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;

namespace JadeGui.ViewModels
{
    using JadeData;

   
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IJadeViewModel
    {
        #region Data

        private JadeCore.IWorkspaceManager _workspaceManager;
        private Commands.NewWorkspace _newWorkspaceCmd;
        private Commands.OpenWorkspace _openWorkspaceCmd;
        private Commands.SaveWorkspace _saveWorkspaceCmd;
        private Commands.SaveAsWorkspace _saveAsWorkspaceCmd;
        private Commands.CloseWorkspace _closeWorkspaceCmd;

        private ICommand _exitCommand;

        #endregion

        public JadeViewModel()
        {
            _workspaceManager = new WorkspaceManager();
            _workspaceManager.WorkspaceOpened += delegate { OnPropertyChanged("Workspace"); };

            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();
            _newWorkspaceCmd = new Commands.NewWorkspace(this);
            _openWorkspaceCmd = new Commands.OpenWorkspace(this);
            _saveWorkspaceCmd = new Commands.SaveWorkspace(this);
            _saveAsWorkspaceCmd = new Commands.SaveAsWorkspace(this);
            _closeWorkspaceCmd = new Commands.CloseWorkspace(this);
        }

        #region EditorControl

        private JadeControls.EditorControl.ViewModel.EditorControlViewModel _editorModel;

        public JadeCore.ViewModels.IEditorViewModel Editor
        {
            get { return _editorModel; }
        }

        #endregion

        #region RequestClose [event]

        /// <summary>
        /// Raised to request ui to close
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

        #region Workspace Tree

        public JadeCore.ViewModels.IWorkspaceViewModel Workspace
        {
            get { return _workspaceManager.ViewModel; }
        }

        #endregion

        #region Commands

        public ICommand CloseWorkspaceCommand { get { return _closeWorkspaceCmd.Command; } }
        public ICommand NewWorkspaceCommand { get { return _newWorkspaceCmd.Command; } }
        public ICommand OpenWorkspaceCommand { get { return _openWorkspaceCmd.Command;}}
        public ICommand SaveWorkspaceCommand { get { return _saveWorkspaceCmd.Command; } }
        public ICommand SaveAsWorkspaceCommand { get { return _saveAsWorkspaceCmd.Command; } }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(delegate { OnRequestClose(); });
                }
                return _exitCommand;
            }
        }

        #endregion

        #region public Methods

        public bool OnExit()
        {
            if (_workspaceManager.RequiresSave)
            {
                return _workspaceManager.SaveOrDiscardWorkspace();
            }
            return true;
        }
       
        #endregion

        public JadeCore.IWorkspaceManager WorkspaceManager
        {
            get
            {
                return _workspaceManager;
            }
        }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class SaveWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path = _viewModel.Workspace.Path;

            if(path == null || path.Length == 0)
            {
                if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    return;                
            }
            _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.RequiresSave; } }
    }

    internal class SaveAsWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveAsWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path;

            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
            }
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.WorkspaceOpen; } }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class OpenWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command 
        { 
            get 
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            } 
        }

        public OpenWorkspace(JadeCore.ViewModels.IJadeViewModel vm)        
        {
            _viewModel = vm;     
        }

        private void OnCommand()
        {
            string path;
            if (JadeCore.GuiUtils.PromptOpenFile(".jws", "Jade Workspace files (.jws)|*.jws", true, out path) == false)
            {
                return;
            }

            try
            {
                _viewModel.WorkspaceManager.OpenWorkspace(path);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening workspace. " + e.ToString());
            }            
        }

        private bool CanDoCommand { get { return true; } }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class NewWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public NewWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string name;

            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                try
                {
                    _viewModel.WorkspaceManager.NewWorkspace(name, "");
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        private bool CanDoCommand 
        { 
            get { return true; } 
        }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class CloseWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public CloseWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            _viewModel.WorkspaceManager.CloseWorkspace();            
        }

        private bool CanDoCommand
        {
            get
            {
                return _viewModel.WorkspaceManager.WorkspaceOpen;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JadeData.Persistence.Workspace
{
    public static class Reader
    {
        private static JadeData.Project.IProject MakeProject(string workspaceDir, ProjectType xml)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                path = System.IO.Path.Combine(workspaceDir, path);
            }
            return Persistence.Project.Reader.Read(path);
        }

        private static JadeData.Workspace.IFolder MakeFolder(string workspaceDir, FolderType xml)
        {
            JadeData.Workspace.IFolder result = new JadeData.Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(workspaceDir, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(workspaceDir, p));
            }
            return result;
        }

        static public JadeData.Workspace.IWorkspace Read(string path)
        {
            WorkspaceType xml;
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceType));
            TextReader tr = new StreamReader(path);
            try
            {                
                xml = (WorkspaceType)serializer.Deserialize(tr);                
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }

            JadeData.Workspace.IWorkspace result = new JadeData.Workspace.Workspace(xml.Name, path);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result.Directory, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(result.Directory, p));
            }

            return result;
        }
    }

    public static class Writer
    {
        static private ProjectType MakeProject(JadeData.Workspace.ProjectItem proj, string workspaceDir)
        {
            ProjectType result = new ProjectType();

            string absPath = proj.Path;

            if (System.IO.Path.IsPathRooted(absPath) == false)
            {
                absPath = System.IO.Path.Combine(workspaceDir, absPath);
            }

            result.Path = proj.Path;
            Persistence.Project.Writer.Write(proj, absPath);
            return result;
        }

        static private FolderType MakeFolder(JadeData.Workspace.IFolder folder, string workspaceDir)
        {
            FolderType result = new FolderType();
            result.Name = folder.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for(int i=0;i<projTemps.Count;i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            return result;
        }

        static private string GetProjectPath(JadeData.Workspace.IWorkspace workspace, JadeData.Project.IProject proj)
        {
            return "";
        }

        static public string Write(JadeData.Workspace.IWorkspace workspace, string path)
        {
            string workspaceDir = System.IO.Path.GetDirectoryName(path);

            JadeData.Workspace.IFolder folder = workspace;
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for (int i = 0; i < projTemps.Count; i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            System.Xml.XmlDocument doc = new XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            System.IO.TextWriter stream = new System.IO.StreamWriter(path); 
            try
            {                    
                serializer.Serialize(stream, result);
                stream.Close();
                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;

namespace JadeGui.ViewModels
{
    using JadeData;

   
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IJadeViewModel
    {
        #region Data

        private JadeCore.IWorkspaceManager _workspaceManager;
        private Commands.NewWorkspace _newWorkspaceCmd;
        private Commands.OpenWorkspace _openWorkspaceCmd;
        private Commands.SaveWorkspace _saveWorkspaceCmd;
        private Commands.SaveAsWorkspace _saveAsWorkspaceCmd;
        private Commands.CloseWorkspace _closeWorkspaceCmd;

        private ICommand _exitCommand;

        #endregion

        public JadeViewModel()
        {
            _workspaceManager = new WorkspaceManager();
            _workspaceManager.WorkspaceOpened += delegate { OnPropertyChanged("Workspace"); };

            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();
            _newWorkspaceCmd = new Commands.NewWorkspace(this);
            _openWorkspaceCmd = new Commands.OpenWorkspace(this);
            _saveWorkspaceCmd = new Commands.SaveWorkspace(this);
            _saveAsWorkspaceCmd = new Commands.SaveAsWorkspace(this);
            _closeWorkspaceCmd = new Commands.CloseWorkspace(this);
        }

        #region EditorControl

        private JadeControls.EditorControl.ViewModel.EditorControlViewModel _editorModel;

        public JadeCore.ViewModels.IEditorViewModel Editor
        {
            get { return _editorModel; }
        }

        #endregion

        #region RequestClose [event]

        /// <summary>
        /// Raised to request ui to close
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

        #region Workspace Tree

        public JadeCore.ViewModels.IWorkspaceViewModel Workspace
        {
            get { return _workspaceManager.ViewModel; }
        }

        #endregion

        #region Commands

        public ICommand CloseWorkspaceCommand { get { return _closeWorkspaceCmd.Command; } }
        public ICommand NewWorkspaceCommand { get { return _newWorkspaceCmd.Command; } }
        public ICommand OpenWorkspaceCommand { get { return _openWorkspaceCmd.Command;}}
        public ICommand SaveWorkspaceCommand { get { return _saveWorkspaceCmd.Command; } }
        public ICommand SaveAsWorkspaceCommand { get { return _saveAsWorkspaceCmd.Command; } }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(delegate { OnRequestClose(); });
                }
                return _exitCommand;
            }
        }

        #endregion

        #region public Methods

        public bool OnExit()
        {
            if (_workspaceManager.RequiresSave)
            {
                return _workspaceManager.SaveOrDiscardWorkspace();
            }
            return true;
        }
       
        #endregion

        public JadeCore.IWorkspaceManager WorkspaceManager
        {
            get
            {
                return _workspaceManager;
            }
        }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class SaveWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path = _viewModel.Workspace.Path;

            if(path == null || path.Length == 0)
            {
                if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    return;                
            }
            _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.RequiresSave; } }
    }

    internal class SaveAsWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveAsWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path;

            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
            }
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.WorkspaceOpen; } }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class OpenWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command 
        { 
            get 
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            } 
        }

        public OpenWorkspace(JadeCore.ViewModels.IJadeViewModel vm)        
        {
            _viewModel = vm;     
        }

        private void OnCommand()
        {
            string path;
            if (JadeCore.GuiUtils.PromptOpenFile(".jws", "Jade Workspace files (.jws)|*.jws", true, out path) == false)
            {
                return;
            }

            try
            {
                _viewModel.WorkspaceManager.OpenWorkspace(path);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening workspace. " + e.ToString());
            }            
        }

        private bool CanDoCommand { get { return true; } }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class NewWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public NewWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string name;

            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                try
                {
                    _viewModel.WorkspaceManager.NewWorkspace(name, "");
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        private bool CanDoCommand 
        { 
            get { return true; } 
        }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class CloseWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public CloseWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            _viewModel.WorkspaceManager.CloseWorkspace();            
        }

        private bool CanDoCommand
        {
            get
            {
                return _viewModel.WorkspaceManager.WorkspaceOpen;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JadeData.Persistence.Workspace
{
    public static class Reader
    {
        private static JadeData.Project.IProject MakeProject(string workspaceDir, ProjectType xml)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                path = System.IO.Path.Combine(workspaceDir, path);
            }
            return Persistence.Project.Reader.Read(path);
        }

        private static JadeData.Workspace.IFolder MakeFolder(string workspaceDir, FolderType xml)
        {
            JadeData.Workspace.IFolder result = new JadeData.Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(workspaceDir, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(workspaceDir, p));
            }
            return result;
        }

        static public JadeData.Workspace.IWorkspace Read(string path)
        {
            WorkspaceType xml;
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceType));
            TextReader tr = new StreamReader(path);
            try
            {                
                xml = (WorkspaceType)serializer.Deserialize(tr);                
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }

            JadeData.Workspace.IWorkspace result = new JadeData.Workspace.Workspace(xml.Name, path);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result.Directory, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(result.Directory, p));
            }

            return result;
        }
    }

    public static class Writer
    {
        static private ProjectType MakeProject(JadeData.Workspace.ProjectItem proj, string workspaceDir)
        {
            ProjectType result = new ProjectType();

            string absPath = proj.Path;

            if (System.IO.Path.IsPathRooted(absPath) == false)
            {
                absPath = System.IO.Path.Combine(workspaceDir, absPath);
            }

            result.Path = proj.Path;
            Persistence.Project.Writer.Write(proj, absPath);
            return result;
        }

        static private FolderType MakeFolder(JadeData.Workspace.IFolder folder, string workspaceDir)
        {
            FolderType result = new FolderType();
            result.Name = folder.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for(int i=0;i<projTemps.Count;i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            return result;
        }

        static private string GetProjectPath(JadeData.Workspace.IWorkspace workspace, JadeData.Project.IProject proj)
        {
            return "";
        }

        static public string Write(JadeData.Workspace.IWorkspace workspace, string path)
        {
            string workspaceDir = System.IO.Path.GetDirectoryName(path);

            JadeData.Workspace.IFolder folder = workspace;
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for (int i = 0; i < projTemps.Count; i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            System.Xml.XmlDocument doc = new XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            System.IO.TextWriter stream = new System.IO.StreamWriter(path); 
            try
            {                    
                serializer.Serialize(stream, result);
                stream.Close();
                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;

namespace JadeGui.ViewModels
{
    using JadeData;

   
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IJadeViewModel
    {
        #region Data

        private JadeCore.IWorkspaceManager _workspaceManager;
        private Commands.NewWorkspace _newWorkspaceCmd;
        private Commands.OpenWorkspace _openWorkspaceCmd;
        private Commands.SaveWorkspace _saveWorkspaceCmd;
        private Commands.SaveAsWorkspace _saveAsWorkspaceCmd;
        private Commands.CloseWorkspace _closeWorkspaceCmd;

        private ICommand _exitCommand;

        #endregion

        public JadeViewModel()
        {
            _workspaceManager = new WorkspaceManager();
            _workspaceManager.WorkspaceOpened += delegate { OnPropertyChanged("Workspace"); };

            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();
            _newWorkspaceCmd = new Commands.NewWorkspace(this);
            _openWorkspaceCmd = new Commands.OpenWorkspace(this);
            _saveWorkspaceCmd = new Commands.SaveWorkspace(this);
            _saveAsWorkspaceCmd = new Commands.SaveAsWorkspace(this);
            _closeWorkspaceCmd = new Commands.CloseWorkspace(this);
        }

        #region EditorControl

        private JadeControls.EditorControl.ViewModel.EditorControlViewModel _editorModel;

        public JadeCore.ViewModels.IEditorViewModel Editor
        {
            get { return _editorModel; }
        }

        #endregion

        #region RequestClose [event]

        /// <summary>
        /// Raised to request ui to close
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

        #region Workspace Tree

        public JadeCore.ViewModels.IWorkspaceViewModel Workspace
        {
            get { return _workspaceManager.ViewModel; }
        }

        #endregion

        #region Commands

        public ICommand CloseWorkspaceCommand { get { return _closeWorkspaceCmd.Command; } }
        public ICommand NewWorkspaceCommand { get { return _newWorkspaceCmd.Command; } }
        public ICommand OpenWorkspaceCommand { get { return _openWorkspaceCmd.Command;}}
        public ICommand SaveWorkspaceCommand { get { return _saveWorkspaceCmd.Command; } }
        public ICommand SaveAsWorkspaceCommand { get { return _saveAsWorkspaceCmd.Command; } }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(delegate { OnRequestClose(); });
                }
                return _exitCommand;
            }
        }

        #endregion

        #region public Methods

        public bool OnExit()
        {
            if (_workspaceManager.RequiresSave)
            {
                return _workspaceManager.SaveOrDiscardWorkspace();
            }
            return true;
        }
       
        #endregion

        public JadeCore.IWorkspaceManager WorkspaceManager
        {
            get
            {
                return _workspaceManager;
            }
        }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class SaveWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path = _viewModel.Workspace.Path;

            if(path == null || path.Length == 0)
            {
                if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    return;                
            }
            _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.RequiresSave; } }
    }

    internal class SaveAsWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveAsWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path;

            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
            }
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.WorkspaceOpen; } }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class OpenWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command 
        { 
            get 
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            } 
        }

        public OpenWorkspace(JadeCore.ViewModels.IJadeViewModel vm)        
        {
            _viewModel = vm;     
        }

        private void OnCommand()
        {
            string path;
            if (JadeCore.GuiUtils.PromptOpenFile(".jws", "Jade Workspace files (.jws)|*.jws", true, out path) == false)
            {
                return;
            }

            try
            {
                _viewModel.WorkspaceManager.OpenWorkspace(path);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening workspace. " + e.ToString());
            }            
        }

        private bool CanDoCommand { get { return true; } }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class NewWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public NewWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string name;

            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                try
                {
                    _viewModel.WorkspaceManager.NewWorkspace(name, "");
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        private bool CanDoCommand 
        { 
            get { return true; } 
        }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class CloseWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public CloseWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            _viewModel.WorkspaceManager.CloseWorkspace();            
        }

        private bool CanDoCommand
        {
            get
            {
                return _viewModel.WorkspaceManager.WorkspaceOpen;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JadeData.Persistence.Workspace
{
    public static class Reader
    {
        private static JadeData.Project.IProject MakeProject(string workspaceDir, ProjectType xml)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                path = System.IO.Path.Combine(workspaceDir, path);
            }
            return Persistence.Project.Reader.Read(path);
        }

        private static JadeData.Workspace.IFolder MakeFolder(string workspaceDir, FolderType xml)
        {
            JadeData.Workspace.IFolder result = new JadeData.Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(workspaceDir, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(workspaceDir, p));
            }
            return result;
        }

        static public JadeData.Workspace.IWorkspace Read(string path)
        {
            WorkspaceType xml;
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceType));
            TextReader tr = new StreamReader(path);
            try
            {                
                xml = (WorkspaceType)serializer.Deserialize(tr);                
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }

            JadeData.Workspace.IWorkspace result = new JadeData.Workspace.Workspace(xml.Name, path);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result.Directory, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(result.Directory, p));
            }

            return result;
        }
    }

    public static class Writer
    {
        static private ProjectType MakeProject(JadeData.Workspace.ProjectItem proj, string workspaceDir)
        {
            ProjectType result = new ProjectType();

            string absPath = proj.Path;

            if (System.IO.Path.IsPathRooted(absPath) == false)
            {
                absPath = System.IO.Path.Combine(workspaceDir, absPath);
            }

            result.Path = proj.Path;
            Persistence.Project.Writer.Write(proj, absPath);
            return result;
        }

        static private FolderType MakeFolder(JadeData.Workspace.IFolder folder, string workspaceDir)
        {
            FolderType result = new FolderType();
            result.Name = folder.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for(int i=0;i<projTemps.Count;i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            return result;
        }

        static private string GetProjectPath(JadeData.Workspace.IWorkspace workspace, JadeData.Project.IProject proj)
        {
            return "";
        }

        static public string Write(JadeData.Workspace.IWorkspace workspace, string path)
        {
            string workspaceDir = System.IO.Path.GetDirectoryName(path);

            JadeData.Workspace.IFolder folder = workspace;
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for (int i = 0; i < projTemps.Count; i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            System.Xml.XmlDocument doc = new XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            System.IO.TextWriter stream = new System.IO.StreamWriter(path); 
            try
            {                    
                serializer.Serialize(stream, result);
                stream.Close();
                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;

namespace JadeGui.ViewModels
{
    using JadeData;

   
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IJadeViewModel
    {
        #region Data

        private JadeCore.IWorkspaceManager _workspaceManager;
        private Commands.NewWorkspace _newWorkspaceCmd;
        private Commands.OpenWorkspace _openWorkspaceCmd;
        private Commands.SaveWorkspace _saveWorkspaceCmd;
        private Commands.SaveAsWorkspace _saveAsWorkspaceCmd;
        private Commands.CloseWorkspace _closeWorkspaceCmd;

        private ICommand _exitCommand;

        #endregion

        public JadeViewModel()
        {
            _workspaceManager = new WorkspaceManager();
            _workspaceManager.WorkspaceOpened += delegate { OnPropertyChanged("Workspace"); };

            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();
            _newWorkspaceCmd = new Commands.NewWorkspace(this);
            _openWorkspaceCmd = new Commands.OpenWorkspace(this);
            _saveWorkspaceCmd = new Commands.SaveWorkspace(this);
            _saveAsWorkspaceCmd = new Commands.SaveAsWorkspace(this);
            _closeWorkspaceCmd = new Commands.CloseWorkspace(this);
        }

        #region EditorControl

        private JadeControls.EditorControl.ViewModel.EditorControlViewModel _editorModel;

        public JadeCore.ViewModels.IEditorViewModel Editor
        {
            get { return _editorModel; }
        }

        #endregion

        #region RequestClose [event]

        /// <summary>
        /// Raised to request ui to close
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

        #region Workspace Tree

        public JadeCore.ViewModels.IWorkspaceViewModel Workspace
        {
            get { return _workspaceManager.ViewModel; }
        }

        #endregion

        #region Commands

        public ICommand CloseWorkspaceCommand { get { return _closeWorkspaceCmd.Command; } }
        public ICommand NewWorkspaceCommand { get { return _newWorkspaceCmd.Command; } }
        public ICommand OpenWorkspaceCommand { get { return _openWorkspaceCmd.Command;}}
        public ICommand SaveWorkspaceCommand { get { return _saveWorkspaceCmd.Command; } }
        public ICommand SaveAsWorkspaceCommand { get { return _saveAsWorkspaceCmd.Command; } }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(delegate { OnRequestClose(); });
                }
                return _exitCommand;
            }
        }

        #endregion

        #region public Methods

        public bool OnExit()
        {
            if (_workspaceManager.RequiresSave)
            {
                return _workspaceManager.SaveOrDiscardWorkspace();
            }
            return true;
        }
       
        #endregion

        public JadeCore.IWorkspaceManager WorkspaceManager
        {
            get
            {
                return _workspaceManager;
            }
        }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class SaveWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path = _viewModel.Workspace.Path;

            if(path == null || path.Length == 0)
            {
                if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    return;                
            }
            _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.RequiresSave; } }
    }

    internal class SaveAsWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveAsWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path;

            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
            }
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.WorkspaceOpen; } }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class OpenWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command 
        { 
            get 
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            } 
        }

        public OpenWorkspace(JadeCore.ViewModels.IJadeViewModel vm)        
        {
            _viewModel = vm;     
        }

        private void OnCommand()
        {
            string path;
            if (JadeCore.GuiUtils.PromptOpenFile(".jws", "Jade Workspace files (.jws)|*.jws", true, out path) == false)
            {
                return;
            }

            try
            {
                _viewModel.WorkspaceManager.OpenWorkspace(path);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening workspace. " + e.ToString());
            }            
        }

        private bool CanDoCommand { get { return true; } }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class NewWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public NewWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string name;

            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                try
                {
                    _viewModel.WorkspaceManager.NewWorkspace(name, "");
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        private bool CanDoCommand 
        { 
            get { return true; } 
        }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class CloseWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public CloseWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            _viewModel.WorkspaceManager.CloseWorkspace();            
        }

        private bool CanDoCommand
        {
            get
            {
                return _viewModel.WorkspaceManager.WorkspaceOpen;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JadeData.Persistence.Workspace
{
    public static class Reader
    {
        private static JadeData.Project.IProject MakeProject(string workspaceDir, ProjectType xml)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                path = System.IO.Path.Combine(workspaceDir, path);
            }
            return Persistence.Project.Reader.Read(path);
        }

        private static JadeData.Workspace.IFolder MakeFolder(string workspaceDir, FolderType xml)
        {
            JadeData.Workspace.IFolder result = new JadeData.Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(workspaceDir, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(workspaceDir, p));
            }
            return result;
        }

        static public JadeData.Workspace.IWorkspace Read(string path)
        {
            WorkspaceType xml;
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceType));
            TextReader tr = new StreamReader(path);
            try
            {                
                xml = (WorkspaceType)serializer.Deserialize(tr);                
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }

            JadeData.Workspace.IWorkspace result = new JadeData.Workspace.Workspace(xml.Name, path);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result.Directory, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(result.Directory, p));
            }

            return result;
        }
    }

    public static class Writer
    {
        static private ProjectType MakeProject(JadeData.Workspace.ProjectItem proj, string workspaceDir)
        {
            ProjectType result = new ProjectType();

            string absPath = proj.Path;

            if (System.IO.Path.IsPathRooted(absPath) == false)
            {
                absPath = System.IO.Path.Combine(workspaceDir, absPath);
            }

            result.Path = proj.Path;
            Persistence.Project.Writer.Write(proj, absPath);
            return result;
        }

        static private FolderType MakeFolder(JadeData.Workspace.IFolder folder, string workspaceDir)
        {
            FolderType result = new FolderType();
            result.Name = folder.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for(int i=0;i<projTemps.Count;i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            return result;
        }

        static private string GetProjectPath(JadeData.Workspace.IWorkspace workspace, JadeData.Project.IProject proj)
        {
            return "";
        }

        static public string Write(JadeData.Workspace.IWorkspace workspace, string path)
        {
            string workspaceDir = System.IO.Path.GetDirectoryName(path);

            JadeData.Workspace.IFolder folder = workspace;
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for (int i = 0; i < projTemps.Count; i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            System.Xml.XmlDocument doc = new XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            System.IO.TextWriter stream = new System.IO.StreamWriter(path); 
            try
            {                    
                serializer.Serialize(stream, result);
                stream.Close();
                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;

namespace JadeGui.ViewModels
{
    using JadeData;

   
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IJadeViewModel
    {
        #region Data

        private JadeCore.IWorkspaceManager _workspaceManager;
        private Commands.NewWorkspace _newWorkspaceCmd;
        private Commands.OpenWorkspace _openWorkspaceCmd;
        private Commands.SaveWorkspace _saveWorkspaceCmd;
        private Commands.SaveAsWorkspace _saveAsWorkspaceCmd;
        private Commands.CloseWorkspace _closeWorkspaceCmd;

        private ICommand _exitCommand;

        #endregion

        public JadeViewModel()
        {
            _workspaceManager = new WorkspaceManager();
            _workspaceManager.WorkspaceOpened += delegate { OnPropertyChanged("Workspace"); };

            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();
            _newWorkspaceCmd = new Commands.NewWorkspace(this);
            _openWorkspaceCmd = new Commands.OpenWorkspace(this);
            _saveWorkspaceCmd = new Commands.SaveWorkspace(this);
            _saveAsWorkspaceCmd = new Commands.SaveAsWorkspace(this);
            _closeWorkspaceCmd = new Commands.CloseWorkspace(this);
        }

        #region EditorControl

        private JadeControls.EditorControl.ViewModel.EditorControlViewModel _editorModel;

        public JadeCore.ViewModels.IEditorViewModel Editor
        {
            get { return _editorModel; }
        }

        #endregion

        #region RequestClose [event]

        /// <summary>
        /// Raised to request ui to close
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

        #region Workspace Tree

        public JadeCore.ViewModels.IWorkspaceViewModel Workspace
        {
            get { return _workspaceManager.ViewModel; }
        }

        #endregion

        #region Commands

        public ICommand CloseWorkspaceCommand { get { return _closeWorkspaceCmd.Command; } }
        public ICommand NewWorkspaceCommand { get { return _newWorkspaceCmd.Command; } }
        public ICommand OpenWorkspaceCommand { get { return _openWorkspaceCmd.Command;}}
        public ICommand SaveWorkspaceCommand { get { return _saveWorkspaceCmd.Command; } }
        public ICommand SaveAsWorkspaceCommand { get { return _saveAsWorkspaceCmd.Command; } }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(delegate { OnRequestClose(); });
                }
                return _exitCommand;
            }
        }

        #endregion

        #region public Methods

        public bool OnExit()
        {
            if (_workspaceManager.RequiresSave)
            {
                return _workspaceManager.SaveOrDiscardWorkspace();
            }
            return true;
        }
       
        #endregion

        public JadeCore.IWorkspaceManager WorkspaceManager
        {
            get
            {
                return _workspaceManager;
            }
        }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class SaveWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path = _viewModel.Workspace.Path;

            if(path == null || path.Length == 0)
            {
                if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    return;                
            }
            _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.RequiresSave; } }
    }

    internal class SaveAsWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveAsWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path;

            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
            }
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.WorkspaceOpen; } }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class OpenWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command 
        { 
            get 
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            } 
        }

        public OpenWorkspace(JadeCore.ViewModels.IJadeViewModel vm)        
        {
            _viewModel = vm;     
        }

        private void OnCommand()
        {
            string path;
            if (JadeCore.GuiUtils.PromptOpenFile(".jws", "Jade Workspace files (.jws)|*.jws", true, out path) == false)
            {
                return;
            }

            try
            {
                _viewModel.WorkspaceManager.OpenWorkspace(path);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening workspace. " + e.ToString());
            }            
        }

        private bool CanDoCommand { get { return true; } }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class NewWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public NewWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string name;

            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                try
                {
                    _viewModel.WorkspaceManager.NewWorkspace(name, "");
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        private bool CanDoCommand 
        { 
            get { return true; } 
        }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class CloseWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public CloseWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            _viewModel.WorkspaceManager.CloseWorkspace();            
        }

        private bool CanDoCommand
        {
            get
            {
                return _viewModel.WorkspaceManager.WorkspaceOpen;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JadeData.Persistence.Workspace
{
    public static class Reader
    {
        private static JadeData.Project.IProject MakeProject(string workspaceDir, ProjectType xml)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                path = System.IO.Path.Combine(workspaceDir, path);
            }
            return Persistence.Project.Reader.Read(path);
        }

        private static JadeData.Workspace.IFolder MakeFolder(string workspaceDir, FolderType xml)
        {
            JadeData.Workspace.IFolder result = new JadeData.Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(workspaceDir, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(workspaceDir, p));
            }
            return result;
        }

        static public JadeData.Workspace.IWorkspace Read(string path)
        {
            WorkspaceType xml;
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceType));
            TextReader tr = new StreamReader(path);
            try
            {                
                xml = (WorkspaceType)serializer.Deserialize(tr);                
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }

            JadeData.Workspace.IWorkspace result = new JadeData.Workspace.Workspace(xml.Name, path);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result.Directory, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(result.Directory, p));
            }

            return result;
        }
    }

    public static class Writer
    {
        static private ProjectType MakeProject(JadeData.Workspace.ProjectItem proj, string workspaceDir)
        {
            ProjectType result = new ProjectType();

            string absPath = proj.Path;

            if (System.IO.Path.IsPathRooted(absPath) == false)
            {
                absPath = System.IO.Path.Combine(workspaceDir, absPath);
            }

            result.Path = proj.Path;
            Persistence.Project.Writer.Write(proj, absPath);
            return result;
        }

        static private FolderType MakeFolder(JadeData.Workspace.IFolder folder, string workspaceDir)
        {
            FolderType result = new FolderType();
            result.Name = folder.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for(int i=0;i<projTemps.Count;i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            return result;
        }

        static private string GetProjectPath(JadeData.Workspace.IWorkspace workspace, JadeData.Project.IProject proj)
        {
            return "";
        }

        static public string Write(JadeData.Workspace.IWorkspace workspace, string path)
        {
            string workspaceDir = System.IO.Path.GetDirectoryName(path);

            JadeData.Workspace.IFolder folder = workspace;
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for (int i = 0; i < projTemps.Count; i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            System.Xml.XmlDocument doc = new XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            System.IO.TextWriter stream = new System.IO.StreamWriter(path); 
            try
            {                    
                serializer.Serialize(stream, result);
                stream.Close();
                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;

namespace JadeGui.ViewModels
{
    using JadeData;

   
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IJadeViewModel
    {
        #region Data

        private JadeCore.IWorkspaceManager _workspaceManager;
        private Commands.NewWorkspace _newWorkspaceCmd;
        private Commands.OpenWorkspace _openWorkspaceCmd;
        private Commands.SaveWorkspace _saveWorkspaceCmd;
        private Commands.SaveAsWorkspace _saveAsWorkspaceCmd;
        private Commands.CloseWorkspace _closeWorkspaceCmd;

        private ICommand _exitCommand;

        #endregion

        public JadeViewModel()
        {
            _workspaceManager = new WorkspaceManager();
            _workspaceManager.WorkspaceOpened += delegate { OnPropertyChanged("Workspace"); };

            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();
            _newWorkspaceCmd = new Commands.NewWorkspace(this);
            _openWorkspaceCmd = new Commands.OpenWorkspace(this);
            _saveWorkspaceCmd = new Commands.SaveWorkspace(this);
            _saveAsWorkspaceCmd = new Commands.SaveAsWorkspace(this);
            _closeWorkspaceCmd = new Commands.CloseWorkspace(this);
        }

        #region EditorControl

        private JadeControls.EditorControl.ViewModel.EditorControlViewModel _editorModel;

        public JadeCore.ViewModels.IEditorViewModel Editor
        {
            get { return _editorModel; }
        }

        #endregion

        #region RequestClose [event]

        /// <summary>
        /// Raised to request ui to close
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

        #region Workspace Tree

        public JadeCore.ViewModels.IWorkspaceViewModel Workspace
        {
            get { return _workspaceManager.ViewModel; }
        }

        #endregion

        #region Commands

        public ICommand CloseWorkspaceCommand { get { return _closeWorkspaceCmd.Command; } }
        public ICommand NewWorkspaceCommand { get { return _newWorkspaceCmd.Command; } }
        public ICommand OpenWorkspaceCommand { get { return _openWorkspaceCmd.Command;}}
        public ICommand SaveWorkspaceCommand { get { return _saveWorkspaceCmd.Command; } }
        public ICommand SaveAsWorkspaceCommand { get { return _saveAsWorkspaceCmd.Command; } }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(delegate { OnRequestClose(); });
                }
                return _exitCommand;
            }
        }

        #endregion

        #region public Methods

        public bool OnExit()
        {
            if (_workspaceManager.RequiresSave)
            {
                return _workspaceManager.SaveOrDiscardWorkspace();
            }
            return true;
        }
       
        #endregion

        public JadeCore.IWorkspaceManager WorkspaceManager
        {
            get
            {
                return _workspaceManager;
            }
        }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class SaveWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path = _viewModel.Workspace.Path;

            if(path == null || path.Length == 0)
            {
                if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    return;                
            }
            _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.RequiresSave; } }
    }

    internal class SaveAsWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveAsWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path;

            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
            }
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.WorkspaceOpen; } }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class OpenWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command 
        { 
            get 
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            } 
        }

        public OpenWorkspace(JadeCore.ViewModels.IJadeViewModel vm)        
        {
            _viewModel = vm;     
        }

        private void OnCommand()
        {
            string path;
            if (JadeCore.GuiUtils.PromptOpenFile(".jws", "Jade Workspace files (.jws)|*.jws", true, out path) == false)
            {
                return;
            }

            try
            {
                _viewModel.WorkspaceManager.OpenWorkspace(path);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening workspace. " + e.ToString());
            }            
        }

        private bool CanDoCommand { get { return true; } }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class NewWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public NewWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string name;

            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                try
                {
                    _viewModel.WorkspaceManager.NewWorkspace(name, "");
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        private bool CanDoCommand 
        { 
            get { return true; } 
        }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class CloseWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public CloseWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            _viewModel.WorkspaceManager.CloseWorkspace();            
        }

        private bool CanDoCommand
        {
            get
            {
                return _viewModel.WorkspaceManager.WorkspaceOpen;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JadeData.Persistence.Workspace
{
    public static class Reader
    {
        private static JadeData.Project.IProject MakeProject(string workspaceDir, ProjectType xml)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                path = System.IO.Path.Combine(workspaceDir, path);
            }
            return Persistence.Project.Reader.Read(path);
        }

        private static JadeData.Workspace.IFolder MakeFolder(string workspaceDir, FolderType xml)
        {
            JadeData.Workspace.IFolder result = new JadeData.Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(workspaceDir, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(workspaceDir, p));
            }
            return result;
        }

        static public JadeData.Workspace.IWorkspace Read(string path)
        {
            WorkspaceType xml;
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceType));
            TextReader tr = new StreamReader(path);
            try
            {                
                xml = (WorkspaceType)serializer.Deserialize(tr);                
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }

            JadeData.Workspace.IWorkspace result = new JadeData.Workspace.Workspace(xml.Name, path);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result.Directory, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(result.Directory, p));
            }

            return result;
        }
    }

    public static class Writer
    {
        static private ProjectType MakeProject(JadeData.Workspace.ProjectItem proj, string workspaceDir)
        {
            ProjectType result = new ProjectType();

            string absPath = proj.Path;

            if (System.IO.Path.IsPathRooted(absPath) == false)
            {
                absPath = System.IO.Path.Combine(workspaceDir, absPath);
            }

            result.Path = proj.Path;
            Persistence.Project.Writer.Write(proj, absPath);
            return result;
        }

        static private FolderType MakeFolder(JadeData.Workspace.IFolder folder, string workspaceDir)
        {
            FolderType result = new FolderType();
            result.Name = folder.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for(int i=0;i<projTemps.Count;i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            return result;
        }

        static private string GetProjectPath(JadeData.Workspace.IWorkspace workspace, JadeData.Project.IProject proj)
        {
            return "";
        }

        static public string Write(JadeData.Workspace.IWorkspace workspace, string path)
        {
            string workspaceDir = System.IO.Path.GetDirectoryName(path);

            JadeData.Workspace.IFolder folder = workspace;
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for (int i = 0; i < projTemps.Count; i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            System.Xml.XmlDocument doc = new XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            System.IO.TextWriter stream = new System.IO.StreamWriter(path); 
            try
            {                    
                serializer.Serialize(stream, result);
                stream.Close();
                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;

namespace JadeGui.ViewModels
{
    using JadeData;

   
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IJadeViewModel
    {
        #region Data

        private JadeCore.IWorkspaceManager _workspaceManager;
        private Commands.NewWorkspace _newWorkspaceCmd;
        private Commands.OpenWorkspace _openWorkspaceCmd;
        private Commands.SaveWorkspace _saveWorkspaceCmd;
        private Commands.SaveAsWorkspace _saveAsWorkspaceCmd;
        private Commands.CloseWorkspace _closeWorkspaceCmd;

        private ICommand _exitCommand;

        #endregion

        public JadeViewModel()
        {
            _workspaceManager = new WorkspaceManager();
            _workspaceManager.WorkspaceOpened += delegate { OnPropertyChanged("Workspace"); };

            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();
            _newWorkspaceCmd = new Commands.NewWorkspace(this);
            _openWorkspaceCmd = new Commands.OpenWorkspace(this);
            _saveWorkspaceCmd = new Commands.SaveWorkspace(this);
            _saveAsWorkspaceCmd = new Commands.SaveAsWorkspace(this);
            _closeWorkspaceCmd = new Commands.CloseWorkspace(this);
        }

        #region EditorControl

        private JadeControls.EditorControl.ViewModel.EditorControlViewModel _editorModel;

        public JadeCore.ViewModels.IEditorViewModel Editor
        {
            get { return _editorModel; }
        }

        #endregion

        #region RequestClose [event]

        /// <summary>
        /// Raised to request ui to close
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

        #region Workspace Tree

        public JadeCore.ViewModels.IWorkspaceViewModel Workspace
        {
            get { return _workspaceManager.ViewModel; }
        }

        #endregion

        #region Commands

        public ICommand CloseWorkspaceCommand { get { return _closeWorkspaceCmd.Command; } }
        public ICommand NewWorkspaceCommand { get { return _newWorkspaceCmd.Command; } }
        public ICommand OpenWorkspaceCommand { get { return _openWorkspaceCmd.Command;}}
        public ICommand SaveWorkspaceCommand { get { return _saveWorkspaceCmd.Command; } }
        public ICommand SaveAsWorkspaceCommand { get { return _saveAsWorkspaceCmd.Command; } }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(delegate { OnRequestClose(); });
                }
                return _exitCommand;
            }
        }

        #endregion

        #region public Methods

        public bool OnExit()
        {
            if (_workspaceManager.RequiresSave)
            {
                return _workspaceManager.SaveOrDiscardWorkspace();
            }
            return true;
        }
       
        #endregion

        public JadeCore.IWorkspaceManager WorkspaceManager
        {
            get
            {
                return _workspaceManager;
            }
        }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class SaveWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path = _viewModel.Workspace.Path;

            if(path == null || path.Length == 0)
            {
                if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    return;                
            }
            _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.RequiresSave; } }
    }

    internal class SaveAsWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveAsWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path;

            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
            }
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.WorkspaceOpen; } }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class OpenWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command 
        { 
            get 
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            } 
        }

        public OpenWorkspace(JadeCore.ViewModels.IJadeViewModel vm)        
        {
            _viewModel = vm;     
        }

        private void OnCommand()
        {
            string path;
            if (JadeCore.GuiUtils.PromptOpenFile(".jws", "Jade Workspace files (.jws)|*.jws", true, out path) == false)
            {
                return;
            }

            try
            {
                _viewModel.WorkspaceManager.OpenWorkspace(path);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening workspace. " + e.ToString());
            }            
        }

        private bool CanDoCommand { get { return true; } }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class NewWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public NewWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string name;

            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                try
                {
                    _viewModel.WorkspaceManager.NewWorkspace(name, "");
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        private bool CanDoCommand 
        { 
            get { return true; } 
        }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class CloseWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public CloseWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            _viewModel.WorkspaceManager.CloseWorkspace();            
        }

        private bool CanDoCommand
        {
            get
            {
                return _viewModel.WorkspaceManager.WorkspaceOpen;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JadeData.Persistence.Workspace
{
    public static class Reader
    {
        private static JadeData.Project.IProject MakeProject(string workspaceDir, ProjectType xml)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                path = System.IO.Path.Combine(workspaceDir, path);
            }
            return Persistence.Project.Reader.Read(path);
        }

        private static JadeData.Workspace.IFolder MakeFolder(string workspaceDir, FolderType xml)
        {
            JadeData.Workspace.IFolder result = new JadeData.Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(workspaceDir, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(workspaceDir, p));
            }
            return result;
        }

        static public JadeData.Workspace.IWorkspace Read(string path)
        {
            WorkspaceType xml;
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceType));
            TextReader tr = new StreamReader(path);
            try
            {                
                xml = (WorkspaceType)serializer.Deserialize(tr);                
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }

            JadeData.Workspace.IWorkspace result = new JadeData.Workspace.Workspace(xml.Name, path);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result.Directory, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(result.Directory, p));
            }

            return result;
        }
    }

    public static class Writer
    {
        static private ProjectType MakeProject(JadeData.Workspace.ProjectItem proj, string workspaceDir)
        {
            ProjectType result = new ProjectType();

            string absPath = proj.Path;

            if (System.IO.Path.IsPathRooted(absPath) == false)
            {
                absPath = System.IO.Path.Combine(workspaceDir, absPath);
            }

            result.Path = proj.Path;
            Persistence.Project.Writer.Write(proj, absPath);
            return result;
        }

        static private FolderType MakeFolder(JadeData.Workspace.IFolder folder, string workspaceDir)
        {
            FolderType result = new FolderType();
            result.Name = folder.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for(int i=0;i<projTemps.Count;i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            return result;
        }

        static private string GetProjectPath(JadeData.Workspace.IWorkspace workspace, JadeData.Project.IProject proj)
        {
            return "";
        }

        static public string Write(JadeData.Workspace.IWorkspace workspace, string path)
        {
            string workspaceDir = System.IO.Path.GetDirectoryName(path);

            JadeData.Workspace.IFolder folder = workspace;
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for (int i = 0; i < projTemps.Count; i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            System.Xml.XmlDocument doc = new XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            System.IO.TextWriter stream = new System.IO.StreamWriter(path); 
            try
            {                    
                serializer.Serialize(stream, result);
                stream.Close();
                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;

namespace JadeGui.ViewModels
{
    using JadeData;

   
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IJadeViewModel
    {
        #region Data

        private JadeCore.IWorkspaceManager _workspaceManager;
        private Commands.NewWorkspace _newWorkspaceCmd;
        private Commands.OpenWorkspace _openWorkspaceCmd;
        private Commands.SaveWorkspace _saveWorkspaceCmd;
        private Commands.SaveAsWorkspace _saveAsWorkspaceCmd;
        private Commands.CloseWorkspace _closeWorkspaceCmd;

        private ICommand _exitCommand;

        #endregion

        public JadeViewModel()
        {
            _workspaceManager = new WorkspaceManager();
            _workspaceManager.WorkspaceOpened += delegate { OnPropertyChanged("Workspace"); };

            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();
            _newWorkspaceCmd = new Commands.NewWorkspace(this);
            _openWorkspaceCmd = new Commands.OpenWorkspace(this);
            _saveWorkspaceCmd = new Commands.SaveWorkspace(this);
            _saveAsWorkspaceCmd = new Commands.SaveAsWorkspace(this);
            _closeWorkspaceCmd = new Commands.CloseWorkspace(this);
        }

        #region EditorControl

        private JadeControls.EditorControl.ViewModel.EditorControlViewModel _editorModel;

        public JadeCore.ViewModels.IEditorViewModel Editor
        {
            get { return _editorModel; }
        }

        #endregion

        #region RequestClose [event]

        /// <summary>
        /// Raised to request ui to close
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

        #region Workspace Tree

        public JadeCore.ViewModels.IWorkspaceViewModel Workspace
        {
            get { return _workspaceManager.ViewModel; }
        }

        #endregion

        #region Commands

        public ICommand CloseWorkspaceCommand { get { return _closeWorkspaceCmd.Command; } }
        public ICommand NewWorkspaceCommand { get { return _newWorkspaceCmd.Command; } }
        public ICommand OpenWorkspaceCommand { get { return _openWorkspaceCmd.Command;}}
        public ICommand SaveWorkspaceCommand { get { return _saveWorkspaceCmd.Command; } }
        public ICommand SaveAsWorkspaceCommand { get { return _saveAsWorkspaceCmd.Command; } }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(delegate { OnRequestClose(); });
                }
                return _exitCommand;
            }
        }

        #endregion

        #region public Methods

        public bool OnExit()
        {
            if (_workspaceManager.RequiresSave)
            {
                return _workspaceManager.SaveOrDiscardWorkspace();
            }
            return true;
        }
       
        #endregion

        public JadeCore.IWorkspaceManager WorkspaceManager
        {
            get
            {
                return _workspaceManager;
            }
        }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class SaveWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path = _viewModel.Workspace.Path;

            if(path == null || path.Length == 0)
            {
                if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    return;                
            }
            _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.RequiresSave; } }
    }

    internal class SaveAsWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveAsWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path;

            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
            }
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.WorkspaceOpen; } }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class OpenWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command 
        { 
            get 
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            } 
        }

        public OpenWorkspace(JadeCore.ViewModels.IJadeViewModel vm)        
        {
            _viewModel = vm;     
        }

        private void OnCommand()
        {
            string path;
            if (JadeCore.GuiUtils.PromptOpenFile(".jws", "Jade Workspace files (.jws)|*.jws", true, out path) == false)
            {
                return;
            }

            try
            {
                _viewModel.WorkspaceManager.OpenWorkspace(path);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening workspace. " + e.ToString());
            }            
        }

        private bool CanDoCommand { get { return true; } }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class NewWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public NewWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string name;

            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                try
                {
                    _viewModel.WorkspaceManager.NewWorkspace(name, "");
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        private bool CanDoCommand 
        { 
            get { return true; } 
        }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class CloseWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public CloseWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            _viewModel.WorkspaceManager.CloseWorkspace();            
        }

        private bool CanDoCommand
        {
            get
            {
                return _viewModel.WorkspaceManager.WorkspaceOpen;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JadeData.Persistence.Workspace
{
    public static class Reader
    {
        private static JadeData.Project.IProject MakeProject(string workspaceDir, ProjectType xml)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                path = System.IO.Path.Combine(workspaceDir, path);
            }
            return Persistence.Project.Reader.Read(path);
        }

        private static JadeData.Workspace.IFolder MakeFolder(string workspaceDir, FolderType xml)
        {
            JadeData.Workspace.IFolder result = new JadeData.Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(workspaceDir, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(workspaceDir, p));
            }
            return result;
        }

        static public JadeData.Workspace.IWorkspace Read(string path)
        {
            WorkspaceType xml;
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceType));
            TextReader tr = new StreamReader(path);
            try
            {                
                xml = (WorkspaceType)serializer.Deserialize(tr);                
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }

            JadeData.Workspace.IWorkspace result = new JadeData.Workspace.Workspace(xml.Name, path);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result.Directory, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(result.Directory, p));
            }

            return result;
        }
    }

    public static class Writer
    {
        static private ProjectType MakeProject(JadeData.Workspace.ProjectItem proj, string workspaceDir)
        {
            ProjectType result = new ProjectType();

            string absPath = proj.Path;

            if (System.IO.Path.IsPathRooted(absPath) == false)
            {
                absPath = System.IO.Path.Combine(workspaceDir, absPath);
            }

            result.Path = proj.Path;
            Persistence.Project.Writer.Write(proj, absPath);
            return result;
        }

        static private FolderType MakeFolder(JadeData.Workspace.IFolder folder, string workspaceDir)
        {
            FolderType result = new FolderType();
            result.Name = folder.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for(int i=0;i<projTemps.Count;i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            return result;
        }

        static private string GetProjectPath(JadeData.Workspace.IWorkspace workspace, JadeData.Project.IProject proj)
        {
            return "";
        }

        static public string Write(JadeData.Workspace.IWorkspace workspace, string path)
        {
            string workspaceDir = System.IO.Path.GetDirectoryName(path);

            JadeData.Workspace.IFolder folder = workspace;
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for (int i = 0; i < projTemps.Count; i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            System.Xml.XmlDocument doc = new XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            System.IO.TextWriter stream = new System.IO.StreamWriter(path); 
            try
            {                    
                serializer.Serialize(stream, result);
                stream.Close();
                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;

namespace JadeGui.ViewModels
{
    using JadeData;

   
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IJadeViewModel
    {
        #region Data

        private JadeCore.IWorkspaceManager _workspaceManager;
        private Commands.NewWorkspace _newWorkspaceCmd;
        private Commands.OpenWorkspace _openWorkspaceCmd;
        private Commands.SaveWorkspace _saveWorkspaceCmd;
        private Commands.SaveAsWorkspace _saveAsWorkspaceCmd;
        private Commands.CloseWorkspace _closeWorkspaceCmd;

        private ICommand _exitCommand;

        #endregion

        public JadeViewModel()
        {
            _workspaceManager = new WorkspaceManager();
            _workspaceManager.WorkspaceOpened += delegate { OnPropertyChanged("Workspace"); };

            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();
            _newWorkspaceCmd = new Commands.NewWorkspace(this);
            _openWorkspaceCmd = new Commands.OpenWorkspace(this);
            _saveWorkspaceCmd = new Commands.SaveWorkspace(this);
            _saveAsWorkspaceCmd = new Commands.SaveAsWorkspace(this);
            _closeWorkspaceCmd = new Commands.CloseWorkspace(this);
        }

        #region EditorControl

        private JadeControls.EditorControl.ViewModel.EditorControlViewModel _editorModel;

        public JadeCore.ViewModels.IEditorViewModel Editor
        {
            get { return _editorModel; }
        }

        #endregion

        #region RequestClose [event]

        /// <summary>
        /// Raised to request ui to close
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

        #region Workspace Tree

        public JadeCore.ViewModels.IWorkspaceViewModel Workspace
        {
            get { return _workspaceManager.ViewModel; }
        }

        #endregion

        #region Commands

        public ICommand CloseWorkspaceCommand { get { return _closeWorkspaceCmd.Command; } }
        public ICommand NewWorkspaceCommand { get { return _newWorkspaceCmd.Command; } }
        public ICommand OpenWorkspaceCommand { get { return _openWorkspaceCmd.Command;}}
        public ICommand SaveWorkspaceCommand { get { return _saveWorkspaceCmd.Command; } }
        public ICommand SaveAsWorkspaceCommand { get { return _saveAsWorkspaceCmd.Command; } }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(delegate { OnRequestClose(); });
                }
                return _exitCommand;
            }
        }

        #endregion

        #region public Methods

        public bool OnExit()
        {
            if (_workspaceManager.RequiresSave)
            {
                return _workspaceManager.SaveOrDiscardWorkspace();
            }
            return true;
        }
       
        #endregion

        public JadeCore.IWorkspaceManager WorkspaceManager
        {
            get
            {
                return _workspaceManager;
            }
        }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class SaveWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path = _viewModel.Workspace.Path;

            if(path == null || path.Length == 0)
            {
                if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    return;                
            }
            _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.RequiresSave; } }
    }

    internal class SaveAsWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveAsWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path;

            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
            }
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.WorkspaceOpen; } }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class OpenWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command 
        { 
            get 
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            } 
        }

        public OpenWorkspace(JadeCore.ViewModels.IJadeViewModel vm)        
        {
            _viewModel = vm;     
        }

        private void OnCommand()
        {
            string path;
            if (JadeCore.GuiUtils.PromptOpenFile(".jws", "Jade Workspace files (.jws)|*.jws", true, out path) == false)
            {
                return;
            }

            try
            {
                _viewModel.WorkspaceManager.OpenWorkspace(path);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening workspace. " + e.ToString());
            }            
        }

        private bool CanDoCommand { get { return true; } }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class NewWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public NewWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string name;

            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                try
                {
                    _viewModel.WorkspaceManager.NewWorkspace(name, "");
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        private bool CanDoCommand 
        { 
            get { return true; } 
        }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class CloseWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public CloseWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            _viewModel.WorkspaceManager.CloseWorkspace();            
        }

        private bool CanDoCommand
        {
            get
            {
                return _viewModel.WorkspaceManager.WorkspaceOpen;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JadeData.Persistence.Workspace
{
    public static class Reader
    {
        private static JadeData.Project.IProject MakeProject(string workspaceDir, ProjectType xml)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                path = System.IO.Path.Combine(workspaceDir, path);
            }
            return Persistence.Project.Reader.Read(path);
        }

        private static JadeData.Workspace.IFolder MakeFolder(string workspaceDir, FolderType xml)
        {
            JadeData.Workspace.IFolder result = new JadeData.Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(workspaceDir, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(workspaceDir, p));
            }
            return result;
        }

        static public JadeData.Workspace.IWorkspace Read(string path)
        {
            WorkspaceType xml;
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceType));
            TextReader tr = new StreamReader(path);
            try
            {                
                xml = (WorkspaceType)serializer.Deserialize(tr);                
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }

            JadeData.Workspace.IWorkspace result = new JadeData.Workspace.Workspace(xml.Name, path);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result.Directory, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(result.Directory, p));
            }

            return result;
        }
    }

    public static class Writer
    {
        static private ProjectType MakeProject(JadeData.Workspace.ProjectItem proj, string workspaceDir)
        {
            ProjectType result = new ProjectType();

            string absPath = proj.Path;

            if (System.IO.Path.IsPathRooted(absPath) == false)
            {
                absPath = System.IO.Path.Combine(workspaceDir, absPath);
            }

            result.Path = proj.Path;
            Persistence.Project.Writer.Write(proj, absPath);
            return result;
        }

        static private FolderType MakeFolder(JadeData.Workspace.IFolder folder, string workspaceDir)
        {
            FolderType result = new FolderType();
            result.Name = folder.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for(int i=0;i<projTemps.Count;i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            return result;
        }

        static private string GetProjectPath(JadeData.Workspace.IWorkspace workspace, JadeData.Project.IProject proj)
        {
            return "";
        }

        static public string Write(JadeData.Workspace.IWorkspace workspace, string path)
        {
            string workspaceDir = System.IO.Path.GetDirectoryName(path);

            JadeData.Workspace.IFolder folder = workspace;
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for (int i = 0; i < projTemps.Count; i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            System.Xml.XmlDocument doc = new XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            System.IO.TextWriter stream = new System.IO.StreamWriter(path); 
            try
            {                    
                serializer.Serialize(stream, result);
                stream.Close();
                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;

namespace JadeGui.ViewModels
{
    using JadeData;

   
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IJadeViewModel
    {
        #region Data

        private JadeCore.IWorkspaceManager _workspaceManager;
        private Commands.NewWorkspace _newWorkspaceCmd;
        private Commands.OpenWorkspace _openWorkspaceCmd;
        private Commands.SaveWorkspace _saveWorkspaceCmd;
        private Commands.SaveAsWorkspace _saveAsWorkspaceCmd;
        private Commands.CloseWorkspace _closeWorkspaceCmd;

        private ICommand _exitCommand;

        #endregion

        public JadeViewModel()
        {
            _workspaceManager = new WorkspaceManager();
            _workspaceManager.WorkspaceOpened += delegate { OnPropertyChanged("Workspace"); };

            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();
            _newWorkspaceCmd = new Commands.NewWorkspace(this);
            _openWorkspaceCmd = new Commands.OpenWorkspace(this);
            _saveWorkspaceCmd = new Commands.SaveWorkspace(this);
            _saveAsWorkspaceCmd = new Commands.SaveAsWorkspace(this);
            _closeWorkspaceCmd = new Commands.CloseWorkspace(this);
        }

        #region EditorControl

        private JadeControls.EditorControl.ViewModel.EditorControlViewModel _editorModel;

        public JadeCore.ViewModels.IEditorViewModel Editor
        {
            get { return _editorModel; }
        }

        #endregion

        #region RequestClose [event]

        /// <summary>
        /// Raised to request ui to close
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

        #region Workspace Tree

        public JadeCore.ViewModels.IWorkspaceViewModel Workspace
        {
            get { return _workspaceManager.ViewModel; }
        }

        #endregion

        #region Commands

        public ICommand CloseWorkspaceCommand { get { return _closeWorkspaceCmd.Command; } }
        public ICommand NewWorkspaceCommand { get { return _newWorkspaceCmd.Command; } }
        public ICommand OpenWorkspaceCommand { get { return _openWorkspaceCmd.Command;}}
        public ICommand SaveWorkspaceCommand { get { return _saveWorkspaceCmd.Command; } }
        public ICommand SaveAsWorkspaceCommand { get { return _saveAsWorkspaceCmd.Command; } }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(delegate { OnRequestClose(); });
                }
                return _exitCommand;
            }
        }

        #endregion

        #region public Methods

        public bool OnExit()
        {
            if (_workspaceManager.RequiresSave)
            {
                return _workspaceManager.SaveOrDiscardWorkspace();
            }
            return true;
        }
       
        #endregion

        public JadeCore.IWorkspaceManager WorkspaceManager
        {
            get
            {
                return _workspaceManager;
            }
        }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class SaveWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path = _viewModel.Workspace.Path;

            if(path == null || path.Length == 0)
            {
                if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    return;                
            }
            _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.RequiresSave; } }
    }

    internal class SaveAsWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveAsWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path;

            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
            }
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.WorkspaceOpen; } }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class OpenWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command 
        { 
            get 
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            } 
        }

        public OpenWorkspace(JadeCore.ViewModels.IJadeViewModel vm)        
        {
            _viewModel = vm;     
        }

        private void OnCommand()
        {
            string path;
            if (JadeCore.GuiUtils.PromptOpenFile(".jws", "Jade Workspace files (.jws)|*.jws", true, out path) == false)
            {
                return;
            }

            try
            {
                _viewModel.WorkspaceManager.OpenWorkspace(path);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening workspace. " + e.ToString());
            }            
        }

        private bool CanDoCommand { get { return true; } }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class NewWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public NewWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string name;

            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                try
                {
                    _viewModel.WorkspaceManager.NewWorkspace(name, "");
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        private bool CanDoCommand 
        { 
            get { return true; } 
        }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class CloseWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public CloseWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            _viewModel.WorkspaceManager.CloseWorkspace();            
        }

        private bool CanDoCommand
        {
            get
            {
                return _viewModel.WorkspaceManager.WorkspaceOpen;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JadeData.Persistence.Workspace
{
    public static class Reader
    {
        private static JadeData.Project.IProject MakeProject(string workspaceDir, ProjectType xml)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                path = System.IO.Path.Combine(workspaceDir, path);
            }
            return Persistence.Project.Reader.Read(path);
        }

        private static JadeData.Workspace.IFolder MakeFolder(string workspaceDir, FolderType xml)
        {
            JadeData.Workspace.IFolder result = new JadeData.Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(workspaceDir, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(workspaceDir, p));
            }
            return result;
        }

        static public JadeData.Workspace.IWorkspace Read(string path)
        {
            WorkspaceType xml;
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceType));
            TextReader tr = new StreamReader(path);
            try
            {                
                xml = (WorkspaceType)serializer.Deserialize(tr);                
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }

            JadeData.Workspace.IWorkspace result = new JadeData.Workspace.Workspace(xml.Name, path);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result.Directory, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(result.Directory, p));
            }

            return result;
        }
    }

    public static class Writer
    {
        static private ProjectType MakeProject(JadeData.Workspace.ProjectItem proj, string workspaceDir)
        {
            ProjectType result = new ProjectType();

            string absPath = proj.Path;

            if (System.IO.Path.IsPathRooted(absPath) == false)
            {
                absPath = System.IO.Path.Combine(workspaceDir, absPath);
            }

            result.Path = proj.Path;
            Persistence.Project.Writer.Write(proj, absPath);
            return result;
        }

        static private FolderType MakeFolder(JadeData.Workspace.IFolder folder, string workspaceDir)
        {
            FolderType result = new FolderType();
            result.Name = folder.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for(int i=0;i<projTemps.Count;i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            return result;
        }

        static private string GetProjectPath(JadeData.Workspace.IWorkspace workspace, JadeData.Project.IProject proj)
        {
            return "";
        }

        static public string Write(JadeData.Workspace.IWorkspace workspace, string path)
        {
            string workspaceDir = System.IO.Path.GetDirectoryName(path);

            JadeData.Workspace.IFolder folder = workspace;
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for (int i = 0; i < projTemps.Count; i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            System.Xml.XmlDocument doc = new XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            System.IO.TextWriter stream = new System.IO.StreamWriter(path); 
            try
            {                    
                serializer.Serialize(stream, result);
               stream.Close();
                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}
 }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;

namespace JadeGui.ViewModels
{
    using JadeData;

   
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IJadeViewModel
    {
        #region Data

        private JadeCore.IWorkspaceManager _workspaceManager;
        private Commands.NewWorkspace _newWorkspaceCmd;
        private Commands.OpenWorkspace _openWorkspaceCmd;
        private Commands.SaveWorkspace _saveWorkspaceCmd;
        private Commands.SaveAsWorkspace _saveAsWorkspaceCmd;
        private Commands.CloseWorkspace _closeWorkspaceCmd;

        private ICommand _exitCommand;

        #endregion

        public JadeViewModel()
        {
            _workspaceManager = new WorkspaceManager();
            _workspaceManager.WorkspaceOpened += delegate { OnPropertyChanged("Workspace"); };

            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();
            _newWorkspaceCmd = new Commands.NewWorkspace(this);
            _openWorkspaceCmd = new Commands.OpenWorkspace(this);
            _saveWorkspaceCmd = new Commands.SaveWorkspace(this);
            _saveAsWorkspaceCmd = new Commands.SaveAsWorkspace(this);
            _closeWorkspaceCmd = new Commands.CloseWorkspace(this);
        }

        #region EditorControl

        private JadeControls.EditorControl.ViewModel.EditorControlViewModel _editorModel;

        public JadeCore.ViewModels.IEditorViewModel Editor
        {
            get { return _editorModel; }
        }

        #endregion

        #region RequestClose [event]

        /// <summary>
        /// Raised to request ui to close
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

        #region Workspace Tree

        public JadeCore.ViewModels.IWorkspaceViewModel Workspace
        {
            get { return _workspaceManager.ViewModel; }
        }

        #endregion

        #region Commands

        public ICommand CloseWorkspaceCommand { get { return _closeWorkspaceCmd.Command; } }
        public ICommand NewWorkspaceCommand { get { return _newWorkspaceCmd.Command; } }
        public ICommand OpenWorkspaceCommand { get { return _openWorkspaceCmd.Command;}}
        public ICommand SaveWorkspaceCommand { get { return _saveWorkspaceCmd.Command; } }
        public ICommand SaveAsWorkspaceCommand { get { return _saveAsWorkspaceCmd.Command; } }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(delegate { OnRequestClose(); });
                }
                return _exitCommand;
            }
        }

        #endregion

        #region public Methods

        public bool OnExit()
        {
            if (_workspaceManager.RequiresSave)
            {
                return _workspaceManager.SaveOrDiscardWorkspace();
            }
            return true;
        }
       
        #endregion

        public JadeCore.IWorkspaceManager WorkspaceManager
        {
            get
            {
                return _workspaceManager;
            }
        }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class SaveWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path = _viewModel.Workspace.Path;

            if(path == null || path.Length == 0)
            {
                if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    return;                
            }
            _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.RequiresSave; } }
    }

    internal class SaveAsWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveAsWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path;

            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
            }
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.WorkspaceOpen; } }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class OpenWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command 
        { 
            get 
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            } 
        }

        public OpenWorkspace(JadeCore.ViewModels.IJadeViewModel vm)        
        {
            _viewModel = vm;     
        }

        private void OnCommand()
        {
            string path;
            if (JadeCore.GuiUtils.PromptOpenFile(".jws", "Jade Workspace files (.jws)|*.jws", true, out path) == false)
            {
                return;
            }

            try
            {
                _viewModel.WorkspaceManager.OpenWorkspace(path);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening workspace. " + e.ToString());
            }            
        }

        private bool CanDoCommand { get { return true; } }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class NewWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public NewWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string name;

            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                try
                {
                    _viewModel.WorkspaceManager.NewWorkspace(name, "");
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        private bool CanDoCommand 
        { 
            get { return true; } 
        }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class CloseWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public CloseWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            _viewModel.WorkspaceManager.CloseWorkspace();            
        }

        private bool CanDoCommand
        {
            get
            {
                return _viewModel.WorkspaceManager.WorkspaceOpen;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JadeData.Persistence.Workspace
{
    public static class Reader
    {
        private static JadeData.Project.IProject MakeProject(string workspaceDir, ProjectType xml)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                path = System.IO.Path.Combine(workspaceDir, path);
            }
            return Persistence.Project.Reader.Read(path);
        }

        private static JadeData.Workspace.IFolder MakeFolder(string workspaceDir, FolderType xml)
        {
            JadeData.Workspace.IFolder result = new JadeData.Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(workspaceDir, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(workspaceDir, p));
            }
            return result;
        }

        static public JadeData.Workspace.IWorkspace Read(string path)
        {
            WorkspaceType xml;
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceType));
            TextReader tr = new StreamReader(path);
            try
            {                
                xml = (WorkspaceType)serializer.Deserialize(tr);                
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }

            JadeData.Workspace.IWorkspace result = new JadeData.Workspace.Workspace(xml.Name, path);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result.Directory, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(result.Directory, p));
            }

            return result;
        }
    }

    public static class Writer
    {
        static private ProjectType MakeProject(JadeData.Workspace.ProjectItem proj, string workspaceDir)
        {
            ProjectType result = new ProjectType();

            string absPath = proj.Path;

            if (System.IO.Path.IsPathRooted(absPath) == false)
            {
                absPath = System.IO.Path.Combine(workspaceDir, absPath);
            }

            result.Path = proj.Path;
            Persistence.Project.Writer.Write(proj, absPath);
            return result;
        }

        static private FolderType MakeFolder(JadeData.Workspace.IFolder folder, string workspaceDir)
        {
            FolderType result = new FolderType();
            result.Name = folder.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for(int i=0;i<projTemps.Count;i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            return result;
        }

        static private string GetProjectPath(JadeData.Workspace.IWorkspace workspace, JadeData.Project.IProject proj)
        {
            return "";
        }

        static public string Write(JadeData.Workspace.IWorkspace workspace, string path)
        {
            string workspaceDir = System.IO.Path.GetDirectoryName(path);

            JadeData.Workspace.IFolder folder = workspace;
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for (int i = 0; i < projTemps.Count; i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            System.Xml.XmlDocument doc = new XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            System.IO.TextWriter stream = new System.IO.StreamWriter(path); 
            try
            {                    
                serializer.Serialize(stream, result);
                stream.Close();
                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;

namespace JadeGui.ViewModels
{
    using JadeData;

   
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IJadeViewModel
    {
        #region Data

        private JadeCore.IWorkspaceManager _workspaceManager;
        private Commands.NewWorkspace _newWorkspaceCmd;
        private Commands.OpenWorkspace _openWorkspaceCmd;
        private Commands.SaveWorkspace _saveWorkspaceCmd;
        private Commands.SaveAsWorkspace _saveAsWorkspaceCmd;
        private Commands.CloseWorkspace _closeWorkspaceCmd;

        private ICommand _exitCommand;

        #endregion

        public JadeViewModel()
        {
            _workspaceManager = new WorkspaceManager();
            _workspaceManager.WorkspaceOpened += delegate { OnPropertyChanged("Workspace"); };

            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();
            _newWorkspaceCmd = new Commands.NewWorkspace(this);
            _openWorkspaceCmd = new Commands.OpenWorkspace(this);
            _saveWorkspaceCmd = new Commands.SaveWorkspace(this);
            _saveAsWorkspaceCmd = new Commands.SaveAsWorkspace(this);
            _closeWorkspaceCmd = new Commands.CloseWorkspace(this);
        }

        #region EditorControl

        private JadeControls.EditorControl.ViewModel.EditorControlViewModel _editorModel;

        public JadeCore.ViewModels.IEditorViewModel Editor
        {
            get { return _editorModel; }
        }

        #endregion

        #region RequestClose [event]

        /// <summary>
        /// Raised to request ui to close
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

        #region Workspace Tree

        public JadeCore.ViewModels.IWorkspaceViewModel Workspace
        {
            get { return _workspaceManager.ViewModel; }
        }

        #endregion

        #region Commands

        public ICommand CloseWorkspaceCommand { get { return _closeWorkspaceCmd.Command; } }
        public ICommand NewWorkspaceCommand { get { return _newWorkspaceCmd.Command; } }
        public ICommand OpenWorkspaceCommand { get { return _openWorkspaceCmd.Command;}}
        public ICommand SaveWorkspaceCommand { get { return _saveWorkspaceCmd.Command; } }
        public ICommand SaveAsWorkspaceCommand { get { return _saveAsWorkspaceCmd.Command; } }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(delegate { OnRequestClose(); });
                }
                return _exitCommand;
            }
        }

        #endregion

        #region public Methods

        public bool OnExit()
        {
            if (_workspaceManager.RequiresSave)
            {
                return _workspaceManager.SaveOrDiscardWorkspace();
            }
            return true;
        }
       
        #endregion

        public JadeCore.IWorkspaceManager WorkspaceManager
        {
            get
            {
                return _workspaceManager;
            }
        }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class SaveWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path = _viewModel.Workspace.Path;

            if(path == null || path.Length == 0)
            {
                if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    return;                
            }
            _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.RequiresSave; } }
    }

    internal class SaveAsWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveAsWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path;

            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
            }
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.WorkspaceOpen; } }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class OpenWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command 
        { 
            get 
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            } 
        }

        public OpenWorkspace(JadeCore.ViewModels.IJadeViewModel vm)        
        {
            _viewModel = vm;     
        }

        private void OnCommand()
        {
            string path;
            if (JadeCore.GuiUtils.PromptOpenFile(".jws", "Jade Workspace files (.jws)|*.jws", true, out path) == false)
            {
                return;
            }

            try
            {
                _viewModel.WorkspaceManager.OpenWorkspace(path);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening workspace. " + e.ToString());
            }            
        }

        private bool CanDoCommand { get { return true; } }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class NewWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public NewWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string name;

            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                try
                {
                    _viewModel.WorkspaceManager.NewWorkspace(name, "");
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        private bool CanDoCommand 
        { 
            get { return true; } 
        }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class CloseWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public CloseWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            _viewModel.WorkspaceManager.CloseWorkspace();            
        }

        private bool CanDoCommand
        {
            get
            {
                return _viewModel.WorkspaceManager.WorkspaceOpen;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JadeData.Persistence.Workspace
{
    public static class Reader
    {
        private static JadeData.Project.IProject MakeProject(string workspaceDir, ProjectType xml)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                path = System.IO.Path.Combine(workspaceDir, path);
            }
            return Persistence.Project.Reader.Read(path);
        }

        private static JadeData.Workspace.IFolder MakeFolder(string workspaceDir, FolderType xml)
        {
            JadeData.Workspace.IFolder result = new JadeData.Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(workspaceDir, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(workspaceDir, p));
            }
            return result;
        }

        static public JadeData.Workspace.IWorkspace Read(string path)
        {
            WorkspaceType xml;
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceType));
            TextReader tr = new StreamReader(path);
            try
            {                
                xml = (WorkspaceType)serializer.Deserialize(tr);                
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }

            JadeData.Workspace.IWorkspace result = new JadeData.Workspace.Workspace(xml.Name, path);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result.Directory, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(result.Directory, p));
            }

            return result;
        }
    }

    public static class Writer
    {
        static private ProjectType MakeProject(JadeData.Workspace.ProjectItem proj, string workspaceDir)
        {
            ProjectType result = new ProjectType();

            string absPath = proj.Path;

            if (System.IO.Path.IsPathRooted(absPath) == false)
            {
                absPath = System.IO.Path.Combine(workspaceDir, absPath);
            }

            result.Path = proj.Path;
            Persistence.Project.Writer.Write(proj, absPath);
            return result;
        }

        static private FolderType MakeFolder(JadeData.Workspace.IFolder folder, string workspaceDir)
        {
            FolderType result = new FolderType();
            result.Name = folder.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for(int i=0;i<projTemps.Count;i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            return result;
        }

        static private string GetProjectPath(JadeData.Workspace.IWorkspace workspace, JadeData.Project.IProject proj)
        {
            return "";
        }

        static public string Write(JadeData.Workspace.IWorkspace workspace, string path)
        {
            string workspaceDir = System.IO.Path.GetDirectoryName(path);

            JadeData.Workspace.IFolder folder = workspace;
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for (int i = 0; i < projTemps.Count; i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            System.Xml.XmlDocument doc = new XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            System.IO.TextWriter stream = new System.IO.StreamWriter(path); 
            try
            {                    
                serializer.Serialize(stream, result);
                stream.Close();
                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;

namespace JadeGui.ViewModels
{
    using JadeData;

   
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IJadeViewModel
    {
        #region Data

        private JadeCore.IWorkspaceManager _workspaceManager;
        private Commands.NewWorkspace _newWorkspaceCmd;
        private Commands.OpenWorkspace _openWorkspaceCmd;
        private Commands.SaveWorkspace _saveWorkspaceCmd;
        private Commands.SaveAsWorkspace _saveAsWorkspaceCmd;
        private Commands.CloseWorkspace _closeWorkspaceCmd;

        private ICommand _exitCommand;

        #endregion

        public JadeViewModel()
        {
            _workspaceManager = new WorkspaceManager();
            _workspaceManager.WorkspaceOpened += delegate { OnPropertyChanged("Workspace"); };

            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();
            _newWorkspaceCmd = new Commands.NewWorkspace(this);
            _openWorkspaceCmd = new Commands.OpenWorkspace(this);
            _saveWorkspaceCmd = new Commands.SaveWorkspace(this);
            _saveAsWorkspaceCmd = new Commands.SaveAsWorkspace(this);
            _closeWorkspaceCmd = new Commands.CloseWorkspace(this);
        }

        #region EditorControl

        private JadeControls.EditorControl.ViewModel.EditorControlViewModel _editorModel;

        public JadeCore.ViewModels.IEditorViewModel Editor
        {
            get { return _editorModel; }
        }

        #endregion

        #region RequestClose [event]

        /// <summary>
        /// Raised to request ui to close
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

        #region Workspace Tree

        public JadeCore.ViewModels.IWorkspaceViewModel Workspace
        {
            get { return _workspaceManager.ViewModel; }
        }

        #endregion

        #region Commands

        public ICommand CloseWorkspaceCommand { get { return _closeWorkspaceCmd.Command; } }
        public ICommand NewWorkspaceCommand { get { return _newWorkspaceCmd.Command; } }
        public ICommand OpenWorkspaceCommand { get { return _openWorkspaceCmd.Command;}}
        public ICommand SaveWorkspaceCommand { get { return _saveWorkspaceCmd.Command; } }
        public ICommand SaveAsWorkspaceCommand { get { return _saveAsWorkspaceCmd.Command; } }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(delegate { OnRequestClose(); });
                }
                return _exitCommand;
            }
        }

        #endregion

        #region public Methods

        public bool OnExit()
        {
            if (_workspaceManager.RequiresSave)
            {
                return _workspaceManager.SaveOrDiscardWorkspace();
            }
            return true;
        }
       
        #endregion

        public JadeCore.IWorkspaceManager WorkspaceManager
        {
            get
            {
                return _workspaceManager;
            }
        }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class SaveWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path = _viewModel.Workspace.Path;

            if(path == null || path.Length == 0)
            {
                if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    return;                
            }
            _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.RequiresSave; } }
    }

    internal class SaveAsWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveAsWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path;

            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
            }
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.WorkspaceOpen; } }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class OpenWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command 
        { 
            get 
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            } 
        }

        public OpenWorkspace(JadeCore.ViewModels.IJadeViewModel vm)        
        {
            _viewModel = vm;     
        }

        private void OnCommand()
        {
            string path;
            if (JadeCore.GuiUtils.PromptOpenFile(".jws", "Jade Workspace files (.jws)|*.jws", true, out path) == false)
            {
                return;
            }

            try
            {
                _viewModel.WorkspaceManager.OpenWorkspace(path);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening workspace. " + e.ToString());
            }            
        }

        private bool CanDoCommand { get { return true; } }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class NewWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public NewWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string name;

            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                try
                {
                    _viewModel.WorkspaceManager.NewWorkspace(name, "");
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        private bool CanDoCommand 
        { 
            get { return true; } 
        }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class CloseWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public CloseWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            _viewModel.WorkspaceManager.CloseWorkspace();            
        }

        private bool CanDoCommand
        {
            get
            {
                return _viewModel.WorkspaceManager.WorkspaceOpen;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JadeData.Persistence.Workspace
{
    public static class Reader
    {
        private static JadeData.Project.IProject MakeProject(string workspaceDir, ProjectType xml)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                path = System.IO.Path.Combine(workspaceDir, path);
            }
            return Persistence.Project.Reader.Read(path);
        }

        private static JadeData.Workspace.IFolder MakeFolder(string workspaceDir, FolderType xml)
        {
            JadeData.Workspace.IFolder result = new JadeData.Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(workspaceDir, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(workspaceDir, p));
            }
            return result;
        }

        static public JadeData.Workspace.IWorkspace Read(string path)
        {
            WorkspaceType xml;
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceType));
            TextReader tr = new StreamReader(path);
            try
            {                
                xml = (WorkspaceType)serializer.Deserialize(tr);                
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }

            JadeData.Workspace.IWorkspace result = new JadeData.Workspace.Workspace(xml.Name, path);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result.Directory, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(result.Directory, p));
            }

            return result;
        }
    }

    public static class Writer
    {
        static private ProjectType MakeProject(JadeData.Workspace.ProjectItem proj, string workspaceDir)
        {
            ProjectType result = new ProjectType();

            string absPath = proj.Path;

            if (System.IO.Path.IsPathRooted(absPath) == false)
            {
                absPath = System.IO.Path.Combine(workspaceDir, absPath);
            }

            result.Path = proj.Path;
            Persistence.Project.Writer.Write(proj, absPath);
            return result;
        }

        static private FolderType MakeFolder(JadeData.Workspace.IFolder folder, string workspaceDir)
        {
            FolderType result = new FolderType();
            result.Name = folder.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for(int i=0;i<projTemps.Count;i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            return result;
        }

        static private string GetProjectPath(JadeData.Workspace.IWorkspace workspace, JadeData.Project.IProject proj)
        {
            return "";
        }

        static public string Write(JadeData.Workspace.IWorkspace workspace, string path)
        {
            string workspaceDir = System.IO.Path.GetDirectoryName(path);

            JadeData.Workspace.IFolder folder = workspace;
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for (int i = 0; i < projTemps.Count; i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            System.Xml.XmlDocument doc = new XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            System.IO.TextWriter stream = new System.IO.StreamWriter(path); 
            try
            {                    
                serializer.Serialize(stream, result);
                stream.Close();
                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;

namespace JadeGui.ViewModels
{
    using JadeData;

   
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IJadeViewModel
    {
        #region Data

        private JadeCore.IWorkspaceManager _workspaceManager;
        private Commands.NewWorkspace _newWorkspaceCmd;
        private Commands.OpenWorkspace _openWorkspaceCmd;
        private Commands.SaveWorkspace _saveWorkspaceCmd;
        private Commands.SaveAsWorkspace _saveAsWorkspaceCmd;
        private Commands.CloseWorkspace _closeWorkspaceCmd;

        private ICommand _exitCommand;

        #endregion

        public JadeViewModel()
        {
            _workspaceManager = new WorkspaceManager();
            _workspaceManager.WorkspaceOpened += delegate { OnPropertyChanged("Workspace"); };

            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();
            _newWorkspaceCmd = new Commands.NewWorkspace(this);
            _openWorkspaceCmd = new Commands.OpenWorkspace(this);
            _saveWorkspaceCmd = new Commands.SaveWorkspace(this);
            _saveAsWorkspaceCmd = new Commands.SaveAsWorkspace(this);
            _closeWorkspaceCmd = new Commands.CloseWorkspace(this);
        }

        #region EditorControl

        private JadeControls.EditorControl.ViewModel.EditorControlViewModel _editorModel;

        public JadeCore.ViewModels.IEditorViewModel Editor
        {
            get { return _editorModel; }
        }

        #endregion

        #region RequestClose [event]

        /// <summary>
        /// Raised to request ui to close
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

        #region Workspace Tree

        public JadeCore.ViewModels.IWorkspaceViewModel Workspace
        {
            get { return _workspaceManager.ViewModel; }
        }

        #endregion

        #region Commands

        public ICommand CloseWorkspaceCommand { get { return _closeWorkspaceCmd.Command; } }
        public ICommand NewWorkspaceCommand { get { return _newWorkspaceCmd.Command; } }
        public ICommand OpenWorkspaceCommand { get { return _openWorkspaceCmd.Command;}}
        public ICommand SaveWorkspaceCommand { get { return _saveWorkspaceCmd.Command; } }
        public ICommand SaveAsWorkspaceCommand { get { return _saveAsWorkspaceCmd.Command; } }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(delegate { OnRequestClose(); });
                }
                return _exitCommand;
            }
        }

        #endregion

        #region public Methods

        public bool OnExit()
        {
            if (_workspaceManager.RequiresSave)
            {
                return _workspaceManager.SaveOrDiscardWorkspace();
            }
            return true;
        }
       
        #endregion

        public JadeCore.IWorkspaceManager WorkspaceManager
        {
            get
            {
                return _workspaceManager;
            }
        }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class SaveWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path = _viewModel.Workspace.Path;

            if(path == null || path.Length == 0)
            {
                if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    return;                
            }
            _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.RequiresSave; } }
    }

    internal class SaveAsWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveAsWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path;

            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
            }
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.WorkspaceOpen; } }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class OpenWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command 
        { 
            get 
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            } 
        }

        public OpenWorkspace(JadeCore.ViewModels.IJadeViewModel vm)        
        {
            _viewModel = vm;     
        }

        private void OnCommand()
        {
            string path;
            if (JadeCore.GuiUtils.PromptOpenFile(".jws", "Jade Workspace files (.jws)|*.jws", true, out path) == false)
            {
                return;
            }

            try
            {
                _viewModel.WorkspaceManager.OpenWorkspace(path);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening workspace. " + e.ToString());
            }            
        }

        private bool CanDoCommand { get { return true; } }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class NewWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public NewWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string name;

            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                try
                {
                    _viewModel.WorkspaceManager.NewWorkspace(name, "");
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        private bool CanDoCommand 
        { 
            get { return true; } 
        }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class CloseWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public CloseWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            _viewModel.WorkspaceManager.CloseWorkspace();            
        }

        private bool CanDoCommand
        {
            get
            {
                return _viewModel.WorkspaceManager.WorkspaceOpen;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JadeData.Persistence.Workspace
{
    public static class Reader
    {
        private static JadeData.Project.IProject MakeProject(string workspaceDir, ProjectType xml)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                path = System.IO.Path.Combine(workspaceDir, path);
            }
            return Persistence.Project.Reader.Read(path);
        }

        private static JadeData.Workspace.IFolder MakeFolder(string workspaceDir, FolderType xml)
        {
            JadeData.Workspace.IFolder result = new JadeData.Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(workspaceDir, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(workspaceDir, p));
            }
            return result;
        }

        static public JadeData.Workspace.IWorkspace Read(string path)
        {
            WorkspaceType xml;
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceType));
            TextReader tr = new StreamReader(path);
            try
            {                
                xml = (WorkspaceType)serializer.Deserialize(tr);                
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }

            JadeData.Workspace.IWorkspace result = new JadeData.Workspace.Workspace(xml.Name, path);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result.Directory, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(result.Directory, p));
            }

            return result;
        }
    }

    public static class Writer
    {
        static private ProjectType MakeProject(JadeData.Workspace.ProjectItem proj, string workspaceDir)
        {
            ProjectType result = new ProjectType();

            string absPath = proj.Path;

            if (System.IO.Path.IsPathRooted(absPath) == false)
            {
                absPath = System.IO.Path.Combine(workspaceDir, absPath);
            }

            result.Path = proj.Path;
            Persistence.Project.Writer.Write(proj, absPath);
            return result;
        }

        static private FolderType MakeFolder(JadeData.Workspace.IFolder folder, string workspaceDir)
        {
            FolderType result = new FolderType();
            result.Name = folder.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for(int i=0;i<projTemps.Count;i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            return result;
        }

        static private string GetProjectPath(JadeData.Workspace.IWorkspace workspace, JadeData.Project.IProject proj)
        {
            return "";
        }

        static public string Write(JadeData.Workspace.IWorkspace workspace, string path)
        {
            string workspaceDir = System.IO.Path.GetDirectoryName(path);

            JadeData.Workspace.IFolder folder = workspace;
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for (int i = 0; i < projTemps.Count; i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            System.Xml.XmlDocument doc = new XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            System.IO.TextWriter stream = new System.IO.StreamWriter(path); 
            try
            {                    
                serializer.Serialize(stream, result);
                stream.Close();
                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;

namespace JadeGui.ViewModels
{
    using JadeData;

   
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IJadeViewModel
    {
        #region Data

        private JadeCore.IWorkspaceManager _workspaceManager;
        private Commands.NewWorkspace _newWorkspaceCmd;
        private Commands.OpenWorkspace _openWorkspaceCmd;
        private Commands.SaveWorkspace _saveWorkspaceCmd;
        private Commands.SaveAsWorkspace _saveAsWorkspaceCmd;
        private Commands.CloseWorkspace _closeWorkspaceCmd;

        private ICommand _exitCommand;

        #endregion

        public JadeViewModel()
        {
            _workspaceManager = new WorkspaceManager();
            _workspaceManager.WorkspaceOpened += delegate { OnPropertyChanged("Workspace"); };

            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();
            _newWorkspaceCmd = new Commands.NewWorkspace(this);
            _openWorkspaceCmd = new Commands.OpenWorkspace(this);
            _saveWorkspaceCmd = new Commands.SaveWorkspace(this);
            _saveAsWorkspaceCmd = new Commands.SaveAsWorkspace(this);
            _closeWorkspaceCmd = new Commands.CloseWorkspace(this);
        }

        #region EditorControl

        private JadeControls.EditorControl.ViewModel.EditorControlViewModel _editorModel;

        public JadeCore.ViewModels.IEditorViewModel Editor
        {
            get { return _editorModel; }
        }

        #endregion

        #region RequestClose [event]

        /// <summary>
        /// Raised to request ui to close
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

        #region Workspace Tree

        public JadeCore.ViewModels.IWorkspaceViewModel Workspace
        {
            get { return _workspaceManager.ViewModel; }
        }

        #endregion

        #region Commands

        public ICommand CloseWorkspaceCommand { get { return _closeWorkspaceCmd.Command; } }
        public ICommand NewWorkspaceCommand { get { return _newWorkspaceCmd.Command; } }
        public ICommand OpenWorkspaceCommand { get { return _openWorkspaceCmd.Command;}}
        public ICommand SaveWorkspaceCommand { get { return _saveWorkspaceCmd.Command; } }
        public ICommand SaveAsWorkspaceCommand { get { return _saveAsWorkspaceCmd.Command; } }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(delegate { OnRequestClose(); });
                }
                return _exitCommand;
            }
        }

        #endregion

        #region public Methods

        public bool OnExit()
        {
            if (_workspaceManager.RequiresSave)
            {
                return _workspaceManager.SaveOrDiscardWorkspace();
            }
            return true;
        }
       
        #endregion

        public JadeCore.IWorkspaceManager WorkspaceManager
        {
            get
            {
                return _workspaceManager;
            }
        }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class SaveWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path = _viewModel.Workspace.Path;

            if(path == null || path.Length == 0)
            {
                if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    return;                
            }
            _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.RequiresSave; } }
    }

    internal class SaveAsWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveAsWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path;

            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
            }
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.WorkspaceOpen; } }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class OpenWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command 
        { 
            get 
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            } 
        }

        public OpenWorkspace(JadeCore.ViewModels.IJadeViewModel vm)        
        {
            _viewModel = vm;     
        }

        private void OnCommand()
        {
            string path;
            if (JadeCore.GuiUtils.PromptOpenFile(".jws", "Jade Workspace files (.jws)|*.jws", true, out path) == false)
            {
                return;
            }

            try
            {
                _viewModel.WorkspaceManager.OpenWorkspace(path);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening workspace. " + e.ToString());
            }            
        }

        private bool CanDoCommand { get { return true; } }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class NewWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public NewWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string name;

            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                try
                {
                    _viewModel.WorkspaceManager.NewWorkspace(name, "");
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        private bool CanDoCommand 
        { 
            get { return true; } 
        }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class CloseWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public CloseWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            _viewModel.WorkspaceManager.CloseWorkspace();            
        }

        private bool CanDoCommand
        {
            get
            {
                return _viewModel.WorkspaceManager.WorkspaceOpen;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JadeData.Persistence.Workspace
{
    public static class Reader
    {
        private static JadeData.Project.IProject MakeProject(string workspaceDir, ProjectType xml)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                path = System.IO.Path.Combine(workspaceDir, path);
            }
            return Persistence.Project.Reader.Read(path);
        }

        private static JadeData.Workspace.IFolder MakeFolder(string workspaceDir, FolderType xml)
        {
            JadeData.Workspace.IFolder result = new JadeData.Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(workspaceDir, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(workspaceDir, p));
            }
            return result;
        }

        static public JadeData.Workspace.IWorkspace Read(string path)
        {
            WorkspaceType xml;
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceType));
            TextReader tr = new StreamReader(path);
            try
            {                
                xml = (WorkspaceType)serializer.Deserialize(tr);                
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }

            JadeData.Workspace.IWorkspace result = new JadeData.Workspace.Workspace(xml.Name, path);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result.Directory, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(result.Directory, p));
            }

            return result;
        }
    }

    public static class Writer
    {
        static private ProjectType MakeProject(JadeData.Workspace.ProjectItem proj, string workspaceDir)
        {
            ProjectType result = new ProjectType();

            string absPath = proj.Path;

            if (System.IO.Path.IsPathRooted(absPath) == false)
            {
                absPath = System.IO.Path.Combine(workspaceDir, absPath);
            }

            result.Path = proj.Path;
            Persistence.Project.Writer.Write(proj, absPath);
            return result;
        }

        static private FolderType MakeFolder(JadeData.Workspace.IFolder folder, string workspaceDir)
        {
            FolderType result = new FolderType();
            result.Name = folder.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for(int i=0;i<projTemps.Count;i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            return result;
        }

        static private string GetProjectPath(JadeData.Workspace.IWorkspace workspace, JadeData.Project.IProject proj)
        {
            return "";
        }

        static public string Write(JadeData.Workspace.IWorkspace workspace, string path)
        {
            string workspaceDir = System.IO.Path.GetDirectoryName(path);

            JadeData.Workspace.IFolder folder = workspace;
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for (int i = 0; i < projTemps.Count; i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            System.Xml.XmlDocument doc = new XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            System.IO.TextWriter stream = new System.IO.StreamWriter(path); 
            try
            {                    
                serializer.Serialize(stream, result);
                stream.Close();
                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;

namespace JadeGui.ViewModels
{
    using JadeData;

   
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IJadeViewModel
    {
        #region Data

        private JadeCore.IWorkspaceManager _workspaceManager;
        private Commands.NewWorkspace _newWorkspaceCmd;
        private Commands.OpenWorkspace _openWorkspaceCmd;
        private Commands.SaveWorkspace _saveWorkspaceCmd;
        private Commands.SaveAsWorkspace _saveAsWorkspaceCmd;
        private Commands.CloseWorkspace _closeWorkspaceCmd;

        private ICommand _exitCommand;

        #endregion

        public JadeViewModel()
        {
            _workspaceManager = new WorkspaceManager();
            _workspaceManager.WorkspaceOpened += delegate { OnPropertyChanged("Workspace"); };

            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();
            _newWorkspaceCmd = new Commands.NewWorkspace(this);
            _openWorkspaceCmd = new Commands.OpenWorkspace(this);
            _saveWorkspaceCmd = new Commands.SaveWorkspace(this);
            _saveAsWorkspaceCmd = new Commands.SaveAsWorkspace(this);
            _closeWorkspaceCmd = new Commands.CloseWorkspace(this);
        }

        #region EditorControl

        private JadeControls.EditorControl.ViewModel.EditorControlViewModel _editorModel;

        public JadeCore.ViewModels.IEditorViewModel Editor
        {
            get { return _editorModel; }
        }

        #endregion

        #region RequestClose [event]

        /// <summary>
        /// Raised to request ui to close
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

        #region Workspace Tree

        public JadeCore.ViewModels.IWorkspaceViewModel Workspace
        {
            get { return _workspaceManager.ViewModel; }
        }

        #endregion

        #region Commands

        public ICommand CloseWorkspaceCommand { get { return _closeWorkspaceCmd.Command; } }
        public ICommand NewWorkspaceCommand { get { return _newWorkspaceCmd.Command; } }
        public ICommand OpenWorkspaceCommand { get { return _openWorkspaceCmd.Command;}}
        public ICommand SaveWorkspaceCommand { get { return _saveWorkspaceCmd.Command; } }
        public ICommand SaveAsWorkspaceCommand { get { return _saveAsWorkspaceCmd.Command; } }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(delegate { OnRequestClose(); });
                }
                return _exitCommand;
            }
        }

        #endregion

        #region public Methods

        public bool OnExit()
        {
            if (_workspaceManager.RequiresSave)
            {
                return _workspaceManager.SaveOrDiscardWorkspace();
            }
            return true;
        }
       
        #endregion

        public JadeCore.IWorkspaceManager WorkspaceManager
        {
            get
            {
                return _workspaceManager;
            }
        }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class SaveWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path = _viewModel.Workspace.Path;

            if(path == null || path.Length == 0)
            {
                if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    return;                
            }
            _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.RequiresSave; } }
    }

    internal class SaveAsWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveAsWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path;

            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
            }
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.WorkspaceOpen; } }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class OpenWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command 
        { 
            get 
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            } 
        }

        public OpenWorkspace(JadeCore.ViewModels.IJadeViewModel vm)        
        {
            _viewModel = vm;     
        }

        private void OnCommand()
        {
            string path;
            if (JadeCore.GuiUtils.PromptOpenFile(".jws", "Jade Workspace files (.jws)|*.jws", true, out path) == false)
            {
                return;
            }

            try
            {
                _viewModel.WorkspaceManager.OpenWorkspace(path);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening workspace. " + e.ToString());
            }            
        }

        private bool CanDoCommand { get { return true; } }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class NewWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public NewWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string name;

            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                try
                {
                    _viewModel.WorkspaceManager.NewWorkspace(name, "");
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        private bool CanDoCommand 
        { 
            get { return true; } 
        }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class CloseWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public CloseWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            _viewModel.WorkspaceManager.CloseWorkspace();            
        }

        private bool CanDoCommand
        {
            get
            {
                return _viewModel.WorkspaceManager.WorkspaceOpen;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JadeData.Persistence.Workspace
{
    public static class Reader
    {
        private static JadeData.Project.IProject MakeProject(string workspaceDir, ProjectType xml)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                path = System.IO.Path.Combine(workspaceDir, path);
            }
            return Persistence.Project.Reader.Read(path);
        }

        private static JadeData.Workspace.IFolder MakeFolder(string workspaceDir, FolderType xml)
        {
            JadeData.Workspace.IFolder result = new JadeData.Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(workspaceDir, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(workspaceDir, p));
            }
            return result;
        }

        static public JadeData.Workspace.IWorkspace Read(string path)
        {
            WorkspaceType xml;
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceType));
            TextReader tr = new StreamReader(path);
            try
            {                
                xml = (WorkspaceType)serializer.Deserialize(tr);                
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }

            JadeData.Workspace.IWorkspace result = new JadeData.Workspace.Workspace(xml.Name, path);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result.Directory, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(result.Directory, p));
            }

            return result;
        }
    }

    public static class Writer
    {
        static private ProjectType MakeProject(JadeData.Workspace.ProjectItem proj, string workspaceDir)
        {
            ProjectType result = new ProjectType();

            string absPath = proj.Path;

            if (System.IO.Path.IsPathRooted(absPath) == false)
            {
                absPath = System.IO.Path.Combine(workspaceDir, absPath);
            }

            result.Path = proj.Path;
            Persistence.Project.Writer.Write(proj, absPath);
            return result;
        }

        static private FolderType MakeFolder(JadeData.Workspace.IFolder folder, string workspaceDir)
        {
            FolderType result = new FolderType();
            result.Name = folder.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for(int i=0;i<projTemps.Count;i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            return result;
        }

        static private string GetProjectPath(JadeData.Workspace.IWorkspace workspace, JadeData.Project.IProject proj)
        {
            return "";
        }

        static public string Write(JadeData.Workspace.IWorkspace workspace, string path)
        {
            string workspaceDir = System.IO.Path.GetDirectoryName(path);

            JadeData.Workspace.IFolder folder = workspace;
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for (int i = 0; i < projTemps.Count; i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            System.Xml.XmlDocument doc = new XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            System.IO.TextWriter stream = new System.IO.StreamWriter(path); 
            try
            {                    
                serializer.Serialize(stream, result);
                stream.Close();
                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;

namespace JadeGui.ViewModels
{
    using JadeData;

   
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IJadeViewModel
    {
        #region Data

        private JadeCore.IWorkspaceManager _workspaceManager;
        private Commands.NewWorkspace _newWorkspaceCmd;
        private Commands.OpenWorkspace _openWorkspaceCmd;
        private Commands.SaveWorkspace _saveWorkspaceCmd;
        private Commands.SaveAsWorkspace _saveAsWorkspaceCmd;
        private Commands.CloseWorkspace _closeWorkspaceCmd;

        private ICommand _exitCommand;

        #endregion

        public JadeViewModel()
        {
            _workspaceManager = new WorkspaceManager();
            _workspaceManager.WorkspaceOpened += delegate { OnPropertyChanged("Workspace"); };

            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();
            _newWorkspaceCmd = new Commands.NewWorkspace(this);
            _openWorkspaceCmd = new Commands.OpenWorkspace(this);
            _saveWorkspaceCmd = new Commands.SaveWorkspace(this);
            _saveAsWorkspaceCmd = new Commands.SaveAsWorkspace(this);
            _closeWorkspaceCmd = new Commands.CloseWorkspace(this);
        }

        #region EditorControl

        private JadeControls.EditorControl.ViewModel.EditorControlViewModel _editorModel;

        public JadeCore.ViewModels.IEditorViewModel Editor
        {
            get { return _editorModel; }
        }

        #endregion

        #region RequestClose [event]

        /// <summary>
        /// Raised to request ui to close
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

        #region Workspace Tree

        public JadeCore.ViewModels.IWorkspaceViewModel Workspace
        {
            get { return _workspaceManager.ViewModel; }
        }

        #endregion

        #region Commands

        public ICommand CloseWorkspaceCommand { get { return _closeWorkspaceCmd.Command; } }
        public ICommand NewWorkspaceCommand { get { return _newWorkspaceCmd.Command; } }
        public ICommand OpenWorkspaceCommand { get { return _openWorkspaceCmd.Command;}}
        public ICommand SaveWorkspaceCommand { get { return _saveWorkspaceCmd.Command; } }
        public ICommand SaveAsWorkspaceCommand { get { return _saveAsWorkspaceCmd.Command; } }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(delegate { OnRequestClose(); });
                }
                return _exitCommand;
            }
        }

        #endregion

        #region public Methods

        public bool OnExit()
        {
            if (_workspaceManager.RequiresSave)
            {
                return _workspaceManager.SaveOrDiscardWorkspace();
            }
            return true;
        }
       
        #endregion

        public JadeCore.IWorkspaceManager WorkspaceManager
        {
            get
            {
                return _workspaceManager;
            }
        }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class SaveWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path = _viewModel.Workspace.Path;

            if(path == null || path.Length == 0)
            {
                if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    return;                
            }
            _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.RequiresSave; } }
    }

    internal class SaveAsWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveAsWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path;

            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
            }
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.WorkspaceOpen; } }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class OpenWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command 
        { 
            get 
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            } 
        }

        public OpenWorkspace(JadeCore.ViewModels.IJadeViewModel vm)        
        {
            _viewModel = vm;     
        }

        private void OnCommand()
        {
            string path;
            if (JadeCore.GuiUtils.PromptOpenFile(".jws", "Jade Workspace files (.jws)|*.jws", true, out path) == false)
            {
                return;
            }

            try
            {
                _viewModel.WorkspaceManager.OpenWorkspace(path);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening workspace. " + e.ToString());
            }            
        }

        private bool CanDoCommand { get { return true; } }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class NewWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public NewWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string name;

            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                try
                {
                    _viewModel.WorkspaceManager.NewWorkspace(name, "");
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        private bool CanDoCommand 
        { 
            get { return true; } 
        }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class CloseWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public CloseWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            _viewModel.WorkspaceManager.CloseWorkspace();            
        }

        private bool CanDoCommand
        {
            get
            {
                return _viewModel.WorkspaceManager.WorkspaceOpen;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JadeData.Persistence.Workspace
{
    public static class Reader
    {
        private static JadeData.Project.IProject MakeProject(string workspaceDir, ProjectType xml)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                path = System.IO.Path.Combine(workspaceDir, path);
            }
            return Persistence.Project.Reader.Read(path);
        }

        private static JadeData.Workspace.IFolder MakeFolder(string workspaceDir, FolderType xml)
        {
            JadeData.Workspace.IFolder result = new JadeData.Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(workspaceDir, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(workspaceDir, p));
            }
            return result;
        }

        static public JadeData.Workspace.IWorkspace Read(string path)
        {
            WorkspaceType xml;
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceType));
            TextReader tr = new StreamReader(path);
            try
            {                
                xml = (WorkspaceType)serializer.Deserialize(tr);                
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }

            JadeData.Workspace.IWorkspace result = new JadeData.Workspace.Workspace(xml.Name, path);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result.Directory, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(result.Directory, p));
            }

            return result;
        }
    }

    public static class Writer
    {
        static private ProjectType MakeProject(JadeData.Workspace.ProjectItem proj, string workspaceDir)
        {
            ProjectType result = new ProjectType();

            string absPath = proj.Path;

            if (System.IO.Path.IsPathRooted(absPath) == false)
            {
                absPath = System.IO.Path.Combine(workspaceDir, absPath);
            }

            result.Path = proj.Path;
            Persistence.Project.Writer.Write(proj, absPath);
            return result;
        }

        static private FolderType MakeFolder(JadeData.Workspace.IFolder folder, string workspaceDir)
        {
            FolderType result = new FolderType();
            result.Name = folder.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for(int i=0;i<projTemps.Count;i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            return result;
        }

        static private string GetProjectPath(JadeData.Workspace.IWorkspace workspace, JadeData.Project.IProject proj)
        {
            return "";
        }

        static public string Write(JadeData.Workspace.IWorkspace workspace, string path)
        {
            string workspaceDir = System.IO.Path.GetDirectoryName(path);

            JadeData.Workspace.IFolder folder = workspace;
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for (int i = 0; i < projTemps.Count; i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            System.Xml.XmlDocument doc = new XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            System.IO.TextWriter stream = new System.IO.StreamWriter(path); 
            try
            {                    
                serializer.Serialize(stream, result);
                stream.Close();
                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;

namespace JadeGui.ViewModels
{
    using JadeData;

   
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IJadeViewModel
    {
        #region Data

        private JadeCore.IWorkspaceManager _workspaceManager;
        private Commands.NewWorkspace _newWorkspaceCmd;
        private Commands.OpenWorkspace _openWorkspaceCmd;
        private Commands.SaveWorkspace _saveWorkspaceCmd;
        private Commands.SaveAsWorkspace _saveAsWorkspaceCmd;
        private Commands.CloseWorkspace _closeWorkspaceCmd;

        private ICommand _exitCommand;

        #endregion

        public JadeViewModel()
        {
            _workspaceManager = new WorkspaceManager();
            _workspaceManager.WorkspaceOpened += delegate { OnPropertyChanged("Workspace"); };

            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();
            _newWorkspaceCmd = new Commands.NewWorkspace(this);
            _openWorkspaceCmd = new Commands.OpenWorkspace(this);
            _saveWorkspaceCmd = new Commands.SaveWorkspace(this);
            _saveAsWorkspaceCmd = new Commands.SaveAsWorkspace(this);
            _closeWorkspaceCmd = new Commands.CloseWorkspace(this);
        }

        #region EditorControl

        private JadeControls.EditorControl.ViewModel.EditorControlViewModel _editorModel;

        public JadeCore.ViewModels.IEditorViewModel Editor
        {
            get { return _editorModel; }
        }

        #endregion

        #region RequestClose [event]

        /// <summary>
        /// Raised to request ui to close
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

        #region Workspace Tree

        public JadeCore.ViewModels.IWorkspaceViewModel Workspace
        {
            get { return _workspaceManager.ViewModel; }
        }

        #endregion

        #region Commands

        public ICommand CloseWorkspaceCommand { get { return _closeWorkspaceCmd.Command; } }
        public ICommand NewWorkspaceCommand { get { return _newWorkspaceCmd.Command; } }
        public ICommand OpenWorkspaceCommand { get { return _openWorkspaceCmd.Command;}}
        public ICommand SaveWorkspaceCommand { get { return _saveWorkspaceCmd.Command; } }
        public ICommand SaveAsWorkspaceCommand { get { return _saveAsWorkspaceCmd.Command; } }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(delegate { OnRequestClose(); });
                }
                return _exitCommand;
            }
        }

        #endregion

        #region public Methods

        public bool OnExit()
        {
            if (_workspaceManager.RequiresSave)
            {
                return _workspaceManager.SaveOrDiscardWorkspace();
            }
            return true;
        }
       
        #endregion

        public JadeCore.IWorkspaceManager WorkspaceManager
        {
            get
            {
                return _workspaceManager;
            }
        }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class SaveWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path = _viewModel.Workspace.Path;

            if(path == null || path.Length == 0)
            {
                if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    return;                
            }
            _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.RequiresSave; } }
    }

    internal class SaveAsWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveAsWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path;

            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
            }
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.WorkspaceOpen; } }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class OpenWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command 
        { 
            get 
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            } 
        }

        public OpenWorkspace(JadeCore.ViewModels.IJadeViewModel vm)        
        {
            _viewModel = vm;     
        }

        private void OnCommand()
        {
            string path;
            if (JadeCore.GuiUtils.PromptOpenFile(".jws", "Jade Workspace files (.jws)|*.jws", true, out path) == false)
            {
                return;
            }

            try
            {
                _viewModel.WorkspaceManager.OpenWorkspace(path);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening workspace. " + e.ToString());
            }            
        }

        private bool CanDoCommand { get { return true; } }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class NewWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public NewWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string name;

            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                try
                {
                    _viewModel.WorkspaceManager.NewWorkspace(name, "");
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        private bool CanDoCommand 
        { 
            get { return true; } 
        }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class CloseWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public CloseWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            _viewModel.WorkspaceManager.CloseWorkspace();            
        }

        private bool CanDoCommand
        {
            get
            {
                return _viewModel.WorkspaceManager.WorkspaceOpen;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JadeData.Persistence.Workspace
{
    public static class Reader
    {
        private static JadeData.Project.IProject MakeProject(string workspaceDir, ProjectType xml)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                path = System.IO.Path.Combine(workspaceDir, path);
            }
            return Persistence.Project.Reader.Read(path);
        }

        private static JadeData.Workspace.IFolder MakeFolder(string workspaceDir, FolderType xml)
        {
            JadeData.Workspace.IFolder result = new JadeData.Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(workspaceDir, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(workspaceDir, p));
            }
            return result;
        }

        static public JadeData.Workspace.IWorkspace Read(string path)
        {
            WorkspaceType xml;
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceType));
            TextReader tr = new StreamReader(path);
            try
            {                
                xml = (WorkspaceType)serializer.Deserialize(tr);                
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }

            JadeData.Workspace.IWorkspace result = new JadeData.Workspace.Workspace(xml.Name, path);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result.Directory, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(result.Directory, p));
            }

            return result;
        }
    }

    public static class Writer
    {
        static private ProjectType MakeProject(JadeData.Workspace.ProjectItem proj, string workspaceDir)
        {
            ProjectType result = new ProjectType();

            string absPath = proj.Path;

            if (System.IO.Path.IsPathRooted(absPath) == false)
            {
                absPath = System.IO.Path.Combine(workspaceDir, absPath);
            }

            result.Path = proj.Path;
            Persistence.Project.Writer.Write(proj, absPath);
            return result;
        }

        static private FolderType MakeFolder(JadeData.Workspace.IFolder folder, string workspaceDir)
        {
            FolderType result = new FolderType();
            result.Name = folder.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for(int i=0;i<projTemps.Count;i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            return result;
        }

        static private string GetProjectPath(JadeData.Workspace.IWorkspace workspace, JadeData.Project.IProject proj)
        {
            return "";
        }

        static public string Write(JadeData.Workspace.IWorkspace workspace, string path)
        {
            string workspaceDir = System.IO.Path.GetDirectoryName(path);

            JadeData.Workspace.IFolder folder = workspace;
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for (int i = 0; i < projTemps.Count; i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            System.Xml.XmlDocument doc = new XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            System.IO.TextWriter stream = new System.IO.StreamWriter(path); 
            try
            {                    
                serializer.Serialize(stream, result);
                stream.Close();
                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;

namespace JadeGui.ViewModels
{
    using JadeData;

   
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IJadeViewModel
    {
        #region Data

        private JadeCore.IWorkspaceManager _workspaceManager;
        private Commands.NewWorkspace _newWorkspaceCmd;
        private Commands.OpenWorkspace _openWorkspaceCmd;
        private Commands.SaveWorkspace _saveWorkspaceCmd;
        private Commands.SaveAsWorkspace _saveAsWorkspaceCmd;
        private Commands.CloseWorkspace _closeWorkspaceCmd;

        private ICommand _exitCommand;

        #endregion

        public JadeViewModel()
        {
            _workspaceManager = new WorkspaceManager();
            _workspaceManager.WorkspaceOpened += delegate { OnPropertyChanged("Workspace"); };

            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();
            _newWorkspaceCmd = new Commands.NewWorkspace(this);
            _openWorkspaceCmd = new Commands.OpenWorkspace(this);
            _saveWorkspaceCmd = new Commands.SaveWorkspace(this);
            _saveAsWorkspaceCmd = new Commands.SaveAsWorkspace(this);
            _closeWorkspaceCmd = new Commands.CloseWorkspace(this);
        }

        #region EditorControl

        private JadeControls.EditorControl.ViewModel.EditorControlViewModel _editorModel;

        public JadeCore.ViewModels.IEditorViewModel Editor
        {
            get { return _editorModel; }
        }

        #endregion

        #region RequestClose [event]

        /// <summary>
        /// Raised to request ui to close
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

        #region Workspace Tree

        public JadeCore.ViewModels.IWorkspaceViewModel Workspace
        {
            get { return _workspaceManager.ViewModel; }
        }

        #endregion

        #region Commands

        public ICommand CloseWorkspaceCommand { get { return _closeWorkspaceCmd.Command; } }
        public ICommand NewWorkspaceCommand { get { return _newWorkspaceCmd.Command; } }
        public ICommand OpenWorkspaceCommand { get { return _openWorkspaceCmd.Command;}}
        public ICommand SaveWorkspaceCommand { get { return _saveWorkspaceCmd.Command; } }
        public ICommand SaveAsWorkspaceCommand { get { return _saveAsWorkspaceCmd.Command; } }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(delegate { OnRequestClose(); });
                }
                return _exitCommand;
            }
        }

        #endregion

        #region public Methods

        public bool OnExit()
        {
            if (_workspaceManager.RequiresSave)
            {
                return _workspaceManager.SaveOrDiscardWorkspace();
            }
            return true;
        }
       
        #endregion

        public JadeCore.IWorkspaceManager WorkspaceManager
        {
            get
            {
                return _workspaceManager;
            }
        }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class SaveWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path = _viewModel.Workspace.Path;

            if(path == null || path.Length == 0)
            {
                if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    return;                
            }
            _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.RequiresSave; } }
    }

    internal class SaveAsWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveAsWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path;

            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
            }
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.WorkspaceOpen; } }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class OpenWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command 
        { 
            get 
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            } 
        }

        public OpenWorkspace(JadeCore.ViewModels.IJadeViewModel vm)        
        {
            _viewModel = vm;     
        }

        private void OnCommand()
        {
            string path;
            if (JadeCore.GuiUtils.PromptOpenFile(".jws", "Jade Workspace files (.jws)|*.jws", true, out path) == false)
            {
                return;
            }

            try
            {
                _viewModel.WorkspaceManager.OpenWorkspace(path);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening workspace. " + e.ToString());
            }            
        }

        private bool CanDoCommand { get { return true; } }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class NewWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public NewWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string name;

            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                try
                {
                    _viewModel.WorkspaceManager.NewWorkspace(name, "");
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        private bool CanDoCommand 
        { 
            get { return true; } 
        }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class CloseWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public CloseWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            _viewModel.WorkspaceManager.CloseWorkspace();            
        }

        private bool CanDoCommand
        {
            get
            {
                return _viewModel.WorkspaceManager.WorkspaceOpen;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JadeData.Persistence.Workspace
{
    public static class Reader
    {
        private static JadeData.Project.IProject MakeProject(string workspaceDir, ProjectType xml)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                path = System.IO.Path.Combine(workspaceDir, path);
            }
            return Persistence.Project.Reader.Read(path);
        }

        private static JadeData.Workspace.IFolder MakeFolder(string workspaceDir, FolderType xml)
        {
            JadeData.Workspace.IFolder result = new JadeData.Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(workspaceDir, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(workspaceDir, p));
            }
            return result;
        }

        static public JadeData.Workspace.IWorkspace Read(string path)
        {
            WorkspaceType xml;
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceType));
            TextReader tr = new StreamReader(path);
            try
            {                
                xml = (WorkspaceType)serializer.Deserialize(tr);                
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }

            JadeData.Workspace.IWorkspace result = new JadeData.Workspace.Workspace(xml.Name, path);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result.Directory, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(result.Directory, p));
            }

            return result;
        }
    }

    public static class Writer
    {
        static private ProjectType MakeProject(JadeData.Workspace.ProjectItem proj, string workspaceDir)
        {
            ProjectType result = new ProjectType();

            string absPath = proj.Path;

            if (System.IO.Path.IsPathRooted(absPath) == false)
            {
                absPath = System.IO.Path.Combine(workspaceDir, absPath);
            }

            result.Path = proj.Path;
            Persistence.Project.Writer.Write(proj, absPath);
            return result;
        }

        static private FolderType MakeFolder(JadeData.Workspace.IFolder folder, string workspaceDir)
        {
            FolderType result = new FolderType();
            result.Name = folder.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for(int i=0;i<projTemps.Count;i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            return result;
        }

        static private string GetProjectPath(JadeData.Workspace.IWorkspace workspace, JadeData.Project.IProject proj)
        {
            return "";
        }

        static public string Write(JadeData.Workspace.IWorkspace workspace, string path)
        {
            string workspaceDir = System.IO.Path.GetDirectoryName(path);

            JadeData.Workspace.IFolder folder = workspace;
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for (int i = 0; i < projTemps.Count; i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            System.Xml.XmlDocument doc = new XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            System.IO.TextWriter stream = new System.IO.StreamWriter(path); 
            try
            {                    
                serializer.Serialize(stream, result);
                stream.Close();
                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;

namespace JadeGui.ViewModels
{
    using JadeData;

   
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IJadeViewModel
    {
        #region Data

        private JadeCore.IWorkspaceManager _workspaceManager;
        private Commands.NewWorkspace _newWorkspaceCmd;
        private Commands.OpenWorkspace _openWorkspaceCmd;
        private Commands.SaveWorkspace _saveWorkspaceCmd;
        private Commands.SaveAsWorkspace _saveAsWorkspaceCmd;
        private Commands.CloseWorkspace _closeWorkspaceCmd;

        private ICommand _exitCommand;

        #endregion

        public JadeViewModel()
        {
            _workspaceManager = new WorkspaceManager();
            _workspaceManager.WorkspaceOpened += delegate { OnPropertyChanged("Workspace"); };

            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();
            _newWorkspaceCmd = new Commands.NewWorkspace(this);
            _openWorkspaceCmd = new Commands.OpenWorkspace(this);
            _saveWorkspaceCmd = new Commands.SaveWorkspace(this);
            _saveAsWorkspaceCmd = new Commands.SaveAsWorkspace(this);
            _closeWorkspaceCmd = new Commands.CloseWorkspace(this);
        }

        #region EditorControl

        private JadeControls.EditorControl.ViewModel.EditorControlViewModel _editorModel;

        public JadeCore.ViewModels.IEditorViewModel Editor
        {
            get { return _editorModel; }
        }

        #endregion

        #region RequestClose [event]

        /// <summary>
        /// Raised to request ui to close
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

        #region Workspace Tree

        public JadeCore.ViewModels.IWorkspaceViewModel Workspace
        {
            get { return _workspaceManager.ViewModel; }
        }

        #endregion

        #region Commands

        public ICommand CloseWorkspaceCommand { get { return _closeWorkspaceCmd.Command; } }
        public ICommand NewWorkspaceCommand { get { return _newWorkspaceCmd.Command; } }
        public ICommand OpenWorkspaceCommand { get { return _openWorkspaceCmd.Command;}}
        public ICommand SaveWorkspaceCommand { get { return _saveWorkspaceCmd.Command; } }
        public ICommand SaveAsWorkspaceCommand { get { return _saveAsWorkspaceCmd.Command; } }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(delegate { OnRequestClose(); });
                }
                return _exitCommand;
            }
        }

        #endregion

        #region public Methods

        public bool OnExit()
        {
            if (_workspaceManager.RequiresSave)
            {
                return _workspaceManager.SaveOrDiscardWorkspace();
            }
            return true;
        }
       
        #endregion

        public JadeCore.IWorkspaceManager WorkspaceManager
        {
            get
            {
                return _workspaceManager;
            }
        }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class SaveWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path = _viewModel.Workspace.Path;

            if(path == null || path.Length == 0)
            {
                if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    return;                
            }
            _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.RequiresSave; } }
    }

    internal class SaveAsWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveAsWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path;

            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
            }
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.WorkspaceOpen; } }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class OpenWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command 
        { 
            get 
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            } 
        }

        public OpenWorkspace(JadeCore.ViewModels.IJadeViewModel vm)        
        {
            _viewModel = vm;     
        }

        private void OnCommand()
        {
            string path;
            if (JadeCore.GuiUtils.PromptOpenFile(".jws", "Jade Workspace files (.jws)|*.jws", true, out path) == false)
            {
                return;
            }

            try
            {
                _viewModel.WorkspaceManager.OpenWorkspace(path);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening workspace. " + e.ToString());
            }            
        }

        private bool CanDoCommand { get { return true; } }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class NewWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public NewWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string name;

            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                try
                {
                    _viewModel.WorkspaceManager.NewWorkspace(name, "");
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        private bool CanDoCommand 
        { 
            get { return true; } 
        }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class CloseWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public CloseWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            _viewModel.WorkspaceManager.CloseWorkspace();            
        }

        private bool CanDoCommand
        {
            get
            {
                return _viewModel.WorkspaceManager.WorkspaceOpen;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JadeData.Persistence.Workspace
{
    public static class Reader
    {
        private static JadeData.Project.IProject MakeProject(string workspaceDir, ProjectType xml)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                path = System.IO.Path.Combine(workspaceDir, path);
            }
            return Persistence.Project.Reader.Read(path);
        }

        private static JadeData.Workspace.IFolder MakeFolder(string workspaceDir, FolderType xml)
        {
            JadeData.Workspace.IFolder result = new JadeData.Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(workspaceDir, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(workspaceDir, p));
            }
            return result;
        }

        static public JadeData.Workspace.IWorkspace Read(string path)
        {
            WorkspaceType xml;
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceType));
            TextReader tr = new StreamReader(path);
            try
            {                
                xml = (WorkspaceType)serializer.Deserialize(tr);                
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }

            JadeData.Workspace.IWorkspace result = new JadeData.Workspace.Workspace(xml.Name, path);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result.Directory, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(result.Directory, p));
            }

            return result;
        }
    }

    public static class Writer
    {
        static private ProjectType MakeProject(JadeData.Workspace.ProjectItem proj, string workspaceDir)
        {
            ProjectType result = new ProjectType();

            string absPath = proj.Path;

            if (System.IO.Path.IsPathRooted(absPath) == false)
            {
                absPath = System.IO.Path.Combine(workspaceDir, absPath);
            }

            result.Path = proj.Path;
            Persistence.Project.Writer.Write(proj, absPath);
            return result;
        }

        static private FolderType MakeFolder(JadeData.Workspace.IFolder folder, string workspaceDir)
        {
            FolderType result = new FolderType();
            result.Name = folder.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for(int i=0;i<projTemps.Count;i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            return result;
        }

        static private string GetProjectPath(JadeData.Workspace.IWorkspace workspace, JadeData.Project.IProject proj)
        {
            return "";
        }

        static public string Write(JadeData.Workspace.IWorkspace workspace, string path)
        {
            string workspaceDir = System.IO.Path.GetDirectoryName(path);

            JadeData.Workspace.IFolder folder = workspace;
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for (int i = 0; i < projTemps.Count; i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            System.Xml.XmlDocument doc = new XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            System.IO.TextWriter stream = new System.IO.StreamWriter(path); 
            try
            {                    
                serializer.Serialize(stream, result);
                stream.Close();
                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;

namespace JadeGui.ViewModels
{
    using JadeData;

   
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IJadeViewModel
    {
        #region Data

        private JadeCore.IWorkspaceManager _workspaceManager;
        private Commands.NewWorkspace _newWorkspaceCmd;
        private Commands.OpenWorkspace _openWorkspaceCmd;
        private Commands.SaveWorkspace _saveWorkspaceCmd;
        private Commands.SaveAsWorkspace _saveAsWorkspaceCmd;
        private Commands.CloseWorkspace _closeWorkspaceCmd;

        private ICommand _exitCommand;

        #endregion

        public JadeViewModel()
        {
            _workspaceManager = new WorkspaceManager();
            _workspaceManager.WorkspaceOpened += delegate { OnPropertyChanged("Workspace"); };

            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();
            _newWorkspaceCmd = new Commands.NewWorkspace(this);
            _openWorkspaceCmd = new Commands.OpenWorkspace(this);
            _saveWorkspaceCmd = new Commands.SaveWorkspace(this);
            _saveAsWorkspaceCmd = new Commands.SaveAsWorkspace(this);
            _closeWorkspaceCmd = new Commands.CloseWorkspace(this);
        }

        #region EditorControl

        private JadeControls.EditorControl.ViewModel.EditorControlViewModel _editorModel;

        public JadeCore.ViewModels.IEditorViewModel Editor
        {
            get { return _editorModel; }
        }

        #endregion

        #region RequestClose [event]

        /// <summary>
        /// Raised to request ui to close
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

        #region Workspace Tree

        public JadeCore.ViewModels.IWorkspaceViewModel Workspace
        {
            get { return _workspaceManager.ViewModel; }
        }

        #endregion

        #region Commands

        public ICommand CloseWorkspaceCommand { get { return _closeWorkspaceCmd.Command; } }
        public ICommand NewWorkspaceCommand { get { return _newWorkspaceCmd.Command; } }
        public ICommand OpenWorkspaceCommand { get { return _openWorkspaceCmd.Command;}}
        public ICommand SaveWorkspaceCommand { get { return _saveWorkspaceCmd.Command; } }
        public ICommand SaveAsWorkspaceCommand { get { return _saveAsWorkspaceCmd.Command; } }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(delegate { OnRequestClose(); });
                }
                return _exitCommand;
            }
        }

        #endregion

        #region public Methods

        public bool OnExit()
        {
            if (_workspaceManager.RequiresSave)
            {
                return _workspaceManager.SaveOrDiscardWorkspace();
            }
            return true;
        }
       
        #endregion

        public JadeCore.IWorkspaceManager WorkspaceManager
        {
            get
            {
                return _workspaceManager;
            }
        }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class SaveWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path = _viewModel.Workspace.Path;

            if(path == null || path.Length == 0)
            {
                if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    return;                
            }
            _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.RequiresSave; } }
    }

    internal class SaveAsWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveAsWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path;

            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
            }
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.WorkspaceOpen; } }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class OpenWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command 
        { 
            get 
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            } 
        }

        public OpenWorkspace(JadeCore.ViewModels.IJadeViewModel vm)        
        {
            _viewModel = vm;     
        }

        private void OnCommand()
        {
            string path;
            if (JadeCore.GuiUtils.PromptOpenFile(".jws", "Jade Workspace files (.jws)|*.jws", true, out path) == false)
            {
                return;
            }

            try
            {
                _viewModel.WorkspaceManager.OpenWorkspace(path);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening workspace. " + e.ToString());
            }            
        }

        private bool CanDoCommand { get { return true; } }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class NewWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public NewWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string name;

            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                try
                {
                    _viewModel.WorkspaceManager.NewWorkspace(name, "");
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        private bool CanDoCommand 
        { 
            get { return true; } 
        }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class CloseWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public CloseWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            _viewModel.WorkspaceManager.CloseWorkspace();            
        }

        private bool CanDoCommand
        {
            get
            {
                return _viewModel.WorkspaceManager.WorkspaceOpen;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JadeData.Persistence.Workspace
{
    public static class Reader
    {
        private static JadeData.Project.IProject MakeProject(string workspaceDir, ProjectType xml)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                path = System.IO.Path.Combine(workspaceDir, path);
            }
            return Persistence.Project.Reader.Read(path);
        }

        private static JadeData.Workspace.IFolder MakeFolder(string workspaceDir, FolderType xml)
        {
            JadeData.Workspace.IFolder result = new JadeData.Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(workspaceDir, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(workspaceDir, p));
            }
            return result;
        }

        static public JadeData.Workspace.IWorkspace Read(string path)
        {
            WorkspaceType xml;
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceType));
            TextReader tr = new StreamReader(path);
            try
            {                
                xml = (WorkspaceType)serializer.Deserialize(tr);                
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }

            JadeData.Workspace.IWorkspace result = new JadeData.Workspace.Workspace(xml.Name, path);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result.Directory, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(result.Directory, p));
            }

            return result;
        }
    }

    public static class Writer
    {
        static private ProjectType MakeProject(JadeData.Workspace.ProjectItem proj, string workspaceDir)
        {
            ProjectType result = new ProjectType();

            string absPath = proj.Path;

            if (System.IO.Path.IsPathRooted(absPath) == false)
            {
                absPath = System.IO.Path.Combine(workspaceDir, absPath);
            }

            result.Path = proj.Path;
            Persistence.Project.Writer.Write(proj, absPath);
            return result;
        }

        static private FolderType MakeFolder(JadeData.Workspace.IFolder folder, string workspaceDir)
        {
            FolderType result = new FolderType();
            result.Name = folder.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for(int i=0;i<projTemps.Count;i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            return result;
        }

        static private string GetProjectPath(JadeData.Workspace.IWorkspace workspace, JadeData.Project.IProject proj)
        {
            return "";
        }

        static public string Write(JadeData.Workspace.IWorkspace workspace, string path)
        {
            string workspaceDir = System.IO.Path.GetDirectoryName(path);

            JadeData.Workspace.IFolder folder = workspace;
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for (int i = 0; i < projTemps.Count; i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            System.Xml.XmlDocument doc = new XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            System.IO.TextWriter stream = new System.IO.StreamWriter(path); 
            try
            {                    
                serializer.Serialize(stream, result);
                stream.Close();
                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;

namespace JadeGui.ViewModels
{
    using JadeData;

   
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IJadeViewModel
    {
        #region Data

        private JadeCore.IWorkspaceManager _workspaceManager;
        private Commands.NewWorkspace _newWorkspaceCmd;
        private Commands.OpenWorkspace _openWorkspaceCmd;
        private Commands.SaveWorkspace _saveWorkspaceCmd;
        private Commands.SaveAsWorkspace _saveAsWorkspaceCmd;
        private Commands.CloseWorkspace _closeWorkspaceCmd;

        private ICommand _exitCommand;

        #endregion

        public JadeViewModel()
        {
            _workspaceManager = new WorkspaceManager();
            _workspaceManager.WorkspaceOpened += delegate { OnPropertyChanged("Workspace"); };

            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();
            _newWorkspaceCmd = new Commands.NewWorkspace(this);
            _openWorkspaceCmd = new Commands.OpenWorkspace(this);
            _saveWorkspaceCmd = new Commands.SaveWorkspace(this);
            _saveAsWorkspaceCmd = new Commands.SaveAsWorkspace(this);
            _closeWorkspaceCmd = new Commands.CloseWorkspace(this);
        }

        #region EditorControl

        private JadeControls.EditorControl.ViewModel.EditorControlViewModel _editorModel;

        public JadeCore.ViewModels.IEditorViewModel Editor
        {
            get { return _editorModel; }
        }

        #endregion

        #region RequestClose [event]

        /// <summary>
        /// Raised to request ui to close
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

        #region Workspace Tree

        public JadeCore.ViewModels.IWorkspaceViewModel Workspace
        {
            get { return _workspaceManager.ViewModel; }
        }

        #endregion

        #region Commands

        public ICommand CloseWorkspaceCommand { get { return _closeWorkspaceCmd.Command; } }
        public ICommand NewWorkspaceCommand { get { return _newWorkspaceCmd.Command; } }
        public ICommand OpenWorkspaceCommand { get { return _openWorkspaceCmd.Command;}}
        public ICommand SaveWorkspaceCommand { get { return _saveWorkspaceCmd.Command; } }
        public ICommand SaveAsWorkspaceCommand { get { return _saveAsWorkspaceCmd.Command; } }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(delegate { OnRequestClose(); });
                }
                return _exitCommand;
            }
        }

        #endregion

        #region public Methods

        public bool OnExit()
        {
            if (_workspaceManager.RequiresSave)
            {
                return _workspaceManager.SaveOrDiscardWorkspace();
            }
            return true;
        }
       
        #endregion

        public JadeCore.IWorkspaceManager WorkspaceManager
        {
            get
            {
                return _workspaceManager;
            }
        }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class SaveWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path = _viewModel.Workspace.Path;

            if(path == null || path.Length == 0)
            {
                if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    return;                
            }
            _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.RequiresSave; } }
    }

    internal class SaveAsWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveAsWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path;

            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
            }
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.WorkspaceOpen; } }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class OpenWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command 
        { 
            get 
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            } 
        }

        public OpenWorkspace(JadeCore.ViewModels.IJadeViewModel vm)        
        {
            _viewModel = vm;     
        }

        private void OnCommand()
        {
            string path;
            if (JadeCore.GuiUtils.PromptOpenFile(".jws", "Jade Workspace files (.jws)|*.jws", true, out path) == false)
            {
                return;
            }

            try
            {
                _viewModel.WorkspaceManager.OpenWorkspace(path);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening workspace. " + e.ToString());
            }            
        }

        private bool CanDoCommand { get { return true; } }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class NewWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public NewWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string name;

            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                try
                {
                    _viewModel.WorkspaceManager.NewWorkspace(name, "");
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        private bool CanDoCommand 
        { 
            get { return true; } 
        }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class CloseWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public CloseWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            _viewModel.WorkspaceManager.CloseWorkspace();            
        }

        private bool CanDoCommand
        {
            get
            {
                return _viewModel.WorkspaceManager.WorkspaceOpen;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JadeData.Persistence.Workspace
{
    public static class Reader
    {
        private static JadeData.Project.IProject MakeProject(string workspaceDir, ProjectType xml)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                path = System.IO.Path.Combine(workspaceDir, path);
            }
            return Persistence.Project.Reader.Read(path);
        }

        private static JadeData.Workspace.IFolder MakeFolder(string workspaceDir, FolderType xml)
        {
            JadeData.Workspace.IFolder result = new JadeData.Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(workspaceDir, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(workspaceDir, p));
            }
            return result;
        }

        static public JadeData.Workspace.IWorkspace Read(string path)
        {
            WorkspaceType xml;
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceType));
            TextReader tr = new StreamReader(path);
            try
            {                
                xml = (WorkspaceType)serializer.Deserialize(tr);                
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }

            JadeData.Workspace.IWorkspace result = new JadeData.Workspace.Workspace(xml.Name, path);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result.Directory, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(result.Directory, p));
            }

            return result;
        }
    }

    public static class Writer
    {
        static private ProjectType MakeProject(JadeData.Workspace.ProjectItem proj, string workspaceDir)
        {
            ProjectType result = new ProjectType();

            string absPath = proj.Path;

            if (System.IO.Path.IsPathRooted(absPath) == false)
            {
                absPath = System.IO.Path.Combine(workspaceDir, absPath);
            }

            result.Path = proj.Path;
            Persistence.Project.Writer.Write(proj, absPath);
            return result;
        }

        static private FolderType MakeFolder(JadeData.Workspace.IFolder folder, string workspaceDir)
        {
            FolderType result = new FolderType();
            result.Name = folder.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for(int i=0;i<projTemps.Count;i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            return result;
        }

        static private string GetProjectPath(JadeData.Workspace.IWorkspace workspace, JadeData.Project.IProject proj)
        {
            return "";
        }

        static public string Write(JadeData.Workspace.IWorkspace workspace, string path)
        {
            string workspaceDir = System.IO.Path.GetDirectoryName(path);

            JadeData.Workspace.IFolder folder = workspace;
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for (int i = 0; i < projTemps.Count; i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            System.Xml.XmlDocument doc = new XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            System.IO.TextWriter stream = new System.IO.StreamWriter(path); 
            try
            {                    
                serializer.Serialize(stream, result);
                stream.Close();
                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;

namespace JadeGui.ViewModels
{
    using JadeData;

   
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IJadeViewModel
    {
        #region Data

        private JadeCore.IWorkspaceManager _workspaceManager;
        private Commands.NewWorkspace _newWorkspaceCmd;
        private Commands.OpenWorkspace _openWorkspaceCmd;
        private Commands.SaveWorkspace _saveWorkspaceCmd;
        private Commands.SaveAsWorkspace _saveAsWorkspaceCmd;
        private Commands.CloseWorkspace _closeWorkspaceCmd;

        private ICommand _exitCommand;

        #endregion

        public JadeViewModel()
        {
            _workspaceManager = new WorkspaceManager();
            _workspaceManager.WorkspaceOpened += delegate { OnPropertyChanged("Workspace"); };

            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();
            _newWorkspaceCmd = new Commands.NewWorkspace(this);
            _openWorkspaceCmd = new Commands.OpenWorkspace(this);
            _saveWorkspaceCmd = new Commands.SaveWorkspace(this);
            _saveAsWorkspaceCmd = new Commands.SaveAsWorkspace(this);
            _closeWorkspaceCmd = new Commands.CloseWorkspace(this);
        }

        #region EditorControl

        private JadeControls.EditorControl.ViewModel.EditorControlViewModel _editorModel;

        public JadeCore.ViewModels.IEditorViewModel Editor
        {
            get { return _editorModel; }
        }

        #endregion

        #region RequestClose [event]

        /// <summary>
        /// Raised to request ui to close
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

        #region Workspace Tree

        public JadeCore.ViewModels.IWorkspaceViewModel Workspace
        {
            get { return _workspaceManager.ViewModel; }
        }

        #endregion

        #region Commands

        public ICommand CloseWorkspaceCommand { get { return _closeWorkspaceCmd.Command; } }
        public ICommand NewWorkspaceCommand { get { return _newWorkspaceCmd.Command; } }
        public ICommand OpenWorkspaceCommand { get { return _openWorkspaceCmd.Command;}}
        public ICommand SaveWorkspaceCommand { get { return _saveWorkspaceCmd.Command; } }
        public ICommand SaveAsWorkspaceCommand { get { return _saveAsWorkspaceCmd.Command; } }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(delegate { OnRequestClose(); });
                }
                return _exitCommand;
            }
        }

        #endregion

        #region public Methods

        public bool OnExit()
        {
            if (_workspaceManager.RequiresSave)
            {
                return _workspaceManager.SaveOrDiscardWorkspace();
            }
            return true;
        }
       
        #endregion

        public JadeCore.IWorkspaceManager WorkspaceManager
        {
            get
            {
                return _workspaceManager;
            }
        }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class SaveWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path = _viewModel.Workspace.Path;

            if(path == null || path.Length == 0)
            {
                if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    return;                
            }
            _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.RequiresSave; } }
    }

    internal class SaveAsWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveAsWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path;

            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
            }
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.WorkspaceOpen; } }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class OpenWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command 
        { 
            get 
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            } 
        }

        public OpenWorkspace(JadeCore.ViewModels.IJadeViewModel vm)        
        {
            _viewModel = vm;     
        }

        private void OnCommand()
        {
            string path;
            if (JadeCore.GuiUtils.PromptOpenFile(".jws", "Jade Workspace files (.jws)|*.jws", true, out path) == false)
            {
                return;
            }

            try
            {
                _viewModel.WorkspaceManager.OpenWorkspace(path);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening workspace. " + e.ToString());
            }            
        }

        private bool CanDoCommand { get { return true; } }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class NewWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public NewWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string name;

            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                try
                {
                    _viewModel.WorkspaceManager.NewWorkspace(name, "");
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        private bool CanDoCommand 
        { 
            get { return true; } 
        }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class CloseWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public CloseWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            _viewModel.WorkspaceManager.CloseWorkspace();            
        }

        private bool CanDoCommand
        {
            get
            {
                return _viewModel.WorkspaceManager.WorkspaceOpen;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JadeData.Persistence.Workspace
{
    public static class Reader
    {
        private static JadeData.Project.IProject MakeProject(string workspaceDir, ProjectType xml)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                path = System.IO.Path.Combine(workspaceDir, path);
            }
            return Persistence.Project.Reader.Read(path);
        }

        private static JadeData.Workspace.IFolder MakeFolder(string workspaceDir, FolderType xml)
        {
            JadeData.Workspace.IFolder result = new JadeData.Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(workspaceDir, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(workspaceDir, p));
            }
            return result;
        }

        static public JadeData.Workspace.IWorkspace Read(string path)
        {
            WorkspaceType xml;
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceType));
            TextReader tr = new StreamReader(path);
            try
            {                
                xml = (WorkspaceType)serializer.Deserialize(tr);                
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }

            JadeData.Workspace.IWorkspace result = new JadeData.Workspace.Workspace(xml.Name, path);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result.Directory, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(result.Directory, p));
            }

            return result;
        }
    }

    public static class Writer
    {
        static private ProjectType MakeProject(JadeData.Workspace.ProjectItem proj, string workspaceDir)
        {
            ProjectType result = new ProjectType();

            string absPath = proj.Path;

            if (System.IO.Path.IsPathRooted(absPath) == false)
            {
                absPath = System.IO.Path.Combine(workspaceDir, absPath);
            }

            result.Path = proj.Path;
            Persistence.Project.Writer.Write(proj, absPath);
            return result;
        }

        static private FolderType MakeFolder(JadeData.Workspace.IFolder folder, string workspaceDir)
        {
            FolderType result = new FolderType();
            result.Name = folder.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for(int i=0;i<projTemps.Count;i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            return result;
        }

        static private string GetProjectPath(JadeData.Workspace.IWorkspace workspace, JadeData.Project.IProject proj)
        {
            return "";
        }

        static public string Write(JadeData.Workspace.IWorkspace workspace, string path)
        {
            string workspaceDir = System.IO.Path.GetDirectoryName(path);

            JadeData.Workspace.IFolder folder = workspace;
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for (int i = 0; i < projTemps.Count; i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            System.Xml.XmlDocument doc = new XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            System.IO.TextWriter stream = new System.IO.StreamWriter(path); 
            try
            {                    
                serializer.Serialize(stream, result);
                stream.Close();
                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;

namespace JadeGui.ViewModels
{
    using JadeData;

   
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IJadeViewModel
    {
        #region Data

        private JadeCore.IWorkspaceManager _workspaceManager;
        private Commands.NewWorkspace _newWorkspaceCmd;
        private Commands.OpenWorkspace _openWorkspaceCmd;
        private Commands.SaveWorkspace _saveWorkspaceCmd;
        private Commands.SaveAsWorkspace _saveAsWorkspaceCmd;
        private Commands.CloseWorkspace _closeWorkspaceCmd;

        private ICommand _exitCommand;

        #endregion

        public JadeViewModel()
        {
            _workspaceManager = new WorkspaceManager();
            _workspaceManager.WorkspaceOpened += delegate { OnPropertyChanged("Workspace"); };

            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();
            _newWorkspaceCmd = new Commands.NewWorkspace(this);
            _openWorkspaceCmd = new Commands.OpenWorkspace(this);
            _saveWorkspaceCmd = new Commands.SaveWorkspace(this);
            _saveAsWorkspaceCmd = new Commands.SaveAsWorkspace(this);
            _closeWorkspaceCmd = new Commands.CloseWorkspace(this);
        }

        #region EditorControl

        private JadeControls.EditorControl.ViewModel.EditorControlViewModel _editorModel;

        public JadeCore.ViewModels.IEditorViewModel Editor
        {
            get { return _editorModel; }
        }

        #endregion

        #region RequestClose [event]

        /// <summary>
        /// Raised to request ui to close
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

        #region Workspace Tree

        public JadeCore.ViewModels.IWorkspaceViewModel Workspace
        {
            get { return _workspaceManager.ViewModel; }
        }

        #endregion

        #region Commands

        public ICommand CloseWorkspaceCommand { get { return _closeWorkspaceCmd.Command; } }
        public ICommand NewWorkspaceCommand { get { return _newWorkspaceCmd.Command; } }
        public ICommand OpenWorkspaceCommand { get { return _openWorkspaceCmd.Command;}}
        public ICommand SaveWorkspaceCommand { get { return _saveWorkspaceCmd.Command; } }
        public ICommand SaveAsWorkspaceCommand { get { return _saveAsWorkspaceCmd.Command; } }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(delegate { OnRequestClose(); });
                }
                return _exitCommand;
            }
        }

        #endregion

        #region public Methods

        public bool OnExit()
        {
            if (_workspaceManager.RequiresSave)
            {
                return _workspaceManager.SaveOrDiscardWorkspace();
            }
            return true;
        }
       
        #endregion

        public JadeCore.IWorkspaceManager WorkspaceManager
        {
            get
            {
                return _workspaceManager;
            }
        }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class SaveWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path = _viewModel.Workspace.Path;

            if(path == null || path.Length == 0)
            {
                if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    return;                
            }
            _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.RequiresSave; } }
    }

    internal class SaveAsWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveAsWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path;

            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
            }
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.WorkspaceOpen; } }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class OpenWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command 
        { 
            get 
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            } 
        }

        public OpenWorkspace(JadeCore.ViewModels.IJadeViewModel vm)        
        {
            _viewModel = vm;     
        }

        private void OnCommand()
        {
            string path;
            if (JadeCore.GuiUtils.PromptOpenFile(".jws", "Jade Workspace files (.jws)|*.jws", true, out path) == false)
            {
                return;
            }

            try
            {
                _viewModel.WorkspaceManager.OpenWorkspace(path);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening workspace. " + e.ToString());
            }            
        }

        private bool CanDoCommand { get { return true; } }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class NewWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public NewWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string name;

            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                try
                {
                    _viewModel.WorkspaceManager.NewWorkspace(name, "");
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        private bool CanDoCommand 
        { 
            get { return true; } 
        }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class CloseWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public CloseWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            _viewModel.WorkspaceManager.CloseWorkspace();            
        }

        private bool CanDoCommand
        {
            get
            {
                return _viewModel.WorkspaceManager.WorkspaceOpen;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JadeData.Persistence.Workspace
{
    public static class Reader
    {
        private static JadeData.Project.IProject MakeProject(string workspaceDir, ProjectType xml)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                path = System.IO.Path.Combine(workspaceDir, path);
            }
            return Persistence.Project.Reader.Read(path);
        }

        private static JadeData.Workspace.IFolder MakeFolder(string workspaceDir, FolderType xml)
        {
            JadeData.Workspace.IFolder result = new JadeData.Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(workspaceDir, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(workspaceDir, p));
            }
            return result;
        }

        static public JadeData.Workspace.IWorkspace Read(string path)
        {
            WorkspaceType xml;
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceType));
            TextReader tr = new StreamReader(path);
            try
            {                
                xml = (WorkspaceType)serializer.Deserialize(tr);                
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }

            JadeData.Workspace.IWorkspace result = new JadeData.Workspace.Workspace(xml.Name, path);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result.Directory, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(result.Directory, p));
            }

            return result;
        }
    }

    public static class Writer
    {
        static private ProjectType MakeProject(JadeData.Workspace.ProjectItem proj, string workspaceDir)
        {
            ProjectType result = new ProjectType();

            string absPath = proj.Path;

            if (System.IO.Path.IsPathRooted(absPath) == false)
            {
                absPath = System.IO.Path.Combine(workspaceDir, absPath);
            }

            result.Path = proj.Path;
            Persistence.Project.Writer.Write(proj, absPath);
            return result;
        }

        static private FolderType MakeFolder(JadeData.Workspace.IFolder folder, string workspaceDir)
        {
            FolderType result = new FolderType();
            result.Name = folder.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for(int i=0;i<projTemps.Count;i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            return result;
        }

        static private string GetProjectPath(JadeData.Workspace.IWorkspace workspace, JadeData.Project.IProject proj)
        {
            return "";
        }

        static public string Write(JadeData.Workspace.IWorkspace workspace, string path)
        {
            string workspaceDir = System.IO.Path.GetDirectoryName(path);

            JadeData.Workspace.IFolder folder = workspace;
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for (int i = 0; i < projTemps.Count; i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            System.Xml.XmlDocument doc = new XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            System.IO.TextWriter stream = new System.IO.StreamWriter(path); 
            try
            {                    
                serializer.Serialize(stream, result);
                stream.Close();
                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;

namespace JadeGui.ViewModels
{
    using JadeData;

   
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IJadeViewModel
    {
        #region Data

        private JadeCore.IWorkspaceManager _workspaceManager;
        private Commands.NewWorkspace _newWorkspaceCmd;
        private Commands.OpenWorkspace _openWorkspaceCmd;
        private Commands.SaveWorkspace _saveWorkspaceCmd;
        private Commands.SaveAsWorkspace _saveAsWorkspaceCmd;
        private Commands.CloseWorkspace _closeWorkspaceCmd;

        private ICommand _exitCommand;

        #endregion

        public JadeViewModel()
        {
            _workspaceManager = new WorkspaceManager();
            _workspaceManager.WorkspaceOpened += delegate { OnPropertyChanged("Workspace"); };

            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();
            _newWorkspaceCmd = new Commands.NewWorkspace(this);
            _openWorkspaceCmd = new Commands.OpenWorkspace(this);
            _saveWorkspaceCmd = new Commands.SaveWorkspace(this);
            _saveAsWorkspaceCmd = new Commands.SaveAsWorkspace(this);
            _closeWorkspaceCmd = new Commands.CloseWorkspace(this);
        }

        #region EditorControl

        private JadeControls.EditorControl.ViewModel.EditorControlViewModel _editorModel;

        public JadeCore.ViewModels.IEditorViewModel Editor
        {
            get { return _editorModel; }
        }

        #endregion

        #region RequestClose [event]

        /// <summary>
        /// Raised to request ui to close
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

        #region Workspace Tree

        public JadeCore.ViewModels.IWorkspaceViewModel Workspace
        {
            get { return _workspaceManager.ViewModel; }
        }

        #endregion

        #region Commands

        public ICommand CloseWorkspaceCommand { get { return _closeWorkspaceCmd.Command; } }
        public ICommand NewWorkspaceCommand { get { return _newWorkspaceCmd.Command; } }
        public ICommand OpenWorkspaceCommand { get { return _openWorkspaceCmd.Command;}}
        public ICommand SaveWorkspaceCommand { get { return _saveWorkspaceCmd.Command; } }
        public ICommand SaveAsWorkspaceCommand { get { return _saveAsWorkspaceCmd.Command; } }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(delegate { OnRequestClose(); });
                }
                return _exitCommand;
            }
        }

        #endregion

        #region public Methods

        public bool OnExit()
        {
            if (_workspaceManager.RequiresSave)
            {
                return _workspaceManager.SaveOrDiscardWorkspace();
            }
            return true;
        }
       
        #endregion

        public JadeCore.IWorkspaceManager WorkspaceManager
        {
            get
            {
                return _workspaceManager;
            }
        }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class SaveWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path = _viewModel.Workspace.Path;

            if(path == null || path.Length == 0)
            {
                if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    return;                
            }
            _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.RequiresSave; } }
    }

    internal class SaveAsWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveAsWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path;

            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
            }
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.WorkspaceOpen; } }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class OpenWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command 
        { 
            get 
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            } 
        }

        public OpenWorkspace(JadeCore.ViewModels.IJadeViewModel vm)        
        {
            _viewModel = vm;     
        }

        private void OnCommand()
        {
            string path;
            if (JadeCore.GuiUtils.PromptOpenFile(".jws", "Jade Workspace files (.jws)|*.jws", true, out path) == false)
            {
                return;
            }

            try
            {
                _viewModel.WorkspaceManager.OpenWorkspace(path);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening workspace. " + e.ToString());
            }            
        }

        private bool CanDoCommand { get { return true; } }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class NewWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public NewWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string name;

            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                try
                {
                    _viewModel.WorkspaceManager.NewWorkspace(name, "");
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        private bool CanDoCommand 
        { 
            get { return true; } 
        }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class CloseWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public CloseWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            _viewModel.WorkspaceManager.CloseWorkspace();            
        }

        private bool CanDoCommand
        {
            get
            {
                return _viewModel.WorkspaceManager.WorkspaceOpen;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JadeData.Persistence.Workspace
{
    public static class Reader
    {
        private static JadeData.Project.IProject MakeProject(string workspaceDir, ProjectType xml)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                path = System.IO.Path.Combine(workspaceDir, path);
            }
            return Persistence.Project.Reader.Read(path);
        }

        private static JadeData.Workspace.IFolder MakeFolder(string workspaceDir, FolderType xml)
        {
            JadeData.Workspace.IFolder result = new JadeData.Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(workspaceDir, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(workspaceDir, p));
            }
            return result;
        }

        static public JadeData.Workspace.IWorkspace Read(string path)
        {
            WorkspaceType xml;
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceType));
            TextReader tr = new StreamReader(path);
            try
            {                
                xml = (WorkspaceType)serializer.Deserialize(tr);                
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }

            JadeData.Workspace.IWorkspace result = new JadeData.Workspace.Workspace(xml.Name, path);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result.Directory, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(result.Directory, p));
            }

            return result;
        }
    }

    public static class Writer
    {
        static private ProjectType MakeProject(JadeData.Workspace.ProjectItem proj, string workspaceDir)
        {
            ProjectType result = new ProjectType();

            string absPath = proj.Path;

            if (System.IO.Path.IsPathRooted(absPath) == false)
            {
                absPath = System.IO.Path.Combine(workspaceDir, absPath);
            }

            result.Path = proj.Path;
            Persistence.Project.Writer.Write(proj, absPath);
            return result;
        }

        static private FolderType MakeFolder(JadeData.Workspace.IFolder folder, string workspaceDir)
        {
            FolderType result = new FolderType();
            result.Name = folder.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for(int i=0;i<projTemps.Count;i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            return result;
        }

        static private string GetProjectPath(JadeData.Workspace.IWorkspace workspace, JadeData.Project.IProject proj)
        {
            return "";
        }

        static public string Write(JadeData.Workspace.IWorkspace workspace, string path)
        {
            string workspaceDir = System.IO.Path.GetDirectoryName(path);

            JadeData.Workspace.IFolder folder = workspace;
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for (int i = 0; i < projTemps.Count; i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            System.Xml.XmlDocument doc = new XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            System.IO.TextWriter stream = new System.IO.StreamWriter(path); 
            try
            {                    
                serializer.Serialize(stream, result);
                stream.Close();
                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;

namespace JadeGui.ViewModels
{
    using JadeData;

   
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IJadeViewModel
    {
        #region Data

        private JadeCore.IWorkspaceManager _workspaceManager;
        private Commands.NewWorkspace _newWorkspaceCmd;
        private Commands.OpenWorkspace _openWorkspaceCmd;
        private Commands.SaveWorkspace _saveWorkspaceCmd;
        private Commands.SaveAsWorkspace _saveAsWorkspaceCmd;
        private Commands.CloseWorkspace _closeWorkspaceCmd;

        private ICommand _exitCommand;

        #endregion

        public JadeViewModel()
        {
            _workspaceManager = new WorkspaceManager();
            _workspaceManager.WorkspaceOpened += delegate { OnPropertyChanged("Workspace"); };

            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();
            _newWorkspaceCmd = new Commands.NewWorkspace(this);
            _openWorkspaceCmd = new Commands.OpenWorkspace(this);
            _saveWorkspaceCmd = new Commands.SaveWorkspace(this);
            _saveAsWorkspaceCmd = new Commands.SaveAsWorkspace(this);
            _closeWorkspaceCmd = new Commands.CloseWorkspace(this);
        }

        #region EditorControl

        private JadeControls.EditorControl.ViewModel.EditorControlViewModel _editorModel;

        public JadeCore.ViewModels.IEditorViewModel Editor
        {
            get { return _editorModel; }
        }

        #endregion

        #region RequestClose [event]

        /// <summary>
        /// Raised to request ui to close
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

        #region Workspace Tree

        public JadeCore.ViewModels.IWorkspaceViewModel Workspace
        {
            get { return _workspaceManager.ViewModel; }
        }

        #endregion

        #region Commands

        public ICommand CloseWorkspaceCommand { get { return _closeWorkspaceCmd.Command; } }
        public ICommand NewWorkspaceCommand { get { return _newWorkspaceCmd.Command; } }
        public ICommand OpenWorkspaceCommand { get { return _openWorkspaceCmd.Command;}}
        public ICommand SaveWorkspaceCommand { get { return _saveWorkspaceCmd.Command; } }
        public ICommand SaveAsWorkspaceCommand { get { return _saveAsWorkspaceCmd.Command; } }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(delegate { OnRequestClose(); });
                }
                return _exitCommand;
            }
        }

        #endregion

        #region public Methods

        public bool OnExit()
        {
            if (_workspaceManager.RequiresSave)
            {
                return _workspaceManager.SaveOrDiscardWorkspace();
            }
            return true;
        }
       
        #endregion

        public JadeCore.IWorkspaceManager WorkspaceManager
        {
            get
            {
                return _workspaceManager;
            }
        }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class SaveWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path = _viewModel.Workspace.Path;

            if(path == null || path.Length == 0)
            {
                if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    return;                
            }
            _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.RequiresSave; } }
    }

    internal class SaveAsWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveAsWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path;

            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
            }
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.WorkspaceOpen; } }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class OpenWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command 
        { 
            get 
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            } 
        }

        public OpenWorkspace(JadeCore.ViewModels.IJadeViewModel vm)        
        {
            _viewModel = vm;     
        }

        private void OnCommand()
        {
            string path;
            if (JadeCore.GuiUtils.PromptOpenFile(".jws", "Jade Workspace files (.jws)|*.jws", true, out path) == false)
            {
                return;
            }

            try
            {
                _viewModel.WorkspaceManager.OpenWorkspace(path);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening workspace. " + e.ToString());
            }            
        }

        private bool CanDoCommand { get { return true; } }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class NewWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public NewWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string name;

            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                try
                {
                    _viewModel.WorkspaceManager.NewWorkspace(name, "");
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        private bool CanDoCommand 
        { 
            get { return true; } 
        }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class CloseWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public CloseWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            _viewModel.WorkspaceManager.CloseWorkspace();            
        }

        private bool CanDoCommand
        {
            get
            {
                return _viewModel.WorkspaceManager.WorkspaceOpen;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JadeData.Persistence.Workspace
{
    public static class Reader
    {
        private static JadeData.Project.IProject MakeProject(string workspaceDir, ProjectType xml)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                path = System.IO.Path.Combine(workspaceDir, path);
            }
            return Persistence.Project.Reader.Read(path);
        }

        private static JadeData.Workspace.IFolder MakeFolder(string workspaceDir, FolderType xml)
        {
            JadeData.Workspace.IFolder result = new JadeData.Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(workspaceDir, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(workspaceDir, p));
            }
            return result;
        }

        static public JadeData.Workspace.IWorkspace Read(string path)
        {
            WorkspaceType xml;
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceType));
            TextReader tr = new StreamReader(path);
            try
            {                
                xml = (WorkspaceType)serializer.Deserialize(tr);                
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }

            JadeData.Workspace.IWorkspace result = new JadeData.Workspace.Workspace(xml.Name, path);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result.Directory, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(result.Directory, p));
            }

            return result;
        }
    }

    public static class Writer
    {
        static private ProjectType MakeProject(JadeData.Workspace.ProjectItem proj, string workspaceDir)
        {
            ProjectType result = new ProjectType();

            string absPath = proj.Path;

            if (System.IO.Path.IsPathRooted(absPath) == false)
            {
                absPath = System.IO.Path.Combine(workspaceDir, absPath);
            }

            result.Path = proj.Path;
            Persistence.Project.Writer.Write(proj, absPath);
            return result;
        }

        static private FolderType MakeFolder(JadeData.Workspace.IFolder folder, string workspaceDir)
        {
            FolderType result = new FolderType();
            result.Name = folder.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for(int i=0;i<projTemps.Count;i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            return result;
        }

        static private string GetProjectPath(JadeData.Workspace.IWorkspace workspace, JadeData.Project.IProject proj)
        {
            return "";
        }

        static public string Write(JadeData.Workspace.IWorkspace workspace, string path)
        {
            string workspaceDir = System.IO.Path.GetDirectoryName(path);

            JadeData.Workspace.IFolder folder = workspace;
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for (int i = 0; i < projTemps.Count; i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            System.Xml.XmlDocument doc = new XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            System.IO.TextWriter stream = new System.IO.StreamWriter(path); 
            try
            {                    
                serializer.Serialize(stream, result);
                stream.Close();
                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;

namespace JadeGui.ViewModels
{
    using JadeData;

   
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IJadeViewModel
    {
        #region Data

        private JadeCore.IWorkspaceManager _workspaceManager;
        private Commands.NewWorkspace _newWorkspaceCmd;
        private Commands.OpenWorkspace _openWorkspaceCmd;
        private Commands.SaveWorkspace _saveWorkspaceCmd;
        private Commands.SaveAsWorkspace _saveAsWorkspaceCmd;
        private Commands.CloseWorkspace _closeWorkspaceCmd;

        private ICommand _exitCommand;

        #endregion

        public JadeViewModel()
        {
            _workspaceManager = new WorkspaceManager();
            _workspaceManager.WorkspaceOpened += delegate { OnPropertyChanged("Workspace"); };

            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();
            _newWorkspaceCmd = new Commands.NewWorkspace(this);
            _openWorkspaceCmd = new Commands.OpenWorkspace(this);
            _saveWorkspaceCmd = new Commands.SaveWorkspace(this);
            _saveAsWorkspaceCmd = new Commands.SaveAsWorkspace(this);
            _closeWorkspaceCmd = new Commands.CloseWorkspace(this);
        }

        #region EditorControl

        private JadeControls.EditorControl.ViewModel.EditorControlViewModel _editorModel;

        public JadeCore.ViewModels.IEditorViewModel Editor
        {
            get { return _editorModel; }
        }

        #endregion

        #region RequestClose [event]

        /// <summary>
        /// Raised to request ui to close
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

        #region Workspace Tree

        public JadeCore.ViewModels.IWorkspaceViewModel Workspace
        {
            get { return _workspaceManager.ViewModel; }
        }

        #endregion

        #region Commands

        public ICommand CloseWorkspaceCommand { get { return _closeWorkspaceCmd.Command; } }
        public ICommand NewWorkspaceCommand { get { return _newWorkspaceCmd.Command; } }
        public ICommand OpenWorkspaceCommand { get { return _openWorkspaceCmd.Command;}}
        public ICommand SaveWorkspaceCommand { get { return _saveWorkspaceCmd.Command; } }
        public ICommand SaveAsWorkspaceCommand { get { return _saveAsWorkspaceCmd.Command; } }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(delegate { OnRequestClose(); });
                }
                return _exitCommand;
            }
        }

        #endregion

        #region public Methods

        public bool OnExit()
        {
            if (_workspaceManager.RequiresSave)
            {
                return _workspaceManager.SaveOrDiscardWorkspace();
            }
            return true;
        }
       
        #endregion

        public JadeCore.IWorkspaceManager WorkspaceManager
        {
            get
            {
                return _workspaceManager;
            }
        }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class SaveWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path = _viewModel.Workspace.Path;

            if(path == null || path.Length == 0)
            {
                if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    return;                
            }
            _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.RequiresSave; } }
    }

    internal class SaveAsWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveAsWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path;

            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
            }
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.WorkspaceOpen; } }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class OpenWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command 
        { 
            get 
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            } 
        }

        public OpenWorkspace(JadeCore.ViewModels.IJadeViewModel vm)        
        {
            _viewModel = vm;     
        }

        private void OnCommand()
        {
            string path;
            if (JadeCore.GuiUtils.PromptOpenFile(".jws", "Jade Workspace files (.jws)|*.jws", true, out path) == false)
            {
                return;
            }

            try
            {
                _viewModel.WorkspaceManager.OpenWorkspace(path);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening workspace. " + e.ToString());
            }            
        }

        private bool CanDoCommand { get { return true; } }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class NewWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public NewWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string name;

            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                try
                {
                    _viewModel.WorkspaceManager.NewWorkspace(name, "");
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        private bool CanDoCommand 
        { 
            get { return true; } 
        }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class CloseWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public CloseWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            _viewModel.WorkspaceManager.CloseWorkspace();            
        }

        private bool CanDoCommand
        {
            get
            {
                return _viewModel.WorkspaceManager.WorkspaceOpen;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JadeData.Persistence.Workspace
{
    public static class Reader
    {
        private static JadeData.Project.IProject MakeProject(string workspaceDir, ProjectType xml)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                path = System.IO.Path.Combine(workspaceDir, path);
            }
            return Persistence.Project.Reader.Read(path);
        }

        private static JadeData.Workspace.IFolder MakeFolder(string workspaceDir, FolderType xml)
        {
            JadeData.Workspace.IFolder result = new JadeData.Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(workspaceDir, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(workspaceDir, p));
            }
            return result;
        }

        static public JadeData.Workspace.IWorkspace Read(string path)
        {
            WorkspaceType xml;
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceType));
            TextReader tr = new StreamReader(path);
            try
            {                
                xml = (WorkspaceType)serializer.Deserialize(tr);                
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }

            JadeData.Workspace.IWorkspace result = new JadeData.Workspace.Workspace(xml.Name, path);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result.Directory, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(result.Directory, p));
            }

            return result;
        }
    }

    public static class Writer
    {
        static private ProjectType MakeProject(JadeData.Workspace.ProjectItem proj, string workspaceDir)
        {
            ProjectType result = new ProjectType();

            string absPath = proj.Path;

            if (System.IO.Path.IsPathRooted(absPath) == false)
            {
                absPath = System.IO.Path.Combine(workspaceDir, absPath);
            }

            result.Path = proj.Path;
            Persistence.Project.Writer.Write(proj, absPath);
            return result;
        }

        static private FolderType MakeFolder(JadeData.Workspace.IFolder folder, string workspaceDir)
        {
            FolderType result = new FolderType();
            result.Name = folder.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for(int i=0;i<projTemps.Count;i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            return result;
        }

        static private string GetProjectPath(JadeData.Workspace.IWorkspace workspace, JadeData.Project.IProject proj)
        {
            return "";
        }

        static public string Write(JadeData.Workspace.IWorkspace workspace, string path)
        {
            string workspaceDir = System.IO.Path.GetDirectoryName(path);

            JadeData.Workspace.IFolder folder = workspace;
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for (int i = 0; i < projTemps.Count; i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            System.Xml.XmlDocument doc = new XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            System.IO.TextWriter stream = new System.IO.StreamWriter(path); 
            try
            {                    
                serializer.Serialize(stream, result);
                stream.Close();
                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;

namespace JadeGui.ViewModels
{
    using JadeData;

   
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IJadeViewModel
    {
        #region Data

        private JadeCore.IWorkspaceManager _workspaceManager;
        private Commands.NewWorkspace _newWorkspaceCmd;
        private Commands.OpenWorkspace _openWorkspaceCmd;
        private Commands.SaveWorkspace _saveWorkspaceCmd;
        private Commands.SaveAsWorkspace _saveAsWorkspaceCmd;
        private Commands.CloseWorkspace _closeWorkspaceCmd;

        private ICommand _exitCommand;

        #endregion

        public JadeViewModel()
        {
            _workspaceManager = new WorkspaceManager();
            _workspaceManager.WorkspaceOpened += delegate { OnPropertyChanged("Workspace"); };

            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();
            _newWorkspaceCmd = new Commands.NewWorkspace(this);
            _openWorkspaceCmd = new Commands.OpenWorkspace(this);
            _saveWorkspaceCmd = new Commands.SaveWorkspace(this);
            _saveAsWorkspaceCmd = new Commands.SaveAsWorkspace(this);
            _closeWorkspaceCmd = new Commands.CloseWorkspace(this);
        }

        #region EditorControl

        private JadeControls.EditorControl.ViewModel.EditorControlViewModel _editorModel;

        public JadeCore.ViewModels.IEditorViewModel Editor
        {
            get { return _editorModel; }
        }

        #endregion

        #region RequestClose [event]

        /// <summary>
        /// Raised to request ui to close
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

        #region Workspace Tree

        public JadeCore.ViewModels.IWorkspaceViewModel Workspace
        {
            get { return _workspaceManager.ViewModel; }
        }

        #endregion

        #region Commands

        public ICommand CloseWorkspaceCommand { get { return _closeWorkspaceCmd.Command; } }
        public ICommand NewWorkspaceCommand { get { return _newWorkspaceCmd.Command; } }
        public ICommand OpenWorkspaceCommand { get { return _openWorkspaceCmd.Command;}}
        public ICommand SaveWorkspaceCommand { get { return _saveWorkspaceCmd.Command; } }
        public ICommand SaveAsWorkspaceCommand { get { return _saveAsWorkspaceCmd.Command; } }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(delegate { OnRequestClose(); });
                }
                return _exitCommand;
            }
        }

        #endregion

        #region public Methods

        public bool OnExit()
        {
            if (_workspaceManager.RequiresSave)
            {
                return _workspaceManager.SaveOrDiscardWorkspace();
            }
            return true;
        }
       
        #endregion

        public JadeCore.IWorkspaceManager WorkspaceManager
        {
            get
            {
                return _workspaceManager;
            }
        }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class SaveWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path = _viewModel.Workspace.Path;

            if(path == null || path.Length == 0)
            {
                if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    return;                
            }
            _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.RequiresSave; } }
    }

    internal class SaveAsWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveAsWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path;

            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
            }
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.WorkspaceOpen; } }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class OpenWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command 
        { 
            get 
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            } 
        }

        public OpenWorkspace(JadeCore.ViewModels.IJadeViewModel vm)        
        {
            _viewModel = vm;     
        }

        private void OnCommand()
        {
            string path;
            if (JadeCore.GuiUtils.PromptOpenFile(".jws", "Jade Workspace files (.jws)|*.jws", true, out path) == false)
            {
                return;
            }

            try
            {
                _viewModel.WorkspaceManager.OpenWorkspace(path);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening workspace. " + e.ToString());
            }            
        }

        private bool CanDoCommand { get { return true; } }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class NewWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public NewWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string name;

            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                try
                {
                    _viewModel.WorkspaceManager.NewWorkspace(name, "");
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        private bool CanDoCommand 
        { 
            get { return true; } 
        }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class CloseWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public CloseWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            _viewModel.WorkspaceManager.CloseWorkspace();            
        }

        private bool CanDoCommand
        {
            get
            {
                return _viewModel.WorkspaceManager.WorkspaceOpen;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JadeData.Persistence.Workspace
{
    public static class Reader
    {
        private static JadeData.Project.IProject MakeProject(string workspaceDir, ProjectType xml)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                path = System.IO.Path.Combine(workspaceDir, path);
            }
            return Persistence.Project.Reader.Read(path);
        }

        private static JadeData.Workspace.IFolder MakeFolder(string workspaceDir, FolderType xml)
        {
            JadeData.Workspace.IFolder result = new JadeData.Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(workspaceDir, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(workspaceDir, p));
            }
            return result;
        }

        static public JadeData.Workspace.IWorkspace Read(string path)
        {
            WorkspaceType xml;
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceType));
            TextReader tr = new StreamReader(path);
            try
            {                
                xml = (WorkspaceType)serializer.Deserialize(tr);                
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }

            JadeData.Workspace.IWorkspace result = new JadeData.Workspace.Workspace(xml.Name, path);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result.Directory, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(result.Directory, p));
            }

            return result;
        }
    }

    public static class Writer
    {
        static private ProjectType MakeProject(JadeData.Workspace.ProjectItem proj, string workspaceDir)
        {
            ProjectType result = new ProjectType();

            string absPath = proj.Path;

            if (System.IO.Path.IsPathRooted(absPath) == false)
            {
                absPath = System.IO.Path.Combine(workspaceDir, absPath);
            }

            result.Path = proj.Path;
            Persistence.Project.Writer.Write(proj, absPath);
            return result;
        }

        static private FolderType MakeFolder(JadeData.Workspace.IFolder folder, string workspaceDir)
        {
            FolderType result = new FolderType();
            result.Name = folder.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for(int i=0;i<projTemps.Count;i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            return result;
        }

        static private string GetProjectPath(JadeData.Workspace.IWorkspace workspace, JadeData.Project.IProject proj)
        {
            return "";
        }

        static public string Write(JadeData.Workspace.IWorkspace workspace, string path)
        {
            string workspaceDir = System.IO.Path.GetDirectoryName(path);

            JadeData.Workspace.IFolder folder = workspace;
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for (int i = 0; i < projTemps.Count; i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            System.Xml.XmlDocument doc = new XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            System.IO.TextWriter stream = new System.IO.StreamWriter(path); 
            try
            {                    
                serializer.Serialize(stream, result);
                stream.Close();
                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;

namespace JadeGui.ViewModels
{
    using JadeData;

   
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IJadeViewModel
    {
        #region Data

        private JadeCore.IWorkspaceManager _workspaceManager;
        private Commands.NewWorkspace _newWorkspaceCmd;
        private Commands.OpenWorkspace _openWorkspaceCmd;
        private Commands.SaveWorkspace _saveWorkspaceCmd;
        private Commands.SaveAsWorkspace _saveAsWorkspaceCmd;
        private Commands.CloseWorkspace _closeWorkspaceCmd;

        private ICommand _exitCommand;

        #endregion

        public JadeViewModel()
        {
            _workspaceManager = new WorkspaceManager();
            _workspaceManager.WorkspaceOpened += delegate { OnPropertyChanged("Workspace"); };

            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();
            _newWorkspaceCmd = new Commands.NewWorkspace(this);
            _openWorkspaceCmd = new Commands.OpenWorkspace(this);
            _saveWorkspaceCmd = new Commands.SaveWorkspace(this);
            _saveAsWorkspaceCmd = new Commands.SaveAsWorkspace(this);
            _closeWorkspaceCmd = new Commands.CloseWorkspace(this);
        }

        #region EditorControl

        private JadeControls.EditorControl.ViewModel.EditorControlViewModel _editorModel;

        public JadeCore.ViewModels.IEditorViewModel Editor
        {
            get { return _editorModel; }
        }

        #endregion

        #region RequestClose [event]

        /// <summary>
        /// Raised to request ui to close
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

        #region Workspace Tree

        public JadeCore.ViewModels.IWorkspaceViewModel Workspace
        {
            get { return _workspaceManager.ViewModel; }
        }

        #endregion

        #region Commands

        public ICommand CloseWorkspaceCommand { get { return _closeWorkspaceCmd.Command; } }
        public ICommand NewWorkspaceCommand { get { return _newWorkspaceCmd.Command; } }
        public ICommand OpenWorkspaceCommand { get { return _openWorkspaceCmd.Command;}}
        public ICommand SaveWorkspaceCommand { get { return _saveWorkspaceCmd.Command; } }
        public ICommand SaveAsWorkspaceCommand { get { return _saveAsWorkspaceCmd.Command; } }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(delegate { OnRequestClose(); });
                }
                return _exitCommand;
            }
        }

        #endregion

        #region public Methods

        public bool OnExit()
        {
            if (_workspaceManager.RequiresSave)
            {
                return _workspaceManager.SaveOrDiscardWorkspace();
            }
            return true;
        }
       
        #endregion

        public JadeCore.IWorkspaceManager WorkspaceManager
        {
            get
            {
                return _workspaceManager;
            }
        }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class SaveWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path = _viewModel.Workspace.Path;

            if(path == null || path.Length == 0)
            {
                if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    return;                
            }
            _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.RequiresSave; } }
    }

    internal class SaveAsWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveAsWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path;

            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
            }
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.WorkspaceOpen; } }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class OpenWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command 
        { 
            get 
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            } 
        }

        public OpenWorkspace(JadeCore.ViewModels.IJadeViewModel vm)        
        {
            _viewModel = vm;     
        }

        private void OnCommand()
        {
            string path;
            if (JadeCore.GuiUtils.PromptOpenFile(".jws", "Jade Workspace files (.jws)|*.jws", true, out path) == false)
            {
                return;
            }

            try
            {
                _viewModel.WorkspaceManager.OpenWorkspace(path);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening workspace. " + e.ToString());
            }            
        }

        private bool CanDoCommand { get { return true; } }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class NewWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public NewWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string name;

            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                try
                {
                    _viewModel.WorkspaceManager.NewWorkspace(name, "");
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        private bool CanDoCommand 
        { 
            get { return true; } 
        }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class CloseWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public CloseWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            _viewModel.WorkspaceManager.CloseWorkspace();            
        }

        private bool CanDoCommand
        {
            get
            {
                return _viewModel.WorkspaceManager.WorkspaceOpen;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JadeData.Persistence.Workspace
{
    public static class Reader
    {
        private static JadeData.Project.IProject MakeProject(string workspaceDir, ProjectType xml)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                path = System.IO.Path.Combine(workspaceDir, path);
            }
            return Persistence.Project.Reader.Read(path);
        }

        private static JadeData.Workspace.IFolder MakeFolder(string workspaceDir, FolderType xml)
        {
            JadeData.Workspace.IFolder result = new JadeData.Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(workspaceDir, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(workspaceDir, p));
            }
            return result;
        }

        static public JadeData.Workspace.IWorkspace Read(string path)
        {
            WorkspaceType xml;
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceType));
            TextReader tr = new StreamReader(path);
            try
            {                
                xml = (WorkspaceType)serializer.Deserialize(tr);                
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }

            JadeData.Workspace.IWorkspace result = new JadeData.Workspace.Workspace(xml.Name, path);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result.Directory, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(result.Directory, p));
            }

            return result;
        }
    }

    public static class Writer
    {
        static private ProjectType MakeProject(JadeData.Workspace.ProjectItem proj, string workspaceDir)
        {
            ProjectType result = new ProjectType();

            string absPath = proj.Path;

            if (System.IO.Path.IsPathRooted(absPath) == false)
            {
                absPath = System.IO.Path.Combine(workspaceDir, absPath);
            }

            result.Path = proj.Path;
            Persistence.Project.Writer.Write(proj, absPath);
            return result;
        }

        static private FolderType MakeFolder(JadeData.Workspace.IFolder folder, string workspaceDir)
        {
            FolderType result = new FolderType();
            result.Name = folder.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for(int i=0;i<projTemps.Count;i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            return result;
        }

        static private string GetProjectPath(JadeData.Workspace.IWorkspace workspace, JadeData.Project.IProject proj)
        {
            return "";
        }

        static public string Write(JadeData.Workspace.IWorkspace workspace, string path)
        {
            string workspaceDir = System.IO.Path.GetDirectoryName(path);

            JadeData.Workspace.IFolder folder = workspace;
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for (int i = 0; i < projTemps.Count; i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            System.Xml.XmlDocument doc = new XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            System.IO.TextWriter stream = new System.IO.StreamWriter(path); 
            try
            {                    
                serializer.Serialize(stream, result);
                stream.Close();
                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;

namespace JadeGui.ViewModels
{
    using JadeData;

   
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IJadeViewModel
    {
        #region Data

        private JadeCore.IWorkspaceManager _workspaceManager;
        private Commands.NewWorkspace _newWorkspaceCmd;
        private Commands.OpenWorkspace _openWorkspaceCmd;
        private Commands.SaveWorkspace _saveWorkspaceCmd;
        private Commands.SaveAsWorkspace _saveAsWorkspaceCmd;
        private Commands.CloseWorkspace _closeWorkspaceCmd;

        private ICommand _exitCommand;

        #endregion

        public JadeViewModel()
        {
            _workspaceManager = new WorkspaceManager();
            _workspaceManager.WorkspaceOpened += delegate { OnPropertyChanged("Workspace"); };

            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();
            _newWorkspaceCmd = new Commands.NewWorkspace(this);
            _openWorkspaceCmd = new Commands.OpenWorkspace(this);
            _saveWorkspaceCmd = new Commands.SaveWorkspace(this);
            _saveAsWorkspaceCmd = new Commands.SaveAsWorkspace(this);
            _closeWorkspaceCmd = new Commands.CloseWorkspace(this);
        }

        #region EditorControl

        private JadeControls.EditorControl.ViewModel.EditorControlViewModel _editorModel;

        public JadeCore.ViewModels.IEditorViewModel Editor
        {
            get { return _editorModel; }
        }

        #endregion

        #region RequestClose [event]

        /// <summary>
        /// Raised to request ui to close
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

        #region Workspace Tree

        public JadeCore.ViewModels.IWorkspaceViewModel Workspace
        {
            get { return _workspaceManager.ViewModel; }
        }

        #endregion

        #region Commands

        public ICommand CloseWorkspaceCommand { get { return _closeWorkspaceCmd.Command; } }
        public ICommand NewWorkspaceCommand { get { return _newWorkspaceCmd.Command; } }
        public ICommand OpenWorkspaceCommand { get { return _openWorkspaceCmd.Command;}}
        public ICommand SaveWorkspaceCommand { get { return _saveWorkspaceCmd.Command; } }
        public ICommand SaveAsWorkspaceCommand { get { return _saveAsWorkspaceCmd.Command; } }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(delegate { OnRequestClose(); });
                }
                return _exitCommand;
            }
        }

        #endregion

        #region public Methods

        public bool OnExit()
        {
            if (_workspaceManager.RequiresSave)
            {
                return _workspaceManager.SaveOrDiscardWorkspace();
            }
            return true;
        }
       
        #endregion

        public JadeCore.IWorkspaceManager WorkspaceManager
        {
            get
            {
                return _workspaceManager;
            }
        }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class SaveWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path = _viewModel.Workspace.Path;

            if(path == null || path.Length == 0)
            {
                if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    return;                
            }
            _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.RequiresSave; } }
    }

    internal class SaveAsWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public SaveAsWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path;

            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
            }
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.WorkspaceOpen; } }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class OpenWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command 
        { 
            get 
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            } 
        }

        public OpenWorkspace(JadeCore.ViewModels.IJadeViewModel vm)        
        {
            _viewModel = vm;     
        }

        private void OnCommand()
        {
            string path;
            if (JadeCore.GuiUtils.PromptOpenFile(".jws", "Jade Workspace files (.jws)|*.jws", true, out path) == false)
            {
                return;
            }

            try
            {
                _viewModel.WorkspaceManager.OpenWorkspace(path);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening workspace. " + e.ToString());
            }            
        }

        private bool CanDoCommand { get { return true; } }
    }
}
using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class NewWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public NewWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string name;

            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                try
                {
                    _viewModel.WorkspaceManager.NewWorkspace(name, "");
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        private bool CanDoCommand 
        { 
            get { return true; } 
        }
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class CloseWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public CloseWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            _viewModel.WorkspaceManager.CloseWorkspace();            
        }

        private bool CanDoCommand
        {
            get
            {
                return _viewModel.WorkspaceManager.WorkspaceOpen;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JadeData.Persistence.Workspace
{
    public static class Reader
    {
        private static JadeData.Project.IProject MakeProject(string workspaceDir, ProjectType xml)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                path = System.IO.Path.Combine(workspaceDir, path);
            }
            return Persistence.Project.Reader.Read(path);
        }

        private static JadeData.Workspace.IFolder MakeFolder(string workspaceDir, FolderType xml)
        {
            JadeData.Workspace.IFolder result = new JadeData.Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(workspaceDir, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(workspaceDir, p));
            }
            return result;
        }

        static public JadeData.Workspace.IWorkspace Read(string path)
        {
            WorkspaceType xml;
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceType));
            TextReader tr = new StreamReader(path);
            try
            {                
                xml = (WorkspaceType)serializer.Deserialize(tr);                
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }

            JadeData.Workspace.IWorkspace result = new JadeData.Workspace.Workspace(xml.Name, path);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result.Directory, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(result.Directory, p));
            }

            return result;
        }
    }

    public static class Writer
    {
        static private ProjectType MakeProject(JadeData.Workspace.ProjectItem proj, string workspaceDir)
        {
            ProjectType result = new ProjectType();

            string absPath = proj.Path;

            if (System.IO.Path.IsPathRooted(absPath) == false)
            {
                absPath = System.IO.Path.Combine(workspaceDir, absPath);
            }

            result.Path = proj.Path;
            Persistence.Project.Writer.Write(proj, absPath);
            return result;
        }

        static private FolderType MakeFolder(JadeData.Workspace.IFolder folder, string workspaceDir)
        {
            FolderType result = new FolderType();
            result.Name = folder.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for(int i=0;i<projTemps.Count;i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            return result;
        }

        static private string GetProjectPath(JadeData.Workspace.IWorkspace workspace, JadeData.Project.IProject proj)
        {
            return "";
        }

        static public string Write(JadeData.Workspace.IWorkspace workspace, string path)
        {
            string workspaceDir = System.IO.Path.GetDirectoryName(path);

            JadeData.Workspace.IFolder folder = workspace;
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for (int i = 0; i < projTemps.Count; i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            System.Xml.XmlDocument doc = new XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            System.IO.TextWriter stream = new System.IO.StreamWriter(path); 
            try
            {                    
                serializer.Serialize(stream, result);
               stream.Close();
                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}
 }
}
