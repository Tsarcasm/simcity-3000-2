using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;

namespace Simcity3000_2
{
    class Map : Drawable
    {
        public int Width { get; }
        public int Height { get; }

        public Tile[,] Tiles;
        public ICollection<Structure> Structures;

        IsoTerrain terrain;
        public Map(int width, int height, Spritesheet terrainSprites)
        {
            Width = width;
            Height = height;
            Tiles = new Tile[width, height];
            SetupTiles();
            terrain = new IsoTerrain(width, height, 19, terrainSprites, Tiles);

        }

        void SetupTiles()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Tiles[x, y] = new Tile() { GroundSprite = "grass" };
                }
            }
        }


        public void Update(float deltatime)
        {

        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            void DrawTile(int x, int y)
            {
                Tile tile = Tiles[x, y];
                // First draw the building

                if (tile.Structure?.TopLeft == new Vector2i(x,y))
                {
                    
                }
            }

            target.Draw(terrain);
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    DrawTile(x, y);
                }
            }

            
        }

        

        //public void SetupTiles()
        //{
        //    Random randgen = new Random();
        //    for (int x = 0; x < Width; x++)
        //    {
        //        for (int y = 0; y < Height; y++)
        //        {
        //            Terrain terrain = randgen.Next(0, 4) switch
        //            {
        //                0 => Terrain.Farmland,
        //                1 => Terrain.Water,
        //                2 => Terrain.Mountain,
        //                _ => Terrain.Flat,
        //            };
        //            Network network = randgen.Next(0, 5) switch
        //            {
        //                0 => new Road(),
        //                _ => null
        //            };

        //            Tiles[x, y] = new Tile(new Vector2i(x, y), terrain, network);
        //        }
        //    }
        //    TilemapChanged?.Invoke(Tiles);
        //}


        //public Action<Tile[,]> TilemapChanged { set;  get; }
        //public Action<Building> BuildingChanged { set; get; }

        //public Tile[,] TilesUnderBuilding(Building building)
        //{
        //    Vector2i size = building.Size;
        //    Tile[,] tiles = new Tile[size.X, size.Y];
        //    for (int y = 0; y < size.Y; y++)
        //    {
        //        for (int x = 0; x < size.X; x++)
        //        {
        //            if (building.Footprint[x, y])
        //                tiles[x, y] = this.Tiles[x + building.TopLeft.X, y + building.TopLeft.Y];
        //        }
        //    }
        //    return tiles;
        //}
    }
}
