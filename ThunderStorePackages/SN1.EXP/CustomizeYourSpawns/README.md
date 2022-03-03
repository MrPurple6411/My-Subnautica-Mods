# Customize Your Spawns
Allows you to change what and where creatures/chunks/deposits/fragments are spawned in the world base on biomes.

This is a VERY powerful mod that does NOTHING on its own.

Configuration files are created in the form of Json files and are put into a subfolder called ChangesToLoad.

Any file located in this folder that ends with .json and does not contain the word Disabled in the name of the file will be loaded and the contents parsed.

The format of the contents of this file consist of a TechType  aka ItemID followed by a List of Biome/Count/Probability information.

Example :

```json
{
  "ItemID": [
    {
      "biome": "safeShallows_Wall",
      "count": 1,
      "probability": 1.0
    }
  ]
}

```

- ItemID:
    
    list of vanilla ItemIDs
    
    **[https://subnauticacommands.com/items](https://web.archive.org/web/20201025050726/https://subnauticacommands.com/items)**
    
    Currently The Cyclops will not work as it is a special system of its own and cannot be spawned this way.
    
- Biome:
    
    The biome type pulled from the biome list file or from checking the default distribution file.
    
    (DO NOT JUST TYPE SafeShallows AND EXPECT IT TO WORK CAUSE IT WONT!)
    
- Count:
    
    How many are spawned at a time when the game decides to spawn from this.
    
- Probability:
    
    This is a weighted value from 0.001 to 1000+.
    
    Values for the probability range from 0.001 to 1.0   where 0.001 is a 0.1% chance if nothing else is in the biome and 1.0  which  means that no matter what something will be spawned.   things set above 1.0  have a even higher weighing  when more then one thing has been chosen to be spawned.
    

Using this information you can do things from making Peepers spawn in the ALZ, up to having random seamoth/prawn vehicles spawn. 

### **WARNING: The Reefbacks and Leviathans from the default game are hand placed onto the map and will *NOT!* be altered by anything you do using this mod.   You can use this to add more Leviathans to the world however those default game leviathans will remain in there original locations.**