using System.Collections.Generic;

public class PDGates
{
	List<RAGE.Elements.MapObject> m_lstObjects = new List<RAGE.Elements.MapObject>();
	public PDGates()
	{
		uint hash = HashHelper.GetHashUnsigned("prop_conslift_door");
		AsyncModelLoader.RequestAsyncLoad(hash, (uint modelLoaded) =>
		{
			// x is to/away from pd
			// y is back towards the forest
			//m_lstObjects.Add(new RAGE.Elements.MapObject(modelLoaded, new RAGE.Vector3(-450.778f, 6043.207f, 34.0f), new RAGE.Vector3(315.0f, 90.0f, 135.0f)));
		});
	}
}