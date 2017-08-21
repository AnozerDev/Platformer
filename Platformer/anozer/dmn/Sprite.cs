using System.Net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenGL;

namespace Platformer.anozer.dmn
{
    public class Sprite
    {
        private Texture2D _spriteSheet;

        private Rectangle[] _frames;
        private float _timePerFrame;
        
        private float _animationTimer;

        private int _currentFrameID;

        private float _scale;
        public float scalePropertie => _scale;
        
        private SpriteEffects _effect;

        public Vector2 sizePropertie => new Vector2(_frames[_currentFrameID].Width, _frames[_currentFrameID].Height);
        
        


        public Sprite(Texture2D spriteSheet,int columnsCount, int linesCount, float timePerFrame, float scale)
        {
            _spriteSheet = spriteSheet;

            _frames = getFramesFrom(_spriteSheet, columnsCount, linesCount);
            _timePerFrame = timePerFrame;

            _scale = scale;

        }
        
        /// <summary>
        /// Incremente la frame à animer (Rectangle[i]) en fonction du deltaTime.
        /// /!\ Necessite variables animation speed et animationTimer dans la class hote.
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <param name="framesToAnime"></param>
        private void animeIt(float deltaTime)
        {
            _animationTimer += deltaTime;
            if (_animationTimer >= _timePerFrame)
            {
                _currentFrameID++;
                if (_currentFrameID >= _frames.Length) _currentFrameID = 0;
                
                _animationTimer = 0;
            }
        }
        
        /// <summary>
        /// Renvois un tableau de rectangles comprennan les positions de chaque frames d' un spritesheet
        /// </summary>
        /// <param name="nbColumns">Le nombre de frames par lignes</param>
        /// <param name="nbLines">Le nombre de frames par colonnes</param>
        /// <param name="textureToSplit">Le spritesheet à couper</param>
        /// <returns>Rectangle [] de la taille du nombre d'images</returns>
        private Rectangle[] getFramesFrom(Texture2D texture, int nbColumns, int nbLines)
        {
            int framesWidth, framesHeight;
            Rectangle[] frames;
            int frameID;
            
            framesWidth = texture.Width / nbColumns;
            framesHeight = texture.Height / nbLines;
            
            frames = new Rectangle[nbColumns*nbLines];
            frameID = 0;

            for (int l = 0; l < nbLines; l++)
            {
                for (int c = 0; c < nbColumns; c++)
                {
                    frames[frameID++] = new Rectangle(
                        new Point( c*framesWidth, l*framesHeight),
                        new Point( framesWidth, framesHeight));
                }
            }
            
            return frames;
        }

        public void update(float deltaTime, SpriteEffects spriteEffect)
        {
            _effect = spriteEffect;
            animeIt(deltaTime);
        }

        public void draw(SpriteBatch spriteBatch, Vector2 position)
        {
            spriteBatch.Draw(
                _spriteSheet, // la Texture
                position, // la position sur l' ecran
                _frames[_currentFrameID], // rectangle source
                Color.White, // Couleur de teinte, white ne color pas
                0f, // rotation en radians
                new Vector2(0, 0), //origin du sprite 0,0 par default
                _scale, // echelle du sprite TODO mise à l' echelle en fonction de la taille de l'ecran
                _effect, // Effet sur le sprite
                0f // layerDeph (z-index)
                );
        }
    }
}