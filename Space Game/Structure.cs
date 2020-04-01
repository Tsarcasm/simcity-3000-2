using SFML.Graphics;
using SFML.System;

namespace Simcity3000_2
{
    class Structure : Entity, IFootprint
    {
        public Vector2i Size { get; }
        public Vector2i TopLeft { get; }
        public Color Color { get; }

        public bool[,] Footprint { get; }

        public Structure(Vector2i topLeft, Color color, bool[,] footprint) { 
            this.TopLeft = topLeft;
            this.Color = color;
            this.Footprint = footprint;
        }

    }
}
