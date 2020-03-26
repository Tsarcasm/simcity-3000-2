using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Simcity3000_2
{
    public class TileMap : Transformable, Drawable
    {
        VertexArray vertices = new VertexArray();
        Texture texture;
        Dictionary<String, IntRect> tileSprites;
        public int Width { get; protected set; }
        public int Height { get; protected set; }

        Vector2f tileSize;
        public TileMap(int boardWidth, int boardHeight, Vector2f tileSize)
        {
            tileSprites = new Dictionary<string, IntRect>();
            this.tileSize = tileSize;
            Width = boardWidth;
            Height = boardHeight;
            vertices.PrimitiveType = PrimitiveType.Quads;
            vertices.Resize((uint)(Width * Height * 4)); // Resize to the number of tiles * 4 vertices for each tile
            Clear();
        }

        public void SetTilesheet(String path)
        {
            using StreamReader sr = new StreamReader(path);
            String imagePath = sr.ReadLine().Trim();
            texture = new Texture(Path.GetDirectoryName(path) + "/" + imagePath);
            int lineNum = 1;
            while (!sr.EndOfStream)
            {
                String line = sr.ReadLine();
                String[] parts = line.Split(",");
                if (parts.Length != 5)
                {
                    Console.WriteLine($"Error parsing line {lineNum} from file {path}");
                }
                else
                {
                    tileSprites.Add(
                        parts[4].Trim(),
                        new IntRect(
                            int.Parse(parts[0]),
                            int.Parse(parts[1]),
                            int.Parse(parts[2]),
                            int.Parse(parts[3])));
                }
            }
        }

        public void Draw(RenderTarget target, RenderStates states)
        {

            // apply the transform
            states.Transform *= Transform;

            // apply the tileset texture
            states.Texture = texture;
            // draw the vertex array
            target.Draw(vertices, states);

        }

        public bool SetTile(int x, int y, String texture)
        {
            if (!tileSprites.ContainsKey(texture)) return false;
            return SetTile(x, y, tileSprites[texture]);
        }

        private bool SetTile(int x, int y, IntRect texCoords)
        {
            if (!InBounds(x, y)) return false;
            //Vector2i TileSize = new Vector2i(tTexel.Width, tTexel.Height);
            //Quad start index, 2d array to 1d array transformation 
            //Multiply by 4 as there are 4 vertices for each quad
            uint q = (uint)((x + y * Height) * 4);
            /*
              1--2
              |  |
              0--3

             */
            vertices[q + 0] =
                new Vertex(new Vector2f(x * tileSize.X, y * tileSize.Y),
                new Vector2f(texCoords.Left, texCoords.Top));

            vertices[q + 1] =
                new Vertex(new Vector2f((x + 1) * tileSize.X, y * tileSize.Y),
                new Vector2f(texCoords.Left + texCoords.Width, texCoords.Top));

            vertices[q + 2] =
                new Vertex(new Vector2f((x + 1) * tileSize.X, (y + 1) * tileSize.Y),
                new Vector2f(texCoords.Left + texCoords.Width, texCoords.Top + texCoords.Height));

            vertices[q + 3] =
                new Vertex(new Vector2f(x * tileSize.X, (y + 1) * tileSize.Y),
                new Vector2f(texCoords.Left, texCoords.Top + texCoords.Height));

            return true;
        }
        public void Clear()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    SetTile(x, y, new IntRect(new Vector2i(0,0), new Vector2i(0,0)));
                }
            }
        }

        private bool InBounds(int x, int y)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
            {
                return true;
            }
            return false;
        }
    }
}
