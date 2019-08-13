# UDK Blog Archive (March 2011)

The following are several unpublished blog posts from a brief experiment with Unreal Engine 3. This was a personal time project where me and some friends were attempting to create a side-scrolling brawler (ala Streets of Rage).

The blog posts demonstrate my introduction to UnrealScript and figuring out how one of the example projects works (namely, how to get the avatar to play a punch-like animation after a keyboard/mouse press).

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

