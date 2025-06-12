using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TheDoraemon.Objects;
using TheDoraemon.Patches;
using static TheDoraemon.TheDoraemon;
namespace TheDoraemon.Roles.Crewmates
    {
        public static class Archaeologist
        {
            static public PlayerControl archaeologist;
            public static Color color = new(71f / 255f, 93f / 255f, 206f / 255f);

            public enum RevealAntique
            {
                Never,
                Immediately,
                AfterMeeting
            }

            public static Arrow arrow = new(color);
            public static Antique target;
            public static Antique antiqueTarget;
            public static List<Antique> revealed = new();
            public static int numCandidates = 3;

            private static Sprite excavateSprite;
            public static Sprite getExcavateSprite()
            {
                if (excavateSprite) return excavateSprite;
                excavateSprite = Helpers.loadSpriteFromResources("namespace TheDoraemon.Resources.ExcavateButton.png", 115f);
                return excavateSprite;
            }

            private static Sprite detectSprite;
            public static Sprite getDetectSprite()
            {
                if (detectSprite) return detectSprite;
                detectSprite = Helpers.loadSpriteFromResources("namespace TheDoraemon.Resources.ArchaeologistDetectButton.png", 115f);
                return detectSprite;
            }
            public static bool arrowActive;
            public static float cooldown;
            /// <summary>
            /// The time the Archaeologist has to wait when detecting the antique
            /// </summary>
            public static float detectDuration;
            /// <summary>
            /// The duration of the discovery arrow
            /// </summary>
            public static float arrowDuration;
            public static RevealAntique revealAntique;

            public static bool hasRemainingAntique()
            {
                if (archaeologist == null || Antique.antiques == null || Antique.antiques.Count == 0) return false;
                var remainingList = Antique.antiques.Where(x => !x.isBroken).ToList();
                return remainingList.Count > 0;
            }

            public static (string, RoleInfo) getRoleInfo()
            {
                var list = new List<PlayerControl>(PlayerControl.AllPlayerControls.ToArray().ToList());
                list.Shuffle();
                while (list.Count > numCandidates) list.Remove(list.LastOrDefault());
                var role = RoleInfo.getRoleInfoForPlayer(list[rnd.Next(list.Count)], false, true).FirstOrDefault();
                string playerNames = string.Join(",", list.Select(p => p.Data.PlayerName));
                return (playerNames, role);
            }

            public static void clearAndReload()
            {
                archaeologist = null;
                arrowActive = false;
                target = null;
                antiqueTarget = null;
                cooldown = CustomOptionHolder.archaeologistCooldown.getFloat();
                detectDuration = CustomOptionHolder.archaeologistExploreDuration.getFloat();
                arrowDuration = CustomOptionHolder.archaeologistArrowDuration.getFloat();
                numCandidates = Mathf.RoundToInt(CustomOptionHolder.archaeologistNumCandidates.getFloat());
                revealAntique = (RevealAntique)CustomOptionHolder.archaeologistRevealAntiqueMode.getSelection();
                revealed = new();
                if (arrow?.arrow != null) Object.Destroy(arrow.arrow);
                arrow = new Arrow(color);
                if (arrow.arrow != null) arrow.arrow.SetActive(false);
            }
        }
    }
