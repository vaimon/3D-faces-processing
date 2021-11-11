﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DFacesProcessing
{
    using FastBitmap;
    public partial class Form1
    {
        bool isAxisVisible = false;
        //Graphics g;
        Pen blackPen = new Pen(Color.Black, 3);
        Pen highlightPen = new Pen(Color.DarkRed, 3);
        FastBitmap fbitmap;

        private void btnShowAxis_Click(object sender, EventArgs e)
        {
            isAxisVisible = !isAxisVisible;
            redrawScene();
            btnShowAxis.Text = isAxisVisible ? "Скрыть оси" : "Показать оси";
        }

        /// <summary>
        /// Рисует фигуры на канвасе, выделяя цветом выбранную фигуру
        /// </summary>
        /// <param name="shape">Фигура, которую надо нарисовать</param>
        void drawShape(Shape shape, Pen pen)
        {
            foreach (var face in shape.Faces)
            {
                drawFace(face, pen);
            }
        }

        /// <summary>
        /// Рисует заданную границу грани заданным цветом
        /// </summary>
        /// <param name="face">Грань, которую надо нарисовать</param>
        /// <param name="pen">Цвет границы</param>
        void drawFace(Face face, Pen pen)
        {
            foreach (var line in face.Edges)
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
            var pf1 = line.Start.to2D(camera);
            var pf2 = line.End.to2D(camera);
            drawVuLine(new System.Drawing.Point((int)pf1.Value.X, (int)(canvas.Height - pf1.Value.Y)), new System.Drawing.Point((int)pf2.Value.X, (int)(canvas.Height - pf2.Value.Y)), pen.Color);
            //drawVuLine(new System.Drawing.Point((int)pf1.X, (int)(canvas.Height - pf1.Y)), new System.Drawing.Point((int)pf2.X, (int)(canvas.Height - pf2.Y)), pen.Color);
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
            //g.DrawString($" X", new Font("Arial", 10, FontStyle.Regular), new SolidBrush(Color.Red), axisX.End.to2D().X, canvas.Height - axisX.End.to2D().Y);
            //g.DrawString($" Y", new Font("Arial", 10, FontStyle.Regular), new SolidBrush(Color.Blue), axisY.End.to2D().X, canvas.Height - axisY.End.to2D().Y);
            //g.DrawString($" Z", new Font("Arial", 10, FontStyle.Regular), new SolidBrush(Color.Green), axisZ.End.to2D().X, canvas.Height - axisZ.End.to2D().Y);
        }

        /// <summary>
        /// Перерисовывает всю сцену
        /// </summary>
        void redrawScene()
        {
            //g.Clear(Color.White);
            var bitmap = new Bitmap(canvas.Width,canvas.Height);
            fbitmap = new FastBitmap(bitmap);
            if (isAxisVisible)
            {
                drawAxis();
            }
            for (int i = 0; i < sceneShapes.Count; i++)
            {
                if(i == listBox.SelectedIndex)
                {
                    drawShape(sceneShapes[i], highlightPen);
                    continue;
                }
                drawShape(sceneShapes[i], blackPen);
            }
            fbitmap.Dispose();
            canvas.Image = bitmap;         
        }
    }
}
