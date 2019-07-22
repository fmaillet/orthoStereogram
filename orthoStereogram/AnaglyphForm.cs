using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace orthoStereogram
{
    public partial class AnaglyphForm : Form
    {
        static int stereogramSize = 800;
        static Bitmap odBitmap = new Bitmap(stereogramSize, stereogramSize);
        static Bitmap ogBitmap = new Bitmap(stereogramSize, stereogramSize);
        public Bitmap eyeBitmap;

        static int ecartInitial = 10;
        static Point odLocation, ogLocation;


        public AnaglyphForm()
        {
            

            InitializeComponent();
            //passer en full view
            this.WindowState = FormWindowState.Normal;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Bounds = Screen.PrimaryScreen.Bounds;

            //odBitmap = new Bitmap(stereogramSize, stereogramSize);
            //ogBitmap = new Bitmap(stereogramSize, stereogramSize);
            eyeBitmap = new Bitmap(stereogramSize, stereogramSize);

            odLocation = new Point((this.Size.Width / 2) + ecartInitial, (this.Size.Height / 2) - (stereogramSize / 2));
            ogLocation = new Point((this.Size.Width / 2) - ecartInitial - stereogramSize, (this.Size.Height / 2) - (stereogramSize / 2));
        }

        //Key pressed event
        private void AnaglyphForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                this.Close();
            }
            
        }

        //Anaglyph form is shown
        private void AnaglyphForm_Shown(object sender, EventArgs e)
        {
            int disparity = 20;
            int bord = 20;
            int alpha = 180;
            
            int stereoSize = stereogramSize / 2 - 2 * bord;

            Random rnd = new Random();
            for (int i=0; i< stereogramSize; i++)
                for (int j=0; j< stereogramSize; j++)
                {
                    if (rnd.Next(2) == 1)
                    {
                        odBitmap.SetPixel(i, j, Color.FromArgb(alpha, Color.Red));
                        ogBitmap.SetPixel(i, j, Color.FromArgb(alpha, Color.Cyan));
                    }
                    else
                    {
                        odBitmap.SetPixel(i, j, Color.FromArgb(alpha, Color.White));
                        ogBitmap.SetPixel(i, j, Color.FromArgb(alpha, Color.White));
                    }
                }

            //Draw stereogram
            for (int i = 0; i < stereoSize; i++)
                for (int j = 0; j < stereoSize; j++)
                {
                    if (rnd.Next(2) == 1)
                    {
                        odBitmap.SetPixel(stereogramSize / 2 - stereoSize / 2 + i + disparity, bord + j, Color.FromArgb(alpha, Color.Red));
                        ogBitmap.SetPixel(stereogramSize / 2 - stereoSize / 2 + i - disparity, bord + j, Color.FromArgb(alpha, Color.Cyan));
                    }
                    else
                    {
                        odBitmap.SetPixel(stereogramSize / 2 - stereoSize / 2 + i + disparity, bord + j, Color.FromArgb(alpha, Color.White));
                        ogBitmap.SetPixel(stereogramSize / 2 - stereoSize / 2 + i - disparity, bord + j, Color.FromArgb(alpha, Color.White));
                    }
                }

            //odBitmap.MakeTransparent(Color.White);
            //ogBitmap.MakeTransparent(Color.White);

            
        }

        private void AnaglyphForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right)
            {
                odLocation = new Point(odLocation.X + 1, odLocation.Y);
                ogLocation = new Point(ogLocation.X - 1, ogLocation.Y);
            }
            if (e.KeyCode == Keys.Left)
            {
                odLocation = new Point(odLocation.X - 1, odLocation.Y);
                ogLocation = new Point(ogLocation.X + 1, ogLocation.Y);
            }
            this.Refresh();
        }

        

        private void AnaglyphForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.CompositingMode = CompositingMode.SourceOver;
            g.DrawImageUnscaled(ogBitmap, ogLocation);
            g.CompositingMode = CompositingMode.SourceOver;
            g.DrawImageUnscaled(odBitmap, odLocation);

            //int recouvrement = odLocation.X - ogLocation.X;
            //if (recouvrement < stereogramSize)
            //{
            //    Bitmap rec = new Bitmap(stereogramSize - recouvrement, stereogramSize);

            //    for (int i = 0; i < stereogramSize - recouvrement; i++)
            //        for (int j = 0; j < stereogramSize; j++)
            //        {
            //            //int red   = ogBitmap.GetPixel(i, j).R;
            //            //int green = odBitmap.GetPixel(i, j).G;
            //            //int blue  = odBitmap.GetPixel(i, j).B;
            //            rec.SetPixel(i, j, Color.FromArgb(ogBitmap.GetPixel(recouvrement+i, j).R, odBitmap.GetPixel(i, j).G, odBitmap.GetPixel(i, j).B));
            //        }
            //    g.DrawImageUnscaled(rec, odLocation);
                
            //}
            
        }

        

        

    }
}
