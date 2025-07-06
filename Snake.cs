using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake_C_
{
   public class Snake
    {
        public Color snakeColor { get; set; }
        public Head head { get; set; } 
        public List<Point> body { get; set; }
        public static string direction { get; set; }

        public Snake(Color S_color, Point S_HeadPosition, List<Point> S_body) 
        {
            this.snakeColor = S_color;
            this.head = new Head(S_HeadPosition); 
            this.body = S_body;
        }
        public class Head
        {
            public Point Position { get; set; }          
            public Point LeftEyePosition { get; set; } 
            public Point RightEyePosition { get; set; }

            public Head(Point position)
            {
                this.Position = position;
             
            }
        }
        public void Move()
        {
            //Don't ask gemini
            //Point movedHead = 
        
        }
    }
}
