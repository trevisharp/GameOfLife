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
form.FormBorderStyle = FormBorderStyle.FixedDialog;
form.Text = "Game of Life";

PictureBox pb = new PictureBox();
pb.Dock = DockStyle.Fill;
form.Controls.Add(pb);

Action<Action<Graphics>> draw = null;
Action onload = null,
       ontick = null;

form.Load += delegate
{
    Bitmap bmp = new Bitmap(pb.Width, pb.Height);
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
};

#endregion

Game game = new Game(10, 10);

onload = delegate
{
    draw(g => g.Clear(Color.White));
};

ontick = delegate
{

};

pb.MouseDown += delegate (object sender, MouseEventArgs e)
{

};

pb.MouseUp += delegate (object sender, MouseEventArgs e)
{

};

pb.MouseMove += delegate (object sender, MouseEventArgs e)
{

};

Application.Run(form);