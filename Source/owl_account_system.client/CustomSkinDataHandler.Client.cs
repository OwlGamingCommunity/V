using ExtensionMethods;
using System;
using System.Collections.Generic;

public static class CustomSkinDataHandler
{
	private enum EMultiFrameTaskType
	{
		Clothing,
		SkinData,
		Prop
	}

	private enum ESkinDataChangeType
	{
		NONE,
		AGEING,
		MAKEUP,
		BLUSH,
		COMPLEXION,
		SUNDAMAGE,
		LIPSTICK,
		MOLESANDFRECKLES,
		NOSESIZEHORIZONTAL,
		NOSESIZEVERTICAL,
		NOSESIZEOUTWARDS,
		NOSESIZEOUTWARDSUPPER,
		NOSESIZEOUTWARDSLOWER,
		NOSEANGLE,
		EYEBROWHEIGHT,
		EYEBROWDEPTH,
		CHEEKBONEHEIGHT,
		CHEEKWIDTH,
		CHEEKWIDTHLOWER,
		EYESIZE,
		LIPSIZE,
		MOUTHSIZE,
		MOUTHSIZELOWER,
		CHINSIZE,
		CHINSIZELOWER,
		CHINWIDTH,
		CHINEFFECT,
		NECKWIDTH,
		NECKWIDTHLOWER,
		FACEBLEND,
		HAIRSTYLE,
		HAIRCOLOR,
		EYECOLOR,
		FACIALHAIRSTYLE,
		FACIALHAIRCOLOR,
		BLEMISHES,
		EYEBROWS,
		EYEBROWSCOLOR,
		MAKEUPCOLOR,
		BODYBLEM,
		CHESTHAIR,
		CHESTHAIRCOLOR,
		BLUSHCOLOR,
		LIPSTICKCOLOR,
		TATTOOS,
		BASEHAIR
	}



	private abstract class MultiFrameTask
	{
		public EMultiFrameTaskType TaskType { get; private set; }
		public RAGE.Elements.Player PlayerInstance { get; private set; }

		protected MultiFrameTask(RAGE.Elements.Player a_PlayerInstance, EMultiFrameTaskType a_TaskType)
		{
			TaskType = a_TaskType;
			PlayerInstance = a_PlayerInstance;
		}

		public abstract void Run();
	}

	private class SkinDataTask : MultiFrameTask
	{
		public SkinDataTask(RAGE.Elements.Player a_PlayerInstance, ESkinDataChangeType a_DataChanged) : base(a_PlayerInstance, EMultiFrameTaskType.SkinData)
		{
			DataChanged = a_DataChanged;
		}

