using HarmonyLib;
using Hazel;
using TheDoraemon.Options;

namespace TheDoraemon;

public enum CustomRPC
{
    SyncCustomSettings
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
public static class HandleRpc
{
    public static void Postfix([HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader)
    {
        switch ((CustomRPC)callId)
        {
            case CustomRPC.SyncCustomSettings:
                Rpc.ReceiveRpc(reader, reader.BytesRemaining > 8);
                break;
        }
    }
}