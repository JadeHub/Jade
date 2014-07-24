using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.CodeCompletion;
using JadeUtils.IO;
using LibClang.CodeCompletion;

namespace JadeControls.EditorControl.CodeCompletion
{
    public interface ICompletionEngine
    {
        void BeginSelection(int offset);
        
        /// <summary>
        /// Extract the 'word' at the point line, column.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        string ExtractTriggerWord(int offset, out int startOffset);
    }

    public class CompletionContext
    {
        public TextArea TextArea;
        public IResultProvider ResultProvider;
        public FilePath Path;
        public CompletionSelection.CallbackDel Callback;
        public string TriggerWord;
        public int Offset;
        public int Line;
        public int Column;
    }

    public class CompletionSelection
    {
        public delegate void CallbackDel(LibClang.CodeCompletion.Result selection, ISegment completionSegment, EventArgs insertionRequestEventArgs);

        private CompletionContext _context;
        private CompletionWindow _completionWindow;
        private Action _onComplete;
        
        public static CompletionSelection Create(CompletionContext context, Action onComplete)
        {
            try
            {
                return new CompletionSelection(context, onComplete);
            }
            catch(System.Exception )
            {
                return null;
            }
        }

        private CompletionSelection(CompletionContext context, Action onComplete)
        {
            _context = context;
            _onComplete = onComplete;
            ResultSet results = _context.ResultProvider.GetResults(_context.Path.Str, _context.Line, _context.Column, this);
            if (results == null)
                throw new System.Exception();

            _completionWindow = new CompletionWindow(_context.TextArea);
            foreach (IResult cd in results.Results)
            {
                _completionWindow.CompletionList.CompletionData.Add(cd);
            }

            _completionWindow.StartOffset = _context.Offset;
            _completionWindow.EndOffset = _completionWindow.StartOffset + _context.TriggerWord.Length;

            if (_context.TriggerWord.Length > 0)
            {
                _completionWindow.CompletionList.SelectItem(_context.TriggerWord);
                if(_completionWindow.CompletionList.SelectedItem == null) //nothing to display
                {
                    _completionWindow = null;
                    _onComplete();
                    ;return;
                }
            }

            _completionWindow.Show();
            
            _completionWindow.Closed += (o, args) =>
            {   
                _completionWindow = null;
                _onComplete();
            };
        }

        public void SelectResult(LibClang.CodeCompletion.Result result, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            _context.Callback(result, completionSegment, insertionRequestEventArgs);
        }

