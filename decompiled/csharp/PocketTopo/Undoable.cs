namespace PocketTopo;

internal abstract class Undoable
{
	internal object obj;

	private Undoable next;

	internal abstract bool Do();

	internal static void Push(ref Undoable list, Undoable op)
	{
		op.next = list;
		list = op;
	}

	internal static Undoable Pop(ref Undoable list)
	{
		Undoable undoable = list;
		if (undoable != null)
		{
			list = undoable.next;
		}
		return undoable;
	}
}
