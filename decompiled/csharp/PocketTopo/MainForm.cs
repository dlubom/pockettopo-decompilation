using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace PocketTopo;

internal class MainForm : Form
{
	private const int H = 12;

	private IContainer components = null;

	private MainMenu mainMenu;

	private MenuItem menuMenu;

	private MenuItem menuOpen;

	private MenuItem menuSave;

	private ToolBar toolBar;

	private ImageList imageList;

	private ToolBarButton toolBarData;

	private ToolBarButton toolBarSketch;

	private ToolBarButton toolBarSep;

	private ToolBarButton toolBarPlus;

	private ToolBarButton toolBarMinus;

	private ToolBarButton toolBarUndo;

	private ToolBarButton toolBarRedo;

	private MenuItem menuTitle;

	private MenuItem menuSep2;

	private Panel panelPaint;

	private Panel panelMap;

	private Panel panelData;

	private ContextMenu dataContextMenu;

	private MenuItem menuComment;

	private BindingSource database;

	private DataGrid dataGrid;

	private MenuItem menuUndo;

	private MenuItem menuExit;

	private DataGridTableStyle dataGridTableStyle1;

	private DataGridTextBoxColumn dataGridTextBoxColumn1;

	private DataGridTextBoxColumn dataGridTextBoxColumn2;

	private DataGridTextBoxColumn dataGridTextBoxColumn3;

	private DataGridTextBoxColumn dataGridTextBoxColumn4;

	private DataGridTextBoxColumn dataGridTextBoxColumn5;

	private TextBox textData;

	private MenuItem menuPort;

	private MenuItem menuCom1;

	private MenuItem menuCom2;

	private MenuItem menuCom3;

	private MenuItem menuCom4;

	private MenuItem menuCom5;

	private MenuItem menuCom6;

	private MenuItem menuCom7;

	private MenuItem menuCom8;

	private MenuItem menuRedo;

	private MenuItem menuD2M;

	private MenuItem menuD2O;

	private MenuItem menuUnit;

	private MenuItem menuDeg;

	private MenuItem menuGrad;

	private SketchView sketchView;

	private MapView mapView;

	private Timer timer;

	private OpenFileDialog openFileDialog;

	private SaveFileDialog saveFileDialog;

	private MenuItem menuNone;

	private MenuItem menuSep4;

	private MenuItem menuInsert;

	private MenuItem menuDelete;

	private MenuItem menuSep5;

	private PictureBox moveLD;

	private PictureBox moveRD;

	private PictureBox moveLU;

	private PictureBox moveRU;

	private ContextMenu mapContextMenu;

	private MenuItem menuM2D;

	private MenuItem menuM2O;

	private MenuItem menuSep1;

	private MenuItem menuNewCave;

	private DataGridTextBoxColumn dataGridTextBoxColumn6;

	private TextBox textComm;

	private MenuItem menuBluetooth;

	private MenuItem menuConnect;

	private MenuItem menuDisconn;

	private MenuItem menuAuto;

	private MenuItem menuSep6;

	private MenuItem menuO2D;

	private MenuItem menuO2M;

	private MenuItem menuStart;

	private MenuItem menuExport;

	private MenuItem menuExpText;

	private MenuItem menuExpDXF;

	private SaveFileDialog txtFileDialog;

	private PictureBox pictBlack;

	private PictureBox pictMove;

	private PictureBox pictRed;

	private PictureBox pictBlue;

	private PictureBox pictBrown;

	private PictureBox pictDelete;

	private PictureBox pictSelDelete;

	private PictureBox pictSelRed;

	private PictureBox pictSelBlue;

	private PictureBox pictSelBrown;

	private PictureBox pictSelGray;

	private PictureBox pictSelBlack;

	private PictureBox pictSelMove;

	private MenuItem menuCalib;

	internal Label saveChanges;

	private MenuItem menuO2S;

	private MenuItem menuM2S;

	private ContextMenu sideContextMenu;

	private MenuItem menuS2D;

	private MenuItem menuS2M;

	private MenuItem menuS2O;

	private MenuItem menuFlip;

	private MenuItem menuFlipAll;

	private MenuItem menuSep7;

	private MenuItem menuSep3;

	private MenuItem menuCopy;

	private MenuItem menuPaste;

	private MenuItem menuD2S;

	private MenuItem menuSep10;

	private MenuItem menuSep11;

	private MenuItem menuRasterS;

	private MenuItem menuNew;

	private MenuItem menuSaveAs;

	private MenuItem menuFile;

	private MenuItem menuTrip;

	private MenuItem menuSTrip;

	private MenuItem menuSep8;

	private ToolBarButton toolBarMap;

	private ContextMenu planContextMenu;

	private DataGrid dataGridRef;

	private DataGridTableStyle dataGridTableStyleRef;

	private DataGridTextBoxColumn dataGridTextBoxColumnR1;

	private BindingSource references;

	private DataGridTextBoxColumn dataGridTextBoxColumnR2;

	private DataGridTextBoxColumn dataGridTextBoxColumnR3;

	private DataGridTextBoxColumn dataGridTextBoxColumnR4;

	private DataGridTextBoxColumn dataGridTextBoxColumnR5;

	private MenuItem menuImport;

	private MenuItem menuToBranch;

	private MenuItem menuRenum;

	private ContextMenu refContextMenu;

	private MenuItem menuRComm;

	private MenuItem menuSep12;

	private MenuItem menuRInsert;

	private MenuItem menuRDelete;

	private MenuItem menuSep14;

	private MenuItem menuRStart;

	private MenuItem menuSep9;

	private MenuItem menuRCopy;

	private MenuItem menuRPaste;

	private MenuItem menuSep13;

	private MenuItem menuR2M;

	private MenuItem menuR2O;

	private MenuItem menuR2S;

	private MenuItem menuSep16;

	private MenuItem menuCom9;

	private MenuItem menuCom0;

	private MenuItem menuSep15;

	private MenuItem menuXSect;

	private MenuItem menuSep17;

	private MenuItem menuXSectV;

	private MenuItem menuXSectH;

	private MenuItem menuSep18;

	private MenuItem menuMeter;

	private MenuItem menuFeet;

	private MenuItem menuCont;

	private MenuItem menuSep19;

	private PictureBox pictSelGreen;

	private PictureBox pictGreen;

	private PictureBox pictGray;

	private MenuItem menuSep20;

	private MenuItem menuInfo;

	private MenuItem menuThick;

	private MenuItem menuThickP;

	private MenuItem menuThickS;

	private MenuItem menuExp3D;

	private MenuItem menuComX;

	internal Label fileError;

	private MenuItem menuSmart;

	private MenuItem menuReverse;

	private MenuItem menuOptP;

	private MenuItem menuRaster;

	private MenuItem menuAll;

	private MenuItem menuIdsP;

	private MenuItem menuOptS;

	private MenuItem menuIdsS;

	private MenuItem menuOptM;

	private MenuItem menuIds;

	private PictureBox pictSelOrange;

	private PictureBox pictOrange;

	private MenuItem menuDumpM;

	private MenuItem menuSep21;

	private MenuItem menuDumpP;

	private MenuItem menuSep22;

	private MenuItem menuDumpS;

	private MenuItem menuSep23;

	private MenuItem menuLoops;

	private MenuItem menuCloseAll;

	private MenuItem menuCloseAct;

	private MenuItem menuEnd;

	private MenuItem menuCloseNone;

	private MenuItem menuItem1;

	private MenuItem menuLOn;

	private MenuItem menuLOff;

	private MenuItem menuMeasure;

	internal static Pen black;

	internal static Pen gray;

	internal static Pen brown;

	internal static Pen blue;

	internal static Pen green;

	internal static Pen red;

	internal static Pen orange;

	internal static Pen lgray;

	internal static Pen shotRd;

	internal static Pen shotOr;

	internal static Pen shotDa;

	internal static Pen shotXs;

	internal static Pen shotXOr;

	internal static Pen dashed;

	internal static Pen selected;

	internal static Pen selectedDa;

	internal static Brush blackBrush;

	internal static Brush grayBrush;

	internal static Font textFont;

	internal static Font labelFont;

	internal static bool pcLayout;

	private DataGrid activeGrid;

	private int numColumns;

	private DataGridCell activeCell;

	private bool cellClicked;

	private Mapping mapMap;

	private Mapping outlineMap;

	private Mapping sideviewMap;

	private Mapping actualMap;

	private Drawing outline;

	private Drawing sideview;

	private Drawing actualList;

	private Drawing dummy;

	private Survey survey;

	private Undo undo;

	private CalibForm calibForm;

	private GraphicsForm dxfForm;

	private Form infoForm;

	internal DataSet dataSet;

	private Point contextPos;

	private Station contextStation;

	private string clipboard;

