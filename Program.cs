using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
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
        Action<Keys, bool, bool> onkey = null;
        float width = 0, height = 0;

        form.PreviewKeyDown += delegate (object sender, PreviewKeyDownEventArgs e)
        {
            onkey(e.KeyCode, e.Control, e.Shift);
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

        Game game = null;
        Point p = Point.Empty; //Cursor
        bool started = false;
        float square = 0, zsquare = 0;
        int speed = 100;
        int accumulation = 0;
        Font font = new Font(FontFamily.Families[15], 20f);

        //Selection Variables
        bool mousedown = false, inselection = false;
        int selx = -1, sely = -1, selx2 = -1, sely2 = -1;

        //Zoom Variables
        int x0 = 0, y0 = 0, xzoom = 0, yzoom = 0;
        float zoom = 1.0f;
        Point movepoint = Point.Empty;

        //Options Variables
        string optinfo = string.Empty;
        int opt = 0;

        //Population Graphics
        bool show = false;
        bool autoskip = false;
        int skip = 0;

        void setsquare()
        {
            square = (width - 20) / game.Width > (height - 20) / game.Height
                    ? (height - 20) / game.Height : (width - 20) / game.Width;
            zsquare = (width - 20) / (game.Width - x0 - 2 * xzoom)
                    > (height - 20) / (game.Height - y0 - 2 * yzoom)
                    ? (height - 20) / (game.Height - y0 - 2 * yzoom)
                    : (width - 20) / (game.Width - x0 - 2 * xzoom);
        }

        void treatzoomboundaries()
        {
            if (x0 < - 2 * xzoom)
                x0 = - 2 * xzoom;
            else if (x0 > 2 * xzoom)
                x0 = 2 * xzoom;

            if (y0 < - 2 * yzoom)
                y0 = - 2 * yzoom;
            else if (y0 > 2 * yzoom)
                y0 = 2 * yzoom;
        }

        void drawgame()
        {
            draw(g =>
            {
                g.Clear(Color.Black);

                if (show)
                {
                    int max = game.Register.Max,
                        min = game.Register.Min,
                        delta = max - min;
                    if (delta == 0)
                    {
                        max = max + 2;
                        delta = 2;
                    }
                    float dpx = (height - 20) / delta;

                    float x = 10, y = 10, ny;
                    foreach (var pop in game.Register.Skip(skip))
                    {
                        ny = dpx * (pop - min) + 10;
                        g.DrawLine(Pens.Red, x, height - y, x + 5, height - ny);
                        y = ny;
                        x += 5;
                    }

                    g.DrawLine(Pens.LimeGreen, new PointF(10, 10), new PointF(10, height - 10));
                    g.DrawLine(Pens.LimeGreen, new PointF(10, height - 10), new PointF(width - 10, height - 10));
                    g.DrawString(max.ToString(), font, Brushes.LimeGreen, new PointF(15, 10));
                    g.DrawString(min.ToString(), font, Brushes.LimeGreen, new PointF(15, height - 20));
                    g.DrawString(game.Generation.ToString(), font, Brushes.LimeGreen, new PointF(width - 40, 20));

                    if (autoskip)
                        skip++;
                }

                for (int i = x0 + xzoom; i < game.Width + x0 - xzoom; i++)
                {
                    for (int j = y0 + yzoom; j < game.Height + y0 - yzoom; j++)
                    {
                        if (game[i, j])
                            g.FillRectangle(Brushes.White,
                                10 + (i - x0 - xzoom) * zsquare,
                                10 + (j - y0 - yzoom) * zsquare,
                                zsquare, zsquare);
                    }
                }
            });
        }

        onload = delegate
        {
            game = new Game(100, (int)(100.0 / pb.Width * pb.Height));
            draw(g => g.Clear(Color.White));
            setsquare();
        };

        ontick = delegate
        {
            if (opt > 0)
            {
                draw(g =>
                {
                    g.Clear(Color.Black);
                    g.DrawString(optinfo, font, Brushes.White, PointF.Empty);
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
                    for (int i = x0 + xzoom; i < game.Width + x0 - xzoom; i++)
                    {
                        for (int j = y0 + yzoom; j < game.Height + y0 - yzoom; j++)
                        {
                            if (game[i, j])
                            {
                                g.FillRectangle(Brushes.White,
                                    10 + (i - x0 - xzoom) * zsquare,
                                    10 + (j - y0 - yzoom) * zsquare,
                                    zsquare, zsquare);
                                g.DrawRectangle(Pens.Black,
                                    10 + (i - x0 - xzoom) * zsquare,
                                    10 + (j - y0 - yzoom) * zsquare,
                                    zsquare, zsquare);
                            }
                            else g.DrawRectangle(Pens.White,
                                    10 + (i - x0 - xzoom) * zsquare,
                                    10 + (j - y0 - yzoom) * zsquare,
                                    zsquare, zsquare);
                        }
                    }
                    if (inselection)
                    {
                        int i = selx < selx2 ? selx : selx2,
                            j = sely < sely2 ? sely : sely2,
                            w = (selx - i) + (selx2 - i),
                            h = (sely - j) + (sely2 - j);
                        w++;
                        h++;
                        g.DrawRectangle(Pens.LightGreen,
                            10 + (i - x0 - xzoom) * zsquare,
                            10 + (j - y0 - yzoom) * zsquare,
                            w * zsquare, h * zsquare);
                    }
                });
            }
        };

        onkey += delegate (Keys key, bool control, bool shift)
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
                                setsquare();
                                break;
                            case 2:
                                int sizeh = int.Parse(optinfo.Substring(8));
                                if (sizeh < 4 || sizeh > 1000)
                                    break;
                                started = false;
                                game = new Game(game.Width, sizeh);
                                setsquare();
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
                case Keys.I:
                    if (opt == 4)
                    {
                        opt = 0;
                        break;
                    }
                    optinfo =
                    @"
                        Regras de Simulação:
                        A cada geração:
                            -Uma célula com 1 ou menos células adjancentes morre de solidão.
                            -Uma célula com 2 ou 3 células adjacentes vive normalmente.
                            -Uma célula com 4 ou mais células adjacentes morre por superpopulação.
                            -Uma nova célula nasce num quadrado sem célula caso tenha exatamente 3 céulas ajdacentes.

                        Lista de Comandos:
                        Enter - Iniciar/Parar Simulação ou aceitar opções.
                        Esc - Fechar Apliação.
                        W - Editar Largura do mapa.
                        H - Editar Altura do mapa.
                        S - Editar velocidade da simulação de 1 a 100.
                        I - Ver esta tela.
                        P - Quando inciada a execução, mostrar gráfico da população.
                        A - Mover gráfico da população automaticamente.
                        Seta para Direita - Mover gráfico para direita.
                        Seta para Esquerda - Mover gráfico para esquerda.
                        Ctrl + C - Copiar Seção.
                        Ctrl + V - Colar Seção.
                        Botão direito - Clique para adicionar celula viva, segure e mova para selecionar Seção.
                        Botão esquerdo - Arraste para arrastar o mapa.
                        Scroll - De zoom no centro do mapa.
                    ";
                    opt = 4;
                    break;
                
                case Keys.P:
                    show = !show;
                    break;
                case Keys.A:
                    autoskip = !autoskip;
                    break;
                case Keys.Right:
                    skip++;
                    break;
                case Keys.Left:
                    skip--;
                    break;

                case Keys.C:
                    if (control || inselection)
                    {
                        int i = selx < selx2 ? selx : selx2,
                            j = sely < sely2 ? sely : sely2,
                            w = (selx - i) + (selx2 - i),
                            h = (sely - j) + (sely2 - j);
                        w++;
                        h++;
                        Clipboard.SetText(game.Copy(i, j, w, h));
                    }
                    break;
                case Keys.V:
                    if (control)
                    {
                        int i = (int)((p.X - 10) / zsquare) + x0 + xzoom,
                            j = (int)((p.Y - 10) / zsquare) + y0 + yzoom;
                        game.Past(i, j, Clipboard.GetText());
                    }
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

        pb.MouseWheel += delegate (object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
                zoom += .1f;
            else if (e.Delta < 0)
                zoom -= .1f;
            if (zoom < 1)
                zoom = 1;

            xzoom = (game.Width - (int)(game.Width / zoom)) / 2;
            yzoom = (game.Height - (int)(game.Height / zoom)) / 2;

            treatzoomboundaries();
            setsquare();
        };

        pb.MouseDown += delegate (object sender, MouseEventArgs e)
        {
            mousedown = true;
            if (e.Button == MouseButtons.Left)
            {
                inselection = false;
                int i = (int)((e.Location.X - 10) / zsquare) + x0 + xzoom,
                    j = (int)((e.Location.Y - 10) / zsquare) + y0 + yzoom;
                selx = i;
                sely = j;
            }
            else if (e.Button == MouseButtons.Right)
            {
                movepoint = e.Location;
            }
        };

        pb.MouseClick += delegate (object sender, MouseEventArgs e)
        {
            if (square < 1 || inselection)
                return;
            if (e.Button == MouseButtons.Left)
            {
                int i = (int)((e.Location.X - 10) / zsquare) + x0 + xzoom,
                    j = (int)((e.Location.Y - 10) / zsquare) + y0 + yzoom;
                if (i > -1 && i < game.Width && j > -1 && j < game.Height)
                    game[i, j] = !game[i, j];
            }
        };

        pb.MouseUp += delegate (object sender, MouseEventArgs e)
        {
            mousedown = false;
            form.Cursor = Cursors.Default;
        };

        pb.MouseMove += delegate (object sender, MouseEventArgs e)
        {
            p = e.Location;
            if (e.Button == MouseButtons.Left)
            {
                if (mousedown)
                {
                    //Selecte mechanics
                    int i = (int)((e.Location.X - 10) / zsquare) + x0 + xzoom,
                        j = (int)((e.Location.Y - 10) / zsquare) + y0 + yzoom;
                    selx2 = i;
                    sely2 = j;
                    if (selx != selx2 || sely != sely2)
                        inselection = true;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (mousedown)
                {
                    int delta;
                    delta = (int)((movepoint.X - p.X) / zsquare);
                    if (delta != 0)
                    {
                        x0 += delta;
                        movepoint.X = p.X;
                    }
                    delta = (int)((movepoint.Y - p.Y) / zsquare);
                    if (delta != 0)
                    {
                        y0 += delta;
                        movepoint.Y = p.Y;
                    }
                    treatzoomboundaries();
                }
            }
        };

        Application.Run(form);
    }
}