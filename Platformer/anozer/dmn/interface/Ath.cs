using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace Platformer.anozer.dmn.@interface
{
    public class Ath
    {

        private bool debug;

        private string debugString = " ";
        private string backgroundString ="";

        private Vector2 position;
        
        private SpriteFont debugFont;
        private Vector2 debugPosition;
        private Vector2 debugFontOrigin;

        public Ath(bool debug)
        {
            this.debug = debug;
            
        }
        public void load(ContentManager content, string fontDirectory)
        {
            if (debug)
            {
                debugFont = content.Load<SpriteFont>(fontDirectory + "debugFont");
                position = new Vector2(Platformer.WINDOW_WIDTH - 5, 5);
            }
        }

        public void update(Personage player, Level level, Vector2 camPosition)
        {
            
            if (debug)
            {
                debugPosition = camPosition + position;
                
                //TODO mettre ça ailleur que dans l' update ? 
                debugString = $"State :: {player.statePropertie}\n" +
                              $"Pos :: {player.positionPropertie}\n" +
                              $"Collide on :: {level.collideType}\n" +
                              $"Velocity :: {player.velocityPropertie}";
                
                debugFontOrigin = new Vector2(debugFont.MeasureString(debugString).X,0);

                backgroundString = Regex.Replace(debugString, @".",Regex.Unescape("\\u2593")).Replace("\n", "▓\n");
                
            }
        }
        
        public void draw(SpriteBatch spriteBatch)
        {
            if (debug)
            {
                Vector2 vect = debugFont.MeasureString(debugString);
                
                Color[] data = new Color[(int)vect.X*(int)vect.Y];
                Texture2D rect = new Texture2D(spriteBatch.GraphicsDevice, (int) vect.X, (int) vect.Y);
                for (int i = 0; i < data.Length; i++) data[i] = Color.Black;
                rect.SetData(data);
                spriteBatch.Draw(rect, debugPosition, null, Color.White,0, debugFontOrigin, 1, SpriteEffects.None, 1);
                
                spriteBatch.DrawString(
                    debugFont,
                    backgroundString,
                    debugPosition,
                    Color.Brown,
                    0,    //rotation,
                    debugFontOrigin,
                    1,    //scale
                    SpriteEffects.None,
                    1    //layer
                     );
                spriteBatch.DrawString(
                    debugFont,
                    debugString,
                    debugPosition,
                    Color.LightGoldenrodYellow,
                    0,    //rotation,
                    debugFontOrigin,
                    1,    //scale
                    SpriteEffects.None,
                    0    //layer
                    );
            }
        }
    }
}