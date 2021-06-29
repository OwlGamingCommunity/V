using System;
using System.Collections.Generic;

public enum EWorldPedType
{
	None = -1,
	TrashCollectorJob,
	MailManJob,
	BusDriverJob,
	TruckerJob,
	DeliveryDriverJob,
	TaxiDriverJob,
	BankTeller,
	DrivingTest,
	VehicleStore,
	VehicleRentalStore,
	FactionCreation,
	StoreCashier,
	TowClerk,
	TagRemoverJob,
	Christmas,
	Vub,
	MarijuanaSales,
	Dancer,
	LockSmith,
	Activity,
	Halloween
}

public class CWorldHintContainer
{
	public CWorldHintContainer(EScriptControlID controlID, string strMessage, WorldHintDrawDelegate callbackOnDraw, WorldHintInteractDelegate callbackOnInteract, RAGE.Vector3 vecPos,
	bool bHideOnRaycast, bool bFadeOnRayCast, float fMaxDistance = 1.5f, RAGE.Vector3 vecRot = null, bool bStrongFont = false, bool bDoInfrontCheck = false, float fAngleForInfrontCheck = 1.5f, bool bAllowInVehicle = false)
	{
		ControlID = controlID;
		Message = strMessage;
		CallbackOnDraw = callbackOnDraw;
		CallbackOnInteract = callbackOnInteract;
		Position = vecPos;
		HideOnRaycast = bHideOnRaycast;
		FadeOnRayCast = bFadeOnRayCast;
		MaxDistance = fMaxDistance;
		Rotation = vecRot;
		StrongFont = bStrongFont;
		DoInfrontCheck = bDoInfrontCheck;
		AngleForInfrontCheck = fAngleForInfrontCheck;
		AllowInVehicle = bAllowInVehicle;
	}

	public EScriptControlID ControlID { get; set; }
	public string Message { get; set; }
	public WorldHintDrawDelegate CallbackOnDraw { get; set; }
	public WorldHintInteractDelegate CallbackOnInteract { get; set; }
	public RAGE.Vector3 Position { get; set; }
	public bool HideOnRaycast { get; set; }
	public bool FadeOnRayCast { get; set; }
	public float MaxDistance { get; set; }
	public RAGE.Vector3 Rotation { get; set; }
	public bool StrongFont { get; set; }
	public bool DoInfrontCheck { get; set; }
	public float AngleForInfrontCheck { get; set; }
	public bool AllowInVehicle { get; set; }
}

public static class WorldPedManager
{
	static WorldPedManager()
	{

	}

	public static void Init()
	{
		RageEvents.RAGE_OnTick_LowFrequency += OnTick;
		RageEvents.RAGE_OnRender += OnRender;

		// TODO_RAGE_HACK: Fix peds sticking around from other servers / on reconnect
		RAGE.Vector3 vecPlayerPos = RAGE.Elements.Player.LocalPlayer.Position;
		RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.ClearAreaOfPeds, vecPlayerPos.X, vecPlayerPos.Y, vecPlayerPos.Z, 5000.0f, 1);
	}

	public static WeakReference<CWorldPed> CreatePed(EWorldPedType worldPedType, uint pedHash, RAGE.Vector3 vecPos, float fRotZ, uint dimension, TransmitAnimation animTransmit = null)
	{
		// Already got one at the same pos with same type etc? Don't create and return the dupe. This can happen due to server on join sending to player + all players. Rare race condition
		foreach (CWorldPed worldPed in m_lstWorldPeds)
		{
			if (worldPed.PedType == worldPedType && worldPed.PedHash == pedHash && worldPed.Position == vecPos && worldPed.RotZ == fRotZ && worldPed.Dimension == dimension)
			{
				return new WeakReference<CWorldPed>(worldPed);
			}
		}

		CWorldPed ped = new CWorldPed(worldPedType, pedHash, vecPos, fRotZ, dimension, animTransmit);
		m_lstWorldPeds.Add(ped);
		return new WeakReference<CWorldPed>(ped);
	}

	public static void DestroyPed(CWorldPed worldPed)
	{
		m_lstWorldPeds.Remove(worldPed);
		worldPed.Destroy();
	}

	private static void OnRender()
	{
		foreach (CWorldPed worldPed in m_lstWorldPeds)
		{
			// Handle world hint
			if (worldPed.IsStreamedIn())
			{
				if (worldPed.WorldHint != null)
				{
					CWorldHintContainer worldHint = worldPed.WorldHint;
					WorldHintManager.DrawExclusiveWorldHint(ScriptControls.GetKeyBoundToControl(worldHint.ControlID), worldHint.Message, worldHint.CallbackOnDraw, worldHint.CallbackOnInteract, worldHint.Position, worldPed.Dimension, worldHint.HideOnRaycast, worldHint.FadeOnRayCast, worldHint.MaxDistance,
						worldHint.Rotation, worldHint.StrongFont, worldHint.DoInfrontCheck, worldHint.AngleForInfrontCheck, worldHint.AllowInVehicle);
				}
			}
		}
	}

	private static void OnTick()
	{
		// Stream in/out peds
		uint playerDimension = RAGE.Elements.Player.LocalPlayer.Dimension;
		RAGE.Vector3 vecPlayerPos = RAGE.Elements.Player.LocalPlayer.Position;

		foreach (CWorldPed worldPed in m_lstWorldPeds)
		{
			bool streamIn = false;
			bool streamOut = false;

			// Do we match its dimension
			if (worldPed.Dimension == playerDimension)
			{
				// Are we close enough?
				float fDist = WorldHelper.GetDistance(vecPlayerPos, worldPed.Position);

				if (fDist < 50.0f) // we're in same dimension and close enough
				{
					streamIn = true;
				}
				else // we're in same dimension but no longer close enough
				{
					streamOut = true;
				}
			}
			else if (worldPed.Dimension != playerDimension && worldPed.IsStreamedIn()) // Dimension doesn't match but the ped is streamed in
			{
				streamOut = true;
			}

			if (streamIn && !worldPed.IsStreamedIn())
			{
				if (worldPed.Dimension == 0)
				{
					worldPed.StreamIn();
				}
				else
				{
					// TODO: does this work?
					worldPed.StreamIn();
				}
			}
			else if (streamOut && worldPed.IsStreamedIn())
			{
				worldPed.StreamOut();
			}
		}
	}

	private static List<CWorldPed> m_lstWorldPeds = new List<CWorldPed>();
}

