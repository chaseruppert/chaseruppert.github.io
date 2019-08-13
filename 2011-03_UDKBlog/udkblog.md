# UDK Blog Archive (March 2011)

The following are several unpublished blog posts from a brief experiment with Unreal Engine 3. This was a personal time project where me and some friends were attempting to create a side-scrolling brawler (ala Streets of Rage).

The blog posts demonstrate my introduction to UnrealScript and figuring out how one of the example projects works (namely, how to get the avatar to play a punch-like animation after a keyboard/mouse press).

Here are the UnrealScript assets for the project:
* [BrawlerGame.zip](BrawlerGame.zip)
* [Brawler\ UnrealScript\ Readme.docx](Brawler UnrealScript Readme.docx)

## 2011-03-16

Summary: created a new weapon class that emulates a short-range melee attack.



Success! After several days of trials and tribulations, grepping through Weapons classes and online documentation, we finally have our first swipe (pun-intended?) at a melee weapon!



It's easy for me to get caught up in "figuring things out". I want to know how things work, why they happen. This is definitely a noble cause, and, in most cases, a desirable trait (if I do say so myself!), however, sometimes you just gotta start... well... somewhere.



Enter my new melee weapon class! Originally, my candidate for the base class of the melee weapon class was the UTWeap_LinkGun class, mainly because it's alt-fire had an "instant hit" type of firing and, frankly, I hadn't played around with any of the other weapons classes.



Just for grins, I took a look at the UTWeap_RocketLauncher and UTWeap_ShockRifleBase classes (note that the implementation of these classes are actually located in the UTGameContent folder whereas the base, abstract classes reside in UTGame), and I grew fond of the shock-rifle. Why? It's primary fire was an "instant hit" type AND it dealt some physical force, which is something that would be pretty cool for a basic melee weapon.



So here's my melee weapon:



class Weapon_Melee extends UTWeap_ShockRifleBase;



Note that I didn't extend from UTWeap_ShockRifle (in UTGameContent). Normally, deriving from a base class means you have more implementation work to do (which, my comfort level in UnrealScript isn't quite there yet), but in this case, the base class is quite simple while the child class had some extra functionality that the melee weapon doesn't need yet - and, frankly, I was curious to see if there were any implementation details I needed to fill. The script compiled fine, so it appeared as if I was good to go!



One thing that you'll want to do when implementing your custom weapon is looking at the defaultproperties of the UTWeapon class (if you're deriving from UTWeapon). There are many properties that you can change in your class to help you customize your weapon to the way you want it to function.



For my melee weapon, the following properties were particularly crucial:



defaultproperties

{

 InstantHitMomentum(0)=+60000.0

 InstantHitDamage(0)=45

 FireInterval(0)=+0.77

 WeaponRange=45

 

 ShouldFireOnRelease(0)=0

 ShotCost(0)=0

 AmmoCount=20

}



Note that these properties were pretty much copied verbatim from UTWeap_ShockRifle, with the exception of the WeaponRange property, which I took a few minutes to tweak to ensure the melee characteristics of the weapon existed in some form. Also, since this is an example of a melee weapon, the ShotCost is zero to allow for "infinite ammo". When it comes time to tweak the weapon, I'll definitely need to revisit these properties (and perhaps even consider an alternate base class to derive from entirely)!



Next, we'll want to change our DefaultInventory property so that our character spawns with our new melee weapon:



class MeleeGame extends UTDeathMatch;





defaultproperties

{ 

 DefaultInventory(0)=class'Weapon_Melee'

}





Now, walking up to an NPC and dealing damage without any visual cues isn't very convincing. Why not play an animation that gives some indication that our protagonist is dealing a deadly blow?



In your custom player controller class, you can override the StartFire game-bindable action (GBA) - take a look at UDKGame\Config\DefaultInput.ini if you're not familiar with how GBAs work - and have it play an animation at the beginning of the fire sequence. Note that this is probably NOT the best way to handle this (an animation set/tree is probably more proper) - furthermore, in multiplayer matches, the animation will need to be broadcast to all other clients from the server:





exec function StartFire( optional byte FireModeNum )

{ 

 UTPawn(Pawn).FullBodyAnimSlot.PlayCustomAnim( 'hoverboardjumprtstart' , 2.0 );

 

 super.StartFire( FireModeNum );

}



