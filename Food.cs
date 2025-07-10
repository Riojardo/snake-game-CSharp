using System;
using System.Collections.Generic;
using System.Linq;
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
        public Random randomColor { get; set; }

        public Food(Snake moveSnake, DataGridView dataGridView)
        {
            randomColor = new Random();
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
            Random rnd = new();

            bool isCorner = false;
            
            foodPosition = new Point(rnd.Next(dataGridView.ColumnCount), rnd.Next(dataGridView.RowCount));

            isCorner = dataGridView.Rows[foodPosition.X].Cells[foodPosition.Y] == dataGridView.Rows[0].Cells[0] ? true : isCorner;
            isCorner = dataGridView.Rows[foodPosition.X].Cells[foodPosition.Y] == dataGridView.Rows[dataGridView.RowCount -1].Cells[0] ? true : isCorner;
            isCorner = dataGridView.Rows[foodPosition.X].Cells[foodPosition.Y] == dataGridView.Rows[0].Cells[0] ? true : isCorner;
            isCorner = dataGridView.Rows[foodPosition.X].Cells[foodPosition.Y] == dataGridView.Rows[dataGridView.RowCount - 1].Cells[dataGridView.ColumnCount - 1] ? true : isCorner;

            while ((moveSnake.head != null && moveSnake.head.Position == foodPosition) || (moveSnake.body != null && moveSnake.body.Contains(foodPosition)) || isCorner)
            {
                foodPosition = new Point(rnd.Next(dataGridView.ColumnCount), rnd.Next(dataGridView.RowCount));
            }
            int index = randomColor.Next(foodColors.Count);
            foodColor = foodColors[index];

            dataGridView.Rows[foodPosition.X].Cells[foodPosition.Y].Style.BackColor = foodColor;

        }
    }
}
