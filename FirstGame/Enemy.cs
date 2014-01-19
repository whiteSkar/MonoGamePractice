using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FirstGame
{
    public class Enemy
    {
        public Animation EnemyAnimation;
        public Vector2 Position;
        public bool Active;
        public int Health;
        public int Damage;
        // The amount of score the enemy will give to the player
        public int Value;
        public float enemyMoveSpeed;

        public int Width { get { return EnemyAnimation.FrameWidth; } }
        public int Height { get { return EnemyAnimation.FrameHeight; } }

        public void Initialize(Animation animation, Vector2 position, float speed)
        {    
            EnemyAnimation = animation;
            Position = position;
            Active = true;
            Health = 30;
            Damage = 15;
            enemyMoveSpeed = speed;
            Value = (int)(23 * speed * speed);
        }

        public void Update(GameTime gameTime)
        {
            Position.X -= enemyMoveSpeed;
            EnemyAnimation.Position = Position;
            EnemyAnimation.Update(gameTime);

            if (Position.X < -Width || Health <= 0)
            {
                Active = false;
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            EnemyAnimation.Draw(spriteBatch);
        }
    }
}
