using System;
using System.Collections.Generic;
using UnityEngine;
using TheDoraemon.Utilities;
using Hazel;
using UnityEngine.Events;
using UnityEngine.UI;
using TheDoraemon.Roles.Neutrals;



namespace TheDoraemon.Roles.Neutrals
{
    public static class SchrodingersCat
    {
        public static Color color = Color.grey;
        public static PlayerControl schrodingersCat;
        public static PlayerControl formerSchrodingersCat;
        public static Team team;

        public static PoolablePlayer playerTemplate;
        public static GameObject parent;
        public static List<PoolablePlayer> teams;
        public static bool shownMenu = false;
        public static PlayerControl currentTarget;

        public static bool isExiled = false;

        public enum Team
        {
            None,
            Impostor,
            Crewmate,
            Jackal,
            JekyllAndHyde,
            Moriarty
        }

        public static bool isTeamJackalAlive()
        {
            foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (p == Jackal.jackal && !p.Data.IsDead)
                {
                    return true;
                }
                else if (p == Sidekick.sidekick && !p.Data.IsDead)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool isLastImpostor()
        {
            foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (PlayerControl.LocalPlayer != p && p.Data.Role.IsImpostor && !p.Data.IsDead) return false;
            }
            return true;
        }

        public static void setImpostorFlag()
        {
            RoleInfo.schrodingersCat.color = Palette.ImpostorRed;
            RoleInfo.schrodingersCat.isNeutral = false;
            team = Team.Impostor;
        }

        public static void setJackalFlag()
        {
            RoleInfo.schrodingersCat.color = Jackal.color;
            team = Team.Jackal;
        }

        public static void setJekyllAndHydeFlag()
        {
            RoleInfo.schrodingersCat.color = JekyllAndHyde.color;
            team = Team.JekyllAndHyde;
        }

        public static void setMoriartyFlag()
        {
            RoleInfo.schrodingersCat.color = Moriarty.color;
            team = Team.Moriarty;
        }

        public static void setCrewFlag()
        {
            RoleInfo.schrodingersCat.color = Color.white;
            RoleInfo.schrodingersCat.isNeutral = false;
            team = Team.Crewmate;
        }

        public static bool tasksComplete(PlayerControl p)
        {
            int counter = 0;
            var option = GameOptionsManager.Instance.currentNormalGameOptions;
            int totalTasks = option.NumLongTasks + option.NumShortTasks + option.NumCommonTasks;
            if (totalTasks == 0) return true;
            foreach (var task in p.Data.Tasks)
            {
                if (task.Complete)
                {
                    counter++;
                }
            }
            return counter == totalTasks;
        }

        public static float killCooldown;
        public static bool becomesImpostor;
        public static bool cantKillUntilLastOne;
        public static bool justDieOnKilledByCrew;
        public static bool hideRole;
        public static bool canChooseImpostor;

        public static bool hasTeam() => team != Team.None;

