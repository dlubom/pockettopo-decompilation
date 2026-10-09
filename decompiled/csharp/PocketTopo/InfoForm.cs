using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PocketTopo;

internal class InfoForm : Form
{
	private IContainer components = null;

	private MainMenu mainMenu;

	private Label label1;

	private Label label2;

	private PictureBox pictureBox;

	private Label label3;

	private Label label4;

	private Label label5;

	private Label label6;

	private Label label7;

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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PocketTopo.InfoForm));
		this.mainMenu = new System.Windows.Forms.MainMenu();
		this.label5 = new System.Windows.Forms.Label();
		this.label4 = new System.Windows.Forms.Label();
		this.label3 = new System.Windows.Forms.Label();
		this.pictureBox = new System.Windows.Forms.PictureBox();
		this.label2 = new System.Windows.Forms.Label();
		this.label1 = new System.Windows.Forms.Label();
		this.label6 = new System.Windows.Forms.Label();
		this.label7 = new System.Windows.Forms.Label();
		base.SuspendLayout();
		resources.ApplyResources(this.label5, "label5");
		this.label5.Name = "label5";
		resources.ApplyResources(this.label4, "label4");
		this.label4.Name = "label4";
		resources.ApplyResources(this.label3, "label3");
		this.label3.Name = "label3";
		resources.ApplyResources(this.pictureBox, "pictureBox");
		this.pictureBox.Name = "pictureBox";
		resources.ApplyResources(this.label2, "label2");
		this.label2.Name = "label2";
		resources.ApplyResources(this.label1, "label1");
		this.label1.Name = "label1";
		resources.ApplyResources(this.label6, "label6");
		this.label6.Name = "label6";
		resources.ApplyResources(this.label7, "label7");
		this.label7.Name = "label7";
		base.AutoScaleDimensions = new System.Drawing.SizeF(96f, 96f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
		resources.ApplyResources(this, "$this");
		base.Controls.Add(this.label7);
		base.Controls.Add(this.label6);
		base.Controls.Add(this.label5);
		base.Controls.Add(this.label4);
		base.Controls.Add(this.label3);
		base.Controls.Add(this.pictureBox);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.label1);
		base.Menu = this.mainMenu;
		base.Name = "InfoForm";
		base.ResumeLayout(false);
	}

	internal InfoForm()
	{
		InitializeComponent();
	}
}
