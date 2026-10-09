using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace PocketTopo;

public class Survey
{
	private class InsDelOp : Undoable
	{
		private BindingSource data;

		private int row;

		private object s;

		internal InsDelOp(BindingSource data, int row, object s)
		{
			obj = (this.data = data);
			this.row = row;
			this.s = s;
		}

		internal override bool Do()
		{
			if (s != null)
			{
				if (row >= data.Count)
				{
					row = data.Add(s);
				}
				else
				{
					data.Insert(row, s);
				}
				s = null;
			}
			else
			{
				s = data[row];
				data.RemoveAt(row);
			}
			return true;
		}
	}

	private class ChangeOp : Undoable
	{
		private BindingSource data;

		private int row;

		private int col;

		private string text;

		internal ChangeOp(BindingSource data, int row, int col, string text)
		{
			obj = (this.data = data);
			this.row = row;
			this.col = col;
			this.text = text;
		}

		internal override bool Do()
		{
			Station station = (Station)data[row];
			string text = station.Get(col);
			if (text != this.text)
			{
				Station.error = false;
				station.Set(col, this.text);
				if (!Station.error)
				{
					data.ResetItem(row);
					this.text = text;
					return true;
				}
			}
			return false;
		}
	}

	private class IncOp : Undoable
	{
		private BindingSource data;

		private int row;

		private int col;

		private int inc;

		internal IncOp(BindingSource data, int row, int col, int inc)
		{
			obj = (this.data = data);
			this.row = row;
			this.col = col;
			this.inc = inc;
		}

		internal override bool Do()
		{
			Station station = (Station)data[row];
			if (col == 0)
			{
				station.from.Adjust(inc);
			}
			else
			{
				if (col != 1)
				{
					return false;
				}
				station.to.Adjust(inc);
			}
			data.ResetItem(row);
			inc = -inc;
			return true;
		}
	}

	private class BranchOp : Undoable
	{
		private BindingSource data;

		private int idx1;

		private int idx2;

		private bool restore;

		private ID from;

		private ID to;

		private Station last;

		internal BranchOp(BindingSource data, int idx1, int idx2, Station last)
		{
			obj = (this.data = data);
			this.idx1 = idx1;
			this.idx2 = idx2;
			this.last = last;
		}

		internal override bool Do()
		{
			Station station = (Station)data[idx1];
			ID iD = from;
			from = station.from;
			ID iD2 = to;
			to = station.to;
			if (restore)
			{
				station.from = iD;
				station.to = iD2;
			}
			else
			{
				restore = true;
				if (!station.TurnToBranch(last))
				{
					return false;
				}
			}
			data.ResetItem(idx1);
			for (int i = idx1 + 1; i <= idx2; i++)
			{
				Station station2 = (Station)data[i];
				station2.from = station.from;
				station2.to = station.to;
				data.ResetItem(i);
			}
			return true;
		}

		internal Station Hint(int idx)
		{
			if (to == ID.UNDEF && Math.Abs(idx1 - idx) == 1)
			{
				return (Station)data[idx1];
			}
			return null;
		}
	}

	private class RenumOp : Undoable
	{
		private BindingSource data;

		private int idx;

		private bool restore;

		private ID from;

		private ID to;

		internal RenumOp(BindingSource data, int idx)
		{
			obj = (this.data = data);
			this.idx = idx;
		}

		internal override bool Do()
		{
			Station station = (Station)data[idx];
			ID iD = station.from;
			ID iD2 = station.to;
			ID oldID = station.ActualId();
			ID iD3 = from;
			from = station.from;
			ID iD4 = to;
			to = station.to;
			if (restore)
			{
				station.from = iD3;
				station.to = iD4;
			}
			else
			{
				restore = true;
				Station station2 = (Station)data[idx - 1];
				if (station.from != station2.from && ID.Dist(station.from, station2.from) == ID.Dist(station.to, station2.to) && Angle.NearTo(station, station2))
				{
					station.from = station2.from;
					station.to = station2.to;
				}
				else if (!station.Renumber(ID.UNDEF, station2.ActualId()))
				{
					return false;
				}
			}
			for (int i = idx + 1; i < data.Count; i++)
			{
				Station station2 = station;
				station = (Station)data[i];
				ID iD5 = station.from;
				ID iD6 = station.to;
				ID iD7 = station.ActualId();
				if (iD5 == iD && iD6 == iD2)
				{
					station.from = station2.from;
					station.to = station2.to;
				}
				else if (iD5 == iD2 && iD6 == iD)
				{
					station.from = station2.to;
					station.to = station2.from;
				}
				else if (!station.Renumber(oldID, station2.ActualId()))
				{
					break;
				}
				iD = iD5;
				iD2 = iD6;
				oldID = iD7;
			}
			data.ResetBindings(metadataChanged: false);
			return true;
		}
	}

	private class ChgTripOp : Undoable
	{
		private Trip trip;

		private DateTime date;

		private bool auto;

		private Angle decl;

		private string comment;

