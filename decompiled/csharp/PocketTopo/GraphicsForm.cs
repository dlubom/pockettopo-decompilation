using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace PocketTopo;

internal class GraphicsForm : Form
{
	private MainForm main;

	private bool canceled = false;

	private int actualScale = 500;

	private IContainer components = null;

	private MainMenu mainMenu;

	private MenuItem menuSave;

	private MenuItem menuCancel;

	private NumericUpDown numericScale;

	private Label label1to;

	private Label labelWritePlan;

	private Label labelWriteSide;

	private CheckBox checkBoxPlan;

	private CheckBox checkBoxSide;

	private TextBox textSuffPlan;

	private TextBox textSuffSide;

	private Label labelSuff;

	private Label labelLayers;

	private CheckBox checkBoxShots;

	private Label labelShots;

	private CheckBox checkBoxXsection;

	private Label labelXsection;

	private CheckBox checkBoxColor;

	private Label labelColor;

	internal SaveFileDialog saveFileDialog;

	private CheckBox checkBoxLabels;

	private Label labelLabels;

	private Label labelInclude;

	private CheckBox checkBoxShotLayer;

	private CheckBox checkBoxXSLayer;

	private CheckBox checkBoxLabelLayer;

	private RadioButton radioButtonScale;

	private RadioButton radioButtonGrid;

	private CheckBox checkBoxAll;

	private Label labelAll;

	private CheckBox checkBoxGrid;

	private Label labelGrid;

	private CheckBox checkBoxGridLayer;

	internal GraphicsForm(MainForm main)
	{
		this.main = main;
		InitializeComponent();
	}

