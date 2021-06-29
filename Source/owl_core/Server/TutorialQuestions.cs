using System.Collections.Generic;

public static class QuizQuestionDefinitions
{
	public static List<CQuizQuestion> g_QuizQuestionDefinitions = new List<CQuizQuestion>()
	{
		{ new CQuizQuestion("Choose the best /me to describe sitting down.", "/me sits down.", "/me turns to the chair, lowering himself to sit in it.", "/me thinks about sitting in the chair, then lowers himself to sit in it.", "None of the above, all are poor examples of roleplay.", 1) },
		{ new CQuizQuestion("Your friend asks you for money VIA Discord so they can buy a car in game, should you give it to them?", "Yes", "Yes, if they owe you money.", "No", "Sometimes", 2) },
		{ new CQuizQuestion("Are you allowed to use a bind to pull a rifle out of your locked safe?", "Yes", "Yes, if your rifle has a sling.", "Yes, if the rifle is unloaded.", "No", 3) },
		{ new CQuizQuestion("When is it prohibited to log off?", "If you are being chased by the police.", "If you are in the middle of roleplaying.", "If an Administrator is handling a player report against you.", "All of the above.", 3) },
		{ new CQuizQuestion("You disagree with an administrators decision, what should you do?", "Immediately create a staff report on the UCP and log off.", "Tell the admin they're wrong and do the opposite anyway.", "Demand to speak to an senior ranking administrator.", "Attempt to negotiate further with the administrator before resorting to a staff report.", 3) },
		{ new CQuizQuestion("You have just been deathmatched, the best course of action is to...", "Shoot them before they deathmatch you again at the hospital.", "PM an administrator.", "Report the deathmatching player and continue to roleplay until the report is answered.", "Shoot them and spawncamp their body until an admin arrives.", 2) },
		{ new CQuizQuestion("You are shot during a driveby, when can you eat a taco to heal yourself?", "Ten minutes after being shot.", "Thirty minutes after being shot.", "After asking the perpetrator's permission out of character.", "After receiving suitable medical attention.", 3) },
		{ new CQuizQuestion("You are shot during a gang altercation. Your unconscious body is robbed after. Is this allowed?", "Yes", "No", "I don't know.", "Sometimes", 0) },

		// TODO_GITHUB: You should replace the below with your own website
		{ new CQuizQuestion("You find a bug which duplicates a gun. What should you do?", "Tell everyone in the support channel on Discord.", "Continue to use the bug and keep it a secret.", "Immediately report the bug to bugs.website.com and an administrator.", "Replicate the bug to increase your stash then report the bug after.", 2) },

		{ new CQuizQuestion("Is neglecting a gunshot wound and driving around after considered disregard for life? ", "Yes", "Yes, only if you refuse the help of a paramedic.", "No", "No, if there is no paramedic around to help.", 0) },
		{ new CQuizQuestion("What is considered extreme or disgusting roleplay?", "Violent behaviour include the use of guns or weapons.", "The following specific actions: Rape, Cannibalism, Bestiality, Necrophilia, Pedophilia, Sexual Harassment", "Going basejumping, skydiving or diving in a shipwreck.", "None of the above.", 1) },
		{ new CQuizQuestion("Which situation is considered powergaming?", "Type a single, brief /me that kills someone with one punch.", "Surviving very severe physical injuries even though you received surgery to heal the wounds at the hospital.", "Walking a long distance to get gas from a gas station because you broke down in your car.", "None of the above.", 0) },
		{ new CQuizQuestion("What types of situations make you eligible for character kills?", "Disregard For Life & Roleplaying Death", "Organized Robberies & Supervised Events", "All of the above.", "None of the above.", 2) },
		{ new CQuizQuestion("You do not agree with the current roleplay scenario, what do you do?", "Submit a report and pause the scene until an admin arrives.", "Submit a report and continue roleplaying until an admin arrives.", "Ask all parties to void the roleplay and go your separate ways.", "Lie to everyone that an administrator PMed you voiding the situation.", 1) },
		{ new CQuizQuestion("You think somebody has metagamed your location, what do you do?", "Submit a report and continue roleplaying until an admin arrives.", "Complain to the rule breakers and demand an explanation.", "Switch characters because the roleplay is unfair.", "Refuse to roleplay the situation and get an admin.", 0) },
		{ new CQuizQuestion("You are able to insult someone out of character if they insult you first. This is...", "True, they have broken the golden rule and you can reciprocate the insulting.", "False, insulting someone is against the rules even if they did it to you.", "I don't know.", "It's sometimes allowed under certain circumstances.", 1) },
		
		// TODO_GITHUB: Replace CommunityName with your community name
		{ new CQuizQuestion("To play CommunityName, you should be able to...", "Read and Write broken English", "Read and Write English and French", "Read and Write Legible English", "Read and Write French", 2) },
		{ new CQuizQuestion("You can only use a bind to pull out a one handed weapon.", "True", "False", "I don't know.", "Binds are not allowed.", 1) },
		{ new CQuizQuestion("What is metagaming?", "Talking to your friends on voice while roleplaying.", "Overpowering another player with a weapon with one /me.", "Using out of character information in character.", "Talking about a roleplay situation with a friend after it happened.", 2) },
		{ new CQuizQuestion("Are you allowed to ignore OOC communication during roleplay?", "Yes, unless it is an on duty administrator.", "No.", "Yes, you should always ignore any OOC during RP.", "Yes, if it's an important scene.", 0) },
		{ new CQuizQuestion("During a robbery, you...", "Are not allowed to steal a player's key in their inventory.", "Are ALWAYS allowed to do it without admin approval.", "Are allowed to steal anything you want.", "Are not allowed to steal more than $5,000.", 0) },
		{ new CQuizQuestion("How often may you steal or attempt to steal a vehicle?", "Every 24 Hours", "Every 12 Hours", "Every 6 Hours", "As much as you want.", 0) },
	};
}

