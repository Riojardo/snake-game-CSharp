
using System;
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;


namespace Snake_C_
{
    public partial class Form1 : Form
    {
        static string executableDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string iconePath = Path.Combine(executableDirectory, "images", "snakeIcone.ico");
        bool goUp, goDown, goLeft, goRight;
        private Snake The_Snake;
        public Image _snakeEyeLeftImage;
        public Image _snakeEyeRightImage;

        
        Random chaos = new Random();

        public Form1()
        {
           
            this.WindowState = FormWindowState.Maximized;
            this.Dock = DockStyle.Fill;
            this.Text = "Snake Inc.";
            this.Name = "Snake Inc.";
            createbackground();
            InitializeComponent();
            this.Load += new System.EventHandler(this.Loading);
            string leftImagePath = Path.Combine(executableDirectory, "images", "snake-eye-left.png");
            if (File.Exists(leftImagePath))
            {
                _snakeEyeLeftImage = Image.FromFile(leftImagePath);
            }
            string rightImagePath = Path.Combine(executableDirectory, "images", "snake-eye-right.png");
            if (File.Exists(rightImagePath))
            {
                _snakeEyeRightImage = Image.FromFile(rightImagePath);
            }
            SetUpDataGridView();
            dataGridView1.Location = new Point(50, 50);
            dataGridView1.BorderStyle = BorderStyle.Fixed3D;
            this.Text = "Snake Inc. ";
            if (File.Exists(iconePath))
            {
                this.Icon = new Icon(iconePath);
            }
            
            this.dataGridView1.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.eyePlacing);
            dataGridView1.Invalidate();
            this.KeyPreview = true;


            this.KeyDown += keyDown; // Ensure this is present
            this.KeyUp += keyUp;     // Ensure this is present       
        }


        private void createbackground()
        {
            string imagePath = Path.Combine(executableDirectory, "images", "snakide.png");
            Image myimage = new Bitmap(imagePath);
            this.BackgroundImage = myimage;
        }
        private void SetUpDataGridView()
        {
            dataGridView1.Visible = false;
            dataGridView1.ColumnCount = 41;
            dataGridView1.RowCount = 41;
            dataGridView1.RowTemplate.Height = 30;
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.Width = 25;
            }
            dataGridView1.Enabled = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.ColumnHeadersVisible = false;
            dataGridView1.Rows[0].Visible = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToOrderColumns = false;
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridView1.MultiSelect = false;
            dataGridView1.EditMode = DataGridViewEditMode.EditProgrammatically;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dataGridView1.GridColor = SystemColors.ActiveBorder;
            dataGridView1.Width = (dataGridView1.ColumnCount * 25) + 3;
            dataGridView1.Height = (dataGridView1.RowCount * 25) - 22;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                {
                    dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.Black;

                }
            }
            dataGridView1.GridColor = Color.Gray;

            dataGridView1.ClearSelection();
            dataGridView1.Update();
            dataGridView1.Visible = true;
        }

        private void Loading(object sender, EventArgs e)
        {
            if (dataGridView1.ColumnCount > 0 && dataGridView1.RowCount > 0)
            {
                foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
                {
                    cell.Selected = false;
                }
                dataGridView1.ClearSelection();
            }
        }

        private void keyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up && Snake.direction != "Down")
            {
                goUp = true;
                Snake.direction = "Up";
            }
            if (e.KeyCode == Keys.Down && Snake.direction != "Up")
            {
                goDown = true;
                Snake.direction = "Down";
            }
            if (e.KeyCode == Keys.Left && Snake.direction != "Right")
            {
                goLeft = true;
                Snake.direction = "left";
                MessageBox.Show(Snake.direction.ToString());
            }
            if (e.KeyCode == Keys.Right && Snake.direction != "Left")
            {
                goRight = true;
                Snake.direction = "Right";
            }
            Console.WriteLine("fhgfhg");
        }

        private void keyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up && Snake.direction != "Down")
            {
                goUp = true;
                Snake.direction = "Up";
            }
            if (e.KeyCode == Keys.Down && Snake.direction != "Up")
            {
                goDown = true;
                Snake.direction = "Down";
            }
            if (e.KeyCode == Keys.Left && Snake.direction != "Right")
            {
                goLeft = true;
                Snake.direction = "left";
            }
            if (e.KeyCode == Keys.Right && Snake.direction != "Left")
            {
                goRight = true;
                Snake.direction = "Right";
            }
           
        }

        private void eyePlacing(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                if (The_Snake != null &&
                    e.RowIndex == The_Snake.head.Position.Y -3 &&
                    e.ColumnIndex == The_Snake.head.Position.X +3)
                {
                    e.PaintBackground(e.ClipBounds, true);
                    if (_snakeEyeLeftImage != null)
                    {
                        int imageWidth = _snakeEyeLeftImage.Width;
                        int imageHeight = _snakeEyeLeftImage.Height;

                        int destX = e.CellBounds.X;
                        int destY = e.CellBounds.Bottom - imageHeight;

                        Rectangle destRect = new Rectangle(destX, destY, imageWidth, imageHeight);
                        e.Graphics.DrawImage(_snakeEyeLeftImage, destRect);
                    }

                    if (_snakeEyeRightImage != null)
                    {
                        int imageWidth = _snakeEyeRightImage.Width;
                        int imageHeight = _snakeEyeRightImage.Height;

                        int destX = e.CellBounds.Right - imageWidth;
                        int destY = e.CellBounds.Bottom - imageHeight;

                        Rectangle destRect = new Rectangle(destX, destY, imageWidth, imageHeight);
                        e.Graphics.DrawImage(_snakeEyeRightImage, destRect);
                    }

                    e.Handled = true;
                }
            }
        }

        public void createSnake()
        {
            Color initialSnakeColor = Color.Green;
            Point initialHeadPosition = new Point(18, 21);
            List<Point> initialBodySegments = new List<Point>();
            initialBodySegments.Add(new Point(17, 21));
            initialBodySegments.Add(new Point(16, 21));

            The_Snake = new Snake(initialSnakeColor, initialHeadPosition, initialBodySegments);
            dataGridView1.Rows[The_Snake.head.Position.X].Cells[The_Snake.head.Position.Y].Style.BackColor = The_Snake.snakeColor;
            foreach(Point part in The_Snake.body)
            {
                dataGridView1.Rows[part.X].Cells[part.Y].Style.BackColor = The_Snake.snakeColor;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            createSnake();
            createFood();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        /*      private void startGame()
{

food = new Circle { X = chaos.Next(2, maxWidth), Y = chaos.Next(2, maxHeight) };
}*/


    }
}
