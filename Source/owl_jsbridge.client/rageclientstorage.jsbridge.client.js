function FlushRageClientStorage(jsonData)
{
	// wipe first (we cant set mp.storage.data directly...)
	for (var oldData in mp.storage.data)
	{
		delete mp.storage.data[oldData]; 
	}

	// write new data
	var serializedJsonData = JSON.parse(jsonData);
	for (var newData in serializedJsonData)
	{
		mp.storage.data[newData] = serializedJsonData[newData];
	}

	mp.storage.flush();
}
mp.events.add("FlushRageClientStorage", FlushRageClientStorage);

function InitRageClientStorage()
{
	var storageData = "{}";
	if (mp.storage.data != null)
	{
		storageData = JSON.stringify(mp.storage.data)
	}

	mp.events.callLocal("CE_LEGACY", "LoadRageClientStorage", storageData);
}
mp.events.add("InitRageClientStorage", InitRageClientStorage);