		public override void Run()
		{
			bool bIsCustom = DataHelper.GetEntityData<bool>(PlayerInstance, EDataNames.IS_CUSTOM);
			if (bIsCustom)
			{
				if (DataChanged == ESkinDataChangeType.AGEING)
				{
					int AGEING = DataHelper.GetEntityData<int>(PlayerInstance, EDataNames.CC_AGEING);
					float AGEINGOPACITY = DataHelper.GetEntityData<float>(PlayerInstance, EDataNames.CC_AGEINGOPACITY);
					PlayerInstance.SetHeadOverlay(3, AGEING, AGEINGOPACITY);
					// Ageing color is not a thing in V
				}
				else if (DataChanged == ESkinDataChangeType.MAKEUP)
				{
					int MAKEUP = DataHelper.GetEntityData<int>(PlayerInstance, EDataNames.CC_MAKEUP);
					float MAKEUPOPACITY = DataHelper.GetEntityData<float>(PlayerInstance, EDataNames.CC_MAKEUPOPACITY);
					PlayerInstance.SetHeadOverlay(4, MAKEUP, MAKEUPOPACITY);
				}
				else if (DataChanged == ESkinDataChangeType.BLUSH)
				{
					int BLUSH = DataHelper.GetEntityData<int>(PlayerInstance, EDataNames.CC_BLUSH);
					float BLUSHOPACITY = DataHelper.GetEntityData<float>(PlayerInstance, EDataNames.CC_BLUSHOPACITY);
					PlayerInstance.SetHeadOverlay(5, BLUSH, BLUSHOPACITY);
				}
				else if (DataChanged == ESkinDataChangeType.COMPLEXION)
				{
					int COMPLEXION = DataHelper.GetEntityData<int>(PlayerInstance, EDataNames.CC_COMPLEXION);
					float COMPLEXIONOPACITY = DataHelper.GetEntityData<float>(PlayerInstance, EDataNames.CC_COMPLEXIONOPACITY);
					PlayerInstance.SetHeadOverlay(6, COMPLEXION, COMPLEXIONOPACITY);
					// Complexion color is not a thing in V
				}
				else if (DataChanged == ESkinDataChangeType.SUNDAMAGE)
				{
					int SUNDAMAGE = DataHelper.GetEntityData<int>(PlayerInstance, EDataNames.CC_SUNDAMAGE);
					float SUNDAMAGEOPACITY = DataHelper.GetEntityData<float>(PlayerInstance, EDataNames.CC_SUNDAMAGEOPACITY);
					PlayerInstance.SetHeadOverlay(7, SUNDAMAGE, SUNDAMAGEOPACITY);
					// Sundamage color is not a thing in V
				}
				else if (DataChanged == ESkinDataChangeType.LIPSTICK)
				{
					int LIPSTICK = DataHelper.GetEntityData<int>(PlayerInstance, EDataNames.CC_LIPSTICK);
					float LIPSTICKOPACITY = DataHelper.GetEntityData<float>(PlayerInstance, EDataNames.CC_LIPSTICKOPACITY);
					PlayerInstance.SetHeadOverlay(8, LIPSTICK, LIPSTICKOPACITY);
				}
				else if (DataChanged == ESkinDataChangeType.MOLESANDFRECKLES)
				{
					int MOLESANDFRECKLES = DataHelper.GetEntityData<int>(PlayerInstance, EDataNames.CC_MOLESANDFRECKLES);
					float MOLESANDFRECKLESOPACITY = DataHelper.GetEntityData<float>(PlayerInstance, EDataNames.CC_MOLESANDFRECKLESOPACITY);
					PlayerInstance.SetHeadOverlay(9, MOLESANDFRECKLES, MOLESANDFRECKLESOPACITY);
					// Moles & Freckles color is not a thing in V
				}
				else if (DataChanged == ESkinDataChangeType.NOSESIZEHORIZONTAL)
				{
					float NOSESIZEHORIZONTAL = DataHelper.GetEntityData<float>(PlayerInstance, EDataNames.CC_NOSESIZEHORIZONTAL);
					if (PlayerInstance.GetDrawableVariation((int)ECustomClothingComponent.Masks) != 0)
					{
						PlayerInstance.SetFaceFeature(0, 0.0f);
					}
					else
					{
						PlayerInstance.SetFaceFeature(0, NOSESIZEHORIZONTAL);
					}
				}
				else if (DataChanged == ESkinDataChangeType.NOSESIZEVERTICAL)
				{
					float NOSESIZEVERTICAL = DataHelper.GetEntityData<float>(PlayerInstance, EDataNames.CC_NOSESIZEVERTICAL);
					if (PlayerInstance.GetDrawableVariation((int)ECustomClothingComponent.Masks) != 0)
					{
						PlayerInstance.SetFaceFeature(1, 0.0f);
					}
					else
					{
						PlayerInstance.SetFaceFeature(1, NOSESIZEVERTICAL);
					}
				}
				else if (DataChanged == ESkinDataChangeType.NOSESIZEOUTWARDS)
				{
					float NOSESIZEOUTWARDS = DataHelper.GetEntityData<float>(PlayerInstance, EDataNames.CC_NOSESIZEOUTWARDS);
					if (PlayerInstance.GetDrawableVariation((int)ECustomClothingComponent.Masks) != 0)
					{
						PlayerInstance.SetFaceFeature(2, 0.0f);
					}
					else
					{
						PlayerInstance.SetFaceFeature(2, NOSESIZEOUTWARDS);
					}
				}
				else if (DataChanged == ESkinDataChangeType.NOSESIZEOUTWARDSUPPER)
				{
					float NOSESIZEOUTWARDSUPPER = DataHelper.GetEntityData<float>(PlayerInstance, EDataNames.CC_NOSESIZEOUTWARDSUPPER);
					if (PlayerInstance.GetDrawableVariation((int)ECustomClothingComponent.Masks) != 0)
					{
						PlayerInstance.SetFaceFeature(3, 0.0f);
					}
					else
					{
						PlayerInstance.SetFaceFeature(3, NOSESIZEOUTWARDSUPPER);
					}
				}
				else if (DataChanged == ESkinDataChangeType.NOSESIZEOUTWARDSLOWER)
				{
					float NOSESIZEOUTWARDSLOWER = DataHelper.GetEntityData<float>(PlayerInstance, EDataNames.CC_NOSESIZEOUTWARDSLOWER);
					if (PlayerInstance.GetDrawableVariation((int)ECustomClothingComponent.Masks) != 0)
					{
						PlayerInstance.SetFaceFeature(4, 0.0f);
					}
					else
					{
						PlayerInstance.SetFaceFeature(4, NOSESIZEOUTWARDSLOWER);
					}
				}
				else if (DataChanged == ESkinDataChangeType.NOSEANGLE)
				{
					float NOSEANGLE = DataHelper.GetEntityData<float>(PlayerInstance, EDataNames.CC_NOSEANGLE);
					if (PlayerInstance.GetDrawableVariation((int)ECustomClothingComponent.Masks) != 0)
					{
						PlayerInstance.SetFaceFeature(5, 0.0f);
					}
					else
					{
						PlayerInstance.SetFaceFeature(5, NOSEANGLE);
					}
				}
				else if (DataChanged == ESkinDataChangeType.EYEBROWHEIGHT)
				{
					float EYEBROWHEIGHT = DataHelper.GetEntityData<float>(PlayerInstance, EDataNames.CC_EYEBROWHEIGHT);
					if (PlayerInstance.GetDrawableVariation((int)ECustomClothingComponent.Masks) != 0)
					{
						PlayerInstance.SetFaceFeature(6, 0.0f);
					}
					else
					{
						PlayerInstance.SetFaceFeature(6, EYEBROWHEIGHT);
					}
				}
				else if (DataChanged == ESkinDataChangeType.EYEBROWDEPTH)
				{
					float EYEBROWDEPTH = DataHelper.GetEntityData<float>(PlayerInstance, EDataNames.CC_EYEBROWDEPTH);
					if (PlayerInstance.GetDrawableVariation((int)ECustomClothingComponent.Masks) != 0)
					{
						PlayerInstance.SetFaceFeature(7, 0.0f);
					}
					else
					{
						PlayerInstance.SetFaceFeature(7, EYEBROWDEPTH);
					}
				}
				else if (DataChanged == ESkinDataChangeType.CHEEKBONEHEIGHT)
				{
					float CHEEKBONEHEIGHT = DataHelper.GetEntityData<float>(PlayerInstance, EDataNames.CC_CHEEKBONEHEIGHT);
					if (PlayerInstance.GetDrawableVariation((int)ECustomClothingComponent.Masks) != 0)
					{
						PlayerInstance.SetFaceFeature(8, 0.0f);
					}
					else
					{
						PlayerInstance.SetFaceFeature(8, CHEEKBONEHEIGHT);
					}
				}
				else if (DataChanged == ESkinDataChangeType.CHEEKWIDTH)
				{
					float CHEEKWIDTH = DataHelper.GetEntityData<float>(PlayerInstance, EDataNames.CC_CHEEKWIDTH);
					if (PlayerInstance.GetDrawableVariation((int)ECustomClothingComponent.Masks) != 0)
					{
						PlayerInstance.SetFaceFeature(9, 0.0f);
					}
					else
					{
						PlayerInstance.SetFaceFeature(9, CHEEKWIDTH);
					}
				}
				else if (DataChanged == ESkinDataChangeType.CHEEKWIDTHLOWER)
				{
					float CHEEKWIDTHLOWER = DataHelper.GetEntityData<float>(PlayerInstance, EDataNames.CC_CHEEKWIDTHLOWER);
					if (PlayerInstance.GetDrawableVariation((int)ECustomClothingComponent.Masks) != 0)
					{
						PlayerInstance.SetFaceFeature(10, 0.0f);
					}
					else
					{
						PlayerInstance.SetFaceFeature(10, CHEEKWIDTHLOWER);
					}

				}
				else if (DataChanged == ESkinDataChangeType.EYESIZE)
				{
					float EYESIZE = DataHelper.GetEntityData<float>(PlayerInstance, EDataNames.CC_EYESIZE);
					PlayerInstance.SetFaceFeature(11, EYESIZE); // we keep eye since you can see it through some masks, e.g. balaclavas
				}
				else if (DataChanged == ESkinDataChangeType.LIPSIZE)
				{
					float LIPSIZE = DataHelper.GetEntityData<float>(PlayerInstance, EDataNames.CC_LIPSIZE);
					if (PlayerInstance.GetDrawableVariation((int)ECustomClothingComponent.Masks) != 0)
					{
						PlayerInstance.SetFaceFeature(12, 0.0f);
					}
					else
					{
						PlayerInstance.SetFaceFeature(12, LIPSIZE);
					}
				}
				else if (DataChanged == ESkinDataChangeType.MOUTHSIZE)
				{
					float MOUTHSIZE = DataHelper.GetEntityData<float>(PlayerInstance, EDataNames.CC_MOUTHSIZE);
					if (PlayerInstance.GetDrawableVariation((int)ECustomClothingComponent.Masks) != 0)
					{
						PlayerInstance.SetFaceFeature(13, 0.0f);
					}
					else
					{
						PlayerInstance.SetFaceFeature(13, MOUTHSIZE);
					}
				}
				else if (DataChanged == ESkinDataChangeType.MOUTHSIZELOWER)
				{
					float MOUTHSIZELOWER = DataHelper.GetEntityData<float>(PlayerInstance, EDataNames.CC_MOUTHSIZELOWER);
					if (PlayerInstance.GetDrawableVariation((int)ECustomClothingComponent.Masks) != 0)
					{
						PlayerInstance.SetFaceFeature(14, 0.0f);
					}
					else
					{
						PlayerInstance.SetFaceFeature(14, MOUTHSIZELOWER);
					}
				}
				else if (DataChanged == ESkinDataChangeType.CHINSIZE)
				{
					float CHINSIZE = DataHelper.GetEntityData<float>(PlayerInstance, EDataNames.CC_CHINSIZE);
					if (PlayerInstance.GetDrawableVariation((int)ECustomClothingComponent.Masks) != 0)
					{
						PlayerInstance.SetFaceFeature(15, 0.0f);
					}
					else
					{
						PlayerInstance.SetFaceFeature(15, CHINSIZE);
					}
				}
				else if (DataChanged == ESkinDataChangeType.CHINSIZELOWER)
				{
					float CHINSIZELOWER = DataHelper.GetEntityData<float>(PlayerInstance, EDataNames.CC_CHINSIZELOWER);
					if (PlayerInstance.GetDrawableVariation((int)ECustomClothingComponent.Masks) != 0)
					{
						PlayerInstance.SetFaceFeature(16, 0.0f);
					}
					else
					{
						PlayerInstance.SetFaceFeature(16, CHINSIZELOWER);
					}
				}
				else if (DataChanged == ESkinDataChangeType.CHINWIDTH)
				{
					float CHINWIDTH = DataHelper.GetEntityData<float>(PlayerInstance, EDataNames.CC_CHINWIDTH);
					if (PlayerInstance.GetDrawableVariation((int)ECustomClothingComponent.Masks) != 0)
					{
						PlayerInstance.SetFaceFeature(17, 0.0f);
					}
					else
					{
						PlayerInstance.SetFaceFeature(17, CHINWIDTH);
					}
				}
				else if (DataChanged == ESkinDataChangeType.CHINEFFECT)
				{
					float CHINEFFECT = DataHelper.GetEntityData<float>(PlayerInstance, EDataNames.CC_CHINEFFECT);
					if (PlayerInstance.GetDrawableVariation((int)ECustomClothingComponent.Masks) != 0)
					{
						PlayerInstance.SetFaceFeature(18, 0.0f);
					}
					else
					{
						PlayerInstance.SetFaceFeature(18, CHINEFFECT);
					}
				}
				else if (DataChanged == ESkinDataChangeType.NECKWIDTH)
				{
					float NECKWIDTH = DataHelper.GetEntityData<float>(PlayerInstance, EDataNames.CC_NECKWIDTH);
					if (PlayerInstance.GetDrawableVariation((int)ECustomClothingComponent.Masks) != 0)
					{
						PlayerInstance.SetFaceFeature(19, 0.0f);
					}
					else
					{
						PlayerInstance.SetFaceFeature(19, NECKWIDTH);
					}
				}
				else if (DataChanged == ESkinDataChangeType.NECKWIDTHLOWER)
				{
					float NECKWIDTHLOWER = DataHelper.GetEntityData<float>(PlayerInstance, EDataNames.CC_NECKWIDTHLOWER);
					if (PlayerInstance.GetDrawableVariation((int)ECustomClothingComponent.Masks) != 0)
					{
						PlayerInstance.SetFaceFeature(20, 0.0f);
					}
					else
					{
						PlayerInstance.SetFaceFeature(20, NECKWIDTHLOWER);
					}
				}
				else if (DataChanged == ESkinDataChangeType.FACEBLEND)
				{
					int FACEBLEND1MOTHER = DataHelper.GetEntityData<int>(PlayerInstance, EDataNames.CC_FACEBLEND1MOTHER);
					int FACEBLEND1FATHER = DataHelper.GetEntityData<int>(PlayerInstance, EDataNames.CC_FACEBLEND1FATHER);
					float FACEBLENDFATHERPERCENT = DataHelper.GetEntityData<float>(PlayerInstance, EDataNames.CC_FACEBLENDFATHERPERCENT);
					float SKINBLENDFATHERPERCENT = DataHelper.GetEntityData<float>(PlayerInstance, EDataNames.CC_SKINBLENDFATHERPERCENT);
					PlayerInstance.SetHeadBlendData(FACEBLEND1MOTHER, FACEBLEND1FATHER, 0, FACEBLEND1MOTHER, FACEBLEND1FATHER, 0, FACEBLENDFATHERPERCENT, SKINBLENDFATHERPERCENT, 0.0f, true);
				}
				else if (DataChanged == ESkinDataChangeType.BASEHAIR)
				{
					SkinHelpers.ApplyTattoosAndHairTattoosForPlayer(PlayerInstance);
				}
				else if (DataChanged == ESkinDataChangeType.HAIRSTYLE)
				{
					int HAIRSTYLE = DataHelper.GetEntityData<int>(PlayerInstance, EDataNames.CC_HAIRSTYLE);
					PlayerInstance.SetComponentVariation(2, HAIRSTYLE, 0, 0);
				}
				else if (DataChanged == ESkinDataChangeType.HAIRCOLOR)
				{
					int HAIRCOLOR = DataHelper.GetEntityData<int>(PlayerInstance, EDataNames.CC_HAIRCOLOR);
					int HAIRCOLORHIGHLIGHTS = DataHelper.GetEntityData<int>(PlayerInstance, EDataNames.CC_HAIRCOLORHIGHLIGHTS);
					PlayerInstance.SetHairColor(HAIRCOLOR, HAIRCOLORHIGHLIGHTS);
				}
				else if (DataChanged == ESkinDataChangeType.EYECOLOR)
				{
					int EYECOLOR = DataHelper.GetEntityData<int>(PlayerInstance, EDataNames.CC_EYECOLOR);
					PlayerInstance.SetEyeColor(EYECOLOR);
				}
				else if (DataChanged == ESkinDataChangeType.FACIALHAIRSTYLE)
				{
					int FACIALHAIRSTYLE = DataHelper.GetEntityData<int>(PlayerInstance, EDataNames.CC_FACIALHAIRSTYLE);
					float FACIALHAIROPACITY = DataHelper.GetEntityData<float>(PlayerInstance, EDataNames.CC_FACIALHAIROPACITY);
					if (PlayerInstance.GetDrawableVariation((int)ECustomClothingComponent.Masks) != 0)
					{
						PlayerInstance.SetHeadOverlay(1, 0, 0.0f);
					}
					else
					{
						PlayerInstance.SetHeadOverlay(1, FACIALHAIRSTYLE, FACIALHAIROPACITY);
					}
				}
				else if (DataChanged == ESkinDataChangeType.FACIALHAIRCOLOR)
				{
					int FACIALHAIRCOLOR = DataHelper.GetEntityData<int>(PlayerInstance, EDataNames.CC_FACIALHAIRCOLOR);
					int FACIALHAIRCOLORHIGHLIGHT = DataHelper.GetEntityData<int>(PlayerInstance, EDataNames.CC_FACIALHAIRCOLORHIGHLIGHT);
					PlayerInstance.SetHeadOverlayColor(1, 1, FACIALHAIRCOLOR, FACIALHAIRCOLORHIGHLIGHT);
				}
				else if (DataChanged == ESkinDataChangeType.BLEMISHES)
				{
					int BLEMISHES = DataHelper.GetEntityData<int>(PlayerInstance, EDataNames.CC_BLEMISHES);
					float BLEMISHESOPACITY = DataHelper.GetEntityData<float>(PlayerInstance, EDataNames.CC_BLEMISHESOPACITY);
					PlayerInstance.SetHeadOverlay(0, BLEMISHES, BLEMISHESOPACITY);
					// Blemishes color is not a thing in V
				}
				else if (DataChanged == ESkinDataChangeType.EYEBROWS)
				{
					int EYEBROWS = DataHelper.GetEntityData<int>(PlayerInstance, EDataNames.CC_EYEBROWS);
					float EYEBROWSOPACITY = DataHelper.GetEntityData<float>(PlayerInstance, EDataNames.CC_EYEBROWSOPACITY);
					PlayerInstance.SetHeadOverlay(2, EYEBROWS, EYEBROWSOPACITY);
				}
				else if (DataChanged == ESkinDataChangeType.EYEBROWSCOLOR)
				{
					int EYEBROWSCOLOR = DataHelper.GetEntityData<int>(PlayerInstance, EDataNames.CC_EYEBROWSCOLOR);
					int EYEBROWSCOLORHIGHLIGHT = DataHelper.GetEntityData<int>(PlayerInstance, EDataNames.CC_EYEBROWSCOLORHIGHLIGHT);
					PlayerInstance.SetHeadOverlayColor(2, 1, EYEBROWSCOLOR, EYEBROWSCOLORHIGHLIGHT);
				}
				else if (DataChanged == ESkinDataChangeType.MAKEUPCOLOR)
				{
					int MAKEUPCOLOR = DataHelper.GetEntityData<int>(PlayerInstance, EDataNames.CC_MAKEUPCOLOR);
					int MAKEUPCOLORHIGHLIGHT = DataHelper.GetEntityData<int>(PlayerInstance, EDataNames.CC_MAKEUPCOLORHIGHLIGHT);
					PlayerInstance.SetHeadOverlayColor(4, 1, MAKEUPCOLOR, MAKEUPCOLORHIGHLIGHT);
				}
				else if (DataChanged == ESkinDataChangeType.BODYBLEM)
				{
					int BODYBLEMISHES = DataHelper.GetEntityData<int>(PlayerInstance, EDataNames.CC_BODYBLEMISHES);
					float BODYBLEMISHESOPACITY = DataHelper.GetEntityData<float>(PlayerInstance, EDataNames.CC_BODYBLEMISHESOPACITY);
					PlayerInstance.SetHeadOverlay((int)EOverlayTypes.BodyBlemishes, BODYBLEMISHES, BODYBLEMISHESOPACITY);
					PlayerInstance.SetHeadOverlayColor((int)EOverlayTypes.BodyBlemishes, 1, 0, 0);
				}
				else if (DataChanged == ESkinDataChangeType.CHESTHAIR)
				{
					int CHESTHAIR = DataHelper.GetEntityData<int>(PlayerInstance, EDataNames.CC_CHESTHAIR);
					float CHESTHAIROPACITY = DataHelper.GetEntityData<float>(PlayerInstance, EDataNames.CC_CHESTHAIROPACITY);
					PlayerInstance.SetHeadOverlay((int)EOverlayTypes.ChestHair, CHESTHAIR, CHESTHAIROPACITY);
				}
				else if (DataChanged == ESkinDataChangeType.CHESTHAIRCOLOR)
				{
					int CHESTHAIRCOLOR = DataHelper.GetEntityData<int>(PlayerInstance, EDataNames.CC_CHESTHAIRCOLOR);
					int CHESTHAIRHIGHLIGHT = DataHelper.GetEntityData<int>(PlayerInstance, EDataNames.CC_CHESTHAIRHIGHLIGHT);
					PlayerInstance.SetHeadOverlayColor((int)EOverlayTypes.ChestHair, 1, CHESTHAIRCOLOR, CHESTHAIRHIGHLIGHT);
				}
				else if (DataChanged == ESkinDataChangeType.BLUSHCOLOR)
				{
					int BLUSHCOLOR = DataHelper.GetEntityData<int>(PlayerInstance, EDataNames.CC_BLUSHCOLOR);
					int BLUSHCOLORHIGHLIGHT = DataHelper.GetEntityData<int>(PlayerInstance, EDataNames.CC_BLUSHCOLORHIGHLIGHT);
					PlayerInstance.SetHeadOverlayColor(5, 1, BLUSHCOLOR, BLUSHCOLORHIGHLIGHT);
				}
				else if (DataChanged == ESkinDataChangeType.LIPSTICKCOLOR)
				{
					int LIPSTICKCOLOR = DataHelper.GetEntityData<int>(PlayerInstance, EDataNames.CC_LIPSTICKCOLOR);
					int LIPSTICKCOLORHIGHLIGHTS = DataHelper.GetEntityData<int>(PlayerInstance, EDataNames.CC_LIPSTICKCOLORHIGHLIGHTS);
					PlayerInstance.SetHeadOverlayColor(8, 2, LIPSTICKCOLOR, LIPSTICKCOLORHIGHLIGHTS);
				}
				else if (DataChanged == ESkinDataChangeType.TATTOOS)
				{
					SkinHelpers.ApplyTattoosAndHairTattoosForPlayer(PlayerInstance);
				}
			}
			else
			{
				if (DataChanged == ESkinDataChangeType.AGEING)
				{
					PlayerInstance.SetHeadOverlay(3, 0, 0);
				}
				else if (DataChanged == ESkinDataChangeType.MAKEUP)
				{
					PlayerInstance.SetHeadOverlay(4, 0, 0.0f);
				}
				else if (DataChanged == ESkinDataChangeType.BLUSH)
				{
					PlayerInstance.SetHeadOverlay(5, 0, 0.0f);
				}
				else if (DataChanged == ESkinDataChangeType.COMPLEXION)
				{
					PlayerInstance.SetHeadOverlay(6, 0, 0.0f);
				}
				else if (DataChanged == ESkinDataChangeType.SUNDAMAGE)
				{
					PlayerInstance.SetHeadOverlay(7, 0, 0.0f);
				}
				else if (DataChanged == ESkinDataChangeType.LIPSTICK)
				{
					PlayerInstance.SetHeadOverlay(8, 0, 0.0f);
				}
				else if (DataChanged == ESkinDataChangeType.MOLESANDFRECKLES)
				{
					PlayerInstance.SetHeadOverlay(9, 0, 0.0f);
				}
				else if (DataChanged == ESkinDataChangeType.NOSESIZEHORIZONTAL)
				{
					PlayerInstance.SetFaceFeature(0, 0.0f);
				}
				else if (DataChanged == ESkinDataChangeType.NOSESIZEVERTICAL)
				{
					PlayerInstance.SetFaceFeature(1, 0.0f);
				}
				else if (DataChanged == ESkinDataChangeType.NOSESIZEOUTWARDS)
				{
					PlayerInstance.SetFaceFeature(2, 0.0f);
				}
				else if (DataChanged == ESkinDataChangeType.NOSESIZEOUTWARDSUPPER)
				{
					PlayerInstance.SetFaceFeature(3, 0.0f);
				}
				else if (DataChanged == ESkinDataChangeType.NOSESIZEOUTWARDSLOWER)
				{
					PlayerInstance.SetFaceFeature(4, 0.0f);
				}
				else if (DataChanged == ESkinDataChangeType.NOSEANGLE)
				{
					PlayerInstance.SetFaceFeature(5, 0.0f);
				}
				else if (DataChanged == ESkinDataChangeType.EYEBROWHEIGHT)
				{
					PlayerInstance.SetFaceFeature(6, 0.0f);
				}
				else if (DataChanged == ESkinDataChangeType.EYEBROWDEPTH)
				{
					PlayerInstance.SetFaceFeature(7, 0.0f);
				}
				else if (DataChanged == ESkinDataChangeType.CHEEKBONEHEIGHT)
				{
					PlayerInstance.SetFaceFeature(8, 0.0f);
				}
				else if (DataChanged == ESkinDataChangeType.CHEEKWIDTH)
				{
					PlayerInstance.SetFaceFeature(9, 0.0f);
				}
				else if (DataChanged == ESkinDataChangeType.CHEEKWIDTHLOWER)
				{
					PlayerInstance.SetFaceFeature(10, 0.0f);

				}
				else if (DataChanged == ESkinDataChangeType.EYESIZE)
				{
					PlayerInstance.SetFaceFeature(11, 0.0f);
				}
				else if (DataChanged == ESkinDataChangeType.LIPSIZE)
				{
					PlayerInstance.SetFaceFeature(12, 0.0f);
				}
				else if (DataChanged == ESkinDataChangeType.MOUTHSIZE)
				{
					PlayerInstance.SetFaceFeature(13, 0.0f);
				}
				else if (DataChanged == ESkinDataChangeType.MOUTHSIZELOWER)
				{
					PlayerInstance.SetFaceFeature(14, 0.0f);
				}
				else if (DataChanged == ESkinDataChangeType.CHINSIZE)
				{
					PlayerInstance.SetFaceFeature(15, 0.0f);
				}
				else if (DataChanged == ESkinDataChangeType.CHINSIZELOWER)
				{
					PlayerInstance.SetFaceFeature(16, 0.0f);
				}
				else if (DataChanged == ESkinDataChangeType.CHINWIDTH)
				{
					PlayerInstance.SetFaceFeature(17, 0.0f);
				}
				else if (DataChanged == ESkinDataChangeType.CHINEFFECT)
				{
					PlayerInstance.SetFaceFeature(18, 0.0f);
				}
				else if (DataChanged == ESkinDataChangeType.NECKWIDTH)
				{
					PlayerInstance.SetFaceFeature(19, 0.0f);
				}
				else if (DataChanged == ESkinDataChangeType.NECKWIDTHLOWER)
				{
					PlayerInstance.SetFaceFeature(20, 0.0f);
				}
				else if (DataChanged == ESkinDataChangeType.FACEBLEND)
				{
					PlayerInstance.SetHeadBlendData(0, 0, 0, 0, 0, 0, 0, 0, 0.0f, true);
				}
				else if (DataChanged == ESkinDataChangeType.BASEHAIR)
				{
					PlayerInstance.ClearFacialDecorations();
				}
				else if (DataChanged == ESkinDataChangeType.HAIRSTYLE)
				{
					PlayerInstance.SetComponentVariation(2, 0, 0, 0);
				}
				else if (DataChanged == ESkinDataChangeType.HAIRCOLOR)
				{
					PlayerInstance.SetHairColor(0, 0);
				}
				else if (DataChanged == ESkinDataChangeType.EYECOLOR)
				{
					PlayerInstance.SetEyeColor(0);
				}
				else if (DataChanged == ESkinDataChangeType.FACIALHAIRSTYLE)
				{
					PlayerInstance.SetHeadOverlay(1, 0, 0.0f);
				}
				else if (DataChanged == ESkinDataChangeType.FACIALHAIRCOLOR)
				{
					PlayerInstance.SetHeadOverlayColor(1, 1, 0, 0);
				}
				else if (DataChanged == ESkinDataChangeType.BLEMISHES)
				{
					PlayerInstance.SetHeadOverlay(0, 0, 0.0f);
				}
				else if (DataChanged == ESkinDataChangeType.EYEBROWS)
				{
					PlayerInstance.SetHeadOverlay(2, 0, 0.0f);
				}
				else if (DataChanged == ESkinDataChangeType.EYEBROWSCOLOR)
				{
					PlayerInstance.SetHeadOverlayColor(2, 1, 0, 0);
				}
				else if (DataChanged == ESkinDataChangeType.MAKEUPCOLOR)
				{
					PlayerInstance.SetHeadOverlayColor(4, 1, 0, 0);
				}
				else if (DataChanged == ESkinDataChangeType.BODYBLEM)
				{
					PlayerInstance.SetHeadOverlay((int)EOverlayTypes.BodyBlemishes, 0, 0.0f);
					PlayerInstance.SetHeadOverlayColor((int)EOverlayTypes.BodyBlemishes, 1, 0, 0);
				}
				else if (DataChanged == ESkinDataChangeType.CHESTHAIR)
				{
					PlayerInstance.SetHeadOverlay((int)EOverlayTypes.ChestHair, 0, 0.0f);
				}
				else if (DataChanged == ESkinDataChangeType.CHESTHAIRCOLOR)
				{
					PlayerInstance.SetHeadOverlayColor((int)EOverlayTypes.ChestHair, 1, 0, 0);
				}
				else if (DataChanged == ESkinDataChangeType.BLUSHCOLOR)
				{
					PlayerInstance.SetHeadOverlayColor(5, 1, 0, 0);
				}
				else if (DataChanged == ESkinDataChangeType.LIPSTICKCOLOR)
				{
					PlayerInstance.SetHeadOverlayColor(8, 2, 0, 0);
				}
				else if (DataChanged == ESkinDataChangeType.TATTOOS)
				{
					PlayerInstance.ClearFacialDecorations();
				}
			}
		}

