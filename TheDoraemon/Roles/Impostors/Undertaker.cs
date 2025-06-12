using System.Linq;
using UnityEngine;
using Hazel;



namespace TheDoraemon.Roles.Impostors
{
    public static class Undertaker
    {
        public static PlayerControl undertaker;
        public static Color color = Palette.ImpostorRed;

        public static DeadBody DraggedBody;
        public static DeadBody TargetBody;
        public static bool CanDropBody;

        public static float speedDecrease = -50f;
        public static bool disableVent = true;

        public static Sprite dragButtonSprite;
        public static Sprite dropButtonSprite;

        public static void RpcDropBody(Vector3 position)
        {
            if (undertaker == null) return;
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UndertakerDropBody, SendOption.Reliable, -1);
            writer.Write(position.x);
            writer.Write(position.y);
            writer.Write(position.z);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            DropBody(position);
        }

        public static void RpcDragBody(byte playerId)
        {
            if (undertaker == null) return;
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UndertakerDragBody, SendOption.Reliable, -1);
            writer.Write(playerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            DragBody(playerId);
        }

        public static void DropBody(Vector3 position)
        {
            if (!DraggedBody) return;
            DraggedBody.transform.position = position;
            DraggedBody = null;
            TargetBody = null;
        }

        public static void DragBody(byte playerId)
        {
            if (undertaker == null) return;
            var body = Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == playerId);
            if (body == null) return;
            DraggedBody = body;
        }

        public static Sprite getDragButtonSprite()
        {
            if (dragButtonSprite) return dragButtonSprite;
            dragButtonSprite = Helpers.loadSpriteFromResources("TheDoraemon.Resources.DragButton.png", 115f);
            return dragButtonSprite;
        }

        public static Sprite getDropButtonSprite()
        {
            if (dropButtonSprite) return dropButtonSprite;
            dropButtonSprite = Helpers.loadSpriteFromResources("TheDoraemon.Resources.DropButton.png", 115f);
            return dropButtonSprite;
        }

        public static void clearAndReload()
        {
            undertaker = null;
            DraggedBody = null;
            TargetBody = null;

            speedDecrease = CustomOptionHolder.undertakerSpeedDecrease.getFloat();
            disableVent = CustomOptionHolder.undertakerDisableVent.getBool();
        }
    }
}
    