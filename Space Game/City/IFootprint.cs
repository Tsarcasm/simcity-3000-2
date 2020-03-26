namespace Simcity3000_2
{
    interface IFootprint
    {
        public bool[,] Footprint { get; }
    }

    static class FootprintBuilder
    {
        public static bool[,] RectangularFootprint(int width, int height)
        {
            bool[,] footprint = new bool[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    footprint[x, y] = true;
                }
            }
            return footprint;
        }
    }
}