// NOTE: Do not remove things from this ever, this is additive only. The index is used in the DB to show which question was asked. Answers will make no sense if you remove things or change the order/index. Instead, set IsActive to false.
public static class QuizWrittenQuestionDefinitions
{
	public static List<CQuizWrittenQuestion> g_QuizQuestionDefinitions = new List<CQuizWrittenQuestion>()
	{
		{ new CQuizWrittenQuestion(0, "Explain what metagaming is and provide at least one (1) complex example and two (2) simple examples.", true) },
		{ new CQuizWrittenQuestion(1, "Explain what powergaming is and provide at least one (1) complex example and two (2) simple examples.", true) },
		{ new CQuizWrittenQuestion(2, "In detail, explain who your first character will be. You must include the name, strengths, weaknesses, skills, ambitions, and general history.", true) },
		{ new CQuizWrittenQuestion(3, "In detail, explain what immersion means in a roleplay server, how it affects your roleplay, and why it is desireable.", true) },
		{ new CQuizWrittenQuestion(4, "Explain what the rules are regarding transferring assets between your own characters and the correct way to do it.", true) },
		{ new CQuizWrittenQuestion(5, "You are driving, when all of a sudden you are hit by another vehicle at a high rate of speed. Explain how you would roleplay the results and give several what-if scenarios.", true) },
		{ new CQuizWrittenQuestion(6, "How would you roleplay a physical altercation between yourself and another player.", true) },
		{ new CQuizWrittenQuestion(7, "Your friend sends you a PM on Discord and tells you to meet at Paleto Bay to rob another player. What do you do?", true) },
		{ new CQuizWrittenQuestion(8, "Please explain what character development and give examples of why it is important. Describe an example of how lacking character development will hurt your roleplay and benifit it.", true) },
		{ new CQuizWrittenQuestion(9, "Another player pulls up to you in a vehicle, asking whether you require a ride through out of character chat. What do you proceed to do?", true) },
		{ new CQuizWrittenQuestion(10, "Please describe what scamming means in our rules. Give one example of scamming and one example of not scamming.", true) },
		{ new CQuizWrittenQuestion(11, "Explain how one should generally behave out of character towards others while playing.", true) },
		{ new CQuizWrittenQuestion(12, "Explain what disregard for life is. Give an example of it and then explain what a reasonable and prudent person would do to avoid disregard in this fictional scenario.", true) },
		{ new CQuizWrittenQuestion(13, "What are the major distinguishing factors between a character kill and a player kill? Explain why a player kill is still a substantial event in a character's life.", true) },
		{ new CQuizWrittenQuestion(14, "You were shot by the police with a taser; explain in detail how you would react and provide a sample /me of the scene.", true) },
		{ new CQuizWrittenQuestion(15, "Explain how you would proceed if a player private messaged you, advertising another server, and you came across a bug which duplicated items.", true) },
		{ new CQuizWrittenQuestion(16, "You ask your friend to meet you at a gas station. They refuse and you do not meet up. What rule did you break, if any?", true) },
		{ new CQuizWrittenQuestion(17, "You decide to roleplay robbing a convenient store and no police arrive. Ten minutes later you hide your cash you stole at your apartment. Can you log out now? Explain why or why not.", true) },
		{ new CQuizWrittenQuestion(18, "Give two (2) examples of what would generally be considered an unrealistic character that you should avoid.", true) },
		{ new CQuizWrittenQuestion(19, "You are sitting in your vehicle in a parking lot when someone approaches you with a weapon, demanding money. Describe how you would proceed and give roleplay examples.", true) },
		{ new CQuizWrittenQuestion(20, "Please explain the rules relating to player killing and robbing another character.", true) },
		{ new CQuizWrittenQuestion(21, "You are shot twice in the arm and run away, bleeding quite a bit. You manage to tie a bandana around the gunshot wounds. Is this acceptable roleplay? Explain why or why not." , true) },
		{ new CQuizWrittenQuestion(22, "A player is unable to roleplay with you in some capacity due to out of character limitations, such as being banned. How do you roleplay their lack of presence?", true) },
		{ new CQuizWrittenQuestion(23, "Give an example of three (3) different roleplay situations where all players involved must agree out of character before participating.", true) },
		{ new CQuizWrittenQuestion(24, "A player is driving along and hits into the side of your vehicle, he/she blames it on lag. How should you handle the situation?", true) },
		{ new CQuizWrittenQuestion(25, "What is a banned roleplay activity?", true) },
	};
}