		internal ChgTripOp(Trip trip, DateTime date, bool auto, Angle decl, string comment)
		{
			obj = (this.trip = trip);
			this.date = date;
			this.auto = auto;
			this.decl = decl;
			this.comment = comment;
		}

		internal override bool Do()
		{
			DateTime dateTime = trip.Date;
			bool autoDecl = trip.AutoDecl;
			Angle declCorr = trip.DeclCorr;
			string text = trip.Comment;
			if (trip.Change(date, auto, decl, comment))
			{
				date = dateTime;
				decl = declCorr;
				auto = autoDecl;
				comment = text;
				return true;
			}
			return false;
		}
	}

	internal BindingSource database;

	internal int dataBoundary;

	internal BindingSource references;

	internal int refBoundary;

	internal Reference refBase;

	internal Undo undo;

	private Station actual;

	private Station template;

	private Loop loops;

	private Loop tmpLoops;

	private Hashtable actDir = new Hashtable();

	private Hashtable tplDir = new Hashtable();

	private int aLength;

	private int tLength;

	private int aMinH;

	private int tMinH;

	private int aMaxH;

	private int tMaxH;

	internal int aMinX;

	internal int tMinX;

	internal int aMaxX;

	internal int tMaxX;

	internal int aMinY;

	internal int tMinY;

	internal int aMaxY;

	internal int tMaxY;

	internal int aMinD;

	internal int tMinD;

	internal int aMaxD;

	internal int tMaxD;

	private bool valid;

	private bool templateValid = true;

	private int lastIndex;

	internal int selectedRow;

	internal int TotLength => tLength + aLength;

	internal int TotHeight => Math.Max(tMaxH, aMaxH) - Math.Min(tMinH, aMinH);

	internal int ActLength => aLength;

	internal int ActHeight => aMaxH - aMinH;

	internal Survey(BindingSource database, BindingSource references, EventHandler undoChanged)
	{
		this.database = database;
		this.references = references;
		undo = new Undo(this);
		undo.UndoChanged += undoChanged;
		Station.survey = this;
	}

	internal Station Nearest(int x, int y, int dist, bool all, Station old, Station act)
	{
		Station result = null;
		for (Station next = actual; next != null; next = next.next)
		{
			int num = Math.Abs(x - next.x) + Math.Abs(y - next.y);
			if (num <= dist)
			{
				if (num < dist || next == act)
				{
					dist = num;
					result = next;
				}
				if (next == old)
				{
					dist++;
				}
			}
		}
		if (all)
		{
			for (Station next = template; next != null; next = next.next)
			{
				int num = Math.Abs(x - next.x) + Math.Abs(y - next.y);
				if (num <= dist)
				{
					if (num < dist || next == act)
					{
						dist = num;
						result = next;
					}
					if (next == old)
					{
						dist++;
					}
				}
			}
		}
		return result;
	}

	internal Station NearestSide(int x, int h, int dist, Station old, Station act)
	{
		Station result = null;
		for (Station next = actual; next != null; next = next.next)
		{
			int num = Math.Abs(x - next.d) + Math.Abs(h - next.h);
			if (num <= dist)
			{
				if (num < dist || next == act)
				{
					dist = num;
					result = next;
				}
				if (next == old)
				{
					dist++;
				}
			}
		}
		return result;
	}

	internal Station DefiningStation(Station s)
	{
		if (s != null)
		{
			while (s.dist == 0 && s.start != null)
			{
				s = s.start;
			}
		}
		return s;
	}

