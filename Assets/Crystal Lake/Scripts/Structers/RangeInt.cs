namespace MyProj
{
    [System.Serializable]
    public struct RangeInt
    {
        public int minValue;
        public int maxValue;

        public RangeInt(int minValue, int maxValue)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        public int GetDelta()
        {
            return maxValue - minValue;
        }
    }
}
