namespace LumineREx.Utils.Grid
{
    public struct Cell2D
    {
        public int X;
        public int Y;   

        public float Value;
        

        public Cell2D(int x, int y, float value)
        {
            X = x;
            Y = y;
            Value = value;
        }

        public Cell2D(int x, int y)
        {
            X = x;
            Y = y;
            Value = 0;
        }
    }
}