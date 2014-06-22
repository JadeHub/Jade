using System;
using System.Collections.Generic;
using System.Linq;

namespace LibClang.CodeCompletion
{
    public class ResultChunk
    {
        private System.IntPtr _completionString;
        private uint _chunkIndex;

        public ResultChunk(IntPtr completionString, uint chunkIndex)
        {
            _completionString = completionString;
            _chunkIndex = chunkIndex;
        }

        public string Text
        {
            get { return Library.clang_getCompletionChunkText(_completionString, _chunkIndex).ManagedString; }
        }

        public ChunkKind Kind
        {
            get { return Library.clang_getCompletionChunkKind(_completionString, _chunkIndex); }
        }

        public override string ToString()
        {
            return Text;
        }
    }

    public class Result
    {
        private List<ResultChunk> _chunks;
        private List<string> _annotations;

        internal Result(Library.CXCompletionResult handle)
        {
            //we dont hold a ref to handle as it will be destroyed once the result set has been loaded
            _chunks = new List<ResultChunk>();
            _annotations = new List<string>();

            for (int i = 0; i < Library.clang_getNumCompletionChunks(handle.CompletionString); i++)
            {
                _chunks.Add(new ResultChunk(handle.CompletionString, (uint)i));
            }

            for (uint i = 0; i < Library.clang_getCompletionNumAnnotations(handle.CompletionString); i++)
            {
                _annotations.Add(Library.clang_getCompletionAnnotation(handle.CompletionString, i).ManagedString);
            }

            CursorKind = (LibClang.CursorKind)handle.CursorKind;
            CompletionPriority = Library.clang_getCompletionPriority(handle.CompletionString);
            Availability = Library.clang_getCompletionAvailability(handle.CompletionString);
            BriefComment = Library.clang_getCompletionBriefComment(handle.CompletionString).ManagedString;
        }

        public CursorKind CursorKind
        {
            get;
            private set;
        }

        public IEnumerable<ResultChunk> Chunks
        {
            get { return _chunks; }
        }

        public uint CompletionPriority
        {
            get;
            private set;
        }

        public AvailabilityKind Availability
        {
            get;
            private set;
        }

        public IEnumerable<string> Annotations
        {
            get { return _annotations; }
        }

        public string BriefComment
        {
            get;
            private set;
        }

        public IEnumerable<ResultChunk> GetChunks(ChunkKind kind)
        {
            foreach (ResultChunk c in _chunks)
            {
                if (c.Kind == kind)
                    yield return c;
            }
        }

        public override string ToString()
        {
            ResultChunk r = GetChunks(ChunkKind.TypedText).First();
            return r.Text;
        }
    }
}
