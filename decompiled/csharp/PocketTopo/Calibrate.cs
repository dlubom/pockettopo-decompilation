using System;
using System.IO;
using System.Windows.Forms;

namespace PocketTopo;

internal class Calibrate
{
	internal const int COEFF_ADDR = 32784;

	internal const float FV = 24000f;

	private const float FM = 16384f;

	private const float FN = 2796f;

	private const float EPS = 1E-06f;

	private const int MAX_IT = 200;

	internal static bool dirty;

	internal static BindingSource database;

	internal static StatusBar statusBar;

	internal static Matrix aG;

	internal static Matrix aM;

	internal static Vector bG;

	internal static Vector bM;

	internal static Vector nl;

	internal static bool useNonLinearity;

	internal static float alpha;

	internal static bool resValid = false;

	private static void OptVectors(Vector gr, Vector mr, out Vector gx, out Vector mx)
	{
		Vector vector = Vector.Normalized(gr % mr);
		float num = (float)Math.Sin(alpha);
		float num2 = (float)Math.Cos(alpha);
		gx = Vector.Normalized(mr * num2 + mr % vector * num + gr);
		mx = gx * num2 + vector % gx * num;
	}

	private static void TurnVectors(Vector gxp, Vector mxp, Vector gr, Vector mr, out Vector gx, out Vector mx)
	{
		float num = gr.z * gxp.y - gr.y * gxp.z + mr.z * mxp.y - mr.y * mxp.z;
		float num2 = gr.y * gxp.y + gr.z * gxp.z + mr.y * mxp.y + mr.z * mxp.z;
		float a = (float)Math.Atan2(num, num2);
		gx = gxp.TurnX(a);
		mx = mxp.TurnX(a);
	}

	private static float Saturate(float x)
	{
		if (x > 0.045422032f)
		{
			x = 0.045422032f;
		}
		else if (x < -0.045422032f)
		{
			x = -0.045422032f;
		}
		return x;
	}

