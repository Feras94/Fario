﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Tile_Engine;
using System.Threading;

namespace Level_Editor
{
    public partial class TilesViewer : Form
    {
        int maxTilesinTemp = 8;
        private Stopwatch totalTime = new Stopwatch();

        public TilesViewer()
        {
            InitializeComponent();
        }

        private void LoadImageList()
        {
            totalTime.Reset();
            totalTime.Start();
            MapEditor.canExit = false;
            string filePath = Application.StartupPath + @"\Content\Textures\Tileset.png";
            Bitmap tileSheet = new Bitmap(filePath);
            int tileCount = 0;
            int TileNumber = 0;
            MapEditor.proMax = (tileSheet.Width / TileMap.TileWidth) * (tileSheet.Height / TileMap.TileHeight);
            MapEditor.proValue = 0;
            for (int y = 0; y < tileSheet.Height / TileMap.TileHeight; y++)
            {
                for (int x = 0; x < tileSheet.Width / TileMap.TileWidth; x++)
                {
                    Bitmap newBitmap = tileSheet.Clone(
                        new System.Drawing.Rectangle(
                            (x * TileMap.TileWidth),
                            (y * TileMap.TileHeight),
                            TileMap.TileWidth,
                            TileMap.TileHeight),
                        System.Drawing.Imaging.PixelFormat.DontCare);

                    this.imgListTiles.Images.Add(newBitmap);

                    string itemName = TileNumber.ToString();
                    this.listTiles.Items.Add(new ListViewItem(itemName, tileCount++));

                    TileNumber++;
                    MapEditor.proValue++;
                }
                Application.DoEvents();
            }
            MapEditor.proValue = 0;
            MapEditor.canExit = true;
            totalTime.Stop();
            Console.WriteLine("Total Time Taken : " + totalTime.Elapsed.TotalSeconds);
        }        

        private void TilesViewer_Load(object sender, EventArgs e)
        {
            LoadImageList();
        }

        private void listTiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listTiles.SelectedIndices.Count > 0)
            {
                Game1.DrawTile = listTiles.SelectedIndices[0];
                foreach (ListViewItem tile in listTiles.SelectedItems)
                {
                    listTemp.Items.Add((ListViewItem)tile.Clone());
                }
                if (listTemp.Items.Count > maxTilesinTemp)
                {
                    listTemp.Items.RemoveAt(0);
                }
            }
        }

        private void listTemp_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listTemp.SelectedIndices.Count > 0)
            {
                foreach (ListViewItem tile in listTemp.SelectedItems)
                {
                    Game1.DrawTile = int.Parse(tile.Text);
                }
            }
        }

        private void TilesViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            return;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = checkBox1.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listTemp.Clear();
        }        
    }
}