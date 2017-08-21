using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Platformer.anozer.dmn
{
    public class Resources
    {
        private static Resources _resources;

        public Dictionary<string, Dictionary<PersoState, Texture2D>> ENEMIES_TEXTURES;

        private ContentManager _content;
        
        //TODO tu guetterai pas apres sqlite ?
        
        private Resources()
        {
            _content = Platformer.CONTENT_MGR;
            completeEnemies();
        }

        public static Resources get() => _resources ?? (_resources = new Resources());

        //TDOO t' as vraiment prevus de charger tout les enemies ? meme ceux qui n' aparaiterai pas ?
        private void completeEnemies()
        {
            ENEMIES_TEXTURES = new Dictionary<string, Dictionary<PersoState, Texture2D>>()
            {
                {
                    "skeleton", new Dictionary<PersoState, Texture2D>
                    {
                        {
                            PersoState.IDLE, _content.Load<Texture2D>("skeleton_idle_r")
                        },
                        {
                            PersoState.WALK, _content.Load<Texture2D>("skeleton_walk")
                        }
                    }
                }
            };
        }

    }

}