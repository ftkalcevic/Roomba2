using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;


namespace WatchRoomba
{
    class DynamicMap
    {
        const int INITIAL_CAPACITY = 2 * 60 * 60;   // 1 point every second, for 2 hours.
        struct Position
        {
            public int x;
            public int y;
            public int theta;
        }
        int minx, maxx;
        int miny, maxy;

        private List<Position> Positions;

        public DynamicMap()
        {
            Init();
        }

        private void Init()
        {
            Positions = null;
            minx = miny = -10;
            maxx = maxy = 10;
        }

        public void Update(int tick, int x, int y, int theta)
        {
            if (Positions==null)
            {
                Positions = new List<Position>(INITIAL_CAPACITY);
            }
            else
            {
                if (x > maxx)
                    maxx = x;
                else if (x < minx)
                    minx = x;
                if (y > maxy)
                    maxy = y;
                else if (y < miny)
                    miny = y;
            }

            Positions.Add(new Position() { x = x, y = y, theta = theta });
        }

        private void Scale(float min1, float max1, float min2, float max2, out float offset, out float scale)
        {
            if (max1 == min1)
                scale = 1;
            else
                scale = (max2 - min2) / (max1 - min1);
            offset = min1;
        }
        public void Render(Panel p)
        {
            Graphics g = p.CreateGraphics();
            
            g.Clear(Color.Wheat);

            float offsetx, offsety;
            float scalex, scaley;

            Scale(minx, maxx, 0, p.Width, out offsetx, out scalex);
            Scale(miny, maxy, 0, p.Height, out offsety, out scaley);

            if (scalex > scaley)
                scalex = scaley;
            else
                scaley = scalex;

            RectangleF home = new RectangleF((-10 - offsetx )* scalex, (-10 - offsety)* scaley, 20 * scalex, 20 * scaley);
            g.FillEllipse(Brushes.Red, home);

            float x1 = (float)((Positions[0].x - offsetx) * scalex);
            float y1 = (float)((Positions[0].y - offsety ) * scaley);
            for ( int i = 1;i < Positions.Count; i++)
            {
                float x2 = (float)((Positions[i].x - offsetx) * scalex);
                float y2 = (float)((Positions[i].y - offsety )* scaley );
                g.DrawLine(Pens.Black, x1, y1, x2, y2);
                x1 = x2;
                y1 = y2;
            }

            float size = (float)(20.0 * scalex);
            RectangleF roomba = new RectangleF(x1-size/2, y1-size/2,size,size);

            g.FillEllipse(Brushes.Aqua, roomba);
            g.Dispose();
        }
    }
}