	private void Form_Closing(object sender, CancelEventArgs e)
	{
		if (!canceled)
		{
			saveFileDialog.FileName = Path.GetFileNameWithoutExtension(Config.actualFile);
			DialogResult dialogResult = saveFileDialog.ShowDialog();
			if (dialogResult == DialogResult.OK)
			{
				int scale = decimal.ToInt32(numericScale.Value);
				DXFWriter.Options options = DXFWriter.Options.NONE;
				if (radioButtonGrid.Checked)
				{
					options |= DXFWriter.Options.GEO;
				}
				if (checkBoxShots.Checked)
				{
					options |= DXFWriter.Options.ENS;
				}
				if (checkBoxShotLayer.Checked)
				{
					options |= DXFWriter.Options.SEPS;
				}
				if (checkBoxXsection.Checked)
				{
					options |= DXFWriter.Options.ENX;
				}
				if (checkBoxXSLayer.Checked)
				{
					options |= DXFWriter.Options.SEPX;
				}
				if (checkBoxLabels.Checked)
				{
					options |= DXFWriter.Options.ENL;
				}
				if (checkBoxLabelLayer.Checked)
				{
					options |= DXFWriter.Options.SEPL;
				}
				if (checkBoxGrid.Checked)
				{
					options |= DXFWriter.Options.ENG;
				}
				if (checkBoxGridLayer.Checked)
				{
					options |= DXFWriter.Options.SEPG;
				}
				if (checkBoxAll.Checked)
				{
					options |= DXFWriter.Options.ENA;
				}
				if (checkBoxColor.Checked)
				{
					options |= DXFWriter.Options.SEPC;
				}
				string text = Path.Combine(Path.GetDirectoryName(saveFileDialog.FileName), Path.GetFileNameWithoutExtension(saveFileDialog.FileName));
				string extension = Path.GetExtension(saveFileDialog.FileName);
				if (checkBoxPlan.Checked)
				{
					string text2 = text;
					if (checkBoxSide.Checked)
					{
						text2 += textSuffPlan.Text;
					}
					DXFWriter.WriteDXF(text2 + extension, main.dataSet.survey, main.dataSet.outline, scale, options);
				}
				if (checkBoxSide.Checked)
				{
					string text2 = text;
					if (checkBoxPlan.Checked)
					{
						text2 += textSuffSide.Text;
					}
					DXFWriter.WriteDXF(text2 + extension, main.dataSet.survey, main.dataSet.sideview, scale, options | DXFWriter.Options.SIDE);
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

	private void menuSave_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void menuCancel_Click(object sender, EventArgs e)
	{
		canceled = true;
		Close();
	}

	private void checkBox_CheckStateChanged(object sender, EventArgs e)
	{
		if (checkBoxPlan.Checked && checkBoxSide.Checked)
		{
			textSuffPlan.Enabled = true;
			textSuffSide.Enabled = true;
		}
		else
		{
			textSuffPlan.Enabled = false;
			textSuffSide.Enabled = false;
		}
		if (radioButtonGrid.Checked && !numericScale.ReadOnly)
		{
			actualScale = decimal.ToInt32(numericScale.Value);
			numericScale.Value = 1m;
			numericScale.ReadOnly = true;
		}
		if (radioButtonScale.Checked && numericScale.ReadOnly)
		{
			numericScale.Value = actualScale;
			numericScale.ReadOnly = false;
		}
		checkBoxShotLayer.Enabled = checkBoxShots.Checked;
		checkBoxXSLayer.Enabled = checkBoxXsection.Checked;
		checkBoxGridLayer.Enabled = checkBoxGrid.Checked;
		checkBoxLabelLayer.Enabled = checkBoxLabels.Checked;
	}

	private void GraphicsForm_Activated(object sender, EventArgs e)
	{
		canceled = false;
		try
		{
			Input.Enable();
		}
		catch
		{
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PocketTopo.GraphicsForm));
		this.mainMenu = new System.Windows.Forms.MainMenu();
		this.menuSave = new System.Windows.Forms.MenuItem();
		this.menuCancel = new System.Windows.Forms.MenuItem();
		this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
		this.numericScale = new System.Windows.Forms.NumericUpDown();
		this.label1to = new System.Windows.Forms.Label();
		this.labelWritePlan = new System.Windows.Forms.Label();
		this.labelWriteSide = new System.Windows.Forms.Label();
		this.checkBoxPlan = new System.Windows.Forms.CheckBox();
		this.checkBoxSide = new System.Windows.Forms.CheckBox();
		this.textSuffPlan = new System.Windows.Forms.TextBox();
		this.textSuffSide = new System.Windows.Forms.TextBox();
		this.labelSuff = new System.Windows.Forms.Label();
		this.labelLayers = new System.Windows.Forms.Label();
		this.checkBoxShots = new System.Windows.Forms.CheckBox();
		this.labelShots = new System.Windows.Forms.Label();
		this.checkBoxXsection = new System.Windows.Forms.CheckBox();
		this.labelXsection = new System.Windows.Forms.Label();
		this.checkBoxColor = new System.Windows.Forms.CheckBox();
		this.labelColor = new System.Windows.Forms.Label();
		this.checkBoxLabels = new System.Windows.Forms.CheckBox();
		this.labelLabels = new System.Windows.Forms.Label();
		this.labelInclude = new System.Windows.Forms.Label();
		this.checkBoxShotLayer = new System.Windows.Forms.CheckBox();
		this.checkBoxXSLayer = new System.Windows.Forms.CheckBox();
		this.checkBoxLabelLayer = new System.Windows.Forms.CheckBox();
		this.radioButtonScale = new System.Windows.Forms.RadioButton();
		this.radioButtonGrid = new System.Windows.Forms.RadioButton();
		this.checkBoxAll = new System.Windows.Forms.CheckBox();
		this.labelAll = new System.Windows.Forms.Label();
		this.checkBoxGrid = new System.Windows.Forms.CheckBox();
		this.labelGrid = new System.Windows.Forms.Label();
		this.checkBoxGridLayer = new System.Windows.Forms.CheckBox();
		base.SuspendLayout();
		this.mainMenu.MenuItems.Add(this.menuSave);
		this.mainMenu.MenuItems.Add(this.menuCancel);
		resources.ApplyResources(this.menuSave, "menuSave");
		this.menuSave.Click += new System.EventHandler(menuSave_Click);
		resources.ApplyResources(this.menuCancel, "menuCancel");
		this.menuCancel.Click += new System.EventHandler(menuCancel_Click);
		resources.ApplyResources(this.saveFileDialog, "saveFileDialog");
		resources.ApplyResources(this.numericScale, "numericScale");
		this.numericScale.Maximum = new decimal(new int[4] { 100000, 0, 0, 0 });
		this.numericScale.Minimum = new decimal(new int[4] { 1, 0, 0, 0 });
		this.numericScale.Name = "numericScale";
		this.numericScale.Value = new decimal(new int[4] { 500, 0, 0, 0 });
		resources.ApplyResources(this.label1to, "label1to");
		this.label1to.Name = "label1to";
		resources.ApplyResources(this.labelWritePlan, "labelWritePlan");
		this.labelWritePlan.Name = "labelWritePlan";
		resources.ApplyResources(this.labelWriteSide, "labelWriteSide");
		this.labelWriteSide.Name = "labelWriteSide";
		this.checkBoxPlan.Checked = true;
		this.checkBoxPlan.CheckState = System.Windows.Forms.CheckState.Checked;
		resources.ApplyResources(this.checkBoxPlan, "checkBoxPlan");
		this.checkBoxPlan.Name = "checkBoxPlan";
		this.checkBoxPlan.CheckStateChanged += new System.EventHandler(checkBox_CheckStateChanged);
		this.checkBoxSide.Checked = true;
		this.checkBoxSide.CheckState = System.Windows.Forms.CheckState.Checked;
		resources.ApplyResources(this.checkBoxSide, "checkBoxSide");
		this.checkBoxSide.Name = "checkBoxSide";
		this.checkBoxSide.CheckStateChanged += new System.EventHandler(checkBox_CheckStateChanged);
		resources.ApplyResources(this.textSuffPlan, "textSuffPlan");
		this.textSuffPlan.Name = "textSuffPlan";
		resources.ApplyResources(this.textSuffSide, "textSuffSide");
		this.textSuffSide.Name = "textSuffSide";
		resources.ApplyResources(this.labelSuff, "labelSuff");
		this.labelSuff.Name = "labelSuff";
		resources.ApplyResources(this.labelLayers, "labelLayers");
		this.labelLayers.Name = "labelLayers";
		this.checkBoxShots.Checked = true;
		this.checkBoxShots.CheckState = System.Windows.Forms.CheckState.Checked;
		resources.ApplyResources(this.checkBoxShots, "checkBoxShots");
		this.checkBoxShots.Name = "checkBoxShots";
		this.checkBoxShots.CheckStateChanged += new System.EventHandler(checkBox_CheckStateChanged);
		resources.ApplyResources(this.labelShots, "labelShots");
		this.labelShots.Name = "labelShots";
		this.checkBoxXsection.Checked = true;
		this.checkBoxXsection.CheckState = System.Windows.Forms.CheckState.Checked;
		resources.ApplyResources(this.checkBoxXsection, "checkBoxXsection");
		this.checkBoxXsection.Name = "checkBoxXsection";
		this.checkBoxXsection.CheckStateChanged += new System.EventHandler(checkBox_CheckStateChanged);
		resources.ApplyResources(this.labelXsection, "labelXsection");
		this.labelXsection.Name = "labelXsection";
		resources.ApplyResources(this.checkBoxColor, "checkBoxColor");
		this.checkBoxColor.Name = "checkBoxColor";
		this.checkBoxColor.CheckStateChanged += new System.EventHandler(checkBox_CheckStateChanged);
		resources.ApplyResources(this.labelColor, "labelColor");
		this.labelColor.Name = "labelColor";
		resources.ApplyResources(this.checkBoxLabels, "checkBoxLabels");
		this.checkBoxLabels.Name = "checkBoxLabels";
		this.checkBoxLabels.CheckStateChanged += new System.EventHandler(checkBox_CheckStateChanged);
		resources.ApplyResources(this.labelLabels, "labelLabels");
		this.labelLabels.Name = "labelLabels";
		resources.ApplyResources(this.labelInclude, "labelInclude");
		this.labelInclude.Name = "labelInclude";
		this.checkBoxShotLayer.Checked = true;
		this.checkBoxShotLayer.CheckState = System.Windows.Forms.CheckState.Checked;
		resources.ApplyResources(this.checkBoxShotLayer, "checkBoxShotLayer");
		this.checkBoxShotLayer.Name = "checkBoxShotLayer";
		this.checkBoxShotLayer.CheckStateChanged += new System.EventHandler(checkBox_CheckStateChanged);
		this.checkBoxXSLayer.Checked = true;
		this.checkBoxXSLayer.CheckState = System.Windows.Forms.CheckState.Checked;
		resources.ApplyResources(this.checkBoxXSLayer, "checkBoxXSLayer");
		this.checkBoxXSLayer.Name = "checkBoxXSLayer";
		this.checkBoxXSLayer.CheckStateChanged += new System.EventHandler(checkBox_CheckStateChanged);
		this.checkBoxLabelLayer.Checked = true;
		this.checkBoxLabelLayer.CheckState = System.Windows.Forms.CheckState.Checked;
		resources.ApplyResources(this.checkBoxLabelLayer, "checkBoxLabelLayer");
		this.checkBoxLabelLayer.Name = "checkBoxLabelLayer";
		this.checkBoxLabelLayer.CheckStateChanged += new System.EventHandler(checkBox_CheckStateChanged);
		this.radioButtonScale.Checked = true;
		resources.ApplyResources(this.radioButtonScale, "radioButtonScale");
		this.radioButtonScale.Name = "radioButtonScale";
		this.radioButtonScale.CheckedChanged += new System.EventHandler(checkBox_CheckStateChanged);
		resources.ApplyResources(this.radioButtonGrid, "radioButtonGrid");
		this.radioButtonGrid.Name = "radioButtonGrid";
		((System.Windows.Forms.Control)this.radioButtonGrid).TabStop = false;
		this.radioButtonGrid.CheckedChanged += new System.EventHandler(checkBox_CheckStateChanged);
		resources.ApplyResources(this.checkBoxAll, "checkBoxAll");
		this.checkBoxAll.Name = "checkBoxAll";
		this.checkBoxAll.CheckStateChanged += new System.EventHandler(checkBox_CheckStateChanged);
		resources.ApplyResources(this.labelAll, "labelAll");
		this.labelAll.Name = "labelAll";
		resources.ApplyResources(this.checkBoxGrid, "checkBoxGrid");
		this.checkBoxGrid.Name = "checkBoxGrid";
		this.checkBoxGrid.CheckStateChanged += new System.EventHandler(checkBox_CheckStateChanged);
		resources.ApplyResources(this.labelGrid, "labelGrid");
		this.labelGrid.Name = "labelGrid";
		this.checkBoxGridLayer.Checked = true;
		this.checkBoxGridLayer.CheckState = System.Windows.Forms.CheckState.Checked;
		resources.ApplyResources(this.checkBoxGridLayer, "checkBoxGridLayer");
		this.checkBoxGridLayer.Name = "checkBoxGridLayer";
		this.checkBoxGridLayer.CheckStateChanged += new System.EventHandler(checkBox_CheckStateChanged);
		base.AutoScaleDimensions = new System.Drawing.SizeF(96f, 96f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
		resources.ApplyResources(this, "$this");
		base.Controls.Add(this.numericScale);
		base.Controls.Add(this.labelWriteSide);
		base.Controls.Add(this.checkBoxGridLayer);
		base.Controls.Add(this.checkBoxLabelLayer);
		base.Controls.Add(this.checkBoxXSLayer);
		base.Controls.Add(this.checkBoxShotLayer);
		base.Controls.Add(this.labelInclude);
		base.Controls.Add(this.labelGrid);
		base.Controls.Add(this.checkBoxGrid);
		base.Controls.Add(this.labelAll);
		base.Controls.Add(this.checkBoxAll);
		base.Controls.Add(this.labelLabels);
		base.Controls.Add(this.checkBoxLabels);
		base.Controls.Add(this.checkBoxShots);
		base.Controls.Add(this.labelColor);
		base.Controls.Add(this.labelXsection);
		base.Controls.Add(this.labelShots);
		base.Controls.Add(this.labelLayers);
		base.Controls.Add(this.labelSuff);
		base.Controls.Add(this.textSuffSide);
		base.Controls.Add(this.checkBoxColor);
		base.Controls.Add(this.textSuffPlan);
		base.Controls.Add(this.checkBoxXsection);
		base.Controls.Add(this.checkBoxSide);
		base.Controls.Add(this.checkBoxPlan);
		base.Controls.Add(this.labelWritePlan);
		base.Controls.Add(this.label1to);
		base.Controls.Add(this.radioButtonScale);
		base.Controls.Add(this.radioButtonGrid);
		base.Menu = this.mainMenu;
		base.Name = "GraphicsForm";
		base.Activated += new System.EventHandler(GraphicsForm_Activated);
		base.Closing += new System.ComponentModel.CancelEventHandler(Form_Closing);
		base.ResumeLayout(false);
	}
}
