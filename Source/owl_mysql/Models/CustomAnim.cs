using System;
using System.Collections.Generic;
using EntityDatabaseID = System.Int64;

namespace Database.Models
{
	public class CustomAnim : BaseModel
	{
		public EntityDatabaseID ID { get; private set; }
		public EntityDatabaseID AccountID { get; private set; }

		public string CommandName { get; private set; }
		public string AnimDictionary { get; private set; }
		public string AnimName { get; private set; }

		public bool Loop { get; private set; }
		public bool StopOnLastFrame { get; private set; }
		public bool OnlyAnimateUpperBody { get; private set; }
		public bool AllowPlayerMovement { get; private set; }
		public int Duration { get; private set; }

		public CustomAnim(EntityDatabaseID id, EntityDatabaseID accountID, string strCommandName, string strAnimDictionary, string strAnimName, bool bLoop, bool bStopOnLastFrame, bool bOnlyAnimateUpperBody, bool bAllowPlayerMovement, int duration)
		{
			ID = id;
			AccountID = accountID;
			CommandName = strCommandName;
			AnimDictionary = strAnimDictionary;
			AnimName = strAnimName;
			Loop = bLoop;
			StopOnLastFrame = bStopOnLastFrame;
			OnlyAnimateUpperBody = bOnlyAnimateUpperBody;
			AllowPlayerMovement = bAllowPlayerMovement;
			Duration = duration;
		}

		public Dictionary<string, object> ToDictionary()
		{
			return new Dictionary<string, object>
			{
				{"id", ID},
				{"account_id", AccountID},
				{"command_name", CommandName},
				{"anim_dictionary", AnimDictionary},
				{"anim_name", AnimName},
				{"loop", Loop},
				{"stop_on_last_frame", StopOnLastFrame},
				{"only_animate_upper_body", OnlyAnimateUpperBody},
				{"allow_player_movement", AllowPlayerMovement},
				{"duration", Duration}
			};
		}

		public static CustomAnim FromDB(CMySQLRow row)
		{
			return new CustomAnim(
				row.GetValue<EntityDatabaseID>("id"),
				row.GetValue<EntityDatabaseID>("account_id"),
				row.GetValue<string>("command_name"),
				row.GetValue<string>("anim_dictionary"),
				row.GetValue<string>("anim_name"),
				row.GetValue<bool>("loop"),
				row.GetValue<bool>("stop_on_last_frame"),
				row.GetValue<bool>("only_animate_upper_body"),
				row.GetValue<bool>("allow_player_movement"),
				row.GetValue<int>("duration")
			);
		}

		public static void Create(EntityDatabaseID accountID, string strCommandName, string strAnimDictionary, string strAnimName, bool bLoop, bool bStopOnLastFrame, bool bOnlyAnimateUpperBody, bool bAllowPlayerMovement, int duration, Action<CustomAnim> callback)
		{
			CustomAnim dbModel = new CustomAnim(0, accountID, strCommandName, strAnimDictionary, strAnimName, bLoop, bStopOnLastFrame, bOnlyAnimateUpperBody, bAllowPlayerMovement, duration);
			dbModel.Save(callback);
		}

		public void Delete(Action callback)
		{
			Functions.Accounts.DeleteCustomAnim(this, callback);
		}

		public void Save(Action<CustomAnim> callback = null)
		{
			if (Id == NO_ID)
			{
				Functions.Accounts.CreateCustomAnim(this, callback);
			}
			else
			{
				Functions.Accounts.UpdateCustomAnim(this, callback);
			}
		}
	}
}