		public ESkinDataChangeType DataChanged { get; private set; }
	}

	private class ClothingTask : MultiFrameTask
	{
		public ClothingTask(RAGE.Elements.Player a_PlayerInstance, int a_Component, int a_Drawable, int a_Texture) : base(a_PlayerInstance, EMultiFrameTaskType.Clothing)
		{
			Component = a_Component;
			Drawable = a_Drawable;
			Texture = a_Texture;
		}

		public override void Run()
		{
			if (PlayerInstance != null)
			{
				PlayerInstance.SetComponentVariation(Component, Drawable, Texture, 0);
			}
		}

		public int Component { get; private set; }
		public int Drawable { get; private set; }
		public int Texture { get; private set; }
	}

	private class PropTask : MultiFrameTask
	{
		public PropTask(RAGE.Elements.Player a_PlayerInstance, int a_Prop, int a_Drawable, int a_Texture) : base(a_PlayerInstance, EMultiFrameTaskType.Prop)
		{
			Prop = a_Prop;
			Drawable = a_Drawable;
			Texture = a_Texture;
		}

		public override void Run()
		{
			if (Drawable <= 0)
			{
				PlayerInstance.ClearProp(Prop);
			}
			else
			{
				PlayerInstance.SetPropIndex(Prop, Drawable, Texture, true);
			}
		}

