using System;

namespace PocketTopo;

internal struct Vector
{
	public float x;

	public float y;

	public float z;

	public static Vector zero = new Vector(0f, 0f, 0f);

	public Vector(float x, float y, float z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public static Vector operator +(Vector a, Vector b)
	{
		return new Vector(a.x + b.x, a.y + b.y, a.z + b.z);
	}

	public static Vector operator -(Vector a, Vector b)
	{
		return new Vector(a.x - b.x, a.y - b.y, a.z - b.z);
	}

	public static Vector operator *(Vector a, float b)
	{
		return new Vector(a.x * b, a.y * b, a.z * b);
	}

	public static float operator *(Vector a, Vector b)
	{
		return a.x * b.x + a.y * b.y + a.z * b.z;
	}

	public static Vector operator %(Vector a, Vector b)
	{
		return new Vector(a.y * b.z - a.z * b.y, a.z * b.x - a.x * b.z, a.x * b.y - a.y * b.x);
	}

	public static Matrix operator &(Vector a, Vector b)
	{
		return new Matrix(b * a.x, b * a.y, b * a.z);
	}

	public static float Abs(Vector a)
	{
		return (float)Math.Sqrt(a.x * a.x + a.y * a.y + a.z * a.z);
	}

	public Vector TurnX(float a)
	{
		float num = (float)Math.Sin(a);
		float num2 = (float)Math.Cos(a);
		return new Vector(x, num2 * y - num * z, num2 * z + num * y);
	}

	public Vector TurnY(float a)
	{
		float num = (float)Math.Sin(a);
		float num2 = (float)Math.Cos(a);
		return new Vector(num2 * x + num * z, y, num2 * z - num * x);
	}

	public Vector TurnZ(float a)
	{
		float num = (float)Math.Sin(a);
		float num2 = (float)Math.Cos(a);
		return new Vector(num2 * x - num * y, num2 * y + num * x, z);
	}

	public static Vector Normalized(Vector a)
	{
		return a * (1f / Abs(a));
	}

	public static float MaxDiff(Vector a, Vector b)
	{
		return Math.Max(Math.Abs(a.x - b.x), Math.Max(Math.Abs(a.y - b.y), Math.Abs(a.z - b.z)));
	}

	public override string ToString()
	{
		return "[" + x + "," + y + "," + z + "]";
	}
}
