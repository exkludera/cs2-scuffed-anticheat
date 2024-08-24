using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Translations;
using CounterStrikeSharp.API.Modules.Cvars;
using static CounterStrikeSharp.API.Core.Listeners;

public partial class Plugin : BasePlugin, IPluginConfig<Config>
{
    public override string ModuleName => "Scuffed Anti-Cheat";
    public override string ModuleVersion => "0.0.1";
    public override string ModuleAuthor => "exkludera";

    public Dictionary<int, PlayerInfo> playerInfo = new Dictionary<int, PlayerInfo>();
    public int TickCount = 0;

    public override void Load(bool hotReload)
    {
        RegisterListener<OnTick>(OnTick);
    }

    public override void Unload(bool hotReload)
    {
        RemoveListener<OnTick>(OnTick);
    }

    public Config Config { get; set; } = new Config();
    public void OnConfigParsed(Config config)
    {
        Config = config;
        Config.Prefix = StringExtensions.ReplaceColorTags(config.Prefix);

        if (ConVar.Find("sv_autobunnyhopping")!.GetPrimitiveValue<bool>() && config.DetectBhop)
        {
            Config.DetectBhop = false;
            Log("Can not detect bhop with sv_autobunnyhopping enabled");
        }
    }
}