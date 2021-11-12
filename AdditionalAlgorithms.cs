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

        // поиск нелицевых граней
        Shape findNonFacial(Shape shape, Camera camera)
        {
            Shape newShape;
            double cos = 0.0;
            switch (shape.getShapeName())
            {
                case "TETRAHEDRON":
                    newShape = new Tetrahedron();
                    break;
                case "HEXAHEDRON":
                    newShape = new Hexahedron();
                    break;
                case "OCTAHEDRON":
                    newShape = new Octahedron();
                    break;
                case "ICOSAHEDRON":
                    newShape = new Icosahedron();
                    break;
                case "DODECAHEDRON":
                    newShape = new Dodecahedron();
                    break;
                case "SURFACESEGMENT":
                    newShape = new SurfaceSegment();
                    break;
                case "ROTATIONSHAPE":
                    newShape = new RotationShape();
                    break;
                default:
                    throw new Exception("Такой фигуры нет :с");
            }

            // вектор обзора
            Vector vect = camera.cameraDirection;

            int sumX = 0, sumY = 0, sumZ = 0;
            foreach (var face in shape.Faces)
            {
                sumX += face.getCenter().X;
                sumY += face.getCenter().Y;
                sumZ += face.getCenter().Z;
            }
            // центр фигуры
            Point center = new Point(sumX / shape.Faces.Count(), sumY / shape.Faces.Count(), sumZ / shape.Faces.Count());

            // вектор проекции
            //Vector vecProec = new Vector(center) - vect;

            foreach (Face face in shape.Faces) // для каждой грани фигуры
            {
                // вектор одного ребра грани
                /* int firstVecX = face.Edges[0].End.X - face.Edges[0].Start.X;
                 int firstVecY = face.Edges[0].End.Y - face.Edges[0].Start.Y;
                 int firstVecZ = face.Edges[0].End.Z - face.Edges[0].Start.Z;*/

                // вектор второго ребра грани
                /* int secondVecX = face.Edges[1].Start.X - face.Edges[1].End.X;
                 int secondVecY = face.Edges[1].Start.Y - face.Edges[1].End.Y;
                 int secondVecZ = face.Edges[1].Start.Z - face.Edges[1].End.Z;*/
                /* int secondVecX = face.Edges[face.Edges.Count() - 1].Start.X - face.Edges[face.Edges.Count() - 1].End.X;
                 int secondVecY = face.Edges[face.Edges.Count() - 1].Start.Y - face.Edges[face.Edges.Count() - 1].End.Y;
                 int secondVecZ = face.Edges[face.Edges.Count() - 1].Start.Z - face.Edges[face.Edges.Count() - 1].End.Z;*/

                // вектор третьего ребра грани
                /*  int thirdVecX = face.Edges[2].End.X - face.Edges[2].Start.X;
                  int thirdVecY = face.Edges[2].End.Y - face.Edges[2].Start.Y;
                  int thirdVecZ = face.Edges[2].End.Z - face.Edges[2].Start.Z;*/
                /* int thirdVecX = face.Edges[face.Edges.Count() - 1].Start.X - face.Edges[face.Edges.Count() - 1].End.X;
                 int thirdVecY = face.Edges[face.Edges.Count() - 1].Start.Y - face.Edges[face.Edges.Count() - 1].End.Y;
                 int thirdVecZ = face.Edges[face.Edges.Count() - 1].Start.Z - face.Edges[face.Edges.Count() - 1].End.Z;*/

                //Vector vect1 = new Vector(firstVecX - secondVecX, firstVecY - secondVecY, firstVecZ - secondVecZ);
                //Vector vect2 = new Vector(secondVecX - thirdVecX, secondVecY - thirdVecY, secondVecZ - thirdVecZ);
                //Vector vect1 = new Vector(firstVecX, firstVecY, firstVecZ);
                //Vector vect2 = new Vector(secondVecX, secondVecY, secondVecZ);

                // вектор нормали грани 
                //Vector vectNoraml = vect1 * vect; // векторное произведение
                Vector vectNoraml = face.NormVector;


                //int vectScalar = vectNoraml.X * vecProec.X + vectNoraml.Y * vecProec.Y + vectNoraml.Z * vecProec.Z; // скалярное произведение
                int vectScalar = vectNoraml.X * vect.X + vectNoraml.Y * vect.Y + vectNoraml.Z * vect.Z; // скалярное произведение

                //var len = Math.Sqrt(vectNoraml.X * vectNoraml.X + vectNoraml.Y * vectNoraml.Y + vectNoraml.Z * vectNoraml.Z) * Math.Sqrt(vecProec.X * vecProec.X + vecProec.Y * vecProec.Y + vecProec.Z * vecProec.Z);
                var len = Math.Sqrt(vectNoraml.X * vectNoraml.X + vectNoraml.Y * vectNoraml.Y + vectNoraml.Z * vectNoraml.Z) * Math.Sqrt(vect.X * vect.X + vect.Y * vect.Y + vect.Z * vect.Z);

                cos = len != 0 ? vectScalar / len : 0;

                if (cos < 0)
                    newShape.addFace(face);
            }
            return newShape;
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
            cameraUp = new Vector(0, 0, 1);
            cameraRight = (cameraDirection * cameraUp).normalize();
        }

        public void move(double leftright = 0, double forwardbackward = 0, double updown = 0)
        {
            cameraPosition.Xf += leftright * cameraRight.x + forwardbackward * cameraDirection.x + updown * cameraUp.x;
            cameraPosition.Yf += leftright * cameraRight.y + forwardbackward * cameraDirection.y + updown * cameraUp.y;
            cameraPosition.Zf += leftright * cameraRight.z + forwardbackward * cameraDirection.z + updown * cameraUp.z;
        }

        public Point toCameraView(Point p)
        {
            return new Point(cameraRight.x * (p.Xf - cameraPosition.Xf) + cameraRight.y * (p.Yf - cameraPosition.Yf) + cameraRight.z * (p.Zf - cameraPosition.Zf),
                             cameraUp.x * (p.Xf - cameraPosition.Xf) + cameraUp.y * (p.Yf - cameraPosition.Yf) + cameraUp.z * (p.Zf - cameraPosition.Zf),
                             cameraDirection.x * (p.Xf - cameraPosition.Xf) + cameraDirection.y * (p.Yf - cameraPosition.Yf) + cameraDirection.z * (p.Zf - cameraPosition.Zf));
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