		public int Prop { get; private set; }
		public int Drawable { get; private set; }
		public int Texture { get; private set; }
	}

	static CustomSkinDataHandler()
	{

	}

	private static bool m_bDebugSpamEnabled = false;
	private static Dictionary<int, double> m_dictPerFrameTimings_SkinData = new Dictionary<int, double>();
	private static Dictionary<int, double> m_dictPerFrameTimings_Accessories = new Dictionary<int, double>();
	private static Dictionary<int, double> m_dictPerFrameTimings_ClothingData = new Dictionary<int, double>();
	private static Dictionary<int, int> m_dictPerFrameCounts_SkinData = new Dictionary<int, int>();
	private static Dictionary<int, int> m_dictPerFrameCounts_Accessories = new Dictionary<int, int>();
	private static Dictionary<int, int> m_dictPerFrameCounts_ClothingData = new Dictionary<int, int>();
	private static void OnRender()
	{
		if (m_bDebugSpamEnabled)
		{
			int frameID = RAGE.Game.Misc.GetFrameCount();
			int prevFrameID = frameID - 1;

			// dump last frame if exists
			bool bHasLastFrameData = m_dictPerFrameTimings_SkinData.ContainsKey(prevFrameID);
			if (bHasLastFrameData)
			{
				if (m_dictPerFrameCounts_SkinData[prevFrameID] > 0 || m_dictPerFrameCounts_Accessories[prevFrameID] > 0 || m_dictPerFrameCounts_ClothingData[prevFrameID] > 0)
				{
					ChatHelper.DebugMessage("LAST FRAME({6}): {0}ms SkinData ({1}) - {2}ms Accessories ({3}) - {4}ms ClothingData ({5}) TOTAL: {7}", m_dictPerFrameTimings_SkinData[prevFrameID], m_dictPerFrameCounts_SkinData[prevFrameID], m_dictPerFrameTimings_Accessories[prevFrameID],
					 m_dictPerFrameCounts_Accessories[prevFrameID], m_dictPerFrameTimings_ClothingData[prevFrameID], m_dictPerFrameCounts_ClothingData[prevFrameID], prevFrameID,
					 m_dictPerFrameTimings_SkinData[prevFrameID] + m_dictPerFrameTimings_Accessories[prevFrameID] + m_dictPerFrameTimings_ClothingData[prevFrameID]);
				}
			}

			m_dictPerFrameTimings_SkinData[frameID] = 0.0;
			m_dictPerFrameTimings_Accessories[frameID] = 0.0;
			m_dictPerFrameTimings_ClothingData[frameID] = 0.0;
			m_dictPerFrameCounts_SkinData[frameID] = 0;
			m_dictPerFrameCounts_Accessories[frameID] = 0;
			m_dictPerFrameCounts_ClothingData[frameID] = 0;
		}
	}

