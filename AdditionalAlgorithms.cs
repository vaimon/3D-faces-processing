using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DFacesProcessing
{
    public partial class Form1
    {
        public static byte bytify(double color)
        {
            return (byte)Math.Round(255 * color);
        }

        private float frac(double f)
        {
            return (float)(f - Math.Truncate(f));
        }

        /// <summary>
        /// https://en.wikipedia.org/wiki/Xiaolin_Wu%27s_line_algorithm
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        private void drawVuLine(System.Drawing.Point p0, System.Drawing.Point p1, Color color)
        {
            float x0 = p0.X, x1 = p1.X, y0 = p0.Y, y1 = p1.Y;
            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep)
            {
                (x0, y0) = (y0, x0);
                (x1, y1) = (y1, x1);
            }
            if (x0 > x1)
            {
                (x0, x1) = (x1, x0);
                (y0, y1) = (y1, y0);
            }
            fbitmap.SetPixel(p1, color);


            float dx = x1 - x0, dy = y1 - y0;
            float gradient = dy / dx;
            if (dx == 0)
            {
                gradient = 1;
            }

            float xend = (float)Math.Round(x0);
            float yend = y0 + gradient * (xend - x0);
            float xgap = (float)(1 - frac(x0 + 0.5));
            float xpxl1 = xend;
            float ypxl1 = (float)Math.Floor(yend);
            if (steep)
            {
                fbitmap.SetPixel(new System.Drawing.Point((int)ypxl1, (int)xpxl1), Color.FromArgb(bytify((1 - frac(yend)) * xgap), color.R, color.G, color.B));
                fbitmap.SetPixel(new System.Drawing.Point((int)ypxl1 + 1, (int)xpxl1), Color.FromArgb(bytify(frac(yend) * xgap), color.R, color.G, color.B));
            }
            else
            {
                fbitmap.SetPixel(new System.Drawing.Point((int)xpxl1, (int)ypxl1), Color.FromArgb(bytify((1 - frac(yend)) * xgap), color.R, color.G, color.B));
                fbitmap.SetPixel(new System.Drawing.Point((int)xpxl1, (int)ypxl1 + 1), Color.FromArgb(bytify((1 - frac(yend)) * xgap), color.R, color.G, color.B));
            }
            float intery = yend + gradient;

            xend = (float)Math.Round(x1);
            yend = y1 + gradient * (xend - x1);
            xgap = (float)frac(x1 + 0.5);
            float xpxl2 = xend;
            float ypxl2 = (float)Math.Floor(yend);
            List<byte> rrr = new List<byte>();
            if (steep)
            {
                rrr.Add(bytify(1 - frac(intery)));
                rrr.Add(bytify(frac(intery)));
                fbitmap.SetPixel(new System.Drawing.Point((int)ypxl2, (int)xpxl2), Color.FromArgb(bytify((1 - frac(yend)) * xgap), color.R, color.G, color.B));
                fbitmap.SetPixel(new System.Drawing.Point((int)ypxl2 + 1, (int)xpxl2), Color.FromArgb(bytify(frac(yend) * xgap), color.R, color.G, color.B));
            }
            else
            {
                rrr.Add(bytify(1 - frac(intery)));
                rrr.Add(bytify(frac(intery)));
                fbitmap.SetPixel(new System.Drawing.Point((int)xpxl2, (int)ypxl2), Color.FromArgb(bytify((1 - frac(yend)) * xgap), color.R, color.G, color.B));
                fbitmap.SetPixel(new System.Drawing.Point((int)xpxl2, (int)ypxl2 + 1), Color.FromArgb(bytify(frac(yend) * xgap), color.R, color.G, color.B));
            }

            if (steep)
            {
                for (var x = xpxl1 + 1; x < xpxl2; x++)
                {
                    rrr.Add(bytify(1 - frac(intery)));
                    rrr.Add(bytify(frac(intery)));
                    fbitmap.SetPixel(new System.Drawing.Point((int)Math.Floor(intery), (int)x), Color.FromArgb(bytify(1 - frac(intery)), color.R, color.G, color.B));
                    fbitmap.SetPixel(new System.Drawing.Point((int)Math.Floor(intery) + 1, (int)x), Color.FromArgb(bytify(frac(intery)), color.R, color.G, color.B));
                    intery += gradient;
                }
            }
            else
            {
                for (var x = xpxl1 + 1; x < xpxl2; x++)
                {
                    rrr.Add(bytify(1 - frac(intery)));
                    rrr.Add(bytify(frac(intery)));
                    fbitmap.SetPixel(new System.Drawing.Point((int)x, (int)Math.Floor(intery)), Color.FromArgb(bytify(1 - frac(intery)), color.R, color.G, color.B));
                    fbitmap.SetPixel(new System.Drawing.Point((int)x, (int)Math.Floor(intery) + 1), Color.FromArgb(bytify(frac(intery)), color.R, color.G, color.B));
                    intery += gradient;
                }
            }
        }

    }

    /// <summary>
    /// Я сдался после 8 часов попыток и взял все идеи отсюда: https://habr.com/ru/post/327604/
    /// </summary>
    public class Camera
    {
        Vector cameraPos;
        public double currentAnglePolar;
        public double currentAzimuthalAlpha;
        Vector worldUp;
        public Vector cameraRight;
        public Vector cameraFront;
        public Vector cameraUp;
        const double cameraSpeed = 5;

        //public Vector Vector { get { return cameraDirection; } }
        public Point Location { get { return new Point(cameraPos.x, cameraPos.y, cameraPos.z); } }

        public Camera()
        {
            cameraPos = new Vector(0,0,0);
            currentAnglePolar = 0;
            currentAzimuthalAlpha = -90;
            worldUp = new Vector(0, 1, 0);
            cameraFront = new Vector(0, 0, -1);
            updateVectors();
        }

        public Matrix LookAt { get { return lookAt(cameraPos, cameraPos + cameraFront, cameraUp); } }

        Matrix lookAt(Vector eye, Vector target, Vector upDir)
        {
            // compute the forward vector from target to eye 
            Vector forward = eye - target;
            forward.normalize();                 // make unit length 

            // compute the left vector 
            Vector left = upDir * forward; // cross product 
            left.normalize();

            // recompute the orthonormal up vector 
            Vector up = forward * left;    // cross product 

            return new Matrix(4,4).fill(left.x,up.x,forward.x,0, left.y, up.y, forward.y,0, left.z, up.z, forward.z,0,(-left.x * eye.x - left.y * eye.y - left.z * eye.z), -up.x * eye.x - up.y * eye.y - up.z * eye.z, -forward.x * eye.x - forward.y * eye.y - forward.z * eye.z,1);         
        }

        public void changeViewAngle(double shiftY, double shiftX)
        {
            currentAnglePolar += shiftY;
            if (currentAnglePolar > 89)
            {
                currentAnglePolar = 89;
            }
            if (currentAnglePolar < -89)
            {
                currentAnglePolar = -89;
            }
            currentAzimuthalAlpha = (currentAzimuthalAlpha + shiftX) % 360;
            //vectorOfView = PolarCoords.polarToCarthesian(currentAnglePolar, currentAzimuthalAlpha);
            //cameraFront = new Vector(Geometry.Cos(currentAnglePolar) * Geometry.Cos(currentAzimuthalAlpha), Geometry.Sin(currentAnglePolar), Geometry.Cos(currentAnglePolar) * Geometry.Sin(currentAzimuthalAlpha), isVectorNeededToBeNormalized: true);
            updateVectors();
        }

        void updateVectors()
        {
            cameraFront = new Vector(Geometry.Cos(currentAnglePolar) * Geometry.Cos(currentAzimuthalAlpha), Geometry.Sin(currentAnglePolar), Geometry.Cos(currentAnglePolar) * Geometry.Sin(currentAzimuthalAlpha), isVectorNeededToBeNormalized: true);
            cameraRight = (cameraFront * worldUp).normalize();
            cameraUp = (cameraRight * cameraFront).normalize();
        }

        public void move(char dir)
        {
            switch (dir)
            {
                case 'f': cameraPos += cameraSpeed * cameraFront; break;
                case 'b': cameraPos -= cameraSpeed * cameraFront; break;
                case 'l': cameraPos -= cameraSpeed * cameraRight; break;
                case 'r': cameraPos += cameraSpeed * cameraRight; break;
            }
        }
    }
}
