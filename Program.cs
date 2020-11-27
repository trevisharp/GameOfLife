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
float width = 0, height = 0;

form.PreviewKeyDown += delegate (object sender, PreviewKeyDownEventArgs e)
{
    onkey(e.KeyCode);
};
form.Load += delegate
{
    width = pb.Width;
    height = pb.Height;
    Bitmap bmp = new Bitmap((int)width, (int)height);
    Graphics g = Graphics.FromImage(bmp);
    draw = f =>
    {
        f(g);
        pb.Image = bmp;
    };
    Timer t = new Timer();
    t.Interval = 50;
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
int opt = 0;
float square = 0;
int speed = 100;
int accumulation = 0;

string optinfo = string.Empty;

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
                    g.FillRectangle(Brushes.White, 10 + i * square, 10 + j * square, square, square);
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
    if (opt > 0)
    {
        draw(g =>
        {
            g.Clear(Color.Black);
            g.DrawString(optinfo, form.Font, Brushes.White, PointF.Empty);
        });
    }
    else if (started)
    {
        accumulation += speed;
        if (accumulation > 99)
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
                        g.FillRectangle(Brushes.White, 10 + i * square, 10 + j * square, square, square);
                    g.DrawRectangle(Pens.White, 10 + i * square, 10 + j * square, square, square);
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
            if (opt == 0)
            {
                started = !started;
                drawgame();
                accumulation = 0;
            }
            else
            {
                switch (opt)
                {
                    case 1:
                        int sizew = int.Parse(optinfo.Substring(7));
                        if (sizew < 4 || sizew > 1000)
                            break;
                        started = false;
                        game = new Game(sizew, game.Height);
                        square = (width - 20) / game.Width > (height - 20) / game.Height
                            ? (height - 20) / game.Height : (width - 20) / game.Width;
                    break;
                    case 2:
                        int sizeh = int.Parse(optinfo.Substring(8));
                        if (sizeh < 4 || sizeh > 1000)
                            break;
                        started = false;
                        game = new Game(game.Width, sizeh);
                        square = (width - 20) / game.Width > (height - 20) / game.Height
                            ? (height - 20) / game.Height : (width - 20) / game.Width;
                    break;
                    case 3:
                        int nspeed = int.Parse(optinfo.Substring(7));
                        if (nspeed < 1)
                            break;
                        speed = nspeed;
                    break;
                }
                opt = 0;
            }
            break;
        case Keys.Escape:
            Application.Exit();
            break;
        case Keys.Back:
            started = false;
            opt = 0;
            game = new Game(game.Width, game.Height);
            break;
        case Keys.W:
            optinfo = "width: ";
            opt = 1;
            break;
        case Keys.H:
            optinfo = "height: ";
            opt = 2;
            break;
        case Keys.S:
            optinfo = "speed: ";
            opt = 3;
            break;
        case Keys.D0:
        case Keys.D1:
        case Keys.D2:
        case Keys.D3:
        case Keys.D4:
        case Keys.D5:
        case Keys.D6:
        case Keys.D7:
        case Keys.D8:
        case Keys.D9:
            optinfo += (char)key;
        break;
    }
};

pb.MouseDown += delegate (object sender, MouseEventArgs e)
{
    if (square < 1)
        return;
    int i = (int)((e.Location.X - 10) / square),
        j = (int)((e.Location.Y - 10) / square);
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