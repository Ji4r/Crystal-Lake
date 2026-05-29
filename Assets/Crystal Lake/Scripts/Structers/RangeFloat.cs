namespace MyProj
{
    [System.Serializable]
    public struct RangeFloat 
    {
        public float minValue;
        public float maxValue;

        public RangeFloat(float minValue, float maxValue)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        public float GetDelta()
        {
            return maxValue - minValue;
        }
    }
}
