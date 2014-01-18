using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FirstGame
{
    public class HealthBar
    {
        public const int DefaultHeight = 10;
        private int _width;

        public bool IsActive;
        private int _maxHealth;

        private int _borderPixel = 2;
        private Rectangle _healthBarRectangle;
        private Rectangle _borderRectangle;
        private Texture2D _pixelTexture;

        public HealthBar(Texture2D pixelTexture, Vector2 centerPos, int width, int maxHealth)
        {
            _width = width;
            int barX = (int)centerPos.X - _width / 2;
            int barY = (int)centerPos.Y - DefaultHeight / 2;
            _borderRectangle = new Rectangle(barX, barY, _width, DefaultHeight);
            _healthBarRectangle = _borderRectangle;

            _pixelTexture = pixelTexture;
            IsActive = true;
            _maxHealth = maxHealth;
        }

        public void Initialize(GraphicsDevice graphicsDevice, Vector2 centerPos)
        {

        }

        public void Update(GameTime gameTime, Vector2 newCenterPos, int currentHealth)
        {
            if (!IsActive) return;

            float healthRatio = (float)currentHealth / _maxHealth;
            int barX = (int)newCenterPos.X - _width / 2;
            int barY = (int)newCenterPos.Y - DefaultHeight / 2;
            _healthBarRectangle = new Rectangle(barX, barY, (int)(_width * healthRatio), DefaultHeight);
            _borderRectangle = new Rectangle(barX, barY, _width, DefaultHeight);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
            {
                spriteBatch.Draw(_pixelTexture, _healthBarRectangle, Color.LawnGreen);
                DrawRectangleBorder(spriteBatch, _pixelTexture, _borderRectangle, Color.White);
            }
        }

        private void DrawRectangleBorder(SpriteBatch spriteBatch, Texture2D borderTexture, Rectangle rectangle, Color color)
        {
            spriteBatch.Draw(borderTexture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, _borderPixel), color);
            spriteBatch.Draw(borderTexture, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height, rectangle.Width + _borderPixel, _borderPixel), color);
            spriteBatch.Draw(borderTexture, new Rectangle(rectangle.X, rectangle.Y, _borderPixel, rectangle.Height), color);
            spriteBatch.Draw(borderTexture, new Rectangle(rectangle.X + rectangle.Width, rectangle.Y, _borderPixel, rectangle.Height), color);
        }
    }
}
