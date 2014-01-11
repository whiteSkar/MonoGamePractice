using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using FirstGame;
using System.Collections.Generic;
using System;

namespace FirstGame
{
    public enum GameState
    {
        Main,
        Game,
        GameOver
    }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class FirstGame : Game
    {
        public GameState gameState;
        public const float gameScale = 1f;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Player _player;

        private KeyboardState _currentKeyboardState;
        private KeyboardState _previousKeyboardState;
        private GamePadState _currentGamePadState;
        private GamePadState _previousGamePadState;
        private MouseState _currentMouseState;
        private MouseState _previousMouseState;

        private Vector2 _defaultPlayerPosition;
        private float _playerMoveSpeed;

        private Texture2D _mainMenuTexture;
        private Texture2D _gameOverTexutre;
        private Texture2D _bottomBackgroundTexture;
        private Rectangle _wholeScreenRectangle;
        private ParallexBackground _midBackgroundParrallex;
        private ParallexBackground _topBackgroundParallex;

        private Texture2D _enemyTexture;
        private List<Enemy> _enemies;
        private TimeSpan _enemySpawnTime;
        private TimeSpan _previousSpawnTime;

        private Texture2D _laserTexture;
        private List<Laser> _lasers;
        private TimeSpan _minimumLaserSpawnIntervalTime;
        private TimeSpan _previousLaserSpawnTime;

        private Texture2D _explosionTexture;
        private List<Explosion> _explosions;

        private TimeSpan _timeDead;
        private TimeSpan _intervalBetweenDeadAndGameOver;

        private Song _gameMusic;
        private Song _mainMenuMusic;
        private SoundEffect _explosionSound;
        private SoundEffect _laserSound;

        private SpriteFont _spriteFont;

        private Random _random;

        public FirstGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            gameState = GameState.Main;
            _player = new Player();

            _midBackgroundParrallex = new ParallexBackground();
            _topBackgroundParallex = new ParallexBackground();
            _wholeScreenRectangle = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            _playerMoveSpeed = 8.0f;

            _enemies = new List<Enemy>();
            _lasers = new List<Laser>();
            _explosions = new List<Explosion>();

            _previousSpawnTime = TimeSpan.Zero;
            _previousLaserSpawnTime = TimeSpan.Zero;

            _enemySpawnTime = TimeSpan.FromSeconds(0.7f);
            _minimumLaserSpawnIntervalTime = TimeSpan.FromSeconds(0.2f);
            _intervalBetweenDeadAndGameOver = TimeSpan.FromSeconds(1.0f);

            _random = new Random();

            TouchPanel.EnabledGestures = GestureType.FreeDrag;

            base.Initialize();
        }

        private void resetGame(GameState gameState)
        {
            this.gameState = gameState;

            _player.Reset(_defaultPlayerPosition);

            _enemies = new List<Enemy>();
            _lasers = new List<Laser>();
            _explosions = new List<Explosion>();

            _previousSpawnTime = TimeSpan.Zero;
            _previousLaserSpawnTime = TimeSpan.Zero;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _mainMenuTexture = Content.Load<Texture2D>("Graphics/mainMenu");
            _gameOverTexutre = Content.Load<Texture2D>("Graphics/endMenu");

            Animation playerAnimation = new Animation();
            Texture2D playerTexture = Content.Load<Texture2D>("Graphics\\shipAnimation");
            playerAnimation.Initialize(playerTexture, Vector2.Zero, 115, 69, 8, 30, Color.White, gameScale, true);

            _defaultPlayerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, 
                GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height / 2);
            _player.Initialize(playerAnimation, _defaultPlayerPosition);

            _enemyTexture = Content.Load<Texture2D>("Graphics/mineAnimation");
            _laserTexture = Content.Load<Texture2D>("Graphics/laser");
            _explosionTexture = Content.Load<Texture2D>("Graphics/explosion");

            _gameMusic = Content.Load<Song>("Sound/gameMusic");
            _mainMenuMusic = Content.Load<Song>("Sound/menuMusic");
            _explosionSound = Content.Load<SoundEffect>("Sound/explosion");
            _laserSound = Content.Load<SoundEffect>("Sound/laserFire");

