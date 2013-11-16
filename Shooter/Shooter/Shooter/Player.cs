using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Shooter
{
    class Player
    {
        // Animation representing the player
        public Animation PlayerAnimation;

        // Animation representing the player
       // public Texture2D PlayerTexture; ***Use just for displaying a texture without movement/animation

        // Position of the Player relative to the upper left side of the screen
        public Vector2 Position;

        // State of the player
        public bool Active;

        // Amount of hit points that player has
        public int Health;

        // Get the width of the player ship
        public int Width
        {
            get { return PlayerAnimation.FrameWidth; } // *** use PlayerTexture.Width for regular texture
        }

        // Get the height of the player ship
        public int Height
        {
            get { return PlayerAnimation.FrameHeight; }  // *** use PlayerTexture.Height for regular texture
        }

        public void Initialize(Animation animation, Vector2 position) // *** have to pass in Texture2D texture if you want regular texture
        {
            //PlayerTexture = texture; *** used for just plain texture

            PlayerAnimation = animation;

            // Set the starting position of the player around the middle of the screen and to the back
            Position = position;

            // Set the player to be active
            Active = true;

            // Set the player health
            Health = 100;
        }

        public void Update(GameTime gameTime)
        {
            PlayerAnimation.Position = Position; // *** If I wanted a regular texture then update would be completely empty, with no parameters
            PlayerAnimation.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
           // spriteBatch.Draw(PlayerTexture, Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f); *** only use this for regular texture
            PlayerAnimation.Draw(spriteBatch);
        }
    }
}