public class CWorldPed
{
	public CWorldPed(EWorldPedType worldPedType, uint pedHash, RAGE.Vector3 vecPos, float fRotZ, uint dimension, TransmitAnimation animTransmit)
	{
		PedType = worldPedType;
		PedHash = pedHash;
		Position = vecPos;
		RotZ = fRotZ;
		Dimension = dimension;
		PedInstance = null;
		transmitAnim = animTransmit;
	}

	~CWorldPed()
	{
		Destroy();
	}

	public void Destroy()
	{
		if (BlipInstance != null)
		{
			BlipInstance.Destroy();
			BlipInstance = null;
		}

		StreamOut();
	}

	public void SetId(Int64 id)
	{
		Id = id;
	}

	public void SetDancerDetails(Int64 id, bool bAllowTip)
	{
		DancerId = id;
		DancerAllowTip = bAllowTip;
	}

	public void UpdateWorldHint(EScriptControlID controlID, string strMessage)
	{
		WorldHint.ControlID = controlID;
		WorldHint.Message = strMessage;
	}

	public void AddWorldInteraction(EScriptControlID controlID, string strMessage, WorldHintDrawDelegate callbackOnDraw, WorldHintInteractDelegate callbackOnInteract,
		bool bHideOnRaycast, bool bFadeOnRayCast, float fMaxDistance = 1.5f, RAGE.Vector3 vecRot = null, bool bStrongFont = false, bool bDoInfrontCheck = false, float fAngleForInfrontCheck = 1.5f, bool bAllowInVehicle = false)
	{
		WorldHint = new CWorldHintContainer(controlID, strMessage, callbackOnDraw, callbackOnInteract, Position, bHideOnRaycast, bFadeOnRayCast, fMaxDistance, vecRot, bStrongFont, bDoInfrontCheck, fAngleForInfrontCheck, bAllowInVehicle);
	}

	public void UpdateDimension(uint dimension)
	{
		Dimension = dimension;
	}

	public void AddBlip(uint sprite, bool bShortRange, string strMessage)
	{
		BlipInstance = new RAGE.Elements.Blip(sprite, Position, strMessage, shortRange: bShortRange);
	}

	public CWorldHintContainer GetWorldInteraction()
	{
		return WorldHint;
	}

	public bool IsStreamedIn()
	{
		return PedInstance != null;
	}

	public void StreamIn()
	{
		if (!m_bIsStreamingIn)
		{
			m_bIsStreamingIn = true;
			AsyncModelLoader.RequestAsyncLoad(PedHash, (uint modelLoaded) =>
			{
				PedInstance = new RAGE.Elements.Ped(modelLoaded, Position, RotZ, Dimension);
				m_bIsStreamingIn = false;

				if (PedType == EWorldPedType.Dancer && transmitAnim != null)
				{
					ApplyAnimation();
				}
			});
		}
	}

	public void StreamOut()
	{
		if (PedInstance != null)
		{
			if (PedType == EWorldPedType.Dancer)
			{
				RAGE.Game.Ai.StopAnimTask(PedInstance.Handle, transmitAnim.Dict, transmitAnim.Name, transmitAnim.Flags);
			}

			try
			{
				PedInstance.Destroy();
				PedInstance = null;
			}
			catch
			{

			}
		}
	}

	public void ApplyAnimation()
	{
		//We use a 1 second timer to ensure all peds have been streamed in properly before applying the animation. Without this the anim won't play inside interiors upon entering or when peds are restreamed in
		ClientTimerPool.CreateTimer((object[] parameters) =>
		{
			AsyncAnimLoader.RequestAsyncLoad(transmitAnim.Dict, (string strDictionary) =>
			{
				RAGE.Game.Ai.TaskPlayAnim(PedInstance.Handle, transmitAnim.Dict, transmitAnim.Name, 8.0f, 1.0f, 4000000, transmitAnim.Flags, 1.0f, false, false, false);
			});
		}, 1000, 1);
	}

	public bool LocalPlayerInRange(int range)
	{
		RAGE.Elements.Player player = RAGE.Elements.Player.LocalPlayer;
		return Math.Abs(Position.X - player.Position.X) < range &&
			   Math.Abs(Position.Y - player.Position.Y) < range &&
			   Math.Abs(Position.Z - player.Position.Z) < range &&
			   Dimension == player.Dimension;
	}

	public EWorldPedType PedType { get; }
	public Int64 Id { get; private set; }
	public Int64 DancerId { get; private set; }
	public bool DancerAllowTip { get; private set; }
	public uint PedHash { get; }
	public RAGE.Vector3 Position { get; }
	public float RotZ { get; }
	public uint Dimension { get; private set; }
	public RAGE.Elements.Ped PedInstance { get; set; } = null;
	public CWorldHintContainer WorldHint { get; set; } = null;
	public RAGE.Elements.Blip BlipInstance { get; set; } = null;
	private TransmitAnimation transmitAnim;

	private bool m_bIsStreamingIn = false;
}