
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using System;

namespace Simcity3000_2
{
    class Program
    {
        const int window_width = 1000;
        const int window_height = 1000;
        const int map_width = 20;
        const int map_height = 20;
        const float size_x = window_width / map_width;
        const float size_y = window_height / map_height;
        static RenderWindow window = new RenderWindow(new VideoMode(window_width, window_height), "test");
        static Clock clock = new Clock();
        
        static void Main(string[] args)
        {
            window.SetActive();
            window.SetFramerateLimit(60);
            window.Closed += (_, __) => window.Close();
            Map map = new Map(map_width, map_height);
            using TileMap tilemap = new TileMap(map_height, map_width, tileSize: new Vector2f(size_x, size_y));
            tilemap.SetTilesheet("Assets/ground.tilesheet");
            
            Building building = new Building(new Vector2i(3, 3), Color.Red, FootprintBuilder.RectangularFootprint(2, 2));




            map.Buildings.Add(building);
            map.TilemapChanged += (t)=>UpdateMap(t, tilemap);
            float deltatime;
            map.SetupTiles();

            using Texture houseTexture = new Texture("Assets/house.png");
            using RectangleShape buildingRect = new RectangleShape
            {
                OutlineColor = Color.Black,
                Size = new Vector2f(size_x, size_y),
                Texture = houseTexture
            };
            View view = new View(new FloatRect(0,0,window_width, window_height));
            while (window.IsOpen)
            {
                Vector2i mouse = Mouse.GetPosition(window);
                view.Size = new Vector2f(window.Size.X, window.Size.Y);


                deltatime = clock.Restart().AsSeconds();
                //window.SetTitle($"{(int)(1/deltatime)} fps");
                window.SetTitle(mouse.ToString() + "   |    " + window.MapPixelToCoords(mouse));
                window.Clear();
                window.SetView(view);
                window.Draw(tilemap);
                tilemap.Rotation = 45f;
                // Draw all buildings
                foreach (Building b in map.Buildings)
                {
                    buildingRect.Position = new Vector2f(size_x * b.TopLeft.X, size_y * b.TopLeft.Y);
                    window.Draw(buildingRect);
                }
                window.DispatchEvents();
                window.Display();
            }
        }

        static void UpdateMap(Tile[,] tiles, TileMap tileMap)
        {
            using Texture texture = new Texture("Assets/house.png");
            
            for (int x = 0; x < tileMap.Width; x++)
            {
                for (int y = 0; y < tileMap.Height; y++)
                {
                    Tile tile = tiles[x, y];
                    tileMap.SetTile(x, y, tile.Terrain switch
                    {
                        Terrain.Farmland => "grass_1",
                        Terrain.Flat => "grass_2",
                        Terrain.Mountain => "grass_3",
                        Terrain.Water => "water",
                        _ => "grass_4"
                    });
                }
            }
            
        }


        static void DrawTile(int x, int y, Map map, RectangleShape tileRect)
        {
            Tile tile = map.Tiles[x, y];
            tileRect.Position = new Vector2f(size_x * tile.Position.X, size_y * tile.Position.Y);
            tileRect.FillColor = tile.Terrain switch
            {
                Terrain.Farmland => Color.Yellow,
                Terrain.Mountain => new Color(200, 200, 200),
                Terrain.Water => Color.Blue,
                Terrain.Flat => Color.Green,
                _ => Color.Magenta,
            };
            window.Draw(tileRect);
        }


    }
}
