using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace PocketTopo;

internal class CalibForm : Form
{
	private MainForm mainForm;

	private byte[] readData;

	private int numBlocks;

	private IContainer components = null;

	private MainMenu calibMenu;

	private MenuItem menuMenu;

	private DataGrid dataGrid;

	private MenuItem menuOpen;

	private MenuItem menuSave;

	private MenuItem menuExport;

	private MenuItem menuNew;

	private OpenFileDialog openFileDialog;

	private SaveFileDialog saveFileDialog;

	private SaveFileDialog txtFileDialog;

	private BindingSource database;

	private DataGridTableStyle dataGridTableStyle1;

	private DataGridTextBoxColumn dataGridTextBoxColumn1;

	private DataGridTextBoxColumn dataGridTextBoxColumn2;

	private DataGridTextBoxColumn dataGridTextBoxColumn3;

	private DataGridTextBoxColumn dataGridTextBoxColumn4;

	private DataGridTextBoxColumn dataGridTextBoxColumn5;

	private DataGridTextBoxColumn dataGridTextBoxColumn6;

	private DataGridTextBoxColumn dataGridTextBoxColumn7;

	private DataGridTextBoxColumn dataGridTextBoxColumn8;

	private DataGridTextBoxColumn dataGridTextBoxColumn9;

	private DataGridTextBoxColumn dataGridTextBoxColumn10;

	private DataGridTextBoxColumn dataGridTextBoxColumn11;

	private DataGridTextBoxColumn dataGridTextBoxColumn12;

	private DataGridTextBoxColumn dataGridTextBoxColumn13;

	private DataGridTextBoxColumn dataGridTextBoxColumn14;

	private DataGridTextBoxColumn dataGridTextBoxColumn15;

	private ContextMenu contextMenu;

	private DataGridTextBoxColumn dataGridTextBoxColumn16;

	private MenuItem menuBT;

	private MenuItem menuSep1;

	private MenuItem menuConnect;

	private MenuItem menuDisconn;

	private MenuItem menuAuto;

	private MenuItem menuSep2;

	private MenuItem menuStart;

	private MenuItem menuStop;

	private MenuItem menuUpdate;

	private Label connFailed;

	private Label inProgress;

	private MenuItem menuEvaluate;

	private StatusBar statusBar;

	private Panel panel;

	private MenuItem menuRead;

	private MenuItem menuSep3;

	private MenuItem menuExtra;

	private MenuItem menuInitial;

	private MenuItem menuResult;

	private MenuItem menuNonlin;

	private MenuItem menuItem1;

	private MenuItem menuLOn;

	private MenuItem menuLOff;

	private MenuItem menuMeasure;

	internal CalibForm(MainForm main)
	{
		InitializeComponent();
		CheckFramework();
		mainForm = main;
		Calibrate.database = database;
		Calibrate.statusBar = statusBar;
		statusBar.Text = "";
		Link.ProgressEvent += ShowProgress;
	}

	private void CheckFramework()
	{
		typeof(DataGridTableStyle).GetProperty("RowHeadersVisible")?.SetValue(dataGridTableStyle1, false, null);
		typeof(Control).GetEvent("Resize")?.AddEventHandler(this, new EventHandler(SizeChanged));
		database.AllowNew = false;
		PropertyInfo property = typeof(BindingSource).GetProperty("AllowEdit");
		if (property != null && property.CanWrite)
		{
			property.SetValue(database, false, null);
		}
		property = typeof(BindingSource).GetProperty("AllowRemove");
		if (property != null && property.CanWrite)
		{
			property.SetValue(database, false, null);
		}
	}

	private new void SizeChanged(object sender, EventArgs e)
	{
		panel.Height = statusBar.Top;
		Refresh();
	}

	internal void LoadFile()
	{
		DialogResult dialogResult = openFileDialog.ShowDialog();
		if (dialogResult != DialogResult.OK)
		{
			return;
		}
		string fileName = openFileDialog.FileName;
		saveFileDialog.FileName = fileName;
		txtFileDialog.FileName = Path.GetFileNameWithoutExtension(fileName);
		try
		{
			using FileStream input = File.OpenRead(fileName);
			using BinaryReader binaryReader = new BinaryReader(input);
			int num = binaryReader.ReadInt32();
			int num2 = num >> 24;
			if ((num & 0xFFFFFF) == 7102787 && num2 == 1)
			{
				Calibrate.Read(binaryReader, num2);
				dialogResult = DialogResult.Yes;
			}
			binaryReader.Close();
		}
		catch
		{
		}
		if (dialogResult != DialogResult.Yes)
		{
			MessageBox.Show(mainForm.fileError.Text + fileName);
		}
	}

