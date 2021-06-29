using System;

public class RetuneRadio
{
	private Int64 m_CurrentRadioBeingRetuned = -1;

	public RetuneRadio()
	{
		NetworkEvents.RetuneRadio += OnRetuneRadio;

		UIEvents.OnRetuneRadio_Cancel += OnRetuneRadio_Cancel;
		UIEvents.OnRetuneRadio_Submit += OnRetuneRadio_Submit;
	}

	private void OnRetuneRadio(Int64 radioID, int radioChannel)
	{
		ItemSystem.GetPlayerInventory()?.HideInventory();
		m_CurrentRadioBeingRetuned = radioID;
		UserInputHelper.RequestUserInput("Tune Radio", "Enter the channel number, or enter -1 to turn the radio off.", radioChannel.ToString(), UIEventID.OnRetuneRadio_Submit, UIEventID.OnRetuneRadio_Cancel);
	}

	private void OnRetuneRadio_Cancel()
	{
		m_CurrentRadioBeingRetuned = -1;
	}

	private void OnRetuneRadio_Submit(string strInput)
	{
		int radioChannel;
		if (int.TryParse(strInput, out radioChannel))
		{
			NetworkEventSender.SendNetworkEvent_RetuneRadio(m_CurrentRadioBeingRetuned, radioChannel);
		}

		m_CurrentRadioBeingRetuned = -1;
		// TODO_POST_LAUNCH: Show error message if invalid?
	}
}