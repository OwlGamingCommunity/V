using System.Collections.Generic;

public class AnimationsUI
{
	private string m_strCommandForDeletion = string.Empty;
	public AnimationsUI()
	{
		NetworkEvents.ShowAnimationList += ShowAnimationWindow;

		UIEvents.CustomAnimationDeletion_Yes += OnYes;
		UIEvents.CustomAnimationDeletion_No += OnNo;
	}

	private void ShowAnimationWindow(List<CAnimationCommand> animationsList)
	{
		List<string> categories = new List<string>
		{
			"all"
		};

		foreach (CAnimationCommand animationCommand in animationsList)
		{
			if (!categories.Contains(animationCommand.category))
			{
				categories.Add(animationCommand.category);
			}
		}

		m_animationsUI.Initialize(animationsList);
		m_animationsUI.LoadCategories(categories);

		m_animationsUI.SetVisible(true, true, false);
		m_strCommandForDeletion = string.Empty;
	}

	public void OnCloseAnimationUI()
	{
		m_animationsUI.SetVisible(false, false, false);
	}

	public void OnPlayAnimationFromUI(string commandName)
	{
		NetworkEventSender.SendNetworkEvent_RequestStopAnimation();
		NetworkEventSender.SendNetworkEvent_PlayerRawCommand(commandName);
	}

	private void OnYes()
	{
		NotificationManager.ShowNotification(Helpers.FormatString("Anim Command Deletion"), Helpers.FormatString("The command /{0} was deleted.", m_strCommandForDeletion), ENotificationIcon.ExclamationSign);

		NetworkEventSender.SendNetworkEvent_CustomAnim_Delete(m_strCommandForDeletion);
		m_strCommandForDeletion = string.Empty;
	}

	private void OnNo()
	{
		NotificationManager.ShowNotification(Helpers.FormatString("Anim Command Deletion"), Helpers.FormatString("The command /{0} was NOT deleted.", m_strCommandForDeletion), ENotificationIcon.ExclamationSign);
		m_strCommandForDeletion = string.Empty;
	}

	public void OnDeleteCustomAnimCmd(string commandName)
	{
		m_strCommandForDeletion = commandName;
		GenericPromptHelper.ShowPrompt("Confirmation", Helpers.FormatString("Are you SURE you want to delete /{0} as custom animation?", commandName), "Yes, delete it", "No! Keep it.", UIEventID.CustomAnimationDeletion_Yes, UIEventID.CustomAnimationDeletion_No);
	}

	public void OnCreateCustomAnimation(string commandName, string animDictionary, string animName, bool loop, bool stopOnLastFrame, bool onlyAnimateUpperBody, bool allowPlayerMovement, int durationSeconds)
	{
		NetworkEventSender.SendNetworkEvent_CustomAnim_Create(commandName, animDictionary, animName, loop, stopOnLastFrame, onlyAnimateUpperBody, allowPlayerMovement, durationSeconds, false);
	}

	private CGUIAnimations m_animationsUI = new CGUIAnimations(() => { });
}