namespace TheDoraemon.Roles
{
    public class Color32 : TimeSpaceManagerColor
    {
        private int v1;
        private int v2;
        private int v3;
        private byte maxValue;

        public Color32(int v1, int v2, int v3, byte maxValue)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
            this.maxValue = maxValue;
        }
    }
}