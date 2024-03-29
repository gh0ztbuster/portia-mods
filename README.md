﻿# portia-mods
My Time at Portia mods (compatible with [Unity Mod Manager](https://www.nexusmods.com/site/mods/21/))

Downloads can be found on the [release](https://github.com/1bakedpotato/portia-mods/releases) tab.

# Goodbye
Consider this repo abandoned and ripe for deletion at some random point in time.

## GiveItem (version 0.3.0) ([changelog](https://github.com/1bakedpotato/portia-mods/blob/master/mods/GiveItem/CHANGELOG.md))
Gives the Player any item (s)he desires.  
NOTE: For now, this mod utilizes UnityModManager's UI. Sorry, it was a quick-and-dirty attempt. Perhaps this will be improved in a future version of the mod...

### Usage
1. Load into your saved game
1. Open the Unity Mod Manager UI (default: **Left Control + F10**)
1. Click the settings icon next to **Give Item**
1. Search for an item (all searches are case **in**​sensitive)
    * Listing all items is supported
      * Enter `*` or leave the search term blank
    * Exact matches are supported
      * Enter `apple` to search for an item with the exact name of `apple`
    * Wildcards are supported
      * Enter `wood*` to search for items beginning with `wood`
      * Enter `*sofa` for items ending with `sofa`
      * Enter `*iron*` for words containing `iron` anywhere
    * Item IDs are supported
      * Prepend the integer item ID with two # characters
        * e.g. "##1234" searches for an item with ID 1234
1. Adjust the quantity as desired
    * Max is 100, but the item button can be clicked multiple times without closing the UI
    * Use the slider or directly enter the number in the text field next to it
1. Scroll (using bar on the right or mousewheel) the item list to find the desired item
1. Click the item name; each click will add the quantity selected above
1. Close the Unity Mod Manager UI

Item notification(s) appear in the lower-left and the desired item(s) is/are added to your inventory. This utilizes the game's default item pickup behavior - hot bar first, bag for overflow, and left on the ground for any amount that can't be carried.
