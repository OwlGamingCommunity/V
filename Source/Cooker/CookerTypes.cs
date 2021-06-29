using Newtonsoft.Json;
using System.Collections.Generic;

#pragma warning disable 0649
#pragma warning disable CA1051 // Do not declare visible instance fields
namespace CookerTypes
{
	public struct AssetFile : System.IEquatable<AssetFile>
	{
		public string Name;
		public EAssetType Type;

		public override bool Equals(object obj)
		{
			AssetFile rhs = (AssetFile)obj;
			return (this.Name == rhs.Name && this.Name == rhs.Name);
		}

		public override int GetHashCode()
		{
			return (int)FastCRC32.ComputeHash(CookerHelpers.GetBytes(Name));
		}

		public static bool operator ==(AssetFile left, AssetFile right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(AssetFile left, AssetFile right)
		{
			return !(left == right);
		}

		public bool Equals(AssetFile other)
		{
			return this == other;
		}
	}

	public enum EAssetType
	{
		ServerScript,
		ClientScript,
		ClientAsset,
		ServerDependency,
		Meta,
		ServerAsset,
		DotNetConfigFile,
		RageOverrideDLL,
		Audio,
		MapFile_Interior,
		MapFile_Persistent,
	}

	enum EModType
	{
		Unknown,
		LegacyRPF,
		ModernRaw,
		ModernData
	}

#pragma warning disable CA1815 // Override equals and operator equals on value types
	public struct AssetExportedFunctions
#pragma warning restore CA1815 // Override equals and operator equals on value types
	{
		public string Class;
		public string Function;
	}

	public class AssetDescriptor
	{
		public List<AssetFile> Files = new List<AssetFile>();
		public List<AssetExportedFunctions> ExportedFunctions = new List<AssetExportedFunctions>();

		public string FastSerialize()
		{
			if (Files.Count == 0 && ExportedFunctions.Count == 0)
			{
				return "{\"Files\": [],\"ExportedFunctions\": []}";
			}

			return JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
		}
	}

	public enum EArchiveResult
	{
		UpToDate,
		NeedsUpdate,
		Error
	}
}
#pragma warning restore CA1051 // Do not declare visible instance fields
#pragma warning restore 0649