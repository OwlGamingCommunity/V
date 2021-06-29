using GTANetworkAPI;
using System.Collections.Generic;

public static class CharacterSkinHelper
{
	public static bool IsCustomSkin(uint skinID)
	{
		PedHash GTAskinID = (PedHash)skinID;
		return IsCustomSkin(GTAskinID);
	}

	public static bool IsCustomSkin(PedHash GTAskinID)
	{
		return (GTAskinID == PedHash.FreemodeMale01 || GTAskinID == PedHash.FreemodeFemale01);
	}
}

public class CustomCharacterSkinData
{
	public int Ageing { get; set; }
	public float AgeingOpacity { get; set; }
	public int Makeup { get; set; }
	public float MakeupOpacity { get; set; }
	public int MakeupColor { get; set; }
	public int MakeupColorHighlight { get; set; }
	public int Blush { get; set; }
	public float BlushOpacity { get; set; }
	public int BlushColor { get; set; }
	public int BlushColorHighlight { get; set; }
	public int Complexion { get; set; }
	public float ComplexionOpacity { get; set; }
	public int SunDamage { get; set; }
	public float SunDamageOpacity { get; set; }
	public int Lipstick { get; set; }
	public float LipstickOpacity { get; set; }
	public int LipstickColor { get; set; }
	public int LipstickColorHighlights { get; set; }
	public int MolesAndFreckles { get; set; }
	public float MolesAndFrecklesOpacity { get; set; }
	public float NoseSizeHorizontal { get; set; }
	public float NoseSizeVertical { get; set; }
	public float NoseSizeOutwards { get; set; }
	public float NoseSizeOutwardsUpper { get; set; }
	public float NoseSizeOutwardsLower { get; set; }
	public float NoseAngle { get; set; }
	public float EyebrowHeight { get; set; }
	public float EyebrowDepth { get; set; }
	public float CheekboneHeight { get; set; }
	public float CheekWidth { get; set; }
	public float CheekWidthLower { get; set; }
	public float EyeSize { get; set; }
	public float LipSize { get; set; }
	public float MouthSize { get; set; }
	public float MouthSizeLower { get; set; }
	public float ChinSize { get; set; }
	public float ChinSizeLower { get; set; }
	public float ChinWidth { get; set; }
	public float ChinEffect { get; set; }
	public float NeckWidth { get; set; }
	public float NeckWidthLower { get; set; }
	public int FaceBlend1Mother { get; set; }
	public int FaceBlend1Father { get; set; }
	public float FaceBlendFatherPercent { get; set; }
	public float SkinBlendFatherPercent { get; set; }
	public int BaseHair { get; set; }
	public int HairStyle { get; set; }
	public int HairColor { get; set; }
	public int HairColorHighlights { get; set; }
	public int EyeColor { get; set; }
	public int FacialHairStyle { get; set; }
	public int FacialHairColor { get; set; }
	public int FacialHairColorHighlight { get; set; }
	public float FacialHairOpacity { get; set; }
	public int Blemishes { get; set; }
	public float BlemishesOpacity { get; set; }
	public int Eyebrows { get; set; }
	public float EyebrowsOpacity { get; set; }
	public int EyebrowsColor { get; set; }
	public int EyebrowsColorHighlight { get; set; }
	public int BodyBlemishes { get; set; }
	public float BodyBlemishesOpacity { get; set; }
	public int ChestHair { get; set; }
	public int ChestHairColor { get; set; }
	public int ChestHairColorHighlight { get; set; }
	public float ChestHairOpacity { get; set; }
	public int FullBeardStyle { get; set; }
	public int FullBeardColor { get; set; }

	public CustomCharacterSkinData()
	{

	}

