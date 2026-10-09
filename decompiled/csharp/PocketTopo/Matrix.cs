using System;

namespace PocketTopo;

internal struct Matrix
{
	public Vector x;

	public Vector y;

	public Vector z;

	public static Matrix zero = new Matrix(Vector.zero, Vector.zero, Vector.zero);

	public static Matrix one = new Matrix(new Vector(1f, 0f, 0f), new Vector(0f, 1f, 0f), new Vector(0f, 0f, 1f));

	public Matrix(Vector x, Vector y, Vector z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public static Matrix operator +(Matrix a, Matrix b)
	{
		return new Matrix(a.x + b.x, a.y + b.y, a.z + b.z);
	}

	public static Matrix operator -(Matrix a, Matrix b)
	{
		return new Matrix(a.x - b.x, a.y - b.y, a.z - b.z);
	}

	public static Matrix operator *(Matrix a, float b)
	{
		return new Matrix(a.x * b, a.y * b, a.z * b);
	}

	public static Vector operator *(Matrix a, Vector b)
	{
		return new Vector(a.x * b, a.y * b, a.z * b);
	}

	public static Matrix operator *(Matrix a, Matrix b)
	{
		b = Transposed(b);
		return new Matrix(b * a.x, b * a.y, b * a.z);
	}

	public static Matrix Transposed(Matrix m)
	{
		float num = m.x.y;
		m.x.y = m.y.x;
		m.y.x = num;
		num = m.x.z;
		m.x.z = m.z.x;
		m.z.x = num;
		num = m.y.z;
		m.y.z = m.z.y;
		m.z.y = num;
		return m;
	}

	public static Matrix Inverse(Matrix m)
	{
		m = Transposed(m);
		Matrix matrix = new Matrix(m.y % m.z, m.z % m.x, m.x % m.y);
		return matrix * (1f / (m.x * matrix.x));
	}

	public static float MaxDiff(Matrix a, Matrix b)
	{
		return Math.Max(Vector.MaxDiff(a.x, b.x), Math.Max(Vector.MaxDiff(a.y, b.y), Vector.MaxDiff(a.z, b.z)));
	}

	public override string ToString()
	{
		return string.Concat("[", x, ",", y, ",", z, "]");
	}
}
