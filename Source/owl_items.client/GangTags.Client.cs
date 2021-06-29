using RAGE;
using RAGE.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using EntityDatabaseID = System.Int64;

// TODO_TAGGING: Dont let person go into inventory or other things when in tagging mode...
// TODO_TAGGING: Handle change character if in tagging mode

public class GangTags
{
    // For tagging mode / preview
    private List<GangTagLayer> m_GangTagLayers = new List<GangTagLayer>();

    private CGUISprayCanPrompt m_SprayCanPromptUI = null;

    private Int64 m_iTagInProgressID = -1;
    private bool m_bCurrentPositionValid = false;

    private const int numRenderTargets = 2;
    private string[] g_RenderObjectModels = new string[numRenderTargets] { "bkr_prop_rt_clubhouse_wall", "bkr_prop_rt_clubhouse_table" };
    private string[] g_RenderTargetNames = new string[numRenderTargets] { "clubhouse_wall", "clubhouse_table" };
    private int[] g_RenderTargetIDs = new int[numRenderTargets] { -1, -1 };
    private RAGE.Elements.MapObject[] g_TagObjects = new MapObject[numRenderTargets] { null, null };
    private EntityDatabaseID[] m_lastTags = new EntityDatabaseID[2] { -1, -1 };

    public GangTags()
    {
        NetworkEvents.GangTags_GotoTagMode += GotoTaggingMode;

        RageEvents.RAGE_OnRender += OnUpdateTaggingModeObjects;

        NetworkEvents.DestroyGangTag += OnDestroyGangTag;

        NetworkEvents.StartTaggingResponse += OnStartTaggingResponse;
        NetworkEvents.InformClientTaggingComplete += OnTaggingComplete;

        NetworkEvents.ClearNearbyTags += ClearNearbyTags;
        NetworkEvents.ListNearbyTags += ListNearbyTags;

        NetworkEvents.UseSprayCan += OnUseSprayCan;

        UIEvents.OnShareGangTag_Submit += OnShareGangTag_Submit;
        UIEvents.OnShareGangTag_Cancel += OnShareGangTag_Cancel;

        NetworkEvents.UpdateGangTagLayers += OnTagDataChanged;
        NetworkEvents.UpdateGangTagProgress += OnUpdateGangTagProgress;

        for (int i = 0; i < numRenderTargets; ++i)
        {
            AsyncModelLoader.RequestSyncInstantLoad(HashHelper.GetHashUnsigned(g_RenderObjectModels[i]));
            g_TagObjects[i] = new MapObject(HashHelper.GetHashUnsigned(g_RenderObjectModels[i]), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f), 255, 0);
        }
    }

    private void OnDestroyGangTag(EntityDatabaseID a_ID)
    {
        for (int i = 0; i < numRenderTargets; ++i)
        {
            if (m_lastTags[i] == a_ID)
            {
                m_lastTags[i] = -1;
            }
        }
    }


    private void OnTagDataChanged(EntityDatabaseID a_ID, List<GangTagLayer> lstLayers)
    {
        GangTagStorage tag = GangTagPool.GetGangTag(a_ID);

        if (tag != null)
        {
            tag.UpdateLayers(lstLayers);
        }
    }

    private void OnUpdateGangTagProgress(EntityDatabaseID a_ID, float fProgress)
    {
        GangTagStorage tag = GangTagPool.GetGangTag(a_ID);

        if (tag != null)
        {
            tag.UpdateProgress(fProgress);
        }
    }

    private void OnShareGangTag_Submit(string strInput)
    {
        NetworkEventSender.SendNetworkEvent_GangTags_ShareTag(strInput);
    }

    private void OnShareGangTag_Cancel()
    {
        // Nothing actually happened, do nothing, ui is already gone
    }

    private int GetRenderTargetForObject(int index)
    {
        if (g_RenderTargetIDs[index] == -1)
        {
            // create
            if (!RAGE.Game.Ui.IsNamedRendertargetRegistered(g_RenderTargetNames[index]))
            {
                RAGE.Game.Ui.RegisterNamedRendertarget(g_RenderTargetNames[index], false);
            }

            // Link it to all models
            if (!RAGE.Game.Ui.IsNamedRendertargetLinked(HashHelper.GetHashUnsigned(g_RenderObjectModels[index])))
            {
                RAGE.Game.Ui.LinkNamedRendertarget(HashHelper.GetHashUnsigned(g_RenderObjectModels[index]));
            }

            // Get the handle
            if (RAGE.Game.Ui.IsNamedRendertargetRegistered(g_RenderTargetNames[index]))
            {
                g_RenderTargetIDs[index] = RAGE.Game.Ui.GetNamedRendertargetRenderId(g_RenderTargetNames[index]);
            }
        }

        return g_RenderTargetIDs[index];
    }

    private bool IsRenderingTaggingModeTag()
    {
        return m_bShowTagPreview;
    }

    private void GotoTaggingMode(List<GangTagLayer> lstLayers)
    {
        m_bShowTagPreview = true;
        m_GangTagLayers = lstLayers;
    }

    private void OnStartTaggingResponse(bool bApproved, Int64 id)
    {
        if (bApproved)
        {
            m_bIsTaggingInProgress = true;
            m_iTagInProgressID = id;
            m_timeStartTag = DateTime.Now;
        }
        else
        {
            m_bIsTaggingInProgress = false;
            m_iTagInProgressID = -1;
        }
    }

    private void OnTaggingComplete()
    {
        m_bIsTaggingInProgress = false;
        m_iTagInProgressID = -1;
        m_bShowTagPreview = false;
    }

    private void CheckTaggingPosition_Internal(ECastDirection direction, out RAGE.Vector3 vecTaggingPos, out bool bIsValid)
    {
        vecTaggingPos = null;
        bIsValid = false;

        float fRaycastDistDirection = 1.0f;
        float fDirectionHeight = 0.7f;

        float fRaycastDist = 1.5f;

        RAGE.Vector3 vecRootPos = null;
        if (direction == ECastDirection.Center)
        {
            vecRootPos = RAGE.Elements.Player.LocalPlayer.Position.CopyVector();
        }
        else if (direction == ECastDirection.Upper)
        {
            vecRootPos = RAGE.Elements.Player.LocalPlayer.Position.CopyVector();
            vecRootPos.Z += fDirectionHeight;
        }
        else if (direction == ECastDirection.Lower)
        {
            vecRootPos = RAGE.Elements.Player.LocalPlayer.Position.CopyVector();
            vecRootPos.Z -= fDirectionHeight;
        }
        else if (direction == ECastDirection.LeftUpper)
        {
            vecRootPos = RAGE.Elements.Player.LocalPlayer.Position.CopyVector();
            float fRaycastRadians = (RAGE.Elements.Player.LocalPlayer.GetRotation(0).Z + 180.0f) * (3.14f / 180.0f);
            vecRootPos.X += (float)Math.Cos(fRaycastRadians) * fRaycastDistDirection;
            vecRootPos.Y += (float)Math.Sin(fRaycastRadians) * fRaycastDistDirection;
            vecRootPos.Z += fDirectionHeight;
        }
        else if (direction == ECastDirection.LeftCenter)
        {
            vecRootPos = RAGE.Elements.Player.LocalPlayer.Position.CopyVector();
            float fRaycastRadians = (RAGE.Elements.Player.LocalPlayer.GetRotation(0).Z + 180.0f) * (3.14f / 180.0f);
            vecRootPos.X += (float)Math.Cos(fRaycastRadians) * fRaycastDistDirection;
            vecRootPos.Y += (float)Math.Sin(fRaycastRadians) * fRaycastDistDirection;
        }
        else if (direction == ECastDirection.LeftLower)
        {
            vecRootPos = RAGE.Elements.Player.LocalPlayer.Position.CopyVector();
            float fRaycastRadians = (RAGE.Elements.Player.LocalPlayer.GetRotation(0).Z + 180.0f) * (3.14f / 180.0f);
            vecRootPos.X += (float)Math.Cos(fRaycastRadians) * fRaycastDistDirection;
            vecRootPos.Y += (float)Math.Sin(fRaycastRadians) * fRaycastDistDirection;
            vecRootPos.Z -= fDirectionHeight;
        }
        else if (direction == ECastDirection.RightUpper)
        {
            vecRootPos = RAGE.Elements.Player.LocalPlayer.Position.CopyVector();
            float fRaycastRadians = (RAGE.Elements.Player.LocalPlayer.GetRotation(0).Z) * (3.14f / 180.0f);
            vecRootPos.X += (float)Math.Cos(fRaycastRadians) * fRaycastDistDirection;
            vecRootPos.Y += (float)Math.Sin(fRaycastRadians) * fRaycastDistDirection;
            vecRootPos.Z += fDirectionHeight;
        }
        else if (direction == ECastDirection.RightCenter)
        {
            vecRootPos = RAGE.Elements.Player.LocalPlayer.Position.CopyVector();
            float fRaycastRadians = (RAGE.Elements.Player.LocalPlayer.GetRotation(0).Z) * (3.14f / 180.0f);
            vecRootPos.X += (float)Math.Cos(fRaycastRadians) * fRaycastDistDirection;
            vecRootPos.Y += (float)Math.Sin(fRaycastRadians) * fRaycastDistDirection;
        }
        else if (direction == ECastDirection.RightLower)
        {
            vecRootPos = RAGE.Elements.Player.LocalPlayer.Position.CopyVector();
            float fRaycastRadians = (RAGE.Elements.Player.LocalPlayer.GetRotation(0).Z) * (3.14f / 180.0f);
            vecRootPos.X += (float)Math.Cos(fRaycastRadians) * fRaycastDistDirection;
            vecRootPos.Y += (float)Math.Sin(fRaycastRadians) * fRaycastDistDirection;
            vecRootPos.Z -= fDirectionHeight;
        }

        if (vecRootPos != null)
        {
            RAGE.Vector3 vecTargetPos = vecRootPos.CopyVector();
            float fRaycastRadians = (RAGE.Elements.Player.LocalPlayer.GetRotation(0).Z + 90.0f) * (3.14f / 180.0f);
            vecTargetPos.X += (float)Math.Cos(fRaycastRadians) * fRaycastDist;
            vecTargetPos.Y += (float)Math.Sin(fRaycastRadians) * fRaycastDist;
            vecTargetPos.Z += 1.0f;

            CRaycastResult raycast = WorldHelper.RaycastFromTo(vecRootPos, vecTargetPos, RAGE.Elements.Player.LocalPlayer.Handle, 1);

            if (raycast.Hit)
            {
                vecTaggingPos = raycast.EndCoords;
                bIsValid = true;
            }
            else
            {
                vecTaggingPos = vecTargetPos;
            }

            //RAGE.Game.Graphics.DrawLine(RAGE.Elements.Player.LocalPlayer.Position.X, RAGE.Elements.Player.LocalPlayer.Position.Y, RAGE.Elements.Player.LocalPlayer.Position.Z, vecTaggingPos == null ? vecRootPos.X : vecTaggingPos.X, vecTaggingPos == null ? vecRootPos.Y : vecTaggingPos.Y, vecTaggingPos == null ? vecRootPos.Z : vecTaggingPos.Z, 255, 0, 0, 255);
        }
    }

    enum ECastDirection
    {
        Center,
        Upper,
        Lower,
        LeftUpper,
        LeftCenter,
        LeftLower,
        RightUpper,
        RightCenter,
        RightLower,
    }

    private List<GangTagStorage> GetNearbyTags(float fRange)
    {
        List<GangTagStorage> lstWithinRange = new List<GangTagStorage>();
        foreach (var kvPair in GangTagPool.GetAll())
        {
            GangTagStorage tag = kvPair.Value;
            if (RAGE.Elements.Player.LocalPlayer.Dimension == tag.Dimension)
            {
                RAGE.Vector3 vecTagPos = tag.Position;
                RAGE.Vector3 vecPlayerPos = RAGE.Elements.Player.LocalPlayer.Position;
                float fDistance = WorldHelper.GetDistance(vecPlayerPos, vecTagPos);

                if (fDistance <= fRange)
                {
                    lstWithinRange.Add(tag);
                }
            }
        }

        return lstWithinRange;
    }

    private void ClearNearbyTags()
    {
        const float fDistanceMaximumTagRange = 15.0f;
        List<GangTagStorage> lstTags = GetNearbyTags(fDistanceMaximumTagRange);
        foreach (GangTagStorage tag in lstTags)
        {
            NetworkEventSender.SendNetworkEvent_AdminClearGangTags(tag.ID);
        }
    }

    private void ListNearbyTags()
    {
        const float fDistanceMaximumTagRange = 15.0f;
        List<GangTagStorage> lstTags = GetNearbyTags(fDistanceMaximumTagRange);
        foreach (GangTagStorage tag in lstTags)
        {
            ChatHelper.PushMessage(EChatChannel.AdminActions, "Tag: {0}", tag.ID);
        }
    }

    private void IsTaggingPositionValid(out RAGE.Vector3 vecTaggingPos, out bool bIsValid, out string strReason)
    {
        bIsValid = true;
        strReason = String.Empty;

        // Check for other tags too nearby (we check this first, since its kind of annoying to find a suitable spot only to be told it's too near another tag)
        const float fDistanceFromOtherTagLimit = 2.0f;
        List<GangTagStorage> lstTags = GetNearbyTags(fDistanceFromOtherTagLimit);
        foreach (GangTagStorage tag in lstTags)
        {
            bIsValid = false;
            strReason = "Another tag is too nearby!";
            break;
        }

        const int numRaycasts = 9;
        RAGE.Vector3[] vecResults = new RAGE.Vector3[numRaycasts];
        bool[] bResults = new bool[numRaycasts];

        CheckTaggingPosition_Internal(ECastDirection.Center, out vecResults[0], out bResults[0]);

        if (bIsValid)
        {
            CheckTaggingPosition_Internal(ECastDirection.Upper, out vecResults[1], out bResults[1]);
            CheckTaggingPosition_Internal(ECastDirection.Lower, out vecResults[2], out bResults[2]);
            CheckTaggingPosition_Internal(ECastDirection.LeftUpper, out vecResults[3], out bResults[3]);
            CheckTaggingPosition_Internal(ECastDirection.LeftCenter, out vecResults[4], out bResults[4]);
            CheckTaggingPosition_Internal(ECastDirection.LeftLower, out vecResults[5], out bResults[5]);
            CheckTaggingPosition_Internal(ECastDirection.RightUpper, out vecResults[6], out bResults[6]);
            CheckTaggingPosition_Internal(ECastDirection.RightCenter, out vecResults[7], out bResults[7]);
            CheckTaggingPosition_Internal(ECastDirection.RightLower, out vecResults[8], out bResults[8]);

            // Coalesce results
            for (int i = 0; i < numRaycasts; ++i)
            {
                bIsValid &= bResults[i];
            }

            if (!bIsValid)
            {
                strReason = "Tag must be placed on a flat surface with enough space";
                vecTaggingPos = vecResults[0];
            }
            else
            {
                // infer real/safe position
                float fDist = (RAGE.Elements.Player.LocalPlayer.Position - vecResults[0]).Length() - 0.1f;
                RAGE.Vector3 vecPos = RAGE.Elements.Player.LocalPlayer.Position.CopyVector();
                float fRadians = (RAGE.Elements.Player.LocalPlayer.GetRotation(0).Z + 90.0f) * (3.14f / 180.0f);
                vecPos.X += (float)Math.Cos(fRadians) * fDist;
                vecPos.Y += (float)Math.Sin(fRadians) * fDist;
                vecTaggingPos = vecPos;
            }
        }
        else
        {
            vecTaggingPos = vecResults[0]; // infront
        }
    }

    private bool m_bShowTagPreview = false;
    private bool m_bTaggingStarted = false;
    private bool m_bIsTaggingInProgress = false;
    private DateTime m_timeStartTag = DateTime.Now;
    private int timeBetweenTagUpdates = 100;
    public void OnTagWall()
    {
        OnExitSprayCan();
        NetworkEventSender.SendNetworkEvent_RequestGotoTagMode();
    }

    public void OnEditTag()
    {
        OnExitSprayCan();
        NetworkEventSender.SendNetworkEvent_RequestEditTagMode();
    }

    public void OnShareTag()
    {
        OnExitSprayCan();
        UserInputHelper.RequestUserInput("Share Gang Tag", "Enter the Full Name of the Recipient", "Forename Surname", UIEventID.OnShareGangTag_Submit, UIEventID.OnShareGangTag_Cancel);
    }

    private void OnUseSprayCan()
    {
        ItemSystem.GetPlayerInventory()?.HideInventory();
        m_SprayCanPromptUI = new CGUISprayCanPrompt(() => { });
        m_SprayCanPromptUI.SetVisible(true, true, false);
    }

    public void OnExitSprayCan()
    {
        if (m_SprayCanPromptUI != null)
        {
            m_SprayCanPromptUI.SetVisible(false, false, false);
            m_SprayCanPromptUI = null;
        }
    }

    const float g_fDistThreshold = 100.0f;

    // TODO_TAGS: Do once per second or something, or multiframe worker
    private GangTagStorage[] GetNearestGangTags()
    {
        GangTagStorage[] outTags = new GangTagStorage[2] { null, null };

        const float fRange = 15.0f;

        // if we're tagging, the nearest is what we're drawing
        const int indexForInProgressTag = 0;
        if (IsRenderingTaggingModeTag() && m_iTagInProgressID != -1)
        {
            outTags[indexForInProgressTag] = GangTagPool.GetGangTag(m_iTagInProgressID);
        }

        Dictionary<GangTagStorage, float> dictNearest = new Dictionary<GangTagStorage, float>();
        foreach (var kvPair in GangTagPool.GetAll())
        {
            GangTagStorage tag = kvPair.Value;
            if (RAGE.Elements.Player.LocalPlayer.Dimension == tag.Dimension)
            {
                // we don't care how near it is if its our tagging one
                if (IsRenderingTaggingModeTag())
                {
                    if (tag == outTags[indexForInProgressTag])
                    {
                        continue;
                    }
                }

                RAGE.Vector3 vecTagPos = tag.Position;
                RAGE.Vector3 vecPlayerPos = RAGE.Elements.Player.LocalPlayer.Position;
                float fDistance = WorldHelper.GetDistance(vecPlayerPos, vecTagPos);

                if (fDistance <= fRange)
                {
                    dictNearest.Add(kvPair.Value, fDistance);
                }
            }
        }

        // TODO_TAGS: This func is super slow, do it less often
        var sortedDict = from entry in dictNearest orderby entry.Value ascending select entry;
        int len = sortedDict.Count();
        if (len >= 1)
        {
            // only if not tagging
            if (!IsRenderingTaggingModeTag())
                outTags[0] = sortedDict.ElementAt(0).Key;
        }

        if (len >= 2)
        {
            // only if not tagging
            if (!IsRenderingTaggingModeTag())
            {
                outTags[1] = sortedDict.ElementAt(1).Key;
            }
            else
            {
                outTags[1] = sortedDict.ElementAt(0).Key;
            }
        }

        return outTags;
    }

    private void RenderWorldTags()
    {
        // render nearest gang tag
        GangTagStorage[] nearestTags = GetNearestGangTags();
        // do we need to move / hide our objects?

        int start = IsRenderingTaggingModeTag() ? 1 : 0; // skip first if tagging, since 0 is the tag in progress
        for (int i = 0; i < numRenderTargets; ++i)
        {
            GangTagStorage tag = nearestTags[i];
            RAGE.Elements.MapObject obj = g_TagObjects[i];
            if (obj == null)
            {
                continue;
            }

            if (tag == null)
            {
                continue;
            }

            if (nearestTags[i] == null && m_lastTags[i] != -1)
            {
                obj.SetAlpha(0, false);
                obj.Position = new Vector3(0.0f, 0.0f, 0.0f);
                m_lastTags[i] = -1;
                continue;
            }

            // render layers
            int renderTargetID = GetRenderTargetForObject(i);
            if (renderTargetID != -1)
            {
                // set render target + rtt layer
                RAGE.Game.Ui.SetTextRenderId(renderTargetID);
                RAGE.Game.Graphics.Set2dLayer(4);

                obj.SetAlpha(255, false);
                obj.Position = tag.Position;
                obj.SetRotation(0.0f + i == 0 ? 0.0f : 90, 0.0f, tag.Rotation, 0, false);

                float fTagProgress = tag.Progress;

                if (!m_bIsTaggingInProgress && fTagProgress < 100.0f)
                {
                    // am I the owner?

                    Int64 tagOwner = tag.OwnerCharacterID;
                    Int64 charID = DataHelper.GetEntityData<Int64>(RAGE.Elements.Player.LocalPlayer, EDataNames.CHARACTER_ID);

                    if (tagOwner == charID)
                    {
                        Int64 tagID = tag.ID;
                        WorldHintManager.DrawExclusiveWorldHint(ScriptControls.GetKeyBoundToControl(EScriptControlID.Interact), "Resume Tagging", null, () => { ResumeTagging(tagID); }, tag.Position, tag.Dimension, false, false, 10.0f);
                    }
                }

                float fPercent = (float)(fTagProgress * (float)tag.Layers.Count);
                int inProgressLayerIndex = (int)Math.Floor(fPercent);
                float fProgressCurrentLayer = fPercent - (float)Math.Floor(fPercent);

                int renderLayerIndex = 0;
                foreach (var renderLayer in tag.Layers)
                {
                    if (renderLayerIndex < inProgressLayerIndex)
                    {
                        renderLayer.Render();
                    }
                    else if (renderLayerIndex == inProgressLayerIndex)
                    {
                        renderLayer.Render((int)(255 * fProgressCurrentLayer));
                    }

                    ++renderLayerIndex;
                }

                // Reset render target
                RAGE.Game.Ui.SetTextRenderId(1);
            }
        }
    }

    private void ResumeTagging(Int64 tagID)
    {
        m_bTaggingStarted = false;
        m_bIsTaggingInProgress = true;
        m_iTagInProgressID = tagID;
        m_timeStartTag = DateTime.Now;
    }

    int counter = 0;
    private void OnUpdateTaggingModeObjects()
    {
        RenderWorldTags();

        if (!m_bIsTaggingInProgress && !m_bShowTagPreview)
        {
            return;
        }

        if (KeyBinds.IsMouseButtonDown(0x01))
        {
            m_bTaggingStarted = true;
            if (!m_bIsTaggingInProgress && g_TagObjects[0] != null)
            {
                if (m_bCurrentPositionValid)
                {
                    RAGE.Elements.MapObject createTagObject = g_TagObjects[0];

                    ++counter;

                    // TODO: set counter and tagging back to 0
                    if (counter > 1)
                    {
                        counter = 0;
                        m_bIsTaggingInProgress = true;

                        NetworkEventSender.SendNetworkEvent_RequestStartTagging(createTagObject.Position.X, createTagObject.Position.Y, createTagObject.Position.Z, createTagObject.GetRotation(0).Z);
                    }
                }
            }
            else if (m_bIsTaggingInProgress)
            {
                double timeSinceLastProc = (DateTime.Now - m_timeStartTag).TotalMilliseconds;

                MapObject tagInProgressObject = g_TagObjects[0];
                if (tagInProgressObject != null)
                {
                    GangTagStorage tag = GangTagPool.GetGangTag(m_iTagInProgressID);
                    TextHelper.Draw2D(Helpers.FormatString("Tagging in progress: {0:0}%", tag.Progress), 0.5f, 0.8f, 0.4f, 255, 255, 255, 255, RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, true);
                }

                if (timeSinceLastProc >= timeBetweenTagUpdates)
                {
                    m_timeStartTag = DateTime.Now;
                    NetworkEventSender.SendNetworkEvent_UpdateTagging(m_iTagInProgressID);
                }
            }
        }
        else if (m_bIsTaggingInProgress && m_bTaggingStarted)
        {
            m_bIsTaggingInProgress = false;
            m_bTaggingStarted = false;

            NetworkEventSender.SendNetworkEvent_CancelTaggingInProgress();
            OnTaggingComplete();
        }
        else if (m_bIsTaggingInProgress && !m_bTaggingStarted)
        {
            TextHelper.Draw2D("Hold left mouse to begin tagging!", 0.5f, 0.8f, 0.4f, 255, 255, 255, 255, RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, true);
        }


        MapObject tagObject = g_TagObjects[0];
        if (tagObject != null && m_bShowTagPreview)
        {
            if (!m_bIsTaggingInProgress)
            {
                // check if valid
                IsTaggingPositionValid(out RAGE.Vector3 vecRenderTargetPos, out m_bCurrentPositionValid, out string strReason);
                // end checks

                if (vecRenderTargetPos != null)
                {
                    tagObject.Position = vecRenderTargetPos;
                    tagObject.SetRotation(0.0f, 0.0f, RAGE.Elements.Player.LocalPlayer.GetRotation(0).Z, 0, false);
                }

                // render layers
                int renderTargetID = GetRenderTargetForObject(0);
                if (renderTargetID != -1)
                {

                    tagObject.Dimension = Player.LocalPlayer.Dimension;

                    // set render target + rtt layer
                    RAGE.Game.Ui.SetTextRenderId(renderTargetID);
                    RAGE.Game.Graphics.Set2dLayer(4);

                    // background
                    RAGE.Game.Graphics.DrawRect(0.0f, 0.0f, 2.0f, 2.0f, m_bCurrentPositionValid ? 0 : 255, m_bCurrentPositionValid ? 255 : 0, 0, 255, 1);

                    tagObject.SetAlpha(255, false);

                    int layerIndex = 0;
                    foreach (var layer in m_GangTagLayers)
                    {
                        if (!m_bIsTaggingInProgress)
                        {
                            layer.Render();
                        }

                        ++layerIndex;
                    }

                    // Reset render target
                    RAGE.Game.Ui.SetTextRenderId(1);

                    TextHelper.Draw2D("Move around until you find a suitable slot for your tag", 0.5f, 0.8f, 0.4f, 255, 255, 255, 255, RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, true);
                    TextHelper.Draw2D("Press right mouse to cancel tagging", 0.5f, 0.83f, 0.4f, 255, 255, 255, 255, RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, true);

                    if (KeyBinds.IsMouseButtonDown(0x02))
                    {
                        ++counter;

                        if (counter > 1)
                        {
                            counter = 0;
                            if (tagObject != null)
                            {
                                tagObject.Destroy();
                                tagObject = null;

                                m_bIsTaggingInProgress = false;
                                m_iTagInProgressID = -1;
                                m_timeStartTag = DateTime.Now;
                            }
                        }
                    }

                    if (strReason.Length > 0)
                    {
                        TextHelper.Draw2D(Helpers.FormatString("Invalid Location: {0}", strReason), 0.5f, 0.86f, 0.4f, 255, 0, 0, 255, RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, true);
                    }
                    else
                    {
                        TextHelper.Draw2D("Valid Location - Hold left mouse to begin tagging!", 0.5f, 0.86f, 0.4f, 0, 255, 0, 255, RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, true);
                    }
                }
            }
        }

        if (tagObject != null && (m_bShowTagPreview || m_bIsTaggingInProgress))
        {
            ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.MeleeAttack1);
            ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.MeleeAttack2);
            ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.MeleeAttackAlternate);
            ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.MeleeAttackHeavy);
            ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.MeleeAttackLight);
            ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.Attack);
            ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.Attack2);
        }
    }
}

