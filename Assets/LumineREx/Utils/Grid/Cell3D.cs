namespace LumineREx.Utils.Grid
{
    public struct Cell3D
    {
        public int X;
        public int Y;
        public int Z;
        public int Value;

        public Cell3D(int x, int y, int z, int value)
        {
            X = x;
            Y = y;
            Z = z;
            Value = value;
        }
    }
}