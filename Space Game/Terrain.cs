using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simcity3000_2
{
    class IsoTerrain : Transformable, Drawable
    {
        Random rng = new Random(1);
        FastNoise noise = new FastNoise();
        const float minorAngle = 40;

        public float OffsetY => tileSize * MathF.Sin((minorAngle / 2) * (MathF.PI / 180));
        public float OffsetX => tileSize * MathF.Cos((minorAngle / 2) * (MathF.PI / 180));

        public int[,] Heightmap { get; set; }

        VertexArray vertices = new VertexArray();
        int width, height;
        float tileSize;
        Texture texture;
        public IsoTerrain(int width, int height, float tileSize)
        {
            this.width = width;
            this.height = height;
            this.tileSize = tileSize;
            texture = new Texture("Assets/ground.png");
            MakeHeightmap();
            MakeQuads();
        }


        public void MakeHeightmap()
        {
            //total vertices = (width + 1) * (height + 1)
            Heightmap = new int[width + 1, height + 1];
            //return;
            for (float y = 0; y < height + 1; y++)
            {
                for (float x = 0; x < width + 1; x++)
                {
                    Heightmap[(int)x, (int)y] = (int)(noise.GetPerlinFractal(x * 1, y * 1) * -25);
                }
            }
        }

        Vector2f QuadVertexHeight(int x, int y, int v)
        {
            return new Vector2f(0, -10 * v switch
            {
                0 => Heightmap[x, y],
                1 => Heightmap[x, y + 1],
                2 => Heightmap[x + 1, y + 1],
                3 => Heightmap[x + 1, y],
                _ => 0
            });
        }

        public void MakeQuads()
        {
            vertices.PrimitiveType = PrimitiveType.Lines;
            vertices.Resize((uint)(width * height * 4 * 2)); // Resize to the number of tiles * 4 vertices for each tile

            for (int y = width - 1; y >= 0; y--)
            {
                for (int x = height - 1; x >= 0; x--)
                {
                    //Quad start index, 2d array to 1d array transformation 
                    //Multiply by 4 as there are 4 vertices for each quad
                    uint q = (uint)((width - 1 - x + y * height) * 4 * 2);
                    /*
                      1--2
                      |  |
                      0--3

                     */

                    Vector2f start = new Vector2f(y * OffsetX + x * OffsetX, x * -OffsetY + y * OffsetY);
                    IntRect texCoords = rng.Next(4, 20) switch
                    {
                        0 => new IntRect(16, 16, 16, 16),
                        1 => new IntRect(0, 16, 16, 16),
                        2 => new IntRect(16, 0, 16, 16),
                        _ => new IntRect(0, 0, 16, 16)
                    };

                    Vector2f[] tileVertices = GetTileVertices(x, y);

                    vertices[q + 0] = new Vertex(tileVertices[0], Color.Black,
                    new Vector2f(texCoords.Left, texCoords.Top));

                    vertices[q + 1] =
                        new Vertex(tileVertices[1], Color.Black,
                        new Vector2f(texCoords.Left + texCoords.Width, texCoords.Top));

                    vertices[q + 2] =
                        new Vertex(tileVertices[2], Color.Black,
                    new Vector2f(texCoords.Left + texCoords.Width, texCoords.Top + texCoords.Height));

                    vertices[q + 3] =
                        new Vertex(tileVertices[3], Color.Black,
                    new Vector2f(texCoords.Left, texCoords.Top + texCoords.Height));

                    vertices[q + 4] =
                            new Vertex(tileVertices[1], Color.Black,
                    new Vector2f(texCoords.Left + texCoords.Width, texCoords.Top));

                    vertices[q + 5] =
                        new Vertex(tileVertices[2], Color.Black,
                    new Vector2f(texCoords.Left + texCoords.Width, texCoords.Top + texCoords.Height));

                    vertices[q + 6] =
                            new Vertex(tileVertices[3], Color.Black,
                   new Vector2f(texCoords.Left + texCoords.Width, texCoords.Top));

                    vertices[q + 7] =
                        new Vertex(tileVertices[0], Color.Black,
                    new Vector2f(texCoords.Left + texCoords.Width, texCoords.Top + texCoords.Height));




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

        const int searchRadius = 10;
        public Vector2i WorldCoordinateToTile(Vector2f coords)
        {
            float x = -MathF.Floor((coords.Y / OffsetY) - (coords.X / OffsetX)) / 2;
            float y = MathF.Floor((coords.Y / OffsetY) + (coords.X / OffsetX)) / 2;

            for (int _x = Math.Max((int)x - searchRadius, 0); _x < Math.Min(width, x + searchRadius); _x++)
            {
                for (int _y = Math.Max((int)y - searchRadius, 0); _y < Math.Min(width, y + searchRadius); _y++)
                {
                    if (MathHelper.PointInPolygon(GetTileVertices(_x, _y), coords))
                    {
                        return new Vector2i(_x, _y);
                    }
                }

            }
            return (Vector2i)new Vector2f(x, y);



            //float X = (coords.X - OffsetX / 2) / (OffsetX * 1);
            //float Y = (coords.Y - OffsetY / 2) / (OffsetY * 1);
        }
        public float GetTileHeight(int x, int y)
        {
            if (!InBounds(x, y)) return -1;
            return (
                Heightmap[x, y] +
                Heightmap[x + 1, y] +
                Heightmap[x, y + 1] +
                Heightmap[x + 1, y + 1]
                ) / 4;
        }
        public float GetTileHeight(Vector2i pos)
        {
            return GetTileHeight(pos.X, pos.Y);
        }

        public void SetTileHeight(Vector2i pos, int height)
        {
            if (!InBounds(pos.X, pos.Y)) return;
            int x = pos.X;
            int y = pos.Y;
            Heightmap[x, y] = height;
            Heightmap[x + 1, y] = height;
            Heightmap[x, y + 1] = height;
            Heightmap[x + 1, y + 1] = height;
            MakeQuads();
        }
        public Vector2f[] GetTileVertices(int x, int y)
        {
            Vector2f[] coords = new Vector2f[4];
            if (InBounds(x, y))
            {
                Vector2f start = new Vector2f(y * OffsetX + x * OffsetX, x * -OffsetY + y * OffsetY);

                coords[0] = start + QuadVertexHeight(x, y, 0);
                coords[1] = start + new Vector2f(OffsetX, OffsetY) + QuadVertexHeight(x, y, 1);
                coords[2] = start + new Vector2f(OffsetX * 2, 0) + QuadVertexHeight(x, y, 2);
                coords[3] = start + new Vector2f(OffsetX, -OffsetY) + QuadVertexHeight(x, y, 3);
            }
            return coords;
        }

        private bool InBounds(int x, int y) => (x >= 0 && x < width && y >= 0 && y < height);
    }
}