Obviously, the hoverboardjumprtstart animation is NOT a melee attack animation, but it's the best I could find (when played at high speeds) to give the "feel" of a melee attack. This is why programmers need artists =)



Now that you have your custom weapon class, you can pwn NPCs all day long! Not to mention impress your friends ... ?

## 2011-03-14

Summary: further musings in investigating how to create an extremely short range "instant hit" weapon (that is, a melee attack).



I've decided to take the UDK Documentation's advice (http://udn.epicgames.com/Three/WeaponsTechnicalGuide.html) regarding reading the Weapon.uc class. It's definitely paying off and I think I'm starting to feel a bit more comfortable regarding how weapons work in UDK.



My plan of attack is to derive from the Link Gun (UTWeap_LinkGun.uc) to implement my melee weapon - this is just a first iteration of my melee attack, so any fine-tuning (and possibly derivation from a more generic parent class) isn't too much of a concern as long as the "point" comes across.



The Weapon class has a WeaponRange property that appears to be used for trajectory traces (Weapon.uc:1144) - hopefully this can be used to implement extremely tight ranges to simulate a melee attack.



The InstantFire function in Weapon.uc:1274 seems to perform all the basic functionality of firing a weapon with an "instant hit" type of firing property; the function also appears to the use the trace range property of the weapon, which means I should be able to perform the "extreme close range" hit property that I'm looking for. The FireAmmunition (Weapon.uc:1094) function calls InstantFire. Interestingly enough, UTWeapon.uc overrides InstantFire (line 2133), so I'll need to look at that to see what specific conditions/constraints the UTWeapon class imposes on instant hit property weapons.



At this point, it seems as if the calling of FireAmmunition, and subsequently, InstantFire, are functions that are triggered by the Unreal weapon system and aren't really things that need to be called or managed directly. It's even possible that declaring a new weapon class deriving directly from Weapon may work "as-is" by implementing any necessary functions and tweaking the desired properties. We'll see how straightforward this is by deriving a weapon from the UTWeap_LinkGun class!

## 2011-03-11

Summary: more work and investigation in seeing how to make a melee attack like a "weapon" that imposes a physical force and collision in the environment (esp. NPCs).



Looking at UTGame\Classes\UTWeap_LinkGun.uc ProcessBeamHit method. It seems that this calls ConsumeBeamAmmo and Victim.TakeDamage.



It looks like each Actor implements its own TakeDamage method (such as the KActor class for physical objects placed in a level).



The UTWeap_LinkGun.uc has an associated projectiles class for its projectile object, UTProj_LinkPlasma.



How does UTWeap_LinkGun's WeaponRange (UTWeap_LinkGun.uc:1243) property get used? This seems like it could be tweaked to be a very short distance to emulate a melee attack. Further investigation leads to the AttemptLinkTo function which seems to use the WeaponRange property to determine if the alternate-fire "beam" can link/attach to an enemy NPC. You might be able to use similar logic to see if an enemy NPC is within a certain range to be able to register an attack (for melee attacks).



It would be interesting to look at a UTWeapon example that isn't a projectile type, but rather an "InstantHit" type of firing. Actually, the alternate fire beam may be an "instant hit" type of ammunition property - see UTWeap_LinkGun.uc:1251-1252.

## 2011-03-08

Summary: Briefly touches upon the weapons system in UDK in attempt to find a way to detect collisions with NPCs (to implement a melee attack).



Now that I have my custom "melee" animation playing, I want to be able to "collide" with objects in the world (specifically, NPCs) when that animation is played close enough to collide with an NPC. My experience thus far has been that the only entity in the UDK that seems to be able to collide (and react) with objects in the world is weapon firing.



So, I took a look at DefaultInput.ini to see what function the game-bindable action (GBA) "fire" was mapped to - StartFire. From here, this lead me down a long path (using my pal, grep, to help me find the way) that eventually landed my in the Engines\Classes\Weapon.uc file (that is, the base Weapon class that UDKWeapon and UTWeapon derive from).



However, I quickly became puzzled with the flow of events. It seemed that I was able to simply see that an event was kicking off, for both client and server, saying that a "fire" was pending. From here it was difficult for me to find out exactly how a pending fire eventually became a true fire event.



I referred to the UDK documentation of the weapon's system and that helped explain the flow of execution much more clearly:

http://udn.epicgames.com/Three/WeaponsTechnicalGuide.html



This lead me to the Weapon::FireAmmunition function (Weapon.uc, line 1094). From here, it looks like there's three "basic" fire types: instant, projectile, and custom. I suppose InstantFire might best suit what I'm looking for (a physics reaction/collision to a melee attack).

## 2011-03-03

Summary: Animation Tree initialization callback, and more investigation into AnimNodeSlot's PlayCustomAnim function; finally got a custom animation to play!



UTPawn::PostInitAnimTree



UTPawn::OnPlayAnim



Pretty much just grep for "FullBodyAnimSlot" in UTPawn.uc for some examples on how to play custom animations on a single AnimNodeSlot (and how to "hook-up" to the AnimTree).



According to AnimNodeSlot.uc, AnimNodeSlot's appear to be primarily used with Matinee for (I'm guessing) in-game cinematics.