	private bool SetupShot(Station act, bool reverse, bool actBounds)
	{
		Station station = (Station)actDir[act.from];
		if (station == null && reverse)
		{
			station = (Station)actDir[act.to];
		}
		if (station == null)
		{
			station = (Station)tplDir[act.from];
		}
		if (station == null && reverse)
		{
			station = (Station)tplDir[act.to];
		}
		if (station == null)
		{
			return false;
		}
		if (act.to == station.num)
		{
			act.num = act.from;
			act.dx = -act.dx;
			act.dy = -act.dy;
			act.dh = -act.dh;
		}
		if (act.projected)
		{
			Station station2 = station;
			while (station2.projected || station2.dist == 0)
			{
				station2 = station2.start;
			}
			int num = Math.Abs(act.dd);
			if (station2.flipped)
			{
				num = -num;
			}
			act.dd = (int)((long)num * (long)Angle.Cos((int)act.FwAzimuth - (int)station2.FwAzimuth) >> 30);
		}
		act.x = station.x + act.dx + (int)(act.ex >> 30);
		act.y = station.y - act.dy + (int)(act.ey >> 30);
		act.h = station.h - act.dh + (int)(act.eh >> 30);
		act.d = station.d + act.dd;
		act.next = actual;
		actual = act;
		Station station3 = (Station)actDir[act.num];
		if (station3 == null)
		{
			actDir.Add(act.num, act);
			station3 = (Station)tplDir[act.num];
		}
		if (station3 != null)
		{
			Loop loop = new Loop(act, station3);
			loop.next = loops;
			loops = loop;
		}
		if (!act.invisible)
		{
			if (actBounds)
			{
				aLength += act.dist;
				if (act.x < aMinX)
				{
					aMinX = act.x;
				}
				if (act.x > aMaxX)
				{
					aMaxX = act.x;
				}
				if (act.y < aMinY)
				{
					aMinY = act.y;
				}
				if (act.y > aMaxY)
				{
					aMaxY = act.y;
				}
				if (act.h < aMinH)
				{
					aMinH = act.h;
				}
				if (act.h > aMaxH)
				{
					aMaxH = act.h;
				}
				if (act.d < aMinD)
				{
					aMinD = act.d;
				}
				if (act.d > aMaxD)
				{
					aMaxD = act.d;
				}
			}
			else
			{
				tLength += act.dist;
				if (act.x < tMinX)
				{
					tMinX = act.x;
				}
				if (act.x > tMaxX)
				{
					tMaxX = act.x;
				}
				if (act.y < tMinY)
				{
					tMinY = act.y;
				}
				if (act.y > tMaxY)
				{
					tMaxY = act.y;
				}
				if (act.h < tMinH)
				{
					tMinH = act.h;
				}
				if (act.h > tMaxH)
				{
					tMaxH = act.h;
				}
				if (act.d < tMinD)
				{
					tMinD = act.d;
				}
				if (act.d > tMaxD)
				{
					tMaxD = act.d;
				}
			}
		}
		act.start = station;
		if (station.num.NextTo(act.num) && !act.projected)
		{
			Station station2 = station;
			if (station2.projected)
			{
				while (station2.projected || station2.dist == 0)
				{
					station2 = station2.start;
				}
			}
			for (Station station4 = station.cross; station4 != null; station4 = station4.next)
			{
				station4.d = station.d + Projection.SideProjection(station2.FwAzimuth, act.FwAzimuth, station4.dd, station4.azimuth, station2.flipped, act.flipped);
			}
		}
		act.SetBox();
		return true;
	}

	private bool SetupXsect(Station dat)
	{
		Station station = (Station)actDir[dat.from];
		if (station == null)
		{
			station = (Station)tplDir[dat.from];
		}
		if (station == null)
		{
			return false;
		}
		if (dat.dist == 0)
		{
			return true;
		}
		Station station2 = station.cross;
		if (station2 != null)
		{
			while (station2.next != null)
			{
				station2 = station2.next;
			}
			station2.next = dat;
		}
		else
		{
			station.cross = dat;
		}
		dat.next = null;
		Station station3 = station;
		if (station3.projected)
		{
			while (station3.projected || station3.dist == 0)
			{
				station3 = station3.start;
			}
		}
		int num = Projection.SideProjection(station3.FwAzimuth, int.MinValue, dat.dd, dat.azimuth, station3.flipped, flip2: false);
		dat.num = dat.from;
		dat.x = station.x + dat.dx;
		dat.y = station.y - dat.dy;
		dat.h = station.h - dat.dh;
		dat.d = station.d + num;
		dat.start = station;
		dat.SetBox();
		if (dat.to == ID.UNDEF)
		{
			station.olBox = Rectangle.Union(station.olBox, dat.olBox);
			station.svBox = Rectangle.Union(station.svBox, dat.svBox);
		}
		return true;
	}

	private void EvalData(int startIdx, int endIdx, bool actBounds)
	{
		if (actBounds)
		{
			aLength = 0;
		}
		else
		{
			tLength = 0;
		}
		Station station = null;
		Station station2 = null;
		for (int i = startIdx; i < endIdx; i++)
		{
			Station station3 = (Station)database[i];
			station3.idx = i;
			station3.num = ID.UNDEF;
			station3.start = (station3.next = (station3.cross = null));
			if (!(station3.from != ID.UNDEF))
			{
				continue;
			}
			station3.num = station3.to;
			Angle.ToRect(station3.dist, station3.incl, out station3.dh, out station3.dd);
			Angle.ToRect(station3.dd, station3.RealAzimuth(), out station3.dx, out station3.dy);
			if (station3.to != ID.UNDEF)
			{
				int num = 1;
				while (i < endIdx - 1)
				{
					Station station4 = (Station)database[i + 1];
					int x;
					int y;
					int x2;
					int y2;
					if (station4.from == station3.from && station4.to == station3.to)
					{
						Angle.ToRect(station4.dist, station4.incl, out x, out y);
						Angle.ToRect(y, station4.RealAzimuth(), out x2, out y2);
						station3.dh += x;
						station3.dx += x2;
						station3.dy += y2;
					}
					else
					{
						if (!(station4.from == station3.to) || !(station4.to == station3.from))
						{
							break;
						}
						Angle.ToRect(station4.dist, station4.incl, out x, out y);
						Angle.ToRect(y, station4.RealAzimuth(), out x2, out y2);
						station3.dh -= x;
						station3.dx -= x2;
						station3.dy -= y2;
					}
					station3.dd += y;
					i++;
					num++;
					station4.num = station3.num;
					station4.start = (station4.next = (station4.cross = null));
				}
				if (num > 1)
				{
					station3.dd /= num;
					station3.dh /= num;
					station3.dx /= num;
					station3.dy /= num;
				}
				if (station3.flipped)
				{
					station3.dd = -station3.dd;
				}
				if (!SetupShot(station3, station3.from == station3.to.Next(), actBounds))
				{
					station3.next = station;
					station = station3;
				}
			}
			else if (!SetupXsect(station3))
			{
				station3.next = station2;
				station2 = station3;
			}
		}
		bool flag = true;
		while (flag)
		{
			Station station5 = null;
			flag = false;
			while (station != null)
			{
				Station station3 = station;
				station = station.next;
				if (SetupShot(station3, station3.from.NextTo(station3.to), actBounds))
				{
					flag = true;
					continue;
				}
				station3.next = station5;
				station5 = station3;
			}
			station = station5;
		}
		flag = true;
		while (flag)
		{
			Station station5 = null;
			flag = false;
			while (station != null)
			{
				Station station3 = station;
				station = station.next;
				if (SetupShot(station3, reverse: true, actBounds))
				{
					flag = true;
					continue;
				}
				station3.next = station5;
				station5 = station3;
			}
			station = station5;
		}
		while (station2 != null)
		{
			Station dat = station2;
			station2 = station2.next;
			SetupXsect(dat);
		}
	}