        public static void showMenu()
        {
            if (!shownMenu)
            {
                if (teams.Count == 0)
                {
                    var colorBG = Helpers.loadSpriteFromResources("TheDoraemon.Resources.White.png", 100f);
                    var hudManager = FastDestroyableSingleton<HudManager>.Instance;
                    parent = new GameObject("PoolableParent");
                    parent.transform.parent = hudManager.transform;
                    parent.transform.localPosition = new Vector3(0, 0, 0);
                    var impostor = createPoolable(parent, "impostor", 0, (UnityAction)((Action)(() =>
                    {
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SchrodingersCatSetTeam, SendOption.Reliable, -1);
                        writer.Write((byte)Team.Impostor);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.schrodingersCatSetTeam((byte)Team.Impostor);
                        showMenu();
                    })));
                    teams.Add(impostor);
                    if (Jackal.jackal != null || Sidekick.sidekick != null)
                    {
                        var jackal = createPoolable(parent, "jackal", 1, (UnityAction)((Action)(() =>
                        {
                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SchrodingersCatSetTeam, SendOption.Reliable, -1);
                            writer.Write((byte)Team.Jackal);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            RPCProcedure.schrodingersCatSetTeam((byte)Team.Jackal);
                            showMenu();
                        })));
                        teams.Add(jackal);
                    }
                    if (Moriarty.moriarty != null)
                    {
                        var moriarty = createPoolable(parent, "moriarty", 2, (UnityAction)((Action)(() =>
                        {
                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SchrodingersCatSetTeam, SendOption.Reliable, -1);
                            writer.Write((byte)Team.Moriarty);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            RPCProcedure.schrodingersCatSetTeam((byte)Team.Moriarty);
                            showMenu();
                        })));
                        teams.Add(moriarty);
                    }
                    if (JekyllAndHyde.jekyllAndHyde != null)
                    {
                        var jekyllAndHyde = createPoolable(parent, "jekyllAndHyde", 6, (UnityAction)((Action)(() =>
                        {
                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SchrodingersCatSetTeam, SendOption.Reliable, -1);
                            writer.Write((byte)Team.JekyllAndHyde);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            RPCProcedure.schrodingersCatSetTeam((byte)Team.JekyllAndHyde);
                            showMenu();
                        })));
                        teams.Add(jekyllAndHyde);
                    }
                    var crewmate = createPoolable(parent, "crewmate", 10, (UnityAction)((Action)(() =>
                    {
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SchrodingersCatSetTeam, SendOption.Reliable, -1);
                        writer.Write((byte)Team.Crewmate);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.schrodingersCatSetTeam((byte)Team.Crewmate);
                        showMenu();
                    })));
                    teams.Add(crewmate);
                    layoutPoolable();
                }
                else
                {
                    teams.ForEach(x =>
                    {
                        x.gameObject.SetActive(true);
                    });
                    layoutPoolable();
                }
            }
            else
            {
                teams.ForEach(x =>
                {
                    x.gameObject.SetActive(false);
                });
            }
            shownMenu = !shownMenu;
        }

        public static bool isJackalButtonEnable()
        {
            if (team == Team.Jackal && PlayerControl.LocalPlayer == schrodingersCat && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                if (!isTeamJackalAlive() || !cantKillUntilLastOne)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool isJekyllAndHydeButtonEnable()
        {
            if (team == Team.JekyllAndHyde && PlayerControl.LocalPlayer == schrodingersCat && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                if (JekyllAndHyde.jekyllAndHyde == null || JekyllAndHyde.jekyllAndHyde.Data.IsDead || !cantKillUntilLastOne)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool isMoriartyButtonEnable()
        {
            if (team == Team.Moriarty && PlayerControl.LocalPlayer == schrodingersCat && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                if (Moriarty.moriarty == null || Moriarty.moriarty.Data.IsDead || !cantKillUntilLastOne)
                {
                    return true;
                }
            }
            return false;
        }

        private static PoolablePlayer createPoolable(GameObject parent, string name, int color, UnityAction func)
        {
            var poolable = UnityEngine.Object.Instantiate(playerTemplate, parent.transform);
            var actionButton = UnityEngine.Object.Instantiate(FastDestroyableSingleton<HudManager>.Instance.KillButton, poolable.gameObject.transform);
            SpriteRenderer spriteRenderer = actionButton.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = null;
            actionButton.transform.localPosition = new Vector3(0, 0, 0);
            actionButton.gameObject.SetActive(true);
            actionButton.gameObject.ForEachChild((Il2CppSystem.Action<GameObject>)((c) => { if (c.name.Equals("HotKeyGuide")) UnityEngine.Object.Destroy(c); }));
            PassiveButton button = actionButton.GetComponent<PassiveButton>();
            button.OnClick = new Button.ButtonClickedEvent();
            button.OnClick.AddListener((UnityAction)func);
            var texts = actionButton.GetComponentsInChildren<TMPro.TextMeshPro>();
            texts.ForEach(x => x.gameObject.SetActive(false));
            poolable.gameObject.SetActive(true);
            poolable.SetBodyColor(color);
            poolable.SetName(ModTranslation.getString(name));
            return poolable;
        }

        public static void ForEach<T>(this IList<T> self, Action<T> todo)
        {
            for (int i = 0; i < self.Count; i++)
            {
                todo(self[i]);
            }
        }

        public static void layoutPoolable()
        {
            float offset = 2f;
            int center = teams.Count / 2;
            for (int i = 0; i < teams.Count; i++)
            {
                float x = teams.Count % 2 != 0 ? (offset * (i - center)) : (offset * (i - center)) + (offset * 0.5f);
                teams[i].transform.localPosition = new Vector3(x, 0, 0);
                teams[i].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                teams[i].GetComponentInChildren<ActionButton>().transform.position = teams[i].transform.position;
            }
        }

        public static void clearAndReload()
        {
            schrodingersCat = null;
            formerSchrodingersCat = null;
            currentTarget = null;
            team = Team.None;
            canChooseImpostor = CustomOptionHolder.schrodingersCatHideRole.getBool() && CustomOptionHolder.schrodingersCatCanChooseImpostor.getBool();
            hideRole = CustomOptionHolder.schrodingersCatHideRole.getBool();
            justDieOnKilledByCrew = CustomOptionHolder.schrodingersCatJustDieOnKilledByCrew.getBool();
            cantKillUntilLastOne = CustomOptionHolder.schrodingersCatCantKillUntilLastOne.getBool();
            becomesImpostor = CustomOptionHolder.schrodingersCatBecomesImpostor.getBool();
            killCooldown = CustomOptionHolder.schrodingersCatKillCooldown.getFloat();
            RoleInfo.schrodingersCat.color = color;
            RoleInfo.schrodingersCat.isNeutral = true;
            shownMenu = false;
            if (teams != null) teams.ForEach(x => x.gameObject.SetActive(false));
            teams = new List<PoolablePlayer>();
            isExiled = false;
        }
    }
}
    