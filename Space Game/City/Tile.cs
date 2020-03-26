using SFML.System;
using System.Text;

namespace Simcity3000_2
{
    class Tile
    {
        public Tile(Vector2i position, Terrain terrain, Network network)
        {
            Position = position;
            Terrain = terrain;
            Network = network;
        }

        public Terrain Terrain { get; set; }
        public Network Network { get; set; }

        public Vector2i Position { get; set; }

        public bool HasNetwork => Network == null;
    }
}