	private void EvalRef(int startIdx, int endIdx, bool actBounds)
	{
		if (actBounds)
		{
			aMinX = (aMinY = (aMinH = (aMinD = int.MaxValue)));
			aMaxX = (aMaxY = (aMaxH = (aMaxD = int.MinValue)));
		}
		else
		{
			tMinX = (tMinY = (tMinH = (tMinD = int.MaxValue)));
			tMaxX = (tMaxY = (tMaxH = (tMaxD = int.MinValue)));
		}
		actual = null;
		loops = null;
		actDir.Clear();
		if (refBase != null && refBase.idx >= startIdx)
		{
			refBase = null;
		}
		for (int i = startIdx; i < endIdx; i++)
		{
			Reference reference = (Reference)references[i];
			reference.idx = i;
			if (!(reference.num != ID.UNDEF))
			{
				continue;
			}
			reference.d = 0;
			if (refBase == null)
			{
				refBase = reference;
				reference.x = (reference.y = (reference.h = 0));
			}
			else
			{
				reference.x = (int)(reference.loc.e - refBase.loc.e);
				reference.y = (int)(refBase.loc.n - reference.loc.n);
				reference.h = refBase.loc.alt - reference.loc.alt;
			}
			reference.next = actual;
			actual = reference;
			if (!actDir.Contains(reference.num))
			{
				actDir.Add(reference.num, reference);
			}
			if (actBounds)
			{
				if (reference.x < aMinX)
				{
					aMinX = reference.x;
				}
				if (reference.x > aMaxX)
				{
					aMaxX = reference.x;
				}
				if (reference.y < aMinY)
				{
					aMinY = reference.y;
				}
				if (reference.y > aMaxY)
				{
					aMaxY = reference.y;
				}
				if (reference.h < aMinH)
				{
					aMinH = reference.h;
				}
				if (reference.h > aMaxH)
				{
					aMaxH = reference.h;
				}
				if (reference.d < aMinD)
				{
					aMinD = reference.d;
				}
				if (reference.d > aMaxD)
				{
					aMaxD = reference.d;
				}
			}
			else
			{
				if (reference.x < tMinX)
				{
					tMinX = reference.x;
				}
				if (reference.x > tMaxX)
				{
					tMaxX = reference.x;
				}
				if (reference.y < tMinY)
				{
					tMinY = reference.y;
				}
				if (reference.y > tMaxY)
				{
					tMaxY = reference.y;
				}
				if (reference.h < tMinH)
				{
					tMinH = reference.h;
				}
				if (reference.h > tMaxH)
				{
					tMaxH = reference.h;
				}
				if (reference.d < tMinD)
				{
					tMinD = reference.d;
				}
				if (reference.d > tMaxD)
				{
					tMaxD = reference.d;
				}
			}
		}
		if (actual != null || template != null || database.Count <= 0)
		{
			return;
		}
		ID refId = GetRefId(0);
		if (refId != ID.UNDEF)
		{
			Reference reference = new Reference(refId, default(MetricLocation));
			reference.idx = -1;
			reference.next = actual;
			actual = reference;
			actDir.Add(reference.num, reference);
			if (actBounds)
			{
				aMinX = (aMinY = (aMinH = (aMinD = (aMaxX = (aMaxY = (aMaxH = (aMaxD = 0)))))));
			}
			else
			{
				tMinX = (tMinY = (tMinH = (tMinD = (tMaxX = (tMaxY = (tMaxH = (tMaxD = 0)))))));
			}
		}
	}

