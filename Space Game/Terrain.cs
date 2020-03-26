using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simcity3000_2
{
    class IsoTerrain : Transformable, Drawable
    {
        Random rng = new Random();
        const float minorAngle = 40;
        const float majorAngle = 180 - 30;

        float OffsetY => tileSize * MathF.Sin((minorAngle / 2) * (MathF.PI / 180));
        float OffsetX => tileSize * MathF.Cos((minorAngle / 2) * (MathF.PI / 180));

        VertexArray vertices = new VertexArray();
        int[,] grid;
        int width, height;
        float tileSize;
        Texture texture;
        public IsoTerrain(int width, int height, float tileSize)
        {
            grid = new int[width, height];
            this.width = width;
            this.height = height;
            this.tileSize = tileSize;
            texture = new Texture("Assets/ground.png");
            GenerateHeight();
            MakeVertices();
        }

        public void GenerateHeight()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    grid[x, y] = rng.Next(0, 10) > 8 ? 2 : 0;
                }
            }
        }

        public void MakeVertices()
        {
            vertices.PrimitiveType = PrimitiveType.Quads;
            vertices.Resize((uint)(width * height * 4 * 3)); // Resize to the number of tiles * 4 vertices for each tile

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    //Quad start index, 2d array to 1d array transformation 
                    //Multiply by 4 as there are 4 vertices for each quad
                    uint q = (uint)((x + y * height) * 4);
                    /*
                      1--2
                      |  |
                      0--3

                     */

                    Vector2f start = new Vector2f(y * OffsetX + x * OffsetX, x * -OffsetY + y * OffsetY);
                    start += new Vector2f(0, grid[x, y] * -10);
                    IntRect texCoords = rng.Next(0, 4) switch
                    {
                        0 => new IntRect(16, 16, 16, 16),
                        1 => new IntRect(0, 16, 16, 16),
                        2 => new IntRect(16, 0, 16, 16),
                        _ => new IntRect(0, 0, 16, 16)
                    };

                    MakeQuad(
                        q, texCoords,
                        start,
                        start + new Vector2f(OffsetX, OffsetY),
                        start + new Vector2f(OffsetX * 2, 0),
                        start + new Vector2f(OffsetX, -OffsetY)
                        );

                    vertices[q + 0] = new Vertex(start,
                        new Vector2f(texCoords.Left, texCoords.Top));

                    vertices[q + 1] =
                        new Vertex(start + new Vector2f(OffsetX, OffsetY),
                        new Vector2f(texCoords.Left + texCoords.Width, texCoords.Top));

                    vertices[q + 2] =
                        new Vertex(start + new Vector2f(OffsetX * 2, 0),
                        new Vector2f(texCoords.Left + texCoords.Width, texCoords.Top + texCoords.Height));

                    vertices[q + 3] =
                        new Vertex(start + new Vector2f(OffsetX, -OffsetY),
                        new Vector2f(texCoords.Left, texCoords.Top + texCoords.Height));
                }
            }
        }

        void MakeQuad(uint idx, IntRect texCoords, Vector2f a, Vector2f b, Vector2f c, Vector2f d)
        {
            vertices[idx + 0] = new Vertex(a,
                        new Vector2f(texCoords.Left, texCoords.Top));

            vertices[idx + 1] =
                new Vertex(b,
                new Vector2f(texCoords.Left + texCoords.Width, texCoords.Top));

            vertices[idx + 2] =
                new Vertex(c,
                new Vector2f(texCoords.Left + texCoords.Width, texCoords.Top + texCoords.Height));

            vertices[idx + 3] =
                new Vertex(d,
                new Vector2f(texCoords.Left, texCoords.Top + texCoords.Height));
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

    }
}
