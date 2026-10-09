using System;

namespace PocketTopo;

internal class Undo
{
	private Survey survey;

	private Undoable undo;

	private Undoable redo;

	internal event EventHandler UndoChanged;

	internal Undo(Survey survey)
	{
		this.survey = survey;
	}

	internal bool CanUndo()
	{
		return undo != null;
	}

	internal bool CanRedo()
	{
		return redo != null;
	}

	internal void Reset()
	{
		undo = null;
		redo = null;
		this.UndoChanged(null, EventArgs.Empty);
	}

	internal void DoOp(Undoable op)
	{
		if (op.Do())
		{
			redo = null;
			Undoable.Push(ref undo, op);
			this.UndoChanged(op.obj, EventArgs.Empty);
			survey.Invalidate();
		}
	}

	internal void DoUndo()
	{
		if (undo != null)
		{
			Undoable undoable = Undoable.Pop(ref undo);
			undoable.Do();
			Undoable.Push(ref redo, undoable);
			this.UndoChanged(undoable.obj, EventArgs.Empty);
			survey.Invalidate();
		}
	}

	internal void DoRedo()
	{
		if (redo != null)
		{
			Undoable undoable = Undoable.Pop(ref redo);
			undoable.Do();
			Undoable.Push(ref undo, undoable);
			this.UndoChanged(undoable.obj, EventArgs.Empty);
			survey.Invalidate();
		}
	}

	internal Undoable LastOp()
	{
		return undo;
	}
}
