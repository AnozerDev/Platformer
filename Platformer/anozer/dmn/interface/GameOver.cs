using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Platformer.anozer.dmn.@interface
{
    public class GameOver
    {
        private Platformer _platformer;
        private string _fontDirectory;
        
        private string _title = "GAME OVER";
        private string _pressStart = "PRESS ENTER\n    to restart";
        private string _pressBack = "BACK\n    to back main menu";

        private SpriteFont _titleFont;
        private Vector2 _titleDimensions;
        private Vector2 _titlePosition;
        private Vector2 _titleOrigin;

        private SpriteFont _indicationsFont;

        private Vector2 _pressStartPosition;
        private Vector2 _pressStartOrigin;
        
        private Vector2 _pressDelPosition;
        private Vector2 _pressDelOrigin;
        
        //TODO continues ?? s' il reste des vie au perso
        // TODO restart
        //TODO return menu principal

        public GameOver(Platformer platformer, string fontDirectory)
        {
            this._platformer = platformer;
            this._fontDirectory = fontDirectory;
            
            _titleFont = platformer.Content.Load<SpriteFont>($"{fontDirectory}Docporn");
            _titleDimensions = _titleFont.MeasureString(_title);
            _titlePosition = new Vector2(Platformer.WINDOW_WIDTH / 2, Platformer.WINDOW_HEIGHT / 3);
            
            _titleOrigin = new Vector2( _titleDimensions.X / 2, _titleDimensions.Y / 2);

            _indicationsFont = platformer.Content.Load<SpriteFont>($"{fontDirectory}Pecita");
            
            _pressStartPosition = new Vector2(_titlePosition.X, _titlePosition.Y + 100);
            _pressStartOrigin = new Vector2(_indicationsFont.MeasureString(_pressStart).X/2,
                _indicationsFont.MeasureString(_pressStart).Y/2);
            
            _pressDelPosition = new Vector2(_pressStartPosition.X, _pressStartPosition.Y + 100);
            _pressDelOrigin = _pressStartOrigin;
        }

        public void update(KeyboardState keyboardState)
        {
            if (keyboardState.GetPressedKeys().Length > 0)
            {
            Console.WriteLine(keyboardState.GetPressedKeys().First());
            }
            if (keyboardState.IsKeyDown(Keys.Back)) _platformer.backToMenu();
            if (keyboardState.IsKeyDown(Keys.Enter)) _platformer.startGame();
            
        }

        public void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(_titleFont, _title, _titlePosition, Color.DarkOrange, 0,
                _titleOrigin,1.02f,SpriteEffects.None, 0);
            spriteBatch.DrawString(_titleFont, _title, _titlePosition, Color.OrangeRed, 0,
                _titleOrigin, 1, SpriteEffects.None, 0);
            
            spriteBatch.DrawString(_indicationsFont, _pressStart, _pressStartPosition, Color.MediumPurple, 0,
                _pressStartOrigin, 1.4f, SpriteEffects.None, 0);
            spriteBatch.DrawString(_indicationsFont, _pressBack, _pressDelPosition, Color.MediumPurple, 0,
                _pressDelOrigin, 1.4f, SpriteEffects.None, 0);
        }
    }
}