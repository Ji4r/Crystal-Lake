namespace MyProj
{
    [System.Serializable]
    public struct RangeByte
    {
        public byte minValue;
        public byte maxValue;

        public RangeByte(byte minValue, byte maxValue)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        public byte GetDelta()
        {
            return (byte)(maxValue - minValue);
        }
    }
}
