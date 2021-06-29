#pragma warning disable 0649
using System.Collections.Generic;

public enum EMapSourceType
{
	MENYOO,
	YMAP_RAW
}

public enum EMapType
{
	Interior,
	Persistent
}

public class OwlMapFile
{
	public EMapSourceType MapSourceType { get; set; }
	public EMapType MapType { get; set; }
	public string MapName { get; set; }
	public List<OwlMapObject> MapData { get; set; } = new List<OwlMapObject>();
	public long PropertyID { get; set; }
	public float MarkerX { get; set; }
	public float MarkerY { get; set; }
	public float MarkerZ { get; set; }
}

public class OwlMapObject
{
	public string model { get; set; }
	public float x { get; set; }
	public float y { get; set; }
	public float z { get; set; }
	public float rx { get; set; }
	public float ry { get; set; }
	public float rz { get; set; }
	public float rw { get; set; }
}
#pragma warning restore 0649