AnimNodeSlot::PlayCustomAnim appears to support a good set of features - animation rate, blend times, looping, and when (in the animation) to start/stop. Nice! The function is qualified with the "native" keyword - I need to look this up in the UnrealScript reference to verify what this means, but I'm guessing that this is UnrealScript's way of calling a C++ defined function since the source code for PlayCustomAnim doesn't appear to be in AnimNodeSlot.uc. Also, another function, PlayCustomAnimByDuration, could be useful to be aware of.



Instead of directly calling AnimNodeSlot::PlayCustomAnim, you may have to call UTPawn::PlayEmote in order for the animation to "broadcast" to other players (in an online setting). However, note the "e-mote mappings" found in UTFamilyInfo.uc (line 250).



You're trying to call AnimNodeSlot::PlayCustomAnim directly in BrawlerPlayerController.uc, but it seems like you have the animation name wrong because it's not being found at run-time. You had the wrong animation name written down - with the right animation name specified, the animation plays!

## 2011-03-02

Summary: this post describes a noob's investigation into UDK to try and play a custom animation from UnrealScript.



I'm a noob to UDK development, so it's taking me quite a while to parse threw all of the UnrealScript calls that are being performed with the provided "UTGame" example.



With the assistance of some online examples, I found out that UDKGame/Config/DefaultInput.ini is the source for performing mapping of HID (human interface device - keyboard, gamepad, etc.) to UnrealScript functions! From here, I attempted to see how "duck"/"crouch" works - after all, I'm trying to play an animation upon keypress, so hitting the crouch input key should play a crouch animation somewhere, right?



Well, I found a crouch event/function (need more details here), but the function call was qualified (in comments), stating that UDK "native code" calls the function. As far as I can tell, UDK "knows" about crouch/duck natively. Not really a good first path to understand how UDK animation calls work!



Perusing the DefaultInput.ini yielded a couple of keyboard inputs that mapped to "taunt" actions. I performed a grep (a simple "find in files" utility that comes with Cygwin) inside the Development/Src/UTGame/Classes directory and found the "Taunt" function that gets called within UTPlayerController:

grep -rin taunt * | grep -i function

UTPlayerController.uc:1288:exec function Taunt(int TauntIndex)



Upon further investigation, it looks like this function, in turn, calls UTPawn::PlayEmote (pardon the C++ style namespace resolution operator, I'm not sure what the proper syntax for this would be in UnrealScript). This, in turn, calls UTPawn::ServerPlayEmote which finally calls UTPawn::DoPlayEmote. It looks like the code example that I've long sought after is within DoPlayEmote!



Looking at the code in DoPlayEmote, it looks like I'll need to create an AnimNodeSlot for my custom Pawn class (or perhaps use the existing AnimNodeSlot, since my custom Pawn class currently derives from UTPawn) to be able to call PlayCustomAnim on it. Hopefully this is what I've been looking for, since, as a programmer, I understand that when code calls something, it (typically) does something. I'm still trying to understand how Animation Trees (not to mention Kismet) in UDK works (and, more importantly, how they work with UnrealScript), since Animation Trees seem to be the workhorse of animation logic for a character in UDK.

