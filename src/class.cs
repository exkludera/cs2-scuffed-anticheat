using CounterStrikeSharp.API.Modules.Utils;

public class PlayerInfo
{
    public string? Name { get; set; }
    public ulong SteamID { get; set; }
    public int DetectedCount { get; set; }
    public float PreSpeed { get; set; }
    public int TicksInAir { get; set; }
    public int LandTick { get; set; }
    public double Sync { get; set; }
    public int GoodSync { get; set; }
    public int TotalSync { get; set; }
    public List<QAngle> Rotation { get; set; } = new List<QAngle>();
}