using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

#region Interface Setup

Application.SetHighDpiMode(HighDpiMode.SystemAware);
Application.EnableVisualStyles();
Application.SetCompatibleTextRenderingDefault(false);

var form = new Form();
form.WindowState = FormWindowState.Maximized;
form.FormBorderStyle = FormBorderStyle.None;
form.KeyPreview = true;
form.Text = "Game of Life";

PictureBox pb = new PictureBox();
pb.Dock = DockStyle.Fill;
form.Controls.Add(pb);

Action<Action<Graphics>> draw = null;
Action onload = null,
       ontick = null;
Action<Keys> onkey = null;
int width = 0, height = 0;

form.PreviewKeyDown += delegate (object sender, PreviewKeyDownEventArgs e)
{
    onkey(e.KeyCode);
};
form.Load += delegate
{
    width = pb.Width;
    height = pb.Height;
    Bitmap bmp = new Bitmap(width, height);
    Graphics g = Graphics.FromImage(bmp);
    draw = f =>
    {
        f(g);
        pb.Image = bmp;
    };
    Timer t = new Timer();
    t.Interval = 100;
    t.Tick += delegate
    {
        ontick();
    };
    onload();
    t.Start();
};

#endregion

Game game = new Game(10, 10);
bool started = false;
int square = 0;
int speed = 10;
int accumulation = 1000;

void drawgame()
{
    draw(g =>
    {
        g.Clear(Color.Black);
        for (int i = 0; i < game.Width; i++)
        {
            for (int j = 0; j < game.Height; j++)
            {
                if (game[i, j])
                    g.FillRectangle(Brushes.White, new Rectangle(
                10 + i * square, 10 + j * square, square, square));
            }
        }
    });
}

onload = delegate
{
    draw(g => g.Clear(Color.White));
    square = (width - 20) / game.Width > (height - 20) / game.Height
            ? (height - 20) / game.Height : (width - 20) / game.Width;
};

ontick = delegate
{
    if (started)
    {
        accumulation += speed;
        if (accumulation > 100)
        {
            accumulation = 0;
            game.RunGeneration();
            drawgame();
        }
    }
    else
    {
        draw(g =>
        {
            g.Clear(Color.Black);
            for (int i = 0; i < game.Width; i++)
            {
                for (int j = 0; j < game.Height; j++)
                {
                    if (game[i, j])
                        g.FillRectangle(Brushes.White, new Rectangle(
                            10 + i * square, 10 + j * square, square, square));
                    g.DrawRectangle(Pens.White, new Rectangle(
                        10 + i * square, 10 + j * square, square, square));
                }
            }
        });
    }
};

onkey += delegate (Keys key)
{
    switch (key)
    {
        case Keys.Enter:
            started = !started;
            drawgame();
            accumulation = 0;
            break;
        case Keys.Escape:
            Application.Exit();
            break;
    }
};

pb.MouseDown += delegate (object sender, MouseEventArgs e)
{
    if (square < 1)
        return;
    int i = (e.Location.X - 10) / square,
        j = (e.Location.Y - 10) / square;
    if (i > -1 && i < game.Width && j > -1 && j < game.Height)
        game[i, j] = !game[i, j];
};

pb.MouseUp += delegate (object sender, MouseEventArgs e)
{

};

pb.MouseMove += delegate (object sender, MouseEventArgs e)
{

};

Application.Run(form);