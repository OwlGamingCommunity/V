public static class CharacterLook
{
    public static void Init()
    {
        NetworkEvents.ShowCharacterLook += OpenShowCharacterLookUI;
        NetworkEvents.ShowUpdateCharacterLookUI += OpenUpdateCharacterLookUI;

        UIEvents.UpdateCharacterLook_Save += SaveCharacterLook;
        UIEvents.UpdateCharacterLook_Close += CloseUpdateUI;
        UIEvents.CharacterLook_Close += CloseLookUI;
    }

    private static void OpenShowCharacterLookUI(long characterID, string characterName, int age, int height, int weight,
        string physicalAppearance, string scars, string tattoos, string makeup, int createdAt, int updatedAt)
    {
        g_LookUI.SetVisible(true, true, false);
        g_LookUI.SetData(characterName, age, height, weight, physicalAppearance, scars, tattoos, makeup, createdAt, updatedAt);
    }

    private static void OpenUpdateCharacterLookUI(long characterid, string charactername, int height,
        int weight, string physicalAppearance, string scars, string tattoos, string makeup, int createdAt,
        int updatedAt)
    {
        g_UpdateLookUI.SetVisible(true, true, false);
        g_UpdateLookUI.SetData(charactername, height, weight, physicalAppearance, scars, tattoos, makeup, createdAt, updatedAt);
    }

    private static void CloseLookUI()
    {
        g_LookUI.SetVisible(false, false, false);
    }

    private static void CloseUpdateUI()
    {
        g_UpdateLookUI.SetVisible(false, false, false);
    }

    private static void SaveCharacterLook(int height, int weight, string physicalappearance, string scars, string tattoos, string makeup)
    {
        NetworkEventSender.SendNetworkEvent_UpdateCharacterLook(height, weight, physicalappearance, scars, tattoos, makeup);
        g_UpdateLookUI.SetVisible(false, false, false);
    }

    private static CGUIShowCharacterLook g_LookUI = new CGUIShowCharacterLook(OnUILoaded);
    private static CGUIUpdateCharacterLook g_UpdateLookUI = new CGUIUpdateCharacterLook(OnUILoaded);

    private static void OnUILoaded()
    {
        // no-op
    }
}