using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;

namespace Platformer.anozer.dmn.@interface
{
    public class Menu
    {
        private Platformer _platformer;
        
        private const string TITLE_FONT_NAME = "Docporn";
        private const string INPUT_INDICATION_FONT = "Pecita";
        
        
        //TODO inclure ces private string dans un xml de config !!
        private string _titre = "Platformer";
        private string _sousTitre = "at least a try...";
        private string _debugMode = $"Debug mode {(Platformer.DEBUG?"ON": "OFF")}"; //TODO euh.. update non ?
        private string _pressToStart = "Press Enter to start";
        
        private Vector2 _titrePos;
        private Vector2 _titreOrigine;
        private SpriteFont _titreFont;

        private Vector2 _titleDimensions;

        private Vector2 _sousTitrePos;
        private Vector2 _pressStartPos;

        private SpriteFont _indicationsFont;
        private Vector2 _pressStartOrigine;
        
        
        
        
        public Menu(Platformer platformer , string fontDirectory)
        {
            _platformer = platformer;
            
            this._titrePos = new Vector2(Platformer.WINDOW_WIDTH / 2, Platformer.WINDOW_HEIGHT / 4);
            _titreFont = platformer.Content.Load<SpriteFont>($"{fontDirectory}{TITLE_FONT_NAME}");
            
            _titleDimensions = _titreFont.MeasureString(_titre);
            this._titreOrigine = new Vector2( _titleDimensions.X / 2, _titleDimensions.Y / 2);

            _sousTitrePos = new Vector2(_titrePos.X, _titrePos.Y + _titleDimensions.Y);

            _indicationsFont = platformer.Content.Load<SpriteFont>($"{fontDirectory}{INPUT_INDICATION_FONT}");
            
            _pressStartPos = new Vector2(_titrePos.X, _sousTitrePos.Y + 100);
            _pressStartOrigine = new Vector2(_indicationsFont.MeasureString(_pressToStart).X/2,
                _indicationsFont.MeasureString(_pressToStart).Y/2);
            
            
        }


        public void update(KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyDown(Keys.Enter)) _platformer.startGame();
            
        }
        
        
        public void drawMenu(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(_titreFont, _titre, _titrePos, Color.White, 0, _titreOrigine, 1, SpriteEffects.None, 0);
            spriteBatch.DrawString(_titreFont, _sousTitre, _sousTitrePos,
                Color.LightGray, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
            spriteBatch.DrawString(_indicationsFont, _pressToStart, _pressStartPos, Color.MediumPurple, 0,
                _pressStartOrigine, 1.4f, SpriteEffects.None, 0);

            if (!Platformer.RELEASED)
            {
            //    spriteBatch.DrawString(_indicationsFont, "\"d\" for debug-mode", new Vector2 );
            }
            
        }
    }
}