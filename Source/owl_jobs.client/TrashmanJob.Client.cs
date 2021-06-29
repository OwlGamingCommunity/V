class TrashmanJobInstance : CollectionCheckpointBasedJob
{
	public TrashmanJobInstance() : base(EJobID.TrashmanJob, "Trash Collector", "Trash Collected!", "Return to the Trash Truck", "Trash Collection Location", "Enter the Trash Truck to begin", EVehicleType.TrashmanJob, EWorldPedType.TrashCollectorJob, 411102470,
		new RAGE.Vector3(-197.978f, 6294.025f, 31.4968f), 7.23f, WorldPedDimension, new RAGE.Vector3(-628.038f, -1636.495f, 25.97497f), 181.462f, WorldPedDimension, 318)
	{

	}

	private const uint WorldPedDimension = 0;
}