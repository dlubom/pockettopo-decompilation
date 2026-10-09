using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace PocketTopo;

internal class ToporobotForm : Form
{
	private ToporobotExp exporter;

	private bool canceled = false;

	private IContainer components = null;

	private MainMenu mainMenu;

	private MenuItem menuCancel;

	private NumericUpDown numericCode;

	private Label labelCode;

	private Label labelTrip;

	private SaveFileDialog saveFileDialog;

	private NumericUpDown numericTrip;

	private CheckBox checkBoxAutoDecl;

	private CheckBox checkBoxGuidelines;

	private MenuItem menuSave;

	private CheckBox checkBoxAdd;

	private CheckBox checkBoxManDecl;

	internal OpenFileDialog openFileDialog;

	private CheckBox checkBoxVertical;

	internal ToporobotForm(ToporobotExp exporter)
	{
		this.exporter = exporter;
		InitializeComponent();
		try
		{
			Input.Enable();
		}
		catch
		{
		}
	}

	private void Form_Closing(object sender, CancelEventArgs e)
	{
		if (!canceled)
		{
			saveFileDialog.FileName = Path.GetFileNameWithoutExtension(Config.actualFile);
			DialogResult dialogResult = DialogResult.OK;
			string text = null;
			if (checkBoxAdd.Checked)
			{
				dialogResult = openFileDialog.ShowDialog();
				text = openFileDialog.FileName;
				saveFileDialog.FileName = text;
			}
			if (dialogResult == DialogResult.OK)
			{
				dialogResult = saveFileDialog.ShowDialog();
				if (dialogResult == DialogResult.OK)
				{
					exporter.WriteExport(text, saveFileDialog.FileName, decimal.ToInt32(numericCode.Value), decimal.ToInt32(numericTrip.Value), checkBoxAutoDecl.Checked, checkBoxManDecl.Checked, checkBoxGuidelines.Checked, checkBoxVertical.Checked);
				}
			}
		}
		try
		{
			Input.Disable();
		}
		catch
		{
		}
	}

	private void menuCancel_Click(object sender, EventArgs e)
	{
		canceled = true;
		Close();
	}

	private void menuSave_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void CheckEnable()
	{
		bool enabled = !checkBoxAdd.Checked;
		labelCode.Enabled = enabled;
		labelTrip.Enabled = enabled;
		numericCode.Enabled = enabled;
		numericTrip.Enabled = enabled;
	}

	private void checkBoxAdd_CheckStateChanged(object sender, EventArgs e)
	{
		CheckEnable();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PocketTopo.ToporobotForm));
		this.mainMenu = new System.Windows.Forms.MainMenu();
		this.menuSave = new System.Windows.Forms.MenuItem();
		this.menuCancel = new System.Windows.Forms.MenuItem();
		this.numericCode = new System.Windows.Forms.NumericUpDown();
		this.labelCode = new System.Windows.Forms.Label();
		this.labelTrip = new System.Windows.Forms.Label();
		this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
		this.numericTrip = new System.Windows.Forms.NumericUpDown();
		this.checkBoxAutoDecl = new System.Windows.Forms.CheckBox();
		this.checkBoxGuidelines = new System.Windows.Forms.CheckBox();
		this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
		this.checkBoxAdd = new System.Windows.Forms.CheckBox();
		this.checkBoxManDecl = new System.Windows.Forms.CheckBox();
		this.checkBoxVertical = new System.Windows.Forms.CheckBox();
		base.SuspendLayout();
		this.mainMenu.MenuItems.Add(this.menuSave);
		this.mainMenu.MenuItems.Add(this.menuCancel);
		resources.ApplyResources(this.menuSave, "menuSave");
		this.menuSave.Click += new System.EventHandler(menuSave_Click);
		resources.ApplyResources(this.menuCancel, "menuCancel");
		this.menuCancel.Click += new System.EventHandler(menuCancel_Click);
		resources.ApplyResources(this.numericCode, "numericCode");
		this.numericCode.Maximum = new decimal(new int[4] { 100000, 0, 0, 0 });
		this.numericCode.Minimum = new decimal(new int[4] { 1, 0, 0, 0 });
		this.numericCode.Name = "numericCode";
		this.numericCode.Value = new decimal(new int[4] { 1, 0, 0, 0 });
		resources.ApplyResources(this.labelCode, "labelCode");
		this.labelCode.Name = "labelCode";
		resources.ApplyResources(this.labelTrip, "labelTrip");
		this.labelTrip.Name = "labelTrip";
		resources.ApplyResources(this.saveFileDialog, "saveFileDialog");
		resources.ApplyResources(this.numericTrip, "numericTrip");
		this.numericTrip.Maximum = new decimal(new int[4] { 100000, 0, 0, 0 });
		this.numericTrip.Minimum = new decimal(new int[4] { 1, 0, 0, 0 });
		this.numericTrip.Name = "numericTrip";
		this.numericTrip.Value = new decimal(new int[4] { 1, 0, 0, 0 });
		resources.ApplyResources(this.checkBoxAutoDecl, "checkBoxAutoDecl");
		this.checkBoxAutoDecl.Name = "checkBoxAutoDecl";
		resources.ApplyResources(this.checkBoxGuidelines, "checkBoxGuidelines");
		this.checkBoxGuidelines.Checked = true;
		this.checkBoxGuidelines.CheckState = System.Windows.Forms.CheckState.Checked;
		this.checkBoxGuidelines.Name = "checkBoxGuidelines";
		resources.ApplyResources(this.openFileDialog, "openFileDialog");
		resources.ApplyResources(this.checkBoxAdd, "checkBoxAdd");
		this.checkBoxAdd.Name = "checkBoxAdd";
		this.checkBoxAdd.CheckStateChanged += new System.EventHandler(checkBoxAdd_CheckStateChanged);
		resources.ApplyResources(this.checkBoxManDecl, "checkBoxManDecl");
		this.checkBoxManDecl.Name = "checkBoxManDecl";
		resources.ApplyResources(this.checkBoxVertical, "checkBoxVertical");
		this.checkBoxVertical.Name = "checkBoxVertical";
		base.AutoScaleDimensions = new System.Drawing.SizeF(96f, 96f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
		resources.ApplyResources(this, "$this");
		base.Controls.Add(this.checkBoxVertical);
		base.Controls.Add(this.checkBoxManDecl);
		base.Controls.Add(this.checkBoxGuidelines);
		base.Controls.Add(this.checkBoxAdd);
		base.Controls.Add(this.checkBoxAutoDecl);
		base.Controls.Add(this.numericTrip);
		base.Controls.Add(this.numericCode);
		base.Controls.Add(this.labelTrip);
		base.Controls.Add(this.labelCode);
		base.Menu = this.mainMenu;
		base.Name = "ToporobotForm";
		base.Closing += new System.ComponentModel.CancelEventHandler(Form_Closing);
		base.ResumeLayout(false);
	}
}
