using CounterStrikeSharp.API.Core;

public class Config : BasePluginConfig
{
    public string Prefix { get; set; } = "{red}[Scuffed-AC]{default}";
    public string PunishCommand { get; set; } = "css_ban {0} 10080 \"Banned by Scuffed Anti-Cheat\"";
    public int TickReset { get; set; } = 640;
    public int PunishDetectedCount { get; set; } = 10;
    public bool DetectStrafe { get; set; } = true;
    public bool DetectUntrustedAngles { get; set; } = true;
    public bool DetectBhop { get; set; } = true;
    public bool DebugMode { get; set; } = false;
}