namespace IncreasedChunkDrops.Configuration;

using Nautilus.Json;
using Nautilus.Options.Attributes;

[Menu("Increased Chunk Drop Settings")]
public class SMLConfig: ConfigFile
{
    [Slider("Extra items min", 0, 100, Step = 1)]
    public int ExtraCount = 0;

    [Slider("Extra items Max", 0, 100, Step = 1)]
    public int ExtraCountMax = 0;
}