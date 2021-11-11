using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DFacesProcessing
{
    class z_buffer
    {
        public List<int> interpolate(int x1, int y1, int x2, int y2)
        {
            List<int> res=new List<int>();
            if (x1 == x2)
            {
                res.Add(y2);
            }
            double step=(y2-y1)*1.0f / (x2 - x1);//с таким шагом будем получать новые точки
            double y = y1;
            for (int i = x1; i < x2; i++)
            {
                res.Add((int)y);
                y += step;
            }
            return res;
        }
        //растеризация треугольника
        public List<Point> Raster(List<Point> points)
        {
            List<Point> res = new List<Point>();
            //отсортировать точки по неубыванию ординаты
            points.Sort((p1, p2) => p1.Y.CompareTo(p2.Y));
            // "рабочие точки"
            // изначально они находятся в верхней точке
            var wpoints=points.Select((p) => (x: (int)p.X,y:(int)p.Y,z:(int)p.Z)).ToList();
            var xy01= interpolate(wpoints[0].y, wpoints[0].x, wpoints[1].y, wpoints[1].x);
            var xy12 = interpolate(wpoints[1].y, wpoints[1].x, wpoints[2].y, wpoints[2].x);
            var xy02 = interpolate(wpoints[0].y, wpoints[0].x, wpoints[2].y, wpoints[2].x);
            var yz01 = interpolate(wpoints[0].y, wpoints[0].z, wpoints[1].y, wpoints[1].z);
            var yz12 = interpolate(wpoints[1].y, wpoints[1].z, wpoints[2].y, wpoints[2].z);
            var yz02 = interpolate(wpoints[0].y, wpoints[0].z, wpoints[2].y, wpoints[2].z);
            xy01.RemoveAt(xy01.Count()-1);//убрать точку, чтобы не было повтора
            var xy = xy01.Concat(xy12).ToList();
            yz01.RemoveAt(yz01.Count() - 1);
            var yz = yz01.Concat(yz12).ToList();
            //когда растеризуем, треугольник делим надвое
            //ищем координаты, чтобы разделить треугольник на 2
            int center = xy.Count() / 2;
            List<int> lx, rx, lz, rz;//для приращений
            if (xy02[center] < xy[center])
            {
                lx = xy02;
                lz = yz02;
                rx = xy;
                rz = yz;
            }
            else
            {
                lx = xy;
                lz = yz;
                rx = xy02;
                rz = yz02;
            }
            int y0 = wpoints[0].y;
            int y2 = wpoints[2].y;
            for (int i = 0; i <= y2 - y0; i++)
            {
                int leftx = lx[i];
                int rightx = rx[i];
                List<int> zcurr = interpolate(leftx, lz[i], rightx, rz[i]);
                for (int j = leftx; j < rightx; j++)
                {
                    res.Add(new Point(j, y0 + i, zcurr[j - leftx]));
                }
            }
            return res;
        }
        //разбиение на треугольники
    List<List<Point>> Triangulate(List<Point> points)
    {
        //если всего 3 точки, то это уже трекгольник
        List<List<Point>> res = new List<List<Point>>();
        if (points.Count == 3)
        {
            res = new List<List<Point>>{points};
        }
        for (int i = 2; i < points.Count(); i++)
        {
            res.Add(new List<Point> { points[0], points[i - 1], points[i] });//points[i-2]
        }
        return res;
    }
        //растеризовать полигон
        public List<List<Point>> RasterPolygon(Shape figure)
        {
            List <List<Point>> res= new List<List<Point>>();
            foreach (var facet in figure.Faces)//каждая грань-это многоугольник, который надо растеризовать
            {
                List<Point> currentface = new List<Point>();
                List<Point> points = new List<Point>();
                //добавим все вершины
                for (int i = 0; i < facet.Verticles.Count(); i++)
                {
                    points.Add(facet.Verticles[i]);
                }
               
            List<List<Point>> triangles = Triangulate(points);//разбили все грани на треугольники
                foreach (var triangle in triangles)
                {
                    currentface.AddRange(Raster(triangle));//projection(triangle)
                }
                res.Add(currentface);
            }
            return res;
        }

    }
   // public List<Point> Projection(List<Point> points)
    //{ 

   // }
    
}