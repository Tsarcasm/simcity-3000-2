using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simcity3000_2
{
    class IsoTerrain : Transformable, Drawable
    {
        readonly Random rng = new Random(1);
        readonly FastNoise noise = new FastNoise();
        const float minorAngle = 26.565f;

        public float OffsetY;
        public float OffsetX;

        public int[,] Heightmap { get; set; }
        Tile[,] Tiles { get; set; }
        readonly VertexArray vertices = new VertexArray();
        public int Width, Height;
        readonly float tileSize;
        Spritesheet terrainSprites;
        public IsoTerrain(int width, int height, float tileSize, Spritesheet terrainSprites, Tile[,] tiles)
        {
            this.Width = width;
            this.Height = height;
            this.tileSize = tileSize;
            this.terrainSprites = terrainSprites;
            OffsetY = tileSize * MathF.Sin(minorAngle * (MathF.PI / 180));
            OffsetX = tileSize * MathF.Cos(minorAngle * (MathF.PI / 180));
            Heightmap = new int[width + 1, height + 1];
            this.Tiles = tiles;
            MakeHeightmap();
            MakeQuads();
        }

        public void SetTileSprite(int x, int y, string sprite)
        {
            if (InBounds(x, y))
            {
                Tiles[x, y].GroundSprite = sprite;
                UpdateQuad(x, y);
            }
        }


        void MakeHeightmap()
        {
            for (float y = 0; y < Height + 1; y++)
            {
                for (float x = 0; x < Width + 1; x++)
                {
                    Heightmap[(int)x, (int)y] = (int)(noise.GetPerlinFractal(x * 1, y * 1) * -25);
                }
            }
        }

        public int[] GetTileHeights(int x, int y)
        {
            return new int[]
            {
                Heightmap[x, y],
                 Heightmap[x, y+1],
                Heightmap[x+1, y+1],
                 Heightmap[x+1, y]
            };
        }

        public int MaxHeight(int x, int y)
        {
            return Math.Max(
                Math.Max(Heightmap[x, y], Heightmap[x, y + 1]),
                Math.Max(Heightmap[x + 1, y + 1], Heightmap[x + 1, y]));
        }



        void UpdateQuad(int x, int y)
        {
            //Quad start index, 2d array to 1d array transformation 
            //Multiply by 4 as there are 4 vertices for each quad
            uint q = (uint)((Width - 1 - x + y * Height) * 4);
            /*
              1--2
              |  |
              0--3

             */
            Vector2f start = new Vector2f(y * OffsetX + x * OffsetX, x * -OffsetY + y * OffsetY);

            IntRect texCoords = terrainSprites.GetSprite(Tiles[x, y].GroundSprite);

            Vector2f[] tileVertices = GetTileVertices(x, y);

            vertices[q + 0] = new Vertex(tileVertices[0],
                new Vector2f(texCoords.Left, texCoords.Top));

            vertices[q + 1] =
                new Vertex(tileVertices[1],
                new Vector2f(texCoords.Left + texCoords.Width, texCoords.Top));

            vertices[q + 2] =
                new Vertex(tileVertices[2],
                new Vector2f(texCoords.Left + texCoords.Width, texCoords.Top + texCoords.Height));

            vertices[q + 3] =
                new Vertex(tileVertices[3],
                new Vector2f(texCoords.Left, texCoords.Top + texCoords.Height));


        }

        public void MakeQuads()
        {
            vertices.PrimitiveType = PrimitiveType.Quads;
            vertices.Resize((uint)(Width * Height * 4)); // Resize to the number of tiles * 4 vertices for each tile

            for (int y = Width - 1; y >= 0; y--)
            {
                for (int x = Height - 1; x >= 0; x--)
                {
                    UpdateQuad(x, y);
                }
            }
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            //MakeQuads();

            // apply the transform
            states.Transform *= Transform;

            // apply the tileset texture
            states.Texture = terrainSprites.Texture;
            // draw the vertex array
            vertices.PrimitiveType = PrimitiveType.Quads;
            target.Draw(vertices, states);
            //target.Draw(vertices, states);

        }

        const int searchRadius = 10;
        public Vector2i WorldCoordinateToTile(Vector2f coords)
        {
            float x = -MathF.Floor((coords.Y / OffsetY) - (coords.X / OffsetX)) / 2;
            float y = MathF.Floor((coords.Y / OffsetY) + (coords.X / OffsetX)) / 2;

            for (int _x = Math.Max((int)x - searchRadius, 0); _x < Math.Min(Width, x + searchRadius); _x++)
            {
                for (int _y = Math.Max((int)y - searchRadius, 0); _y < Math.Min(Width, y + searchRadius); _y++)
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
                const int heightMod = -10;
                int[] heights = GetTileHeights(x, y);
                coords[0] = start + new Vector2f(0, heights[0] * heightMod);
                coords[1] = start + new Vector2f(OffsetX, OffsetY + heights[1] * heightMod);
                coords[2] = start + new Vector2f(OffsetX * 2, 0 + heights[2] * heightMod);
                coords[3] = start + new Vector2f(OffsetX, -OffsetY + heights[3] * heightMod);
            }
            return coords;
        }

        private bool InBounds(int x, int y) => (x >= 0 && x < Width && y >= 0 && y < Height);

    }
}