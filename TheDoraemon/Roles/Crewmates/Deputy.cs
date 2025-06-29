using System;
using System.Collections.Generic;
using UnityEngine;
using Hazel;



namespace TheDoraemon.Roles.Crewmates
{
        public static class Deputy
        {
            public static PlayerControl deputy;
            public static Color color = TimespaceManager.color;

            public static PlayerControl currentTarget;
            public static List<byte> handcuffedPlayers = new();
            public static int promotesToSheriff; // No: 0, Immediately: 1, After Meeting: 2
            public static bool keepsHandcuffsOnPromotion;
            public static float handcuffDuration;
            public static float remainingHandcuffs;
            public static float handcuffCooldown;
            public static bool knowsSheriff;
            public static bool stopsGameEnd;
            public static Dictionary<byte, float> handcuffedKnows = new();

            private static Sprite buttonSprite;
            private static Sprite handcuffedSprite;
            
            public static Sprite getButtonSprite()
            {
                if (buttonSprite) return buttonSprite;
                buttonSprite = Helpers.loadSpriteFromResources("namespace TheDoraemon.Resources.DeputyHandcuffButton.png", 115f);
                return buttonSprite;
            }

            public static Sprite getHandcuffedButtonSprite()
            {
                if (handcuffedSprite) return handcuffedSprite;
                handcuffedSprite = Helpers.loadSpriteFromResources("namespace TheDoraemon.Resources.DeputyHandcuffed.png", 115f);
                return handcuffedSprite;
            }

            // Can be used to enable / disable the handcuff effect on the target's buttons
            public static void setHandcuffedKnows(bool active = true, byte playerId = byte.MaxValue)
            {
                if (playerId == byte.MaxValue)
                    playerId = PlayerControl.LocalPlayer.PlayerId;

                if (active && playerId == PlayerControl.LocalPlayer.PlayerId) {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ShareGhostInfo, SendOption.Reliable, -1);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write((byte)RPCProcedure.GhostInfoTypes.HandcuffNoticed);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }

                if (active) {
                    handcuffedKnows.Add(playerId, handcuffDuration);
                    handcuffedPlayers.RemoveAll(x => x == playerId);
               }

                if (playerId == PlayerControl.LocalPlayer.PlayerId) {
                    HudManagerStartPatch.setAllButtonsHandcuffedStatus(active);
                    SoundEffectsManager.play("deputyHandcuff");
		}
 
	    }

            public static void clearAndReload()
            {
                deputy = null;
                currentTarget = null;
                handcuffedPlayers = new List<byte>();
                handcuffedKnows = new Dictionary<byte, float>();
                HudManagerStartPatch.setAllButtonsHandcuffedStatus(false, true);
                promotesToSheriff = CustomOptionHolder.deputyGetsPromoted.getSelection();
                remainingHandcuffs = CustomOptionHolder.deputyNumberOfHandcuffs.getFloat();
                handcuffCooldown = CustomOptionHolder.deputyHandcuffCooldown.getFloat();
                keepsHandcuffsOnPromotion = CustomOptionHolder.deputyKeepsHandcuffs.getBool();
                handcuffDuration = CustomOptionHolder.deputyHandcuffDuration.getFloat();
                knowsSheriff = CustomOptionHolder.deputyKnowsSheriff.getBool();
                stopsGameEnd = CustomOptionHolder.deputyStopsGameEnd.getBool();
            }
        }
    }

  

    