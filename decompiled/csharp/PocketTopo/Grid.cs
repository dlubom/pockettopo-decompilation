namespace PocketTopo;

public abstract class Grid
{
	public abstract string ToString(MetricLocation loc);

	public abstract string GetE(MetricLocation loc);

	public abstract string GetN(MetricLocation loc);

	public abstract string GetH(MetricLocation loc);

	public abstract bool PutE(ref MetricLocation loc, string s);

	public abstract bool PutN(ref MetricLocation loc, string s);

	public abstract bool PutH(ref MetricLocation loc, string s);
}
