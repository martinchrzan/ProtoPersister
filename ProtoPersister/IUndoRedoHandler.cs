using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proto
{
    internal interface IUndoRedoHandler<T>
    {
        Tuple<T, string> Undo(T currentObject, string historyId);

        Tuple<T, string> Redo(T currentObject, string historyId);

        void Push(T item, string historyId);

        bool CanUndo();

        bool CanRedo();

        event EventHandler CanUndoChanged;

        event EventHandler CanRedoChanged;
    }
}