	internal void Update()
	{
		if (!templateValid)
		{
			Hashtable hashtable = actDir;
			actDir = tplDir;
			Cursor.Current = Cursors.WaitCursor;
			EvalRef(0, refBoundary, actBounds: false);
			Trip.Evaluate(refBase);
			EvalData(0, dataBoundary, actBounds: false);
			Cursor.Current = null;
			template = actual;
			tmpLoops = loops;
			actual = null;
			loops = null;
			tplDir = actDir;
			actDir = hashtable;
			actDir.Clear();
			templateValid = true;
		}
		if (!valid)
		{
			Cursor.Current = Cursors.WaitCursor;
			EvalRef(refBoundary, references.Count, actBounds: true);
			Trip.Evaluate(refBase);
			EvalData(dataBoundary, database.Count, actBounds: true);
			Cursor.Current = null;
			valid = true;
		}
	}

	internal void CloseLoops(bool close, bool all)
	{
		int i = 0;
		if (!all)
		{
			i = dataBoundary;
		}
		for (; i < database.Count; i++)
		{
			((Station)database[i]).ResetError();
		}
		valid = false;
		if (all)
		{
			templateValid = false;
		}
		Update();
		if (close)
		{
			Loop.CloseLoops(loops, all ? tmpLoops : null, all);
			valid = false;
			if (all)
			{
				templateValid = false;
			}
			Update();
		}
	}

	internal void Invalidate()
	{
		valid = false;
	}

	internal void Reset()
	{
		database.Clear();
		dataBoundary = 0;
		references.Clear();
		refBoundary = 0;
		refBase = null;
		tLength = (aLength = 0);
		tMinH = (aMinH = int.MaxValue);
		tMaxH = (aMaxH = int.MinValue);
		template = null;
		actual = null;
		tmpLoops = (loops = null);
		templateValid = true;
		actDir.Clear();
		tplDir.Clear();
		valid = true;
		undo.Reset();
	}

	internal void SetBoundary()
	{
		if (actual != null || template != null)
		{
			throw new Exception();
		}
		dataBoundary = database.Count;
		refBoundary = references.Count;
		templateValid = valid;
		valid = true;
		Trip.SetBoundary();
	}

	internal Station SearchById(ID id)
	{
		Update();
		Station station = (Station)actDir[id];
		if (station == null)
		{
			station = (Station)tplDir[id];
		}
		return station;
	}

	public Station[] GetStations(bool inclTemplate)
	{
		Update();
		int num = 0;
		for (Station next = actual; next != null; next = next.next)
		{
			if (next.idx >= 0)
			{
				num++;
			}
		}
		if (inclTemplate)
		{
			for (Station next = template; next != null; next = next.next)
			{
				num++;
			}
		}
		Station[] array = new Station[num];
		int num2 = 0;
		for (Station next = actual; next != null; next = next.next)
		{
			if (next.idx >= 0)
			{
				array[num2++] = next;
			}
		}
		if (inclTemplate)
		{
			for (Station next = template; next != null; next = next.next)
			{
				array[num2++] = next;
			}
		}
		Array.Sort(array);
		return array;
	}

	public string[] GetSeries()
	{
		SortedList sortedList = new SortedList();
		for (int i = 0; i < database.Count; i++)
		{
			Station station = (Station)database[i];
			string key = station.from.SeriesString();
			if (!sortedList.Contains(key))
			{
				sortedList.Add(key, null);
			}
			string key2 = station.to.SeriesString();
			if (!sortedList.Contains(key2))
			{
				sortedList.Add(key2, null);
			}
		}
		string[] array = new string[sortedList.Count];
		for (int i = 0; i < sortedList.Count; i++)
		{
			array[i] = (string)sortedList.GetKey(i);
		}
		return array;
	}

	private ID GetRefId(int first)
	{
		int count = database.Count;
		if (count == 0)
		{
			return ID.UNDEF;
		}
		if (first == count)
		{
			first = 0;
		}
		Station station = (Station)database[first];
		while (station.from == ID.UNDEF && first < count - 1)
		{
			station = (Station)database[++first];
		}
		return station.from;
	}

	internal void GetRef(out int x, out int y, out int h, out int d)
	{
		x = 0;
		y = 0;
		h = 0;
		d = 0;
		Update();
		ID refId = GetRefId(dataBoundary);
		if (refId != ID.UNDEF)
		{
			Station station = SearchById(refId);
			if (station != null)
			{
				x = station.x;
				y = station.y;
				h = station.h;
				d = station.d;
			}
		}
	}

	internal string StationInfo(Station s)
	{
		string text = s.num.ToString();
		if (refBase != null)
		{
			text = text + ":  " + Reference.grid.ToString(new MetricLocation(refBase.loc.e + s.x, refBase.loc.n - s.y, refBase.loc.alt - s.h));
		}
		return text;
	}