	private static int Optimize(Vector[] g, Vector[] m, byte[] group, float[] delta)
	{
		int num = g.Length;
		Vector[] array = new Vector[num];
		Vector[] array2 = new Vector[num];
		Vector[] array3 = new Vector[num];
		Vector[] array4 = new Vector[num];
		Vector[] array5 = new Vector[num];
		Matrix[] array6 = new Matrix[num];
		float num2 = 1f / (float)num;
		Vector zero = Vector.zero;
		Vector zero2 = Vector.zero;
		Matrix zero3 = Matrix.zero;
		Matrix zero4 = Matrix.zero;
		float num3 = 0f;
		float num4 = 0f;
		for (int i = 0; i < num; i++)
		{
			num3 += Vector.Abs(g[i] % m[i]);
			num4 += g[i] * m[i];
			zero += g[i];
			zero2 += m[i];
			zero3 += g[i] & g[i];
			zero4 += m[i] & m[i];
			ref Vector reference = ref array5[i];
			reference = g[i];
			ref Matrix reference2 = ref array6[i];
			reference2 = Matrix.zero;
			array6[i].x.x = g[i].x * g[i].x - 0.5f;
			array6[i].y.y = g[i].y * g[i].y - 0.5f;
			array6[i].z.z = g[i].z * g[i].z - 0.5f;
		}
		alpha = (float)Math.Atan2(num3, num4);
		Vector vector = zero * num2;
		Vector vector2 = zero2 * num2;
		Matrix matrix = Matrix.Inverse(zero3 - (zero & vector));
		Matrix matrix2 = Matrix.Inverse(zero4 - (zero2 & vector2));
		nl = Vector.zero;
		aG = (aM = Matrix.one);
		bG = Vector.zero - vector;
		bM = Vector.zero - vector2;
		int num5 = 0;
		Matrix b;
		Matrix b2;
		do
		{
			for (int i = 0; i < num; i++)
			{
				ref Vector reference3 = ref array[i];
				reference3 = aG * array5[i] + bG;
				ref Vector reference4 = ref array2[i];
				reference4 = aM * m[i] + bM;
			}
			num3 = (num4 = 0f);
			for (int i = 0; i < num; i++)
			{
				if (group[i] > 0)
				{
					Vector zero5 = Vector.zero;
					Vector zero6 = Vector.zero;
					int num6 = group[i];
					int num7 = i;
					for (; group[i] == num6; i++)
					{
						TurnVectors(array[i], array2[i], array[num7], array2[num7], out var gx, out var mx);
						zero5 += gx;
						zero6 += mx;
					}
					OptVectors(zero5, zero6, out var gx2, out var mx2);
					num3 += Vector.Abs(zero6 % gx2);
					num4 += zero6 * gx2;
					for (i = num7; group[i] == num6; i++)
					{
						TurnVectors(gx2, mx2, array[i], array2[i], out array3[i], out array4[i]);
					}
					i--;
				}
				else
				{
					OptVectors(array[i], array2[i], out array3[i], out array4[i]);
					num3 += Vector.Abs(array2[i] % array3[i]);
					num4 += array2[i] * array3[i];
				}
			}
			alpha = (float)Math.Atan2(num3, num4);
			Vector zero7 = Vector.zero;
			Vector zero8 = Vector.zero;
			Matrix zero9 = Matrix.zero;
			Matrix zero10 = Matrix.zero;
			for (int i = 0; i < num; i++)
			{
				zero7 += array3[i];
				zero8 += array4[i];
				zero9 += array3[i] & array5[i];
				zero10 += array4[i] & m[i];
			}
			b = aG;
			b2 = aM;
			zero7 *= num2;
			zero8 *= num2;
			aG = (zero9 - (zero7 & zero)) * matrix;
			aM = (zero10 - (zero8 & zero2)) * matrix2;
			aG.y.z = (aG.z.y = (aG.y.z + aG.z.y) * 0.5f);
			bG = zero7 - aG * vector;
			bM = zero8 - aM * vector2;
			if (useNonLinearity)
			{
				Matrix zero11 = Matrix.zero;
				Vector zero12 = Vector.zero;
				for (int i = 0; i < num; i++)
				{
					Matrix matrix3 = aG * array6[i];
					Vector vector3 = array3[i] - aG * g[i] - bG;
					Matrix matrix4 = Matrix.Transposed(matrix3);
					zero11 += matrix4 * matrix3;
					zero12 += matrix4 * vector3;
				}
				nl = Matrix.Inverse(zero11) * zero12;
				nl.x = Saturate(nl.x);
				nl.y = Saturate(nl.y);
				nl.z = Saturate(nl.z);
				zero = Vector.zero;
				zero3 = Matrix.zero;
				for (int i = 0; i < num; i++)
				{
					ref Vector reference5 = ref array5[i];
					reference5 = g[i] + array6[i] * nl;
					zero += array5[i];
					zero3 += array5[i] & array5[i];
				}
				vector = zero * num2;
				matrix = Matrix.Inverse(zero3 - (zero & vector));
			}
			num5++;
		}
		while (num5 < 200 && Math.Max(Matrix.MaxDiff(aG, b), Matrix.MaxDiff(aM, b2)) > 1E-06f);
		for (int i = 0; i < num; i++)
		{
			Vector vector4 = array3[i] - array[i];
			Vector vector5 = array4[i] - array2[i];
			delta[i] = (float)(Math.Sqrt(vector4 * vector4 + vector5 * vector5) * 100.0);
		}
		return num5;
	}

