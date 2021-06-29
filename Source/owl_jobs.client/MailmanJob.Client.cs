class MailmanJobInstance : CollectionCheckpointBasedJob
{
	public MailmanJobInstance() : base(EJobID.MailmanJob, "Mail man", "Mail Delivered!", "Return to the Postal Truck", "Mail Box Location", "Enter the Postal Truck to begin", EVehicleType.MailmanJob, EWorldPedType.MailManJob, 411102470,
		new RAGE.Vector3(-406.3188f, 6148.999f, 31.6783f), 219.9f, WorldPedDimension, new RAGE.Vector3(78.4063f, 111.9975f, 81.16817f), 247.8353f, WorldPedDimension, 568)
	{
		RAGE.Game.Entity.CreateModelHide(60.31027f, 109.4036f, 78.17268f, 10.0f, 242636620, true);
		RAGE.Game.Entity.CreateModelHide(68.35451f, 106.4758f, 78.17268f, 10.0f, 406416082, true);
	}

	private const uint WorldPedDimension = 0;
}