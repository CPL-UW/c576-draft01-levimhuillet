# Win-surance Tower Defense

Author: Levi Huillet

Theme: Tower Defense

Feature: Multiplayer Sync

Learning Objectives: Insurance (why it's important and Adverse Selection)


Game Overview:

	You are managing a weather control station in the birthplace of all the world's storms and natural disasters.
	As you can imagine, this location affords lots of research opportunities, but also high risks. Heavy infrastructure damage is common.
	Luckily, you can buy insurance. You and other stations in the region are all insured under a company called Win-surance Co.
	This game teaches basic principles of insurance (such as why someone might want insurance), as well as the more advanced principle of insurance known as Adverse Selection.


Tower Defense Aspects:

	- Oncoming elements take a shortest path from their starting point to the player's base (-ish. Still needs to make the transition to Dijkstra's A*).
	
	- Players can place their towers on the grid in valid cells (designated for towers and empty)

	- Players can select from multiple types of towers, each which have their own specifications (range, cost, etc.)
	
	- The player's base has reserves that depletes whenever a natural disaster reaches it and the player must pay for the loss


Insurance Aspects:
	
	- Buy insurance against natural disasters (hurricanes, wildfires, and floods).
	
	- Insurance is broken into premiums (paid every period) and deductibles (how much is paid out-of-pocket for each instance).
	
	- Player can choose whether to buy insurance


Multiplayer Aspects:

	- A waiting room has been added before each level. However, this is not currently functional.

	- Once they are implemented, multiple players will be key for teaching about Adverse Selection
	
	- Build1 and Build2 are included to facilitate running in multiplayer
	

Miscellaneous Mechanics:

	- Elements spawn out of Nexuses, which in turn spawn from Chaos Butterflies. Every butterfly that crosses the screen has a chance to instigate a Nexus pretty much anywhere, which grows and grows before moving to its designated spawn point. How many enemies a Nexus spawns depends on how much it grew during its growth stage.
	
	- Nexuses have a slight chance to be severe weather, which results in more, stronger oncoming elements.

	- Player receives a set amount of money per period to pay for towers and insurance
	
	- Menus are much more navigable than previously.


Issues (Mostly Multiplayer):

	- Multiplayer is still very, very basic, since that is the aspect I am least familiar with.
	
	- I am aware that I need to be careful of mechanic bloat.
	
	- While the game currently has 8 levels, only 1, 2, and 7 have seen significant testing and balancing. The rest may be ridiculously difficult or ridiculously easy.
	

Future plans:

	- Now that the groundwork is mostly laid for insurance and base mechanics, I'm looking to work more on multiplayer for the next iteration.
	
	- I would like to add more mechanics for insurance, such as adding forms to fill out and raising premiums each time you submit a report. But since I am already concerned about bloat, I need to serious consideration about the most important aspects of insurance to teach and which mechanics are most relevant to that learning objective. If it all works together, great. If it's too much, I'll need to be okay with trimming.
	
	- More explicit tutorializing. Explaining what is going on beneath the deductibles when you take losses.