	public static void Init()
	{
		// EVENTS
		RageEvents.RAGE_OnEntityStreamIn += OnEntityStreamIn;
		NetworkEvents.ApplyCustomSkinData += ApplyAllCustomSkinData;

		RageEvents.RAGE_OnRender += OnRender;

		NetworkEvents.ToggleDebugSpam += () =>
		{
			m_bDebugSpamEnabled = !m_bDebugSpamEnabled;
			NotificationManager.ShowNotification("Debug", Helpers.FormatString("Debug: {0}", m_bDebugSpamEnabled ? "Enabled" : "Disabled"), ENotificationIcon.InfoSign);
		};

		/*
		RageEvents.RAGE_OnTick_OncePerSecond += () =>
		{
			RAGE.Elements.Player player = RAGE.Elements.Player.LocalPlayer;

			DateTime dtStart = DateTime.Now;
			ApplyCustomSkinData(player);
			double msSkinData = (DateTime.Now - dtStart).TotalMilliseconds;

			dtStart = DateTime.Now;
			ApplyCustomAccessories(player);
			double msAccessories = (DateTime.Now - dtStart).TotalMilliseconds;

			dtStart = DateTime.Now;
			ApplyClothingData(player);
			double msClothes = (DateTime.Now - dtStart).TotalMilliseconds;

			ChatHelper.DebugMessage("SKINS: {0} and {1} and {2} (TOTAL: {3})", msSkinData, msAccessories, msClothes, msSkinData + msAccessories + msClothes);
		};
		*/

		RageEvents.AddDataHandler(EDataNames.CLOTHING, OnClothingChanged);

		// TIMERS
		//ClientTimerPool.CreateTimer(UpdateAccessoriesTimerCallback, 1000);

		// DATA HANDLERS
		// TODO_CSHARP: Extend event manager to take care of this, so we don't have to use the rage one? maybe...
		RageEvents.AddDataHandler(EDataNames.PROP_MODELS, OnAccessoriesChanged);
		RageEvents.AddDataHandler(EDataNames.PROP_TEXTS, OnAccessoriesChanged); // TODO_LAUNCH: Do we need this? Presumably the first changes also.

		RageEvents.AddDataHandler(EDataNames.CC_TATTOOS, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_TATTOOS); });
		RageEvents.AddDataHandler(EDataNames.CC_AGEING, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_AGEING); });
		RageEvents.AddDataHandler(EDataNames.CC_AGEINGOPACITY, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_AGEINGOPACITY); });
		RageEvents.AddDataHandler(EDataNames.CC_MAKEUP, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_MAKEUP); });
		RageEvents.AddDataHandler(EDataNames.CC_MAKEUPOPACITY, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_MAKEUPOPACITY); });
		RageEvents.AddDataHandler(EDataNames.CC_BLUSH, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_BLUSH); });
		RageEvents.AddDataHandler(EDataNames.CC_BLUSHOPACITY, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_BLUSHOPACITY); });
		RageEvents.AddDataHandler(EDataNames.CC_BLUSHCOLOR, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_BLUSHCOLOR); });
		RageEvents.AddDataHandler(EDataNames.CC_BLUSHCOLORHIGHLIGHT, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_BLUSHCOLORHIGHLIGHT); });
		RageEvents.AddDataHandler(EDataNames.CC_COMPLEXION, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_COMPLEXION); });
		RageEvents.AddDataHandler(EDataNames.CC_COMPLEXIONOPACITY, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_COMPLEXIONOPACITY); });
		RageEvents.AddDataHandler(EDataNames.CC_SUNDAMAGE, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_SUNDAMAGE); });
		RageEvents.AddDataHandler(EDataNames.CC_SUNDAMAGEOPACITY, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_SUNDAMAGEOPACITY); });
		RageEvents.AddDataHandler(EDataNames.CC_LIPSTICK, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_LIPSTICK); });
		RageEvents.AddDataHandler(EDataNames.CC_LIPSTICKOPACITY, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_LIPSTICKOPACITY); });
		RageEvents.AddDataHandler(EDataNames.CC_LIPSTICKCOLOR, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_LIPSTICKCOLOR); });
		RageEvents.AddDataHandler(EDataNames.CC_LIPSTICKCOLORHIGHLIGHTS, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_LIPSTICKCOLORHIGHLIGHTS); });
		RageEvents.AddDataHandler(EDataNames.CC_MOLESANDFRECKLES, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_MOLESANDFRECKLES); });
		RageEvents.AddDataHandler(EDataNames.CC_MOLESANDFRECKLESOPACITY, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_MOLESANDFRECKLESOPACITY); });
		RageEvents.AddDataHandler(EDataNames.CC_NOSESIZEHORIZONTAL, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_NOSESIZEHORIZONTAL); });
		RageEvents.AddDataHandler(EDataNames.CC_NOSESIZEVERTICAL, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_NOSESIZEVERTICAL); });
		RageEvents.AddDataHandler(EDataNames.CC_NOSESIZEOUTWARDS, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_NOSESIZEOUTWARDS); });
		RageEvents.AddDataHandler(EDataNames.CC_NOSESIZEOUTWARDSUPPER, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_NOSESIZEOUTWARDSUPPER); });
		RageEvents.AddDataHandler(EDataNames.CC_NOSESIZEOUTWARDSLOWER, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_NOSESIZEOUTWARDSLOWER); });
		RageEvents.AddDataHandler(EDataNames.CC_NOSEANGLE, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_NOSEANGLE); });
		RageEvents.AddDataHandler(EDataNames.CC_EYEBROWHEIGHT, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_EYEBROWHEIGHT); });
		RageEvents.AddDataHandler(EDataNames.CC_EYEBROWDEPTH, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_EYEBROWDEPTH); });
		RageEvents.AddDataHandler(EDataNames.CC_CHEEKBONEHEIGHT, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_CHEEKBONEHEIGHT); });
		RageEvents.AddDataHandler(EDataNames.CC_CHEEKWIDTH, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_CHEEKWIDTH); });
		RageEvents.AddDataHandler(EDataNames.CC_CHEEKWIDTHLOWER, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_CHEEKWIDTHLOWER); });
		RageEvents.AddDataHandler(EDataNames.CC_EYESIZE, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_EYESIZE); });
		RageEvents.AddDataHandler(EDataNames.CC_LIPSIZE, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_LIPSIZE); });
		RageEvents.AddDataHandler(EDataNames.CC_MOUTHSIZE, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_MOUTHSIZE); });
		RageEvents.AddDataHandler(EDataNames.CC_MOUTHSIZELOWER, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_MOUTHSIZELOWER); });
		RageEvents.AddDataHandler(EDataNames.CC_CHINSIZE, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_CHINSIZE); });
		RageEvents.AddDataHandler(EDataNames.CC_CHINSIZELOWER, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_CHINSIZELOWER); });
		RageEvents.AddDataHandler(EDataNames.CC_CHINWIDTH, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_CHINWIDTH); });
		RageEvents.AddDataHandler(EDataNames.CC_CHINEFFECT, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_CHINEFFECT); });
		RageEvents.AddDataHandler(EDataNames.CC_NECKWIDTH, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_NECKWIDTH); });
		RageEvents.AddDataHandler(EDataNames.CC_NECKWIDTHLOWER, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_NECKWIDTHLOWER); });
		RageEvents.AddDataHandler(EDataNames.CC_FACEBLEND1MOTHER, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_FACEBLEND1MOTHER); });
		RageEvents.AddDataHandler(EDataNames.CC_FACEBLEND1FATHER, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_FACEBLEND1FATHER); });
		RageEvents.AddDataHandler(EDataNames.CC_FACEBLENDFATHERPERCENT, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_FACEBLENDFATHERPERCENT); });
		RageEvents.AddDataHandler(EDataNames.CC_SKINBLENDFATHERPERCENT, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_SKINBLENDFATHERPERCENT); });
		RageEvents.AddDataHandler(EDataNames.CC_BASEHAIR, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_BASEHAIR); });
		RageEvents.AddDataHandler(EDataNames.CC_HAIRSTYLE, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_HAIRSTYLE); });
		RageEvents.AddDataHandler(EDataNames.CC_HAIRCOLOR, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_HAIRCOLOR); });
		RageEvents.AddDataHandler(EDataNames.CC_HAIRCOLORHIGHLIGHTS, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_HAIRCOLORHIGHLIGHTS); });
		RageEvents.AddDataHandler(EDataNames.CC_EYECOLOR, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_EYECOLOR); });
		RageEvents.AddDataHandler(EDataNames.CC_FACIALHAIRSTYLE, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_FACIALHAIRSTYLE); });
		RageEvents.AddDataHandler(EDataNames.CC_FACIALHAIRCOLOR, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_FACIALHAIRCOLOR); });
		RageEvents.AddDataHandler(EDataNames.CC_FACIALHAIRCOLORHIGHLIGHT, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_FACIALHAIRCOLORHIGHLIGHT); });
		RageEvents.AddDataHandler(EDataNames.CC_FACIALHAIROPACITY, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_FACIALHAIROPACITY); });
		RageEvents.AddDataHandler(EDataNames.CC_BLEMISHES, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_BLEMISHES); });
		RageEvents.AddDataHandler(EDataNames.CC_BLEMISHESOPACITY, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_BLEMISHESOPACITY); });
		RageEvents.AddDataHandler(EDataNames.CC_EYEBROWS, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_EYEBROWS); });
		RageEvents.AddDataHandler(EDataNames.CC_EYEBROWSOPACITY, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_EYEBROWSOPACITY); });
		RageEvents.AddDataHandler(EDataNames.CC_EYEBROWSCOLOR, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_EYEBROWSCOLOR); });
		RageEvents.AddDataHandler(EDataNames.CC_EYEBROWSCOLORHIGHLIGHT, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_EYEBROWSCOLORHIGHLIGHT); });
		RageEvents.AddDataHandler(EDataNames.CC_BODYBLEMISHES, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_BODYBLEMISHES); });
		RageEvents.AddDataHandler(EDataNames.CC_BODYBLEMISHESOPACITY, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_BODYBLEMISHESOPACITY); });
		RageEvents.AddDataHandler(EDataNames.CC_CHESTHAIR, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_CHESTHAIR); });
		RageEvents.AddDataHandler(EDataNames.CC_CHESTHAIROPACITY, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_CHESTHAIROPACITY); });
		RageEvents.AddDataHandler(EDataNames.CC_CHESTHAIRCOLOR, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_CHESTHAIRCOLOR); });
		RageEvents.AddDataHandler(EDataNames.CC_CHESTHAIRHIGHLIGHT, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyCustomSkinData((RAGE.Elements.Player)entity, EDataNames.CC_CHESTHAIRHIGHLIGHT); });

		InitMultiFrameWorker();
	}

	private static void InitMultiFrameWorker()
	{
		MultiFrameWorkLoad workLoad = new MultiFrameWorkLoad(EWorkLoadProcessingType.FrameMillisecondsBudget, 0.7,
		(Queue<object> workQueue) => // init - this function is recalled when this is a looped multiframe queue, so you'll want to clear out any temp vars
		{
			// add everything pending
			workQueue.AddRange(g_lstPendingTasks);
			g_lstPendingTasks.Clear();
		}, (object objectToProcess) => // tick
		{
			// process object
			MultiFrameTask task = (MultiFrameTask)objectToProcess;
			task.Run();
		}, () => // completion
		{
			return true; // re queue, we want to keep doing this
		});

		MultiFrameWorkScheduler.QueueWork(workLoad);
	}

	private static void ApplyAllCustomSkinData(RAGE.Elements.Player player)
	{
		for (EDataNames dataName = EDataNames.CC_AGEING; dataName <= EDataNames.CC_TATTOOS; ++dataName)
		{
			ApplyCustomSkinData(player, dataName);
		}
	}

	/*
	private static void OnSkinDataChanged(RAGE.Elements.Entity entity, object newValue, object oldValue)
	{
		if (entity != null && entity.Type == RAGE.Elements.Type.Player)
		{
			RAGE.Elements.Player player = (RAGE.Elements.Player)entity;
			ApplyCustomSkinData(player);
		}
	}
	*/

	private static void OnAccessoriesChanged(RAGE.Elements.Entity entity, object newValue, object oldValue)
	{
		if (entity != null && entity.Type == RAGE.Elements.Type.Player)
		{
			RAGE.Elements.Player player = (RAGE.Elements.Player)entity;
			ApplyCustomAccessories(player);
		}
	}

	private static void OnClothingChanged(RAGE.Elements.Entity entity, object newValue, object oldValue)
	{
		if (entity != null && entity.Type == RAGE.Elements.Type.Player)
		{
			RAGE.Elements.Player player = (RAGE.Elements.Player)entity;
			ApplyClothingData(player);
		}
	}

	private static List<MultiFrameTask> g_lstPendingTasks = new List<MultiFrameTask>();
	private static void ApplyClothingData(RAGE.Elements.Player player)
	{
		DateTime dtStart = DateTime.Now;

		// dont apply if remote player is not logged in, OR it is local player and we are in duty, char create, or also not logged in...
		bool isLocalPlayerLoggedIn = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.IS_LOGGED_IN);
		bool isLoggedIn = DataHelper.GetEntityData<bool>(player, EDataNames.IS_LOGGED_IN);

		// we never allow accesories in duty, only hair/face etc if custom. Accessories are controlled by duty
		bool bShouldApply = true;
		if (player == RAGE.Elements.Player.LocalPlayer && (FactionSystem.GetDutySystem().IsInDutyUI() || CharacterCreation.IsInCharacterCreation() || !isLocalPlayerLoggedIn))
		{
			bShouldApply = false;
		}

		if (!isLoggedIn)
		{
			bShouldApply = false;
		}

		if (!bShouldApply)
		{
			return;
		}

		if (DataHelper.HasEntityData(player, EDataNames.CLOTHING))
		{
			string strData = DataHelper.GetEntityData<string>(player, EDataNames.CLOTHING);
			BulkClothing clothing = new BulkClothing(strData);

			// NOTE: No need to dump existing tasks because they are ordered anyway
			bool bIsCustom = DataHelper.GetEntityData<bool>(player, EDataNames.IS_CUSTOM);

			for (int i = 0; i < BulkClothing.numComponents; ++i)
			{
				if (i != 2 || !bIsCustom) // ignore hair if custom
				{
					g_lstPendingTasks.Add(new ClothingTask(player, i, clothing.GetComponent(i), clothing.GetTexture(i)));
				}
			}
		}

		if (m_bDebugSpamEnabled)
		{
			double ms = (DateTime.Now - dtStart).TotalMilliseconds;
			m_dictPerFrameTimings_ClothingData[RAGE.Game.Misc.GetFrameCount()] += ms;
			m_dictPerFrameCounts_ClothingData[RAGE.Game.Misc.GetFrameCount()]++;
		}
	}

	private static void OnEntityStreamIn(RAGE.Elements.Entity entity)
	{
		if (entity != null && entity.Type == RAGE.Elements.Type.Player)
		{
			RAGE.Elements.Player player = (RAGE.Elements.Player)entity;
			ApplyAllCustomSkinData(player);
			ApplyCustomAccessories(player);
			ApplyClothingData(player);
		}
	}

	// TODO_RAGE: Timer is a fix for a rage bug where it seems to overwrite all the accesory data with zeros...
	private static void UpdateAccessoriesTimerCallback(object[] a_Parameters)
	{
		foreach (var player in RAGE.Elements.Entities.Players.Streamed)
		{
			ApplyCustomAccessories(player);
		}
	}

	private static void ApplyCustomSkinData(RAGE.Elements.Player player, EDataNames dataNameChanged)
	{
		DateTime dtStart = DateTime.Now;

		// dont apply if remote player is not logged in, OR it is local player and we are in duty, char create, or also not logged in...
		bool isLocalPlayerLoggedIn = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.IS_LOGGED_IN);
		bool isLoggedIn = DataHelper.GetEntityData<bool>(player, EDataNames.IS_LOGGED_IN);

		// we never allow accessories in duty, only hair/face etc if custom. Accessories are controlled by duty
		bool bShouldApply = true;
		if (player == RAGE.Elements.Player.LocalPlayer && ((FactionSystem.GetDutySystem().IsInDutyUI() && player.Model != CharacterConstants.CustomMaleSkin && player.Model != CharacterConstants.CustomFemaleSkin) || !isLocalPlayerLoggedIn))
		{
			bShouldApply = false;
		}

		if (!isLoggedIn)
		{
			bShouldApply = false;
		}

		if (!bShouldApply)
		{
			return;
		}

		ESkinDataChangeType SkinDataChangeType = ESkinDataChangeType.NONE;
		if (dataNameChanged == EDataNames.CC_AGEING)
		{
			SkinDataChangeType = ESkinDataChangeType.AGEING;
		}
		else if (dataNameChanged == EDataNames.CC_AGEINGOPACITY)
		{
			SkinDataChangeType = ESkinDataChangeType.AGEING;
		}
		else if (dataNameChanged == EDataNames.CC_MAKEUP)
		{
			SkinDataChangeType = ESkinDataChangeType.MAKEUP;
		}
		else if (dataNameChanged == EDataNames.CC_MAKEUPOPACITY)
		{
			SkinDataChangeType = ESkinDataChangeType.MAKEUP;
		}
		else if (dataNameChanged == EDataNames.CC_BLUSH)
		{
			SkinDataChangeType = ESkinDataChangeType.BLUSH;
		}
		else if (dataNameChanged == EDataNames.CC_BLUSHOPACITY)
		{
			SkinDataChangeType = ESkinDataChangeType.BLUSH;
		}
		else if (dataNameChanged == EDataNames.CC_BLUSHCOLOR)
		{
			SkinDataChangeType = ESkinDataChangeType.BLUSHCOLOR;
		}
		else if (dataNameChanged == EDataNames.CC_BLUSHCOLORHIGHLIGHT)
		{
			SkinDataChangeType = ESkinDataChangeType.BLUSHCOLOR;
		}
		else if (dataNameChanged == EDataNames.CC_COMPLEXION)
		{
			SkinDataChangeType = ESkinDataChangeType.COMPLEXION;
		}
		else if (dataNameChanged == EDataNames.CC_COMPLEXIONOPACITY)
		{
			SkinDataChangeType = ESkinDataChangeType.COMPLEXION;
		}
		else if (dataNameChanged == EDataNames.CC_SUNDAMAGE)
		{
			SkinDataChangeType = ESkinDataChangeType.SUNDAMAGE;
		}
		else if (dataNameChanged == EDataNames.CC_SUNDAMAGEOPACITY)
		{
			SkinDataChangeType = ESkinDataChangeType.SUNDAMAGE;
		}
		else if (dataNameChanged == EDataNames.CC_LIPSTICK)
		{
			SkinDataChangeType = ESkinDataChangeType.LIPSTICK;
		}
		else if (dataNameChanged == EDataNames.CC_LIPSTICKOPACITY)
		{
			SkinDataChangeType = ESkinDataChangeType.LIPSTICK;
		}
		else if (dataNameChanged == EDataNames.CC_LIPSTICKCOLOR)
		{
			SkinDataChangeType = ESkinDataChangeType.LIPSTICKCOLOR;
		}
		else if (dataNameChanged == EDataNames.CC_LIPSTICKCOLORHIGHLIGHTS)
		{
			SkinDataChangeType = ESkinDataChangeType.LIPSTICKCOLOR;
		}
		else if (dataNameChanged == EDataNames.CC_MOLESANDFRECKLES)
		{
			SkinDataChangeType = ESkinDataChangeType.MOLESANDFRECKLES;
		}
		else if (dataNameChanged == EDataNames.CC_MOLESANDFRECKLESOPACITY)
		{
			SkinDataChangeType = ESkinDataChangeType.MOLESANDFRECKLES;
		}
		else if (dataNameChanged == EDataNames.CC_NOSESIZEHORIZONTAL)
		{
			SkinDataChangeType = ESkinDataChangeType.NOSESIZEHORIZONTAL;
		}
		else if (dataNameChanged == EDataNames.CC_NOSESIZEVERTICAL)
		{
			SkinDataChangeType = ESkinDataChangeType.NOSESIZEVERTICAL;
		}
		else if (dataNameChanged == EDataNames.CC_NOSESIZEOUTWARDS)
		{
			SkinDataChangeType = ESkinDataChangeType.NOSESIZEOUTWARDS;
		}
		else if (dataNameChanged == EDataNames.CC_NOSESIZEOUTWARDSUPPER)
		{
			SkinDataChangeType = ESkinDataChangeType.NOSESIZEOUTWARDSUPPER;
		}
		else if (dataNameChanged == EDataNames.CC_NOSESIZEOUTWARDSLOWER)
		{
			SkinDataChangeType = ESkinDataChangeType.NOSESIZEOUTWARDSLOWER;
		}
		else if (dataNameChanged == EDataNames.CC_NOSEANGLE)
		{
			SkinDataChangeType = ESkinDataChangeType.NOSEANGLE;
		}
		else if (dataNameChanged == EDataNames.CC_EYEBROWHEIGHT)
		{
			SkinDataChangeType = ESkinDataChangeType.EYEBROWHEIGHT;
		}
		else if (dataNameChanged == EDataNames.CC_EYEBROWDEPTH)
		{
			SkinDataChangeType = ESkinDataChangeType.EYEBROWDEPTH;
		}
		else if (dataNameChanged == EDataNames.CC_CHEEKBONEHEIGHT)
		{
			SkinDataChangeType = ESkinDataChangeType.CHEEKBONEHEIGHT;
		}
		else if (dataNameChanged == EDataNames.CC_CHEEKWIDTH)
		{
			SkinDataChangeType = ESkinDataChangeType.CHEEKWIDTH;
		}
		else if (dataNameChanged == EDataNames.CC_CHEEKWIDTHLOWER)
		{
			SkinDataChangeType = ESkinDataChangeType.CHEEKWIDTHLOWER;
		}
		else if (dataNameChanged == EDataNames.CC_EYESIZE)
		{
			SkinDataChangeType = ESkinDataChangeType.EYESIZE;
		}
		else if (dataNameChanged == EDataNames.CC_LIPSIZE)
		{
			SkinDataChangeType = ESkinDataChangeType.LIPSIZE;
		}
		else if (dataNameChanged == EDataNames.CC_MOUTHSIZE)
		{
			SkinDataChangeType = ESkinDataChangeType.MOUTHSIZE;
		}
		else if (dataNameChanged == EDataNames.CC_MOUTHSIZELOWER)
		{
			SkinDataChangeType = ESkinDataChangeType.MOUTHSIZELOWER;
		}
		else if (dataNameChanged == EDataNames.CC_CHINSIZE)
		{
			SkinDataChangeType = ESkinDataChangeType.CHINSIZE;
		}
		else if (dataNameChanged == EDataNames.CC_CHINSIZELOWER)
		{
			SkinDataChangeType = ESkinDataChangeType.CHINSIZELOWER;
		}
		else if (dataNameChanged == EDataNames.CC_CHINWIDTH)
		{
			SkinDataChangeType = ESkinDataChangeType.CHINWIDTH;
		}
		else if (dataNameChanged == EDataNames.CC_CHINEFFECT)
		{
			SkinDataChangeType = ESkinDataChangeType.CHINEFFECT;
		}
		else if (dataNameChanged == EDataNames.CC_NECKWIDTH)
		{
			SkinDataChangeType = ESkinDataChangeType.NECKWIDTH;
		}
		else if (dataNameChanged == EDataNames.CC_NECKWIDTHLOWER)
		{
			SkinDataChangeType = ESkinDataChangeType.NECKWIDTHLOWER;
		}
		else if (dataNameChanged == EDataNames.CC_FACEBLEND1MOTHER)
		{
			SkinDataChangeType = ESkinDataChangeType.FACEBLEND;
		}
		else if (dataNameChanged == EDataNames.CC_FACEBLEND1FATHER)
		{
			SkinDataChangeType = ESkinDataChangeType.FACEBLEND;
		}
		else if (dataNameChanged == EDataNames.CC_FACEBLENDFATHERPERCENT)
		{
			SkinDataChangeType = ESkinDataChangeType.FACEBLEND;
		}
		else if (dataNameChanged == EDataNames.CC_SKINBLENDFATHERPERCENT)
		{
			SkinDataChangeType = ESkinDataChangeType.FACEBLEND;
		}
		else if (dataNameChanged == EDataNames.CC_BASEHAIR)
		{
			SkinDataChangeType = ESkinDataChangeType.BASEHAIR;
		}
		else if (dataNameChanged == EDataNames.CC_HAIRSTYLE)
		{
			SkinDataChangeType = ESkinDataChangeType.HAIRSTYLE;
		}
		else if (dataNameChanged == EDataNames.CC_HAIRCOLOR)
		{
			SkinDataChangeType = ESkinDataChangeType.HAIRCOLOR;
		}
		else if (dataNameChanged == EDataNames.CC_HAIRCOLORHIGHLIGHTS)
		{
			SkinDataChangeType = ESkinDataChangeType.HAIRCOLOR;
		}
		else if (dataNameChanged == EDataNames.CC_EYECOLOR)
		{
			SkinDataChangeType = ESkinDataChangeType.EYECOLOR;
		}
		else if (dataNameChanged == EDataNames.CC_FACIALHAIRSTYLE)
		{
			SkinDataChangeType = ESkinDataChangeType.FACIALHAIRSTYLE;
		}
		else if (dataNameChanged == EDataNames.CC_FACIALHAIRCOLOR)
		{
			SkinDataChangeType = ESkinDataChangeType.FACIALHAIRCOLOR;
		}
		else if (dataNameChanged == EDataNames.CC_FACIALHAIRCOLORHIGHLIGHT)
		{
			SkinDataChangeType = ESkinDataChangeType.FACIALHAIRCOLOR;
		}
		else if (dataNameChanged == EDataNames.CC_FACIALHAIROPACITY)
		{
			SkinDataChangeType = ESkinDataChangeType.FACIALHAIRSTYLE;
		}
		else if (dataNameChanged == EDataNames.CC_BLEMISHES)
		{
			SkinDataChangeType = ESkinDataChangeType.BLEMISHES;
		}
		else if (dataNameChanged == EDataNames.CC_BLEMISHESOPACITY)
		{
			SkinDataChangeType = ESkinDataChangeType.BLEMISHES;
		}
		else if (dataNameChanged == EDataNames.CC_EYEBROWS)
		{
			SkinDataChangeType = ESkinDataChangeType.EYEBROWS;
		}
		else if (dataNameChanged == EDataNames.CC_EYEBROWSOPACITY)
		{
			SkinDataChangeType = ESkinDataChangeType.EYEBROWS;
		}
		else if (dataNameChanged == EDataNames.CC_EYEBROWSCOLOR)
		{
			SkinDataChangeType = ESkinDataChangeType.EYEBROWSCOLOR;
		}
		else if (dataNameChanged == EDataNames.CC_EYEBROWSCOLORHIGHLIGHT)
		{
			SkinDataChangeType = ESkinDataChangeType.EYEBROWSCOLOR;
		}
		else if (dataNameChanged == EDataNames.CC_MAKEUPCOLOR)
		{
			SkinDataChangeType = ESkinDataChangeType.MAKEUPCOLOR;
		}
		else if (dataNameChanged == EDataNames.CC_MAKEUPCOLORHIGHLIGHT)
		{
			SkinDataChangeType = ESkinDataChangeType.MAKEUPCOLOR;
		}
		else if (dataNameChanged == EDataNames.CC_BODYBLEMISHES)
		{
			SkinDataChangeType = ESkinDataChangeType.BODYBLEM;
		}
		else if (dataNameChanged == EDataNames.CC_BODYBLEMISHESOPACITY)
		{
			SkinDataChangeType = ESkinDataChangeType.BODYBLEM;
		}
		else if (dataNameChanged == EDataNames.CC_CHESTHAIR)
		{
			SkinDataChangeType = ESkinDataChangeType.CHESTHAIR;
		}
		else if (dataNameChanged == EDataNames.CC_CHESTHAIROPACITY)
		{
			SkinDataChangeType = ESkinDataChangeType.CHESTHAIR;
		}
		else if (dataNameChanged == EDataNames.CC_CHESTHAIRCOLOR)
		{
			SkinDataChangeType = ESkinDataChangeType.CHESTHAIRCOLOR;
		}
		else if (dataNameChanged == EDataNames.CC_CHESTHAIRHIGHLIGHT)
		{
			SkinDataChangeType = ESkinDataChangeType.CHESTHAIRCOLOR;
		}
		else if (dataNameChanged == EDataNames.CC_TATTOOS)
		{
			SkinDataChangeType = ESkinDataChangeType.TATTOOS;
		}

		// TODO remove dupes
		if (SkinDataChangeType != ESkinDataChangeType.NONE)
		{
			g_lstPendingTasks.Add(new SkinDataTask(player, SkinDataChangeType));
		}

		if (m_bDebugSpamEnabled)
		{
			double ms = (DateTime.Now - dtStart).TotalMilliseconds;
			m_dictPerFrameTimings_SkinData[RAGE.Game.Misc.GetFrameCount()] += ms;
			m_dictPerFrameCounts_SkinData[RAGE.Game.Misc.GetFrameCount()]++;
		}
	}

	// TODO_CSHARP: Face stuff is weird, hat doesnt show on cop etc. Dont think it applies at all. Check everything
	private static void ApplyCustomAccessories(RAGE.Elements.Player playerEntity)
	{
		DateTime dtStart = DateTime.Now;
		RAGE.Elements.Player player = (RAGE.Elements.Player)playerEntity;

		// dont apply if remote player is not logged in, OR it is local player and we are in duty, char create, or also not logged in...
		bool isLocalPlayerLoggedIn = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.IS_LOGGED_IN);
		bool isLoggedIn = DataHelper.GetEntityData<bool>(playerEntity, EDataNames.IS_LOGGED_IN);

		// we never allow accessories in duty, only hair/face etc if custom. Accessories are controlled by duty
		bool bShouldApply = true;
		if (player == RAGE.Elements.Player.LocalPlayer && (FactionSystem.GetDutySystem().IsInDutyUI() || CharacterCreation.IsInCharacterCreation() || !isLocalPlayerLoggedIn))
		{
			bShouldApply = false;
		}

		if (!isLoggedIn)
		{
			bShouldApply = false;
		}

		if (!bShouldApply)
		{
			return;
		}

		string strPropsJSON = DataHelper.GetEntityData<string>(playerEntity, EDataNames.PROP_MODELS);
		string strPropsTexturesJSON = DataHelper.GetEntityData<string>(playerEntity, EDataNames.PROP_TEXTS);

		if (strPropsJSON != null && strPropsTexturesJSON != null)
		{
			Dictionary<int, int> propModels = OwlJSON.DeserializeObject<Dictionary<int, int>>(strPropsJSON, EJsonTrackableIdentifier.CustomAccessoriesProps);
			Dictionary<int, int> propTextures = OwlJSON.DeserializeObject<Dictionary<int, int>>(strPropsTexturesJSON, EJsonTrackableIdentifier.CustomAccessoriesTextures);

			foreach (var kvPair in propModels)
			{
				int propIndex = kvPair.Key;
				int propModel = kvPair.Value;

				if (propTextures.ContainsKey(propIndex))
				{
					var propTexture = propTextures[propIndex];
					g_lstPendingTasks.Add(new PropTask(player, propIndex, propModel, propTexture));
				}
			}
		}

		if (m_bDebugSpamEnabled)
		{
			double ms = (DateTime.Now - dtStart).TotalMilliseconds;
			m_dictPerFrameTimings_Accessories[RAGE.Game.Misc.GetFrameCount()] += ms;
			m_dictPerFrameCounts_Accessories[RAGE.Game.Misc.GetFrameCount()]++;
		}
	}
}

