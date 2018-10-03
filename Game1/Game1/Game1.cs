﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Graphics;
using MonoGame.Extended.ViewportAdapters;
using System.Collections;


namespace Game1
{
    

    public class Game1 : Game
    {

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Player player = new Player(); //Create an instance of our player class

        Camera2D camera = null; // Creates an instance of a 2D camera
        TiledMap map = null; // Creates an instance of a Tiled map
        TiledMapRenderer mapRenderer = null; // Creates an instance of what makes a tiled map

        TiledMapTileLayer collisionLayer;
        public ArrayList allCollisionTiles = new ArrayList();
        public Sprite[,] levelGrid;

        public int tileHeight = 0;
        public int levelTileWidth = 0;
        public int levelTileHeight = 0;

        public Rectangle myMap = new Rectangle();


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            
            graphics.PreferredBackBufferWidth = 1600;
            graphics.PreferredBackBufferHeight = 900;

            Content.RootDirectory = "Content";
        }

        
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            myMap.X = 0;
            myMap.Y = 0;
            myMap.Width = 6400; // each block is 64 by 64 px
            myMap.Height = 6400;

            base.Initialize();
        }

        
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);


            player.Load(Content, this); // Call the 'Load' Function in the player class. 
            // 'this' basically means "pass all information in our class through as a variable"

            BoxingViewportAdapter viewportAdapter = new BoxingViewportAdapter(Window,
                                                                               GraphicsDevice,
                                                                               graphics.GraphicsDevice.Viewport.Width,
                                                                               graphics.GraphicsDevice.Viewport.Height);

            camera = new Camera2D(viewportAdapter);
            camera.Position = new Vector2(0, graphics.GraphicsDevice.Viewport.Height);

            map = Content.Load<TiledMap>("Level 1");
            mapRenderer = new TiledMapRenderer(GraphicsDevice);

            SetUpTiles();

        }

       
        protected override void UnloadContent()
        {
            
        }

        public void SetUpTiles()
        {
            tileHeight = map.TileHeight;
            levelTileHeight = map.Height;
            levelTileWidth = map.Width;
            levelGrid = new Sprite[levelTileWidth, levelTileHeight];
            foreach(TiledMapTileLayer layer in map.TileLayers)
            {
                if(layer.Name == "Collision")
                {
                    collisionLayer = layer;
                }
            }

            int columns = 0;
            int rows = 0;
            int loopCount = 0;
            while (loopCount < collisionLayer.Tiles.Count)
            {
                if(collisionLayer.Tiles[loopCount].GlobalIdentifier != 0)
                {
                    Sprite tileSprite = new Sprite();
                    tileSprite.position.X = columns * tileHeight;
                    tileSprite.position.Y = rows * tileHeight;
                    tileSprite.width = tileHeight;
                    tileSprite.height = tileHeight;
                    tileSprite.UpdateHitBox();
                    allCollisionTiles.Add(tileSprite);
                    levelGrid[columns, rows] = tileSprite;
                }

                columns++;

                if(columns == levelTileWidth)
                {
                    columns = 0;
                    rows++;
                }

                loopCount++;
                
            }

        }

        
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            player.Update(deltaTime); // Call the 'Update' from our player class

            //This makes the camera follow the player's position, it also should center the player to the centre of the camera
            camera.Position = player.playerSprite.position - new Vector2(graphics.GraphicsDevice.Viewport.Width / 2,
                                                                            graphics.GraphicsDevice.Viewport.Height / 2);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //Clears anything previously drawn to the screen
            GraphicsDevice.Clear(Color.Gray);

            var viewMatrix = camera.GetViewMatrix();
            var projectionMatrix = Matrix.CreateOrthographicOffCenter(0,
                                                                      GraphicsDevice.Viewport.Width,
                                                                      GraphicsDevice.Viewport.Height,
                                                                      0, 0.0f, -1.0f);


            //Begin drawing
            spriteBatch.Begin(transformMatrix: viewMatrix);


            mapRenderer.Draw(map, ref viewMatrix, ref projectionMatrix);
            //call the 'Draw' function from our player class

            //camera.Position = player.playerSprite.position - new Vector2(graphics.GraphicsDevice.Viewport.Width / 2,
                                                                            //graphics.GraphicsDevice.Viewport.Height / 2);

            player.Draw(spriteBatch);
            //finish drawing
            spriteBatch.End();
                
            base.Draw(gameTime);
        }
    }
}
