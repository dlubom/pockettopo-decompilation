using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PocketTopo;

public class PortForm : Form
{
	private bool canceled;

	private IContainer components = null;

	private MainMenu mainMenu1;

	private TextBox textBox;

	private MenuItem menuCancel;

	private Label label;

	public PortForm()
	{
		InitializeComponent();
		textBox.Text = ((Config.portName == "") ? "COM1" : Config.portName);
		try
		{
			Input.Enable();
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

	private void PortForm_Closing(object sender, CancelEventArgs e)
	{
		if (!canceled)
		{
			Config.portName = textBox.Text;
		}
		try
		{
			Input.Disable();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PocketTopo.PortForm));
		this.mainMenu1 = new System.Windows.Forms.MainMenu();
		this.menuCancel = new System.Windows.Forms.MenuItem();
		this.textBox = new System.Windows.Forms.TextBox();
		this.label = new System.Windows.Forms.Label();
		base.SuspendLayout();
		this.mainMenu1.MenuItems.Add(this.menuCancel);
		resources.ApplyResources(this.menuCancel, "menuCancel");
		this.menuCancel.Click += new System.EventHandler(menuCancel_Click);
		resources.ApplyResources(this.textBox, "textBox");
		this.textBox.Name = "textBox";
		resources.ApplyResources(this.label, "label");
		this.label.Name = "label";
		base.AutoScaleDimensions = new System.Drawing.SizeF(96f, 96f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
		resources.ApplyResources(this, "$this");
		base.Controls.Add(this.label);
		base.Controls.Add(this.textBox);
		base.Menu = this.mainMenu1;
		base.Name = "PortForm";
		base.Closing += new System.ComponentModel.CancelEventHandler(PortForm_Closing);
		base.ResumeLayout(false);
	}
}
