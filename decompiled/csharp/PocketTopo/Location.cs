namespace PocketTopo;

public struct Location
{
	public double lat;

	public double lon;

	public int alt;

	public Location(double lat, double lon, int alt)
	{
		this.lat = lat;
		this.lon = lon;
		this.alt = alt;
	}
}
