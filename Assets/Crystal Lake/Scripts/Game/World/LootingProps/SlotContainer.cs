namespace MyProj
{
    public struct SlotContainer 
    {
        public ushort IdProp;
        public bool IsFill;

        public SlotContainer(bool IsFill)
        {
            this.IsFill = IsFill;
            IdProp = ushort.MaxValue;
        }

        public SlotContainer(ushort idProp)
        {
            this.IdProp = idProp;
            this.IsFill = true;
        }

        public void SetProp(ushort prop)
        {
            IdProp = prop;
            IsFill = true;
        }

        public void RealeseProp()
        {
            IdProp = 0;
            IsFill = false;
        }
    }
}
