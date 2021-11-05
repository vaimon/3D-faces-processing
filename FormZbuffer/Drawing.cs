using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DFacesProcessing
{
    public partial class FormZbuffer
    {
        bool isAxisVisible = false;
        bool arePointsVisible = false;
        ShapeType currentShapeType;
        Shape currentShape;
        Graphics g;

        private void btnShowAxis_Click(object sender, EventArgs e)
        {
            isAxisVisible = !isAxisVisible;
            redraw();
            btnShowAxis.Text = isAxisVisible ? "Скрыть оси" : "Показать оси";
        }

        private void btnShowPoints_Click(object sender, EventArgs e)
        {
            arePointsVisible = !arePointsVisible;
            redraw();
            btnShowPoints.Text = arePointsVisible ? "Скрыть точки" : "Показать точки";
        }
        private void buttonShape_Click(object sender, EventArgs e)
        {
            if (isInteractiveMode)
            {
                setFlags(false);
                g.Clear(Color.White);
                RotationShapePoints.Clear();
            }
            else
            {
                if (tabControl.SelectedIndex == 0)
                {
                    currentShape = ShapeGetter.getShape(currentShapeType);
                }
                else
                {
                    if (tabControl.SelectedIndex == 1)
                    {
                        currentShape = ShapeGetter.getSurfaceSegment(currentFun, int.Parse(etX0.Text), int.Parse(etX1.Text), int.Parse(etY0.Text), int.Parse(etY1.Text), int.Parse(etSplit.Text));
                    }
                    else
                    {
                        Div = int.Parse(getDiv.Text);
                        currentShape = ShapeGetter.getRotationShape(RotationShapePoints, Div, AxisforRotate);
                    }
                }
                
                redraw();
                setFlags(true);
            }
            
        }

        /// <summary>
        /// Рисует фигуры на канвасе, выделяя цветом некоторые грани у додекаэдра и икосаэра
        /// </summary>
        /// <param name="shape">Фигура, которую надо нарисовать</param>
        void drawShape(Shape shape)
        {
            if (shape is Icosahedron)
            {
                Pen pen = new Pen(Color.Blue, 3);
                for (int i = 11; i < 15; i++)
                {
                    Face face = shape.Faces[i];
                    drawFace(face, pen);
                }
                pen = new Pen(Color.Black, 3);
                for (int i = 0; i < 10; i++)
                {
                    Face face = shape.Faces[i];
                    drawFace(face, pen);
                }
                pen = new Pen(Color.Red, 3);
                for (int i = 15; i < 20; i++)
                {
                    Face face = shape.Faces[i];
                    drawFace(face, pen);
                }
                return;
            }
            if (shape is Dodecahedron)
            {
                for (int i = 0; i < 10; i++)
                {
                    Face face = shape.Faces[i];
                    Pen pen = new Pen(Color.Red, 3);
                    if(i % 2 == 0)
                    {
                        pen = new Pen(Color.Blue, 3);
                    }
                    for (int j = 0; j < face.Edges.Count; j++)
                    {
                        Line line = face.Edges[j];
                        if(j < 2)
                        {
                            drawLine(line, new Pen(Color.Black, 3));
                            continue;
                        }
                        drawLine(line, pen);
                    }
                }
                return;
            }
            foreach (var face in shape.Faces)
            {
                Pen pen = new Pen(Color.Black, 3);
                drawFace(face,pen);
            }
        }

        /// <summary>
        /// Рисует заданную границу грани заданным цветом
        /// </summary>
        /// <param name="face">Грань, которую надо нарисовать</param>
        /// <param name="pen">Цвет границы</param>
        void drawFace(Face face, Pen pen)
        {
            foreach(var line in face.Edges)
            {
                drawLine(line, pen);
            }
        }

        /// <summary>
        /// Рисует линию, переводя её координаты из 3D в 2D
        /// </summary>
        /// <param name="line">Линия, которую надо нарисовать</param>
        /// <param name="pen">Цвет линии</param>
        void drawLine(Line line, Pen pen)
        {
            g.DrawLine(pen, line.Start.to2D(), line.End.to2D());
        }

        /// <summary>
        /// Рисует коодинатные прямые (с подписями) и подписывает координаты каждой точки
        /// </summary>
        void drawAxis()
        {
            Line axisX = new Line(new Point(0, 0, 0), new Point(300, 0, 0));
            Line axisY = new Line(new Point(0, 0, 0), new Point(0, 300, 0));
            Line axisZ = new Line(new Point(0, 0, 0), new Point(0, 0, 300));
            drawLine(axisX, new Pen(Color.Red, 4));
            drawLine(axisY, new Pen(Color.Blue, 4));
            drawLine(axisZ, new Pen(Color.Green, 4));
            g.ScaleTransform(1.0F, -1.0F);
            g.TranslateTransform(0.0F, -(float)canvas.Height);
            g.DrawString($" X", new Font("Arial", 10, FontStyle.Regular), new SolidBrush(Color.Red), axisX.End.to2D().X, canvas.Height - axisX.End.to2D().Y);
            g.DrawString($" Y", new Font("Arial", 10, FontStyle.Regular), new SolidBrush(Color.Blue), axisY.End.to2D().X, canvas.Height - axisY.End.to2D().Y);
            g.DrawString($" Z", new Font("Arial", 10, FontStyle.Regular), new SolidBrush(Color.Green), axisZ.End.to2D().X, canvas.Height - axisZ.End.to2D().Y);
            g.ScaleTransform(1.0F, -1.0F);
            g.TranslateTransform(0.0F, -(float)canvas.Height);
        }

        void drawPoints()
        {
            g.ScaleTransform(1.0F, -1.0F);
            g.TranslateTransform(0.0F, -(float)canvas.Height);
            foreach (var face in currentShape.Faces)
            {
                foreach (var line in face.Edges)
                {
                    g.DrawString($" ({line.Start.X}, {line.Start.Y}, {line.Start.Z})", new Font("Arial", 8, FontStyle.Italic), new SolidBrush(Color.DarkBlue), line.Start.to2D().X, canvas.Height - line.Start.to2D().Y);
                    g.DrawString($" ({line.End.X}, {line.End.Y}, {line.End.Z})", new Font("Arial", 8, FontStyle.Italic), new SolidBrush(Color.DarkBlue), line.End.to2D().X, canvas.Height - line.End.to2D().Y);
                }
            }
            g.ScaleTransform(1.0F, -1.0F);
            g.TranslateTransform(0.0F, -(float)canvas.Height);
        }

        /// <summary>
        /// Перерисовывает всю сцену
        /// </summary>
        void redraw()
        {
            g.Clear(Color.White);
            
            if (isAxisVisible)
            {
                drawAxis();
            }
            if (arePointsVisible)
            {
                drawPoints();
            }

            drawShape(currentShape);
        }
    }
}
