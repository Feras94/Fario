﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace IMPORT_PLATFORM
{
    public class Spinner : PatrolingEnemy
    {
        public Spinner(ContentManager Content, Vector2 location)
        {
            this.texture = EnemiesManager.EnemiesSheet;

            animations.Add(
                "Walk",
                new AnimationStrip(
                    texture,
                    Content.Load<Rectangle[]>(@"SheetData/Enemies/Spinner/SpinnerWalk"),
                    "Walk"));
            animations["Walk"].LoopAnimation = true;
            animations["Walk"].FrameLength = 0.1f;

            animations.Add(
                "Hurt",
                new AnimationStrip(
                    texture,
                    Content.Load<Rectangle[]>(@"SheetData\Enemies\Spinner\SpinnerHurt"),
                    "Hurt"));
            animations["Hurt"].LoopAnimation = false;
            animations["Hurt"].NextAnimation = "Walk";
            animations["Hurt"].FrameLength = 1.0f;

            animations.Add(
                "Dead",
                new AnimationStrip(
                    texture,
                    Content.Load<Rectangle>(@"SheetData\Enemies\Spinner\SpinnerDead"),
                    "Dead"));
            animations["Dead"].LoopAnimation = true;

            this.worldLocation = location;
            this.maxSbd = new Vector2(2f * 60.0f, 100.0f * 60.0f);
            this.acc = 50.0f * 60.0f;
            this.dec = 0;// 15.0f * 60.0f;
            this.hasGravity = true;
            this.checkCollisions = true;
            this.limitSpeed = true;
            this.InitializePhisics(maxSbd, acc, dec, hasGravity, checkCollisions);

            this.drawDepth = 0.9f;
            this.enabled = true;
            this.scoreValue = 10;
            this.PlayAnimation("Walk");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
