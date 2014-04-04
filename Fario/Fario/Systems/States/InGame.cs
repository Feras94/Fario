﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

using Tile_Engine;

using Application = System.Windows.Forms.Application;

namespace IMPORT_PLATFORM
{
    public enum InGameState { Playing, Died, Won, GameOver }

    public  class InGame
    {
        #region Declerations

        private  Player player;

        private  NumbersHelper numHelper;

        private  ContentManager Content;

        private  FarioMain mainGame;

        private  SpriteFont gameFont;

        private LevelManager levelManager;

        private StringBuilder states = new StringBuilder();

        private InGameState state = InGameState.Playing;

        #region Strings

        private string canMoveText = "Press Enter To Move To The Next Level";
        private string diedText = "You Have Died...\nPress Enter To Restart The Level";
        private string wonLevelText = "Congratulations You Have Moved To The Next Level\nPress Enter To Continue...";

        #endregion

        private bool canAdvance = false;

        private bool finishedGame = false;

        #endregion

        #region Properties

        public  string LevelName
        {
            get { return levelManager.CurrentLevel; }
        }

        public LevelManager LevelMANAGER
        {
            get { return levelManager; }
        }

        public InGameState InGameState
        {
            get { return state; }
            set { state = value; }
        }

        #endregion

        #region Initialize

        public InGame(FarioMain parentGame)
        {
            mainGame = parentGame;
            Content = parentGame.Content;
            numHelper = parentGame.NumHelper;
            player = mainGame.GamePlayer;
            gameFont = mainGame.StateFont;

            TileMap.Initialize(Content.Load<Texture2D>(@"Textures\Tileset"));
            TileMap.EditorMode = false;
            TileMap.spriteFont = mainGame.DebugFont;

            levelManager = new LevelManager(mainGame);
            levelManager.LoadLevel("1");
        }

        #endregion

        #region Update And Draw

        public  void Update(GameTime gameTime)
        {
            switch (state)
            {
                case InGameState.Playing:
                    UpdatePlaying(gameTime);
                    break;
                case InGameState.Died:
                    UpdateDied(gameTime);
                    break;
                case InGameState.Won:
                    UpdateWon(gameTime);
                    break;
                case InGameState.GameOver:
                    UpdateGameOver(gameTime);
                    break;
            }
        }

        #region Update Helper Methods

        private void UpdatePlaying(GameTime gameTime)
        {
            PlayingInput();
            player.Update(gameTime);
            levelManager.Update(gameTime);
            if (player.WorldLocation.Y > Camera.WorldRectangle.Height + 300)
            {
                MusicManager.Instance.PlayEffect(SFXType.Fall);
                player.LoseLife();
            }
            string code = DestinationMapName(player.WorldCenter);
            if (code != "")
            {
                string filePath = Application.StartupPath + "/Content/Maps/" + code + ".FarioMap";
                if (File.Exists(filePath))
                {
                    canAdvance = true;
                    finishedGame = false;
                }
                else
                {
                    canAdvance = false;
                    finishedGame = true;
                }
            }
            else
            {
                canAdvance = false;
                finishedGame = false;
            }
        }

        private void UpdateDied(GameTime gameTime)
        {
            levelManager.Update(gameTime);
            if (Input.KeyPressed(Keys.Enter))
            {
                mainGame.RestartLevel();
            }
        }

        private void UpdateWon(GameTime gameTime)
        {
            levelManager.Update(gameTime);
            if (Input.KeyPressed(Keys.Enter))
            {
                string name = DestinationMapName(player.WorldCenter);
                levelManager.LoadLevel(name);
                state = IMPORT_PLATFORM.InGameState.Playing;
            }
        }

        private void UpdateGameOver(GameTime gameTime)
        {
        }

