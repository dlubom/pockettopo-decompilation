using System.Globalization;
using System.Windows.Forms;

namespace PocketTopo;

public abstract class Importer : Extension
{
	private CultureInfo inv = CultureInfo.InvariantCulture;

	internal static MainForm main;

	internal static MenuItem menu;

	public void AddMenuItem(MenuItem item)
	{
		menu.MenuItems.Add(item);
	}

	public Survey ActualSurvey()
	{
		return main.dataSet.survey;
	}

	public bool SaveActual()
	{
		return main.SaveDialog();
	}

	public void Reset()
	{
		DataSet.Reset();
		Cursor.Current = Cursors.WaitCursor;
		main.dataSet.survey.database.RaiseListChangedEvents = false;
	}

	public void Open()
	{
		main.dataSet.survey.Reset();
		main.dataSet.survey.SetBoundary();
		Trip.Reset();
	}

	public void Close()
	{
		main.dataSet.survey.Invalidate();
		DataSet.MakeDirty();
	}

	public void Update()
	{
		main.dataSet.survey.database.RaiseListChangedEvents = true;
		main.dataSet.survey.database.ResetBindings(metadataChanged: false);
		Cursor.Current = null;
		main.dataSet.outline.Reset(resetMapping: true);
		main.dataSet.sideview.Reset(resetMapping: true);
		Config.actualFile = "";
		main.SelectLast(both: true);
	}
}
