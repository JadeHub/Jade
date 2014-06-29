using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace JadeControls.EditorControl.CodeCompletion
{
    public interface IResult : ICSharpCode.AvalonEdit.CodeCompletion.ICompletionData
    {
        LibClang.CodeCompletion.Result Result { get; }
    }

    public class CompletionData : IResult
    {
        private LibClang.CodeCompletion.Result _result;
        private CompletionSelection _selection;

        public CompletionData(LibClang.CodeCompletion.Result r, CompletionSelection selection)
        {
            _result = r;
            _selection = selection;
        }

        public LibClang.CodeCompletion.Result Result 
        {
            get { return _result; }
        }

        public ImageSource Image { get { return null; } }

        /// <summary>
        /// Gets the text. This property is used to filter the list of visible elements.
        /// </summary>
        public string Text 
        {
            get 
            { 
                if(_result.TypedChunk != null)
                    return _result.TypedChunk.Text;
                return "";
            }
        }

        /// <summary>
        /// The displayed content. This can be the same as 'Text', or a WPF UIElement if
        /// you want to display rich content.
        /// </summary>
        public object Content { get {return Text;} }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public object Description { get { return _result.CursorKind.ToString(); } }

        /// <summary>
        /// Gets the priority. This property is used in the selection logic. You can use it to prefer selecting those items
        /// which the user is accessing most frequently.
        /// </summary>
        public double Priority { get { return _result.CompletionPriority; } }

        /// <summary>
        /// Perform the completion.
        /// </summary>
        /// <param name="textArea">The text area on which completion is performed.</param>
        /// <param name="completionSegment">The text segment that was used by the completion window if
        /// the user types (segment between CompletionWindow.StartOffset and CompletionWindow.EndOffset).</param>
        /// <param name="insertionRequestEventArgs">The EventArgs used for the insertion request.
        /// These can be TextCompositionEventArgs, KeyEventArgs, MouseEventArgs, depending on how
        /// the insertion was triggered.</param>
        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            _selection.SelectResult(_result, completionSegment);
        }

        public override bool Equals(object obj)
        {
            if(obj != null && obj is CompletionData)
            {
                CompletionData rhs = obj as CompletionData;
                return rhs.Result.CursorKind == Result.CursorKind && rhs.Text == Text;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            string s = _result.CursorKind.ToString() + Text;
            return s.GetHashCode();
        }
    }
}
