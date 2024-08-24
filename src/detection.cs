using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

public partial class Plugin : BasePlugin, IPluginConfig<Config>
{
    public void OnTick()
    {
        TickCount++;

        if (TickCount == Config.TickReset)
        {
            playerInfo.Clear();
            TickCount = 0;
            Debug("Tick timer reset! Cleared player info");
        }

        foreach (var player in Utilities.GetPlayers())
        {
            if (!player.IsValid || !player.PawnIsAlive || player.IsBot)
                return;

            if (!playerInfo.ContainsKey(player.Slot))
                playerInfo[player.Slot] = new PlayerInfo();

            if (playerInfo.TryGetValue(player.Slot, out PlayerInfo? pInfo))
            {
                if (Config.DetectUntrustedAngles)
                {
                    QAngle eyeAngles = player.PlayerPawn.Value!.EyeAngles;
                    float pitch = eyeAngles.X;
                    float yaw = eyeAngles.Y;
                    float roll = eyeAngles.Z;

                    if (pitch < -89f || pitch > 89f || yaw < -180f || yaw > 180f || roll < -50f || roll > 50f)
                        CheatDetected(player.Slot, 2);
                }

                if (player.MoveType == MoveType_t.MOVETYPE_NOCLIP ||
                    player.MoveType == MoveType_t.MOVETYPE_PUSH ||
                    player.MoveType == MoveType_t.MOVETYPE_LADDER)
                    return;

                var playerButtons = player.Buttons;
                float playerSpeed = player.PlayerPawn!.Value!.AbsVelocity.Length2D();

                pInfo.Name = player.PlayerName;
                pInfo.SteamID = player.SteamID;

                bool OnGround = ((PlayerFlags)player.Pawn.Value!.Flags & PlayerFlags.FL_ONGROUND) == PlayerFlags.FL_ONGROUND;

                if (!OnGround)
                {
                    pInfo.TicksInAir++;

                    if (pInfo.TicksInAir == 1)
                    {
                        pInfo.LandTick = -1;
                        pInfo.PreSpeed = playerSpeed;
                    }

                    if (Config.DetectStrafe)
                    {
                        OnSyncTick(player, playerButtons, player.PlayerPawn.Value!.EyeAngles);

                        if (TickCount == Config.TickReset -1 && pInfo.Sync >= 99 && pInfo.GoodSync >= Config.TickReset / 2)
                            CheatDetected(player.Slot, 1);
                    }
                }
                else
                {
                    pInfo.TicksInAir = 0;

                    if (pInfo.LandTick == -1)
                        pInfo.LandTick = TickCount;

                    if (Config.DetectBhop)
                    {
                        if (playerButtons.HasFlag(PlayerButtons.Jump) && pInfo.LandTick == TickCount)
                            CheatDetected(player.Slot, 3);
                    }
                }
            }
        }
    }

    public void CheatDetected(int slot, int type)
    {
        if (playerInfo.TryGetValue(slot, out PlayerInfo? pInfo))
        {
            pInfo.DetectedCount++;

            string reason = "unknown";

            switch (type)
            {
                case 1:
                    reason = "Strafe";
                    pInfo.DetectedCount = Config.PunishDetectedCount;
                    break;
                case 2:
                    reason = "Untrusted Angels";
                    break;
                case 3:
                    reason = "Bhop";
                    break;
            }

            MessageAdmin($"{ChatColors.Grey}Detected {ChatColors.White}{reason} {ChatColors.Grey}on {ChatColors.LightPurple}{pInfo.Name} {ChatColors.Grey}({pInfo.SteamID})");

            if (Config.PunishDetectedCount == pInfo.DetectedCount)
            {
                PunishPlayer(Utilities.GetPlayerFromSlot(slot)!);

                playerInfo.Remove(slot);
            }
        }
    }

    // straight from sharptimer :D
    public void OnSyncTick(CCSPlayerController player, PlayerButtons? buttons, QAngle eyeangle)
    {
        try
        {
            var pInfo = playerInfo[player.Slot];
            bool left = false;
            bool right = false;
#pragma warning disable CS0219
            bool leftRight = false;
#pragma warning restore CS0219

            if ((buttons & PlayerButtons.Moveleft) != 0 && (buttons & PlayerButtons.Moveright) != 0)
                leftRight = true;
            else if ((buttons & PlayerButtons.Moveleft) != 0)
                left = true;
            else if ((buttons & PlayerButtons.Moveright) != 0)
                right = true;
            else return;

            QAngle newEyeAngle = new QAngle(eyeangle.X, eyeangle.Y, eyeangle.Z);
            pInfo.Rotation.Add(newEyeAngle);
            pInfo.TotalSync++;

            //Check left goodsync
            if (pInfo.Rotation != null && pInfo.Rotation.Count > 1 && eyeangle.Y > pInfo.Rotation[pInfo.TotalSync - 2].Y && left)
                pInfo.GoodSync++;

            //Check right goodsync
            if (pInfo.Rotation != null && pInfo.Rotation.Count > 1 && eyeangle.Y < pInfo.Rotation[pInfo.TotalSync - 2].Y && right)
                pInfo.GoodSync++;

            pInfo.Sync = Math.Round((float)pInfo.GoodSync / pInfo.TotalSync * 100, 0);

            Debug($"{player.PlayerName} - Sync: {pInfo.Sync}%, GoodSync: {pInfo.GoodSync}, TotalSync: {pInfo.TotalSync}");
        }
        catch (Exception ex)
        {
            Log($"Exception in OnSyncTick: {ex}");
        }
    }
}