	private Extension extensions;

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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PocketTopo.MainForm));
		this.menuTitle = new System.Windows.Forms.MenuItem();
		this.menuSmart = new System.Windows.Forms.MenuItem();
		this.menuReverse = new System.Windows.Forms.MenuItem();
		this.menuUnit = new System.Windows.Forms.MenuItem();
		this.menuDeg = new System.Windows.Forms.MenuItem();
		this.menuGrad = new System.Windows.Forms.MenuItem();
		this.menuSep18 = new System.Windows.Forms.MenuItem();
		this.menuMeter = new System.Windows.Forms.MenuItem();
		this.menuFeet = new System.Windows.Forms.MenuItem();
		this.menuPort = new System.Windows.Forms.MenuItem();
		this.menuNone = new System.Windows.Forms.MenuItem();
		this.menuCom0 = new System.Windows.Forms.MenuItem();
		this.menuCom1 = new System.Windows.Forms.MenuItem();
		this.menuCom2 = new System.Windows.Forms.MenuItem();
		this.menuCom3 = new System.Windows.Forms.MenuItem();
		this.menuCom4 = new System.Windows.Forms.MenuItem();
		this.menuCom5 = new System.Windows.Forms.MenuItem();
		this.menuCom6 = new System.Windows.Forms.MenuItem();
		this.menuCom7 = new System.Windows.Forms.MenuItem();
		this.menuCom8 = new System.Windows.Forms.MenuItem();
		this.menuCom9 = new System.Windows.Forms.MenuItem();
		this.menuComX = new System.Windows.Forms.MenuItem();
		this.menuSep20 = new System.Windows.Forms.MenuItem();
		this.menuInfo = new System.Windows.Forms.MenuItem();
		this.mainMenu = new System.Windows.Forms.MainMenu();
		this.menuMenu = new System.Windows.Forms.MenuItem();
		this.menuBluetooth = new System.Windows.Forms.MenuItem();
		this.menuConnect = new System.Windows.Forms.MenuItem();
		this.menuDisconn = new System.Windows.Forms.MenuItem();
		this.menuAuto = new System.Windows.Forms.MenuItem();
		this.menuSep16 = new System.Windows.Forms.MenuItem();
		this.menuTrip = new System.Windows.Forms.MenuItem();
		this.menuCalib = new System.Windows.Forms.MenuItem();
		this.menuSep2 = new System.Windows.Forms.MenuItem();
		this.menuFile = new System.Windows.Forms.MenuItem();
		this.menuNewCave = new System.Windows.Forms.MenuItem();
		this.menuNew = new System.Windows.Forms.MenuItem();
		this.menuOpen = new System.Windows.Forms.MenuItem();
		this.menuSave = new System.Windows.Forms.MenuItem();
		this.menuSaveAs = new System.Windows.Forms.MenuItem();
		this.menuImport = new System.Windows.Forms.MenuItem();
		this.menuExport = new System.Windows.Forms.MenuItem();
		this.menuExpDXF = new System.Windows.Forms.MenuItem();
		this.menuExp3D = new System.Windows.Forms.MenuItem();
		this.menuExpText = new System.Windows.Forms.MenuItem();
		this.menuLoops = new System.Windows.Forms.MenuItem();
		this.menuCloseAll = new System.Windows.Forms.MenuItem();
		this.menuCloseAct = new System.Windows.Forms.MenuItem();
		this.menuCloseNone = new System.Windows.Forms.MenuItem();
		this.menuSep6 = new System.Windows.Forms.MenuItem();
		this.menuUndo = new System.Windows.Forms.MenuItem();
		this.menuRedo = new System.Windows.Forms.MenuItem();
		this.menuSep1 = new System.Windows.Forms.MenuItem();
		this.menuExit = new System.Windows.Forms.MenuItem();
		this.toolBar = new System.Windows.Forms.ToolBar();
		this.toolBarData = new System.Windows.Forms.ToolBarButton();
		this.toolBarMap = new System.Windows.Forms.ToolBarButton();
		this.toolBarSketch = new System.Windows.Forms.ToolBarButton();
		this.toolBarSep = new System.Windows.Forms.ToolBarButton();
		this.toolBarPlus = new System.Windows.Forms.ToolBarButton();
		this.toolBarMinus = new System.Windows.Forms.ToolBarButton();
		this.toolBarUndo = new System.Windows.Forms.ToolBarButton();
		this.toolBarRedo = new System.Windows.Forms.ToolBarButton();
		this.imageList = new System.Windows.Forms.ImageList();
		this.dataContextMenu = new System.Windows.Forms.ContextMenu();
		this.menuComment = new System.Windows.Forms.MenuItem();
		this.menuSTrip = new System.Windows.Forms.MenuItem();
		this.menuSep4 = new System.Windows.Forms.MenuItem();
		this.menuInsert = new System.Windows.Forms.MenuItem();
		this.menuDelete = new System.Windows.Forms.MenuItem();
		this.menuSep8 = new System.Windows.Forms.MenuItem();
		this.menuToBranch = new System.Windows.Forms.MenuItem();
		this.menuRenum = new System.Windows.Forms.MenuItem();
		this.menuStart = new System.Windows.Forms.MenuItem();
		this.menuCont = new System.Windows.Forms.MenuItem();
		this.menuEnd = new System.Windows.Forms.MenuItem();
		this.menuSep3 = new System.Windows.Forms.MenuItem();
		this.menuCopy = new System.Windows.Forms.MenuItem();
		this.menuPaste = new System.Windows.Forms.MenuItem();
		this.menuSep5 = new System.Windows.Forms.MenuItem();
		this.menuD2M = new System.Windows.Forms.MenuItem();
		this.menuD2O = new System.Windows.Forms.MenuItem();
		this.menuD2S = new System.Windows.Forms.MenuItem();
		this.panelPaint = new System.Windows.Forms.Panel();
		this.pictSelDelete = new System.Windows.Forms.PictureBox();
		this.pictSelGreen = new System.Windows.Forms.PictureBox();
		this.pictSelOrange = new System.Windows.Forms.PictureBox();
		this.pictSelRed = new System.Windows.Forms.PictureBox();
		this.pictSelBlue = new System.Windows.Forms.PictureBox();
		this.pictSelBrown = new System.Windows.Forms.PictureBox();
		this.pictSelGray = new System.Windows.Forms.PictureBox();
		this.pictSelBlack = new System.Windows.Forms.PictureBox();
		this.pictSelMove = new System.Windows.Forms.PictureBox();
		this.pictDelete = new System.Windows.Forms.PictureBox();
		this.pictGreen = new System.Windows.Forms.PictureBox();
		this.pictOrange = new System.Windows.Forms.PictureBox();
		this.pictRed = new System.Windows.Forms.PictureBox();
		this.pictBlue = new System.Windows.Forms.PictureBox();
		this.pictBrown = new System.Windows.Forms.PictureBox();
		this.pictGray = new System.Windows.Forms.PictureBox();
		this.pictBlack = new System.Windows.Forms.PictureBox();
		this.pictMove = new System.Windows.Forms.PictureBox();
		this.moveRD = new System.Windows.Forms.PictureBox();
		this.moveLD = new System.Windows.Forms.PictureBox();
		this.moveRU = new System.Windows.Forms.PictureBox();
		this.moveLU = new System.Windows.Forms.PictureBox();
		this.sketchView = new PocketTopo.SketchView();
		this.planContextMenu = new System.Windows.Forms.ContextMenu();
		this.menuDumpP = new System.Windows.Forms.MenuItem();
		this.menuSep22 = new System.Windows.Forms.MenuItem();
		this.menuOptP = new System.Windows.Forms.MenuItem();
		this.menuRaster = new System.Windows.Forms.MenuItem();
		this.menuAll = new System.Windows.Forms.MenuItem();
		this.menuIdsP = new System.Windows.Forms.MenuItem();
		this.menuThickP = new System.Windows.Forms.MenuItem();
		this.menuSep15 = new System.Windows.Forms.MenuItem();
		this.menuXSect = new System.Windows.Forms.MenuItem();
		this.menuSep10 = new System.Windows.Forms.MenuItem();
		this.menuO2D = new System.Windows.Forms.MenuItem();
		this.menuO2M = new System.Windows.Forms.MenuItem();
		this.menuO2S = new System.Windows.Forms.MenuItem();
		this.fileError = new System.Windows.Forms.Label();
		this.panelMap = new System.Windows.Forms.Panel();
		this.mapView = new PocketTopo.MapView();
		this.mapContextMenu = new System.Windows.Forms.ContextMenu();
		this.menuDumpM = new System.Windows.Forms.MenuItem();
		this.menuSep21 = new System.Windows.Forms.MenuItem();
		this.menuOptM = new System.Windows.Forms.MenuItem();
		this.menuIds = new System.Windows.Forms.MenuItem();
		this.menuThick = new System.Windows.Forms.MenuItem();
		this.menuSep19 = new System.Windows.Forms.MenuItem();
		this.menuM2D = new System.Windows.Forms.MenuItem();
		this.menuM2O = new System.Windows.Forms.MenuItem();
		this.menuM2S = new System.Windows.Forms.MenuItem();
		this.panelData = new System.Windows.Forms.Panel();
		this.textData = new System.Windows.Forms.TextBox();
		this.textComm = new System.Windows.Forms.TextBox();
		this.references = new System.Windows.Forms.BindingSource(this.components);
		this.dataGridRef = new System.Windows.Forms.DataGrid();
		this.refContextMenu = new System.Windows.Forms.ContextMenu();
		this.menuRComm = new System.Windows.Forms.MenuItem();
		this.menuSep12 = new System.Windows.Forms.MenuItem();
		this.menuRInsert = new System.Windows.Forms.MenuItem();
		this.menuRDelete = new System.Windows.Forms.MenuItem();
		this.menuSep14 = new System.Windows.Forms.MenuItem();
		this.menuRStart = new System.Windows.Forms.MenuItem();
		this.menuSep9 = new System.Windows.Forms.MenuItem();
		this.menuRCopy = new System.Windows.Forms.MenuItem();
		this.menuRPaste = new System.Windows.Forms.MenuItem();
		this.menuSep13 = new System.Windows.Forms.MenuItem();
		this.menuR2M = new System.Windows.Forms.MenuItem();
		this.menuR2O = new System.Windows.Forms.MenuItem();
		this.menuR2S = new System.Windows.Forms.MenuItem();
		this.dataGridTableStyleRef = new System.Windows.Forms.DataGridTableStyle();
		this.dataGridTextBoxColumnR1 = new System.Windows.Forms.DataGridTextBoxColumn();
		this.dataGridTextBoxColumnR2 = new System.Windows.Forms.DataGridTextBoxColumn();
		this.dataGridTextBoxColumnR3 = new System.Windows.Forms.DataGridTextBoxColumn();
		this.dataGridTextBoxColumnR4 = new System.Windows.Forms.DataGridTextBoxColumn();
		this.dataGridTextBoxColumnR5 = new System.Windows.Forms.DataGridTextBoxColumn();
		this.database = new System.Windows.Forms.BindingSource(this.components);
		this.dataGrid = new System.Windows.Forms.DataGrid();
		this.dataGridTableStyle1 = new System.Windows.Forms.DataGridTableStyle();
		this.dataGridTextBoxColumn1 = new System.Windows.Forms.DataGridTextBoxColumn();
		this.dataGridTextBoxColumn2 = new System.Windows.Forms.DataGridTextBoxColumn();
		this.dataGridTextBoxColumn3 = new System.Windows.Forms.DataGridTextBoxColumn();
		this.dataGridTextBoxColumn4 = new System.Windows.Forms.DataGridTextBoxColumn();
		this.dataGridTextBoxColumn5 = new System.Windows.Forms.DataGridTextBoxColumn();
		this.dataGridTextBoxColumn6 = new System.Windows.Forms.DataGridTextBoxColumn();
		this.timer = new System.Windows.Forms.Timer();
		this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
		this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
		this.txtFileDialog = new System.Windows.Forms.SaveFileDialog();
		this.saveChanges = new System.Windows.Forms.Label();
		this.sideContextMenu = new System.Windows.Forms.ContextMenu();
		this.menuDumpS = new System.Windows.Forms.MenuItem();
		this.menuSep23 = new System.Windows.Forms.MenuItem();
		this.menuFlip = new System.Windows.Forms.MenuItem();
		this.menuFlipAll = new System.Windows.Forms.MenuItem();
		this.menuSep11 = new System.Windows.Forms.MenuItem();
		this.menuOptS = new System.Windows.Forms.MenuItem();
		this.menuRasterS = new System.Windows.Forms.MenuItem();
		this.menuIdsS = new System.Windows.Forms.MenuItem();
		this.menuThickS = new System.Windows.Forms.MenuItem();
		this.menuSep17 = new System.Windows.Forms.MenuItem();
		this.menuXSectV = new System.Windows.Forms.MenuItem();
		this.menuXSectH = new System.Windows.Forms.MenuItem();
		this.menuSep7 = new System.Windows.Forms.MenuItem();
		this.menuS2D = new System.Windows.Forms.MenuItem();
		this.menuS2M = new System.Windows.Forms.MenuItem();
		this.menuS2O = new System.Windows.Forms.MenuItem();
		this.menuItem1 = new System.Windows.Forms.MenuItem();
		this.menuLOn = new System.Windows.Forms.MenuItem();
		this.menuLOff = new System.Windows.Forms.MenuItem();
		this.menuMeasure = new System.Windows.Forms.MenuItem();
		System.Windows.Forms.MenuItem menuItem = new System.Windows.Forms.MenuItem();
		this.panelPaint.SuspendLayout();
		this.panelMap.SuspendLayout();
		this.panelData.SuspendLayout();
		base.SuspendLayout();
		menuItem.MenuItems.Add(this.menuTitle);
		menuItem.MenuItems.Add(this.menuSmart);
		menuItem.MenuItems.Add(this.menuReverse);
		menuItem.MenuItems.Add(this.menuUnit);
		menuItem.MenuItems.Add(this.menuPort);
		menuItem.MenuItems.Add(this.menuSep20);
		menuItem.MenuItems.Add(this.menuInfo);
		resources.ApplyResources(menuItem, "menuOpt");
		resources.ApplyResources(this.menuTitle, "menuTitle");
		this.menuTitle.Click += new System.EventHandler(menuTitle_Click);
		resources.ApplyResources(this.menuSmart, "menuSmart");
		this.menuSmart.Click += new System.EventHandler(menuSmart_Click);
		resources.ApplyResources(this.menuReverse, "menuReverse");
		this.menuReverse.Click += new System.EventHandler(menuReverse_Click);
		this.menuUnit.MenuItems.Add(this.menuDeg);
		this.menuUnit.MenuItems.Add(this.menuGrad);
		this.menuUnit.MenuItems.Add(this.menuSep18);
		this.menuUnit.MenuItems.Add(this.menuMeter);
		this.menuUnit.MenuItems.Add(this.menuFeet);
		resources.ApplyResources(this.menuUnit, "menuUnit");
		resources.ApplyResources(this.menuDeg, "menuDeg");
		this.menuDeg.Click += new System.EventHandler(menuDeg_Click);
		resources.ApplyResources(this.menuGrad, "menuGrad");
		this.menuGrad.Click += new System.EventHandler(menuGrad_Click);
		resources.ApplyResources(this.menuSep18, "menuSep18");
		resources.ApplyResources(this.menuMeter, "menuMeter");
		this.menuMeter.Click += new System.EventHandler(menuMeter_Click);
		resources.ApplyResources(this.menuFeet, "menuFeet");
		this.menuFeet.Click += new System.EventHandler(menuFeet_Click);
		this.menuPort.MenuItems.Add(this.menuNone);
		this.menuPort.MenuItems.Add(this.menuCom0);
		this.menuPort.MenuItems.Add(this.menuCom1);
		this.menuPort.MenuItems.Add(this.menuCom2);
		this.menuPort.MenuItems.Add(this.menuCom3);
		this.menuPort.MenuItems.Add(this.menuCom4);
		this.menuPort.MenuItems.Add(this.menuCom5);
		this.menuPort.MenuItems.Add(this.menuCom6);
		this.menuPort.MenuItems.Add(this.menuCom7);
		this.menuPort.MenuItems.Add(this.menuCom8);
		this.menuPort.MenuItems.Add(this.menuCom9);
		this.menuPort.MenuItems.Add(this.menuComX);
		resources.ApplyResources(this.menuPort, "menuPort");
		resources.ApplyResources(this.menuNone, "menuNone");
		this.menuNone.Click += new System.EventHandler(menuNone_Click);
		resources.ApplyResources(this.menuCom0, "menuCom0");
		this.menuCom0.Click += new System.EventHandler(menuCom_Click);
		resources.ApplyResources(this.menuCom1, "menuCom1");
		this.menuCom1.Click += new System.EventHandler(menuCom_Click);
		resources.ApplyResources(this.menuCom2, "menuCom2");
		this.menuCom2.Click += new System.EventHandler(menuCom_Click);
		resources.ApplyResources(this.menuCom3, "menuCom3");
		this.menuCom3.Click += new System.EventHandler(menuCom_Click);
		resources.ApplyResources(this.menuCom4, "menuCom4");
		this.menuCom4.Click += new System.EventHandler(menuCom_Click);
		resources.ApplyResources(this.menuCom5, "menuCom5");
		this.menuCom5.Click += new System.EventHandler(menuCom_Click);
		resources.ApplyResources(this.menuCom6, "menuCom6");
		this.menuCom6.Click += new System.EventHandler(menuCom_Click);
		resources.ApplyResources(this.menuCom7, "menuCom7");
		this.menuCom7.Click += new System.EventHandler(menuCom_Click);
		resources.ApplyResources(this.menuCom8, "menuCom8");
		this.menuCom8.Click += new System.EventHandler(menuCom_Click);
		resources.ApplyResources(this.menuCom9, "menuCom9");
		this.menuCom9.Click += new System.EventHandler(menuCom_Click);
		resources.ApplyResources(this.menuComX, "menuComX");
		this.menuComX.Click += new System.EventHandler(menuComX_Click);
		resources.ApplyResources(this.menuSep20, "menuSep20");
		resources.ApplyResources(this.menuInfo, "menuInfo");
		this.menuInfo.Click += new System.EventHandler(menuInfo_Click);
		this.mainMenu.MenuItems.Add(this.menuMenu);
		this.menuMenu.MenuItems.Add(this.menuBluetooth);
		this.menuMenu.MenuItems.Add(menuItem);
		this.menuMenu.MenuItems.Add(this.menuSep16);
		this.menuMenu.MenuItems.Add(this.menuTrip);
		this.menuMenu.MenuItems.Add(this.menuCalib);
		this.menuMenu.MenuItems.Add(this.menuSep2);
		this.menuMenu.MenuItems.Add(this.menuFile);
		this.menuMenu.MenuItems.Add(this.menuImport);
		this.menuMenu.MenuItems.Add(this.menuExport);
		this.menuMenu.MenuItems.Add(this.menuLoops);
		this.menuMenu.MenuItems.Add(this.menuSep6);
		this.menuMenu.MenuItems.Add(this.menuUndo);
		this.menuMenu.MenuItems.Add(this.menuRedo);
		this.menuMenu.MenuItems.Add(this.menuSep1);
		this.menuMenu.MenuItems.Add(this.menuExit);
		resources.ApplyResources(this.menuMenu, "menuMenu");
		this.menuMenu.Popup += new System.EventHandler(menuMenu_Popup);
		this.menuBluetooth.MenuItems.Add(this.menuConnect);
		this.menuBluetooth.MenuItems.Add(this.menuDisconn);
		this.menuBluetooth.MenuItems.Add(this.menuAuto);
		this.menuBluetooth.MenuItems.Add(this.menuItem1);
		this.menuBluetooth.MenuItems.Add(this.menuLOn);
		this.menuBluetooth.MenuItems.Add(this.menuLOff);
		this.menuBluetooth.MenuItems.Add(this.menuMeasure);
		resources.ApplyResources(this.menuBluetooth, "menuBluetooth");
		resources.ApplyResources(this.menuConnect, "menuConnect");
		this.menuConnect.Click += new System.EventHandler(menuConnect_Click);
		resources.ApplyResources(this.menuDisconn, "menuDisconn");
		this.menuDisconn.Click += new System.EventHandler(menuDisconn_Click);
		resources.ApplyResources(this.menuAuto, "menuAuto");
		this.menuAuto.Click += new System.EventHandler(menuAuto_Click);
		resources.ApplyResources(this.menuSep16, "menuSep16");
		resources.ApplyResources(this.menuTrip, "menuTrip");
		this.menuTrip.Click += new System.EventHandler(menuTrip_Click);
		resources.ApplyResources(this.menuCalib, "menuCalib");
		this.menuCalib.Click += new System.EventHandler(menuCalib_Click);
		resources.ApplyResources(this.menuSep2, "menuSep2");
		this.menuFile.MenuItems.Add(this.menuNewCave);
		this.menuFile.MenuItems.Add(this.menuNew);
		this.menuFile.MenuItems.Add(this.menuOpen);
		this.menuFile.MenuItems.Add(this.menuSave);
		this.menuFile.MenuItems.Add(this.menuSaveAs);
		resources.ApplyResources(this.menuFile, "menuFile");
		resources.ApplyResources(this.menuNewCave, "menuNewCave");
		this.menuNewCave.Click += new System.EventHandler(menuNewCave_Click);
		resources.ApplyResources(this.menuNew, "menuNew");
		this.menuNew.Click += new System.EventHandler(menuNew_Click);
		resources.ApplyResources(this.menuOpen, "menuOpen");
		this.menuOpen.Click += new System.EventHandler(menuOpen_Click);
		resources.ApplyResources(this.menuSave, "menuSave");
		this.menuSave.Click += new System.EventHandler(menuSave_Click);
		resources.ApplyResources(this.menuSaveAs, "menuSaveAs");
		this.menuSaveAs.Click += new System.EventHandler(menuSaveAs_Click);
		resources.ApplyResources(this.menuImport, "menuImport");
		this.menuExport.MenuItems.Add(this.menuExpDXF);
		this.menuExport.MenuItems.Add(this.menuExp3D);
		this.menuExport.MenuItems.Add(this.menuExpText);
		resources.ApplyResources(this.menuExport, "menuExport");
		resources.ApplyResources(this.menuExpDXF, "menuExpDXF");
		this.menuExpDXF.Click += new System.EventHandler(menuExpDXF_Click);
		resources.ApplyResources(this.menuExp3D, "menuExp3D");
		this.menuExp3D.Click += new System.EventHandler(menuExp3D_Click);
		resources.ApplyResources(this.menuExpText, "menuExpText");
		this.menuExpText.Click += new System.EventHandler(menuExpText_Click);
		this.menuLoops.MenuItems.Add(this.menuCloseAll);
		this.menuLoops.MenuItems.Add(this.menuCloseAct);
		this.menuLoops.MenuItems.Add(this.menuCloseNone);
		resources.ApplyResources(this.menuLoops, "menuLoops");
		resources.ApplyResources(this.menuCloseAll, "menuCloseAll");
		this.menuCloseAll.Click += new System.EventHandler(menuCloseAll_Click);
		resources.ApplyResources(this.menuCloseAct, "menuCloseAct");
		this.menuCloseAct.Click += new System.EventHandler(menuCloseAct_Click);
		resources.ApplyResources(this.menuCloseNone, "menuCloseNone");
		this.menuCloseNone.Click += new System.EventHandler(menuCloseNone_Click);
		resources.ApplyResources(this.menuSep6, "menuSep6");
		resources.ApplyResources(this.menuUndo, "menuUndo");
		this.menuUndo.Click += new System.EventHandler(menuUndo_Click);
		resources.ApplyResources(this.menuRedo, "menuRedo");
		this.menuRedo.Click += new System.EventHandler(menuRedo_Click);
		resources.ApplyResources(this.menuSep1, "menuSep1");
		resources.ApplyResources(this.menuExit, "menuExit");
		this.menuExit.Click += new System.EventHandler(menuExit_Click);
		this.toolBar.Buttons.Add(this.toolBarData);
		this.toolBar.Buttons.Add(this.toolBarMap);
		this.toolBar.Buttons.Add(this.toolBarSketch);
		this.toolBar.Buttons.Add(this.toolBarSep);
		this.toolBar.Buttons.Add(this.toolBarPlus);
		this.toolBar.Buttons.Add(this.toolBarMinus);
		this.toolBar.Buttons.Add(this.toolBarUndo);
		this.toolBar.Buttons.Add(this.toolBarRedo);
		this.toolBar.ImageList = this.imageList;
		this.toolBar.Name = "toolBar";
		this.toolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(toolBar_ButtonClick);
		resources.ApplyResources(this.toolBarData, "toolBarData");
		this.toolBarData.Pushed = true;
		resources.ApplyResources(this.toolBarMap, "toolBarMap");
		resources.ApplyResources(this.toolBarSketch, "toolBarSketch");
		this.toolBarSep.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
		resources.ApplyResources(this.toolBarPlus, "toolBarPlus");
		resources.ApplyResources(this.toolBarMinus, "toolBarMinus");
		resources.ApplyResources(this.toolBarUndo, "toolBarUndo");
		resources.ApplyResources(this.toolBarRedo, "toolBarRedo");
		this.imageList.Images.Clear();
		this.imageList.Images.Add((System.Drawing.Image)resources.GetObject("resource"));
		this.imageList.Images.Add((System.Drawing.Image)resources.GetObject("resource1"));
		this.imageList.Images.Add((System.Drawing.Image)resources.GetObject("resource2"));
		this.imageList.Images.Add((System.Drawing.Image)resources.GetObject("resource3"));
		this.imageList.Images.Add((System.Drawing.Image)resources.GetObject("resource4"));
		this.imageList.Images.Add((System.Drawing.Image)resources.GetObject("resource5"));
		this.imageList.Images.Add((System.Drawing.Image)resources.GetObject("resource6"));
		this.imageList.Images.Add((System.Drawing.Image)resources.GetObject("resource7"));
		this.imageList.Images.Add((System.Drawing.Image)resources.GetObject("resource8"));
		this.imageList.Images.Add((System.Drawing.Image)resources.GetObject("resource9"));
		this.imageList.Images.Add((System.Drawing.Image)resources.GetObject("resource10"));
		this.imageList.Images.Add((System.Drawing.Image)resources.GetObject("resource11"));
		this.imageList.Images.Add((System.Drawing.Image)resources.GetObject("resource12"));
		this.imageList.Images.Add((System.Drawing.Image)resources.GetObject("resource13"));
		this.dataContextMenu.MenuItems.Add(this.menuComment);
		this.dataContextMenu.MenuItems.Add(this.menuSTrip);
		this.dataContextMenu.MenuItems.Add(this.menuSep4);
		this.dataContextMenu.MenuItems.Add(this.menuInsert);
		this.dataContextMenu.MenuItems.Add(this.menuDelete);
		this.dataContextMenu.MenuItems.Add(this.menuSep8);
		this.dataContextMenu.MenuItems.Add(this.menuToBranch);
		this.dataContextMenu.MenuItems.Add(this.menuRenum);
		this.dataContextMenu.MenuItems.Add(this.menuStart);
		this.dataContextMenu.MenuItems.Add(this.menuCont);
		this.dataContextMenu.MenuItems.Add(this.menuEnd);
		this.dataContextMenu.MenuItems.Add(this.menuSep3);
		this.dataContextMenu.MenuItems.Add(this.menuCopy);
		this.dataContextMenu.MenuItems.Add(this.menuPaste);
		this.dataContextMenu.MenuItems.Add(this.menuSep5);
		this.dataContextMenu.MenuItems.Add(this.menuD2M);
		this.dataContextMenu.MenuItems.Add(this.menuD2O);
		this.dataContextMenu.MenuItems.Add(this.menuD2S);
		this.dataContextMenu.Popup += new System.EventHandler(dataContextMenu_Popup);
		resources.ApplyResources(this.menuComment, "menuComment");
		this.menuComment.Click += new System.EventHandler(menuComment_Click);
		resources.ApplyResources(this.menuSTrip, "menuSTrip");
		this.menuSTrip.Click += new System.EventHandler(menuSTrip_Click);
		resources.ApplyResources(this.menuSep4, "menuSep4");
		resources.ApplyResources(this.menuInsert, "menuInsert");
		this.menuInsert.Click += new System.EventHandler(menuInsert_Click);
		resources.ApplyResources(this.menuDelete, "menuDelete");
		this.menuDelete.Click += new System.EventHandler(menuDelete_Click);
		resources.ApplyResources(this.menuSep8, "menuSep8");
		resources.ApplyResources(this.menuToBranch, "menuToBranch");
		this.menuToBranch.Click += new System.EventHandler(menuToBranch_Click);
		resources.ApplyResources(this.menuRenum, "menuRenum");
		this.menuRenum.Click += new System.EventHandler(menuRenum_Click);
		resources.ApplyResources(this.menuStart, "menuStart");
		this.menuStart.Click += new System.EventHandler(menuStart_Click);
		resources.ApplyResources(this.menuCont, "menuCont");
		this.menuCont.Click += new System.EventHandler(menuCont_Click);
		resources.ApplyResources(this.menuEnd, "menuEnd");
		this.menuEnd.Click += new System.EventHandler(menuEnd_Click);
		resources.ApplyResources(this.menuSep3, "menuSep3");
		resources.ApplyResources(this.menuCopy, "menuCopy");
		this.menuCopy.Click += new System.EventHandler(menuCopy_Click);
		resources.ApplyResources(this.menuPaste, "menuPaste");
		this.menuPaste.Click += new System.EventHandler(menuPaste_Click);
		resources.ApplyResources(this.menuSep5, "menuSep5");
		resources.ApplyResources(this.menuD2M, "menuD2M");
		this.menuD2M.Click += new System.EventHandler(menuD2M_Click);
		resources.ApplyResources(this.menuD2O, "menuD2O");
		this.menuD2O.Click += new System.EventHandler(menuD2O_Click);
		resources.ApplyResources(this.menuD2S, "menuD2S");
		this.menuD2S.Click += new System.EventHandler(menuD2S_Click);
		this.panelPaint.Controls.Add(this.pictSelDelete);
		this.panelPaint.Controls.Add(this.pictSelGreen);
		this.panelPaint.Controls.Add(this.pictSelOrange);
		this.panelPaint.Controls.Add(this.pictSelRed);
		this.panelPaint.Controls.Add(this.pictSelBlue);
		this.panelPaint.Controls.Add(this.pictSelBrown);
		this.panelPaint.Controls.Add(this.pictSelGray);
		this.panelPaint.Controls.Add(this.pictSelBlack);
		this.panelPaint.Controls.Add(this.pictSelMove);
		this.panelPaint.Controls.Add(this.pictDelete);
		this.panelPaint.Controls.Add(this.pictGreen);
		this.panelPaint.Controls.Add(this.pictOrange);
		this.panelPaint.Controls.Add(this.pictRed);
		this.panelPaint.Controls.Add(this.pictBlue);
		this.panelPaint.Controls.Add(this.pictBrown);
		this.panelPaint.Controls.Add(this.pictGray);
		this.panelPaint.Controls.Add(this.pictBlack);
		this.panelPaint.Controls.Add(this.pictMove);
		this.panelPaint.Controls.Add(this.moveRD);
		this.panelPaint.Controls.Add(this.moveLD);
		this.panelPaint.Controls.Add(this.moveRU);
		this.panelPaint.Controls.Add(this.moveLU);
		this.panelPaint.Controls.Add(this.sketchView);
		this.panelPaint.Controls.Add(this.fileError);
		resources.ApplyResources(this.panelPaint, "panelPaint");
		this.panelPaint.Name = "panelPaint";
		resources.ApplyResources(this.pictSelDelete, "pictSelDelete");
		this.pictSelDelete.Name = "pictSelDelete";
		resources.ApplyResources(this.pictSelGreen, "pictSelGreen");
		this.pictSelGreen.Name = "pictSelGreen";
		resources.ApplyResources(this.pictSelOrange, "pictSelOrange");
		this.pictSelOrange.Name = "pictSelOrange";
		resources.ApplyResources(this.pictSelRed, "pictSelRed");
		this.pictSelRed.Name = "pictSelRed";
		resources.ApplyResources(this.pictSelBlue, "pictSelBlue");
		this.pictSelBlue.Name = "pictSelBlue";
		resources.ApplyResources(this.pictSelBrown, "pictSelBrown");
		this.pictSelBrown.Name = "pictSelBrown";
		resources.ApplyResources(this.pictSelGray, "pictSelGray");
		this.pictSelGray.Name = "pictSelGray";
		resources.ApplyResources(this.pictSelBlack, "pictSelBlack");
		this.pictSelBlack.Name = "pictSelBlack";
		resources.ApplyResources(this.pictSelMove, "pictSelMove");
		this.pictSelMove.Name = "pictSelMove";
		resources.ApplyResources(this.pictDelete, "pictDelete");
		this.pictDelete.Name = "pictDelete";
		this.pictDelete.Click += new System.EventHandler(symbol_Click);
		resources.ApplyResources(this.pictGreen, "pictGreen");
		this.pictGreen.Name = "pictGreen";
		this.pictGreen.Click += new System.EventHandler(symbol_Click);
		resources.ApplyResources(this.pictOrange, "pictOrange");
		this.pictOrange.Name = "pictOrange";
		this.pictOrange.Click += new System.EventHandler(symbol_Click);
		resources.ApplyResources(this.pictRed, "pictRed");
		this.pictRed.Name = "pictRed";
		this.pictRed.Click += new System.EventHandler(symbol_Click);
		resources.ApplyResources(this.pictBlue, "pictBlue");
		this.pictBlue.Name = "pictBlue";
		this.pictBlue.Click += new System.EventHandler(symbol_Click);
		resources.ApplyResources(this.pictBrown, "pictBrown");
		this.pictBrown.Name = "pictBrown";
		this.pictBrown.Click += new System.EventHandler(symbol_Click);
		resources.ApplyResources(this.pictGray, "pictGray");
		this.pictGray.Name = "pictGray";
		this.pictGray.Click += new System.EventHandler(symbol_Click);
		resources.ApplyResources(this.pictBlack, "pictBlack");
		this.pictBlack.Name = "pictBlack";
		this.pictBlack.Click += new System.EventHandler(symbol_Click);
		resources.ApplyResources(this.pictMove, "pictMove");
		this.pictMove.Name = "pictMove";
		this.pictMove.Click += new System.EventHandler(symbol_Click);
		resources.ApplyResources(this.moveRD, "moveRD");
		this.moveRD.Name = "moveRD";
		resources.ApplyResources(this.moveLD, "moveLD");
		this.moveLD.Name = "moveLD";
		resources.ApplyResources(this.moveRU, "moveRU");
		this.moveRU.Name = "moveRU";
		resources.ApplyResources(this.moveLU, "moveLU");
		this.moveLU.Name = "moveLU";
		this.sketchView.ContextMenu = this.planContextMenu;
		resources.ApplyResources(this.sketchView, "sketchView");
		this.sketchView.Name = "sketchView";
		this.planContextMenu.MenuItems.Add(this.menuDumpP);
		this.planContextMenu.MenuItems.Add(this.menuSep22);
		this.planContextMenu.MenuItems.Add(this.menuOptP);
		this.planContextMenu.MenuItems.Add(this.menuSep15);
		this.planContextMenu.MenuItems.Add(this.menuXSect);
		this.planContextMenu.MenuItems.Add(this.menuSep10);
		this.planContextMenu.MenuItems.Add(this.menuO2D);
		this.planContextMenu.MenuItems.Add(this.menuO2M);
		this.planContextMenu.MenuItems.Add(this.menuO2S);
		this.planContextMenu.Popup += new System.EventHandler(outlineContextMenu_Popup);
		resources.ApplyResources(this.menuDumpP, "menuDumpP");
		this.menuDumpP.Click += new System.EventHandler(menuDump_Click);
		resources.ApplyResources(this.menuSep22, "menuSep22");
		this.menuOptP.MenuItems.Add(this.menuRaster);
		this.menuOptP.MenuItems.Add(this.menuAll);
		this.menuOptP.MenuItems.Add(this.menuIdsP);
		this.menuOptP.MenuItems.Add(this.menuThickP);
		resources.ApplyResources(this.menuOptP, "menuOptP");
		resources.ApplyResources(this.menuRaster, "menuRaster");
		this.menuRaster.Click += new System.EventHandler(menuRaster_Click);
		resources.ApplyResources(this.menuAll, "menuAll");
		this.menuAll.Click += new System.EventHandler(menuAll_Click);
		resources.ApplyResources(this.menuIdsP, "menuIdsP");
		this.menuIdsP.Click += new System.EventHandler(menuIdsS_Click);
		resources.ApplyResources(this.menuThickP, "menuThickP");
		this.menuThickP.Click += new System.EventHandler(menuThick_Click);
		resources.ApplyResources(this.menuSep15, "menuSep15");
		resources.ApplyResources(this.menuXSect, "menuXSect");
		this.menuXSect.Click += new System.EventHandler(menuXSect_Click);
		resources.ApplyResources(this.menuSep10, "menuSep10");
		resources.ApplyResources(this.menuO2D, "menuO2D");
		this.menuO2D.Click += new System.EventHandler(menuO2D_Click);
		resources.ApplyResources(this.menuO2M, "menuO2M");
		this.menuO2M.Click += new System.EventHandler(menuO2M_Click);
		resources.ApplyResources(this.menuO2S, "menuO2S");
		this.menuO2S.Click += new System.EventHandler(menuO2S_Click);
		resources.ApplyResources(this.fileError, "fileError");
		this.fileError.Name = "fileError";
		this.panelMap.Controls.Add(this.mapView);
		resources.ApplyResources(this.panelMap, "panelMap");
		this.panelMap.Name = "panelMap";
		this.mapView.ContextMenu = this.mapContextMenu;
		resources.ApplyResources(this.mapView, "mapView");
		this.mapView.Name = "mapView";
		this.mapContextMenu.MenuItems.Add(this.menuDumpM);
		this.mapContextMenu.MenuItems.Add(this.menuSep21);
		this.mapContextMenu.MenuItems.Add(this.menuOptM);
		this.mapContextMenu.MenuItems.Add(this.menuSep19);
		this.mapContextMenu.MenuItems.Add(this.menuM2D);
		this.mapContextMenu.MenuItems.Add(this.menuM2O);
		this.mapContextMenu.MenuItems.Add(this.menuM2S);
		this.mapContextMenu.Popup += new System.EventHandler(mapContextMenu_Popup);
		resources.ApplyResources(this.menuDumpM, "menuDumpM");
		this.menuDumpM.Click += new System.EventHandler(menuDumpM_Click);
		resources.ApplyResources(this.menuSep21, "menuSep21");
		this.menuOptM.MenuItems.Add(this.menuIds);
		this.menuOptM.MenuItems.Add(this.menuThick);
		resources.ApplyResources(this.menuOptM, "menuOptM");
		resources.ApplyResources(this.menuIds, "menuIds");
		this.menuIds.Click += new System.EventHandler(menuIds_Click);
		resources.ApplyResources(this.menuThick, "menuThick");
		this.menuThick.Click += new System.EventHandler(menuThick_Click);
		resources.ApplyResources(this.menuSep19, "menuSep19");
		resources.ApplyResources(this.menuM2D, "menuM2D");
		this.menuM2D.Click += new System.EventHandler(menuM2D_Click);
		resources.ApplyResources(this.menuM2O, "menuM2O");
		this.menuM2O.Click += new System.EventHandler(menuM2O_Click);
		resources.ApplyResources(this.menuM2S, "menuM2S");
		this.menuM2S.Click += new System.EventHandler(menuM2S_Click);
		this.panelData.Controls.Add(this.textData);
		this.panelData.Controls.Add(this.textComm);
		this.panelData.Controls.Add(this.dataGridRef);
		this.panelData.Controls.Add(this.dataGrid);
		resources.ApplyResources(this.panelData, "panelData");
		this.panelData.Name = "panelData";
		resources.ApplyResources(this.textData, "textData");
		this.textData.Name = "textData";
		this.textData.TabStop = false;
		resources.ApplyResources(this.textComm, "textComm");
		this.textComm.Name = "textComm";
		this.textComm.TabStop = false;
		this.references.AllowNew = true;
		this.references.DataSource = typeof(PocketTopo.Reference);
		this.dataGridRef.BackgroundColor = System.Drawing.Color.FromArgb(128, 128, 128);
		this.dataGridRef.ContextMenu = this.refContextMenu;
		this.dataGridRef.DataSource = this.references;
		resources.ApplyResources(this.dataGridRef, "dataGridRef");
		this.dataGridRef.Name = "dataGridRef";
		this.dataGridRef.RowHeadersVisible = false;
		this.dataGridRef.TableStyles.Add(this.dataGridTableStyleRef);
		this.dataGridRef.DoubleClick += new System.EventHandler(dataGrid_DoubleClick);
		this.dataGridRef.CurrentCellChanged += new System.EventHandler(dataGrid_CurrentCellChanged);
		this.dataGridRef.MouseDown += new System.Windows.Forms.MouseEventHandler(dataGrid_MouseDown);
		this.dataGridRef.Click += new System.EventHandler(dataGrid_Click);
		this.refContextMenu.MenuItems.Add(this.menuRComm);
		this.refContextMenu.MenuItems.Add(this.menuSep12);
		this.refContextMenu.MenuItems.Add(this.menuRInsert);
		this.refContextMenu.MenuItems.Add(this.menuRDelete);
		this.refContextMenu.MenuItems.Add(this.menuSep14);
		this.refContextMenu.MenuItems.Add(this.menuRStart);
		this.refContextMenu.MenuItems.Add(this.menuSep9);
		this.refContextMenu.MenuItems.Add(this.menuRCopy);
		this.refContextMenu.MenuItems.Add(this.menuRPaste);
		this.refContextMenu.MenuItems.Add(this.menuSep13);
		this.refContextMenu.MenuItems.Add(this.menuR2M);
		this.refContextMenu.MenuItems.Add(this.menuR2O);
		this.refContextMenu.MenuItems.Add(this.menuR2S);
		this.refContextMenu.Popup += new System.EventHandler(refContextMenu_Popup);
		resources.ApplyResources(this.menuRComm, "menuRComm");
		this.menuRComm.Click += new System.EventHandler(menuRComment_Click);
		resources.ApplyResources(this.menuSep12, "menuSep12");
		resources.ApplyResources(this.menuRInsert, "menuRInsert");
		this.menuRInsert.Click += new System.EventHandler(menuRInsert_Click);
		resources.ApplyResources(this.menuRDelete, "menuRDelete");
		this.menuRDelete.Click += new System.EventHandler(menuRDelete_Click);
		resources.ApplyResources(this.menuSep14, "menuSep14");
		resources.ApplyResources(this.menuRStart, "menuRStart");
		this.menuRStart.Click += new System.EventHandler(menuRStart_Click);
		resources.ApplyResources(this.menuSep9, "menuSep9");
		resources.ApplyResources(this.menuRCopy, "menuRCopy");
		this.menuRCopy.Click += new System.EventHandler(menuRCopy_Click);
		resources.ApplyResources(this.menuRPaste, "menuRPaste");
		this.menuRPaste.Click += new System.EventHandler(menuRPaste_Click);
		resources.ApplyResources(this.menuSep13, "menuSep13");
		resources.ApplyResources(this.menuR2M, "menuR2M");
		this.menuR2M.Click += new System.EventHandler(menuD2M_Click);
		resources.ApplyResources(this.menuR2O, "menuR2O");
		this.menuR2O.Click += new System.EventHandler(menuD2O_Click);
		resources.ApplyResources(this.menuR2S, "menuR2S");
		this.menuR2S.Click += new System.EventHandler(menuD2S_Click);
		this.dataGridTableStyleRef.GridColumnStyles.Add(this.dataGridTextBoxColumnR1);
		this.dataGridTableStyleRef.GridColumnStyles.Add(this.dataGridTextBoxColumnR2);
		this.dataGridTableStyleRef.GridColumnStyles.Add(this.dataGridTextBoxColumnR3);
		this.dataGridTableStyleRef.GridColumnStyles.Add(this.dataGridTextBoxColumnR4);
		this.dataGridTableStyleRef.GridColumnStyles.Add(this.dataGridTextBoxColumnR5);
		this.dataGridTableStyleRef.MappingName = "Reference";
		resources.ApplyResources(this.dataGridTextBoxColumnR1, "dataGridTextBoxColumnR1");
		resources.ApplyResources(this.dataGridTextBoxColumnR2, "dataGridTextBoxColumnR2");
		resources.ApplyResources(this.dataGridTextBoxColumnR3, "dataGridTextBoxColumnR3");
		resources.ApplyResources(this.dataGridTextBoxColumnR4, "dataGridTextBoxColumnR4");
		resources.ApplyResources(this.dataGridTextBoxColumnR5, "dataGridTextBoxColumnR5");
		this.database.AllowNew = true;
		this.database.DataSource = typeof(PocketTopo.Station);
		this.dataGrid.BackgroundColor = System.Drawing.Color.FromArgb(128, 128, 128);
		this.dataGrid.ContextMenu = this.dataContextMenu;
		this.dataGrid.DataSource = this.database;
		resources.ApplyResources(this.dataGrid, "dataGrid");
		this.dataGrid.Name = "dataGrid";
		this.dataGrid.RowHeadersVisible = false;
		this.dataGrid.TableStyles.Add(this.dataGridTableStyle1);
		this.dataGrid.TabStop = false;
		this.dataGrid.DoubleClick += new System.EventHandler(dataGrid_DoubleClick);
		this.dataGrid.CurrentCellChanged += new System.EventHandler(dataGrid_CurrentCellChanged);
		this.dataGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(dataGrid_MouseDown);
		this.dataGrid.Click += new System.EventHandler(dataGrid_Click);
		this.dataGridTableStyle1.GridColumnStyles.Add(this.dataGridTextBoxColumn1);
		this.dataGridTableStyle1.GridColumnStyles.Add(this.dataGridTextBoxColumn2);
		this.dataGridTableStyle1.GridColumnStyles.Add(this.dataGridTextBoxColumn3);
		this.dataGridTableStyle1.GridColumnStyles.Add(this.dataGridTextBoxColumn4);
		this.dataGridTableStyle1.GridColumnStyles.Add(this.dataGridTextBoxColumn5);
		this.dataGridTableStyle1.GridColumnStyles.Add(this.dataGridTextBoxColumn6);
		this.dataGridTableStyle1.MappingName = "Station";
		resources.ApplyResources(this.dataGridTextBoxColumn1, "dataGridTextBoxColumn1");
		resources.ApplyResources(this.dataGridTextBoxColumn2, "dataGridTextBoxColumn2");
		resources.ApplyResources(this.dataGridTextBoxColumn3, "dataGridTextBoxColumn3");
		resources.ApplyResources(this.dataGridTextBoxColumn4, "dataGridTextBoxColumn4");
		resources.ApplyResources(this.dataGridTextBoxColumn5, "dataGridTextBoxColumn5");
		resources.ApplyResources(this.dataGridTextBoxColumn6, "dataGridTextBoxColumn6");
		this.timer.Interval = 1000;
		this.timer.Tick += new System.EventHandler(timer_Tick);
		resources.ApplyResources(this.openFileDialog, "openFileDialog");
		resources.ApplyResources(this.saveFileDialog, "saveFileDialog");
		resources.ApplyResources(this.txtFileDialog, "txtFileDialog");
		resources.ApplyResources(this.saveChanges, "saveChanges");
		this.saveChanges.Name = "saveChanges";
		this.sideContextMenu.MenuItems.Add(this.menuDumpS);
		this.sideContextMenu.MenuItems.Add(this.menuSep23);
		this.sideContextMenu.MenuItems.Add(this.menuFlip);
		this.sideContextMenu.MenuItems.Add(this.menuFlipAll);
		this.sideContextMenu.MenuItems.Add(this.menuSep11);
		this.sideContextMenu.MenuItems.Add(this.menuOptS);
		this.sideContextMenu.MenuItems.Add(this.menuSep17);
		this.sideContextMenu.MenuItems.Add(this.menuXSectV);
		this.sideContextMenu.MenuItems.Add(this.menuXSectH);
		this.sideContextMenu.MenuItems.Add(this.menuSep7);
		this.sideContextMenu.MenuItems.Add(this.menuS2D);
		this.sideContextMenu.MenuItems.Add(this.menuS2M);
		this.sideContextMenu.MenuItems.Add(this.menuS2O);
		this.sideContextMenu.Popup += new System.EventHandler(sideContextMenu_Popup);
		resources.ApplyResources(this.menuDumpS, "menuDumpS");
		this.menuDumpS.Click += new System.EventHandler(menuDump_Click);
		resources.ApplyResources(this.menuSep23, "menuSep23");
		resources.ApplyResources(this.menuFlip, "menuFlip");
		this.menuFlip.Click += new System.EventHandler(menuFlip_Click);
		resources.ApplyResources(this.menuFlipAll, "menuFlipAll");
		this.menuFlipAll.Click += new System.EventHandler(menuFlipAll_Click);
		resources.ApplyResources(this.menuSep11, "menuSep11");
		this.menuOptS.MenuItems.Add(this.menuRasterS);
		this.menuOptS.MenuItems.Add(this.menuIdsS);
		this.menuOptS.MenuItems.Add(this.menuThickS);
		resources.ApplyResources(this.menuOptS, "menuOptS");
		resources.ApplyResources(this.menuRasterS, "menuRasterS");
		this.menuRasterS.Click += new System.EventHandler(menuRaster_Click);
		resources.ApplyResources(this.menuIdsS, "menuIdsS");
		this.menuIdsS.Click += new System.EventHandler(menuIdsS_Click);
		resources.ApplyResources(this.menuThickS, "menuThickS");
		this.menuThickS.Click += new System.EventHandler(menuThick_Click);
		resources.ApplyResources(this.menuSep17, "menuSep17");
		resources.ApplyResources(this.menuXSectV, "menuXSectV");
		this.menuXSectV.Click += new System.EventHandler(menuXSectV_Click);
		resources.ApplyResources(this.menuXSectH, "menuXSectH");
		this.menuXSectH.Click += new System.EventHandler(menuXSectH_Click);
		resources.ApplyResources(this.menuSep7, "menuSep7");
		resources.ApplyResources(this.menuS2D, "menuS2D");
		this.menuS2D.Click += new System.EventHandler(menuS2D_Click);
		resources.ApplyResources(this.menuS2M, "menuS2M");
		this.menuS2M.Click += new System.EventHandler(menuS2M_Click);
		resources.ApplyResources(this.menuS2O, "menuS2O");
		this.menuS2O.Click += new System.EventHandler(menuS2O_Click);
		resources.ApplyResources(this.menuItem1, "menuItem1");
		resources.ApplyResources(this.menuLOn, "menuLOn");
		this.menuLOn.Click += new System.EventHandler(menuLOn_Click);
		resources.ApplyResources(this.menuLOff, "menuLOff");
		this.menuLOff.Click += new System.EventHandler(menuLOff_Click);
		resources.ApplyResources(this.menuMeasure, "menuMeasure");
		this.menuMeasure.Click += new System.EventHandler(menuMeasure_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(96f, 96f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
		resources.ApplyResources(this, "$this");
		base.Controls.Add(this.toolBar);
		base.Controls.Add(this.panelPaint);
		base.Controls.Add(this.panelMap);
		base.Controls.Add(this.saveChanges);
		base.Controls.Add(this.panelData);
		base.KeyPreview = true;
		base.Menu = this.mainMenu;
		base.Name = "MainForm";
		base.Deactivate += new System.EventHandler(Form_Deactivate);
		base.Closed += new System.EventHandler(Form_Closed);
		base.Activated += new System.EventHandler(Form_Activated);
		base.Closing += new System.ComponentModel.CancelEventHandler(Form_Closing);
		base.KeyPress += new System.Windows.Forms.KeyPressEventHandler(Form_KeyPress);
		base.KeyDown += new System.Windows.Forms.KeyEventHandler(Form_KeyDown);
		this.panelPaint.ResumeLayout(false);
		this.panelMap.ResumeLayout(false);
		this.panelData.ResumeLayout(false);
		base.ResumeLayout(false);
	}

	static MainForm()
	{
		black = new Pen(Color.Black);
		gray = new Pen(Color.FromArgb(176, 176, 176));
		brown = new Pen(Color.Brown);
		blue = new Pen(Color.Blue);
		green = new Pen(Color.FromArgb(0, 192, 0));
		red = new Pen(Color.Red);
		orange = new Pen(Color.Orange);
		lgray = new Pen(Color.FromArgb(224, 224, 224));
		shotRd = new Pen(Color.Red);
		shotOr = new Pen(Color.Orange);
		shotDa = new Pen(Color.Red);
		shotXs = new Pen(Color.Red);
		shotXOr = new Pen(Color.Orange);
		dashed = new Pen(Color.LightGray);
		selected = new Pen(Color.FromArgb(224, 0, 255));
		selectedDa = new Pen(Color.FromArgb(224, 0, 255));
		blackBrush = new SolidBrush(Color.Black);
		grayBrush = new SolidBrush(Color.Gray);
		textFont = new Font("Tohama", 8f, FontStyle.Regular);
		labelFont = new Font("Tohama", 7f, FontStyle.Regular);
		pcLayout = false;
		dashed.DashStyle = DashStyle.Dash;
		shotDa.DashStyle = DashStyle.Dash;
		selectedDa.DashStyle = DashStyle.Dash;
	}

	internal MainForm()
	{
		InitializeComponent();
		CheckFramework();
		Config.Open();
		Trip.Reset();
		if (Config.titleVisible)
		{
			base.WindowState = FormWindowState.Normal;
		}
		if (base.CurrentAutoScaleDimensions.Width > 150f)
		{
			imageList.ImageSize = new Size(32, 32);
			Mapping.SetVga();
		}
		survey = new Survey(database, references, menu_Enable);
		undo = survey.undo;
		mapMap = new Mapping(mapView);
		outlineMap = new Mapping(sketchView);
		sideviewMap = new Mapping(sketchView);
		outline = new Drawing(outlineMap, side: false);
		outline.UndoChanged += menu_Enable;
		sideview = new Drawing(sideviewMap, side: true);
		sideview.UndoChanged += menu_Enable;
		dummy = new Drawing(null, side: false);
		actualList = null;
		dataSet = new DataSet(survey, mapMap, outline, sideview);
		Exporter.main = this;
		Exporter.menu = menuExport;
		Importer.main = this;
		Importer.menu = menuImport;
		MenuItem menuItem = new MenuItem();
		menuItem.Text = "Therion...";
		menuItem.Click += menuExpTherion_Click;
		menuExport.MenuItems.Add(menuItem);
		LoadAssemblies();
		moveLD.MouseDown += sketchView.MoveMouseDown;
		moveLD.MouseMove += sketchView.MoveMouseMove;
		moveLD.MouseUp += sketchView.MoveMouseUp;
		moveRD.MouseDown += sketchView.MoveMouseDown;
		moveRD.MouseMove += sketchView.MoveMouseMove;
		moveRD.MouseUp += sketchView.MoveMouseUp;
		moveLU.MouseDown += sketchView.MoveMouseDown;
		moveLU.MouseMove += sketchView.MoveMouseMove;
		moveLU.MouseUp += sketchView.MoveMouseUp;
		moveRU.MouseDown += sketchView.MoveMouseDown;
		moveRU.MouseMove += sketchView.MoveMouseMove;
		moveRU.MouseUp += sketchView.MoveMouseUp;
		mapView.survey = survey;
		mapView.map = mapMap;
		sketchView.survey = survey;
		sketchView.form = this;
		XSection.survey = survey;
		timer.Enabled = true;
		database.Add(new Station(ID.DEFAULT, ID.UNDEF, -1));
		references.Add(new Reference(ID.DEFAULT, default(MetricLocation)));
		calibForm = new CalibForm(this);
		dxfForm = new GraphicsForm(this);
		infoForm = new InfoForm();
		SwitchToData(1);
		if (Config.actualFile != "")
		{
			dataSet.LoadBackup();
			SelectLast(both: true);
		}
		survey.Invalidate();
		Power.Start();
	}

	private void CheckFramework()
	{
		SuspendLayout();
		Control control = toolBar;
		control.Dock = DockStyle.Bottom;
		panelData.Dock = DockStyle.Top;
		panelMap.Dock = DockStyle.Top;
		panelPaint.Dock = DockStyle.Top;
		ResumeLayout();
		database.AllowNew = false;
		references.AllowNew = false;
		PropertyInfo property = typeof(DataGridTableStyle).GetProperty("RowHeadersVisible");
		if (property != null)
		{
			property.SetValue(dataGridTableStyle1, false, null);
			property.SetValue(dataGridTableStyleRef, false, null);
		}
		EventInfo eventInfo = typeof(Control).GetEvent("MouseWheel");
		if (eventInfo != null)
		{
			pcLayout = true;
			eventInfo.AddEventHandler(mapView, new MouseEventHandler(MouseWheel));
			eventInfo.AddEventHandler(sketchView, new MouseEventHandler(MouseWheel));
		}
		typeof(Control).GetEvent("Resize")?.AddEventHandler(this, new EventHandler(SizeChanged));
	}

	private new void SizeChanged(object sender, EventArgs e)
	{
		Control control = toolBar;
		panelData.Height = control.Top;
		panelMap.Height = control.Top;
		panelPaint.Height = control.Top;
		Refresh();
	}

	private new void MouseWheel(object sender, MouseEventArgs e)
	{
		Type typeFromHandle = typeof(MouseEventArgs);
		PropertyInfo property = typeFromHandle.GetProperty("Delta");
		int inc = (int)property.GetValue(e, null);
		PlusMinus(inc);
	}

	private void CheckAssembly(Assembly ass)
	{
		Type[] types = ass.GetTypes();
		for (int i = 0; i < types.Length; i++)
		{
			if (types[i].IsSubclassOf(typeof(Extension)))
			{
				try
				{
					Extension extension = (Extension)ass.CreateInstance(types[i].FullName);
					extension.next = extensions;
					extensions = extension;
					extension.Install();
				}
				catch
				{
				}
			}
		}
	}

	private void LoadAssemblies()
	{
		Assembly executingAssembly = Assembly.GetExecutingAssembly();
		CheckAssembly(executingAssembly);
		string directoryName = Path.GetDirectoryName(executingAssembly.ManifestModule.FullyQualifiedName);
		string[] files = Directory.GetFiles(directoryName, "*.dll");
		for (int i = 0; i < files.Length; i++)
		{
			Assembly assembly = Assembly.LoadFrom(files[i]);
			if (assembly != null)
			{
				CheckAssembly(assembly);
			}
		}
	}

	internal void SelectLast(bool both)
	{
		try
		{
			dataGrid.CurrentCell = new DataGridCell(database.Count - 1, 1);
			if (both)
			{
				dataGridRef.CurrentCell = new DataGridCell(references.Count - 1, 0);
			}
		}
		catch
		{
		}
	}

	private void SwitchToData(int sel)
	{
		CloseGridEdit(save: false);
		toolBarMap.Pushed = false;
		toolBarSketch.Pushed = false;
		panelMap.Visible = false;
		panelPaint.Visible = false;
		actualList = null;
		actualMap = null;
		panelData.Visible = true;
		switch (sel)
		{
		case 1:
			toolBarData.ImageIndex = 0;
			break;
		case 2:
			toolBarData.ImageIndex = 1;
			break;
		case 3:
			toolBarData.ImageIndex = 1 - toolBarData.ImageIndex;
			break;
		}
		if (toolBarData.ImageIndex == 1)
		{
			dataGridRef.Visible = true;
			activeGrid = dataGridRef;
			numColumns = 5;
		}
		else
		{
			dataGridRef.Visible = false;
			activeGrid = dataGrid;
			numColumns = 6;
		}
		toolBarData.Pushed = true;
		activeGrid.Focus();
		menu_Enable(null, EventArgs.Empty);
	}

	private void SwitchToData(Station at)
	{
		if (at == null)
		{
			SwitchToData(1);
		}
		else if (at is Reference)
		{
			SwitchToData(2);
			dataGridRef.CurrentCell = new DataGridCell(at.idx, 0);
		}
		else
		{
			SwitchToData(1);
			int c = ((at.to == at.num) ? 1 : 0);
			dataGrid.CurrentCell = new DataGridCell(at.idx, c);
		}
	}

	private void SwitchToMap()
	{
		CloseGridEdit(save: false);
		toolBarData.Pushed = false;
		toolBarSketch.Pushed = false;
		panelData.Visible = false;
		panelPaint.Visible = false;
		activeGrid = null;
		actualList = dummy;
		actualMap = mapMap;
		panelMap.Visible = true;
		toolBarMap.Pushed = true;
		menu_Enable(null, EventArgs.Empty);
		mapView.Setup();
		mapView.Focus();
	}

	private void SwitchToMap(Station at)
	{
		if (at != null)
		{
			survey.Update();
			mapMap.SetOrigin(at.x, at.y);
		}
		SwitchToMap();
	}

	private void SwitchToSketch(int sel)
	{
		CloseGridEdit(save: false);
		toolBarData.Pushed = false;
		toolBarMap.Pushed = false;
		panelData.Visible = false;
		panelMap.Visible = false;
		activeGrid = null;
		switch (sel)
		{
		case 1:
			toolBarSketch.ImageIndex = 3;
			break;
		case 2:
			toolBarSketch.ImageIndex = 4;
			break;
		case 3:
			toolBarSketch.ImageIndex = 7 - toolBarSketch.ImageIndex;
			break;
		}
		if (toolBarSketch.ImageIndex == 3)
		{
			sketchView.ContextMenu = (sketchView.cornerMove ? null : planContextMenu);
			actualList = outline;
			actualMap = outlineMap;
			sketchView.Setup(outline, outlineMap, side: false);
		}
		else
		{
			sketchView.ContextMenu = (sketchView.cornerMove ? null : sideContextMenu);
			actualList = sideview;
			actualMap = sideviewMap;
			sketchView.Setup(sideview, sideviewMap, side: true);
		}
		toolBarSketch.Pushed = true;
		panelPaint.Visible = true;
		menu_Enable(null, EventArgs.Empty);
		sketchView.Invalidate();
		sketchView.Focus();
	}

	private void SwitchToSketch(Station at, int sel)
	{
		if (at != null)
		{
			survey.Update();
			outlineMap.SetOrigin(at.x, at.y);
			sideviewMap.SetOrigin(at.d, at.h);
		}
		SwitchToSketch(sel);
	}

	private DialogResult SaveFile(string name, bool ask)
	{
		DialogResult dialogResult = DialogResult.OK;
		try
		{
			if (ask || !Path.HasExtension(name))
			{
				saveFileDialog.FileName = Config.actualFile;
				dialogResult = saveFileDialog.ShowDialog();
				name = saveFileDialog.FileName;
				if (dialogResult == DialogResult.OK && Config.actualFile != name)
				{
					Config.actualFile = name;
				}
			}
			if (dialogResult == DialogResult.OK)
			{
				dataSet.Write(name);
				DataSet.Reset();
			}
		}
		catch
		{
			dialogResult = DialogResult.Abort;
		}
		return dialogResult;
	}

	internal bool SaveDialog()
	{
		if (DataSet.Dirty)
		{
			DialogResult dialogResult = MessageBox.Show(saveChanges.Text, Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
			if (dialogResult == DialogResult.Yes)
			{
				dialogResult = SaveFile(Config.actualFile, ask: false);
			}
			if (dialogResult == DialogResult.Cancel)
			{
				return false;
			}
		}
		return true;
	}

	private void timer_Tick(object sender, EventArgs e)
	{
		if (Config.portName != "" && Link.Poll(out var dist, out var azimuth, out var incl, out var roll))
		{
			Station act = new Station(ID.UNDEF, ID.UNDEF, dist, azimuth, incl, roll, Trip.NewIdx());
			survey.AppendNew(act);
			sketchView.Invalidate();
			mapView.Invalidate();
			if (dataGrid.CurrentCell.RowNumber == database.Count - 2)
			{
				SelectLast(both: false);
			}
		}
		dataSet.CheckBackup();
	}

	private void Form_Activated(object sender, EventArgs e)
	{
		Link.appActive = true;
		if (!Config.titleVisible)
		{
			base.WindowState = FormWindowState.Maximized;
		}
	}

	private void Form_Deactivate(object sender, EventArgs e)
	{
		Link.appActive = false;
		base.WindowState = FormWindowState.Normal;
	}

	private void Form_Closed(object sender, EventArgs e)
	{
		Power.Stop();
		DataSet.Reset();
	}

	private void Form_Closing(object sender, CancelEventArgs e)
	{
		if (SaveDialog() && calibForm.SaveDialog())
		{
			Link.Cleanup();
		}
		else
		{
			e.Cancel = true;
		}
	}

	private void Form_KeyPress(object sender, KeyPressEventArgs e)
	{
	}

	private void Form_KeyDown(object sender, KeyEventArgs e)
	{
		switch (e.KeyCode)
		{
		case Keys.Return:
			if (textData.Visible)
			{
				CloseGridEdit(save: true);
			}
			break;
		case Keys.Tab:
			MoveGridEdit();
			break;
		case Keys.Up:
			if (actualMap != null)
			{
				actualMap.MoveOrigin(0, -(sketchView.Width >> 3));
			}
			break;
		case Keys.Down:
			if (actualMap != null)
			{
				actualMap.MoveOrigin(0, sketchView.Width >> 3);
			}
			break;
		case Keys.Left:
			if (actualMap != null)
			{
				actualMap.MoveOrigin(-(sketchView.Width >> 3), 0);
			}
			break;
		case Keys.Right:
			if (actualMap != null)
			{
				actualMap.MoveOrigin(sketchView.Width >> 3, 0);
			}
			break;
		case Keys.Add:
			PlusMinus(1);
			break;
		case Keys.Subtract:
			PlusMinus(-1);
			break;
		case Keys.D1:
			if (panelPaint.Visible)
			{
				symbol_Click(pictMove, EventArgs.Empty);
			}
			break;
		case Keys.D2:
			if (panelPaint.Visible)
			{
				symbol_Click(pictBlack, EventArgs.Empty);
			}
			break;
		case Keys.D3:
			if (panelPaint.Visible)
			{
				symbol_Click(pictGray, EventArgs.Empty);
			}
			break;
		case Keys.D4:
			if (panelPaint.Visible)
			{
				symbol_Click(pictBrown, EventArgs.Empty);
			}
			break;
		case Keys.D5:
			if (panelPaint.Visible)
			{
				symbol_Click(pictBlue, EventArgs.Empty);
			}
			break;
		case Keys.D6:
			if (panelPaint.Visible)
			{
				symbol_Click(pictRed, EventArgs.Empty);
			}
			break;
		case Keys.D7:
			if (panelPaint.Visible)
			{
				symbol_Click(pictOrange, EventArgs.Empty);
			}
			break;
		case Keys.D8:
			if (panelPaint.Visible)
			{
				symbol_Click(pictGreen, EventArgs.Empty);
			}
			break;
		case Keys.D9:
			if (panelPaint.Visible)
			{
				symbol_Click(null, EventArgs.Empty);
			}
			break;
		case Keys.D:
			if (panelPaint.Visible)
			{
				symbol_Click(pictDelete, EventArgs.Empty);
			}
			break;
		}
	}

	private void dataGrid_MouseDown(object sender, MouseEventArgs e)
	{
		CloseGridEdit(save: true);
		try
		{
			activeCell = activeGrid.CurrentCell;
			if (activeGrid.GetCellBounds(activeCell.RowNumber, activeCell.ColumnNumber).Contains(e.X, e.Y))
			{
				cellClicked = true;
			}
		}
		catch
		{
		}
	}

	private void dataGrid_Click(object sender, EventArgs e)
	{
		if (cellClicked)
		{
			OpenGridEdit();
		}
	}

	private void dataGrid_DoubleClick(object sender, EventArgs e)
	{
		if (cellClicked)
		{
			OpenGridEdit();
			return;
		}
		contextPos = activeGrid.PointToClient(Control.MousePosition);
		DataGrid.HitTestInfo hitTestInfo = activeGrid.HitTest(contextPos.X, contextPos.Y);
		if (hitTestInfo.Type != DataGrid.HitTestType.ColumnHeader)
		{
			return;
		}
		try
		{
			activeGrid.CurrentCell = new DataGridCell(((BindingSource)activeGrid.DataSource).Count - 1, 0);
		}
		catch
		{
		}
	}

	private void dataGrid_CurrentCellChanged(object sender, EventArgs e)
	{
		CloseGridEdit(save: true);
		survey.selectedRow = Survey.First(database, dataGrid.CurrentCell.RowNumber, rev: false);
	}

	private void OpenGridEdit()
	{
		if (activeGrid == null)
		{
			return;
		}
		BindingSource bindingSource = (BindingSource)activeGrid.DataSource;
		textComm.ReadOnly = false;
		if (activeCell.ColumnNumber == numColumns - 1)
		{
			textComm.Text = ((Station)bindingSource[activeCell.RowNumber]).Get(5);
			if (Survey.Writable(bindingSource, activeCell.RowNumber))
			{
				try
				{
					Input.Enable();
				}
				catch
				{
				}
				textComm.SelectionStart = 0;
				textComm.SelectionLength = textComm.TextLength;
			}
			else
			{
				textComm.ReadOnly = true;
			}
			textComm.Visible = true;
			textComm.Focus();
		}
		else if (Survey.Writable(bindingSource, activeCell.RowNumber))
		{
			Rectangle cellBounds = activeGrid.GetCellBounds(activeCell.RowNumber, activeCell.ColumnNumber);
			int height = textData.Size.Height;
			int num = cellBounds.Top - (height - cellBounds.Height >> 1);
			int num2 = 0;
			try
			{
				num2 = Input.Enable();
			}
			catch
			{
			}
			if (num + height > num2)
			{
				num = num2 - height - 1;
			}
			textData.Location = new Point(Math.Max(cellBounds.Left - 4, 0), num);
			textData.Size = new Size(cellBounds.Width + 8, height);
			textData.Text = (string)activeGrid[activeCell.RowNumber, activeCell.ColumnNumber];
			textData.Visible = true;
			textData.Focus();
			textData.SelectionStart = 0;
			textData.SelectionLength = textData.TextLength;
		}
		toolBarUndo.Enabled = true;
		toolBarRedo.Enabled = false;
		toolBarPlus.Enabled = false;
		toolBarMinus.Enabled = false;
	}

	private void CloseGridEdit(bool save)
	{
		if (activeGrid != null)
		{
			BindingSource data = (BindingSource)activeGrid.DataSource;
			if (textData.Visible)
			{
				if (save)
				{
					Station.error = false;
					survey.Change(data, activeCell.RowNumber, activeCell.ColumnNumber, textData.Text);
					if (Station.error)
					{
						return;
					}
				}
				textData.Visible = false;
				try
				{
					Input.Disable();
				}
				catch
				{
				}
			}
			else if (textComm.Visible)
			{
				if (save)
				{
					survey.Change(data, activeCell.RowNumber, 5, textComm.Text);
				}
				textComm.Visible = false;
				try
				{
					Input.Disable();
				}
				catch
				{
				}
			}
		}
		menu_Enable(null, EventArgs.Empty);
		toolBarPlus.Enabled = true;
		toolBarMinus.Enabled = true;
		cellClicked = false;
	}

	private void MoveGridEdit()
	{
		if (textData.Visible)
		{
			CloseGridEdit(save: true);
			activeCell.ColumnNumber += 1;
			if (activeCell.ColumnNumber >= numColumns - 1)
			{
				activeCell.ColumnNumber = 0;
				activeCell.RowNumber += 1;
				if (activeGrid == dataGridRef)
				{
					if (activeCell.RowNumber >= references.Count)
					{
						survey.Append(references, new Reference(ID.UNDEF, default(MetricLocation)));
					}
				}
				else if (activeCell.RowNumber >= database.Count)
				{
					survey.Append(database, new Station(ID.UNDEF, ID.UNDEF, Trip.NewIdx()));
				}
			}
			activeGrid.CurrentCell = activeCell;
			OpenGridEdit();
		}
		cellClicked = false;
	}

	private void menu_Enable(object sender, EventArgs e)
	{
		if (actualList != null)
		{
			toolBarUndo.Enabled = actualList.CanUndo();
			toolBarRedo.Enabled = actualList.CanRedo();
		}
		else
		{
			toolBarUndo.Enabled = undo.CanUndo();
			toolBarRedo.Enabled = undo.CanRedo();
		}
		if (sender != null)
		{
			DataSet.MakeDirty();
			if (sender == database)
			{
				SwitchToData(1);
			}
			if (sender == references)
			{
				SwitchToData(2);
			}
		}
	}

	private void PlusMinus(int inc)
	{
		if (panelData.Visible)
		{
			if (activeGrid == dataGrid)
			{
				DataGridCell currentCell = activeGrid.CurrentCell;
				survey.Increment(currentCell.RowNumber, currentCell.ColumnNumber, inc);
			}
		}
		else if (inc > 0)
		{
			actualMap.Larger();
		}
		else if (inc < 0)
		{
			actualMap.Smaller();
		}
	}

	private void toolBar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
	{
		ToolBarButton button = e.Button;
		if (button == toolBarPlus)
		{
			PlusMinus(1);
		}
		else if (button == toolBarMinus)
		{
			PlusMinus(-1);
		}
		else if (button == toolBarUndo)
		{
			if (textComm.Visible || textData.Visible)
			{
				CloseGridEdit(save: false);
			}
			else if (actualList != null)
			{
				actualList.Undo(sketchView);
			}
			else
			{
				undo.DoUndo();
			}
		}
		else if (button == toolBarRedo)
		{
			if (actualList != null)
			{
				actualList.Redo(sketchView);
			}
			else
			{
				undo.DoRedo();
			}
		}
		else if (button == toolBarData)
		{
			if (toolBarData.Pushed)
			{
				SwitchToData(3);
			}
			else
			{
				SwitchToData(0);
			}
		}
		else if (button == toolBarMap)
		{
			SwitchToMap();
		}
		else if (button == toolBarSketch)
		{
			if (toolBarSketch.Pushed)
			{
				SwitchToSketch(3);
			}
			else
			{
				SwitchToSketch(0);
			}
		}
	}

	private void symbol_Click(object sender, EventArgs e)
	{
		PictureBox pictureBox = (PictureBox)sender;
		pictSelBlack.Visible = false;
		pictSelGray.Visible = false;
		pictSelBrown.Visible = false;
		pictSelBlue.Visible = false;
		pictSelRed.Visible = false;
		pictSelOrange.Visible = false;
		pictSelGreen.Visible = false;
		pictSelDelete.Visible = false;
		pictSelMove.Visible = false;
		sketchView.erase = false;
		sketchView.offset = false;
		sketchView.pen = null;
		sketchView.cornerMove = true;
		XSection.actual = null;
		if (pictureBox == pictBlack)
		{
			sketchView.pen = black;
			pictSelBlack.Visible = true;
		}
		else if (pictureBox == pictGray)
		{
			sketchView.pen = gray;
			pictSelGray.Visible = true;
		}
		else if (pictureBox == pictBrown)
		{
			sketchView.pen = brown;
			pictSelBrown.Visible = true;
		}
		else if (pictureBox == pictBlue)
		{
			sketchView.pen = blue;
			pictSelBlue.Visible = true;
		}
		else if (pictureBox == pictRed)
		{
			sketchView.pen = red;
			pictSelRed.Visible = true;
		}
		else if (pictureBox == pictOrange)
		{
			sketchView.pen = orange;
			pictSelOrange.Visible = true;
		}
		else if (pictureBox == pictGreen)
		{
			sketchView.pen = green;
			pictSelGreen.Visible = true;
		}
		else if (pictureBox == pictDelete)
		{
			sketchView.erase = true;
			pictSelDelete.Visible = true;
		}
		else if (pictureBox == pictMove)
		{
			sketchView.cornerMove = false;
			pictSelMove.Visible = true;
		}
		else if (pictureBox == null)
		{
			sketchView.offset = true;
		}
		sketchView.ContextMenu = (sketchView.cornerMove ? null : ((actualList == sideview) ? sideContextMenu : planContextMenu));
		moveLD.Visible = sketchView.cornerMove;
		moveRD.Visible = sketchView.cornerMove;
		moveLU.Visible = sketchView.cornerMove;
		moveRU.Visible = sketchView.cornerMove;
		sketchView.ResetFocus();
	}

	private void menuOpt_Popup(object sender, EventArgs e)
	{
		menuTitle.Checked = Config.titleVisible;
		menuSmart.Checked = Config.smartMode;
		menuReverse.Checked = Config.reverse;
		menuDeg.Checked = Config.fullCircle == 360;
		menuGrad.Checked = Config.fullCircle == 400;
		menuMeter.Checked = Config.unit == 1000.0;
		menuFeet.Checked = Config.unit == 304.79999999999995;
		string portName = Config.portName;
		int num = 0;
		for (int i = 0; i < menuPort.MenuItems.Count; i++)
		{
			MenuItem menuItem = menuPort.MenuItems[i];
			menuItem.Checked = menuItem.Text == portName;
			if (menuItem.Checked)
			{
				num++;
			}
		}
		menuNone.Checked = portName == "";
		menuComX.Checked = num == 0 && !menuNone.Checked;
	}

	private void menuNone_Click(object sender, EventArgs e)
	{
		Config.portName = "";
		Link.Close();
	}

	private void menuCom_Click(object sender, EventArgs e)
	{
		Config.portName = ((MenuItem)sender).Text;
		Link.Close();
	}

	private void menuComX_Click(object sender, EventArgs e)
	{
		Form form = new PortForm();
		form.ShowDialog();
	}

	private void menuTitle_Click(object sender, EventArgs e)
	{
		if (base.WindowState == FormWindowState.Maximized)
		{
			base.WindowState = FormWindowState.Normal;
			Config.titleVisible = true;
		}
		else
		{
			base.WindowState = FormWindowState.Maximized;
			Config.titleVisible = false;
		}
	}

	private void menuSmart_Click(object sender, EventArgs e)
	{
		Config.smartMode = !Config.smartMode;
	}

	private void menuReverse_Click(object sender, EventArgs e)
	{
		Config.reverse = !Config.reverse;
	}

	private void menuAll_Click(object sender, EventArgs e)
	{
		Config.paintAll = !Config.paintAll;
		sketchView.Invalidate();
	}

	private void menuDeg_Click(object sender, EventArgs e)
	{
		Config.fullCircle = 360;
		dataGrid.Invalidate();
	}

	private void menuGrad_Click(object sender, EventArgs e)
	{
		Config.fullCircle = 400;
		dataGrid.Invalidate();
	}

	private void menuMeter_Click(object sender, EventArgs e)
	{
		Config.metricUnits = true;
		dataGrid.Invalidate();
		mapView.Invalidate();
		sketchView.Invalidate();
	}

	private void menuFeet_Click(object sender, EventArgs e)
	{
		Config.metricUnits = false;
		dataGrid.Invalidate();
		mapView.Invalidate();
		sketchView.Invalidate();
	}

	private void menuMenu_Popup(object sender, EventArgs e)
	{
		if (textData.Visible || textComm.Visible)
		{
			menuUndo.Enabled = true;
			menuRedo.Enabled = false;
		}
		else if (actualList != null)
		{
			menuUndo.Enabled = actualList.CanUndo();
			menuRedo.Enabled = actualList.CanRedo();
		}
		else
		{
			menuUndo.Enabled = undo.CanUndo();
			menuRedo.Enabled = undo.CanRedo();
		}
		menuBT_Popup(null, e);
		menuOpt_Popup(null, e);
	}

	private void menuExit_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void menuUndo_Click(object sender, EventArgs e)
	{
		if (textData.Visible || textComm.Visible)
		{
			CloseGridEdit(save: false);
		}
		else if (actualList != null)
		{
			actualList.Undo(sketchView);
		}
		else
		{
			undo.DoUndo();
		}
	}

	private void menuRedo_Click(object sender, EventArgs e)
	{
		if (actualList != null)
		{
			actualList.Redo(sketchView);
		}
		else
		{
			undo.DoRedo();
		}
	}

	private void menuNewCave_Click(object sender, EventArgs e)
	{
		if (SaveDialog())
		{
			survey.Reset();
			survey.SetBoundary();
			Trip.Reset();
			DataSet.Reset();
			database.Add(new Station(ID.DEFAULT, ID.UNDEF, -1));
			survey.Invalidate();
			outline.Reset(resetMapping: true);
			sideview.Reset(resetMapping: true);
			Config.actualFile = "";
			sketchView.Invalidate();
			mapView.Invalidate();
			mapView.map.Reset();
		}
	}

	private void menuNew_Click(object sender, EventArgs e)
	{
		if (SaveDialog())
		{
			if (Config.actualFile == "")
			{
				menuNewCave_Click(sender, e);
				return;
			}
			Config.actualFile = Path.Combine(DataSet.ActualDirectory(), "new");
			Cursor.Current = Cursors.WaitCursor;
			dataSet.Load("");
			DataSet.Reset();
			outline.Reset(resetMapping: false);
			sideview.Reset(resetMapping: false);
			SelectLast(both: true);
			sketchView.Invalidate();
			mapView.Invalidate();
			Cursor.Current = null;
		}
	}

	private void menuOpen_Click(object sender, EventArgs e)
	{
		if (!SaveDialog())
		{
			return;
		}
		openFileDialog.FileName = Config.actualFile;
		openFileDialog.InitialDirectory = DataSet.ActualDirectory();
		if (openFileDialog.ShowDialog() == DialogResult.OK)
		{
			string fileName = openFileDialog.FileName;
			if (Config.actualFile != fileName)
			{
				Config.actualFile = fileName;
			}
			Cursor.Current = Cursors.WaitCursor;
			if (dataSet.Load(fileName))
			{
				DataSet.Reset();
				SelectLast(both: true);
				sketchView.Invalidate();
				mapView.Invalidate();
			}
			else
			{
				MessageBox.Show(fileError.Text + fileName);
			}
			Cursor.Current = null;
		}
	}

	private void menuSave_Click(object sender, EventArgs e)
	{
		SaveFile(Config.actualFile, ask: false);
	}

	private void menuSaveAs_Click(object sender, EventArgs e)
	{
		SaveFile(Config.actualFile, ask: true);
	}

	private void menuCalib_Click(object sender, EventArgs e)
	{
		calibForm.ShowDialog();
	}

	private void menuInfo_Click(object sender, EventArgs e)
	{
		infoForm.ShowDialog();
	}

	private void menuTrip_Click(object sender, EventArgs e)
	{
		Form form = new TripForm(null, survey, writable: true);
		form.ShowDialog();
	}

	private void dataContextMenu_Popup(object sender, EventArgs e)
	{
		contextPos = dataGrid.PointToClient(Control.MousePosition);
		DataGrid.HitTestInfo hitTestInfo = dataGrid.HitTest(contextPos.X, contextPos.Y);
		int row = hitTestInfo.Row;
		contextStation = null;
		if (row >= 0)
		{
			contextStation = (Station)database[Survey.First(database, row, rev: true)];
		}
		menuComment.Enabled = row >= 0;
		menuSTrip.Enabled = row >= 0 && ((Station)database[row]).trip >= 0;
		menuDelete.Enabled = Survey.Writable(database, row);
		menuInsert.Enabled = (row < 0 && hitTestInfo.Type != DataGrid.HitTestType.ColumnHeader) || Survey.Writable(database, row);
		menuCopy.Enabled = row >= 0;
		menuPaste.Enabled = clipboard != null && Survey.Writable(database, row);
		menuToBranch.Enabled = Survey.Writable(database, row);
		if (survey.ToBack(row))
		{
			menuToBranch.Text = menuToBranch.Text.Replace("->", "<-");
		}
		else
		{
			menuToBranch.Text = menuToBranch.Text.Replace("<-", "->");
		}
		menuRenum.Enabled = Survey.Writable(database, row);
		menuStart.Enabled = row >= 0;
		menuCont.Enabled = row >= 0;
		menuEnd.Enabled = row >= 0;
		menuD2M.Enabled = contextStation != null;
		menuD2O.Enabled = contextStation != null && (Config.paintAll || !contextStation.readOnly);
		menuD2S.Enabled = contextStation != null && !contextStation.readOnly;
	}

	private void refContextMenu_Popup(object sender, EventArgs e)
	{
		contextPos = dataGridRef.PointToClient(Control.MousePosition);
		DataGrid.HitTestInfo hitTestInfo = dataGridRef.HitTest(contextPos.X, contextPos.Y);
		int row = hitTestInfo.Row;
		contextStation = null;
		if (row >= 0)
		{
			contextStation = (Reference)references[row];
		}
		menuRComm.Enabled = row >= 0;
		menuRDelete.Enabled = Survey.Writable(references, row);
		menuRInsert.Enabled = (row < 0 && hitTestInfo.Type != DataGrid.HitTestType.ColumnHeader) || Survey.Writable(references, row);
		menuRCopy.Enabled = row >= 0;
		menuRPaste.Enabled = clipboard != null && Survey.Writable(references, row);
		menuRStart.Enabled = row >= 0;
		menuR2M.Enabled = contextStation != null;
		menuR2O.Enabled = contextStation != null && (Config.paintAll || !contextStation.readOnly);
		menuR2S.Enabled = contextStation != null && !contextStation.readOnly;
	}

	private void menuComment_Click(object sender, EventArgs e)
	{
		activeCell.RowNumber = dataGrid.HitTest(contextPos.X, contextPos.Y).Row;
		if (activeCell.RowNumber >= 0)
		{
			activeCell.ColumnNumber = 5;
			OpenGridEdit();
		}
	}

	private void menuRComment_Click(object sender, EventArgs e)
	{
		activeCell.RowNumber = dataGridRef.HitTest(contextPos.X, contextPos.Y).Row;
		if (activeCell.RowNumber >= 0)
		{
			activeCell.ColumnNumber = 4;
			OpenGridEdit();
		}
	}

	private void menuSTrip_Click(object sender, EventArgs e)
	{
		int row = dataGrid.HitTest(contextPos.X, contextPos.Y).Row;
		if (row >= 0)
		{
			Station station = (Station)database[row];
			if (station.trip >= 0)
			{
				Form form = new TripForm(Trip.ByIndex(station.trip), survey, Survey.Writable(database, row));
				form.ShowDialog();
			}
		}
	}

	private void menuToBranch_Click(object sender, EventArgs e)
	{
		DataGrid.HitTestInfo hitTestInfo = dataGrid.HitTest(contextPos.X, contextPos.Y);
		if (hitTestInfo.Row >= 0)
		{
			survey.TurnToBranch(hitTestInfo.Row);
		}
	}

	private void menuRenum_Click(object sender, EventArgs e)
	{
		DataGrid.HitTestInfo hitTestInfo = dataGrid.HitTest(contextPos.X, contextPos.Y);
		if (hitTestInfo.Row > 0)
		{
			survey.Renumber(hitTestInfo.Row);
		}
	}

	private void menuStart_Click(object sender, EventArgs e)
	{
		DataGrid.HitTestInfo hitTestInfo = dataGrid.HitTest(contextPos.X, contextPos.Y);
		if (hitTestInfo.Row >= 0)
		{
			Station station = (Station)database[hitTestInfo.Row];
			ID iD = station.num;
			if (hitTestInfo.Column == 0 && station.from != ID.UNDEF)
			{
				iD = station.from;
			}
			if (hitTestInfo.Column == 1 && station.to != ID.UNDEF)
			{
				iD = station.to;
			}
			Station s = new Station(iD, ID.NewStart(survey, iD), Trip.NewIdx());
			survey.Append(database, s);
			SelectLast(both: false);
		}
	}

	private void menuRStart_Click(object sender, EventArgs e)
	{
		DataGrid.HitTestInfo hitTestInfo = dataGridRef.HitTest(contextPos.X, contextPos.Y);
		if (hitTestInfo.Row >= 0)
		{
			Reference reference = (Reference)references[hitTestInfo.Row];
			Station s = new Station(reference.num, ID.NewStart(survey, reference.num), Trip.NewIdx());
			survey.Append(database, s);
			SwitchToData(1);
			SelectLast(both: false);
		}
	}

	private void menuCont_Click(object sender, EventArgs e)
	{
		DataGrid.HitTestInfo hitTestInfo = dataGrid.HitTest(contextPos.X, contextPos.Y);
		if (hitTestInfo.Row >= 0)
		{
			Station station = (Station)database[hitTestInfo.Row];
			ID f = station.num;
			if (hitTestInfo.Column == 0 && station.from != ID.UNDEF)
			{
				f = station.from;
			}
			if (hitTestInfo.Column == 1 && station.to != ID.UNDEF)
			{
				f = station.to;
			}
			Station s = new Station(f, ID.UNDEF, Trip.NewIdx());
			survey.Append(database, s);
			SelectLast(both: false);
		}
	}

	private void menuEnd_Click(object sender, EventArgs e)
	{
		DataGrid.HitTestInfo hitTestInfo = dataGrid.HitTest(contextPos.X, contextPos.Y);
		if (hitTestInfo.Row >= 0)
		{
			Station station = (Station)database[hitTestInfo.Row];
			ID t = station.num;
			if (hitTestInfo.Column == 0 && station.from != ID.UNDEF)
			{
				t = station.from;
			}
			if (hitTestInfo.Column == 1 && station.to != ID.UNDEF)
			{
				t = station.to;
			}
			int count = database.Count;
			if (count > 0)
			{
				Station station2 = (Station)database[count - 1];
				Station s = new Station(station2.ActualId(), t, Trip.NewIdx());
				survey.Append(database, s);
				SelectLast(both: false);
			}
		}
	}

	private void menuInsert_Click(object sender, EventArgs e)
	{
		DataGrid.HitTestInfo hitTestInfo = dataGrid.HitTest(contextPos.X, contextPos.Y);
		int num = hitTestInfo.Row;
		if ((num < 0) & (hitTestInfo.Type == DataGrid.HitTestType.ColumnHeader))
		{
			num = 0;
		}
		Station station;
		if (num >= 0)
		{
			station = new Station(ID.UNDEF, ID.UNDEF, ((Station)database[num]).trip);
			survey.Insert(database, station, num);
		}
		else
		{
			station = new Station(ID.UNDEF, ID.UNDEF, Trip.NewIdx());
			num = database.Count;
			survey.Append(database, station);
		}
		if (num >= 0)
		{
			if (((Station)database[num]).flipped)
			{
				station.flipped = true;
			}
			if (((Station)database[num]).projected)
			{
				station.projected = true;
			}
		}
	}

	private void menuRInsert_Click(object sender, EventArgs e)
	{
		DataGrid.HitTestInfo hitTestInfo = dataGridRef.HitTest(contextPos.X, contextPos.Y);
		int num = hitTestInfo.Row;
		if ((num < 0) & (hitTestInfo.Type == DataGrid.HitTestType.ColumnHeader))
		{
			num = 0;
		}
		Reference s = new Reference(ID.UNDEF, default(MetricLocation));
		if (num >= 0)
		{
			survey.Insert(references, s, num);
		}
		else
		{
			survey.Append(references, s);
		}
	}

	private void menuDelete_Click(object sender, EventArgs e)
	{
		int row = dataGrid.HitTest(contextPos.X, contextPos.Y).Row;
		if (row >= 0)
		{
			survey.Delete(database, row);
		}
	}

	private void menuRDelete_Click(object sender, EventArgs e)
	{
		int row = dataGridRef.HitTest(contextPos.X, contextPos.Y).Row;
		if (row >= 0)
		{
			survey.Delete(references, row);
		}
	}

	private void menuCopy_Click(object sender, EventArgs e)
	{
		DataGrid.HitTestInfo hitTestInfo = dataGrid.HitTest(contextPos.X, contextPos.Y);
		if (hitTestInfo.Row >= 0)
		{
			clipboard = ((Station)database[hitTestInfo.Row]).Get(hitTestInfo.Column);
		}
	}

	private void menuRCopy_Click(object sender, EventArgs e)
	{
		DataGrid.HitTestInfo hitTestInfo = dataGridRef.HitTest(contextPos.X, contextPos.Y);
		if (hitTestInfo.Row >= 0)
		{
			clipboard = ((Reference)references[hitTestInfo.Row]).Get(hitTestInfo.Column);
		}
	}

	private void menuPaste_Click(object sender, EventArgs e)
	{
		DataGrid.HitTestInfo hitTestInfo = dataGrid.HitTest(contextPos.X, contextPos.Y);
		if (clipboard != null && Survey.Writable(database, hitTestInfo.Row))
		{
			survey.Change(database, hitTestInfo.Row, hitTestInfo.Column, clipboard);
			dataGrid.CurrentCell = new DataGridCell(hitTestInfo.Row, hitTestInfo.Column);
		}
	}

	private void menuRPaste_Click(object sender, EventArgs e)
	{
		DataGrid.HitTestInfo hitTestInfo = dataGridRef.HitTest(contextPos.X, contextPos.Y);
		if (clipboard != null && Survey.Writable(references, hitTestInfo.Row))
		{
			survey.Change(references, hitTestInfo.Row, hitTestInfo.Column, clipboard);
			dataGridRef.CurrentCell = new DataGridCell(hitTestInfo.Row, hitTestInfo.Column);
		}
	}

	private void mapContextMenu_Popup(object sender, EventArgs e)
	{
		contextPos = mapView.PointToClient(Control.MousePosition);
		actualMap.Screen2World(contextPos.X, contextPos.Y, out var wx, out var wy);
		contextStation = survey.Nearest(wx, wy, actualMap.Delta, all: true, null, mapView.tapped);
		menuIds.Checked = Config.showMapId;
		menuThick.Checked = Config.thickLines;
		menuM2D.Enabled = contextStation != null;
		menuM2O.Enabled = contextStation != null && (Config.paintAll || !contextStation.readOnly);
		menuM2S.Enabled = contextStation != null && !contextStation.readOnly;
	}

	private void outlineContextMenu_Popup(object sender, EventArgs e)
	{
		contextPos = sketchView.PointToClient(Control.MousePosition);
		actualMap.Screen2World(contextPos.X, contextPos.Y, out var wx, out var wy);
		contextStation = survey.Nearest(wx, wy, actualMap.Delta, Config.paintAll, null, sketchView.tapped);
		menuRaster.Checked = Config.rasterVisible;
		menuAll.Checked = Config.paintAll;
		menuIdsP.Checked = Config.showSketchId;
		menuThickP.Checked = Config.thickLines;
		menuXSect.Enabled = contextStation != null && !contextStation.readOnly;
		MenuItem menuItem = menuO2D;
		bool enabled = (menuO2M.Enabled = contextStation != null);
		menuItem.Enabled = enabled;
		menuO2S.Enabled = contextStation != null && !contextStation.readOnly;
	}

	private void sideContextMenu_Popup(object sender, EventArgs e)
	{
		contextPos = sketchView.PointToClient(Control.MousePosition);
		actualMap.Screen2World(contextPos.X, contextPos.Y, out var wx, out var wy);
		contextStation = survey.NearestSide(wx, wy, actualMap.Delta, null, sketchView.tapped);
		contextStation = survey.DefiningStation(contextStation);
		if (contextStation != null && !(contextStation is Reference))
		{
			menuFlip.Checked = contextStation.flipped;
			menuFlip.Enabled = true;
			menuFlipAll.Enabled = true;
		}
		else
		{
			menuFlip.Enabled = false;
			menuFlipAll.Enabled = false;
		}
		menuRasterS.Checked = Config.rasterVisible;
		menuIdsS.Checked = Config.showSketchId;
		menuThickS.Checked = Config.thickLines;
		MenuItem menuItem = menuXSectH;
		bool enabled = (menuXSectV.Enabled = contextStation != null && !contextStation.readOnly);
		menuItem.Enabled = enabled;
		MenuItem menuItem2 = menuS2D;
		enabled = (menuS2M.Enabled = contextStation != null);
		menuItem2.Enabled = enabled;
		menuS2O.Enabled = contextStation != null && (Config.paintAll || !contextStation.readOnly);
	}

	private void menuRaster_Click(object sender, EventArgs e)
	{
		Config.rasterVisible = !Config.rasterVisible;
		sketchView.Invalidate();
	}

	private void menuIdsS_Click(object sender, EventArgs e)
	{
		Config.showSketchId = !Config.showSketchId;
		sketchView.Invalidate();
	}

	private void menuIds_Click(object sender, EventArgs e)
	{
		Config.showMapId = !Config.showMapId;
		sketchView.Invalidate();
	}

	private void menuThick_Click(object sender, EventArgs e)
	{
		Config.thickLines = !Config.thickLines;
		mapView.Invalidate();
		sketchView.Invalidate();
	}

	private void menuXSect_Click(object sender, EventArgs e)
	{
		XSection.Create(survey, contextStation, hor: false);
		pictSelMove.Visible = false;
	}

	private void menuXSectV_Click(object sender, EventArgs e)
	{
		XSection.Create(survey, contextStation, hor: false);
		pictSelMove.Visible = false;
	}

	private void menuXSectH_Click(object sender, EventArgs e)
	{
		XSection.Create(survey, contextStation, hor: true);
		pictSelMove.Visible = false;
	}

	internal void ReenableMove()
	{
		pictSelMove.Visible = true;
	}

	private void menuFlip_Click(object sender, EventArgs e)
	{
		if (contextStation != null && !(contextStation is Reference))
		{
			survey.Flip(contextStation, all: false);
			sketchView.Invalidate();
		}
	}

	private void menuFlipAll_Click(object sender, EventArgs e)
	{
		if (contextStation != null && !(contextStation is Reference))
		{
			survey.Flip(contextStation, all: true);
			sketchView.Invalidate();
		}
	}

	private void menuProjection_Click(object sender, EventArgs e)
	{
		if (contextStation != null && !(contextStation is Reference))
		{
			survey.Project(contextStation);
			sketchView.Invalidate();
		}
	}

	private void menuD2M_Click(object sender, EventArgs e)
	{
		SwitchToMap(contextStation);
	}

	private void menuD2O_Click(object sender, EventArgs e)
	{
		SwitchToSketch(contextStation, 1);
	}

	private void menuD2S_Click(object sender, EventArgs e)
	{
		SwitchToSketch(contextStation, 2);
	}

	private void menuM2D_Click(object sender, EventArgs e)
	{
		SwitchToData(contextStation);
	}

	private void menuM2O_Click(object sender, EventArgs e)
	{
		SwitchToSketch(contextStation, 1);
	}

	private void menuM2S_Click(object sender, EventArgs e)
	{
		SwitchToSketch(contextStation, 2);
	}

	private void menuO2D_Click(object sender, EventArgs e)
	{
		SwitchToData(contextStation);
	}

	private void menuO2M_Click(object sender, EventArgs e)
	{
		SwitchToMap(contextStation);
	}

	private void menuO2S_Click(object sender, EventArgs e)
	{
		SwitchToSketch(contextStation, 2);
	}

	private void menuS2D_Click(object sender, EventArgs e)
	{
		SwitchToData(contextStation);
	}

	private void menuS2M_Click(object sender, EventArgs e)
	{
		SwitchToMap(contextStation);
	}

	private void menuS2O_Click(object sender, EventArgs e)
	{
		SwitchToSketch(contextStation, 1);
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

	private void menuExpText_Click(object sender, EventArgs e)
	{
		txtFileDialog.FileName = Path.GetFileNameWithoutExtension(Config.actualFile);
		DialogResult dialogResult = txtFileDialog.ShowDialog();
		if (dialogResult == DialogResult.OK)
		{
			Exporter.WriteText(txtFileDialog.FileName, survey);
		}
	}

	private void menuExpTherion_Click(object sender, EventArgs e)
	{
		txtFileDialog.FileName = Path.GetFileNameWithoutExtension(Config.actualFile);
		DialogResult dialogResult = txtFileDialog.ShowDialog();
		if (dialogResult == DialogResult.OK)
		{
			Therion.WriteExport(txtFileDialog.FileName, survey, outline, sideview);
		}
	}

	private void menuExpDXF_Click(object sender, EventArgs e)
	{
		dxfForm.ShowDialog();
	}

	private void menuExp3D_Click(object sender, EventArgs e)
	{
		dxfForm.saveFileDialog.FileName = Path.GetFileNameWithoutExtension(Config.actualFile);
		DialogResult dialogResult = dxfForm.saveFileDialog.ShowDialog();
		if (dialogResult == DialogResult.OK)
		{
			ThreeD.WriteDXF(dxfForm.saveFileDialog.FileName, survey, side: false);
		}
	}

	private string DumpFile()
	{
		SaveFileDialog saveFileDialog = new SaveFileDialog();
		saveFileDialog.FileName = Path.GetFileNameWithoutExtension(Config.actualFile);
		saveFileDialog.Filter = "Bitmap (.png)|*.png|Bitmap (.gif)|*.gif|Bitmap (.bmp)|*.bmp";
		DialogResult dialogResult = saveFileDialog.ShowDialog();
		if (dialogResult == DialogResult.OK)
		{
			return saveFileDialog.FileName;
		}
		return null;
	}

	private void menuDumpM_Click(object sender, EventArgs e)
	{
		string text = DumpFile();
		if (text != null)
		{
			mapView.Dump(text);
		}
	}

	private void menuDump_Click(object sender, EventArgs e)
	{
		string text = DumpFile();
		if (text != null)
		{
			sketchView.Dump(text);
		}
	}

	private void menuCloseAll_Click(object sender, EventArgs e)
	{
		Cursor.Current = Cursors.WaitCursor;
		survey.CloseLoops(close: true, all: true);
		sketchView.Invalidate();
		mapView.Invalidate();
		Cursor.Current = null;
	}

	private void menuCloseAct_Click(object sender, EventArgs e)
	{
		Cursor.Current = Cursors.WaitCursor;
		survey.CloseLoops(close: true, all: false);
		sketchView.Invalidate();
		mapView.Invalidate();
		Cursor.Current = null;
	}

	private void menuCloseNone_Click(object sender, EventArgs e)
	{
		Cursor.Current = Cursors.WaitCursor;
		survey.CloseLoops(close: false, all: true);
		sketchView.Invalidate();
		mapView.Invalidate();
		Cursor.Current = null;
	}

	private void menuLOn_Click(object sender, EventArgs e)
	{
		Link.LaserOn();
	}

	private void menuLOff_Click(object sender, EventArgs e)
	{
		Link.LaserOff();
	}

	private void menuMeasure_Click(object sender, EventArgs e)
	{
		Link.Measure();
	}
}
