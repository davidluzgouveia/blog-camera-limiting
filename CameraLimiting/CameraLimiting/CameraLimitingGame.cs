namespace CameraLimiting
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    public class CameraLimitingGame : Game
    {
        private enum Mode
        {
            Unbound,
            Total,
            Four,
            Single
        }

        public CameraLimitingGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 800;
            Content.RootDirectory = "Content";
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _bg = Content.Load<Texture2D>("bg");
            _font = Content.Load<SpriteFont>("font");
            _camera = new Camera(GraphicsDevice.Viewport);
        }

        protected override void Update(GameTime gameTime)
        {
            float elapsedTime = (float) gameTime.ElapsedGameTime.TotalSeconds;
            
            _currentKeyboard = Keyboard.GetState();

            if (_currentKeyboard.IsKeyDown(Keys.Right))
                _camera.Position += new Vector2(400.0f * elapsedTime, 0.0f);

            if (_currentKeyboard.IsKeyDown(Keys.Left))
                _camera.Position += new Vector2(-400.0f * elapsedTime, 0.0f);

            if (_currentKeyboard.IsKeyDown(Keys.Down))
                _camera.Position += new Vector2(0.0f, 400.0f * elapsedTime);

            if (_currentKeyboard.IsKeyDown(Keys.Up))
                _camera.Position += new Vector2(0.0f, -400.0f * elapsedTime);

            if (_currentKeyboard.IsKeyDown(Keys.PageUp))
                _camera.Zoom += 2.5f * elapsedTime * _camera.Zoom;

            if (_currentKeyboard.IsKeyDown(Keys.PageDown))
            {
                _camera.Zoom -= 2.5f * elapsedTime * _camera.Zoom;
            }

            if (_currentKeyboard.IsKeyDown(Keys.A) && _previousKeyboard.IsKeyUp(Keys.A))
            {
                _camera.Limits = null;
                ResetCamera();
                _mode = Mode.Unbound;
            }

            if (_currentKeyboard.IsKeyDown(Keys.S) && _previousKeyboard.IsKeyUp(Keys.S))
            {
                _camera.Limits = new Rectangle(0, 0, 512, 512);
                ResetCamera();
                _mode = Mode.Total;
            }

            if (_currentKeyboard.IsKeyDown(Keys.D) && _previousKeyboard.IsKeyUp(Keys.D))
            {
                _camera.Limits = CalculateFrameRectangle(512, 512, 4, 4, _tileCounter4);
                ResetCamera();
                _tileCounter4 = (_tileCounter4 + 1) % (4 * 4);
                _mode = Mode.Four;
            }

            if (_currentKeyboard.IsKeyDown(Keys.F) && _previousKeyboard.IsKeyUp(Keys.F))
            {
                _camera.Limits = CalculateFrameRectangle(512, 512, 16, 16, _tileCounter16);
                ResetCamera();
                _tileCounter16 = (_tileCounter16 + 1) % (16 * 16);
                _mode = Mode.Single;
            }

            _previousKeyboard = _currentKeyboard;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            // Draw scene
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, _camera.ViewMatrix);
            _spriteBatch.Draw(_bg, Vector2.Zero, Color.White);
            _spriteBatch.End();

            // Draw demo text
            string text = string.Empty;
            switch (_mode)
            {
                case Mode.Unbound:
                    text = "Mode: Free camera";
                    break;
                case Mode.Total:
                    text = "Mode: Limit camera to 512x512 region";
                    break;
                case Mode.Four:
                    text = "Mode: Limit camera to 128x128 regions (" + _tileCounter4 + ")";
                    break;
                case Mode.Single:
                    text = "Mode: Limit camera to 32x32 regions (" + _tileCounter16 + ")";
                    break;
            }
            _spriteBatch.Begin();
            DrawStringHelper(_spriteBatch, _font, text, new Vector2(10,10));
            DrawStringHelper(_spriteBatch, _font, "Keys: A/S/D/F = Change Modes ", new Vector2(10, 720));
            DrawStringHelper(_spriteBatch, _font, "http://www.david-gouveia.com", new Vector2(10, 750));
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private static void DrawStringHelper(SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position)
        {
            spriteBatch.DrawString(font, text, new Vector2(2, 2) + position, Color.Black);
            spriteBatch.DrawString(font, text, new Vector2(-2, 2) + position, Color.Black);
            spriteBatch.DrawString(font, text, new Vector2(2, -2) + position, Color.Black);
            spriteBatch.DrawString(font, text, new Vector2(-2, -2) + position, Color.Black);
            spriteBatch.DrawString(font, text, position, Color.Yellow);  
        }

        private void ResetCamera()
        {
            _camera.Zoom = 1f;
            _camera.Position = Vector2.Zero;
        }

        private static Rectangle CalculateFrameRectangle(int width, int height, int columns, int rows, int frame)
        {
            int tileWidth = width / columns;
            int tileHeight = height / rows;
            int x = frame % columns;
            int y = frame / columns;
            return new Rectangle(x * tileWidth, y * tileHeight, tileWidth, tileHeight);
        }

        private Camera _camera;
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _bg;
        private SpriteFont _font;

        private KeyboardState _previousKeyboard;
        private KeyboardState _currentKeyboard;
        private Mode _mode;
        private int _tileCounter4;
        private int _tileCounter16;

        static void Main(string[] args)
        {
            using (CameraLimitingGame game = new CameraLimitingGame())
            {
                game.Run();
            }
        }
    }
}