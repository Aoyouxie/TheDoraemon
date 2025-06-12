using System.Collections.Generic;
using UnityEngine;
using TheDoraemon.Utilities;
using Hazel;



namespace TheDoraemon.Roles.Neutrals
{
    public static class PlagueDoctor
    {
        public static PlayerControl plagueDoctor;
        public static Color color = new Color32(255, 192, 0, byte.MaxValue);

        public static Dictionary<int, PlayerControl> infected;
        public static Dictionary<int, float> progress;
        public static Dictionary<int, bool> dead;
        public static TMPro.TMP_Text statusText = null;
        public static bool triggerPlagueDoctorWin = false;

        public static PlayerControl currentTarget;
        public static int numInfections = 0;
        public static bool meetingFlag = false;

        public static float infectCooldown = 10f;
        public static int maxInfectable;
        public static float infectDistance = 1f;
        public static float infectDuration = 5f;
        public static float immunityTime = 10f;
        public static bool infectKiller = true;
        public static bool canWinDead = true;

        public static Sprite plagueDoctorIcon;

        public static Sprite getSyringeIcon()
        {
            if (plagueDoctorIcon) return plagueDoctorIcon;
            plagueDoctorIcon = Helpers.loadSpriteFromResources("TheDoraemon.Resources.InfectButton.png", 115f);
            return plagueDoctorIcon;
        }

        public static void updateDead()
        {
            if (statusText != null) Object.Destroy(statusText);
            statusText = null; // Update positions!
            foreach (var pc in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                dead[pc.PlayerId] = pc.Data.IsDead;
            }
        }

        public static string getProgressString(float progress)
        {
            // Go from green -> yellow -> red based on infection progress
            Color color;
            var prog = progress / infectDuration;
            if (prog < 0.5f)
                color = Color.Lerp(Color.green, Color.yellow, prog * 2);
            else
                color = Color.Lerp(Color.yellow, Color.red, prog * 2 - 1);

            float progPercent = prog * 100;
            return Helpers.cs(color, $"{progPercent.ToString("F1")}%");
        }

        public static void checkWinStatus()
        {
            bool winFlag = true;
            foreach (PlayerControl p in PlayerControl.AllPlayerControls)
            {
                if (p.Data.IsDead) continue;
                if (p == plagueDoctor) continue;
                if (!infected.ContainsKey(p.PlayerId))
                {
                    winFlag = false;
                    break;
                }
            }

            if (winFlag)
            {
                MessageWriter winWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PlagueDoctorWin, SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(winWriter);
                RPCProcedure.plagueDoctorWin();
            }
        }

        public static void clearAndReload()
        {
            plagueDoctor = null;
            infectCooldown = CustomOptionHolder.plagueDoctorInfectCooldown.getFloat();
            maxInfectable = Mathf.RoundToInt(CustomOptionHolder.plagueDoctorNumInfections.getFloat());
            infectDistance = CustomOptionHolder.plagueDoctorDistance.getFloat();
            infectDuration = CustomOptionHolder.plagueDoctorDuration.getFloat();
            immunityTime = CustomOptionHolder.plagueDoctorImmunityTime.getFloat();
            infectKiller = CustomOptionHolder.plagueDoctorInfectKiller.getBool();
            canWinDead = CustomOptionHolder.plagueDoctorWinDead.getBool();
            meetingFlag = false;
            triggerPlagueDoctorWin = false;
            numInfections = maxInfectable;
            currentTarget = null;
            infected = new Dictionary<int, PlayerControl>();
            progress = new Dictionary<int, float>();
            dead = new Dictionary<int, bool>();
            if (statusText != null) Object.Destroy(statusText);
            statusText = null;
        }
    }
}
    