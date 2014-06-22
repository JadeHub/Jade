using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;


namespace JadeControls.EditorControl.CodeCompletion
{
    public class CompletionData : ICSharpCode.AvalonEdit.CodeCompletion.ICompletionData
    {
        private LibClang.CodeCompletion.Result _result;

        public CompletionData(LibClang.CodeCompletion.Result r)
        {
            _result = r;
        }

        public ImageSource Image { get { return null; } }

        /// <summary>
        /// Gets the text. This property is used to filter the list of visible elements.
        /// </summary>
        public string Text 
        {
            get { return _result.TypedChunk.Text;}
        }

        /// <summary>
        /// The displayed content. This can be the same as 'Text', or a WPF UIElement if
        /// you want to display rich content.
        /// </summary>
        public object Content { get {return Text;} }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public object Description { get { return Text; } }

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
            textArea.Document.Replace(completionSegment, _result.TypedChunk.Text);
        }
    }
}
