using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;

public partial class Plugin : BasePlugin, IPluginConfig<Config>
{
    public void Debug(string message)
    {
        if (Config.DebugMode)
            Logger.LogInformation("Debug - " + message);
    }

    public void Log(string message)
    {
        Logger.LogInformation("Information - " + message);
    }

    public void MessageAdmin(string message)
    {
        var admins = Utilities.GetPlayers().Where(p => AdminManager.PlayerHasPermissions(p, "@css/ban"));

        foreach (var admin in admins)
            admin.PrintToChat($" {Config.Prefix} {message}");
    }

    public void PunishPlayer(CCSPlayerController player)
    {
        string UserID = "#" + player.UserId;
        string command = string.Format(Config.PunishCommand, UserID);

        Server.ExecuteCommand(command);

        MessageAdmin($"{ChatColors.Grey}Punished player: {ChatColors.LightPurple}{player.PlayerName} {ChatColors.Grey}({player.SteamID})");
        Log($"Punished player: ({command}) - {player.PlayerName} ({player.SteamID})");
    }
}