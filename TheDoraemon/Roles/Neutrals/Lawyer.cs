//using UnityEngine;



//namespace TheDoraemon.Roles.Neutrals
//{
//    public static class Lawyer {
//        public static PlayerControl lawyer;
//        public static PlayerControl target;
//        public static Color color = new Color32(134, 153, 25, byte.MaxValue);
//        public static Sprite targetSprite;
//        //public static bool triggerProsecutorWin = false;
//        //public static bool isProsecutor = false;
//        public static bool targetKnows = true;
//        public static bool triggerLawyerWin = false;
//        public static int meetings = 0;

//        public static bool winsAfterMeetings = false;
//        public static int neededMeetings = 4;
//        public static float vision = 1f;
//        public static bool lawyerTargetKnows = true;
//        public static bool lawyerKnowsRole = false;
//        public static bool targetCanBeJester = false;
//        public static bool targetWasGuessed = false;

//        public static Sprite getTargetSprite() {
//            if (targetSprite) return targetSprite;
//            targetSprite = Helpers.loadSpriteFromResources("", 150f);
//            return targetSprite;
//        }

//        public static void clearAndReload(bool clearTarget = true) {
//            lawyer = null;
//            if (clearTarget) {
//                target = null;
//                targetWasGuessed = false;
//            }
//            triggerLawyerWin = false;
//            meetings = 0;
//            //isProsecutor = false;
//            //triggerProsecutorWin = false;
//            vision = CustomOptionHolder.lawyerVision.getFloat();
//            lawyerKnowsRole = CustomOptionHolder.lawyerKnowsRole.getBool();
//            lawyerTargetKnows = CustomOptionHolder.lawyerTargetKnows.getBool();
//            targetCanBeJester = CustomOptionHolder.lawyerTargetCanBeJester.getBool();
//            winsAfterMeetings = CustomOptionHolder.lawyerWinsAfterMeetings.getBool();
//            neededMeetings = Mathf.RoundToInt(CustomOptionHolder.lawyerNeededMeetings.getFloat());
//            targetKnows = CustomOptionHolder.lawyerTargetKnows.getBool();
//        }
//    }
//}
    