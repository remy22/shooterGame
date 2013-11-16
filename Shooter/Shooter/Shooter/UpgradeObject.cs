using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shooter
{
    class UpgradeObject
    {
        // Image representing the upgrade object
        public Texture2D Texture;

        // The position of the upgrade object relative to the top left corner of thescreen
        public Vector2 Position;

        // Represents the viewable boundary of the game
        Viewport viewport;

        // The state of the upgrade object
        public bool Active;

       // has the upgrade object collided with the player
        public bool collided;

        // The amount of upgrade the upgrade object will give to the player
        public int Value;

        // Get the width of the enemy ship
        public int Width
        {
            get { return Texture.Width; }
        }

        // Get the height of the enemy ship
        public int Height
        {
            get { return Texture.Height; }
        }

        // The speed at which the upgrade object moves
        public float upgradeObjectMoveSpeed; // added public so that I could access it in my game1 class to make the upgrade object faster upon level change

        public void Initialize(Viewport viewport, Texture2D texture, Vector2 position)
        {
            Texture = texture;
            Position = position;
            this.viewport = viewport;

            Active = true;

            collided = false;

            Value = 10;

            upgradeObjectMoveSpeed = 5f;
        }
        public void Update()
        {
            // Projectiles always move down
            Position.Y += upgradeObjectMoveSpeed;

            // Deactivate the upgrade object if it goes out of screen
            if (Position.Y + Texture.Width / 2 > viewport.Height)
                Active = false;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, 0f, new Vector2(Width / 2, Height / 2), 1f, SpriteEffects.None, 0f);
        }
    }
}
