using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Super_Mario
{
    class Player : GameObject
    {
        private Vector2 myPosition;
        private Point mySize;

        public Player(Vector2 aPosition, Point aSize) : base(aPosition, aSize)
        {
            this.myPosition = aPosition;
            this.mySize = aSize;
        }

        public void Update()
        {

        }

        public override void Draw(SpriteBatch aSpriteBatch)
        {
            
        }
    }
}
