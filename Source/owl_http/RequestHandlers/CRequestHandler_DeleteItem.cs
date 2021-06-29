using Newtonsoft.Json;
using System;
using System.Net;

internal class CRequestHandler_DeleteItem : CRequestHandler
{
	public CRequestHandler_DeleteItem() : base("DeleteItem")
	{

	}

	public override string Post(HttpListenerRequest request)
	{
		JSONRequest_DeleteItem json = JsonConvert.DeserializeObject<JSONRequest_DeleteItem>(StreamedText());
		if (json != null && json.itemID != 0 && json.itemValue.Length != 0)
		{

			JSONResponse_DeleteItem response;

			if (Enum.IsDefined(typeof(EItemID), json.itemID))
			{
				EItemID itemID = (EItemID)json.itemID;
				CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromJSONStringNoDBID(itemID, json.itemValue);
				HelperFunctions.Items.DeleteAllItems(ItemInstanceDef);

				response = new JSONResponse_DeleteItem()
				{
					success = true
				};
			}
			else
			{
				response = new JSONResponse_DeleteItem()
				{
					success = false,
					reason = "Improper item ID."
				};
			}

			return JsonConvert.SerializeObject(response);
		}

		return null;
	}
}

internal class JSONRequest_DeleteItem
{
	public int itemID = 0;
	public string itemValue = string.Empty;
}

internal class JSONResponse_DeleteItem : JSONResponse
{

}

