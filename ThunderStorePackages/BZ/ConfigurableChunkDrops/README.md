# Configurable Chunk Drops
This mod allows to configure What drops from the different chunks in the game.

After running the game and loading into a world for the first time this mod will make 2 files.

1. A config.json file which is where you add things to change what spawns when you break a chunk.
2. A Defaults file which will show the default values for all the different chunks.

Subnautica's Default file will look something like this:

```json
{
    "Breakables":{
        "LimestoneChunk":{
            "Copper":"0.5"
        },
        "SandstoneChunk":{
            "Gold":"0.25",
            "Silver":"0.5"
        },
        "BasaltChunk":{
            "Titanium":"0.2",
            "Diamond":"0.15",
            "Lithium":"0.2"
        },
        "ShaleChunk":{
            "Gold":"0.3",
            "Lithium":"0.45"
        },
        "ObsidianChunk":{
            
        }
    }
}
```

this can be broken down to be used in the config file like this: 

```json
{
    "Breakables":{
        "LimestoneChunk":{
            "Copper":"1.0"
        }
    }
}
```

That will make it so Copper will be the ONLY thing that drops from Limestone Chunks as the copper now has a 100% chance to spawn.

if instead you did this:

```json
{
    "Breakables":{
        "LimestoneChunk":{
            
        }
    }
}
```

it will now ONLY spawn the Default material for that chunk which is titanium. For some reason setting thing to 0.0 does not guarantee that they wont spawn and so you have to remove them for them to no longer show up at all.