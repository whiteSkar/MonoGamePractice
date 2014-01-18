using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FirstGame
{
    public class Player
    {
        const int DefaultHealth = 100;

        // Animation representing the player
        //public Texture2D PlayerTexture;
        public Animation PlayerAnimation;

        // Position of the Player relative to the upper left side of the screen
        public Vector2 Position;

        public HealthBar HealthBar;

        // State of the player
        public bool Active;

        // Amount of hit points that player has
        public int Health;

        // Get the width of the player ship
        public int Width
        {
            get { return PlayerAnimation.FrameWidth; }
        }

        // Get the height of the player ship
        public int Height
        {
            get { return PlayerAnimation.FrameHeight; }
        }

        public void Reset(Vector2 position)
        {
            Position = position;
            Active = true;
            Health = DefaultHealth;
        }

        public void Initialize(GraphicsDevice graphicsDevice, Animation animation, Vector2 position, Texture2D healthBarTexture)
        {
            PlayerAnimation = animation;
            Position = position;
            Active = true;
            Health = DefaultHealth;
            HealthBar = new HealthBar(healthBarTexture, new Vector2(Position.X + Width / 2, Position.Y + Height + 10), Width, Health);
        }

        public void Update(GameTime gameTime)
        {
            PlayerAnimation.Active = Active;
            PlayerAnimation.Position = Position;
            PlayerAnimation.Update(gameTime);

            HealthBar.IsActive = Active;
            HealthBar.Update(gameTime, new Vector2(Position.X + Width / 2, Position.Y + Height + 10), Health);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            PlayerAnimation.Draw(spriteBatch);
            HealthBar.Draw(spriteBatch);
        }
    }
}