	private static float CompleteEntries(float[] delta)
	{
		int count = database.Count;
		float num = 0f;
		int num2 = 0;
		for (int i = 0; i < count; i++)
		{
			CalibEntry calibEntry = (CalibEntry)database[i];
			if (delta != null && calibEntry.valid)
			{
				calibEntry.delta = delta[num2];
				num += calibEntry.delta * calibEntry.delta;
				num2++;
			}
			Vector vector = new Vector((float)calibEntry.gx / 24000f, (float)calibEntry.gy / 24000f, (float)calibEntry.gz / 24000f);
			Vector vector2 = new Vector((float)calibEntry.mx / 24000f, (float)calibEntry.my / 24000f, (float)calibEntry.mz / 24000f);
			vector.x += nl.x * (vector.x * vector.x - 0.5f);
			vector.y += nl.y * (vector.y * vector.y - 0.5f);
			vector.z += nl.z * (vector.z * vector.z - 0.5f);
			vector = aG * vector + bG;
			vector2 = aM * vector2 + bM;
			calibEntry.glen = Vector.Abs(vector);
			calibEntry.mlen = Vector.Abs(vector2);
			calibEntry.alpha = (float)Math.Acos(vector * vector2 / (calibEntry.glen * calibEntry.mlen));
			calibEntry.roll = (float)Math.Atan2(vector.y, vector.z);
			vector = vector.TurnX(calibEntry.roll);
			vector2 = vector2.TurnX(calibEntry.roll);
			calibEntry.incl = 0f - (float)Math.Atan2(vector.x, vector.z);
			vector = vector.TurnY(calibEntry.incl);
			vector2 = vector2.TurnY(calibEntry.incl);
			calibEntry.azimuth = 0f - (float)Math.Atan2(vector2.y, vector2.x);
			if (calibEntry.azimuth < 0f)
			{
				calibEntry.azimuth += (float)Math.PI * 2f;
			}
			calibEntry.evaluated = true;
			database.ResetItem(i);
		}
		return (float)Math.Sqrt(num / (float)num2);
	}

