﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Scumm.Engine.Resources.Graphics
{
    public struct ScummChar
    {
        public byte width, height;
        public byte offX, offY;
        public Texture2D pic;
    }

    public class Charset : Resource
    {
        ScummChar[] chars = new ScummChar[256];

        public ScummChar[] Chars
        {
            get { return chars; }
            set { chars = value; }
        }

        public Charset(string resourceId)
            : base(resourceId)
        {
            
        }

        public int GetTextWidth(string text)
        {
            int sum = 0;
            for (int i = 0; i < text.Length && text[i] != 0; ++i)
            {
                ScummChar chr = chars[text[i]];
                if (chr.height == 0)
                    continue;
                sum += chr.width * 2;
            }
            return sum;
        }

        public void DrawText(SpriteBatch spriteBatch, string text, Vector2 position)
        {
            int positionX = (int)position.X*2;
            for (int i = 0; i < text.Length && text[i] != 0; ++i)
            {
                ScummChar chr = chars[text[i]];
                if (chr.height == 0)
                    continue;
                spriteBatch.Draw(chr.pic, new Rectangle((int)(positionX + chr.offX), (int)(position.Y + chr.offY) * 2 - chr.height * 2, chr.width * 2, chr.height * 2), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                positionX += chr.width * 2;
            }
        }
    }
}
