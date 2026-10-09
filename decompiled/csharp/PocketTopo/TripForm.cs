using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PocketTopo;

internal class TripForm : Form
{
	private Trip trip;

	private Survey survey;

	private bool canceled;

	private Angle decl;

	private bool auto;

	private IContainer components = null;

	private MainMenu mainMenu;

	private MenuItem menuNew;

	private TextBox textDecl;

	private TextBox textNextId;

	private Label labelDecl;

	private Label labelNextId;

	private DateTimePicker dateTimePicker;

	private Label labelDate;

	private TextBox textComment;

	private MenuItem menuCancel;

	private CheckBox checkBoxAuto;

	internal TripForm(Trip trip, Survey survey, bool writable)
	{
		InitializeComponent();
		this.trip = trip;
		this.survey = survey;
		dateTimePicker.Enabled = writable;
		textDecl.ReadOnly = !writable;
		textComment.ReadOnly = !writable;
		checkBoxAuto.Enabled = false;
		canceled = !writable;
		DateTime dateTime;
		if (trip == null)
		{
			dateTime = Config.tripDate;
			if ((int)Config.declCorr == -32768)
			{
				decl = 0;
				auto = true;
			}
			else
			{
				decl = Config.declCorr;
				auto = false;
			}
			if (auto)
			{
				decl = 0;
			}
			textComment.Text = Config.tripComment;
			textNextId.Text = Config.nextId.ToString();
			textNextId.Visible = true;
			labelNextId.Visible = true;
			menuNew.Enabled = true;
		}
		else
		{
			dateTime = trip.Date;
			decl = trip.DeclCorr;
			auto = trip.AutoDecl;
			textComment.Text = trip.Comment;
			textNextId.Visible = false;
			labelNextId.Visible = false;
			menuNew.Enabled = false;
		}
		dateTimePicker.Value = dateTime;
		if (auto && survey.refBase == null)
		{
			survey.Update();
		}
		textDecl.Text = Trip.GetAng(Trip.GetDeclination(auto, decl, dateTime, survey.refBase));
		if (auto)
		{
			checkBoxAuto.Checked = true;
			textDecl.ReadOnly = true;
		}
		else
		{
			checkBoxAuto.Checked = false;
		}
		if (writable)
		{
			try
			{
				Input.Enable();
			}
			catch
			{
			}
		}
	}

	private void Form_Closing(object sender, CancelEventArgs e)
	{
		if (!canceled)
		{
			DateTime value = dateTimePicker.Value;
			Angle val = 0;
			auto = checkBoxAuto.Checked;
			Trip.SetAng(ref val, textDecl.Text);
			string text = textComment.Text;
			if (trip == null)
			{
				ID nextId = Config.nextId;
				nextId = new ID(textNextId.Text);
				Config.tripDate = value;
				Config.declCorr = (auto ? ((Angle)32768) : val);
				Config.tripComment = text;
				Config.nextId = nextId;
			}
			else
			{
				survey.ChangeTrip(trip, value, auto, val, text);
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

	private void menuNew_Click(object sender, EventArgs e)
	{
		dateTimePicker.Value = DateTime.Today;
		textComment.Text = "";
	}

	private void CheckDecl(object sender, EventArgs e)
	{
		if (!checkBoxAuto.Enabled)
		{
			return;
		}
		if (checkBoxAuto.Checked)
		{
			textDecl.ReadOnly = true;
			if (survey.refBase == null)
			{
				survey.Update();
			}
			textDecl.Text = Trip.GetAng(Trip.GetDeclination(auto: true, decl, dateTimePicker.Value, survey.refBase));
		}
		else
		{
			textDecl.ReadOnly = false;
			textDecl.Text = Trip.GetAng(decl);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PocketTopo.TripForm));
		this.mainMenu = new System.Windows.Forms.MainMenu();
		this.menuCancel = new System.Windows.Forms.MenuItem();
		this.menuNew = new System.Windows.Forms.MenuItem();
		this.textDecl = new System.Windows.Forms.TextBox();
		this.textNextId = new System.Windows.Forms.TextBox();
		this.labelDecl = new System.Windows.Forms.Label();
		this.labelNextId = new System.Windows.Forms.Label();
		this.dateTimePicker = new System.Windows.Forms.DateTimePicker();
		this.labelDate = new System.Windows.Forms.Label();
		this.textComment = new System.Windows.Forms.TextBox();
		this.checkBoxAuto = new System.Windows.Forms.CheckBox();
		base.SuspendLayout();
		this.mainMenu.MenuItems.Add(this.menuCancel);
		this.mainMenu.MenuItems.Add(this.menuNew);
		resources.ApplyResources(this.menuCancel, "menuCancel");
		this.menuCancel.Click += new System.EventHandler(menuCancel_Click);
		resources.ApplyResources(this.menuNew, "menuNew");
		this.menuNew.Click += new System.EventHandler(menuNew_Click);
		resources.ApplyResources(this.textDecl, "textDecl");
		this.textDecl.Name = "textDecl";
		resources.ApplyResources(this.textNextId, "textNextId");
		this.textNextId.Name = "textNextId";
		resources.ApplyResources(this.labelDecl, "labelDecl");
		this.labelDecl.Name = "labelDecl";
		resources.ApplyResources(this.labelNextId, "labelNextId");
		this.labelNextId.Name = "labelNextId";
		resources.ApplyResources(this.dateTimePicker, "dateTimePicker");
		this.dateTimePicker.CustomFormat = null;
		this.dateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
		this.dateTimePicker.Name = "dateTimePicker";
		this.dateTimePicker.Value = new System.DateTime(2008, 1, 1, 0, 0, 0, 0);
		this.dateTimePicker.ValueChanged += new System.EventHandler(CheckDecl);
		resources.ApplyResources(this.labelDate, "labelDate");
		this.labelDate.Name = "labelDate";
		resources.ApplyResources(this.textComment, "textComment");
		this.textComment.Name = "textComment";
		resources.ApplyResources(this.checkBoxAuto, "checkBoxAuto");
		this.checkBoxAuto.Name = "checkBoxAuto";
		this.checkBoxAuto.CheckStateChanged += new System.EventHandler(CheckDecl);
		base.AutoScaleDimensions = new System.Drawing.SizeF(96f, 96f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
		resources.ApplyResources(this, "$this");
		base.Controls.Add(this.checkBoxAuto);
		base.Controls.Add(this.dateTimePicker);
		base.Controls.Add(this.textNextId);
		base.Controls.Add(this.textDecl);
		base.Controls.Add(this.labelDate);
		base.Controls.Add(this.labelDecl);
		base.Controls.Add(this.labelNextId);
		base.Controls.Add(this.textComment);
		base.Menu = this.mainMenu;
		base.Name = "TripForm";
		base.Closing += new System.ComponentModel.CancelEventHandler(Form_Closing);
		base.ResumeLayout(false);
	}
}
