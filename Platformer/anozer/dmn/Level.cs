using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TiledSharp;

namespace Platformer.anozer.dmn
{

    public enum TerrainType
    {
        FLOOR,
        DESCENT_RIGHT,
        DESCENT_LEFT
    }

    public class Level
    {
        private TmxMap map;
        private int mapWidth;

        private Texture2D tileset;
        private int tilesetColumnsCount;

        private int tilesWidth, tilesHeight;

        private Dictionary<int, String> terrainsTypes;
        private Dictionary<Vector2, int> terrainTiles;

        public bool hasCollidedFloor { get; private set; }
        public bool hasCollidedWall { get; private set; }

        public enum CollideType
        {
            NONE,
            BOTTOM,
            DESCENT_LEFT,
            DESCENT_RIGHT,
            WALL
        }
        public CollideType collideType { get; private set; }

        public Level(ContentManager contentMgr, String mapPath, String tilesetsRoot, String tilesetName)
        {
            map = new TmxMap(mapPath);
            mapWidth = map.Width;

            tileset = contentMgr.Load<Texture2D>(tilesetsRoot + map.Tilesets[tilesetName].Name);
            tilesetColumnsCount = (int) map.Tilesets[tilesetName].Columns;

            tilesWidth = map.TileWidth;
            tilesHeight = map.TileHeight;
            
            terrainsTypes = map.Tilesets["tiles"].Terrains
                .ToDictionary( tileID => tileID.Tile, type => type.Name);

            terrainTiles = map.Layers["terrain"].Tiles
                .ToDictionary(pos => new Vector2(pos.X, pos.Y), gid => gid.Gid );
            
        }

        private int getGidAt(Vector2 position)
        {
            int column, line;
            column = (int) Math.Floor(position.X / tilesWidth);
            line = (int) Math.Floor(position.Y / tilesHeight);
            int gid;
            if (terrainTiles.TryGetValue(new Vector2(column, line), out gid))
            {
                return gid-1;
            }
            return -1;
        }

        private bool isCollideLeft(Vector2 topLeft, Vector2 bottomRight)
        {
            int highterMiddleGID = getGidAt(new Vector2(topLeft.X, topLeft.Y + (bottomRight.Y - topLeft.Y) / 3 ));
            int lowerMiddleGID = getGidAt(new Vector2(topLeft.X, Convert.ToSingle(topLeft.Y + (bottomRight.Y - topLeft.Y) / 1.5) ));

            string type;
            if (terrainsTypes.TryGetValue(highterMiddleGID, out type) ||
                terrainsTypes.TryGetValue(lowerMiddleGID, out type))
            {
                return true;
            }
            
            return false;
        }
        
        private bool isCollideRight(Vector2 topLeft, Vector2 bottomRight)
        {
            int highterMiddleGID = getGidAt(new Vector2(bottomRight.X, topLeft.Y + (bottomRight.Y - topLeft.Y) / 3 ));
            int lowerMiddleGID = getGidAt(new Vector2(bottomRight.X, Convert.ToSingle(topLeft.Y + (bottomRight.Y - topLeft.Y) / 1.5) ));

            string type;
            if (terrainsTypes.TryGetValue(highterMiddleGID, out type) ||
                terrainsTypes.TryGetValue(lowerMiddleGID, out type))
            {
                return true;
            }
            
            return false;
        }

        private bool isCollide(Vector2 topLeftPOS, Vector2 bottomRightPOS, Direction direction)//TODO prend le perso complet, tu t'emmerde !!
        {
            //TODO collideTOPLEFT, collideBOTTOMRIGHT 
            //collide bottomCenter 
            int bottomTileGid = getGidAt(new Vector2(topLeftPOS.X + (bottomRightPOS.X - topLeftPOS.X) / 2, bottomRightPOS.Y));
            int rightTiledGid = getGidAt(new Vector2(topLeftPOS.X + (bottomRightPOS.X - topLeftPOS.X) / 2, bottomRightPOS.Y));

            hasCollidedWall = (direction == Direction.LEFT)
                ? isCollideLeft(topLeftPOS, bottomRightPOS)
                : isCollideRight(topLeftPOS, bottomRightPOS);
            
            
            string type;
            if (terrainsTypes.TryGetValue(bottomTileGid, out type))
            {
                hasCollidedFloor = true;
                switch (type)
                {
                    case "floor":
                        collideType = CollideType.BOTTOM;
                        break;
                        
                    case "descent_left":
                        //TODO left && right, ne colisionner que lorsque que l' on est dans la moitié de l'image affichée !! ( triangle rectangle ?? )
                        collideType = CollideType.DESCENT_LEFT;
                        break;
                        
                    case "descent_right":
                        collideType = CollideType.DESCENT_RIGHT;
                        break;
                        
                    case "wall": // TODO floor... wall... un peu la même chose non ?
                        collideType = CollideType.WALL; // TODO et quand tu prend un sol de coté ?? devrai verifier la position de l'impact sur la tuile, pas seulement la tuile !!
                        break;
                }
                return true;
            }

            hasCollidedFloor = false;
            collideType = CollideType.NONE;
            return false;
        }
        
        public void update(Vector2 persoPos, Vector2 bottomRight, Direction direction)
        {
            isCollide(persoPos, bottomRight, direction);
        }
        
        public void draw(SpriteBatch spriteBatch)
        {
            int nbLayers = map.Layers.Count;

            int line, column;
            for (int layer = 0; layer < nbLayers; layer++)
            {
                line = 0; column = 0;
                for (int i = 0; i < map.Layers[layer].Tiles.Count; i++)
                {
                    int gid = map.Layers[layer].Tiles[i].Gid;

                    if (gid != 0)
                    {
                        int frameID = gid - 1;
                        int frameColumn = frameID % tilesetColumnsCount;
                        int frameLine = (int) Math.Floor((double) (frameID / tilesetColumnsCount));

                        float x = column * tilesWidth;
                        float y = line * tilesHeight;

                        Rectangle frameRectangle = new Rectangle(
                            tilesWidth * frameColumn, tilesHeight * frameLine, tilesWidth, tilesHeight
                        );
                        
                        spriteBatch.Draw(tileset, new Vector2(x, y), frameRectangle, Color.White);
                        
                    }
                    column++;
                    if (column == mapWidth)
                    {
                        column = 0;
                        line++;
                    }
                }
            }

        }

    }//    end class
}