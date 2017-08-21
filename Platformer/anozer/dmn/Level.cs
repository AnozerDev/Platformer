using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Platformer.anozer.dmn.enemies;
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
        //nombre de tuiles sur la largeur
        private int mapWidth;
        //Height & width en pixel
        private Vector2 mapDimension;
        public Vector2 getMapDimensions => mapDimension;

        private Texture2D tileset;
        private int tilesetColumnsCount;

        private int tilesWidth, tilesHeight;

        private Dictionary<int, String> terrainsTypes;
        private Dictionary<Vector2, int> terrainTiles;
        
        public Vector2 playerStartPosition { get; private set; }
        private List<Enemy> _enemies = new List<Enemy>();

        public bool hasCollidedFloor { get; private set; }
        public bool hasCollidedWall { get; private set; }
        
        public float gravity { get; private set; }
        public float friction { get; private set; }
        
        public enum CollideType
        {
            NONE,
            BOTTOM,
            DESCENT_LEFT,
            DESCENT_RIGHT,
            WALL,
            DEADLY
        }
        public CollideType collideType { get; private set; }

        public Level(ContentManager contentMgr, string mapPath, string tilesetsRoot, string tilesetName)
        {
            map = new TmxMap(mapPath);
            mapWidth = map.Width;

            tileset = contentMgr.Load<Texture2D>(tilesetsRoot + map.Tilesets[tilesetName].Name);
            tilesetColumnsCount = (int) map.Tilesets[tilesetName].Columns;

            tilesWidth = map.TileWidth;
            tilesHeight = map.TileHeight;
            
            mapDimension = new Vector2(map.Width*tilesWidth, map.Height*tilesHeight); //TODO un peu redondant avec mapWidth nan ?

    /*wtf        foreach (var property in map.Properties)
            {
                Console.WriteLine(property.Key);
                Console.WriteLine(double.Parse(property.Value.Trim()));
            }
    */      gravity = 0.17f;//TODO Convert.ToSingle(map.Properties["gravity"]);  float.PARSE ? pfff peut être en fonction de la langue par default.. format() ?
            friction = 0.37f;
            
            terrainsTypes = map.Tilesets["tiles"].Terrains
                .ToDictionary( tileID => tileID.Tile, type => type.Name);

            terrainTiles = map.Layers["terrain"].Tiles
                .ToDictionary(pos => new Vector2(pos.X, pos.Y), gid => gid.Gid );
            TmxObject player = map.ObjectGroups["player_position"].Objects["player"];
            playerStartPosition = new Vector2( (float) player.X, (float) player.Y);

            foreach (var enemy in map.ObjectGroups["enemies"].Objects)
            {
                switch (enemy.Name)
                {
                    case "skeleton":
                        _enemies.Add(new Skeleton(new Vector2((float) enemy.X, (float) enemy.Y)));
                        break;
                 default:
                     Console.WriteLine("ENI");
                     break;   
                }
            }
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
Console.WriteLine("COLLIDE LEFT");
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
                
Console.WriteLine("COLLIDE RIGHT");                
                return true;
            }
            
            return false;
        }

        private bool isCollide(Vector2 topLeftPOS, Vector2 bottomRightPOS, Direction direction)//TODO prend le perso complet, tu t'emmerde !!
        {
            //TODO collideTOPLEFT, collideBOTTOMRIGHT 
            //collide bottomCenter 
            int bottomTileGid = getGidAt(new Vector2(topLeftPOS.X + (bottomRightPOS.X - topLeftPOS.X) / 2, bottomRightPOS.Y));
            int rightTiledGid = getGidAt(new Vector2(bottomRightPOS.X + (bottomRightPOS.X - topLeftPOS.X) / 2, bottomRightPOS.Y));

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
                        if (terrainsTypes.TryGetValue(getGidAt(new Vector2(topLeftPOS.X, bottomRightPOS.Y)), out type))
                        {
                            if (type == "floor")
                            {
                                collideType = CollideType.BOTTOM;
                                break;//TODO ICI POIVRO EN PERMANENCE EN SLIP dois y avoir un soucis dans le change(state) quand en slip..
                            }
                        }
                        collideType = CollideType.DESCENT_LEFT;
                        break;
                        
                    case "descent_right":
                        collideType = CollideType.DESCENT_RIGHT;
                        //TODO check pieddroit
                        break;
                        
                    case "wall": // TODO floor... wall... un peu la même chose non ? -> points de collisions different ?
                        collideType = CollideType.WALL; // TODO et quand tu prend un sol de coté ?? devrai verifier la position de l'impact sur la tuile, pas seulement la tuile !!
                        break;
                    case "deadly":
                        collideType = CollideType.DEADLY;
                        break;
                }
                return true;
            }

            hasCollidedFloor = false;
            collideType = CollideType.NONE;
            return false;
        }
        
        public void checkCollisions(Vector2 persoPos, Vector2 bottomRight, Direction direction)
        {
            isCollide(persoPos, bottomRight, direction);
            
        }
        
        public void update(float deltaTime)
        {
            foreach (Enemy enemy in _enemies)
            {
                enemy.update(deltaTime, gravity,friction );
                checkCollisions(enemy.positionPropertie, enemy.bottomRightPosition, enemy.directionPropertie);
                if ( hasCollidedFloor || hasCollidedWall)
                {
                    enemy.collidedOn(collideType, hasCollidedWall);
                }
            }
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

            foreach (var enemy in _enemies)
            {
                enemy.draw(spriteBatch);
            }

        }

    }//    end class
}