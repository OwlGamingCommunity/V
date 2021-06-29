class DeliveryDriverJobInstance : PickupDropoffBasedJob
{
	public DeliveryDriverJobInstance() : base(EJobID.DeliveryDriverJob, "Delivery Driver", "Packages Delivered!", "Packages Collected!", "Deliver the packages to the location", "Return to the Delivery Van", "Packages Pickup", "Packages Dropoff", "Enter the Delivery Van to begin",
		EVehicleType.DeliveryDriverJob, EWorldPedType.DeliveryDriverJob, 2680389410, new RAGE.Vector3(-73.25321f, 6280.05f, 31.44199f), 41.02f, WorldPedDimension, new RAGE.Vector3(204.5464f, -3132.774f, 5.79027f), 356.7152f, WorldPedDimension, 67)
	{

	}

	private const uint WorldPedDimension = 0;
}