        public bool RequestInsertion(System.Windows.Input.TextCompositionEventArgs e)
        {
            if (_completionWindow != null) //todo fix
            {
                _completionWindow.CompletionList.RequestInsertion(e);
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Manages CodeCompletion for a single source file
    /// </summary>
    public class CompletionEngine : ICompletionEngine
    {
        private enum Phase
        {
            Inactive,
            Selecting, //User is selecting from the list of results
            Completing //User is completing their sellection
        }

        private TextArea _textArea;
        private CompletionSelection _currentSelection;
        private IResultProvider _resulsProvider;
        private JadeCore.ITextDocument _sourceDoc;
        private Phase _currentPhase;

        public CompletionEngine(JadeCore.ITextDocument sourceDoc, TextArea textArea, IResultProvider resultsProvider)
        {
            _textArea = textArea;
            _sourceDoc = sourceDoc;
            _resulsProvider = resultsProvider;
            _textArea.TextEntering += TextAreaTextEntering;
            _textArea.TextEntered += TextAreaTextEntered;
            _currentPhase = Phase.Inactive;
        }

        private bool IsCompletionStartChar(char c)
        {
            return char.IsLetter(c) || c == '.' || c == '(' || c == '<';
        }

        private void TextAreaTextEntered(object sender, TextCompositionEventArgs e)
        {
            if (_currentSelection == null && e.Text.Length > 0 && IsCompletionStartChar(e.Text[0]))
            {
                BeginSelection(_textArea.Caret.Offset);// - e.Text.Length, e.Text);
            }
        }

        private void TextAreaTextEntering(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (_currentSelection != null && e.Text.Length > 0)
            {
                if (!char.IsLetterOrDigit(e.Text[0]) )
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    _currentSelection.RequestInsertion(e); ;
                }
            }
        }

        public bool IsInactive
        { 
            get { return _currentPhase == Phase.Inactive; } 
        }

        public bool IsSelecting
        {
            get { return _currentPhase == Phase.Selecting; }
        }

        public bool IsCompleting
        {
            get { return _currentPhase == Phase.Completing; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="triggerWord">Initial text to be selected in the completion window. line and col should point to the start of this word</param>
        /// <param name="line"></param>
        /// <param name="col"></param>
        public void BeginSelection(int offset)
        {
            if (_currentSelection != null) return;

            string triggerWord = ExtractTriggerWord(offset, out offset);

            int line, col;
            if (!_sourceDoc.GetLineAndColumnForOffset(offset, out line, out col))
                return;

            CompletionContext context = new CompletionContext
            {
                Line = line,
                Column = col,
                Offset = offset,
                Path = _sourceDoc.File.Path,
                ResultProvider = _resulsProvider,
                TextArea = _textArea,
                TriggerWord = triggerWord,
                Callback = delegate(LibClang.CodeCompletion.Result selection, ISegment completionSegment, EventArgs insertionRequestEventArgs) 
                    {
                        int caretLoc;
                        int startOffset = completionSegment.Offset;
                        string s = GetInsertionText(selection, insertionRequestEventArgs, out caretLoc);
                        _textArea.Document.Replace(completionSegment, s);
                        if (caretLoc != -1)
                        {
                            _textArea.Caret.Offset = startOffset + caretLoc;
                        }
                    }
            };
            _currentSelection = CompletionSelection.Create(context, delegate { _currentSelection = null; });
        }
        
        public string ExtractTriggerWord(int offset, out int startOffset)
        {
            startOffset = offset;
            while (startOffset > 0 && (char.IsLetterOrDigit(_sourceDoc.Text[startOffset - 1])))
                startOffset--;
            int endOffset = offset;
            while (char.IsLetterOrDigit(_sourceDoc.Text[endOffset]))
                endOffset++;

            int line = 0;
            int col = 0;
            _sourceDoc.GetLineAndColumnForOffset(startOffset, out line, out col);
            return _sourceDoc.Text.Substring(startOffset, endOffset - startOffset);
        }

        private string GetInsertionText(LibClang.CodeCompletion.Result result, EventArgs insertionRequestEventArgs, out int caretLocation)
        {
            StringBuilder sb = new StringBuilder();
            
            caretLocation = -1;
            foreach(ResultChunk rc in result.Chunks)
            {
                switch(rc.Kind)
                {
                    case (ChunkKind.TypedText):
                    case (ChunkKind.Text):                    
                        sb.Append(rc.Text);
                        break;
                    case (ChunkKind.Placeholder):
                        if (caretLocation == -1)
                            caretLocation = sb.Length;
                        break;
                    case (ChunkKind.LeftParen):
                        sb.Append('(');
                        break;
                    case (ChunkKind.RightParen):
                        sb.Append(')');
                        break;
                    case (ChunkKind.LeftBracket):
                        sb.Append(']');
                        break;
                    case (ChunkKind.RightBracket):
                        sb.Append('[');
                        break;
                    case (ChunkKind.LeftBrace):
                        sb.Append('{');
                        break;
                    case (ChunkKind.RightBrace):
                        sb.Append('}');
                        break;
                    case (ChunkKind.LeftAngle):
                        sb.Append('<');
                        break;
                    case (ChunkKind.RightAngle):
                        sb.Append('>');
                        break;
                    case (ChunkKind.Comma):
                        sb.Append(", ");
                        break;
                    case (ChunkKind.Colon):
                        sb.Append(':');
                        break;
                    case (ChunkKind.SemiColon):
                        sb.Append(';');
                        break;
                    case (ChunkKind.Equal):
                        sb.Append('=');
                        break;
                    case (ChunkKind.HorizontalSpace):
                        sb.Append(' ');
                        break;
                    case (ChunkKind.VerticalSpace):
                        sb.Append('\n');
                        break;
                }
            }
            if (insertionRequestEventArgs is System.Windows.Input.TextCompositionEventArgs)
            {
              //  sb.Append((insertionRequestEventArgs as System.Windows.Input.TextCompositionEventArgs).Text);
            }
            
            return sb.ToString();
        }
    }
}
