using System;
using System.Collections.Generic;

using EntityDatabaseID = System.Int64;

public class CAchievementInstance
{
	public CAchievementInstance(EntityDatabaseID a_DatabaseID, int a_AccountId, EAchievementID a_AchievementID, Int64 a_UnlockTimestamp)
	{
		DatabaseID = a_DatabaseID;
		AccountId = a_AccountId;
		AchievementID = a_AchievementID;
		UnlockTimestamp = a_UnlockTimestamp;
	}

	public EntityDatabaseID DatabaseID { get; }
	public int AccountId { get; }
	public EAchievementID AchievementID { get; }
	public Int64 UnlockTimestamp { get; }
}

public class CAchievementDefinition
{
	public CAchievementDefinition(EAchievementID a_AchievementID, string a_strTitle, string a_strCaption, int a_Points)
	{
		AchievementID = a_AchievementID;
		Title = a_strTitle;
		Caption = a_strCaption;
		Points = a_Points;
	}

	public EAchievementID AchievementID { get; }
	public string Title { get; }
	public string Caption { get; }
	public int Points { get; }
}

public static class AchievementDefinitions
{
	public static Dictionary<EAchievementID, CAchievementDefinition> g_AchievementDefinitions = new Dictionary<EAchievementID, CAchievementDefinition>()
	{
		{ EAchievementID.WelcomeToPaletoBay, new CAchievementDefinition(EAchievementID.WelcomeToPaletoBay, "Welcome to Paleto Bay", "Arrive in Paleto Bay", 5) },
		{ EAchievementID.Play_5Hours, new CAchievementDefinition(EAchievementID.Play_5Hours, "I'm just gettin' started", "Play 5 hours on your account", 5) },
		{ EAchievementID.Play_10Hours, new CAchievementDefinition(EAchievementID.Play_10Hours, "I could do this all day", "Play 10 hours on your account", 5) },
		{ EAchievementID.Play_50Hours, new CAchievementDefinition(EAchievementID.Play_50Hours, "Veteran", "Play 50 hours on your account", 5) },
		{ EAchievementID.Play_100Hours, new CAchievementDefinition(EAchievementID.Play_100Hours, "Vitamin D deficiency", "Play 100 hours on your account", 5) },
		{ EAchievementID.Play_500Hours, new CAchievementDefinition(EAchievementID.Play_500Hours, "I should get out more", "Play 500 hours on your account", 5) },
		{ EAchievementID.Play_1000Hours, new CAchievementDefinition(EAchievementID.Play_1000Hours, "No Lifer", "Play 1000 hours on your account", 5) },
		{ EAchievementID.Earn_10kMoney, new CAchievementDefinition(EAchievementID.Earn_10kMoney, "Small Change", "Obtain $10,000 balance", 5) },
		{ EAchievementID.Earn_50kMoney, new CAchievementDefinition(EAchievementID.Earn_50kMoney, "It's a start", "Obtain $50,000 balance", 5) },
		{ EAchievementID.Earn_100kMoney, new CAchievementDefinition(EAchievementID.Earn_100kMoney, "Moving up a class", "Obtain $100,000 balance", 5) },
		{ EAchievementID.Earn_500kMoney, new CAchievementDefinition(EAchievementID.Earn_500kMoney, "The First 500k", "Obtain $500,000 balance", 5) },
		{ EAchievementID.Earn_1mMoney, new CAchievementDefinition(EAchievementID.Earn_1mMoney, "Donald Trump", "Obtain $1,000,000 balance", 5) },
		{ EAchievementID.BuyCar, new CAchievementDefinition(EAchievementID.BuyCar, "New Wheels", "Purchase a vehicle", 5) },
		{ EAchievementID.TruckerJob, new CAchievementDefinition(EAchievementID.TruckerJob, "Truckin' For My Paychecks", "Accept the Trucker job", 5) },
		{ EAchievementID.DeliveryJob, new CAchievementDefinition(EAchievementID.DeliveryJob, "Dropping Packages", "Accept the Delivery Driver job", 5) },
		{ EAchievementID.MailmanJob, new CAchievementDefinition(EAchievementID.MailmanJob, "I deliver on time, 35% of the time", "Become a mail man", 5) },
		{ EAchievementID.TaxiDriverJob, new CAchievementDefinition(EAchievementID.TaxiDriverJob, "Cabbie", "Become a taxi driver", 5) },
		{ EAchievementID.TrashManJob, new CAchievementDefinition(EAchievementID.TrashManJob, "Dirty Work", "Become a trash collector", 5) },
		{ EAchievementID.BusDriverJob, new CAchievementDefinition(EAchievementID.BusDriverJob, "All Aboard", "Become a bus driver", 5) },
		{ EAchievementID.LEOFaction, new CAchievementDefinition(EAchievementID.LEOFaction, "Got My Badge", "Join a Law Enforcement Faction", 5) },
		{ EAchievementID.EMSFaction, new CAchievementDefinition(EAchievementID.EMSFaction, "First Responder", "Join the EMS Faction", 5) },
		{ EAchievementID.NewsFaction, new CAchievementDefinition(EAchievementID.NewsFaction, "Not Fake News", "Join Weazel News", 5) },
		{ EAchievementID.JoinFaction, new CAchievementDefinition(EAchievementID.JoinFaction, "I'm just a Minion", "Join a Faction", 5) },
		{ EAchievementID.CreateFaction, new CAchievementDefinition(EAchievementID.CreateFaction, "CEO", "Create a Faction", 5) },
		{ EAchievementID.BeFined, new CAchievementDefinition(EAchievementID.BeFined, "Law 1, Me 0", "Be fined by the police", 5) },
		{ EAchievementID.BeJailed, new CAchievementDefinition(EAchievementID.BeJailed, "Don't drop the soap...", "Be arrested by the police", 5) },
		{ EAchievementID.ArrestSomeone, new CAchievementDefinition(EAchievementID.ArrestSomeone, "Enforcement", "Arrest someone", 5) },
		{ EAchievementID.ShootSomeoneAsCop, new CAchievementDefinition(EAchievementID.ShootSomeoneAsCop, "You don't forget your first time", "Kill someone whilst on LEO duty", 5) },
		{ EAchievementID.BeShotByCop, new CAchievementDefinition(EAchievementID.BeShotByCop, "Internal Affairs Investigation Incoming!", "Be killed by an on duty LEO", 5) },
		{ EAchievementID.CustomCharacter, new CAchievementDefinition(EAchievementID.CustomCharacter, "Fussy", "Create a custom character", 5) },
		{ EAchievementID.BuyProperty, new CAchievementDefinition(EAchievementID.BuyProperty, "New Digs", "Purchase a property", 5) },
		{ EAchievementID.CompleteTruckerJob, new CAchievementDefinition(EAchievementID.CompleteTruckerJob, "Master Trucker", "Complete all levels of the trucker job", 25) },
		{ EAchievementID.CompleteDeliveryJob, new CAchievementDefinition(EAchievementID.CompleteDeliveryJob, "Master Delivery Driver", "Complete all levels of the delivery driver job", 25) },
		{ EAchievementID.CompleteMailmanJob, new CAchievementDefinition(EAchievementID.CompleteMailmanJob, "Master Mail man", "Complete all levels of the mail man job", 25) },
		{ EAchievementID.CompleteFareTaxiDriverJob, new CAchievementDefinition(EAchievementID.CompleteFareTaxiDriverJob, "Got you there in one piece", "Complete a taxi fare", 25) },
		{ EAchievementID.CompleteTrashManJob, new CAchievementDefinition(EAchievementID.CompleteTrashManJob, "Master Trash Collector", "Complete all levels of the trash collector job", 25) },
		{ EAchievementID.CompleteBusDriverJob, new CAchievementDefinition(EAchievementID.CompleteBusDriverJob, "Master Bus Driver", "Complete all levels of the bus driver job", 25) },
		{ EAchievementID.CrimeFaction, new CAchievementDefinition(EAchievementID.CrimeFaction, "Bad Hombre", "Join a criminal faction", 5) },
		{ EAchievementID.BailOut, new CAchievementDefinition(EAchievementID.BailOut, "Bail Bonds", "Bail out of prison", 5) },
		{ EAchievementID.IssueFine, new CAchievementDefinition(EAchievementID.IssueFine, "Earning Dollars for the State", "Issue a ticket", 5) },
		{ EAchievementID.PremadeCharacter, new CAchievementDefinition(EAchievementID.PremadeCharacter, "Keeping It Simple", "Create a premade character", 5) },
		{ EAchievementID.Uganda, new CAchievementDefinition(EAchievementID.Uganda, "Do You Know Da Wae?", "Pay respects to a dead Ugandan meme.", 15) },
		{ EAchievementID.EatATaco, new CAchievementDefinition(EAchievementID.EatATaco, "It's Taco Time", "Eat some fine Mexican cuisine. The nearest toilet is in Paleto Gas.", 5) },
		{ EAchievementID.WelcomeToLosSantos, new CAchievementDefinition(EAchievementID.WelcomeToLosSantos, "Welcome to Los Santos", "Arrive in Los Santos", 5) },
		{ EAchievementID.TagRemoverJob, new CAchievementDefinition(EAchievementID.TagRemoverJob, "I hate art", "Become a graffiti cleaner", 5) },
		{ EAchievementID.CreateCustomRadioStation, new CAchievementDefinition(EAchievementID.CreateCustomRadioStation, "Pirate Radio", "Create a custom radio station", 5) },
		{ EAchievementID.Christmas, new CAchievementDefinition(EAchievementID.Christmas, "Merry Christmas", "Take part in the Christmas celebrations", 5) },
		{ EAchievementID.Vubstersmurf, new CAchievementDefinition(EAchievementID.Vubstersmurf, "Ello Guvnah", "Attempt to teach Vubstersmurf proper English", 5) },
		{ EAchievementID.StartFishing, new CAchievementDefinition(EAchievementID.StartFishing, "Baiting", "Fish for the first time", 5) },
		{ EAchievementID.FishingMaxLevel, new CAchievementDefinition(EAchievementID.FishingMaxLevel, "Master Baitor", "Reach the maximum level in fishing", 25) },
		{ EAchievementID.BuyBoat, new CAchievementDefinition(EAchievementID.BuyBoat, "I'm on a boat", "Purchase a boat", 5) },
		{ EAchievementID.LearnLanguage, new CAchievementDefinition(EAchievementID.LearnLanguage, "Now you can complain in a different language", "Learn a new language for 100%", 5) },
		{ EAchievementID.MakeOutfit, new CAchievementDefinition(EAchievementID.MakeOutfit, "The Zuck", "Possess one outfit, and one outfit only", 5) },
		{ EAchievementID.Make10Outfits, new CAchievementDefinition(EAchievementID.Make10Outfits, "Fashionista", "Have 10 or more outfits", 25) },
		{ EAchievementID.Halloween, new CAchievementDefinition(EAchievementID.Halloween, "Happy Halloween", "Take part in the Halloween celebrations", 5) },
		{ EAchievementID.Halloween_RIP, new CAchievementDefinition(EAchievementID.Halloween_RIP, "Rest In Pieces", "Die and come back to life during Halloween", 5) },
		{ EAchievementID.RunningCode, new CAchievementDefinition(EAchievementID.RunningCode, "Running Code", "Respond to an incident at high speed with your lights on.", 5) },
		{ EAchievementID.GoingInQuiet, new CAchievementDefinition(EAchievementID.GoingInQuiet, "Going In Quiet", "Use your lights without the siren.", 5) },
		{ EAchievementID.RiskyMove, new CAchievementDefinition(EAchievementID.RiskyMove, "Risky Move", "Hang from a vehicle.", 5) },
		{ EAchievementID.NeverGoFullSemiAuto, new CAchievementDefinition(EAchievementID.NeverGoFullSemiAuto, "Never Go Full Semi Auto", "Acquire a semi-automatic civilian weapon.", 5) },
		{ EAchievementID.WinBlackjack, new CAchievementDefinition(EAchievementID.WinBlackjack, "I'm a Winner!", "Win a hand of Blackjack.", 5) },
		{ EAchievementID.LoseBlackjack, new CAchievementDefinition(EAchievementID.LoseBlackjack, "I'm a loser baby!", "Lose a hand of Blackjack.", 5) },
		{ EAchievementID.GetBlackjack, new CAchievementDefinition(EAchievementID.GetBlackjack, "21", "Get Blackjack.", 15) },
		{ EAchievementID.GoBust, new CAchievementDefinition(EAchievementID.GoBust, "I can't count", "Go bust at Blackjack.", 5) },
		{ EAchievementID.GetFiveCardsAndWinBlackjack, new CAchievementDefinition(EAchievementID.GetFiveCardsAndWinBlackjack, "Five Card Trick", "During a hand of Blackjack, get 5 cards and still win.", 15) },
		{ EAchievementID.SpeedCamera, new CAchievementDefinition(EAchievementID.SpeedCamera, "Speed Demon", "Get caught speeding by a traffic camera.", 5) },
		{ EAchievementID.CayoPericoIsland, new CAchievementDefinition(EAchievementID.CayoPericoIsland, "The Island", "Visit Cayo Perico.", 5) },
	};
}