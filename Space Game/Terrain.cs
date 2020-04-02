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

        public int MinHeight(int x, int y)
        {
            return Math.Min(
                Math.Min(Heightmap[x, y], Heightmap[x, y + 1]),
                Math.Min(Heightmap[x + 1, y + 1], Heightmap[x + 1, y]));
        }



        void UpdateQuad(int x, int y)
        {
            //Quad start index, 2d array to 1d array transformation 
            //Multiply by 4 as there are 4 vertices for each quad
            uint q = (uint)((Width - 1 - x + y * Height) * 4);

            IntRect texCoords = terrainSprites.GetSprite(GetTileName(x,y));

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
            //vertices.PrimitiveType = PrimitiveType.Lines;
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

        static readonly string[] slopes =
        {
            "flat", // 0000
            "corner_s", // 0001
            "corner_w", //0010
            "slope_s", //0011
            "corner_n", //0100
            "saddle_ns", //0101
            "slope_w", //0110
            "corner_slope_w", //0111
            "corner_e", //1000
            "slope_e", //1001
            "saddle_ew", //1010
            "corner_slope_s", //1011
            "slope_n", //1100
            "corner_slope_e", //1101
            "corner_slope_n", //1110
            "high", //1111
        };


        public string GetTileName(int x, int y)
        {
            //Get the coordinates of each vertex 
            int min = MinHeight(x, y);
            int n = Heightmap[x + 1, y] > min? 0xF : 0;
            int e = Heightmap[x + 1, y + 1] > min ? 0xF : 0;
            int s = Heightmap[x, y + 1] > min ? 0xF : 0;
            int w = Heightmap[x, y] > min ? 0xF : 0;
            // Construct a bitmask to find the appropriate slope
            string p = slopes[(0b0001 & n) | (0b0010 & e) | (0b0100 & s) | (0b1000 & w)];
            return p ;

        }




        public Vector2f[] GetTileVertices(int x, int y)
        {
            Vector2f[] coords = new Vector2f[4];
            if (InBounds(x, y))
            {
                Vector2f start = new Vector2f(y * 16 + x * 16, x * -8 + y * 8);
                const int heightMod = -10;
                int[] heights = GetTileHeights(x, y);
                start += new Vector2f(0, MinHeight(x, y) * -8);
                //coords[0] = start + new Vector2f(0, -OffsetY);
                //coords[1] = start + new Vector2f(2 * OffsetX, -OffsetY);
                //coords[2] = start + new Vector2f(2 * OffsetX, -2 * OffsetY);
                //coords[3] = start + new Vector2f(0, -2 * OffsetY);
                coords[0] = start + new Vector2f(0, 0);
                coords[1] = start + new Vector2f(32, 0);
                coords[2] = start + new Vector2f(32, 32);
                coords[3] = start + new Vector2f(0, 32);

            }
            return coords;
        }

        private bool InBounds(int x, int y) => (x >= 0 && x < Width && y >= 0 && y < Height);

    }
}