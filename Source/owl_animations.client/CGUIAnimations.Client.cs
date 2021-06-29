using System.Collections.Generic;

internal class CGUIAnimations : CEFCore
{
	public CGUIAnimations(OnGUILoadedDelegate callbackOnLoad) : base("owl_animations.client/animations.html", EGUIID.AnimationsList, callbackOnLoad)
	{
		UIEvents.CloseAnimationUI += () => { AnimationSystem.GetAnimationsUI().OnCloseAnimationUI(); };
		UIEvents.DeleteCustomAnimCmd += (string commandName) => { AnimationSystem.GetAnimationsUI().OnDeleteCustomAnimCmd(commandName); };
		UIEvents.PlayAnimationFromUI += (string commandName) => { AnimationSystem.GetAnimationsUI().OnPlayAnimationFromUI(commandName); };
		UIEvents.CreateCustomAnimation += (string commandName, string animDictionary, string animName,
			bool loop, bool stopOnLastFrame, bool onlyAnimateUpperBody, bool allowPlayerMovement, int durationSeconds) =>
		{
			AnimationSystem.GetAnimationsUI().OnCreateCustomAnimation(commandName, animDictionary, animName, loop, stopOnLastFrame, onlyAnimateUpperBody, allowPlayerMovement, durationSeconds);
		};
	}

	public override void OnLoad()
	{

	}

	public void Initialize(List<CAnimationCommand> animationList)
	{
		Execute("loadAnimationData", OwlJSON.SerializeObject(animationList, EJsonTrackableIdentifier.InitializeAnimList));
	}

	public void LoadCategories(List<string> categories)
	{
		foreach (string category in categories)
		{
			Execute("loadCategory", category);
		}
	}
}