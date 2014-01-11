using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FirstGame
{
    public class Explosion
    {
        public Animation ExplosionAnimation;
        public Vector2 Position;
        public bool Active;

        public int Width { get { return ExplosionAnimation.FrameWidth; } }
        public int Height { get { return ExplosionAnimation.FrameHeight; } }

        public void Initialize(Animation animation, Vector2 position)
        {
            ExplosionAnimation = animation;
            Position = position;
            Active = true;
        }

        public void Update(GameTime gameTime)
        {
            ExplosionAnimation.Position = Position;
            ExplosionAnimation.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            ExplosionAnimation.Draw(spriteBatch);
        }
    }
}
