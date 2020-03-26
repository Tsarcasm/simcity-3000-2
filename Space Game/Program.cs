
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
            float deltatime;

            using IsoTerrain terrain = new IsoTerrain(30, 30, 50);
            View camera = new View(new FloatRect(-200, -200, window_width, window_height));
            while (window.IsOpen)
            {
                deltatime = clock.Restart().AsSeconds();
                window.SetTitle($"{(int)(1 / deltatime)} fps");

                Vector2i mouse = Mouse.GetPosition(window);
                camera.Size = new Vector2f(window.Size.X, window.Size.Y);

                float speed = -16;

                Vector2f viewCenter = camera.Center;
                Vector2f halfExtents = camera.Size / 2.0f;
                Vector2f cameraTopLeft = viewCenter - halfExtents;

                Vector2f resultant = new Vector2f();
                if (Keyboard.IsKeyPressed(Keyboard.Key.W))
                {
                    if (camera.Center.Y > 0)
                        resultant += new Vector2f(0, speed);
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.S))
                {
                    resultant += new Vector2f(0, -speed);
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.A))
                {
                    if (camera.Center.X > 0)
                        resultant += new Vector2f(+speed, 0);
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.D))
                {
                    resultant += new Vector2f(-speed, 0);
                }


                camera.Move(resultant);


                window.Clear(Color.White);
                window.SetView(camera);
                window.Draw(terrain);

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
                        Terrain.Farmland => "farm",
                        Terrain.Flat => "grass",
                        Terrain.Mountain => "hill",
                        Terrain.Water => "water",
                        _ => "grass"
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