	internal static void Evaluate(bool evaluate)
	{
		resValid = false;
		statusBar.Text = "";
		int count = database.Count;
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < count; i++)
		{
			CalibEntry calibEntry = (CalibEntry)database[i];
			calibEntry.evaluated = false;
			if (calibEntry.valid)
			{
				num++;
				if (calibEntry.group != 0)
				{
					num2++;
				}
			}
			database.ResetItem(i);
		}
		if (num > 0)
		{
			statusBar.Text = "n: " + num;
		}
		if ((!evaluate && !MainForm.pcLayout) || num < 32 || num2 < 8)
		{
			return;
		}
		Vector[] array = new Vector[num];
		Vector[] array2 = new Vector[num];
		float[] delta = new float[num];
		float[] array3 = new float[num];
		float[] array4 = new float[num];
		byte[] array5 = new byte[num + 1];
		int num3 = 0;
		for (int i = 0; i < count; i++)
		{
			CalibEntry calibEntry = (CalibEntry)database[i];
			if (calibEntry.valid)
			{
				ref Vector reference = ref array[num3];
				reference = new Vector((float)calibEntry.gx / 24000f, (float)calibEntry.gy / 24000f, (float)calibEntry.gz / 24000f);
				ref Vector reference2 = ref array2[num3];
				reference2 = new Vector((float)calibEntry.mx / 24000f, (float)calibEntry.my / 24000f, (float)calibEntry.mz / 24000f);
				array5[num3] = calibEntry.group;
				num3++;
			}
		}
		int num4 = Optimize(array, array2, array5, delta);
		if (num4 < 200)
		{
			float num5 = CompleteEntries(delta);
			CheckOverflow(ref aG, ref bG);
			CheckOverflow(ref aM, ref bM);
			resValid = true;
			statusBar.Text = "n: " + num + "     i: " + num4 + "     Δ: " + num5.ToString("f2");
			if (nl.x != 0f || nl.y != 0f || nl.z != 0f)
			{
				StatusBar obj = statusBar;
				string text = obj.Text;
				obj.Text = text + "    nl: " + (nl.x * 100f).ToString("f1") + ", " + (nl.y * 100f).ToString("f1") + ", " + (nl.z * 100f).ToString("f1");
			}
		}
	}

	internal static void SetNonlinearity(bool use)
	{
		useNonLinearity = use;
	}

	private static void CheckOverflow(ref Matrix m, ref Vector v)
	{
		float num = Math.Max(Matrix.MaxDiff(m, Matrix.zero) * 16384f, Vector.MaxDiff(v, Vector.zero) * 24000f);
		if (num > 32767f)
		{
			m *= 32767f / num;
			v *= 32767f / num;
		}
	}

	private static void PutCoeff(byte[] data, int index, float value)
	{
		int num = (int)Math.Round(value);
		data[index] = (byte)num;
		data[index + 1] = (byte)(num >> 8);
	}

	internal static byte[] GetCoeff()
	{
		byte[] array = new byte[useNonLinearity ? 52 : 48];
		PutCoeff(array, 0, bG.x * 24000f);
		PutCoeff(array, 2, aG.x.x * 16384f);
		PutCoeff(array, 4, aG.x.y * 16384f);
		PutCoeff(array, 6, aG.x.z * 16384f);
		PutCoeff(array, 8, bG.y * 24000f);
		PutCoeff(array, 10, aG.y.x * 16384f);
		PutCoeff(array, 12, aG.y.y * 16384f);
		PutCoeff(array, 14, aG.y.z * 16384f);
		PutCoeff(array, 16, bG.z * 24000f);
		PutCoeff(array, 18, aG.z.x * 16384f);
		PutCoeff(array, 20, aG.z.y * 16384f);
		PutCoeff(array, 22, aG.z.z * 16384f);
		PutCoeff(array, 24, bM.x * 24000f);
		PutCoeff(array, 26, aM.x.x * 16384f);
		PutCoeff(array, 28, aM.x.y * 16384f);
		PutCoeff(array, 30, aM.x.z * 16384f);
		PutCoeff(array, 32, bM.y * 24000f);
		PutCoeff(array, 34, aM.y.x * 16384f);
		PutCoeff(array, 36, aM.y.y * 16384f);
		PutCoeff(array, 38, aM.y.z * 16384f);
		PutCoeff(array, 40, bM.z * 24000f);
		PutCoeff(array, 42, aM.z.x * 16384f);
		PutCoeff(array, 44, aM.z.y * 16384f);
		PutCoeff(array, 46, aM.z.z * 16384f);
		if (useNonLinearity)
		{
			int num = (int)Math.Round(nl.x * 2796f);
			int num2 = (int)Math.Round(nl.y * 2796f);
			int num3 = (int)Math.Round(nl.z * 2796f);
			array[48] = (byte)(num - 1);
			array[49] = (byte)(num2 - 1);
			array[50] = (byte)(num3 - 1);
			array[51] = byte.MaxValue;
		}
		return array;
	}

	internal static bool CoeffValid()
	{
		return resValid;
	}

	internal static void AddG(short x, short y, short z)
	{
		CalibEntry calibEntry = new CalibEntry();
		calibEntry.gx = x;
		calibEntry.gy = y;
		calibEntry.gz = z;
		if (database.Count < 16)
		{
			calibEntry.group = (byte)(1 + ((database.Count >> 2) & 1));
		}
		database.Add(calibEntry);
		dirty = true;
	}

	internal static void AddM(short x, short y, short z)
	{
		if (database.Count > 0)
		{
			CalibEntry calibEntry = (CalibEntry)database[database.Count - 1];
			if (calibEntry.mx == 0 && calibEntry.my == 0 && calibEntry.mz == 0)
			{
				calibEntry.mx = x;
				calibEntry.my = y;
				calibEntry.mz = z;
				database.ResetItem(database.Count - 1);
				dirty = true;
			}
		}
	}

	private static void AddVector(float psi, float theta, float phi)
	{
		Vector vector = new Vector(0f, 0f, 1f).TurnY(0f - theta).TurnX(0f - phi);
		vector = aG * (vector - bG);
		vector.x -= nl.x * (vector.x * vector.x - 0.5f) * (1f - 2f * nl.x * vector.x);
		vector.y -= nl.y * (vector.y * vector.y - 0.5f) * (1f - 2f * nl.y * vector.y);
		vector.z -= nl.z * (vector.z * vector.z - 0.5f) * (1f - 2f * nl.z * vector.z);
		AddG((short)Math.Round(vector.x * 24000f), (short)Math.Round(vector.y * 24000f), (short)Math.Round(vector.z * 24000f));
		Vector vector2 = new Vector(1f, 0f, 0f).TurnZ(0f - psi).TurnY(0f - theta).TurnX(0f - phi);
		vector2 = aM * (vector2 - bM);
		AddM((short)Math.Round(vector2.x * 24000f), (short)Math.Round(vector2.y * 24000f), (short)Math.Round(vector2.z * 24000f));
	}

	private static void AddVectors(float psi, float theta)
	{
		AddVector(psi, theta, 0f);
		AddVector(psi, theta, (float)Math.PI / 2f);
		AddVector(psi, theta, -(float)Math.PI);
		AddVector(psi, theta, -(float)Math.PI / 2f);
		if (theta != 0f)
		{
			AddVector(psi, 0f - theta, 0f);
			AddVector(psi, 0f - theta, (float)Math.PI / 2f);
			AddVector(psi, 0f - theta, -(float)Math.PI);
			AddVector(psi, 0f - theta, -(float)Math.PI / 2f);
		}
	}

	private static void AddAllVectors()
	{
		AddVectors(0f, 0f);
		AddVectors((float)Math.PI / 2f, 0f);
		AddVectors((float)Math.PI, 0f);
		AddVectors(4.712389f, 0f);
		AddVectors(0f, (float)Math.PI / 2f);
		AddVectors((float)Math.PI / 4f, 0.6154797f);
		AddVectors((float)Math.PI * 3f / 4f, 0.6154797f);
		AddVectors(3.9269907f, 0.6154797f);
		AddVectors(5.497787f, 0.6154797f);
	}

	private static float GetCoeff(byte[] data, int index)
	{
		return (short)(data[index] + (data[index + 1] << 8));
	}

	internal static void ReadDevice(byte[] data)
	{
		database.Clear();
		bG.x = GetCoeff(data, 0) / 24000f;
		aG.x.x = GetCoeff(data, 2) / 16384f;
		aG.x.y = GetCoeff(data, 4) / 16384f;
		aG.x.z = GetCoeff(data, 6) / 16384f;
		bG.y = GetCoeff(data, 8) / 24000f;
		aG.y.x = GetCoeff(data, 10) / 16384f;
		aG.y.y = GetCoeff(data, 12) / 16384f;
		aG.y.z = GetCoeff(data, 14) / 16384f;
		bG.z = GetCoeff(data, 16) / 24000f;
		aG.z.x = GetCoeff(data, 18) / 16384f;
		aG.z.y = GetCoeff(data, 20) / 16384f;
		aG.z.z = GetCoeff(data, 22) / 16384f;
		bM.x = GetCoeff(data, 24) / 24000f;
		aM.x.x = GetCoeff(data, 26) / 16384f;
		aM.x.y = GetCoeff(data, 28) / 16384f;
		aM.x.z = GetCoeff(data, 30) / 16384f;
		bM.y = GetCoeff(data, 32) / 24000f;
		aM.y.x = GetCoeff(data, 34) / 16384f;
		aM.y.y = GetCoeff(data, 36) / 16384f;
		aM.y.z = GetCoeff(data, 38) / 16384f;
		bM.z = GetCoeff(data, 40) / 24000f;
		aM.z.x = GetCoeff(data, 42) / 16384f;
		aM.z.y = GetCoeff(data, 44) / 16384f;
		aM.z.z = GetCoeff(data, 46) / 16384f;
		if (data.Length >= 52)
		{
			nl.x = (float)((sbyte)data[48] + 1) / 2796f;
			nl.y = (float)((sbyte)data[49] + 1) / 2796f;
			nl.z = (float)((sbyte)data[50] + 1) / 2796f;
		}
		else
		{
			nl = Vector.zero;
		}
		aG = Matrix.Inverse(aG);
		aM = Matrix.Inverse(aM);
		AddAllVectors();
		dirty = true;
		Evaluate(evaluate: true);
	}

	internal static void New()
	{
		database.Clear();
		dirty = false;
		Evaluate(evaluate: false);
	}

	internal static void Initial()
	{
		database.Clear();
		aG = Matrix.one;
		aM = Matrix.one;
		bG = Vector.zero;
		bM = Vector.zero;
		nl = Vector.zero;
		AddAllVectors();
		dirty = false;
		Evaluate(evaluate: true);
	}

	internal static void Read(BinaryReader r, int version)
	{
		database.RaiseListChangedEvents = false;
		try
		{
			database.Clear();
			int num = r.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				CalibEntry calibEntry = new CalibEntry();
				calibEntry.Read(r, version);
				database.Add(calibEntry);
			}
		}
		finally
		{
			database.RaiseListChangedEvents = true;
			database.ResetBindings(metadataChanged: false);
		}
		Evaluate(evaluate: true);
		dirty = false;
	}

	internal static void Write(BinaryWriter w)
	{
		int count = database.Count;
		w.Write(count);
		for (int i = 0; i < count; i++)
		{
			CalibEntry calibEntry = (CalibEntry)database[i];
			calibEntry.Write(w);
		}
		dirty = false;
	}

	private static void WriteVector(TextWriter w, Vector v, float d)
	{
		w.WriteLine(string.Format(Exporter.inv, "{0,7:f3}{1,7:f3}{2,7:f3}{3,9:f3}", v.x, v.y, v.z, d));
	}

	private static void WriteIVector(TextWriter w, Vector v, float d)
	{
		w.WriteLine(string.Format(Exporter.inv, "{0,7:f0}{1,7:f0}{2,7:f0}{3,9:f0}", v.x * 16384f, v.y * 16384f, v.z * 16384f, d * 24000f));
	}

	private static void WriteResult(TextWriter w)
	{
		w.WriteLine();
		w.WriteLine("G");
		if (nl.x != 0f || nl.y != 0f || nl.z != 0f)
		{
			w.WriteLine(string.Format(Exporter.inv, "{0,7:f3}{1,7:f3}{2,7:f3}", nl.x, nl.y, nl.z));
		}
		WriteVector(w, aG.x, bG.x);
		WriteVector(w, aG.y, bG.y);
		WriteVector(w, aG.z, bG.z);
		w.WriteLine("M");
		WriteVector(w, aM.x, bM.x);
		WriteVector(w, aM.y, bM.y);
		WriteVector(w, aM.z, bM.z);
		w.WriteLine();
		w.WriteLine("Cg");
		if (nl.x != 0f || nl.y != 0f || nl.z != 0f)
		{
			w.WriteLine(string.Format(Exporter.inv, "{0,7:f0}{1,7:f0}{2,7:f0}", nl.x * 2796f, nl.y * 2796f, nl.z * 2796f));
		}
		WriteIVector(w, aG.x, bG.x);
		WriteIVector(w, aG.y, bG.y);
		WriteIVector(w, aG.z, bG.z);
		w.WriteLine("Cm");
		WriteIVector(w, aM.x, bM.x);
		WriteIVector(w, aM.y, bM.y);
		WriteIVector(w, aM.z, bM.z);
	}

	internal static void SaveText(TextWriter w)
	{
		w.WriteLine("    Gx     Gy     Gz     Mx     My     Mz    Azim   Incl   Roll   |G|    |M|     a      d");
		int count = database.Count;
		for (int i = 0; i < count; i++)
		{
			CalibEntry calibEntry = (CalibEntry)database[i];
			calibEntry.WriteText(w);
		}
		if (resValid)
		{
			WriteResult(w);
		}
	}

	internal static string GetCoeffText()
	{
		StringWriter stringWriter = new StringWriter();
		WriteResult(stringWriter);
		return stringWriter.ToString();
	}
}
