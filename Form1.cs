
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
        private static System.Timers.Timer snakeTimer;
        static string executableDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string iconePath = Path.Combine(executableDirectory, "images", "snakeIcone.ico");
        private Snake The_Snake;
        private Food The_Food;
        private Point The_Objective;
        private Color New_Color;
        public Image _snakeEyeLeftImage;
        public Image _snakeEyeRightImage;
        Color initialSnakeColor = Color.Green;
        Random chaos = new Random();
        public static int timerCounter = 0;
        public static int score = 0;
        public static int initial = 0;
        public static bool foodExist = false;

        public Form1()
        {
            this.DoubleBuffered = true;
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

            snakeTimer = new System.Timers.Timer();
            snakeTimer.Interval = 333;
            snakeTimer.Elapsed += SnakeTimer_Tick; //
            snakeTimer.AutoReset = true;           

            button2.Enabled = false;
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
            bool takeTurn = false;
            if (e.KeyCode == Keys.Z && Snake.direction != "Down")
            {
                Snake.direction = "Up";
                takeTurn = true;
            }
            if (e.KeyCode == Keys.S && Snake.direction != "Up")
            {
                Snake.direction = "Down";
                takeTurn = true;
            }
            if (e.KeyCode == Keys.Q && Snake.direction != "Right")
            {
                Snake.direction = "Left";
                takeTurn = true;
            }
            if (e.KeyCode == Keys.D && Snake.direction != "Left")
            {
                Snake.direction = "Right";
                takeTurn = true;
            }
            if (e.KeyCode == Keys.Z || e.KeyCode == Keys.S ||
                e.KeyCode == Keys.Q || e.KeyCode == Keys.D)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            if (takeTurn)
            {  
                SnakeTimer_Tick(null, null); 
            }
        }

        private void SnakeTimer_Tick(object sender, EventArgs e)
        {
            timerCounter++;
            try
            {
                if (The_Snake != null)
                {
                    if (The_Snake.body.Contains(The_Snake.head.Position))
                    {
                        gameOver();
                        return;
                    }
                    if (!foodExist)
                    {
                        The_Snake.snakeColor = New_Color;
                        createFood();
                    }
                    The_Snake = Snake.Move(The_Snake, dataGridView1, The_Objective);
                
                }              
            }
            catch (System.ArgumentOutOfRangeException)
            {gameOver();}
            if (dataGridView1.InvokeRequired) 
            {
                dataGridView1.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                {
                    dataGridView1.Invalidate();
                }));
            }
            else
            {
                dataGridView1.Invalidate();
            }
            if (this.InvokeRequired)
            {
                this.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                {
                    label2.Text = "Score : " + score.ToString();
                }));
            }
            else
            {
                label2.Text = "Score : " + score.ToString();
            }
        }

        private void eyePlacing(object sender, DataGridViewCellPaintingEventArgs e)
        {
                int imageWidth = _snakeEyeLeftImage.Width;
                int imageHeight = _snakeEyeLeftImage.Height;
                int destX = 0;
                int destY = 0;

                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    if (The_Snake != null &&
                        e.RowIndex == The_Snake.head.Position.X &&
                        e.ColumnIndex == The_Snake.head.Position.Y)
                    {
                        e.PaintBackground(e.ClipBounds, true);
                        if (_snakeEyeLeftImage != null)
                        {

                            switch (Snake.direction)
                            {
                                case "Down":
                                    destX = e.CellBounds.X;
                                    destY = e.CellBounds.Bottom - imageHeight;
                                    break;
                                case "Up":
                                    destX = e.CellBounds.X;
                                    destY = e.CellBounds.Y; 
                                    break;
                                case "Left":
                                    destX = e.CellBounds.X;
                                    destY = e.CellBounds.Y;
                                    break;
                                case "Right":                               
                                    destX = e.CellBounds.Right - imageWidth;
                                    destY = e.CellBounds.Y;
                                    break;
                            }                      
                            Rectangle destRect = new Rectangle(destX, destY, imageWidth, imageHeight);
                            e.Graphics.DrawImage(_snakeEyeLeftImage, destRect);
                        }

                        if (_snakeEyeRightImage != null)
                        {
                            switch(Snake.direction)
                            {
                                case "Down":
                                    destX = e.CellBounds.Right - imageWidth;
                                    destY = e.CellBounds.Bottom - imageHeight;
                                    break;
                                case "Up":                            
                                    destX = e.CellBounds.Right - imageWidth; 
                                    destY = e.CellBounds.Y; 
                                    break;
                                case "Left":
                                    destX = e.CellBounds.X;
                                    destY = e.CellBounds.Bottom - imageHeight; 
                                    break;
                                case "Right":
                                    destX = e.CellBounds.Right - imageWidth;
                                    destY = e.CellBounds.Bottom - imageHeight;
                                    break;
                            }                       

                            Rectangle destRect = new Rectangle(destX, destY, imageWidth, imageHeight);
                            e.Graphics.DrawImage(_snakeEyeRightImage, destRect);
                        }
                        e.Handled = true;
                    }
                }
        }

        public void createSnake()
        {
            Point initialHeadPosition = new Point(18, 21);
            List<Point> initialBodySegments = new List<Point>();
            initialBodySegments.Add(new Point(17, 21));
            initialBodySegments.Add(new Point(16, 21));

            The_Snake = new Snake(initialSnakeColor, initialHeadPosition, initialBodySegments);
            dataGridView1.Rows[The_Snake.head.Position.X].Cells[The_Snake.head.Position.Y].Style.BackColor = The_Snake.snakeColor;
            foreach (Point part in The_Snake.body)
            {
                dataGridView1.Rows[part.X].Cells[part.Y].Style.BackColor = The_Snake.snakeColor;
            }
        }

        public void createFood()
        {
            Food The_Food = new Food(The_Snake,dataGridView1);
            The_Objective = The_Food.foodPosition;
            New_Color = The_Food.foodColor;
            foodExist = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (snakeTimer != null)
            {
                snakeTimer.Stop();
            }          
            this.KeyDown += keyDown;
            if (!foodExist)
            {
                Snake.direction = "Down";
                The_Snake = null;
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    for (int j = 0; j < dataGridView1.Columns.Count; j++)
                    { 
                       dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.Black; }                   
                    }
                createSnake();
                createFood();
                }        
            snakeTimer.Start();
            //createFood();
            button1.Enabled = false;
            button2.Enabled = true;
        }

        public void gameOver()
        {
            this.KeyDown -= keyDown;
            if (initial < 10)
            {
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    for (int j = 0; j < dataGridView1.Columns.Count; j++)
                    { dataGridView1.Rows[i].Cells[j].Style.BackColor = dataGridView1.Rows[i].Cells[j].Style.BackColor == Color.Black ? Color.White : Color.Black; }
                }
                initial++;
            }
            else
            {
                foodExist = false;
                snakeTimer.Stop();
                The_Snake = null;
                for (int i = 0; i < dataGridView1.RowCount; i++) // Boucle sur les lignes (Y)
                {
                    for (int j = 0; j < dataGridView1.ColumnCount; j++) // Boucle sur les colonnes (X)
                    {
                        // Par défaut, la cellule est noire (fond)
                        dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.Black;

                        // Logique pour les lettres "GAME" (toutes les coordonnées ajustées)
                        // Les valeurs numériques sont directement modifiées pour le décalage.
                        // G (originalement à partir de ligne 15, col 12) -> maintenant à partir de ligne 10, col 9
                        if ((i == 10 && j >= 9 && j <= 12) || // Haut
                            (i == 11 && (j == 8 || j == 12)) || // Côtés
                            (i == 12 && j == 8) ||
                            (i == 13 && ((j >= 8 && j <= 8) || (j >= 11 && j <= 12))) || // Milieu avec barre
                            (i == 14 && (j == 8 || j == 12)) ||
                            (i == 15 && (j >= 9 && j <= 12))) // Bas
                        {
                            dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.White;
                        }
                        // A (originalement à partir de ligne 15, col 18) -> maintenant à partir de ligne 10, col 15
                        else if ((i == 10 && j >= 15 && j <= 17) || // Haut
                                 ((i >= 11 && i <= 12) && (j == 14 || j == 18)) || // Côtés
                                 (i == 13 && j >= 14 && j <= 18) || // Barre du milieu
                                 ((i >= 14 && i <= 15) && (j == 14 || j == 18))) // Bas des côtés
                        {
                            dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.White;
                        }
                        // M (originalement à partir de ligne 15, col 24) -> maintenant à partir de ligne 10, col 21
                        else if (((i >= 10 && i <= 15) && (j == 21 || j == 25)) || // Montants
                                 (i == 11 && (j == 22 || j == 24)) || // Diagonales haut
                                 (i == 12 && j == 23)) // Diagonales milieu
                        {
                            dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.White;
                        }
                        // E (originalement à partir de ligne 15, col 31) -> maintenant à partir de ligne 10, col 28
                        else if (((i >= 10 && i <= 15) && j == 28) || // Barre verticale
                                 (i == 10 && j >= 28 && j <= 31) || // Barre horizontale haut
                                 (i == 13 && j >= 28 && j <= 30) || // Barre horizontale milieu
                                 (i == 15 && j >= 28 && j <= 31)) // Barre horizontale bas
                        {
                            dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.White;
                        }

                        // Logique pour les lettres "OVER" (positions originales de référence : ligne 24, col 13)
                        // O (originalement à partir de ligne 24, col 13) -> maintenant à partir de ligne 19, col 10
                        else if ((i == 19 && j >= 10 && j <= 12) || // Haut
                                 ((i >= 20 && i <= 22) && (j == 9 || j == 13)) || // Côtés
                                 (i == 23 && j >= 10 && j <= 12)) // Bas
                        {
                            dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.White;
                        }
                        // V (originalement à partir de ligne 24, col 19) -> maintenant à partir de ligne 19, col 16
                        else if (((i >= 19 && i <= 21) && (j == 16 || j == 20)) || // Bras hauts
                                 (i == 22 && (j == 17 || j == 19)) || // Bras milieux
                                 (i == 23 && j == 18)) // Pointe
                        {
                            dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.White;
                        }
                        // E (originalement à partir de ligne 24, col 26) -> maintenant à partir de ligne 19, col 23
                        else if (((i >= 19 && i <= 24) && j == 23) || // Barre verticale
                                 (i == 19 && j >= 23 && j <= 26) || // Barre horizontale haut
                                 (i == 22 && j >= 23 && j <= 25) || // Barre horizontale milieu
                                 (i == 24 && j >= 23 && j <= 26)) // Barre horizontale bas
                        {
                            dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.White;
                        }
                        // R (originalement à partir de ligne 24, col 31) -> maintenant à partir de ligne 19, col 28
                        else if (((i >= 19 && i <= 24) && j == 28) || // Barre verticale
                                 (i == 19 && j >= 28 && j <= 30) || // Barre horizontale haut
                                 ((i >= 20 && i <= 21) && j == 31) || // Côté haut
                                 (i == 22 && j >= 28 && j <= 30) || // Barre horizontale milieu
                                 ((i >= 23 && i <= 24) && j == 31)) // Jambe diagonale
                        {
                            dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.White;
                        }
                    }
                }
                dataGridView1.Invalidate();
                initial = 0;
                if (this.InvokeRequired)
                {
                    this.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                    {
                        button1.Enabled = true;
                        button2.Enabled = false;
                    }));
                }
                else
                {
                    button1.Enabled = true;
                    button2.Enabled = false;
                }               
            }
        }
                    

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            button1.Enabled = true;
            snakeTimer.Stop();
            button2.Enabled = false;
        }

        /*      private void startGame()
{

food = new Circle { X = chaos.Next(2, maxWidth), Y = chaos.Next(2, maxHeight) };
}*/


    }
}
