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
            /*           label3.Text = "";
                        Shape newShape;
                        //double cos = 0.0;
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
                        //Vector vect = new Vector(camera.cameraPosition);
                        Vector vect = camera.cameraDirection;
            */
            /*     int sumX = 0, sumY = 0, sumZ = 0;
                 foreach (var face in shape.Faces)
                 {
                     sumX += face.getCenter().X;
                     sumY += face.getCenter().Y;
                     sumZ += face.getCenter().Z;
                 }

                 // центр фигуры
                 Point centerFig = new Point(sumX / shape.Faces.Count(), sumY / shape.Faces.Count(), sumZ / shape.Faces.Count());*/

            /*            foreach (Face face in shape.Faces) // для каждой грани фигуры
                        {
                            // вектор нормали грани 
                            //Vector vectNoraml = face.NormVector;

                            //int firstVecX = face.Edges[0].Start.X - face.Edges[0].End.X;
                            //int firstVecY = face.Edges[0].Start.Y - face.Edges[0].End.Y;
                            //int firstVecZ = face.Edges[0].Start.Z - face.Edges[0].End.Z;

                            // вектор второго ребра грани
                            //int secondVecX = face.Edges[face.Edges.Count() - 1].End.X - face.Edges[face.Edges.Count() - 1].Start.X;
                            //int secondVecY = face.Edges[face.Edges.Count() - 1].End.Y - face.Edges[face.Edges.Count() - 1].Start.Y;
                            //int secondVecZ = face.Edges[face.Edges.Count() - 1].End.Z - face.Edges[face.Edges.Count() - 1].Start.Z;

                            //Vector vect1 = new Vector(camera.toCameraView(new Point(firstVecX, firstVecY, firstVecZ)));
                            //Vector vect2 = new Vector(camera.toCameraView(new Point(secondVecX, secondVecY, secondVecZ)));

                            Vector vect1 = new Vector(camera.toCameraView(face.Edges.First().getVectorCoordinates()));
                            Vector vect2 = new Vector(camera.toCameraView(face.Edges.Last().getReverseVectorCoordinates()));
                            Vector vectNoraml = (vect2 * vect1).normalize();

                            //Vector centerFace = new Vector(camera.toCameraView(new Point(centerFig.X - face.getCenter().X, centerFig.Y - face.getCenter().Y, centerFig.Z - face.getCenter().Z)));


                            Point centerFace = camera.toCameraView(face.getCenter());

                            // вектор проецирования
                            Vector vectProec = new Vector(new Point(centerFace.X - vect.X, centerFace.Y - vect.Y, centerFace.Z - vect.Z));

                             double angle = (vectNoraml.X * vectProec.X + vectNoraml.Y * vectProec.Y + vectNoraml.Z * vectProec.Z) /
                               (Math.Sqrt(vectNoraml.X * vectNoraml.X + vectNoraml.Y * vectNoraml.Y + vectNoraml.Z * vectNoraml.Z) *
                                           Math.Sqrt(vectProec.X * vectProec.X + vectProec.Y * vectProec.Y + vectProec.Z * vectProec.Z));

                            //double angle = Math.Acos((vectNoraml.X * vectProec.X + vectNoraml.Y * vectProec.Y + vectNoraml.Z * vectProec.Z) /
                            //               ((Math.Sqrt(vectNoraml.X * vectNoraml.X + vectNoraml.Y * vectNoraml.Y + vectNoraml.Z * vectNoraml.Z) *
                            //                Math.Sqrt(vectProec.X * vectProec.X + vectProec.Y * vectProec.Y + vectProec.Z * vectProec.Z)))) * 180 / Math.PI;

                            //double scalar = (vectNoraml.X * vectProec.X + vectNoraml.Y * vectProec.Y + vectNoraml.Z * vectProec.Z);

                            //double len = Math.Sqrt(vectNoraml.X * vectNoraml.X + vectNoraml.Y * vectNoraml.Y + vectNoraml.Z * vectNoraml.Z) *
                             //              Math.Sqrt(vectProec.X * vectProec.X + vectProec.Y * vectProec.Y + vectProec.Z * vectProec.Z);

                            //var cos = 0.0;
                            //if (len != 0)
                            //    cos = scalar / len;
                            //if (cos > 0)
                             //   newShape.addFace(face);

                            string str = "";
                            foreach (Line ed in face.Edges)
                                str += camera.toCameraView(ed.Start) + "," + camera.toCameraView(ed.End) + " ";
                            str += " Angle: " + angle + " ";
                            str += " Cent: " + centerFace.X + " " + centerFace.Y + " " + centerFace.Z + " ";
                            str += " Proec: " + vectProec.X + " " + vectProec.Y + " " + vectProec.Z + "\n";
                            label3.Text += str;

                            if (angle < 0)
                                newShape.addFace(face);
                        }
                        return newShape;

            */
            //label3.Text = "";
            Shape newShape;
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
            //Vector vect = camera.cameraDirection;
            //Vector vect = new Vector(camera.cameraPosition);

            /*            int sumX = 0, sumY = 0, sumZ = 0;
                        foreach (var face in shape.Faces)
                        {
                            sumX += face.getCenter().X;
                            sumY += face.getCenter().Y;
                            sumZ += face.getCenter().Z;
                        }*/
            // центр фигуры
            //Point center = new Point(sumX / shape.Faces.Count(), sumY / shape.Faces.Count(), sumZ / shape.Faces.Count());


            foreach (Face face in shape.Faces) // для каждой грани фигуры
            {
                Vector vectProec = new Vector(camera.toCameraView(face.getCenter())).normalize();
                //Point center = face.getCenter();
                // вектор проекции
                //Vector proec = new Vector(center);
                //proec = new Vector(camera.toCameraView(new Point(proec.X, proec.Y, proec.Z)));

                /*         double xx = vectProec.X, yy = vectProec.Y, zz = vectProec.Z;
                         double normalizationP = Math.Sqrt(Math.Pow(xx, 2) + Math.Pow(yy, 2) + Math.Pow(zz, 2));
                         xx = xx / normalizationP;
                         yy = yy / normalizationP;
                         zz = zz / normalizationP;
                         if (xx < 1 && yy < 1 && zz < 1)
                         {
                             if (yy == Math.Max(xx, yy))
                             {
                                 if (yy == Math.Max(zz, yy))
                                     yy = 1;
                                 else
                                     zz = 1;
                             }
                             else if (xx == Math.Max(xx, zz))
                                 xx = 1;
                             else
                                 zz = 1;
                         }
                         vectProec.X = xx;
                         vectProec.Y = yy;
                         vectProec.Z = zz;*/

                // вектор одного ребра грани
                //int firstVecX = face.Edges[0].Start.X - face.Edges[0].End.X;
                //int firstVecY = face.Edges[0].Start.Y - face.Edges[0].End.Y;
                //int firstVecZ = face.Edges[0].Start.Z - face.Edges[0].End.Z;

                // вектор второго ребра грани
                //int secondVecX = face.Edges[face.Edges.Count() - 1].End.X - face.Edges[face.Edges.Count() - 1].Start.X;
                //int secondVecY = face.Edges[face.Edges.Count() - 1].End.Y - face.Edges[face.Edges.Count() - 1].Start.Y;
                //int secondVecZ = face.Edges[face.Edges.Count() - 1].End.Z - face.Edges[face.Edges.Count() - 1].EStartnd.Z;

                // вектор нормали грани 
                //Vector vectNoraml = (vect2 * vect1).normalize(); // векторное произведение

             /* вариант 1
               Vector vect1 = new Vector(camera.toCameraView(face.Edges.First().getVectorCoordinates()));
                Vector vect2 = new Vector(camera.toCameraView(face.Edges.Last().getReverseVectorCoordinates()));
                Vector vectNormal = (vect2 * vect1).normalize2();
             */

                /* вариант 2 */
                Vector vect1 = new Vector(face.Edges.First().getVectorCoordinates());
                Vector vect2 = new Vector(face.Edges.Last().getReverseVectorCoordinates());          
                Vector vectNormal = vect1 * vect2;
                vectNormal = new Vector(camera.toCameraView(new Point(vectNormal.X, vectNormal.Y, vectNormal.Z))).normalize();



                /*         double x = vectNormal.X, y = vectNormal.Y, z = vectNormal.Z;
                         double normalization = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2));
                         x = x / normalization;
                         y = y / normalization;
                         z = z / normalization;
                         if (x < 1 && y < 1 && z < 1)
                         {
                             if (y == Math.Max(x, y))
                             {
                                 if (y == Math.Max(z, y))
                                     y = 1;
                                 else
                                     z = 1;
                             }
                             else if (x == Math.Max(x, z))
                                 x = 1;
                             else
                                 z = 1;
                         }
                         vectNormal.X = x;
                         vectNormal.Y = y;
                         vectNormal.Z = z;*/

                double vectScalar = vectNormal.X * vectProec.X + vectNormal.Y * vectProec.Y + vectNormal.Z * vectProec.Z; // скалярное произведение

                //var len = Math.Sqrt(vectNoraml.X * vectNoraml.X + vectNoraml.Y * vectNoraml.Y + vectNoraml.Z * vectNoraml.Z) * Math.Sqrt(proec.X * proec.X + proec.Y * proec.Y + proec.Z * proec.Z);

                //var cos = 0.0;
                //if (len != 0)
                //    cos = vectScalar / len;
                if (vectScalar > 0)
                    newShape.addFace(face);

              /*  string str = "";
                foreach (Line ed in face.Edges)
                    str += camera.toCameraView(ed.Start) + "," + camera.toCameraView(ed.End) + " ";
                str += " Angle: " + vectScalar + " ";
                //str += " Cent: " + center.X + " " + center.Y + " " + center.Z + " ";
                str += " Proec: " + vectProec.X + " " + vectProec.Y + " " + vectProec.Z + "\n";
                label3.Text += str;*/
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
