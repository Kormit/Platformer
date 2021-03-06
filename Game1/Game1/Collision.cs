﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
    class Collision
    {
        public Game1 game;


        public bool IsColliding(Sprite Hero, Sprite otherSprite)
        {

            if(Hero.rightEdge < otherSprite.leftEdge ||
               Hero.leftEdge > otherSprite.rightEdge ||
               Hero.bottomEdge < otherSprite.topEdge ||
               Hero.topEdge > otherSprite.bottomEdge)
            {
                //these two rectangles are not colliding
                return false;
            }
            //otherwise, the two are overlapping therefore there is a collision
            return true;

        }

        bool CheckForTile(Vector2 coordinates) // Checks if there is a tile at the specified coordinates
        {
            int column = (int)coordinates.X;
            int row = (int)coordinates.Y;

            if (column < 0 || column > game.levelTileWidth - 1)
            {
                return false;
            }
            if (row < 0 || row > game.levelTileHeight - 1)
            {
                return true;
            }

            Sprite tilefound = game.levelGrid[column, row];

            if (tilefound != null)
            {
                return true;
            }
            return false;
        

        }

        Sprite CollideLeft(Sprite hero, Vector2 tileIndex, Sprite playerPrediction)
        {
            Sprite tile = game.levelGrid[(int)tileIndex.X, (int)tileIndex.Y];
            if (IsColliding(playerPrediction, tile) == true && hero.velocity.X < 0)
            {
                hero.position.X = tile.rightEdge + hero.offset.X;
                hero.velocity.X = 0;
            }
            return hero;


        }

        Sprite CollideRight(Sprite hero, Vector2 tileIndex, Sprite playerPrediction)
        {
            Sprite title = game.levelGrid[(int)tileIndex.X, (int)tileIndex.Y];

            if (IsColliding(playerPrediction, title) == true && hero.velocity.X > 0)
            {
                hero.position.X = title.leftEdge - hero.width + hero.offset.X;
                hero.velocity.X = 0;
            }

            return hero;


        }


        Sprite CollideAbove(Sprite hero, Vector2 tileIndex, Sprite playerPrediction)
        {
            Sprite tile = game.levelGrid[(int)tileIndex.X, (int)tileIndex.Y];

            if (IsColliding(playerPrediction, tile) == true && hero.velocity.Y < 0)
            {
                hero.position.Y = tile.bottomEdge + hero.offset.Y;
                hero.velocity.Y = 0;
            }
            return hero;

        }

        Sprite CollideBelow(Sprite hero, Vector2 tileIndex, Sprite playerPrediction)
        {
            Sprite tile = game.levelGrid[(int)tileIndex.X, (int)tileIndex.Y];
            if (IsColliding(playerPrediction, tile) == true && hero.velocity.Y > 0)
            {
                hero.position.Y = tile.topEdge - hero.height + hero.offset.Y;
                hero.velocity.Y = 0;
            }
            return hero;

        }

        Sprite CollideBottomDiagonals(Sprite hero, Vector2 tileIndex, Sprite playerPrediction)
        {
            Sprite tile = game.levelGrid[(int)tileIndex.X, (int)tileIndex.Y];
            int leftEdgeDistance = Math.Abs(tile.leftEdge - playerPrediction.rightEdge);
            int rightEdgeDistance = Math.Abs(tile.rightEdge - playerPrediction.leftEdge);
            int topEdgeDistance = Math.Abs(tile.topEdge - playerPrediction.bottomEdge);

            if (IsColliding(playerPrediction, tile) == true)
            {
                if (topEdgeDistance < rightEdgeDistance && topEdgeDistance < leftEdgeDistance)
                {
                    //if the top edge is closest, collision is happening above the platform
                    hero.position.Y = tile.topEdge - hero.height + hero.offset.Y;
                    hero.velocity.Y = 0;
                }
                else if (rightEdgeDistance < leftEdgeDistance)
                {
                    // if the right edge is closest, the collision is happening to the right of the platform
                    hero.position.X = tile.rightEdge + hero.offset.X;
                    hero.velocity.X = 0;
                }
                else
                {
                    //else if the left edge is closest, the collision is happening to the left of the platform
                    hero.position.X = tile.leftEdge - hero.width + hero.offset.X;
                    hero.velocity.X = 0;
                }



            }
        }

        Sprite CollideAboveDiagonals(Sprite hero, Vector2 tileIndex, Sprite playerPrediction)
        {
            Sprite tile = game.levelGrid[(int)tileIndex.X, (int)tileIndex.Y];
            int leftEdgeDistance = Math.Abs(tile.rightEdge - playerPrediction.leftEdge);
            int rightEdgeDistance = Math.Abs(tile.leftEdge - playerPrediction.rightEdge);
            int bottomEdgeDistance = Math.Abs(tile.bottomEdge - playerPrediction.topEdge);
            if(IsColliding(playerPrediction, tile) == true)
            {
                if (bottomEdgeDistance < leftEdgeDistance && bottomEdgeDistance < rightEdgeDistance)
                {
                    hero.position.Y = tile.bottomEdge + hero.offset.Y;
                    hero.velocity.Y = 0;
                }


            }
        }



        public Sprite CollideWithPlatforms(Sprite hero, float deltaTime)
        {
            //create a copy f the hero that will move to where the hero will be in the next frame
            Sprite playerPrediction = new Sprite();
            playerPrediction.position = hero.position;
            playerPrediction.width = hero.width;
            playerPrediction.height = hero.height;
            playerPrediction.offset = hero.offset;
            playerPrediction.UpdateHitBox();

            playerPrediction.position += hero.velocity * deltaTime;

            int playerColumn = (int)Math.Round(playerPrediction.position.X / game.tileHeight);
            int playerRow = (int)Math.Round(playerPrediction.position.Y / game.tileHeight);

            Vector2 playerTile = new Vector2(playerColumn, playerRow);

            Vector2 leftTile = new Vector2(playerTile.X - 1, playerTile.Y);
            Vector2 rightTile = new Vector2(playerTile.X + 1, playerTile.Y);
            Vector2 topTile = new Vector2(playerTile.X, playerTile.Y - 1);
            Vector2 bottomTile = new Vector2(playerTile.X, playerTile.Y + 1);

            Vector2 bottomLeftTile = new Vector2(playerTile.X - 1, playerTile.Y + 1);
            Vector2 bottomRightTile = new Vector2(playerTile.X + 1, playerTile.Y + 1);
            Vector2 topLeftTile = new Vector2(playerTile.X - 1, playerTile.Y - 1);
            Vector2 topRightTile = new Vector2(playerTile.X + 1, playerTile.Y - 1);
            // This allows us to predict if the hero wil be overlapping an obstacle in the next frame.

            bool leftCheck = CheckForTile(leftTile);
            bool rightCheck = CheckForTile(rightTile);
            bool topCheck = CheckForTile(topTile);
            bool bottomCheck = CheckForTile(bottomTile);

            bool bottomLeftCheck = CheckForTile(bottomLeftTile);
            bool bottomRightCheck = CheckForTile(bottomRightTile);
            bool topLeftCheck = CheckForTile(topLeftTile);
            bool topRightCheck = CheckForTile(topRightTile);

            if (leftCheck == true)//check for collisions with the tiles left of the player
            {
                hero = CollideLeft(hero, leftTile, playerPrediction);
            }
            if (rightCheck == true)
            {
                hero = CollideRight(hero, rightTile, playerPrediction);
            }
            if (bottomCheck == true)
            {
                hero = CollideBelow(hero, bottomTile, playerPrediction);
            }
            if (topCheck == true)
            {
                hero = CollideAbove(hero, topTile, playerPrediction);
            }








            return hero;
        }




    }
}
