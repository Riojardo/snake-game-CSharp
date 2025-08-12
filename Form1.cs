
using System;
using System.IO;
using System.Media;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace Snake_C_
{
    public partial class Form1 : Form
    {
        public enum MenuOption
        {
            SizeSelection,
            DifficultySelection,
            Etablished
        }

        //Private field into public propiété (ex dans audiomanager) arrive plusieurs fois à check 
        public MenuOption CurrentMenuState = MenuOption.SizeSelection;

        private static System.Timers.Timer snakeTimer;
        //On évite souvent le static avec du public parce que ça peut causer des pbs sur pas mal de programmes -> Peut être le convertir en readonly ou const ? Ou bien jarter le static arrive plusieurs fois à check
        //Private field into public propiété
        public static int parameterTime;
        public static string executableDirectory = AppDomain.CurrentDomain.BaseDirectory;
        //Je ferais de "images" une constante pour pas l'utiliser 4 fois 
        string iconePath = Path.Combine(executableDirectory, "images", "snakeIcone.ico");
        private static Snake The_Snake;
        private static Point The_Objective;
        public static Color New_Color;
        internal static Food The_Food;
        //private static DataGridView dataGridView1 = new DataGridView();
        public Image _snakeEyeLeftImage;
        public Image _snakeEyeRightImage;
        Color initialSnakeColor = Color.Green;
        public static int timerCounter = 0;
        public static int score = 0;
        public static int initial = 0;
        public static int widthSize;
        public static int heightSize;
        public static bool foodExist = false;
        public static MusicManager simpleMusic;
       

        public Form1()
        {
            this.DoubleBuffered = true;
            this.WindowState = FormWindowState.Maximized;
            this.Dock = DockStyle.Fill;
            this.Text = "Snake Inc.";
            this.Name = "Snake Inc.";
            this.Text = "Snake Inc. ";
            // J'ai modifié ta méthode "Form1_Closing" pour expliciter le fait que "object" puisse accepter un nul parce que formclosingeventhandler attend un object nullable 
            this.FormClosing += Form1_Closing;
            if (File.Exists(iconePath))
            {
                this.Icon = new Icon(iconePath);
            }

            createbackground();
            InitializeComponent();

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //Variable non utilisée, work in progress ? 
            string soundLocation = Path.Combine(Form1.executableDirectory, "sounds", "game_music.wav");
            simpleMusic = new MusicManager("game_music");
            simpleMusic.PlayThis(true);        
        }

        private void StartGameSetup()
        {
            panel1.Visible = true;
            //Same que pour Form1_Closing
            this.Load += new System.EventHandler(this.Loading);
            createImageEyes();
            SetUpDataGridView();
            //On aime pas trop les ternaires nesté normalement ahah c'est un peu chiant à lire, donc on passe svt ça en if/else
            dataGridView1.Location = widthSize == 21 ? new Point(150, 150) :
                                     widthSize == 31 ? new Point(100, 100) : new Point(50, 50);

            dataGridView1.BorderStyle = BorderStyle.Fixed3D;

            dataGridView1.Invalidate();

            this.KeyPreview = true;

            snakeTimer = new System.Timers.Timer();
            snakeTimer.Interval = parameterTime;
            snakeTimer.Elapsed += SnakeTimer_Tick;
            snakeTimer.AutoReset = true;

            button2.Enabled = false;
        }

        private void createImageEyes()
        {
            string leftImagePath = Path.Combine(executableDirectory, "images", "snake-eye-left.png");
            if (File.Exists(leftImagePath))
            {
                _snakeEyeLeftImage = Image.FromFile(leftImagePath);
            }
            string rightImagePath = Path.Combine(executableDirectory, "images", "snake-eye-right.png");
            this.dataGridView1.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.eyePlacing);
            if (File.Exists(rightImagePath))
            {
                _snakeEyeRightImage = Image.FromFile(rightImagePath);
            }
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
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ColumnCount = widthSize;
            dataGridView1.RowCount = heightSize;
            dataGridView1.RowTemplate.Height = 25;
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.Width = 25;
            }
            dataGridView1.Enabled = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.ColumnHeadersVisible = false;
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
            dataGridView1.Height = (dataGridView1.RowCount * 25) + 3;

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

        private void Loading(object? sender, EventArgs e)
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
        //Comme playsimplesound utilise pas de data instanciée tu pourrait le passer en static
        public void playSimpleSound(string sound)
        {
            string soundLocation = Path.Combine(Form1.executableDirectory, "sounds", sound + ".wav");
            SoundPlayer simpleSound = new SoundPlayer(soundLocation);
            simpleSound.Play();
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
                    if (The_Food != null)
                    {
                        if (The_Snake.head.Position == The_Food.foodPosition)
                        {
                            The_Snake.snakeColor = The_Food.foodColor;
                        }
                    }
                    The_Snake = Snake.Move(The_Snake, dataGridView1, The_Objective, this);
                }
            }
            //C'est cool les catchs mdr Si c'est un choix de pas gérer l'exception, on aime bien commenter et dire pourquoi y'a pas besoin de la gérer dans ce cas-ci 

            catch (Exception outOfRange)
            { }
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

        //C'est une grosse méthode ahah je la refactoriserais pour réduire sa complexité, elle est un peu lourde à elle seule
        private void eyePlacing(object sender, DataGridViewCellPaintingEventArgs e)
        {
            try
            {
                if (_snakeEyeLeftImage != null || _snakeEyeRightImage != null)
                {
                    int imageWidth = _snakeEyeLeftImage.Width;
                    int imageHeight = _snakeEyeLeftImage.Height;
                    int destX = 0;
                    int destY = 0;
                    //Vu que le premier if ne fait rien seul, je le mergerais avec le suivant
                    if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                    {
                        if (The_Snake != null &&
                            e.RowIndex == The_Snake.head.Position.X &&
                            e.ColumnIndex == The_Snake.head.Position.Y)
                        {
                            e.PaintBackground(e.ClipBounds, true);

                            switch (Snake.direction)
                            {
                                case "Down":
                                    destX = e.CellBounds.X;
                                    destY = e.CellBounds.Bottom - imageHeight;
                                    break;
                                // Tu pourrais merge up & left, mais ça pourrait se discuter avec le fait que c'est + facilement modifiable si tu le veux comme ça 
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
                            Rectangle destRectLeft = new Rectangle(destX, destY, imageWidth, imageHeight);
                            e.Graphics.DrawImage(_snakeEyeLeftImage, destRectLeft);

                            switch (Snake.direction)
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

                            Rectangle destRectRight = new Rectangle(destX, destY, imageWidth, imageHeight);
                            e.Graphics.DrawImage(_snakeEyeRightImage, destRectRight);

                            e.Handled = true;
                        }
                    }
                }
            }
            catch (System.Exception)
            {
                e.Handled = true;
            }
        }

        public void createSnake()
        {
            Point initialHeadPosition = new Point((dataGridView1.RowCount - 3) / 2, (dataGridView1.ColumnCount + 1) / 2);
            List<Point> initialBodySegments = new List<Point>();
            initialBodySegments.Add(new Point((dataGridView1.RowCount - 5) / 2, (dataGridView1.ColumnCount + 1) / 2));
            initialBodySegments.Add(new Point((dataGridView1.RowCount - 7) / 2, (dataGridView1.ColumnCount + 1) / 2));

            The_Snake = new Snake(initialSnakeColor, initialHeadPosition, initialBodySegments);
            dataGridView1.Rows[The_Snake.head.Position.X].Cells[The_Snake.head.Position.Y].Style.BackColor = The_Snake.snakeColor;
            foreach (Point part in The_Snake.body)
            {
                dataGridView1.Rows[part.X].Cells[part.Y].Style.BackColor = The_Snake.snakeColor;
            }
        }
        public static void createFood(DataGridView dataGridView1)
        {
            Food The_Food = new Food(The_Snake, dataGridView1);
            The_Objective = The_Food.foodPosition;
            New_Color = The_Food.foodColor;
            foodExist = true;

        }
        public void gameOver()
        {
            simpleMusic.StopPlaying();
            if (foodExist) { playSimpleSound("game_over"); }
            foodExist = false;
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
                snakeTimer.Stop();
                The_Snake = null;
                DisplayGame_Over(widthSize);
                dataGridView1.Invalidate();
                initial = 0;
                this.dataGridView1.CellPainting -= new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.eyePlacing);
                _snakeEyeLeftImage = null;
                _snakeEyeRightImage = null;
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
        public void DisplayGame_Over(int widthSize)
        {
            // Clear the grid by setting all cells to black
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                {
                    dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.Black;
                }
            }
            // --- Define the base offsets for centering the entire text block ---
            // The target text block width is 19 cells.
            // The target text block height (assuming 5-cell tall letters + 3 cell vertical spacing) is 5 + 3 + 5 = 13 cells.
            int desiredTextWidth = 19;
            int desiredTextHeight = 13; // Calculated from the new letter sizes and spacing

            // Calculate dynamic offsets for centering based on the actual grid widthSize
            int startColumnOffset = (widthSize - desiredTextWidth) / 2;
            if (startColumnOffset < 0) startColumnOffset = 0; // Prevent negative offset

            int startRowOffset = (widthSize - desiredTextHeight) / 2;
            if (startRowOffset < 0) startRowOffset = 0; // Prevent negative offset

            // Define the fixed starting rows for each line relative to the startRowOffset
            int baseRowGame = startRowOffset;
            int baseRowOver = startRowOffset + 8; // 5 (letter height) + 3 (vertical spacing) = 8

            // --- Iterate through each cell and set color based on new, smaller coordinates ---
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                {
                    dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.Black; // Default to black

                    // --- G (4x6 cells) ---
                    // Relative to (baseRowGame, startColumnOffset)
                    if ((i == baseRowGame + 0 && j >= startColumnOffset + 1 && j <= startColumnOffset + 3) || // Top bar
                         (i == baseRowGame + 1 && j == startColumnOffset + 0) ||                               // Left side
                         (i == baseRowGame + 2 && (j == startColumnOffset + 0 || j == startColumnOffset + 2 || j == startColumnOffset + 3)) || // Middle bar + right
                         (i == baseRowGame + 3 && (j == startColumnOffset + 0 || j == startColumnOffset + 3)) || // Left side + right
                         (i == baseRowGame + 4 && j == startColumnOffset + 0) ||                               // Left side
                         (i == baseRowGame + 5 && j >= startColumnOffset + 1 && j <= startColumnOffset + 3))   // Bottom bar
                    {
                        dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.White;
                    }
                    // --- A (4x6 cells) ---
                    // Relative to (baseRowGame, startColumnOffset + 5)
                    else if ((i == baseRowGame + 0 && (j == startColumnOffset + 5 + 1 || j == startColumnOffset + 5 + 2)) || // Top point
                              (i == baseRowGame + 1 && (j == startColumnOffset + 5 + 0 || j == startColumnOffset + 5 + 3)) || // Sides
                              (i == baseRowGame + 2 && (j == startColumnOffset + 5 + 0 || j == startColumnOffset + 5 + 3)) || // Sides
                              (i == baseRowGame + 3 && j >= startColumnOffset + 5 + 0 && j <= startColumnOffset + 5 + 3) ||  // Middle bar
                              (i == baseRowGame + 4 && (j == startColumnOffset + 5 + 0 || j == startColumnOffset + 5 + 3)) || // Sides
                              (i == baseRowGame + 5 && (j == startColumnOffset + 5 + 0 || j == startColumnOffset + 5 + 3)))  // Bottom sides
                    {
                        dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.White;
                    }
                    // --- M (4x6 cells) ---
                    // Relative to (baseRowGame, startColumnOffset + 10)
                    else if ((i >= baseRowGame + 0 && i <= baseRowGame + 5 && (j == startColumnOffset + 10 + 0 || j == startColumnOffset + 10 + 3)) || // Posts
                              (i == baseRowGame + 1 && (j == startColumnOffset + 10 + 1 || j == startColumnOffset + 10 + 2)) || // Diagonals
                              (i == baseRowGame + 2 && (j == startColumnOffset + 10 + 1 || j == startColumnOffset + 10 + 2))) // Diagonals (inner parts)
                                                                                                                              // The original M definition had (1,0),(1,1),(1,3) and (2,0),(2,2),(2,4). Let's use a simpler 4x6 M
                                                                                                                              // This simplified M below might be better for 4-wide.                                                                                                                              // Let's re-use the M from the previous response which was good for 4x6
                    {
                        // M from the previous detailed response (4x6)
                        if (((i >= baseRowGame + 0 && i <= baseRowGame + 5) && (j == startColumnOffset + 10 + 0 || j == startColumnOffset + 10 + 3)) || // Vertical bars
                             (i == baseRowGame + 1 && (j == startColumnOffset + 10 + 1 || j == startColumnOffset + 10 + 2)) || // Inner diagonals
                             (i == baseRowGame + 2 && (j == startColumnOffset + 10 + 1 || j == startColumnOffset + 10 + 2))) // Inner diagonals
                        {
                            dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.White;
                        }
                    }
                    // --- E (4x6 cells) ---
                    // Relative to (baseRowGame, startColumnOffset + 15)
                    else if (((i >= baseRowGame + 0 && i <= baseRowGame + 5) && j == startColumnOffset + 15 + 0) || // Vertical bar
                              (i == baseRowGame + 0 && j >= startColumnOffset + 15 + 0 && j <= startColumnOffset + 15 + 3) || // Top bar
                              (i == baseRowGame + 2 && j >= startColumnOffset + 15 + 0 && j <= startColumnOffset + 15 + 2) || // Middle bar
                              (i == baseRowGame + 5 && j >= startColumnOffset + 15 + 0 && j <= startColumnOffset + 15 + 3)) // Bottom bar
                    {
                        dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.White;
                    }
                    // Logique pour les lettres "OVER"
                    // O: (4 wide) | V: (4 wide) | E: (4 wide) | R: (4 wide)
                    // Total: (4+1) + (4+1) + (4+1) + 4 = 19 cells.

                    // --- O (4x6 cells) ---
                    // Relative to (baseRowOver, startColumnOffset)
                    else if ((i == baseRowOver + 0 && j >= startColumnOffset + 1 && j <= startColumnOffset + 2) || // Top curve
                              ((i >= baseRowOver + 1 && i <= baseRowOver + 4) && (j == startColumnOffset + 0 || j == startColumnOffset + 3)) || // Sides
                              (i == baseRowOver + 5 && j >= startColumnOffset + 1 && j <= startColumnOffset + 2)) // Bottom curve
                    {
                        dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.White;
                    }
                    // --- V (4x6 cells) ---
                    // Relative to (baseRowOver, startColumnOffset + 5)
                    else if (((i >= baseRowOver + 0 && i <= baseRowOver + 2) && (j == startColumnOffset + 5 + 0 || j == startColumnOffset + 5 + 3)) || // Top arms
                              ((i >= baseRowOver + 3 && i <= baseRowOver + 5) && (j == startColumnOffset + 5 + 1 || j == startColumnOffset + 5 + 2))) // Bottom V
                    {
                        dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.White;
                    }
                    // --- E (4x6 cells) ---
                    // Relative to (baseRowOver, startColumnOffset + 10)
                    else if (((i >= baseRowOver + 0 && i <= baseRowOver + 5) && j == startColumnOffset + 10 + 0) || // Vertical bar
                              (i == baseRowOver + 0 && j >= startColumnOffset + 10 + 0 && j <= startColumnOffset + 10 + 3) || // Top bar
                              (i == baseRowOver + 2 && j >= startColumnOffset + 10 + 0 && j <= startColumnOffset + 10 + 2) || // Middle bar
                              (i == baseRowOver + 5 && j >= startColumnOffset + 10 + 0 && j <= startColumnOffset + 10 + 3)) // Bottom bar
                    {
                        dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.White;
                    }
                    // --- R (4x6 cells) ---
                    // Relative to (baseRowOver, startColumnOffset + 15)
                    else if (((i >= baseRowOver + 0 && i <= baseRowOver + 5) && j == startColumnOffset + 15 + 0) || // Vertical bar
                              (i == baseRowOver + 0 && j >= startColumnOffset + 15 + 0 && j <= startColumnOffset + 15 + 2) || // Top curve
                              ((i >= baseRowOver + 1 && i <= baseRowOver + 2) && j == startColumnOffset + 15 + 3) || // Right side of top curve
                              (i == baseRowOver + 3 && j >= startColumnOffset + 15 + 0 && j <= startColumnOffset + 15 + 2) || // Middle bar
                              (i == baseRowOver + 4 && j == startColumnOffset + 15 + 3) || // Leg (diagonal)
                              (i == baseRowOver + 5 && j == startColumnOffset + 15 + 3)) // Leg (bottom)
                    {
                        dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.White;
                    }
                }
            }
            // Invalidate the DataGridView to force a redraw
            dataGridView1.Invalidate();
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
                simpleMusic.PlayThis(true);
                createImageEyes();
                this.dataGridView1.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.eyePlacing);
                Snake.direction = "Down";
                The_Snake = null;
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    for (int j = 0; j < dataGridView1.Columns.Count; j++)
                    {
                        dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.Black;
                    }
                }
                score = 0;
                label2.Text = "Score : " + score.ToString();
                createSnake();
                createFood(dataGridView1);
            }
            snakeTimer.Start();
            button1.Enabled = false;
            button2.Enabled = true;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            button1.Enabled = true;
            snakeTimer.Stop();
            button2.Enabled = false;
            //simpleMusic.SetVolume(1);
        }

        //Utilisation de champ statique dans une méthode non statique -> rends la méthode statique si ça a du sens ou enlève le statique aux champs
        private void button3_Click(object sender, EventArgs e)
        {
            if (CurrentMenuState == MenuOption.SizeSelection)
            {
                CurrentMenuState = MenuOption.DifficultySelection;
                button3.Text = "Slow";
                button4.Text = "Average";
                button5.Text = "FAST !";
                widthSize = 21;
                heightSize = 21;
            }
            else if (CurrentMenuState == MenuOption.DifficultySelection)
            {
                parameterTime = 333;
                panel2.Visible = false;
                StartGameSetup();
            }
        }
        //Same as above
        private void button4_Click_1(object sender, EventArgs e)
        {
            if (CurrentMenuState == MenuOption.SizeSelection)
            {
                CurrentMenuState = MenuOption.DifficultySelection;
                button3.Text = "Slow";
                button4.Text = "Average";
                button5.Text = "FAST !";
                widthSize = 31;
                heightSize = 31;
            }
            else if (CurrentMenuState == MenuOption.DifficultySelection)
            {
                parameterTime = 250;
                panel2.Visible = false;
                StartGameSetup();
            }
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            if (CurrentMenuState == MenuOption.SizeSelection)
            {
                CurrentMenuState = MenuOption.DifficultySelection;
                button3.Text = "Slow";
                button4.Text = "Average";
                button5.Text = "FAST !";
                widthSize = 41;
                heightSize = 41;
            }
            else if (CurrentMenuState == MenuOption.DifficultySelection)
            {
                parameterTime = 166;
                panel2.Visible = false;
                StartGameSetup();
            }
        }

        private void Form1_Closing(object? sender, FormClosingEventArgs e)
        {
            simpleMusic.StopPlaying();
        }

    }
}
