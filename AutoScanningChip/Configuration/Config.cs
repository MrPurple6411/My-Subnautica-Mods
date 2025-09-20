using Nautilus.Json;
using Nautilus.Options.Attributes;
using Nautilus.Handlers;
using AutoScanningChip;

namespace AutoScanningChip.Configuration;

[Menu("Auto-Scanning Chip")] // Title in Mods tab
public class ASCSettings : ConfigFile
{
    // Speed multiplier vs stock scan time (1.0 = stock).
    [Slider("Scan Speed Multiplier", 0.1f, 5f, DefaultValue = 1f, Step = 0.05f, Format = "{0:F2}x", Tooltip = "Scaling applied to time to scan. Lower = slower, higher = faster."), OnChange(nameof(applySpeedChange))]
    public float scanSpeedMultiplier = 1f;

    // Base trigger radius with one chip equipped (meters).
    [Slider("Base Radius (m)", 1f, 25f, DefaultValue = 5f, Step = 0.5f, Format = "{0:F1}m"), OnChange(nameof(applyRadiusChange))]
    public float baseRadius = 5f;

    // Additional radius scale per equipped chip.
    [Slider("Per-Chip Radius Scale", 0.1f, 5f, DefaultValue = 1f, Step = 0.1f, Format = "{0:F1}x"), OnChange(nameof(applyRadiusChange))]
    public float perChipScale = 1f;

    // Pulse interval in seconds for automatic area scan.
    [Slider("Pulse Interval (s)", 1f, 60f, DefaultValue = 10f, Step = 1f, Format = "{0:F0}s", Tooltip = "How often to pulse-scan around the player."), OnChange(nameof(applyPulseIntervalChange))]
    public float pulseIntervalSeconds = 10f;

    // Register this config with Nautilus and expose a singleton for convenience.
    public static ASCSettings Instance { get; } = OptionsPanelHandler.RegisterModOptions<ASCSettings>();

    private static void applySpeedChange()
    {
        // Nothing needed immediately; new scans will pick up the value.
    }

    private static void applyRadiusChange()
    {
        // Kick an immediate pulse to apply changes quickly
        Main.Instance?.ScheduleImmediatePulse();
    }

    private static void applyPulseIntervalChange()
    {
        // Kick an immediate pulse to apply changes quickly
        Main.Instance?.ScheduleImmediatePulse();
    }
}
