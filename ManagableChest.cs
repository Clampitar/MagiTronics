namespace MagiTronics
{
    internal abstract class ManagableChest
    {

        public abstract bool WaterOut(out int liquidType);

        public abstract bool WaterIn(int liquidType);
    }
}
