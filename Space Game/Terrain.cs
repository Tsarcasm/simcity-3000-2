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
        const float minorAngle = 45;

        public float OffsetX { get; private set; }
        public float OffsetY { get; private set; }

        public int[,] Heightmap { get; set; }
        public Tile[,] tiles { get; set; }

        TerrainLayer[] terrainLayers;
        public int width, height;
        float tileSize;
        public SpriteSheet terrainSprites { get; protected set; }

        public IsoTerrain(int width, int height, float tileSize, SpriteSheet terrainSprites)
        {
            this.width = width;
            this.height = height;
            this.tileSize = tileSize;
            this.terrainSprites = terrainSprites;

            OffsetX = tileSize * MathF.Cos((minorAngle / 2) * (MathF.PI / 180));
            OffsetY = tileSize * MathF.Sin((minorAngle / 2) * (MathF.PI / 180));

            terrainLayers = new TerrainLayer[15];
            for (int i = 0; i < terrainLayers.Length; i++) terrainLayers[i] = new TerrainLayer(this);
            Heightmap = new int[width + 1, height + 1];
            tiles = new Tile[width, height];
            MakeTiles();
            MakeHeightmap();
            MakeQuads();
        }

        public void SetTileSprite(int x, int y, string sprite)
        {
            if (InBounds(x, y))
            {
                tiles[x, y].Sprite = sprite;
                UpdateQuad(x, y);
            }
        }

        void MakeTiles()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    tiles[x, y] = new Tile() { Sprite = "grass" };
                }
            }
        }


        void MakeHeightmap()
        {
            for (float y = 0; y < height + 1; y++)
            {
                for (float x = 0; x < width + 1; x++)
                {
                    Heightmap[(int)x, (int)y] = (int)(noise.GetPerlinFractal(x * 1, y * 1) * 15) + 5;
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

        void UpdateQuad(int x, int y)
        {
            GetTileLayer(x, y).UpdateTile(x, y);
        }

        TerrainLayer GetTileLayer(int x, int y)
        {

            int[] heights = GetTileHeights(x, y);
            //Determine if the tile is backward facing
            if (heights[3] < heights[1])
            {
                return terrainLayers[Math.Min(Math.Min(heights[0], heights[1]), Math.Min(heights[2], heights[3]))];
            }
            else
            {
                return terrainLayers[Math.Max(Math.Max(heights[0], heights[1]), Math.Max(heights[2], heights[3]))];
            }

        }

        public void MakeQuads()
        {
            for (int y = height - 1; y >= 0; y--)
            {
                for (int x = width - 1; x >= 0; x--)
                {
                    UpdateQuad(x, y);
                }
            }
        }

        int i = 0;
        public void Draw(RenderTarget target, RenderStates states)
        {

            // apply the transform
            states.Transform *= Transform;

            // apply the tileset texture
            // draw the vertex array
            //target.Draw(terrainLayers[i], states);
            //i++;
            //if (i == terrainLayers.Length)
            //{
            //    i = 0;
            //}
            MakeQuads();
            for (int i = 0; i < terrainLayers.Length; i++)
            //for (int i = terrainLayers.Length - 1; i >= 0; i--)
            {
                using Texture texture = new Texture("Assets/heights.png", new IntRect((i % 8) * 16, 0, 16, 16));
                states.Texture = texture;
                //states.Texture = terrainSprites.Texture;
                target.Draw(terrainLayers[i], states);
            }

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

        Vector2i[] GetAdjacentTiles(int x, int y)
        {
            List<Vector2i> adj = new List<Vector2i>();
            for (int _y = y - 1; _y < y + 2; _y++)
            {
                for (int _x = x - 1; _x < x + 2; _x++)
                {
                    if (InBounds(_x, _y)) adj.Add(new Vector2i(_x, _y));
                }
            }
            return adj.ToArray();
        }

        public void SetTileHeight(Vector2i pos, int height)
        {
            if (!InBounds(pos.X, pos.Y)) return;
            int x = pos.X;
            int y = pos.Y;
            //foreach (var tile in GetAdjacentTiles(x, y))
            //{
            //    GetTileLayer(tile.X, tile.Y).RemoveTile(tile.X, tile.Y);
            //}
            Heightmap[x, y] = height;
            Heightmap[x + 1, y] = height;
            Heightmap[x, y + 1] = height;
            Heightmap[x + 1, y + 1] = height;
            foreach (var layer in terrainLayers)
            {
                layer.Clear();
            }
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

        private bool InBounds(int x, int y) => (x >= 0 && x < width && y >= 0 && y < height);

    }

    class TerrainLayer : Drawable
    {
        VertexArray vertices;
        IsoTerrain terrain;
        IDictionary<(int, int), uint> tiles;
        public TerrainLayer(IsoTerrain terrain)
        {
            this.terrain = terrain;
            tiles = new Dictionary<(int, int), uint>();
            vertices = new VertexArray();
            vertices.PrimitiveType = PrimitiveType.Quads;
        }

        public void Clear()
        {
            vertices.Resize(0);
            tiles.Clear();
        }

        void MakeQuad(int x, int y, uint q)
        {
            Vector2f start = new Vector2f(y * terrain.OffsetX + x * terrain.OffsetX, x * -terrain.OffsetY + y * terrain.OffsetY);
            IntRect texCoords = terrain.terrainSprites.GetSprite(terrain.tiles[x, y].Sprite);

            Vector2f[] tileVertices = terrain.GetTileVertices(x, y);

            vertices[q + 0] = new Vertex(tileVertices[0],
                new Vector2f(texCoords.Left, texCoords.Top));

            vertices[q + 1] =
                new Vertex(tileVertices[1],
                new Vector2f(texCoords.Left + texCoords.Width, texCoords.Top));

            vertices[q + 2] =
                new Vertex(tileVertices[3],
                new Vector2f(texCoords.Left, texCoords.Top + texCoords.Height));

            vertices[q + 3] =
               new Vertex(tileVertices[1],
               new Vector2f(texCoords.Left + texCoords.Width, texCoords.Top));

            vertices[q + 4] =
                new Vertex(tileVertices[2],
                new Vector2f(texCoords.Left + texCoords.Width, texCoords.Top + texCoords.Height));

            vertices[q + 5] =
                new Vertex(tileVertices[3],
                new Vector2f(texCoords.Left, texCoords.Top + texCoords.Height));


        }
        public void UpdateTile(int x, int y)
        {
            if (tiles.TryGetValue((x, y), out uint q))
            {
                MakeQuad(x, y, q);
            }
            else
            {
                AddTile(x, y);
            }
        }
        private void AddTile(int x, int y)
        {
            uint q = vertices.VertexCount;
            vertices.Resize(q + 6);
            MakeQuad(x, y, q);
            tiles.Add((x, y), q);
        }

        public void RemoveTile(int x, int y)
        {
            //vertices.Resize(vertices.VertexCount - 4);
            //tiles.Clear();
            uint q = tiles[(x, y)];
            tiles.Remove((x, y));

            vertices[q + 0] = new Vertex(new Vector2f(0, 0));
            vertices[q + 1] = new Vertex(new Vector2f(0, 0));
            vertices[q + 2] = new Vertex(new Vector2f(0, 0));
            vertices[q + 3] = new Vertex(new Vector2f(0, 0));
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            vertices.PrimitiveType = PrimitiveType.Triangles;
            target.Draw(vertices, states);
            //vertices.PrimitiveType = PrimitiveType.Lines;
            //target.Draw(vertices);
        }
    }



    class Tile
    {
        public string Sprite { get; set; }
    }
}
