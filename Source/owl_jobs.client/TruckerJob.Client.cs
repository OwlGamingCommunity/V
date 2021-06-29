class TruckerJobInstance : PickupDropoffBasedJob
{
	public TruckerJobInstance() : base(EJobID.TruckerJob, "Trucker", "Load Delivered!", "Load Collected!", "Deliver the load to the location", "Return to the Truck", "Load Pickup", "Load Dropoff", "Enter the Truck to begin",
		EVehicleType.TruckerJob, EWorldPedType.TruckerJob, 2680389410, new RAGE.Vector3(77.06548f, 6351.425f, 31.37587f), 123.09f, WorldPedDimension, new RAGE.Vector3(156.3927f, -3081.633f, 7.033777f), 215.841f, WorldPedDimension, 67)
	{

	}

	private const uint WorldPedDimension = 0;
}