	public CustomCharacterSkinData(Dictionary<string, string> row)
	{
		Ageing = int.Parse(row["ageing"]);
		AgeingOpacity = float.Parse(row["ageing_opacity"]);
		Makeup = int.Parse(row["makeup"]);
		MakeupOpacity = float.Parse(row["makeup_opacity"]);
		MakeupColor = int.Parse(row["makeup_color"]);
		MakeupColorHighlight = int.Parse(row["makeup_color_highlight"]);
		Blush = int.Parse(row["blush"]);
		BlushOpacity = float.Parse(row["blush_opacity"]);
		BlushColor = int.Parse(row["blush_color"]);
		BlushColorHighlight = int.Parse(row["blush_color_highlight"]);
		Complexion = int.Parse(row["complexion"]);
		ComplexionOpacity = float.Parse(row["complexion_opacity"]);
		SunDamage = int.Parse(row["sundamage"]);
		SunDamageOpacity = float.Parse(row["sundamage_opacity"]);
		Lipstick = int.Parse(row["lipstick"]);
		LipstickOpacity = float.Parse(row["lipstick_opacity"]);
		LipstickColor = int.Parse(row["lipstick_color"]);
		LipstickColorHighlights = int.Parse(row["lipstick_color_highlights"]);
		MolesAndFreckles = int.Parse(row["moles_and_freckles"]);
		MolesAndFrecklesOpacity = float.Parse(row["moles_and_freckles_opacity"]);
		NoseSizeHorizontal = float.Parse(row["nose_size_horizontal"]);
		NoseSizeVertical = float.Parse(row["nose_size_vertical"]);
		NoseSizeOutwards = float.Parse(row["nose_size_outwards"]);
		NoseSizeOutwardsUpper = float.Parse(row["nose_size_outwards_upper"]);
		NoseSizeOutwardsLower = float.Parse(row["nose_size_outwards_lower"]);
		NoseAngle = float.Parse(row["nose_angle"]);
		EyebrowHeight = float.Parse(row["eyebrow_height"]);
		EyebrowDepth = float.Parse(row["eyebrow_depth"]);
		CheekboneHeight = float.Parse(row["cheekbone_height"]);
		CheekWidth = float.Parse(row["cheek_width"]);
		CheekWidthLower = float.Parse(row["cheek_width_lower"]);
		EyeSize = float.Parse(row["eye_size"]);
		LipSize = float.Parse(row["lip_size"]);
		MouthSize = float.Parse(row["mouth_size"]);
		MouthSizeLower = float.Parse(row["mouth_size_lower"]);
		ChinSize = float.Parse(row["chin_size"]);
		ChinSizeLower = float.Parse(row["chin_size_lower"]);
		ChinWidth = float.Parse(row["chin_width"]);
		ChinEffect = float.Parse(row["chin_effect"]);
		NeckWidth = float.Parse(row["neck_width"]);
		NeckWidthLower = float.Parse(row["neck_width_lower"]);
		FaceBlend1Mother = int.Parse(row["face_blend_1_mother"]);
		FaceBlend1Father = int.Parse(row["face_blend_1_father"]);
		FaceBlendFatherPercent = float.Parse(row["face_blend_father_percent"]);
		SkinBlendFatherPercent = float.Parse(row["skin_blend_father_percent"]);
		BaseHair = int.Parse(row["base_hair"]);
		HairStyle = int.Parse(row["hair_style"]);
		HairColor = int.Parse(row["hair_color"]);
		HairColorHighlights = int.Parse(row["hair_color_highlight"]);
		EyeColor = int.Parse(row["eye_color"]);
		FacialHairStyle = int.Parse(row["facial_hair_style"]);
		FacialHairColor = int.Parse(row["facial_hair_color"]);
		FacialHairColorHighlight = int.Parse(row["facial_hair_color_highlight"]);
		FacialHairOpacity = float.Parse(row["facial_hair_opacity"]);
		Blemishes = int.Parse(row["blemishes"]);
		BlemishesOpacity = float.Parse(row["blemishes_opacity"]);
		Eyebrows = int.Parse(row["eyebrows"]);
		EyebrowsOpacity = float.Parse(row["eyebrows_opacity"]);
		EyebrowsColor = int.Parse(row["eyebrows_color"]);
		EyebrowsColorHighlight = int.Parse(row["eyebrows_color_highlight"]);
		BodyBlemishes = int.Parse(row["body_blemishes"]);
		BodyBlemishesOpacity = float.Parse(row["body_blemishes_opacity"]);
		ChestHair = int.Parse(row["chest_hair"]);
		ChestHairColor = int.Parse(row["chest_hair_color"]);
		ChestHairColorHighlight = int.Parse(row["chest_hair_color_highlights"]);
		ChestHairOpacity = float.Parse(row["chest_hair_opacity"]);
		FullBeardStyle = int.Parse(row["full_beard_style"]);
		FullBeardColor = int.Parse(row["full_beard_color"]);
	}
}
