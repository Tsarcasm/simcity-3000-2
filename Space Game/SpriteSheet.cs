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
            Dictionary<string, int> symbols = new Dictionary<string, int>();
            String imagePath = sr.ReadLine().Trim();
            loadSymbols(sr.ReadLine().Trim());
            Texture = new Texture(Path.GetDirectoryName(path) + "/" + imagePath);
            int lineNum = 2;
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
                            getInt(parts[0]),
                            getInt(parts[1]),
                            getInt(parts[2]),
                            getInt(parts[3])));
                }
            }

            int getInt(string str)
            {
                if (!int.TryParse(str, out int result))
                {
                    return symbols[str.Trim()];
                }
                return result;
            }

            void loadSymbols(string line)
            {
                symbols = new Dictionary<string, int>();
                string[] defs = line.Split(',');
                foreach (var def in defs)
                {
                    string[] parts = def.Split('=');
                    if (parts.Length == 2)
                    {
                        symbols.Add(parts[0].Trim(), getInt(parts[1]));
                    }
                }
            }
        }
    }
}