	internal void GetLocation(MetricLocation loc, out int x, out int y, out int h)
	{
		x = (int)(loc.e - refBase.loc.e);
		y = (int)(refBase.loc.n - loc.n);
		h = refBase.loc.alt - loc.alt;
	}

	internal int PaintOutline(Mapping m, Graphics g, Rectangle clip, bool all, bool showId, Pen line, Pen cross, Pen tLine, Pen tCross)
	{
		Update();
		m.Prepare();
		clip.X -= 2;
		clip.Y -= 2;
		clip.Width += 4;
		clip.Height += 4;
		int num = 0;
		Rectangle w = default(Rectangle);
		m.Screen2World(clip, ref w);
		Drawing.SetupPen(line);
		Drawing.SetupPen(cross);
		Drawing.SetupPen(tLine);
		Drawing.SetupPen(tCross);
		Drawing.SetupPen(MainForm.blue);
		Drawing.SetupPen(MainForm.selected);
		int showStat = ((!showId) ? 1 : 2);
		if (all)
		{
			for (Station next = template; next != null; next = next.next)
			{
				if (next.OutlineVisible(w))
				{
					next.PaintOutline(m, g, tLine, showStat);
					if (tCross != null)
					{
						for (Station station = next.cross; station != null; station = station.next)
						{
							station.PaintOutline(m, g, tCross, 0);
						}
					}
					num++;
				}
			}
		}
		for (Station next = actual; next != null; next = next.next)
		{
			if (next.OutlineVisible(w))
			{
				next.PaintOutline(m, g, line, showStat);
				if (cross != null)
				{
					for (Station station = next.cross; station != null; station = station.next)
					{
						station.PaintOutline(m, g, cross, 0);
					}
				}
				num++;
			}
		}
		return num;
	}

	internal int PaintSideview(Mapping m, Graphics g, Rectangle clip, bool showId, Pen line, Pen cross)
	{
		Update();
		m.Prepare();
		clip.X -= 2;
		clip.Y -= 2;
		clip.Width += 4;
		clip.Height += 4;
		int num = 0;
		Rectangle w = default(Rectangle);
		m.Screen2World(clip, ref w);
		Drawing.SetupPen(line);
		Drawing.SetupPen(cross);
		Drawing.SetupPen(MainForm.selected);
		int showStat = ((!showId) ? 1 : 2);
		for (Station next = actual; next != null; next = next.next)
		{
			if (next.SideviewVisible(w))
			{
				next.PaintSideview(m, g, line, showStat);
				if (cross != null)
				{
					for (Station station = next.cross; station != null; station = station.next)
					{
						station.PaintSideview(m, g, cross, 0);
					}
				}
				num++;
			}
		}
		return num;
	}

	internal void Write(FileWriter w)
	{
		int tripOffs = Trip.WriteList(w);
		int num = database.Count - dataBoundary;
		w.w.Write(num);
		for (int i = 0; i < num; i++)
		{
			Station station = (Station)database[dataBoundary + i];
			station.Write(w, tripOffs);
		}
		int num2 = references.Count - refBoundary;
		w.w.Write(num2);
		for (int i = 0; i < num2; i++)
		{
			Reference reference = (Reference)references[refBoundary + i];
			reference.Write(w);
		}
	}

