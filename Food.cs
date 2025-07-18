using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Snake_C_.Snake;

namespace Snake_C_
{
    internal class Food
    {
        public Color foodColor { get; set; }
        public Point foodPosition { get; set; }
        public List<Color> foodColors { get; set; }
        
        private static Random rnd = new Random();

        public Food(Snake moveSnake, DataGridView dataGridView)
        {
            foodColors = new List<Color>
            {
                Color.Blue,
                Color.Green,
                Color.Red,
                Color.DarkViolet,
                Color.Cyan,
                Color.Yellow,
                Color.Orange,
                Color.Magenta,
                Color.LimeGreen,
                Color.DeepSkyBlue,
                Color.Gold,
                Color.Fuchsia,
                Color.Aqua
            };

            bool isCorner = false; 
            foodColor = foodColors[rnd.Next(foodColors.Count)];
           while (foodColor == Form1.New_Color || foodColor == Color.White)
            {
                foodColor = foodColors[rnd.Next(foodColors.Count)];
            }

            foodPosition = new Point(rnd.Next(dataGridView.ColumnCount), rnd.Next(dataGridView.RowCount));

            isCorner = dataGridView.Rows[foodPosition.X].Cells[foodPosition.Y] == dataGridView.Rows[0].Cells[0] ? true : isCorner;
            isCorner = dataGridView.Rows[foodPosition.X].Cells[foodPosition.Y] == dataGridView.Rows[dataGridView.RowCount - 1].Cells[0] ? true : isCorner;
            isCorner = dataGridView.Rows[foodPosition.X].Cells[foodPosition.Y] == dataGridView.Rows[0].Cells[dataGridView.ColumnCount - 1] ? true : isCorner;
            isCorner = dataGridView.Rows[foodPosition.X].Cells[foodPosition.Y] == dataGridView.Rows[dataGridView.RowCount - 1].Cells[dataGridView.ColumnCount - 1] ? true : isCorner;

            while ((moveSnake.head != null && moveSnake.head.Position == foodPosition) ||
                   (moveSnake.body != null && moveSnake.body.Contains(foodPosition)) ||
                   isCorner)
            {
                foodPosition = new Point(rnd.Next(dataGridView.ColumnCount), rnd.Next(dataGridView.RowCount));            
            }
     
            dataGridView.Rows[foodPosition.X].Cells[foodPosition.Y].Style.BackColor = foodColor;
        }
    }
}
