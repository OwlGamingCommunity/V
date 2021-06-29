using System.Collections.Generic;

public class TattooArtist : GenericCharacterCustomization
{
	private List<int> m_lstCurrentTattoos = null;
	private CTattooDefinition m_PreviewTattoo = null;

	public TattooArtist() : base(EGUIID.TattooArtist)
	{
		SetNameAndCallbacks("Tattoo Artist", null, OnFinish, OnRequestShow, null, OnExit, OnRender);

		NetworkEvents.TattooArtist_GotPrice += GotPriceInfo;
		NetworkEvents.EnterTattooArtist_Response += OnEnterTattooArtistResponse;

		UIEvents.TattooArtist_Cancel += Tattoo_Cancel;
		UIEvents.TattooArtist_AddNew += Tattoo_AddNew;
		UIEvents.TattooArtist_Create += Tattoo_Create;
		UIEvents.TattooArtist_RemoveTattoo += Tattoo_RemoveTattoo;
		UIEvents.TattooArtist_ChangeZone += Tattoo_ChangeZone;
		UIEvents.TattooArtist_ChangeTattoo += Tattoo_ChangeTattoo;

		// TODO_TATTOOS: Remove this later, its hacky
		ClientTimerPool.CreateTimer((object[] parameters) =>
		{
			if (m_UI.IsVisible())
			{
				UpdateTattooCount();
			}
		}, 100, 0);
	}

	private void GotPriceInfo(float fPrice, bool bHasToken, uint numAdded, uint numRemoved)
	{
		string strPriceString = Helpers.FormatString("Cost: {0} {1}<br>{2} Added<br>{3} Removed", bHasToken ? "Free" : Helpers.FormatString("${0:0.00}", fPrice), bHasToken ? "(Legacy Character Tattoo Token)" : "", numAdded, numRemoved);
		m_UI.SetPriceString(strPriceString);
	}

	private bool OnRequestShow()
	{
		// Got to trigger a server event to handle skin and dimension
		NetworkEventSender.SendNetworkEvent_EnterTattooArtist();

		// false = don't show, wait for event
		return false;
	}

	// TODO_TATTOOS: Save an event and get this from data?
	private void OnEnterTattooArtistResponse(List<int> lstCurrentTattoos)
	{
		// TODO_CSHARP: Move these all after the other execute calls, and cache execute calls
		base.ForceShow();

		m_lstCurrentTattoos = lstCurrentTattoos;

		m_UI.AddTabContent_GenericButton("Add New Tattoo", UIEventID.TattooArtist_AddNew);

		foreach (int tattooID in m_lstCurrentTattoos)
		{
			UI_AddTattooByID(tattooID);
		}

		UpdateTattooStates();

		// get initial pricing info, mainly so we can see if we have a token or not
		RequestPricing();
	}

	private void OnRender()
	{

	}

	private void ClearPreviewTattoo()
	{
		m_PreviewTattoo = null;
		UpdateTattooStates();
	}

	private void UpdateTattooStates()
	{
		// Current tattoos
		if (m_lstCurrentTattoos != null)
		{
			EGender gender = DataHelper.GetLocalPlayerEntityData<EGender>(EDataNames.GENDER);

			RAGE.Elements.Player.LocalPlayer.ClearFacialDecorations();

			// reapply hair tattoo
			SkinHelpers.ApplyHairTattooForPlayer(RAGE.Elements.Player.LocalPlayer);

			foreach (int tattooID in m_lstCurrentTattoos)
			{
				CTattooDefinition tattooDef = TattooDefinitions.GetTattooDefinitionFromID(tattooID);
				if (tattooDef != null)
				{
					RAGE.Elements.Player.LocalPlayer.SetFacialDecoration(tattooDef.GetHash_Collection(), tattooDef.GetHash_Tattoo(gender));
				}
			}

			// Also add the preview tattoo if available (the one the user is currently adding)
			if (m_PreviewTattoo != null)
			{
				RAGE.Elements.Player.LocalPlayer.SetFacialDecoration(m_PreviewTattoo.GetHash_Collection(), m_PreviewTattoo.GetHash_Tattoo(gender));
			}

			UpdateTattooCount();
		}
	}

