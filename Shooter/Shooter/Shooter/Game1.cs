using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input.Touch;

/* PROBLEM LOG

 * when I go from end screen to start screen the music doesn't reset
 * fix upgrade object sprite so that it displays properly, problem with width...
 * 
 * TO DO
 * add in animation and sound effect for upgrade object (power up theme)
 * add new upgrade objects in
 * consider putting in downgrade objects (probably not, but maybe in higher levels)
 * implement a scoring system where the program saves the highest scores (probably in a text file)
 * get all new sprites and make it a space game instead
 * make it so enemies start firing back eventually
 * add new enemies
 * make it so that user can pause game
 * make game start and end menu interactive (with array and other screens)
 * add settings/controls/objects to main menu
 * add my info to back page (could put it under info/about option in main menu as well)

*/
namespace Shooter
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        // Represents the player 
        Player player;



       // bool gameState;
        Texture2D startScreen;
        Texture2D endScreen;

        Rectangle screen;

        bool startScreenIsOn;
        bool endScreenIsOn;
        // Keyboard states used to determine key presses
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;

        // Gamepad states used to determine button presses
       // GamePadState currentGamePadState;
        //GamePadState previousGamePadState;

        // A movement speed for the player
        float playerMoveSpeed;

        // Image used to display the static background
        //Texture2D mainBackground;

        int backgroundSpeed;
        
        // Parallaxing Layers
        ParallaxingBackground bgLayer1;
        //ParallaxingBackground bgLayer2;

        // if we wanted just one enemy in the game we’d add one Enemy class and get it into the Draw() and Update() loops, and be done
        
        // Enemies
        Texture2D enemyTexture;
        List<Enemy> enemies;

        // The rate at which the enemies appear
        TimeSpan enemySpawnTime;
        TimeSpan previousSpawnTime;

        // A random number generator
        Random random;

        Texture2D projectileTexture;
        List<Projectile> projectiles;

        Texture2D upgradeObjectTexture;
        List<UpgradeObject> upgradeObjects;

        // The rate of fire of the player laser
        TimeSpan fireTime;
        TimeSpan previousFireTime;

        TimeSpan dropTime;
        TimeSpan previousDropTime;

        Texture2D explosionTexture;
        List<Animation> explosions;

        Texture2D upgradeTexture;
        List<Animation> upgrades;

        // The sound that is played when a laser is fired
        SoundEffect laserSound;

        // The sound used when the player or an enemy dies
        SoundEffect explosionSound;

        // The sound used when the player gets an upgrade object
        SoundEffect upgradeSound;

        // The music played during gameplay
        Song gameplayMusic;

        //Number that holds the player score
        int score;

        // The score that I need to meet to go onto the next level
        int metScore;

        // The level that I am currently on
        int level;

        // The font used to display UI elements
        SpriteFont font;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            
            // Initialize the player class
            player = new Player();

            startScreenIsOn = true;
            endScreenIsOn = false;
            //gameState = false;
            // Set a constant player move speed
            playerMoveSpeed = 8.0f;

            //Enable the FreeDrag gesture.
            //TouchPanel.EnabledGestures = GestureType.FreeDrag;

            backgroundSpeed = -2;
            
            bgLayer1 = new ParallaxingBackground();
            //bgLayer2 = new ParallaxingBackground();


            // Initialize the enemies list
            enemies = new List<Enemy>();

            // Set the time keepers to zero
            previousSpawnTime = TimeSpan.Zero;

            // Used to determine how fast enemy respawns
            enemySpawnTime = TimeSpan.FromSeconds(2f);

            // Initialize our random number generator
            random = new Random();

            projectiles = new List<Projectile>();

            upgradeObjects = new List<UpgradeObject>();

            // Set the laser to fire every quarter second
            fireTime = TimeSpan.FromSeconds(.15f);

            previousDropTime = TimeSpan.Zero;
            
            dropTime = TimeSpan.FromSeconds(4f);

            //dropTime = TimeSpan.FromSeconds(random.Next(0, 1));

            explosions = new List<Animation>();

            upgrades = new List<Animation>();

            //Set player's score to zero
            score = 0;

            metScore = 200;

            level = 1;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            // Load the player resources 
            //Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height / 2);
            //player.Initialize(Content.Load<Texture2D>("player"), playerPosition); *** use only for displays regular texture
            startScreen = this.Content.Load<Texture2D>("mainMenu");
            endScreen = this.Content.Load<Texture2D>("endMenu");
            
            // Load the player resources
            Animation playerAnimation = new Animation();
            Texture2D playerTexture = Content.Load<Texture2D>("shipAnimation");
            playerAnimation.Initialize(playerTexture, Vector2.Zero, 115, 69, 8, 30, Color.White, 1f, true);

            Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height / 2);
            player.Initialize(playerAnimation, playerPosition);

            
            // Load the parallaxing background
            bgLayer1.Initialize(Content, "spaceBackground1", GraphicsDevice.Viewport.Width, backgroundSpeed);
           // bgLayer2.Initialize(Content, "bgLayer2", GraphicsDevice.Viewport.Width, -2);

            enemyTexture = Content.Load<Texture2D>("mineAnimation");

            projectileTexture = Content.Load<Texture2D>("laser");

            upgradeObjectTexture = Content.Load<Texture2D>("healthPack");

            explosionTexture = Content.Load<Texture2D>("explosion");

            upgradeTexture = Content.Load<Texture2D>("healthOrb");

            // make rectangle that fits entire screen
            screen = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            // Load the music
             if (startScreenIsOn == false && endScreenIsOn == false)
                gameplayMusic = Content.Load<Song>("sound/gameMusic");
             else // *** fix conditions so that when I go from end screen to start screen the music doesn't reset
                 gameplayMusic = Content.Load<Song>("sound/menuMusic");

            // Load the laser and explosion sound effect
            laserSound = Content.Load<SoundEffect>("sound/laserFire");
            explosionSound = Content.Load<SoundEffect>("sound/explosion");
            upgradeSound = Content.Load<SoundEffect>("sound/upgrade");

            // Load the score font
            font = Content.Load<SpriteFont>("gameFont");

            // Start the music right away
            PlayMusic(gameplayMusic);

            //mainBackground = Content.Load<Texture2D>("spaceBackground3");
        }

        private void PlayMusic(Song song)
        {
            // Due to the way the MediaPlayer plays music,
            // we have to catch the exception. Music will play when the game is not tethered
            try
            {          
                    // Play the music
                    MediaPlayer.Play(song);

                    // Loop the currently playing song
                    MediaPlayer.IsRepeating = true;
            }
            catch { }
        }
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keys = Keyboard.GetState();

            if(startScreenIsOn == true)
            {
                if (keys.IsKeyDown(Keys.Enter))
                {
                    startScreenIsOn = false;
                    LoadContent();
                }
            }

            if(endScreenIsOn == true)
            {
                if (keys.IsKeyDown(Keys.Escape))
                {
                    Initialize();
                }
            }
            
            if (startScreenIsOn == false && endScreenIsOn == false)
            {
                // Allows the game to exit
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    this.Exit();

                // Save the previous state of the keyboard and game pad so we can determinesingle key/button presses
                // previousGamePadState = currentGamePadState;
                previousKeyboardState = currentKeyboardState;

                // Read the current state of the keyboard and gamepad and store it
                currentKeyboardState = Keyboard.GetState();
                //currentGamePadState = GamePad.GetState(PlayerIndex.One);

                UpgradeObject upgradeObject2 = new UpgradeObject();
                Enemy enemy2 = new Enemy();
                Projectile projectile2 = new Projectile();

                UpdateUpgradeSprite(gameTime);
                
                //Update the player
                UpdatePlayer(gameTime);

                // Update the parallaxing background
                bgLayer1.Update();
                //bgLayer2.Update();

                // Update the enemies
                UpdateEnemies(gameTime);

                if (score >= metScore)
                {
                    levelChange();
                }

                UpdateCollision();

                // Update the projectiles
                UpdateProjectiles();

                UpdateUpgradeObjects(gameTime);

                // Update the explosions
                UpdateExplosions(gameTime);

                base.Update(gameTime);
            }
        }

        private void AddExplosion(Vector2 position)
        {
            Animation explosion = new Animation();
            explosion.Initialize(explosionTexture, position, 134, 134, 12, 45, Color.White, 1f, false);
            explosions.Add(explosion);
        }

        private void UpdateExplosions(GameTime gameTime)
        {
            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                explosions[i].Update(gameTime);

                if (explosions[i].Active == false)
                {
                    explosions.RemoveAt(i);
                }
            }
        }
        
        private void AddUpgradeSprite(Vector2 position)
        {
            Animation upgrade = new Animation();
            upgrade.Initialize(upgradeTexture, position, 80, 70, 12, 45, Color.White, 1f, false);
            upgrades.Add(upgrade);
        }
        private void UpdateUpgradeSprite(GameTime gameTime)
        {
            for (int i = upgrades.Count - 1; i >= 0; i--)
            {
                upgrades[i].Update(gameTime);

                if (upgrades[i].Active == false)
                {
                    upgrades.RemoveAt(i);
                }
            }
        }

        private void AddProjectile(Vector2 position)
        {
            Projectile projectile = new Projectile();
            projectile.Initialize(GraphicsDevice.Viewport, projectileTexture, position); 
            projectiles.Add(projectile);

            /*
            if(levelUp == true)
            {
                projectile.Damage = projectile.Damage + 20;
                projectile.projectileMoveSpeed = projectile.projectileMoveSpeed + 2;
            }
             */
        }

        private void UpdateProjectiles()
        {
            // Update the Projectiles
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                projectiles[i].Update();

                if (projectiles[i].Active == false)
                {
                    projectiles.RemoveAt(i);
                }
            }
        }

        private void AddUpgradeObjects()
        {
            // Randomly generate the position of the upgrade object
            Vector2 position = new Vector2(random.Next(250, 750), 10);
            //Vector2 position = new Vector2(random.Next(100, GraphicsDevice.Viewport.Width - 100), GraphicsDevice.Viewport.Height - upgradeObjectTexture.Height / 2); // 50, and -50 represent the area in which they spawn (higher the number the smaller the area, closer to center)
            UpgradeObject upgradeObject = new UpgradeObject();
            upgradeObject.Initialize(GraphicsDevice.Viewport, upgradeObjectTexture, position);
            upgradeObjects.Add(upgradeObject);
            /*
            if (levelUp == true)
            {
                upgradeObject.Value = upgradeObject.Value + 10;
                upgradeObject.upgradeObjectMoveSpeed = upgradeObject.upgradeObjectMoveSpeed + 5;
            }
            */
        }

        private void levelChange()
        {
            for (int i = upgradeObjects.Count - 1; i >= 0; i--)
            {
                upgradeObjects[i].Value += 10;
                upgradeObjects[i].upgradeObjectMoveSpeed += 5;
            }

            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                enemies[i].Damage += 20;
                enemies[i].Health += 20;
                enemies[i].enemyMoveSpeed += 2;
                enemies[i].Value += 20;
            }

            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                projectiles[i].Damage += 20;
                projectiles[i].projectileMoveSpeed += 2;
            }

            player.Health = player.Health * 2;
            backgroundSpeed -= 25;
            bgLayer1.Initialize(Content, "spaceBackground1", GraphicsDevice.Viewport.Width, backgroundSpeed);
            metScore *= 2;
            level++;
        }

        private void UpdateUpgradeObjects(GameTime gameTime)
        {
            if (gameTime.TotalGameTime - previousDropTime > dropTime)
            {
                previousDropTime = gameTime.TotalGameTime;

                // Add an Enemy
                AddUpgradeObjects();
            }
            
            // Update the upgrade objects
            for (int i = upgradeObjects.Count - 1; i >= 0; i--)
            {
                upgradeObjects[i].Update();


                if (upgradeObjects[i].collided == true)
                {
                    //AddPowerUp(upgradeObjects[i].Position) *** Add in later when I can find some sprites for my power up animation
                    //powerUpSound.Play() ** Add in later when I can find a sound effect for my power up
                    score += upgradeObjects[i].Value;

                    // Add an explosion
                    AddUpgradeSprite(upgradeObjects[i].Position);      
                    
                    upgradeObjects.RemoveAt(i);

                   // Play the upgrade object sound
                    upgradeSound.Play();             
                }
            }
        }

        private void UpdateCollision() // can solve rectagular collision problem by implementing a “pixel-perfect” collision system
        {
            // Use the Rectangle's built-in intersect function to 
            // determine if two objects are overlapping
            Rectangle rectangle1;
            Rectangle rectangle2;
            Rectangle rectangle3;

            // Only create the rectangle once for the player
            rectangle1 = new Rectangle((int)player.Position.X, (int)player.Position.Y, player.Width, player.Height);

            // Do the collision between the player and the enemies
            for (int i = 0; i < enemies.Count; i++)
            {
                rectangle2 = new Rectangle((int)enemies[i].Position.X, (int)enemies[i].Position.Y, enemies[i].Width, enemies[i].Height);

                // Determine if the two objects collided with each
                // other
                if (rectangle1.Intersects(rectangle2))
                {
                    // Subtract the health from the player based on
                    // the enemy damage
                    player.Health -= enemies[i].Damage;

                    // Since the enemy collided with the player
                    // destroy it
                    enemies[i].Health = 0;

                    // If the player health is less than zero we died
                    if (player.Health <= 0)
                    {
                        player.Active = false;
                    }
                }
            }


            // Projectile vs Enemy Collision
            for (int i = 0; i < projectiles.Count; i++)
            {
                for (int j = 0; j < enemies.Count; j++)
                {
                    // Create the rectangles we need to determine if we collided with each other
                    rectangle1 = new Rectangle((int)projectiles[i].Position.X - projectiles[i].Width / 2, (int)projectiles[i].Position.Y - projectiles[i].Height / 2, projectiles[i].Width, projectiles[i].Height);

                    rectangle2 = new Rectangle((int)enemies[j].Position.X - enemies[j].Width / 2, (int)enemies[j].Position.Y - enemies[j].Height / 2, enemies[j].Width, enemies[j].Height);

                    // Determine if the two objects collided with each other
                    if (rectangle1.Intersects(rectangle2))
                    {
                        enemies[j].Health -= projectiles[i].Damage;
                        projectiles[i].Active = false;
                    }
                }
            }

            // player vs upgrade object collision
            for (int i = 0; i < upgradeObjects.Count; i++)
            {
                    rectangle1 = new Rectangle((int)player.Position.X, (int)player.Position.Y, player.Width, player.Height);

                    // Create the rectangles we need to determine if we collided with each other
                     rectangle3 = new Rectangle((int)upgradeObjects[i].Position.X, (int)upgradeObjects[i].Position.Y, upgradeObjects[i].Width, upgradeObjects[i].Height);

                    // Determine if the two objects collided with each other
                    if (rectangle1.Intersects(rectangle3))
                    {
                        player.Health += upgradeObjects[i].Value;
                        upgradeObjects[i].Active = false;
                        upgradeObjects[i].collided = true;
                    }
                
            }
        }

        private void AddEnemy()
        { 
            // Create the animation object
            Animation enemyAnimation = new Animation();

            // Initialize the animation with the correct animation information
            enemyAnimation.Initialize(enemyTexture, Vector2.Zero, 47, 61, 8, 30,Color.White, 1f, true);

            // Randomly generate the position of the enemy
            Vector2 position = new Vector2(GraphicsDevice.Viewport.Width +enemyTexture.Width / 2, random.Next(50, GraphicsDevice.Viewport.Height -50)); // 50, and -50 represent the area in which they spawn (higher the number the smaller the area, closer to center)

            // Create an enemy
            Enemy enemy = new Enemy();

            // Initialize the enemy
            enemy.Initialize(enemyAnimation, position);

            // Condition to see if the level changed then make my enemies harder 
            /*
            if (level != prevLevel)
            {
                levelUp = true;
            }
            */
            /*
            if (levelUp == true)
            {
                enemy.Damage = enemy.Damage + 20;
                enemy.Health = enemy.Health + 20;
                enemy.enemyMoveSpeed = enemy.enemyMoveSpeed + 2;
                enemy.Value = enemy.Value + 20;
                //UpdatePlayer(blah); // figure out parameter shit because I need to change player class attributes before leaving if statement :)
                AddProjectile(position);

                levelUp = false;
            }
            */
            // Add the enemy to the active enemies list
            enemies.Add(enemy);
        }

        private void UpdateEnemies(GameTime gameTime)
        {
          
            // Spawn a new enemy enemy every 1.5 seconds
            if (gameTime.TotalGameTime - previousSpawnTime > enemySpawnTime)
            {
                previousSpawnTime = gameTime.TotalGameTime;

                // Add an Enemy
                AddEnemy();
            }

            // Update the Enemies
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                enemies[i].Update(gameTime);

                if (enemies[i].Active == false)
                {
                    // If not active and health <= 0
                    if (enemies[i].Health <= 0)
                    {
                        // Add an explosion
                        AddExplosion(enemies[i].Position);

                        // Play the explosion sound
                        explosionSound.Play();

                        //Add to the player's score
                        score += enemies[i].Value;

                        /*
                        if(score == metScore)
                        {                          
                            level++;
                            metScore *= 2;
                        }
                         * */
                    }

                    enemies.RemoveAt(i);
                }
            }
        }

        private void UpdatePlayer(GameTime gameTime)
        {
            player.Update(gameTime); // *** don't need this line for displaying regular textures

            /*
            // Condition to see if the level changed then make my enemies harder
            if (levelUp == true) // when the level changes, increase players health (update player is called specifically from addEnemy to do this)
            {
                player.Health = player.Health * 2;

                levelUp = false;
            }
            */
            
            // Get Thumbstick Controls
            //player.Position.X += currentGamePadState.ThumbSticks.Left.X * playerMoveSpeed;
            //player.Position.Y -= currentGamePadState.ThumbSticks.Left.Y * playerMoveSpeed;

            // Use the Keyboard / Dpad
            if (currentKeyboardState.IsKeyDown(Keys.Left)) // || currentGamePadState.DPad.Left == ButtonState.Pressed ** add if using gamepad
            {
                player.Position.X -= playerMoveSpeed;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Right)) // || currentGamePadState.DPad.Right == ButtonState.Pressed ** add if using gamepad          
            {
                player.Position.X += playerMoveSpeed;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Up)) // || currentGamePadState.DPad.Up == ButtonState.Pressed ** add if using gamepad           
            {
                player.Position.Y -= playerMoveSpeed;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Down)) // || currentGamePadState.DPad.Down == ButtonState.Pressed ** add if using gamepad         
            {
                player.Position.Y += playerMoveSpeed;
            }

            // Make sure that the player does not go out of bounds
            player.Position.X = MathHelper.Clamp(player.Position.X, GraphicsDevice.Viewport.TitleSafeArea.Left + player.Width / 2, GraphicsDevice.Viewport.TitleSafeArea.Right - player.Width / 2);
            player.Position.Y = MathHelper.Clamp(player.Position.Y, GraphicsDevice.Viewport.TitleSafeArea.Top + player.Height / 2, GraphicsDevice.Viewport.TitleSafeArea.Bottom - player.Height / 2);

            // Fire only every interval we set as the fireTime
            if (currentKeyboardState.IsKeyDown(Keys.Space))
            {
                if (gameTime.TotalGameTime - previousFireTime > fireTime)
                {
                    // Reset our current time
                    previousFireTime = gameTime.TotalGameTime;

                    // Add the projectile, but add it to the front and center of the player
                    AddProjectile(player.Position + new Vector2(player.Width / 2, 0));
                    
                    // Play the laser sound
                    laserSound.Play();
                }
            }

            // reset score if player health goes to zero
            if (player.Health <= 0)
            {
                endScreenIsOn = true;
                LoadContent();
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            // Start drawing
            spriteBatch.Begin();

            if(startScreenIsOn == true)
            {
                spriteBatch.Draw(startScreen, screen, Color.White);

                // Press enter to start game
                spriteBatch.DrawString(font, "Press enter to play game", new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + 475, GraphicsDevice.Viewport.TitleSafeArea.Y + 400), Color.Goldenrod);
            }

            if(endScreenIsOn == true)
            {
                spriteBatch.Draw(endScreen, screen, Color.White);

                // Press esc to go back to main menu
                spriteBatch.DrawString(font, "Score: " + score, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + 325, GraphicsDevice.Viewport.TitleSafeArea.Y + 200), Color.Red); // not printing out proper score *** Need to fix
                spriteBatch.DrawString(font, "Press escape to go back to main menu", new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + 150, GraphicsDevice.Viewport.TitleSafeArea.Y + 250), Color.Red);
            }

            if (startScreenIsOn == false && endScreenIsOn == false)
            {
                //spriteBatch.Draw(mainBackground, Vector2.Zero, Color.Black);
                //GraphicsDevice.Clear(Color.Black);
                
                // Draw the moving background
                bgLayer1.Draw(spriteBatch);
                //bgLayer2.Draw(spriteBatch);

                // Draw the upgrade sprites
                for (int i = 0; i < upgrades.Count; i++)
                {
                    upgrades[i].Draw(spriteBatch);
                }

                // Draw the Enemies
                for (int i = 0; i < enemies.Count; i++)
                {
                    enemies[i].Draw(spriteBatch);
                }

                // Draw the Projectiles
                for (int i = 0; i < projectiles.Count; i++)
                {
                    projectiles[i].Draw(spriteBatch);
                }

                // Draw the Projectiles
                for (int i = 0; i < upgradeObjects.Count; i++)
                {
                    upgradeObjects[i].Draw(spriteBatch);
                }

                // Draw the explosions
                for (int i = 0; i < explosions.Count; i++)
                {
                    explosions[i].Draw(spriteBatch);
                }
                          
             
                // Draw the score *** score is not outputting correctly FIX IT!!!!!!!!
                spriteBatch.DrawString(font, "Score: " + score, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);

                // Draw the player health
                spriteBatch.DrawString(font, "Health: " + player.Health, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + 300, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);

                // Draw the current level
                spriteBatch.DrawString(font, "Level: " + level, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + 700, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);

                // Draw the Player
                player.Draw(spriteBatch);
            }

            // Stop drawing
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
