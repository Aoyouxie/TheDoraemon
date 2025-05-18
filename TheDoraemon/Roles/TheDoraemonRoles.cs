using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheDoraemon.Roles
{
    public static class TheDoraemonRoles
    {

        [HarmonyPatch]
        public static class TheDoraemon
        {
            public static System.Random rnd = new((int)DateTime.Now.Ticks);

            public static void clearAndReloadRoles()
            {
                //以下职业使用了TheDoraemonGMIA代码
                //时空管理者
                TimeSpaceManager.clearAndReloadRoles();
            }
        }
    }
}
