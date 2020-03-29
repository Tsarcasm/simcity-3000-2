using SFML.System;
using System;
using System.Collections.Generic;

namespace Simcity3000_2
{
    class Map
    {
        //public int Width { get; }
        //public int Height { get; }

        //public Tile[,] Tiles;
        //public ICollection<Building> Buildings;

        //public Map(int width, int height)
        //{
        //    Width = width;
        //    Height = height;
        //    Tiles = new Tile[width, height];
        //    Buildings = new List<Building>();
        //    SetupTiles();
        //}

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
