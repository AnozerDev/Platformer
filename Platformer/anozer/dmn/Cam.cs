using Microsoft.Xna.Framework;

namespace Platformer.anozer.dmn
{
    public class Cam
    {
        private Vector2 _position;
        public Vector2 getPosition => _position;
        
        private Matrix _matrix; 
        public Matrix getMatrix => _matrix;

        private Vector2 mapDimensions;

        public Cam(Vector2 mapDimensions)
        {
            this.mapDimensions = mapDimensions;
        }
        
        public void update(Vector2 playerPosition)
        {
            _position.X = playerPosition.X - Platformer.WINDOW_WIDTH / 2;
            _position.Y = playerPosition.Y - Platformer.WINDOW_HEIGHT / 2.5f;

            if (_position.X < 0)
                _position.X = 0;
            if (_position.Y < 0)
                _position.Y = 0;
            if (_position.X > mapDimensions.X - Platformer.WINDOW_WIDTH)
                _position.X = mapDimensions.X - Platformer.WINDOW_WIDTH;
            if (_position.Y > mapDimensions.Y - Platformer.WINDOW_HEIGHT)
                _position.Y = mapDimensions.Y - Platformer.WINDOW_HEIGHT;

            _matrix = Matrix.CreateTranslation(new Vector3(-_position, 0));
            
        }
    }
}