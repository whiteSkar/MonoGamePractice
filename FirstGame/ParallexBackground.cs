using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace FirstGame
{
    public class ParallexBackground
    {
        private Texture2D _backGroundTexture;
        private Vector2[] _tilePositions;
        private int _leftMostTile;
        private int _rightMostTile;
        private int _speed;
        private int _screenWidth;
        private int _screenHeight;

        public void Initialize(ContentManager content, String texturePath, int screenWidth, int screenHeight, int speed)
        {
            _screenHeight = screenHeight;
            _screenWidth = screenWidth;
            _backGroundTexture = content.Load<Texture2D>(texturePath);
            _speed = speed;
            _tilePositions = new Vector2[screenWidth / _backGroundTexture.Width + 2];
            _leftMostTile = 0;
            _rightMostTile = _tilePositions.Length - 1;
            
            for (int i = 0; i < _tilePositions.Length; i++)
            {
                _tilePositions[i] = new Vector2(_backGroundTexture.Width * i, 0);
            }
        }

        public void Update(GameTime gametime)
        {
            var leftMostTileX = _tilePositions[_leftMostTile].X + _speed;
            var rightMostTileX = _tilePositions[_rightMostTile].X + _speed;

            for (int i = 0; i < _tilePositions.Length; i++)
            {
                _tilePositions[i].X += _speed;
                
                if (_speed <= 0)
                {
                    if (_tilePositions[i].X <= -_backGroundTexture.Width)
                    {
                        _tilePositions[i].X = rightMostTileX + _backGroundTexture.Width;
                        _rightMostTile = i;
                    }
                }
                else
                {
                    if (_tilePositions[i].X >= _screenWidth)
                    {
                        _tilePositions[i].X = leftMostTileX - _backGroundTexture.Width;
                        _leftMostTile = i;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < _tilePositions.Length; i++)
            {
                Rectangle rectBg = new Rectangle((int)_tilePositions[i].X, (int)_tilePositions[i].Y, _backGroundTexture.Width, _screenHeight);
                spriteBatch.Draw(_backGroundTexture, rectBg, Color.White);
            }
        }
    }
}