	internal void Read(FileReader r, bool readOnly)
	{
		int tripOffs = ((r.version < 3) ? Trip.GetDefault() : Trip.ReadList(r));
		database.RaiseListChangedEvents = false;
		try
		{
			int num = r.r.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				Station station = new Station();
				station.Read(r, tripOffs);
				station.readOnly = readOnly;
				database.Add(station);
			}
		}
		finally
		{
			database.RaiseListChangedEvents = true;
			database.ResetBindings(metadataChanged: false);
		}
		if (r.version >= 3)
		{
			references.RaiseListChangedEvents = false;
			try
			{
				int num2 = r.r.ReadInt32();
				for (int i = 0; i < num2; i++)
				{
					Reference reference = new Reference(ID.DEFAULT, default(MetricLocation));
					reference.Read(r);
					reference.readOnly = readOnly;
					references.Add(reference);
				}
			}
			finally
			{
				references.RaiseListChangedEvents = true;
				references.ResetBindings(metadataChanged: false);
			}
		}
		else
		{
			for (int num3 = r.r.ReadInt32(); num3 >= 0; num3 = r.r.ReadInt32())
			{
				Station station = (Station)database[dataBoundary + num3];
				station.comment = r.r.ReadString();
			}
		}
		Invalidate();
	}

	internal void WriteText(TextWriter w)
	{
		Update();
		int tripOffs = Trip.WriteText(w);
		w.WriteLine();
		int num = references.Count - refBoundary;
		for (int i = 0; i < num; i++)
		{
			Reference reference = (Reference)references[refBoundary + i];
			reference.WriteText(w);
		}
		w.WriteLine();
		int num2 = database.Count - dataBoundary;
		for (int i = 0; i < num2; i++)
		{
			Station station = (Station)database[dataBoundary + i];
			station.WriteText(w, tripOffs);
		}
	}

	internal void WriteTherion(TextWriter w)
	{
		Update();
		int i;
		if (references.Count > refBoundary)
		{
			w.WriteLine("FIX");
			for (i = refBoundary; i < references.Count; i++)
			{
				Reference reference = (Reference)references[i];
				reference.WriteThRef(w);
			}
		}
		i = 0;
		while (true)
		{
			bool flag = true;
			int num = Trip.WriteTherion(w, i);
			if (num < 0)
			{
				break;
			}
			w.WriteLine("DATA");
			for (int j = dataBoundary; j < database.Count; j++)
			{
				if (First(database, j, rev: true) == j)
				{
					Station station = (Station)database[j];
					if (station.trip == num)
					{
						station.WriteTherion(w);
					}
				}
			}
			i++;
		}
	}

	internal void WriteThSketch1(TextWriter w, bool side)
	{
		SortedList sortedList = new SortedList();
		for (Station next = actual; next != null; next = next.next)
		{
			if (!sortedList.Contains(next.num))
			{
				sortedList.Add(next.num, next);
			}
		}
		w.WriteLine("STATIONS");
		for (int i = 0; i < sortedList.Count; i++)
		{
			((Station)sortedList.GetByIndex(i)).WriteThStation(w, side);
		}
	}

	internal void WriteThSketch2(TextWriter w, bool side)
	{
		w.WriteLine("SHOTS");
		for (Station next = actual; next != null; next = next.next)
		{
			next.WriteThShot(w, side);
			for (Station station = next.cross; station != null; station = station.next)
			{
				station.WriteThShot(w, side);
			}
		}
	}

	internal int GridSize(bool largeScale)
	{
		int num = (int)Config.unit;
		if ((double)num <= 304.79999999999995)
		{
			num *= 5;
		}
		if (largeScale)
		{
			num /= 5;
		}
		return num;
	}

	internal void WriteDXFGrid(DXFWriter w)
	{
		Update();
		int num = GridSize(largeScale: false);
		int num2;
		int num3;
		int num4;
		int num5;
		if ((w.opt & DXFWriter.Options.SIDE) != DXFWriter.Options.NONE)
		{
			num2 = aMinD;
			num3 = aMaxD;
			num4 = aMinH;
			num5 = aMaxH;
			if ((w.opt & DXFWriter.Options.ENA) != DXFWriter.Options.NONE)
			{
				if (tMinD < num2)
				{
					num2 = tMinD;
				}
				if (tMaxD > num3)
				{
					num3 = tMaxD;
				}
				if (tMinH < num4)
				{
					num4 = tMinH;
				}
				if (tMaxH > num5)
				{
					num5 = tMaxH;
				}
			}
		}
		else
		{
			num2 = aMinX;
			num3 = aMaxX;
			num4 = aMinY;
			num5 = aMaxY;
			if ((w.opt & DXFWriter.Options.ENA) != DXFWriter.Options.NONE)
			{
				if (tMinX < num2)
				{
					num2 = tMinX;
				}
				if (tMaxX > num3)
				{
					num3 = tMaxX;
				}
				if (tMinY < num4)
				{
					num4 = tMinY;
				}
				if (tMaxY > num5)
				{
					num5 = tMaxY;
				}
			}
		}
		int num6 = num * 2;
		num2 = num2 / num * num - num6;
		num3 = num3 / num * num + num6;
		num4 = num4 / num * num - num6;
		num5 = num5 / num * num + num6;
		for (int i = num2; i <= num3; i += num)
		{
			w.DxfLine(new Point(i, num4), new Point(i, num5), MainForm.lgray, 0, 0, 0, 0);
		}
		for (int j = num4; j <= num5; j += num)
		{
			w.DxfLine(new Point(num2, j), new Point(num3, j), MainForm.lgray, 0, 0, 0, 0);
		}
	}

	internal void WriteDXF(DXFWriter w)
	{
		Update();
		if ((w.opt & DXFWriter.Options.ENA) != DXFWriter.Options.NONE)
		{
			for (Station next = template; next != null; next = next.next)
			{
				if ((w.opt & DXFWriter.Options.ENS) != DXFWriter.Options.NONE)
				{
					next.WriteDXF(w, MainForm.shotOr, null);
				}
				if ((w.opt & DXFWriter.Options.ENX) != DXFWriter.Options.NONE)
				{
					for (Station station = next.cross; station != null; station = station.next)
					{
						station.WriteDXF(w, MainForm.shotXOr, null);
					}
				}
			}
		}
		for (Station next = actual; next != null; next = next.next)
		{
			if ((w.opt & DXFWriter.Options.ENS) != DXFWriter.Options.NONE)
			{
				next.WriteDXF(w, MainForm.shotRd, MainForm.shotDa);
			}
			if ((w.opt & DXFWriter.Options.ENX) != DXFWriter.Options.NONE)
			{
				for (Station station = next.cross; station != null; station = station.next)
				{
					station.WriteDXF(w, MainForm.shotXs, MainForm.shotDa);
				}
			}
		}
	}

	internal void AddStation(Station s)
	{
		database.Add(s);
	}

	internal void AddReference(Station r)
	{
		references.Add(r);
	}

	internal int AddTrip(Trip t)
	{
		return Trip.AddTrip(t);
	}

	internal Station GetEntry(ID from, ID to)
	{
		int num = lastIndex;
		if (num < dataBoundary || num >= database.Count)
		{
			num = dataBoundary;
		}
		int num2 = num;
		do
		{
			Station station = (Station)database[num2];
			if (station.from == from && station.to == to)
			{
				lastIndex = num2;
				return station;
			}
			num2++;
			if (num2 == database.Count)
			{
				num2 = dataBoundary;
			}
		}
		while (num2 != num);
		return null;
	}

	internal static bool Writable(BindingSource data, int row)
	{
		return row >= 0 && !((Station)data[row]).readOnly;
	}

	internal bool ToBack(int idx)
	{
		if (idx >= 0)
		{
			return ((Station)database[idx]).ToBack();
		}
		return false;
	}

	internal static int First(BindingSource data, int idx, bool rev)
	{
		Station station = (Station)data[idx];
		if (station.to != ID.UNDEF)
		{
			while (idx > 0)
			{
				Station station2 = (Station)data[idx - 1];
				if ((station2.from == station.from && station2.to == station.to) || (rev && station2.from == station.to && station2.to == station.from))
				{
					station = station2;
					idx--;
					continue;
				}
				break;
			}
		}
		return idx;
	}

	internal static int Last(BindingSource data, int idx)
	{
		Station station = (Station)data[idx];
		if (station.to != ID.UNDEF)
		{
			while (idx < data.Count - 1)
			{
				Station station2 = (Station)data[idx + 1];
				if (station2.from == station.from && station2.to == station.to)
				{
					station = station2;
					idx++;
					continue;
				}
				break;
			}
		}
		return idx;
	}

	internal void AppendNew(Station act)
	{
		int count = database.Count;
		if (count > 0)
		{
			Station prev = (Station)database[count - 1];
			Station third = null;
			if (count > 1)
			{
				third = (Station)database[count - 2];
			}
			Station fourth = null;
			if (count > 2)
			{
				fourth = (Station)database[count - 3];
			}
			act.SetupNew(prev, third, fourth);
		}
		database.Add(act);
		DataSet.MakeDirty();
		Invalidate();
	}

	internal void Flip(Station d, bool all)
	{
		bool flipped = (d.flipped = !d.flipped);
		for (int i = d.idx + 1; i < database.Count; i++)
		{
			Station station = (Station)database[i];
			if (station.to == ID.UNDEF || (station.from == d.from && station.to == d.to))
			{
				station.flipped = flipped;
				continue;
			}
			if (all && (station.from == d.num || station.to == d.num))
			{
				station.flipped = flipped;
				d = station;
				continue;
			}
			break;
		}
		DataSet.MakeDirty();
		Invalidate();
	}

	internal void Project(Station d)
	{
		bool projected = (d.projected = !d.projected);
		for (int i = d.idx + 1; i < database.Count; i++)
		{
			Station station = (Station)database[i];
			if (station.to == ID.UNDEF || (station.from == d.from && station.to == d.to))
			{
				station.projected = projected;
				continue;
			}
			break;
		}
		DataSet.MakeDirty();
		Invalidate();
	}

	internal void Append(BindingSource data, Station s)
	{
		undo.DoOp(new InsDelOp(data, int.MaxValue, s));
	}

	internal void Insert(BindingSource data, Station s, int row)
	{
		undo.DoOp(new InsDelOp(data, row, s));
	}

	internal void Delete(BindingSource data, int row)
	{
		undo.DoOp(new InsDelOp(data, row, null));
	}

	internal void Change(BindingSource data, int row, int col, string text)
	{
		if (Writable(data, row))
		{
			undo.DoOp(new ChangeOp(data, row, col, text));
		}
	}

	internal void Increment(int row, int col, int inc)
	{
		if (Writable(database, row))
		{
			undo.DoOp(new IncOp(database, row, col, inc));
		}
	}

	internal void TurnToBranch(int idx)
	{
		if (idx >= dataBoundary)
		{
			int num = First(database, idx, rev: false);
			int idx2 = Last(database, idx);
			Station last = null;
			Undoable undoable = undo.LastOp();
			if (undoable is BranchOp)
			{
				last = ((BranchOp)undoable).Hint(num);
			}
			undo.DoOp(new BranchOp(database, num, idx2, last));
		}
	}

	internal void Renumber(int idx)
	{
		if (idx >= dataBoundary)
		{
			undo.DoOp(new RenumOp(database, idx));
		}
	}

	internal void ChangeTrip(Trip trip, DateTime date, bool auto, Angle decl, string comment)
	{
		undo.DoOp(new ChgTripOp(trip, date, auto, decl, comment));
	}
}
