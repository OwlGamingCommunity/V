# V Game Data

Associated Game Data for the GTA:V server. 
 
## itemdata.json

JSON formatted file which contains information about items in game. 

#General Info

```
{
    "ItemId": 102, // see the appended list below
    "Name": "Smoke Grenade Ammo", // item name
    "ValueName": "", // do not touch
    "Desc": "A box of ammo", // description
    "Weight": 0.1, // weight consumed (in lbs)
    "Cost": 25.0, // cost in store
    "DefaultVal": 0.0, // default value when bought, can probably stay at zero
    "Limit": 255, // how many can one person carry, regardless of weight?
    "Model": -1497794201, // world model hash, find something suitable (if you can't find something from here: https://cdn.rage.mp/public/odb then ask ThatGuy i can find a suitable model in OpenIV)
    "CanTake": true, // can this item be taken via frisking?
    "SerializationType": "CItemValueBasic, owl_core", // do not touch
    "CanSplit": true, // can this item be split, aka is it stackable
    "IsContainer": false, // is this a container?
    "MaxStack": 200, // how big can we stack these (if stackable) for example if for heroin it's set to 20, you can have up to 20 heroin items in a single slot. so if you then have 40 heroin, it's 2 stacks of 20.
    "Sockets": [
      // sockets that this item can go to (e.g. in pockets, etc), see list below. there's currently not a hands socket for carrying large objects so use the "back" slot. (e.g. a minigun can't be put in pockets but it'd be carried in your hands, so use the back slot for now, or with the canvas tote, it'd be in your hands or on your shoulder, so use the back slot)
    ],
    "AcceptedItems": [
      // what items can this container hold (if container), provide ItemID None from list if it can take every item (unlikely)
    ],
    "ContainerCapacity": 0, // how many extra slots does this container provide
    "WeightAddon": 0.0, // how much extra weight does this container let the person carry
    "DefaultStackSize": 1 // how big is the stack when given in a store? this is 1 for most cases, for ammo its the number of bullets
},
```

#Socket and Item List

```
ITEMS

0,  //  Knife
1,  //  Nightstick
2,  //  Hammer
3,  //  Baseball Bat
4,  //  Golf Club
5,  //  Crowbar
6,  //  Pistol
7,  //  Combat Pistol
8,  //  AP Pistol
9,  //  .50 Pistol
10, //  Micro SMG
11, //  SMG
12, //  Assault SMG
13, //  Assault Rifle
14, //  Carbine Rifle
15, //  Advanced Rifle
16, //  Machine Gun
17, //  Combat Machine Gun
18, //  Pump Shotgun
19, //  Sawed-Off Shotgun
20, //  Assault Shotgun
21, //  Bullpup Shotgun
22, //  Stun Gun
23, //  Sniper Rifle
24, //  Heavy Sniper Rifle
25, //  Grenade Launcher
26, //  Smoke Grenade Launcher
27, //  RPG Launcher
28, //  Minigun
29, //  Grenade
30, //  Sticky Bomb
31, //  Smoke Grenade
32, //  Tear Gas
33, //  Molotov Cocktail
34, //  Fire Extinguisher
35, //  Gas Can
36, //  SNS Pistol
37, //  Special Carbine
38, //  Heavy Pistol
39, //  Bullpup Rifle
40, //  Homing Launcher
41, //  Proximity Mine
42, //  Snowball
43, //  Vintage Pistol
44, //  Dagger
45, //  Firework Launcher
46, //  Musket
47, //  Marksman Rifle
48, //  Heavy Shotgun
49, //  Gusenberg Sweeper
50, //  Hatchet
51, //  Railgun
52, //  Combat PDW
53, //  Knuckle Duster
54, //  Marksman Pistol
55, //  Broken Bottle
56, //  Flare Gun
57, //  Flare
58, //  Revolver
59, //  Switchblade
60, //  Machete
61, //  Flashlight
62, //  Machine Pistol
63, //  Double Barrel Shotgun
64, //  Compact Rifle
65, //  Battle Axe
66, //  Baseball
67, //  Parachute
68, //  Pipe Wrench
69, //  Compact Launcher
70, //  Mini SMG
71, //  Automatic Shotgun
72, //  Driving Permit (Motorbike)
73, //  Driving Permit (Car)
74, //  Driving Permit (Large Vehicle)
75, //  Watch
76, //  Vehicle Key
77, //  Taco
78, //  Firearm Permit
79, //  Large Weapons Permit
80, //  Property Key
81, //  IIA Kevlar Vest
82, //  IIIA Kevlar Vest
83, //  IV Ceramic Plate and Carrier
84, //  Clothes
85, //  Handcuffs
86, //  Handcuff Key
87, //  Backpack
88, //  Holster
89, //  Leg Holster
90, //  Small Canvas Tote
91, //  Marijuana
92, //  Cellphone
93, //  Spike Strip
94, //  Handgun Ammo
95, //  Rifle Ammo
96, //  Shotgun Ammo
97, //  Stun Gun Ammo
98, //  Grenade Ammo
99, //  Rocket
100, // Flare
101, // Riot Shield
102, // Ballistic Shield
103, // Clothes (Custom Face)
104, // Clothes (Custom Mask)
105, // Clothes (Custom Hair)
106, // Clothes (Custom Torso)
107, // Clothes (Custom Legs)
108, // Clothes (Custom Back )
109, // Clothes (Custom Feet)
110, // Clothes (Custom Accessory)
111, // Clothes (Custom Undershirt)
112, // Clothes (Custom BodyArmor)
113, // Clothes (Custom Decals)
114, // Clothes (Custom Tops)
115, // Clothes (Custom Helmet)
116, // Clothes (Custom Glasses)
117, // Clothes (Custom Earrings)
118, // Duty Clothes
119, // Methamphetamine
120, // Cocaine
121, // Heroin
122, // Xanax
123, // Beer
124, // Vodka
125, // Whiskey


SOCKETS

-1, // None
0, // Heart
1, // Back
2, // RearPockets
3, // FrontPockets
4, // LeftWaist
5, // RightWaist
6, // BackPants
7, // FrontPants
8, // LeftAnkle
9, // RightAnkle
10, // Chest
11, // Head
12, // Clothing
13, // MAX_PLAYER
13, // Vehicle_Trunk
15, // Vehicle_Console_And_Glovebox
16 // MAX
```

## vehicledata.json

JSON formatted file which contains information about items in game.

Add documentation about it's fields here...