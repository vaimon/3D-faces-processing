using _3DShapes;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace _3DFacesProcessing
{
    public partial class FormCutoff : System.Windows.Forms.Form
    {
        bool isInteractiveMode = false;
        Func<double, double, double> currentFun;
        List<Point> RotationShapePoints=new List<Point>();
        int Div;
        AxisType AxisforRotate;
        public AxisType currentAxis;
        public bool isScaleModeWorldCenter = true;


        public FormCutoff()
        {
            InitializeComponent();
            selectShape.SelectedIndex = 0;
            selectAxis.SelectedIndex = 0;
            axizRotate.SelectedIndex = 0;
            g = canvas.CreateGraphics();

            // Здесь мы задаём Декартову систему координат на канвасе
            g.ScaleTransform(1.0F, -1.0F);
            g.TranslateTransform(0.0F, -(float)canvas.Height);

            // А здесь задаём точку начала координат
            Point.worldCenter = new PointF(canvas.Width / 2, canvas.Height / 2);
            Point.projection = ProjectionType.TRIMETRIC;
            setFlags();
        }
        void setFlags(bool interactiveMode = false)
        {
            isInteractiveMode = interactiveMode;
            selectAxis.Enabled = interactiveMode;
            buttonRotate.Enabled = interactiveMode;
            buttonScale.Enabled = interactiveMode;
            buttonShift.Enabled = interactiveMode;
            rbAxonometric.Enabled = interactiveMode;
            rbPerspective.Enabled = interactiveMode;
            rbIsometric.Enabled = interactiveMode;
            btnShowAxis.Enabled = interactiveMode;
            textAngle.Enabled = interactiveMode;
            textScaleX.Enabled = interactiveMode;
            textScaleY.Enabled = interactiveMode;
            textScaleZ.Enabled = interactiveMode;
            textShiftX.Enabled = interactiveMode;
            textShiftY.Enabled = interactiveMode;
            textShiftZ.Enabled = interactiveMode;
            rbAxonometric.Enabled = interactiveMode;
            rbWorldCenter.Enabled = interactiveMode;
            rbCenter.Enabled = interactiveMode;
            rbDimetric.Enabled = interactiveMode;
            btnShowPoints.Enabled = interactiveMode;

            btnChoosePlot.Enabled = !interactiveMode;
            etSplit.Enabled = !interactiveMode;
            etX0.Enabled = !interactiveMode;
            etX1.Enabled = !interactiveMode;
            etY0.Enabled = !interactiveMode;
            etY1.Enabled = !interactiveMode;
            tabControl.Enabled = !interactiveMode;
            btnLoad.Text = interactiveMode ? "Сохранить" : "Загрузить из файла";
            buttonShape.Text = interactiveMode ? "Очистить" : "Нарисовать";
            selectShape.Enabled = !interactiveMode;
        }

        private void comboBoxShape_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (selectShape.SelectedIndex)
            {
                case 0: currentShapeType = ShapeType.TETRAHEDRON; break;
                case 1: currentShapeType = ShapeType.HEXAHEDRON; break;
                case 2: currentShapeType = ShapeType.OCTAHEDRON; break;
                case 3: currentShapeType = ShapeType.ICOSAHEDRON; break;
                case 4: currentShapeType = ShapeType.DODECAHEDRON; break;
                default: throw new Exception("Фигурки всё сломали :(");
            }
        }

        private void selectAxis_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (selectAxis.SelectedIndex)
            {
                case 0: currentAxis = AxisType.X; break;
                case 1: currentAxis = AxisType.Y; break;
                case 2: currentAxis = AxisType.Z; break;
                default: throw new Exception("Оси всё сломали :(");
            }
        }

        private void rbPerspective_CheckedChanged(object sender, EventArgs e)
        {
            if (rbPerspective.Checked)
            {
                Point.projection = ProjectionType.PERSPECTIVE;
                redraw();
            }
        }

        private void rbIsometric_CheckedChanged(object sender, EventArgs e)
        {
            if (rbIsometric.Checked)
            {
                Point.projection = ProjectionType.ISOMETRIC;
                redraw();
            }
        }

        private void rbAxonometric_CheckedChanged(object sender, EventArgs e)
        {
            if (rbAxonometric.Checked)
            {
                Point.projection = ProjectionType.TRIMETRIC;
                redraw();
            }
        }

        private void textScaleX_TextChanged(object sender, EventArgs e)
        {
            if(textScaleX.Text == "")
            {
                textScaleX.Text = "1";
            }
        }

        private void textScaleY_TextChanged(object sender, EventArgs e)
        {
            if (textScaleY.Text == "")
            {
                textScaleY.Text = "1";
            }
        }

        private void textScaleZ_TextChanged(object sender, EventArgs e)
        {
            if (textScaleZ.Text == "")
            {
                textScaleZ.Text = "1";
            }
        }

        private void textShiftX_TextChanged(object sender, EventArgs e)
        {
            if (textShiftX.Text == "")
            {
                textShiftX.Text = "0";
            }
        }

        private void textShiftY_TextChanged(object sender, EventArgs e)
        {
            if (textShiftY.Text == "")
            {
                textShiftY.Text = "0";
            }
        }

        private void textShiftZ_TextChanged(object sender, EventArgs e)
        {
            if (textShiftZ.Text == "")
            {
                textShiftZ.Text = "0";
            }
        }

        private void rbWorldCenter_CheckedChanged(object sender, EventArgs e)
        {
            isScaleModeWorldCenter = rbWorldCenter.Checked;
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (btnLoad.Text == "Загрузить из файла")
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string fileName = openFileDialog1.FileName;
                    if (File.Exists(fileName))
                    {
                        currentShape = Shape.readShape(fileName);
                        redraw();
                        setFlags(true);
                    }
                }
            }
            else
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    currentShape.saveShape(saveFileDialog1.FileName);
            }
        }

        private void btnChoosePlot_Click(object sender, EventArgs e)
        {
            using (var form = new ChooseFunForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    currentFun = form.SelectedFunction;
                    pbFormula.Size = form.ImageSize;
                    pbFormula.Image = form.Formula;
                }
            }
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            pbFormula.Visible = tabControl.SelectedIndex == 1;
        }

        private void AddPoint_Click(object sender, EventArgs e)
        {
            int x = int.Parse(getX.Text);
            int y = int.Parse(getY.Text);
            int z = int.Parse(getZ.Text);
            Point p = new Point(x,y,z);
            RotationShapePoints.Add( p);
            //Div = int.Parse(getDiv.Text);
        }



        private void axizRotate_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (axizRotate.SelectedIndex)
            {
                case 0: AxisforRotate = AxisType.X; break;
                case 1: AxisforRotate = AxisType.Y; break;
                case 2: AxisforRotate = AxisType.Z; break;
                default: throw new Exception("Оси всё сломали :(");
            }
        }

        private void rbDimetric_CheckedChanged_1(object sender, EventArgs e)
        {
            if (rbDimetric.Checked)
            {
                Point.projection = ProjectionType.DIMETRIC;
                redraw();
            }
        }

        private void buttonScale_Click(object sender, EventArgs e)
        {
            if (isScaleModeWorldCenter)
            {
                AffineTransformations.scale(ref currentShape, double.Parse(textScaleX.Text.Replace('.', ',')), double.Parse(textScaleY.Text.Replace('.', ',')), double.Parse(textScaleZ.Text.Replace('.', ',')));
                redraw();
            }
            else
            {
                // TODO: scale from shape center
                double sumX = 0, sumY = 0, sumZ = 0;
                foreach (var face in currentShape.Faces)
                {
                    sumX += face.getCenter().Xf;
                    sumY += face.getCenter().Yf;
                    sumZ += face.getCenter().Zf;
                }

                Point center = new Point(sumX / currentShape.Faces.Count(), sumY / currentShape.Faces.Count(), sumZ / currentShape.Faces.Count());
                double cx = double.Parse(textScaleX.Text.Replace('.', ','));
                double cy = double.Parse(textScaleY.Text.Replace('.', ','));
                double cz = double.Parse(textScaleZ.Text.Replace('.', ','));
                AffineTransformations.shift(ref currentShape, -center.Xf, -center.Yf, -center.Zf);
                AffineTransformations.scale(ref currentShape, cx, cy, cz);
                AffineTransformations.shift(ref currentShape, center.Xf, center.Yf, center.Zf);
                redraw();
            }

        }
        private void buttonRotate_Click(object sender, EventArgs e)
        {
            AffineTransformations.rotate(ref currentShape, currentAxis, int.Parse(textAngle.Text));
            redraw();
        }

        private void buttonShift_Click(object sender, EventArgs e)
        {
            AffineTransformations.shift(ref currentShape, int.Parse(textShiftX.Text), int.Parse(textShiftY.Text), int.Parse(textShiftZ.Text));
            redraw();
        }

    }
}