	private void OnExit()
	{
		m_lstCurrentTattoos = null;
		m_PreviewTattoo = null;
	}

	private void OnFinish()
	{
		NetworkEventSender.SendNetworkEvent_TattooArtist_Checkout(StoreSystem.CurrentStoreID, m_lstCurrentTattoos);
		m_lstCurrentTattoos = null;
		m_PreviewTattoo = null;
	}

	public void Tattoo_AddNew()
	{
		ClearErrorMessages();

		if (m_lstCurrentTattoos.Count >= CharacterConstants.MaxTattoos)
		{
			ShowErrorMessage("You have reached the maximum number of tattoos.");
		}
		else
		{
			ClearPreviewTattoo();
			m_UI.ShowTattooCreator();
		}
	}

	public void Tattoo_Cancel()
	{
		ClearPreviewTattoo();
	}

	private void UpdateTattooCount()
	{
		m_UI.SetName(Helpers.FormatString("{0} ({1}/{2})", m_strStoreName, m_lstCurrentTattoos.Count, CharacterConstants.MaxTattoos));
	}

	private void UI_AddTattooByID(int tattooID)
	{
		CTattooDefinition tattooDef = TattooDefinitions.GetTattooDefinitionFromID(tattooID);
		if (tattooDef != null)
		{
			m_UI.AddTabContent_GenericListItem(tattooDef.LocalizedName, tattooDef.Zone.ToString().Replace("ZONE_", ""), "Click to Remove", UIEventID.TattooArtist_RemoveTattoo, UIEventID.TattooArtist_OnMouseEnter, UIEventID.TattooArtist_OnMouseExit, tattooDef.ID);
		}
	}

	public void Tattoo_Create()
	{
		if (m_PreviewTattoo != null)
		{
			m_lstCurrentTattoos.Add(m_PreviewTattoo.ID);
			UI_AddTattooByID(m_PreviewTattoo.ID);
			UpdateTattooStates();
			RequestPricing();
		}
	}

	private void RemoveTattoo(int targetTattooID)
	{
		m_lstCurrentTattoos.Remove(targetTattooID);
		UpdateTattooStates();
		RequestPricing();
	}

	public void Tattoo_RemoveTattoo(string strElementName, int tattooID)
	{
		RemoveTattoo(tattooID);
		m_UI.DeleteTabContent_GenericListItem(strElementName);
	}

	public void Tattoo_ChangeZone(TattooZone zone)
	{
		// reset on change category
		ClearPreviewTattoo();

		foreach (var kvPair in TattooDefinitions.g_TattooDefinitions)
		{
			CTattooDefinition tattoo = kvPair.Value;

			if (tattoo.Zone == zone)
			{
				// only add it if we dont already have the tattoo on our character - tattoos are unique
				if (!m_lstCurrentTattoos.Contains(tattoo.ID))
				{
					EGender gender = DataHelper.GetLocalPlayerEntityData<EGender>(EDataNames.GENDER);
					if (tattoo.SupportsGender(gender))
					{
						m_UI.AddTattooListItem(tattoo.ID, tattoo.LocalizedName);
					}
				}
			}
		}

		m_UI.CommitTattooList();
	}

	public void Tattoo_ChangeTattoo(int tattooID)
	{
		CTattooDefinition tattooDef = null;
		foreach (var kvPair in TattooDefinitions.g_TattooDefinitions)
		{
			CTattooDefinition tattoo = kvPair.Value;

			if (tattoo.ID == tattooID)
			{
				tattooDef = tattoo;
				break;
			}
		}

		m_PreviewTattoo = tattooDef;
		UpdateTattooStates();
	}

	private void RequestPricing()
	{
		NetworkEventSender.SendNetworkEvent_TattooArtist_CalculatePrice(m_lstCurrentTattoos);
	}
}