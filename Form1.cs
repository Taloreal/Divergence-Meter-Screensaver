using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace ScreenSaver
{
    public partial class Form1 : Form
    {
        bool _renderDispose = true;
        BufferedGraphicsContext graphicsContext;
        BufferedGraphics graphics;
        Thread RenderThread;

        private int mXStart = 0; // 마우스 포인트 가로 좌표
        private int mYStart = 0; // 마우스 포인트 세로 좌표

        bool smallMode = false;

        public Form1()
        {
            InitializeComponent();
            Cursor.Hide();
            ShowScreenSaver();
        }

        Point BasedSize = new Point(800, 600);
        Point CanvaseSize;
        Rectangle fullScreen;
        private void ShowScreenSaver()
        {
            fullScreen = Screen.PrimaryScreen.Bounds;

            if (smallMode)
            {
                imageSize = new Point(126, 322);
                float t1 = fullScreen.Height / BasedSize.Y;
                float t2 = BasedSize.X * t1;
                float t3 = fullScreen.Width - t2;
                CanvaseSize = new Point(Convert.ToInt32(t3), fullScreen.Height);
            }
            else
            {
                imageSize = new Point(180, 460);
                CanvaseSize = new Point(fullScreen.Width, fullScreen.Height);
            }

            graphicsContext = BufferedGraphicsManager.Current;
            graphicsContext.MaximumBuffer = new Size(Width + 1, Height + Screen.PrimaryScreen.Bounds.Height - Screen.PrimaryScreen.WorkingArea.Height + 1);
            graphics = graphicsContext.Allocate(this.CreateGraphics(), new Rectangle((fullScreen.Width - CanvaseSize.X) / 2, 0, CanvaseSize.X, CanvaseSize.Y));

            RenderThread = new Thread(RenderMethod);
            RenderThread.Start();
        }

        Point HotSpot;
        Point imageSize;
        int Space = 10;
        int index = 8;

        internal Rectangle getCoordinate(Rectangle point, int X, int Y, int Width, int Height)
        {
            Rectangle temp = new Rectangle(point.Left * Width / X, point.Top * Height / Y, point.Right * Width / X, point.Bottom * Height / Y);
            return temp;
        }

        private void RenderMethod()
        {
            if (smallMode)
                HotSpot = new Point((fullScreen.Width - CanvaseSize.X) / 2, CanvaseSize.Y / 2 - imageSize.Y / 2);
            else
                HotSpot = new Point(CanvaseSize.X / 2 - imageSize.X * index / 2, CanvaseSize.Y / 2 - imageSize.Y / 2);

            while (_renderDispose)
            {
                int Count = 0;
                graphics.Graphics.Clear(Color.Black);
                DrawDivergenceMeter('0', new Point(HotSpot.X + ((imageSize.X + Space) * Count++), HotSpot.Y));
                DrawDot(ref Count);
                DrawHour(ref Count);
                DrawMinute(ref Count);
                DrawSecond(ref Count);
                //DrawMSecond(ref Count);

                graphics.Render(this.CreateGraphics());
            }
            Application.Exit();
        }
        bool minuteChanged;
        bool SecondChanged;

        bool minuteTrigger;
        bool hourTrigger;
        bool secondTrigger;

        private void DrawDot(ref int Count)
        {
            DrawDivergenceMeter(new Point(HotSpot.X + ((imageSize.X + Space) * Count++), HotSpot.Y));
        }

        private void DrawYear(ref int Count)
        {
            DrawDivergenceMeter((DateTime.Now.Year.ToString()[0]), new Point(HotSpot.X + ((imageSize.X + Space) * Count++), HotSpot.Y));
            DrawDivergenceMeter((DateTime.Now.Year.ToString()[1]), new Point(HotSpot.X + ((imageSize.X + Space) * Count++), HotSpot.Y));
            DrawDivergenceMeter((DateTime.Now.Year.ToString()[2]), new Point(HotSpot.X + ((imageSize.X + Space) * Count++), HotSpot.Y));
            DrawDivergenceMeter((DateTime.Now.Year.ToString()[3]), new Point(HotSpot.X + ((imageSize.X + Space) * Count++), HotSpot.Y));
        }

        private void DrawMonth(ref int Count)
        {
            if (DateTime.Now.Month < 10)
            {
                DrawDivergenceMeter('0', new Point(HotSpot.X + ((imageSize.X + Space) * Count++), HotSpot.Y));
                DrawDivergenceMeter((DateTime.Now.Month.ToString()[0]), new Point(HotSpot.X + ((imageSize.X + Space) * Count++), HotSpot.Y));
            }
            else
            {
                DrawDivergenceMeter((DateTime.Now.Month.ToString()[0]), new Point(HotSpot.X + ((imageSize.X + Space) * Count++), HotSpot.Y));
                DrawDivergenceMeter((DateTime.Now.Month.ToString()[1]), new Point(HotSpot.X + ((imageSize.X + Space) * Count++), HotSpot.Y));
            }
        }
        private void DrawDay(ref int Count)
        {
            if (DateTime.Now.Day < 10)
            {
                DrawDivergenceMeter('0', new Point(HotSpot.X + ((imageSize.X + Space) * Count++), HotSpot.Y));
                DrawDivergenceMeter((DateTime.Now.Day.ToString()[0]), new Point(HotSpot.X + ((imageSize.X + Space) * Count++), HotSpot.Y));
            }
            else
            {
                DrawDivergenceMeter((DateTime.Now.Day.ToString()[0]), new Point(HotSpot.X + ((imageSize.X + Space) * Count++), HotSpot.Y));
                DrawDivergenceMeter((DateTime.Now.Day.ToString()[1]), new Point(HotSpot.X + ((imageSize.X + Space) * Count++), HotSpot.Y));
            }
        }

        private void DrawHour(ref int Count)
        {
            string mHour = DateTime.Now.Hour.ToString();
            if (true)
            {
                if (int.Parse(mHour) < 10)
                {
                    DrawDivergenceMeter('0', new Point(HotSpot.X + ((imageSize.X + Space) * Count++), HotSpot.Y));
                    DrawDivergenceMeter((mHour[0]), new Point(HotSpot.X + ((imageSize.X + Space) * Count++), HotSpot.Y));
                }
                else
                {
                    DrawDivergenceMeter((mHour[0]), new Point(HotSpot.X + ((imageSize.X + Space) * Count++), HotSpot.Y));
                    DrawDivergenceMeter((mHour[1]), new Point(HotSpot.X + ((imageSize.X + Space) * Count++), HotSpot.Y));
                }
                hourTrigger = false;
            }
        }
        private void DrawMinute(ref int Count)
        {
            string mMin = DateTime.Now.Minute.ToString();
            if (!minuteChanged)
            {
                if (int.Parse(mMin) < 10)
                {
                    DrawDivergenceMeter('0', new Point(HotSpot.X + ((imageSize.X + Space) * Count++), HotSpot.Y));
                    DrawDivergenceMeter((mMin[0]), new Point(HotSpot.X + ((imageSize.X + Space) * Count++), HotSpot.Y));
                }
                else
                {
                    DrawDivergenceMeter((mMin[0]), new Point(HotSpot.X + ((imageSize.X + Space) * Count++), HotSpot.Y));
                    DrawDivergenceMeter((mMin[1]), new Point(HotSpot.X + ((imageSize.X + Space) * Count++), HotSpot.Y));
                }
            }
            else
            {
                Random rNum = new Random();

                DrawDivergenceMeter(Convert.ToChar(rNum.Next(10) + '0'), new Point(HotSpot.X + ((imageSize.X + Space) * Count++), HotSpot.Y));
                DrawDivergenceMeter(Convert.ToChar(rNum.Next(10) + '0'), new Point(HotSpot.X + ((imageSize.X + Space) * Count++), HotSpot.Y));

                if (DateTime.Now.Millisecond > 300)
                {
                    minuteChanged = false;
                }

            }
            if (DateTime.Now.Minute == 0 && hourTrigger == false)
            {
                hourTrigger = true;
            }
            if(DateTime.Now.Minute == 1)
            {
                hourTrigger = false;
            }
        }
        private void DrawSecond(ref int Count)
        {
            string mSec = DateTime.Now.Second.ToString();
            if (!SecondChanged)
            {
                if (int.Parse(mSec) < 10)
                {
                    DrawDivergenceMeter('0', new Point(HotSpot.X + ((imageSize.X + Space) * Count++), HotSpot.Y));
                    DrawDivergenceMeter((mSec[0]), new Point(HotSpot.X + ((imageSize.X + Space) * Count++), HotSpot.Y));
                }
                else
                {
                    DrawDivergenceMeter((mSec[0]), new Point(HotSpot.X + ((imageSize.X + Space) * Count++), HotSpot.Y));
                    DrawDivergenceMeter((mSec[1]), new Point(HotSpot.X + ((imageSize.X + Space) * Count++), HotSpot.Y));
                }

            }
            else
            {
                Random rNum = new Random();

                DrawDivergenceMeter(Convert.ToChar(rNum.Next(10)+'0'), new Point(HotSpot.X + ((imageSize.X + Space) * Count++), HotSpot.Y));
                DrawDivergenceMeter(Convert.ToChar(rNum.Next(10) + '0'), new Point(HotSpot.X + ((imageSize.X + Space) * Count++), HotSpot.Y));

                if (DateTime.Now.Millisecond < 200)
                    SecondChanged = false;

            }
            if (DateTime.Now.Millisecond < 100 && SecondChanged == false)
            {
                SecondChanged = true;
            }
            if (DateTime.Now.Second == 00 && minuteTrigger == false)
            {
                minuteChanged = true;
                minuteTrigger = true;
            }
            if(DateTime.Now.Second == 1)
            {
                minuteTrigger = false;
            }
        }
        private void DrawMSecond(ref int Count)
        {
            string mSec = DateTime.Now.Millisecond.ToString();
            if (int.Parse(mSec) < 10)
            {
                DrawDivergenceMeter('0', new Point(HotSpot.X + ((imageSize.X + Space) * Count++), HotSpot.Y));
                DrawDivergenceMeter('0', new Point(HotSpot.X + ((imageSize.X + Space) * Count++), HotSpot.Y));
                DrawDivergenceMeter((mSec[0]), new Point(HotSpot.X + ((imageSize.X + Space) * Count++), HotSpot.Y));
            }
            else if (int.Parse(mSec) < 100)
            {
                DrawDivergenceMeter('0', new Point(HotSpot.X + ((imageSize.X + Space) * Count++), HotSpot.Y));
                DrawDivergenceMeter((mSec[0]), new Point(HotSpot.X + ((imageSize.X + Space) * Count++), HotSpot.Y));
                DrawDivergenceMeter((mSec[1]), new Point(HotSpot.X + ((imageSize.X + Space) * Count++), HotSpot.Y));
            }
            else
            {

                DrawDivergenceMeter((mSec[0]), new Point(HotSpot.X + ((imageSize.X + Space) * Count++), HotSpot.Y));
                DrawDivergenceMeter((mSec[1]), new Point(HotSpot.X + ((imageSize.X + Space) * Count++), HotSpot.Y));
                DrawDivergenceMeter((mSec[2]), new Point(HotSpot.X + ((imageSize.X + Space) * Count++), HotSpot.Y));
            }
        }

        private void DrawDivergenceMeter(char num, Point pos)
        {
            Rectangle size = getCoordinate(new Rectangle(pos.X, pos.Y, imageSize.X, imageSize.Y), Width, Height, Width, Height);

            var delta = pos;

            if (smallMode)
            {
                switch (num)
                {
                    case '0':
                        graphics.Graphics.DrawImage(new Bitmap(Properties.Resources.s0), delta);
                        break;
                    case '1':
                        graphics.Graphics.DrawImage(new Bitmap(Properties.Resources.s1), delta);
                        break;
                    case '2':
                        graphics.Graphics.DrawImage(new Bitmap(Properties.Resources.s2), delta);
                        break;
                    case '3':
                        graphics.Graphics.DrawImage(new Bitmap(Properties.Resources.s3), delta);
                        break;
                    case '4':
                        graphics.Graphics.DrawImage(new Bitmap(Properties.Resources.s4), delta);
                        break;
                    case '5':
                        graphics.Graphics.DrawImage(new Bitmap(Properties.Resources.s5), delta);
                        break;
                    case '6':
                        graphics.Graphics.DrawImage(new Bitmap(Properties.Resources.s6), delta);
                        break;
                    case '7':
                        graphics.Graphics.DrawImage(new Bitmap(Properties.Resources.s7), delta);
                        break;
                    case '8':
                        graphics.Graphics.DrawImage(new Bitmap(Properties.Resources.s8), delta);
                        break;
                    case '9':
                        graphics.Graphics.DrawImage(new Bitmap(Properties.Resources.s9), delta);
                        break;
                }
            }
            else
            {

                switch (num)
                {
                    case '0':
                        graphics.Graphics.DrawImage(new Bitmap(Properties.Resources._0), delta);
                        break;
                    case '1':
                        graphics.Graphics.DrawImage(new Bitmap(Properties.Resources._1), delta);
                        break;
                    case '2':
                        graphics.Graphics.DrawImage(new Bitmap(Properties.Resources._2), delta);
                        break;
                    case '3':
                        graphics.Graphics.DrawImage(new Bitmap(Properties.Resources._3), delta);
                        break;
                    case '4':
                        graphics.Graphics.DrawImage(new Bitmap(Properties.Resources._4), delta);
                        break;
                    case '5':
                        graphics.Graphics.DrawImage(new Bitmap(Properties.Resources._5), delta);
                        break;
                    case '6':
                        graphics.Graphics.DrawImage(new Bitmap(Properties.Resources._6), delta);
                        break;
                    case '7':
                        graphics.Graphics.DrawImage(new Bitmap(Properties.Resources._7), delta);
                        break;
                    case '8':
                        graphics.Graphics.DrawImage(new Bitmap(Properties.Resources._8), delta);
                        break;
                    case '9':
                        graphics.Graphics.DrawImage(new Bitmap(Properties.Resources._9), delta);
                        break;
                }
            }
        }

        private void DrawDivergenceMeter(Point pos)
        {
            if (smallMode) graphics.Graphics.DrawImage(new Bitmap(Properties.Resources.s_), pos);
            else graphics.Graphics.DrawImage(new Bitmap(Properties.Resources._), pos);
        }

        private void StopScreenSaver()
        {
            _renderDispose = false;
            Cursor.Show();
            timer.Enabled = false;
        }

        private void timer_Tick(object sender, EventArgs e)
        {

        }

        private void MouseMoveEvent(MouseEventArgs e)
        {
            if ((mXStart == 0) && (mYStart == 0))
            {
                mXStart = e.X;
                mYStart = e.Y;
                return;
            }
            else if ((e.X != mXStart) || (e.Y != mYStart))
            {
                StopScreenSaver();
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            MouseMoveEvent(e);
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {

            StopScreenSaver();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            StopScreenSaver();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

    }
}
