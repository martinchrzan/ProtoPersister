using System;
using System.Collections.Generic;

namespace Proto
{
    internal class UndoRedoHandler<T> : IUndoRedoHandler<T>
    {
        private Stack<Tuple<T,string>> _undoStack;
        private Stack<Tuple<T, string>> _redoStack;
        private readonly object _lock = new object();

        public UndoRedoHandler(int capacity)
        {
            _undoStack = new Stack<Tuple<T, string>>(capacity);
            _redoStack = new Stack<Tuple<T, string>>(capacity);
        }

        public event EventHandler CanUndoChanged;

        public event EventHandler CanRedoChanged;

        public bool CanRedo() => _redoStack.Count > 0;

        public bool CanUndo() => _undoStack.Count > 0;
        
        public void Push(T item, string historyId)
        {
            lock(_lock)
            {
                CheckCanUndoRedo(() => 
                {
                    _undoStack.Push(new Tuple<T, string>(item, historyId));
                    // we need to destroy redo history as the new item was pushed into undo stack
                    _redoStack.Clear();
                    // no need to return anything
                    return null;
                });
            }
        }

        public Tuple<T, string> Redo(T currentObject, string historyId)
        {
            lock (_lock)
            {
                return CheckCanUndoRedo(() =>
                {
                    _undoStack.Push(new Tuple<T, string>(currentObject, historyId));
                    return _redoStack.Pop();
                });
            }
        }

        public Tuple<T, string> Undo(T currentObject, string historyId)
        {
            lock (_lock)
            {
                return CheckCanUndoRedo(() =>
                {
                    _redoStack.Push(new Tuple<T, string>(currentObject, historyId));
                    return _undoStack.Pop();
                });
            }
        }

        private Tuple<T, string> CheckCanUndoRedo(Func<Tuple<T,string>> action)
        {
            var canUndo = CanUndo();
            var canRedo = CanRedo();
            var result = action.Invoke();

            if(canUndo != CanUndo())
            {
                RaiseCanUndoChanged();
            }
            if(canRedo != CanRedo())
            {
                RaiseCanRedoChanged();
            }

            return result;
        }

        private void RaiseCanUndoChanged()
        {
            CanUndoChanged?.Invoke(this, EventArgs.Empty);
        }

        private void RaiseCanRedoChanged()
        {
            CanRedoChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