            _spriteFont = Content.Load<SpriteFont>("Graphics/gameFont");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            _previousGamePadState = _currentGamePadState;
            _previousKeyboardState = _currentKeyboardState;
            _previousMouseState = _currentMouseState;

            _currentKeyboardState = Keyboard.GetState();
            _currentGamePadState = GamePad.GetState(PlayerIndex.One);
            _currentMouseState = Mouse.GetState();

            if (gameState == GameState.Main)
            {
                if (MediaPlayer.State != MediaState.Playing || MediaPlayer.Queue.ActiveSong.Name != "menuMusic")
                {
                    MediaPlayer.Volume = 0.6f;
                    MediaPlayer.Play(_mainMenuMusic);
                }

                UpdateMenu(gameTime);
            }
            else if (gameState == GameState.Game)
            {
                if (MediaPlayer.State != MediaState.Playing || MediaPlayer.Queue.ActiveSong.Name != "gameMusic")
                {
                    MediaPlayer.Volume = 1.0f;
                    MediaPlayer.Play(_gameMusic);
                }

                UpdatePlayer(gameTime);
                UpdateEnemies(gameTime);
                UpdateLaser(gameTime);
                UpdateCollision(gameTime);
                UpdateExplosions(gameTime);

                _midBackgroundParrallex.Update(gameTime);
                _topBackgroundParallex.Update(gameTime);
            }
            else
            {
                UpdateGameOver(gameTime);
            }

