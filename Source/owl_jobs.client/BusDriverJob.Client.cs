class BusDriverJobInstance : CollectionCheckpointBasedJob
{
	public BusDriverJobInstance() : base(EJobID.BusDriverJob, "Bus Driver", "Passengers Picked Up!", "Return to the Bus", "Bus Stop", "Enter the Bus to begin", EVehicleType.BusDriverJob, EWorldPedType.BusDriverJob, 411102470,
		new RAGE.Vector3(-275.4709f, 6074.685f, 31.43427f), 86.2f, WorldPedDimension, new RAGE.Vector3(430.6038f, -655.7892f, 28.73511f), 89.9f, WorldPedDimension, 513)
	{

	}

	private const uint WorldPedDimension = 0;
}