        private void PlayingInput()
        {
            if (Input.KeyPressed(Keys.Enter))
            {
                if (canAdvance && !finishedGame)
                {
                    MusicManager.Instance.PlayEffect(SFXType.WonLevel);
                    state = IMPORT_PLATFORM.InGameState.Won;
                }
                if (finishedGame && !canAdvance)
                {
                    MusicManager.Instance.PlayEffect(SFXType.WonLevel);
                    mainGame.SetState(GameState.FinishedGame);
                }
            }
            if (Input.KeyPressed(Keys.Escape))
            {
                mainGame.SetState(GameState.Paused);
            }
        }

        #endregion

        public  void Draw(SpriteBatch spriteBatch)
        {
            switch (state)
            {
                case InGameState.Playing:
                    DrawPlaying(spriteBatch);
                    break;
                case InGameState.Died:
                    DrawDied(spriteBatch);
                    break;
                case InGameState.GameOver:
                    DrawGameOver(spriteBatch);
                    break;
                case InGameState.Won:
                    DrawWon(spriteBatch);
                    break;
            }
        }

        #region Draw Helper Methods

        private void DrawPlaying(SpriteBatch spriteBatch)
        {
            TileMap.Draw(spriteBatch);
            player.Draw(spriteBatch);
            levelManager.Draw(spriteBatch);
            DrawStates(spriteBatch);
            if (canAdvance && !finishedGame)
            {
                Vector2 loc = Extensions.AllignThing(mainGame.StateFont.MeasureString(canMoveText), TextLocation.Center, mainGame.Window.ClientBounds);

                spriteBatch.DrawString(mainGame.StateFont, canMoveText, loc, Color.White);
            }
            if (finishedGame && !canAdvance)
            {
                string finishedText = "Press Enter To Finish The Game\nGongratulations...";
                Vector2 loc = Extensions.AllignThing(mainGame.StateFont.MeasureString(finishedText), TextLocation.Center, mainGame.Window.ClientBounds);

                spriteBatch.DrawString(mainGame.StateFont, finishedText, loc, Color.White);
            }
        }

        private void DrawDied(SpriteBatch spriteBatch)
        {
            TileMap.Draw(spriteBatch);
            levelManager.Draw(spriteBatch);

            Vector2 loc = Extensions.AllignThing(mainGame.StateFont.MeasureString(diedText), TextLocation.Center, mainGame.Window.ClientBounds);

            spriteBatch.DrawString(mainGame.StateFont, diedText, loc, Color.White);
        }

        private void DrawWon(SpriteBatch spriteBatch)
        {
            TileMap.Draw(spriteBatch);
            levelManager.Draw(spriteBatch);

            Vector2 loc = Extensions.AllignThing(mainGame.StateFont.MeasureString(wonLevelText), TextLocation.Center, mainGame.Window.ClientBounds);

            spriteBatch.DrawString(mainGame.StateFont, wonLevelText, loc, Color.White);
        }

        private void DrawGameOver(SpriteBatch spriteBatch)
        {
        }

        #endregion

        #endregion

        #region Helper Methods

        private string DestinationMapName(Vector2 tileLocation)
        {
            string code = TileMap.CellCodeValue(TileMap.GetCellByPixel(tileLocation));
            if (code.StartsWith("To"))
            {
                string[] codes = code.Split(' ');
                string mapName = codes[1];
                return mapName;
            }
            else
            {
                return "";
            }
        }

        private void DrawStates(SpriteBatch spriteBatch)
        {
            states.Clear();
            states.AppendLine("Score : " + player.Score);
            states.AppendLine("Lives : " + player.LivesRemaining);
            states.AppendLine("Deaths : " + player.DeathsRemaining);
            states.AppendLine("Game Time " + mainGame.TotalTime.Minutes + " : " + mainGame.TotalTime.Seconds);

#if DEBUG
            states.AppendLine("OnGround : " + player.OnGround);
            states.AppendLine("Loc X = " + player.WorldLocation.X + " ");
            states.AppendLine("Loc Y = " + player.WorldLocation.Y);
            states.AppendLine("Speed X = " + player.Speed.X + " ");
            states.AppendLine("Speed Y = " + player.Speed.Y);
#endif

            spriteBatch.DrawString(mainGame.StateFont, states.ToString(), Vector2.Zero, Color.White);
        }

        #endregion
    }
}
