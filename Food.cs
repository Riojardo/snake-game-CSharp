using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Snake_C_.Snake;

namespace Snake_C_
{
    internal class Food
    {
        public Color foodColor { get; set; }
        public Point position { get; set; }
        public List<Color> foodColors { get; set; }

        public Food()
        {
            foodColor = Color.Blue;
            position = new Point(1, 1);
            foodColors = new List<Color> { Color.Blue, Color.Green , Color.Red, Color.DarkViolet }; 
        }

    }
}
