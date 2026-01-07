using GAlgoT2530.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab06
{
    public class Background : SpriteGameObject, ICollidable
    {
        private Rectangle _rectangle;

        public Background(string textureName) : base(textureName)
        {
            // Intentionally left blank
        }

        public override void Initialize()
        {
            _rectangle = _game.Window.ClientBounds;
            _rectangle.X = 0;
            _rectangle.Y = 0;
        }

        public override void Draw()
        {
            _game.SpriteBatch.Begin(samplerState: SamplerState.LinearWrap);
            _game.SpriteBatch.Draw(Texture, Vector2.Zero, _game.Window.ClientBounds, Color.White);
            _game.SpriteBatch.End();
        }

        string ICollidable.GetGroupName()
        {
            return this.GetType().Name;
        }
        
        Rectangle ICollidable.GetBound()
        {
            return _rectangle;
        }
    }
}
