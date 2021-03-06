﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FirstGame
{
    public class Laser
    {
        public Texture2D sprite;
        public float scale;
        public Color color;
        public bool Active;
        public Vector2 Position;
        public int Damage;
        public float LaserMoveSpeed;
        public int ScreenWidth;

        public int Width { get { return sprite.Width; } }
        public int Height { get { return sprite.Height; } }

        public void Initialize(Texture2D texture, Vector2 position, int screenWidth, Color color, float scale)
        {
            // Keep a local copy of the values passed in
            this.color = color;
            this.ScreenWidth = screenWidth;
            this.scale = scale;
            Position = position;
            sprite = texture;

            LaserMoveSpeed = 10;
            Damage = 10;
            Active = true;
        }

        public void Update(GameTime gameTime)
        {
            if (Active == false) return;

            Position.X += LaserMoveSpeed;
            if (Position.X > ScreenWidth)
            {
                Active = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, Position, color);
        }
    }
}