            base.Update(gameTime);
        }

        private void UpdateMenu(GameTime gameTime)
        {
            if (_currentKeyboardState.IsKeyDown(Keys.Space) || _currentMouseState.LeftButton == ButtonState.Pressed || _currentGamePadState.Buttons.X == ButtonState.Pressed)
            {
                _midBackgroundParrallex.Initialize(Content, "Graphics/bgLayer1", GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, -1);
                _topBackgroundParallex.Initialize(Content, "Graphics/bgLayer2", GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, -2);
                _bottomBackgroundTexture = Content.Load<Texture2D>("Graphics/mainbackground");

                gameState = GameState.Game;
            }
        }

        private void UpdateGameOver(GameTime gameTime)
        {
            if (_currentKeyboardState.IsKeyDown(Keys.Space) || _currentMouseState.LeftButton == ButtonState.Pressed || _currentGamePadState.Buttons.X == ButtonState.Pressed)
            {
                resetGame(GameState.Game);
            }
        }

        private void UpdatePlayer(GameTime gameTime)
        {
            _player.Update(gameTime);
            if (!_player.Active)
            {
                if (gameTime.TotalGameTime - _timeDead >= _intervalBetweenDeadAndGameOver)
                    gameState = GameState.GameOver;
                return;
            }

            // Windows 8 Touch Gestures for MonoGame
            while (TouchPanel.IsGestureAvailable)
            {
                GestureSample gesture = TouchPanel.ReadGesture();

                if (gesture.GestureType == GestureType.FreeDrag)
                {
                    _player.Position += gesture.Delta;
                }
            }

            if (_currentMouseState.LeftButton == ButtonState.Pressed)
            {
                Vector2 mousePosition = new Vector2(_currentMouseState.X, _currentMouseState.Y);
                Vector2 posDelta = mousePosition - _player.Position;

                posDelta.Normalize();
                posDelta = posDelta * _playerMoveSpeed;
                _player.Position = _player.Position + posDelta;
            }
            else
            {
                // Thumbstick
                _player.Position.X += _currentGamePadState.ThumbSticks.Left.X * _playerMoveSpeed;
                _player.Position.Y -= _currentGamePadState.ThumbSticks.Left.Y * _playerMoveSpeed;

                // Side movement should be slower than forward/backward movment
                if (_currentKeyboardState.IsKeyDown(Keys.Left) || _currentGamePadState.DPad.Left == ButtonState.Pressed)
                {
                    _player.Position.X -= _playerMoveSpeed;
                }

                if (_currentKeyboardState.IsKeyDown(Keys.Right) || _currentGamePadState.DPad.Right == ButtonState.Pressed)
                {
                    _player.Position.X += _playerMoveSpeed;
                }

                if (_currentKeyboardState.IsKeyDown(Keys.Up) || _currentGamePadState.DPad.Up == ButtonState.Pressed)
                {
                    _player.Position.Y -= _playerMoveSpeed;
                }

                if (_currentKeyboardState.IsKeyDown(Keys.Down) || _currentGamePadState.DPad.Down == ButtonState.Pressed)
                {
                    _player.Position.Y += _playerMoveSpeed;
                }
            }

            if (_currentKeyboardState.IsKeyDown(Keys.Space) || _currentGamePadState.Buttons.X == ButtonState.Pressed)
            {
                if (gameTime.TotalGameTime - _previousLaserSpawnTime >= _minimumLaserSpawnIntervalTime)
                {
                    _previousLaserSpawnTime = gameTime.TotalGameTime;
                    AddLaser(_player.Position);
                }
            }

            _player.Position.X = MathHelper.Clamp(_player.Position.X, 0,
                GraphicsDevice.Viewport.Width - (_player.Width * _player.PlayerAnimation.scale));
            _player.Position.Y = MathHelper.Clamp(_player.Position.Y, 0, 
                GraphicsDevice.Viewport.Height - (_player.Height * _player.PlayerAnimation.scale));
        }

        private void AddLaser(Vector2 _playerPosition)
        {
            Laser laser = new Laser();
            var position = new Vector2();
            position.X = _player.Position.X + 95;
            position.Y = _player.Position.Y + _player.Height / 2 - _laserTexture.Height / 2;
            laser.Initialize(_laserTexture, position, GraphicsDevice.Viewport.Width, Color.White, 1f);
            _lasers.Add(laser);
        }

        private void UpdateLaser(GameTime gameTime)
        {
            for (int i = _lasers.Count - 1; i >= 0; i--)
            {
                _lasers[i].Update(gameTime);
                if (_lasers[i].Active == false)
                {
                    _lasers.RemoveAt(i);
                }
            }
        }

        private void AddEnemy()
        {
            Animation enemyAnimation = new Animation();
            enemyAnimation.Initialize(_enemyTexture, Vector2.Zero, 47, 61, 8, 30, Color.White, 1f, true);
            Vector2 position = new Vector2(GraphicsDevice.Viewport.Width + _enemyTexture.Width / 2, _random.Next(100, GraphicsDevice.Viewport.Height - 100));
            Enemy enemy = new Enemy();
            enemy.Initialize(enemyAnimation, position, _random.Next(2, 6));
            _enemies.Add(enemy);
        }

        private void UpdateEnemies(GameTime gameTime)
        {
            // Spawn a new enemy every enemySpawnTime seconds
            if (gameTime.TotalGameTime - _previousSpawnTime > _enemySpawnTime)
            {
                _previousSpawnTime = gameTime.TotalGameTime;
                AddEnemy();
            }

            // Update the Enemies
            for (int i = _enemies.Count - 1; i >= 0; i--)
            {
                _enemies[i].Update(gameTime);
                if (_enemies[i].Active == false)
                {
                    _enemies.RemoveAt(i);
                }
            }
        }

        private void AddExplosion(Vector2 centerOfDestroyedObject)
        {
            Animation explosionAnimation = new Animation();
            explosionAnimation.Initialize(_explosionTexture, Vector2.Zero, 133, 134, 12, 30, Color.White, 1f, false);
            Explosion explosion = new Explosion();
            Vector2 position = new Vector2(centerOfDestroyedObject.X - 133 / 2, centerOfDestroyedObject.Y - 134 / 2);
            explosion.Initialize(explosionAnimation, position);
            _explosions.Add(explosion);
        }

        private void UpdateExplosions(GameTime gameTime)
        {
            for (int i = _explosions.Count - 1; i >= 0; --i)
            {
                _explosions[i].Update(gameTime);
                if (!_explosions[i].Active)
                    _explosions.RemoveAt(i);
            }
        }

        private void UpdateCollision(GameTime gameTime)
        {
            var explosionSoundInstance = _explosionSound.CreateInstance();

            // Use the Rectangle’s built-in intersect function to
            // determine if two objects are overlapping
            Rectangle playerRectangle;
            Rectangle enemyRectangle;
            Rectangle laserRectangle;

            playerRectangle = new Rectangle((int)_player.Position.X, (int)_player.Position.Y, _player.Width, _player.Height);

            foreach (var enemy in _enemies)
            {
                enemyRectangle = new Rectangle((int)enemy.Position.X, (int)enemy.Position.Y, enemy.Width, enemy.Height);

                // Do the collision between the laser and the enemies
                foreach (var laser in _lasers)
                {
                    laserRectangle = new Rectangle((int)laser.Position.X, (int)laser.Position.Y, laser.Width, laser.Height);
                    if (laserRectangle.Intersects(enemyRectangle))
                    {
                        laser.Active = false;
                        enemy.Health -= laser.Damage;

                        if (enemy.Health <= 0)
                        {
                            enemy.Active = false;

                            AddExplosion(new Vector2(enemy.Position.X + enemy.Width / 2, enemy.Position.Y + enemy.Height / 2));

                            explosionSoundInstance.Volume = 0.2f;
                            explosionSoundInstance.Play();
                        }
                    }
                }

                // Do the collision between the player and the enemies
                if (_player.Active && enemy.Active && playerRectangle.Intersects(enemyRectangle))
                {
                    _player.Health -= enemy.Damage;
                    enemy.Health = 0;

                    AddExplosion(new Vector2(enemy.Position.X + enemy.Width / 2, enemy.Position.Y + enemy.Height / 2));
                    explosionSoundInstance.Volume = 0.2f;
                    explosionSoundInstance.Play();

                    if (_player.Health <= 0)
                    {
                        _player.Active = false;
                        _timeDead = gameTime.TotalGameTime;

                        AddExplosion(new Vector2(_player.Position.X + _player.Width / 2, _player.Position.Y + _player.Height / 2));
                        explosionSoundInstance.Volume = 0.5f;
                        explosionSoundInstance.Play();
                    }
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            var instructionTextPos = new Vector2(_graphics.GraphicsDevice.Viewport.Width / 2, _graphics.GraphicsDevice.Viewport.Height - _graphics.GraphicsDevice.Viewport.Height / 5);

            if (gameState == GameState.Main)
            {
                _spriteBatch.Draw(_mainMenuTexture, _wholeScreenRectangle, Color.White);

                var mainMenuText = "Press space or touch the panel to start.";
                var textOrigin = _spriteFont.MeasureString(mainMenuText) / 2;
                _spriteBatch.DrawString(_spriteFont, mainMenuText, instructionTextPos, Color.Gray, 0, textOrigin, 1.5f, SpriteEffects.None, 0.5f);
            }
            else if (gameState == GameState.Game)
            {
                _spriteBatch.Draw(_bottomBackgroundTexture, _wholeScreenRectangle, Color.White);
                _midBackgroundParrallex.Draw(_spriteBatch);
                _topBackgroundParallex.Draw(_spriteBatch);

                _player.Draw(_spriteBatch);

                foreach (var enemy in _enemies)
                    enemy.Draw(_spriteBatch);

                foreach (var laser in _lasers)
                    laser.Draw(_spriteBatch);

                foreach (var explosion in _explosions)
                    explosion.Draw(_spriteBatch);
            }
            else
            {
                _spriteBatch.Draw(_gameOverTexutre, _wholeScreenRectangle, Color.White);

                var gameOverText = "Press space or touch the panel to start again.";
                var textOrigin = _spriteFont.MeasureString(gameOverText) / 2;
                _spriteBatch.DrawString(_spriteFont, gameOverText, instructionTextPos, Color.Gray, 0, textOrigin, 1.5f, SpriteEffects.None, 0.5f);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