	private DialogResult SaveFile(string name)
	{
		DialogResult dialogResult = DialogResult.OK;
		try
		{
			if (name == null)
			{
				dialogResult = saveFileDialog.ShowDialog();
				name = saveFileDialog.FileName;
			}
			if (dialogResult == DialogResult.OK)
			{
				using FileStream output = File.Open(name, FileMode.Create);
				using BinaryWriter binaryWriter = new BinaryWriter(output);
				binaryWriter.Write((byte)67);
				binaryWriter.Write((byte)97);
				binaryWriter.Write((byte)108);
				binaryWriter.Write((byte)1);
				Calibrate.Write(binaryWriter);
				binaryWriter.Close();
			}
		}
		catch
		{
			dialogResult = DialogResult.Cancel;
		}
		return dialogResult;
	}

	internal bool SaveDialog()
	{
		if (Calibrate.dirty)
		{
			DialogResult dialogResult = MessageBox.Show(mainForm.saveChanges.Text, Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
			if (dialogResult == DialogResult.Yes)
			{
				dialogResult = SaveFile(null);
			}
			if (dialogResult == DialogResult.Cancel)
			{
				return false;
			}
		}
		return true;
	}

	internal void ShowProgress(object o, EventArgs e)
	{
		int num = Link.Progress();
		if (num < 0)
		{
			readData = null;
			statusBar.Text = connFailed.Text;
		}
		else if (num >= numBlocks)
		{
			if (readData != null)
			{
				Calibrate.ReadDevice(readData);
			}
			readData = null;
			statusBar.Text = "OK";
		}
		else
		{
			statusBar.Text = inProgress.Text + ".............".Substring(0, num);
		}
	}

	internal void menuEvaluate_Click(object sender, EventArgs e)
	{
		Calibrate.Evaluate(evaluate: true);
	}

	private void menuMenu_Popup(object sender, EventArgs e)
	{
		menuStart.Enabled = Config.portName != "";
		menuStop.Enabled = Config.portName != "";
		int num = Link.Progress();
		menuUpdate.Enabled = Config.portName != "" && !Link.Busy() && Calibrate.CoeffValid();
		menuRead.Enabled = Config.portName != "" && !Link.Busy();
		menuBT_Popup(null, e);
	}

	internal void menuStart_Click(object sender, EventArgs e)
	{
		if (!Link.StartCalib())
		{
			statusBar.Text = connFailed.Text;
		}
	}

	internal void menuStop_Click(object sender, EventArgs e)
	{
		if (!Link.StopCalib())
		{
			statusBar.Text = connFailed.Text;
		}
	}

	internal void menuUpdate_Click(object sender, EventArgs e)
	{
		Calibrate.Evaluate(evaluate: true);
		if (Calibrate.CoeffValid())
		{
			byte[] coeff = Calibrate.GetCoeff();
			numBlocks = coeff.Length >> 2;
			if (!Link.StartWrite(32784, coeff))
			{
				statusBar.Text = connFailed.Text;
			}
		}
	}

	internal void menuRead_Click(object sender, EventArgs e)
	{
		if (SaveDialog())
		{
			readData = Calibrate.GetCoeff();
			numBlocks = readData.Length >> 2;
			if (!Link.StartRead(32784, readData))
			{
				statusBar.Text = connFailed.Text;
			}
		}
	}

	private void menuNew_Click(object sender, EventArgs e)
	{
		if (SaveDialog())
		{
			Calibrate.New();
		}
	}

	private void menuInitial_Click(object sender, EventArgs e)
	{
		if (SaveDialog())
		{
			Calibrate.Initial();
		}
	}

	private void menuOpen_Click(object sender, EventArgs e)
	{
		if (SaveDialog())
		{
			LoadFile();
		}
	}

	private void menuSave_Click(object sender, EventArgs e)
	{
		SaveFile(null);
	}

	private void menuExpText_Click(object sender, EventArgs e)
	{
		DialogResult dialogResult = txtFileDialog.ShowDialog();
		if (dialogResult != DialogResult.OK)
		{
			return;
		}
		using FileStream stream = File.Open(txtFileDialog.FileName, FileMode.Create);
		using TextWriter textWriter = new StreamWriter(stream);
		Calibrate.SaveText(textWriter);
		textWriter.Close();
	}

	private void dataGrid_Click(object sender, EventArgs e)
	{
		Point point = dataGrid.PointToClient(Control.MousePosition);
		DataGrid.HitTestInfo hitTestInfo = dataGrid.HitTest(point.X, point.Y);
		if (hitTestInfo.Row >= 0 && hitTestInfo.Row < database.Count)
		{
			if (hitTestInfo.Column == 0 || hitTestInfo.Column == 15)
			{
				CalibEntry calibEntry = (CalibEntry)database[hitTestInfo.Row];
				calibEntry.ToggleValid();
				database.ResetItem(hitTestInfo.Row);
				Calibrate.Evaluate(evaluate: false);
			}
			else if (hitTestInfo.Column == 1)
			{
				CalibEntry calibEntry = (CalibEntry)database[hitTestInfo.Row];
				calibEntry.IncGroup();
				database.ResetItem(hitTestInfo.Row);
				Calibrate.Evaluate(evaluate: false);
			}
		}
	}

	internal void menuBT_Popup(object sender, EventArgs e)
	{
		menuDisconn.Enabled = Link.IsOpen();
		menuConnect.Enabled = Config.portName != "" && !Link.IsConnected();
		menuAuto.Enabled = Config.portName != "";
		menuAuto.Checked = Config.autoConnect;
	}

	internal void menuConnect_Click(object sender, EventArgs e)
	{
		Link.Reconnect();
	}

	internal void menuDisconn_Click(object sender, EventArgs e)
	{
		Link.Close();
	}

	internal void menuAuto_Click(object sender, EventArgs e)
	{
		Config.autoConnect = !Config.autoConnect;
	}

	private void dataGrid_CurrentCellChanged(object sender, EventArgs e)
	{
	}

	private void CalibForm_Activated(object sender, EventArgs e)
	{
		Link.appActive = true;
	}

	private void CalibForm_Deactivate(object sender, EventArgs e)
	{
		Link.appActive = false;
	}

	private void menuResult_Click(object sender, EventArgs e)
	{
		TextBox textBox = new TextBox();
		textBox.Dock = DockStyle.Fill;
		textBox.Multiline = true;
		textBox.ScrollBars = ScrollBars.Vertical;
		textBox.Text = Calibrate.GetCoeffText();
		Form form = new Form();
		form.Controls.Add(textBox);
		form.ShowDialog();
	}

	private void menuNonlin_Click(object sender, EventArgs e)
	{
		menuNonlin.Checked = !menuNonlin.Checked;
		Calibrate.SetNonlinearity(menuNonlin.Checked);
	}

	private void menuLOn_Click(object sender, EventArgs e)
	{
		if (!Link.LaserOn())
		{
			statusBar.Text = connFailed.Text;
		}
	}

	private void menuLOff_Click(object sender, EventArgs e)
	{
		if (!Link.LaserOff())
		{
			statusBar.Text = connFailed.Text;
		}
	}

	private void menuMeasure_Click(object sender, EventArgs e)
	{
		if (!Link.Measure())
		{
			statusBar.Text = connFailed.Text;
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PocketTopo.CalibForm));
		this.calibMenu = new System.Windows.Forms.MainMenu();
		this.menuMenu = new System.Windows.Forms.MenuItem();
		this.menuBT = new System.Windows.Forms.MenuItem();
		this.menuConnect = new System.Windows.Forms.MenuItem();
		this.menuDisconn = new System.Windows.Forms.MenuItem();
		this.menuAuto = new System.Windows.Forms.MenuItem();
		this.menuItem1 = new System.Windows.Forms.MenuItem();
		this.menuLOn = new System.Windows.Forms.MenuItem();
		this.menuLOff = new System.Windows.Forms.MenuItem();
		this.menuMeasure = new System.Windows.Forms.MenuItem();
		this.menuSep1 = new System.Windows.Forms.MenuItem();
		this.menuNew = new System.Windows.Forms.MenuItem();
		this.menuOpen = new System.Windows.Forms.MenuItem();
		this.menuSave = new System.Windows.Forms.MenuItem();
		this.menuExport = new System.Windows.Forms.MenuItem();
		this.menuSep2 = new System.Windows.Forms.MenuItem();
		this.menuStart = new System.Windows.Forms.MenuItem();
		this.menuStop = new System.Windows.Forms.MenuItem();
		this.menuUpdate = new System.Windows.Forms.MenuItem();
		this.menuSep3 = new System.Windows.Forms.MenuItem();
		this.menuNonlin = new System.Windows.Forms.MenuItem();
		this.menuExtra = new System.Windows.Forms.MenuItem();
		this.menuInitial = new System.Windows.Forms.MenuItem();
		this.menuRead = new System.Windows.Forms.MenuItem();
		this.menuResult = new System.Windows.Forms.MenuItem();
		this.menuEvaluate = new System.Windows.Forms.MenuItem();
		this.database = new System.Windows.Forms.BindingSource(this.components);
		this.dataGrid = new System.Windows.Forms.DataGrid();
		this.contextMenu = new System.Windows.Forms.ContextMenu();
		this.dataGridTableStyle1 = new System.Windows.Forms.DataGridTableStyle();
		this.dataGridTextBoxColumn1 = new System.Windows.Forms.DataGridTextBoxColumn();
		this.dataGridTextBoxColumn15 = new System.Windows.Forms.DataGridTextBoxColumn();
		this.dataGridTextBoxColumn14 = new System.Windows.Forms.DataGridTextBoxColumn();
		this.dataGridTextBoxColumn11 = new System.Windows.Forms.DataGridTextBoxColumn();
		this.dataGridTextBoxColumn12 = new System.Windows.Forms.DataGridTextBoxColumn();
		this.dataGridTextBoxColumn13 = new System.Windows.Forms.DataGridTextBoxColumn();
		this.dataGridTextBoxColumn2 = new System.Windows.Forms.DataGridTextBoxColumn();
		this.dataGridTextBoxColumn3 = new System.Windows.Forms.DataGridTextBoxColumn();
		this.dataGridTextBoxColumn4 = new System.Windows.Forms.DataGridTextBoxColumn();
		this.dataGridTextBoxColumn5 = new System.Windows.Forms.DataGridTextBoxColumn();
		this.dataGridTextBoxColumn6 = new System.Windows.Forms.DataGridTextBoxColumn();
		this.dataGridTextBoxColumn7 = new System.Windows.Forms.DataGridTextBoxColumn();
		this.dataGridTextBoxColumn8 = new System.Windows.Forms.DataGridTextBoxColumn();
		this.dataGridTextBoxColumn9 = new System.Windows.Forms.DataGridTextBoxColumn();
		this.dataGridTextBoxColumn10 = new System.Windows.Forms.DataGridTextBoxColumn();
		this.dataGridTextBoxColumn16 = new System.Windows.Forms.DataGridTextBoxColumn();
		this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
		this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
		this.txtFileDialog = new System.Windows.Forms.SaveFileDialog();
		this.connFailed = new System.Windows.Forms.Label();
		this.inProgress = new System.Windows.Forms.Label();
		this.statusBar = new System.Windows.Forms.StatusBar();
		this.panel = new System.Windows.Forms.Panel();
		this.panel.SuspendLayout();
		base.SuspendLayout();
		this.calibMenu.MenuItems.Add(this.menuMenu);
		this.calibMenu.MenuItems.Add(this.menuEvaluate);
		this.menuMenu.MenuItems.Add(this.menuBT);
		this.menuMenu.MenuItems.Add(this.menuSep1);
		this.menuMenu.MenuItems.Add(this.menuNew);
		this.menuMenu.MenuItems.Add(this.menuOpen);
		this.menuMenu.MenuItems.Add(this.menuSave);
		this.menuMenu.MenuItems.Add(this.menuExport);
		this.menuMenu.MenuItems.Add(this.menuSep2);
		this.menuMenu.MenuItems.Add(this.menuStart);
		this.menuMenu.MenuItems.Add(this.menuStop);
		this.menuMenu.MenuItems.Add(this.menuUpdate);
		this.menuMenu.MenuItems.Add(this.menuSep3);
		this.menuMenu.MenuItems.Add(this.menuNonlin);
		this.menuMenu.MenuItems.Add(this.menuExtra);
		resources.ApplyResources(this.menuMenu, "menuMenu");
		this.menuMenu.Popup += new System.EventHandler(menuMenu_Popup);
		this.menuBT.MenuItems.Add(this.menuConnect);
		this.menuBT.MenuItems.Add(this.menuDisconn);
		this.menuBT.MenuItems.Add(this.menuAuto);
		this.menuBT.MenuItems.Add(this.menuItem1);
		this.menuBT.MenuItems.Add(this.menuLOn);
		this.menuBT.MenuItems.Add(this.menuLOff);
		this.menuBT.MenuItems.Add(this.menuMeasure);
		resources.ApplyResources(this.menuBT, "menuBT");
		resources.ApplyResources(this.menuConnect, "menuConnect");
		this.menuConnect.Click += new System.EventHandler(menuConnect_Click);
		resources.ApplyResources(this.menuDisconn, "menuDisconn");
		this.menuDisconn.Click += new System.EventHandler(menuDisconn_Click);
		resources.ApplyResources(this.menuAuto, "menuAuto");
		this.menuAuto.Click += new System.EventHandler(menuAuto_Click);
		resources.ApplyResources(this.menuItem1, "menuItem1");
		resources.ApplyResources(this.menuLOn, "menuLOn");
		this.menuLOn.Click += new System.EventHandler(menuLOn_Click);
		resources.ApplyResources(this.menuLOff, "menuLOff");
		this.menuLOff.Click += new System.EventHandler(menuLOff_Click);
		resources.ApplyResources(this.menuMeasure, "menuMeasure");
		this.menuMeasure.Click += new System.EventHandler(menuMeasure_Click);
		resources.ApplyResources(this.menuSep1, "menuSep1");
		resources.ApplyResources(this.menuNew, "menuNew");
		this.menuNew.Click += new System.EventHandler(menuNew_Click);
		resources.ApplyResources(this.menuOpen, "menuOpen");
		this.menuOpen.Click += new System.EventHandler(menuOpen_Click);
		resources.ApplyResources(this.menuSave, "menuSave");
		this.menuSave.Click += new System.EventHandler(menuSave_Click);
		resources.ApplyResources(this.menuExport, "menuExport");
		this.menuExport.Click += new System.EventHandler(menuExpText_Click);
		resources.ApplyResources(this.menuSep2, "menuSep2");
		resources.ApplyResources(this.menuStart, "menuStart");
		this.menuStart.Click += new System.EventHandler(menuStart_Click);
		resources.ApplyResources(this.menuStop, "menuStop");
		this.menuStop.Click += new System.EventHandler(menuStop_Click);
		resources.ApplyResources(this.menuUpdate, "menuUpdate");
		this.menuUpdate.Click += new System.EventHandler(menuUpdate_Click);
		resources.ApplyResources(this.menuSep3, "menuSep3");
		resources.ApplyResources(this.menuNonlin, "menuNonlin");
		this.menuNonlin.Click += new System.EventHandler(menuNonlin_Click);
		this.menuExtra.MenuItems.Add(this.menuInitial);
		this.menuExtra.MenuItems.Add(this.menuRead);
		this.menuExtra.MenuItems.Add(this.menuResult);
		resources.ApplyResources(this.menuExtra, "menuExtra");
		resources.ApplyResources(this.menuInitial, "menuInitial");
		this.menuInitial.Click += new System.EventHandler(menuInitial_Click);
		resources.ApplyResources(this.menuRead, "menuRead");
		this.menuRead.Click += new System.EventHandler(menuRead_Click);
		resources.ApplyResources(this.menuResult, "menuResult");
		this.menuResult.Click += new System.EventHandler(menuResult_Click);
		resources.ApplyResources(this.menuEvaluate, "menuEvaluate");
		this.menuEvaluate.Click += new System.EventHandler(menuEvaluate_Click);
		this.database.DataSource = typeof(PocketTopo.CalibEntry);
		this.dataGrid.BackgroundColor = System.Drawing.Color.FromArgb(128, 128, 128);
		this.dataGrid.ContextMenu = this.contextMenu;
		this.dataGrid.DataSource = this.database;
		resources.ApplyResources(this.dataGrid, "dataGrid");
		this.dataGrid.Name = "dataGrid";
		this.dataGrid.RowHeadersVisible = false;
		this.dataGrid.TableStyles.Add(this.dataGridTableStyle1);
		this.dataGrid.TabStop = false;
		this.dataGrid.DoubleClick += new System.EventHandler(dataGrid_Click);
		this.dataGrid.CurrentCellChanged += new System.EventHandler(dataGrid_CurrentCellChanged);
		this.dataGrid.Click += new System.EventHandler(dataGrid_Click);
		this.dataGridTableStyle1.GridColumnStyles.Add(this.dataGridTextBoxColumn1);
		this.dataGridTableStyle1.GridColumnStyles.Add(this.dataGridTextBoxColumn15);
		this.dataGridTableStyle1.GridColumnStyles.Add(this.dataGridTextBoxColumn14);
		this.dataGridTableStyle1.GridColumnStyles.Add(this.dataGridTextBoxColumn11);
		this.dataGridTableStyle1.GridColumnStyles.Add(this.dataGridTextBoxColumn12);
		this.dataGridTableStyle1.GridColumnStyles.Add(this.dataGridTextBoxColumn13);
		this.dataGridTableStyle1.GridColumnStyles.Add(this.dataGridTextBoxColumn2);
		this.dataGridTableStyle1.GridColumnStyles.Add(this.dataGridTextBoxColumn3);
		this.dataGridTableStyle1.GridColumnStyles.Add(this.dataGridTextBoxColumn4);
		this.dataGridTableStyle1.GridColumnStyles.Add(this.dataGridTextBoxColumn5);
		this.dataGridTableStyle1.GridColumnStyles.Add(this.dataGridTextBoxColumn6);
		this.dataGridTableStyle1.GridColumnStyles.Add(this.dataGridTextBoxColumn7);
		this.dataGridTableStyle1.GridColumnStyles.Add(this.dataGridTextBoxColumn8);
		this.dataGridTableStyle1.GridColumnStyles.Add(this.dataGridTextBoxColumn9);
		this.dataGridTableStyle1.GridColumnStyles.Add(this.dataGridTextBoxColumn10);
		this.dataGridTableStyle1.GridColumnStyles.Add(this.dataGridTextBoxColumn16);
		this.dataGridTableStyle1.MappingName = "CalibEntry";
		resources.ApplyResources(this.dataGridTextBoxColumn1, "dataGridTextBoxColumn1");
		resources.ApplyResources(this.dataGridTextBoxColumn15, "dataGridTextBoxColumn15");
		resources.ApplyResources(this.dataGridTextBoxColumn14, "dataGridTextBoxColumn14");
		resources.ApplyResources(this.dataGridTextBoxColumn11, "dataGridTextBoxColumn11");
		resources.ApplyResources(this.dataGridTextBoxColumn12, "dataGridTextBoxColumn12");
		resources.ApplyResources(this.dataGridTextBoxColumn13, "dataGridTextBoxColumn13");
		resources.ApplyResources(this.dataGridTextBoxColumn2, "dataGridTextBoxColumn2");
		resources.ApplyResources(this.dataGridTextBoxColumn3, "dataGridTextBoxColumn3");
		resources.ApplyResources(this.dataGridTextBoxColumn4, "dataGridTextBoxColumn4");
		resources.ApplyResources(this.dataGridTextBoxColumn5, "dataGridTextBoxColumn5");
		resources.ApplyResources(this.dataGridTextBoxColumn6, "dataGridTextBoxColumn6");
		resources.ApplyResources(this.dataGridTextBoxColumn7, "dataGridTextBoxColumn7");
		resources.ApplyResources(this.dataGridTextBoxColumn8, "dataGridTextBoxColumn8");
		resources.ApplyResources(this.dataGridTextBoxColumn9, "dataGridTextBoxColumn9");
		resources.ApplyResources(this.dataGridTextBoxColumn10, "dataGridTextBoxColumn10");
		resources.ApplyResources(this.dataGridTextBoxColumn16, "dataGridTextBoxColumn16");
		resources.ApplyResources(this.openFileDialog, "openFileDialog");
		resources.ApplyResources(this.saveFileDialog, "saveFileDialog");
		resources.ApplyResources(this.txtFileDialog, "txtFileDialog");
		resources.ApplyResources(this.connFailed, "connFailed");
		this.connFailed.Name = "connFailed";
		resources.ApplyResources(this.inProgress, "inProgress");
		this.inProgress.Name = "inProgress";
		resources.ApplyResources(this.statusBar, "statusBar");
		this.statusBar.Name = "statusBar";
		this.panel.Controls.Add(this.dataGrid);
		resources.ApplyResources(this.panel, "panel");
		this.panel.Name = "panel";
		base.AutoScaleDimensions = new System.Drawing.SizeF(96f, 96f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
		resources.ApplyResources(this, "$this");
		base.Controls.Add(this.panel);
		base.Controls.Add(this.statusBar);
		base.Controls.Add(this.connFailed);
		base.Controls.Add(this.inProgress);
		base.Menu = this.calibMenu;
		base.Name = "CalibForm";
		base.Deactivate += new System.EventHandler(CalibForm_Deactivate);
		base.Activated += new System.EventHandler(CalibForm_Activated);
		this.panel.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
