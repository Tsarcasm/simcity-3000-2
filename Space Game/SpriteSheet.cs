using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Simcity3000_2
{
    public class Spritesheet
    {
        public Texture Texture { get; protected set; }
        readonly Dictionary<String, IntRect> sprites;
        public Spritesheet(string path)
        {
            sprites = new Dictionary<string, IntRect>();
            LoadSpritesheet(path);
        }

        public IntRect GetSprite(string name)
        {
            if (sprites.ContainsKey(name))
            {
                return sprites[name];
            }
            else
            {
                return new IntRect(0, 0, 0, 0);
            }
        }
        public void LoadSpritesheet(string path)
        {
            using StreamReader sr = new StreamReader(path);
            String imagePath = sr.ReadLine().Trim();
            Texture = new Texture(Path.GetDirectoryName(path) + "/" + imagePath);
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
                    sprites.Add(
                        parts[4].Trim(),
                        new IntRect(
                            int.Parse(parts[0]),
                            int.Parse(parts[1]),
                            int.Parse(parts[2]),
                            int.Parse(parts[3])));
                }
            }
        }
    }
}
