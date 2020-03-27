
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
        static RenderWindow window = new RenderWindow(new VideoMode(window_width, window_height), "test", Styles.Default, new ContextSettings() { AntialiasingLevel = 8 });
        static Clock clock = new Clock();
        static double zoom = 2;
        
        
        static void Main(string[] args)
        {
            using IsoTerrain terrain = new IsoTerrain(200, 200, 32);
            ConvexShape selection = new ConvexShape(4);
            Vector2i selectedTile = new Vector2i(0,0);

            window.SetActive();
            window.SetFramerateLimit(60);
            window.Closed += (_, __) => window.Close();
            window.KeyReleased += (_, k)=> {
                if (k.Code == Keyboard.Key.Up)
                {
                    terrain.SetTileHeight(selectedTile, (int)MathF.Ceiling(terrain.GetTileHeight(selectedTile)) + 1);
                }
                if (k.Code == Keyboard.Key.Down)
                {
                    terrain.SetTileHeight(selectedTile, (int)MathF.Ceiling(terrain.GetTileHeight(selectedTile)) - 1);
                }
            };
            float deltatime;

            View camera = new View(new FloatRect(-200, -200, window_width, window_height));
            while (window.IsOpen)
            {
                deltatime = clock.Restart().AsSeconds();
                window.SetTitle($"{(int)(1 / deltatime)} fps");
                camera.Size = new Vector2f(window.Size.X, window.Size.Y);

                Vector2i mouse = Mouse.GetPosition(window);
                selectedTile = terrain.WorldCoordinateToTile(window.MapPixelToCoords(mouse));
                window.SetTitle($"Tile height: {terrain.GetTileHeight(selectedTile)}m");

                Vector2f[] selectionVertices = terrain.GetTileVertices(selectedTile.X, selectedTile.Y);
                selection.SetPoint(0, selectionVertices[0]);
                selection.SetPoint(1, selectionVertices[1]);
                selection.SetPoint(2, selectionVertices[2]);
                selection.SetPoint(3, selectionVertices[3]);
                selection.OutlineColor = Color.Red;
                selection.OutlineThickness = 1f;

                float speed = -20;

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
                if (Keyboard.IsKeyPressed(Keyboard.Key.Q))
                {
                    zoom -= 0.1;
                }
                else if (Keyboard.IsKeyPressed(Keyboard.Key.E))
                {
                    zoom += 0.1;
                }

                camera.Zoom((float)zoom);
                camera.Move(resultant);

                
                window.Clear(Color.White);
                window.SetView(camera);
                window.Draw(terrain);
                window.Draw(selection);
                window.DispatchEvents();
                window.Display();
            }
        }

        private static void Window_KeyReleased(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Up)
            {

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
