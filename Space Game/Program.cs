
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
        const int map_width = 300;
        const int map_height = 300;
        const float size_x = window_width / map_width;
        const float size_y = window_height / map_height;
        static RenderWindow window = new RenderWindow(new VideoMode(window_width, window_height), "test", Styles.Default, new ContextSettings() { AntialiasingLevel = 0 });
        static Clock clock = new Clock();
        static double zoom = 2;


        static void Main(string[] args)
        {
            SpriteSheet terrainSprites = new SpriteSheet("Assets/ground.tilesheet");
            using IsoTerrain terrain = new IsoTerrain(30, 30, 32, terrainSprites);
            ConvexShape selection = new ConvexShape(4);
            selection.OutlineColor = Color.Black;
            selection.OutlineThickness = 1;

            Vector2i selectedTile = new Vector2i(0, 0);
            RectangleShape house = new RectangleShape()
            {
                Size = new Vector2f(terrain.OffsetX*2, terrain.OffsetX * 2),
                Texture = new Texture("Assets/buildings.png"),
                OutlineColor = Color.Black,
                OutlineThickness = 1
            };
            window.SetActive();
            window.SetFramerateLimit(60);
            window.Closed += (_, __) => window.Close();
            window.KeyReleased += (_, k) =>
            {
                switch (k.Code)
                {
                    case Keyboard.Key.R:
                        terrain.SetTileSprite(selectedTile.X, selectedTile.Y, "road");
                        break;
                    case Keyboard.Key.Space:
                        int[] heights = terrain.GetTileHeights(selectedTile.X, selectedTile.Y);
                        int newHeight = Math.Min(Math.Min(heights[0], heights[1]), Math.Min(heights[2], heights[3]));
                        terrain.SetTileHeight(selectedTile, newHeight + 1);
                        break;
                }
            };
            window.MouseButtonReleased += (_, m) =>
            {
                Vector2f[] positions = terrain.GetTileVertices(selectedTile.X, selectedTile.Y);
                house.Position = positions[0] - new Vector2f(0, house.Size.Y - terrain.OffsetY);
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



                window.Draw(house);
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

        


    }
}
