using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MineSweeper
{
    public partial class FrmMain : Form
    {
        public const int Size = 20;

        public int W, H, TotalMine;
        public bool[,] Map, Show, Flag;
        public int[,] Num;

        public bool LeftDown, RightDown;

        public bool GameOver, Started, Win;

        public Random Rnd = new Random();

        public int Time;

        public void NewGame()
        {
            Show = new bool[W, H];
            Flag = new bool[W, H];
            GameOver = false;
            Started = false;
            Win = false;
            LeftDown = RightDown = false;
            timer1.Stop();
            Time = 0;
            ShowTime();
            toolStripStatusLabel3.Text = W.ToString() + "x" + H.ToString();
            //resize window
            Pic.Left = 0;
            Pic.Top = menuStrip1.Height;
            Pic.Width = W * Size;
            Pic.Height = H * Size + statusStrip1.Height;
            Pic.Invalidate();
        }

        private void GenMap(int EX, int EY)
        {
            Map = new bool[W, H];
            int[] EmptyX, EmptyY;
            EmptyX = new int[W * H];
            EmptyY = new int[W * H];
            int EmptyCnt = 0;
            for (int i = 0; i < W; i++)
            {
                for (int j = 0; j < H; j++)
                {
                    if (!(i == EX && j == EY))
                    {
                        EmptyX[EmptyCnt] = i;
                        EmptyY[EmptyCnt] = j;
                        EmptyCnt++;
                    }
                }
            }
            for (int c = 0; c < TotalMine; c++)
            {
                int R = Rnd.Next(EmptyCnt);
                Map[EmptyX[R], EmptyY[R]] = true;
                EmptyCnt--;
                for (int i = R; i < EmptyCnt; i++)
                {
                    EmptyX[i] = EmptyX[i + 1];
                    EmptyY[i] = EmptyY[i + 1];
                }
            }
            Num = new int[W, H];
            for (int i = 0; i < W; i++)
            {
                for (int j = 0; j < H; j++)
                {
                    for (int ii = -1; ii <= 1; ii++)
                    {
                        for (int jj = -1; jj <= 1; jj++)
                        {
                            if (inrange(i + ii, j + jj) && Map[i + ii, j + jj]) Num[i, j]++;
                        }
                    }
                }
            }
        }

        private bool inrange(int x, int y)
        {
            return x >= 0 && x < W && y >= 0 && y < H;
        }

        public FrmMain()
        {
            InitializeComponent();
        }

        private void Pic_Click(object sender, EventArgs e)
        {

        }

        private void DrawNum(int n, int i, int j, Graphics g, Color c)
        {
            int[,] Rect = new int[7,4] { { 6, 3, 8, 2 }, { 6, 3, 2, 8 }, { 12, 3, 2, 8 }, { 6, 9, 8, 2 }, { 6, 9, 2, 8 }, { 12, 9, 2, 8 }, { 6, 15, 8, 2 } };
            int[,] Num = new int[8, 7] { { 0, 0, 1, 0, 0, 1, 0 }, { 1, 0, 1, 1, 1, 0, 1 }, { 1, 0, 1, 1, 0, 1, 1 }, { 0, 1, 1, 1, 0, 1, 0 }, { 1, 1, 0, 1, 0, 1, 1 }, { 1, 1, 0, 1, 1, 1, 1 }, { 1, 0, 1, 0, 0, 1, 0 }, { 1, 1, 1, 1, 1, 1, 1 } };
            for (int k = 0; k < 7; k++)
            {
                if (Num[n - 1, k] == 1)
                {
                    g.FillRectangle(new SolidBrush(c), i * Size + Rect[k, 0], j * Size + Rect[k, 1], Rect[k, 2], Rect[k, 3]);
                }
            }
        }

        private void FillRect(int i, int j, Graphics g, Color c)
        {
            g.FillRectangle(new SolidBrush(c), i * Size + 1, j * Size + 1, Size - 2, Size - 2);
        }

        private void Pic_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.White);
            for (int i = 0; i < W; i++)
            {
                for (int j = 0; j < H; j++)
                {
                    g.DrawRectangle(new Pen(Color.Black), i * Size, j * Size, Size - 1, Size - 1);
                    if (!Show[i, j])
                    {
                        if (Flag[i, j])
                        {
                            FillRect(i, j, g, Color.Red);
                        }
                        else
                        {
                            FillRect(i, j, g, Color.Gray);
                        }
                    }
                    else
                    {
                        if (Map[i, j])
                        {
                            FillRect(i, j, g, Color.Black);
                        }
                        else
                        {
                            if (Num[i, j] > 0)
                            {
                                DrawNum(Num[i, j], i, j, g, Color.Blue);
                            }
                        }
                    }
                }
            }
            if (GameOver)
            {
                if (Win)
                {
                    toolStripStatusLabel1.Text = "Clear!";
                }
                else
                {
                    toolStripStatusLabel1.Text = "Game over";
                }
            }
            else
            {
                int MineLeft = TotalMine;
                for (int i = 0; i < W; i++)
                {
                    for (int j = 0; j < H; j++)
                    {
                        if (!Show[i, j] && Flag[i, j]) MineLeft--;
                    }
                }
                toolStripStatusLabel1.Text = MineLeft.ToString() + "/" + TotalMine.ToString() + " mines left";
            }
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            W = 30;
            H = 16;
            TotalMine = 99;
            NewGame();
        }

        private void Click(int x, int y)
        {
            if (inrange(x, y) && !Show[x, y] && !Flag[x, y])
            {
                if (!Started)
                {
                    GenMap(x, y);
                    Started = true;
                    Time = 0;
                    ShowTime();
                    timer1.Start();
                    Click(x, y);
                }
                else if (Map[x, y])//it is mine!
                {
                    for (int i = 0; i < W; i++)
                    {
                        for (int j = 0; j < H; j++)
                        {
                            Show[i, j] = true;
                        }
                    }
                    GameOver = true;
                    timer1.Stop();
                    Pic.Invalidate();
                    MessageBox.Show("You lose!");
                }
                else
                {
                    Show[x, y] = true;
                    if (Num[x, y] == 0)
                    {
                        for (int i = -1; i <= 1; i++)
                        {
                            for (int j = -1; j <= 1; j++)
                            {
                                Click(x + i, y + j);
                            }
                        }
                    }
                    if (!GameOver) JudgeWin();
                }
            }
        }

        private void JudgeWin()
        {
            Win = true;
            for (int i = 0; i < W; i++)
            {
                for (int j = 0; j < H; j++)
                {
                    if (!Map[i, j] && !Show[i, j]) Win = false;
                }
            }
            if (Win)
            {
                GameOver = true;
                timer1.Stop();
                Pic.Invalidate();
                MessageBox.Show("You win within " + Time.ToString() + " second" + (Time > 1 ? "s" : "") + "!");
            }
        }

        private void Pic_MouseDown(object sender, MouseEventArgs e)
        {
            if (GameOver) return;
            if (e.Button == MouseButtons.Left)
            {
                LeftDown = true;
            }
            else if (e.Button == MouseButtons.Right)
            {
                RightDown = true;
            }
        }

        private void Pic_MouseUp(object sender, MouseEventArgs e)
        {
            if (GameOver) return;
            int x = e.X / Size, y = e.Y / Size;
            if (inrange(x, y))
            {
                if (e.Button == MouseButtons.Left && RightDown || e.Button == MouseButtons.Right && LeftDown || e.Button == MouseButtons.Middle)
                {
                    LeftDown = RightDown = false;
                    if (Show[x, y])
                    {
                        int FlagCount = 0;
                        for (int i = x - 1; i <= x + 1; i++)
                        {
                            for (int j = y - 1; j <= y + 1; j++)
                            {
                                if (inrange(i, j) && !Show[i, j] && Flag[i, j])
                                {
                                    FlagCount++;
                                }
                            }
                        }
                        if (FlagCount == Num[x, y])
                        {
                            for (int i = x - 1; i <= x + 1; i++)
                            {
                                for (int j = y - 1; j <= y + 1; j++)
                                {
                                    if (inrange(i, j) && !Show[i, j] && !Flag[i, j])
                                    {
                                        Click(i, j);
                                    }
                                }
                            }
                        }
                    }
                }
                else if (e.Button == MouseButtons.Left)
                {
                    LeftDown = false;
                    Click(x, y);
                }
                else if (e.Button == MouseButtons.Right)
                {
                    RightDown = false;
                    Flag[x, y] = !Flag[x, y];
                }
            }
            Pic.Invalidate();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            NewGame();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Programmed by 负一的平方根\n2015/1/29");
        }

        private void smallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            W = 9;
            H = 9;
            TotalMine = 10;
            NewGame();
        }

        private void meduimToolStripMenuItem_Click(object sender, EventArgs e)
        {
            W = 16;
            H = 16;
            TotalMine = 40;
            NewGame();
        }

        private void largeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            W = 30;
            H = 16;
            TotalMine = 99;
            NewGame();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Time++;
            ShowTime();
        }

        private void ShowTime()
        {
            toolStripStatusLabel2.Text = "Time: " + Time.ToString();
        }

        private void customizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmSettings Frm = new FrmSettings();
            Frm.ShowDialog(this);
        }

        private void FrmMain_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                NewGame();
            }
        }
    }
}
