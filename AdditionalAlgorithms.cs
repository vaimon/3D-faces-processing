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

    public class Camera
    {
        public Point cameraPosition;
        public Vector cameraDirection;
        public Vector cameraUp;
        public Vector cameraRight;
        const double cameraRotationSpeed = 1;
        double yaw = 0.0, pitch = 0.0;

        public Camera()
        {
            cameraPosition = new Point(-10, 0, 0);
            cameraDirection = new Vector(1, 0, 0);
            cameraUp = new Vector(0, 1, 0);
            cameraRight = (cameraDirection * cameraUp).normalize();
        }

        public void move(double shiftX = 0, double shiftY = 0, double shiftZ = 0)
        {
            cameraPosition.Xf += shiftX;
            cameraPosition.Yf += shiftY;
            cameraPosition.Zf += shiftZ;
        }

        public Point toCameraView(Point p)
        {
            //return new Point(cameraRight.x * (p.Xf - cameraPosition.Xf) + cameraRight.y * (p.Yf - cameraPosition.Yf) + cameraRight.z * (p.Zf - cameraPosition.Zf),
            //                 cameraUp.x * (p.Xf - cameraPosition.Xf) + cameraUp.y * (p.Yf - cameraPosition.Yf) + cameraUp.z * (p.Zf - cameraPosition.Zf),
            //                 cameraDirection.x * (p.Xf - cameraPosition.Xf) + cameraDirection.y * (p.Yf - cameraPosition.Yf) + cameraDirection.z * (p.Zf - cameraPosition.Zf));
            var res = cameraMatrix() * new Matrix(4, 1).fill(p.Xf, p.Yf, p.Zf, 1);
            return new Point(res[0, 0], res[1, 0], res[2, 0]);
        }

        public Matrix cameraMatrix()
        {
            return new Matrix(4, 4).fill(1, 0, 0, 0,
                                        0, 1, 0, 0,
                                        0, 0, 1, -200,
                                        0, 0, 0, 1) *
                new Matrix(4, 4).fill(1, 0, 0, 0, 0, Geometry.Cos(pitch), -Geometry.Sin(pitch), 0, 0, Geometry.Sin(pitch), Geometry.Cos(pitch), 0, 0, 0, 0, 1) *
                new Matrix(4, 4).fill(Geometry.Cos(yaw), 0, Geometry.Sin(yaw), 0, 0, 1, 0, 0, -Geometry.Sin(yaw), 0, Geometry.Cos(yaw), 0, 0, 0, 0, 1) *
                new Matrix(4, 4).fill(1, 0, 0, 0,
                                        0, 1, 0, 0,
                                        0, 0, 1, 200,
                                        0, 0, 0, 1) *
                new Matrix(4, 4).fill(1, 0, 0, -cameraPosition.Xf,
                                        0, 1, 0, -cameraPosition.Yf,
                                        0, 0, 1, -cameraPosition.Zf,
                                        0, 0, 0, 1);
        }

        public void changeView(double shiftX = 0, double shiftY = 0)
        {
            var newPitch = Math.Clamp(pitch + shiftY * cameraRotationSpeed,-89.0,89.0);
            var newYaw = (yaw + shiftX) % 360;
            if(newPitch != pitch)
            {
                AffineTransformations.rotateVectors(ref cameraDirection, ref cameraUp, (newPitch - pitch), cameraRight);
                pitch = newPitch;
            }
            if(newYaw != yaw)
            {
                AffineTransformations.rotateVectors(ref cameraDirection, ref cameraRight, (newYaw - yaw), cameraUp);
                yaw = newYaw;
            }
        }
    }
}
