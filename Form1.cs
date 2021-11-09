using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _3DFacesProcessing
{
    public partial class Form1 : Form
    {
        BindingList<Shape> sceneShapes;
        bool isMoving = false;
        Camera camera;
        public Form1()
        {
            sceneShapes = new BindingList<Shape>();
            InitializeComponent();
            listBox.DataSource = sceneShapes;
            canvas.Image = new Bitmap(canvas.Width, canvas.Height);
            camera = new Camera(new Point(200, 0, 0));
            // А здесь задаём точку начала координат
            Point.worldCenter = new PointF(canvas.Width / 2, canvas.Height / 2);
            Point.projection = ProjectionType.TRIMETRIC;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var form = new FormEditShape();
            form.Show();
            Point.projection = ProjectionType.PERSPECTIVE;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                sceneShapes.Add(Shape.readShape(openFileDialog1.FileName));
                changeToolsAccessibility(true);
                redrawScene();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            sceneShapes.Remove((Shape)listBox.SelectedValue);
            if(sceneShapes.Count == 0)
            {
                changeToolsAccessibility(false);
            }
            redrawScene();
        }

        void changeToolsAccessibility(bool isOn)
        {
            btnDelete.Enabled = isOn;
            textShiftX.Enabled = isOn;
            textShiftY.Enabled = isOn;
            textShiftZ.Enabled = isOn;
            buttonShift.Enabled = isOn;
            btnShowAxis.Enabled = isOn;
        }

        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            redrawScene();
        }

        private void rbParallel_CheckedChanged(object sender, EventArgs e)
        {
            if (rbParallel.Checked)
            {
                Point.projection = ProjectionType.PARALLEL;
            }
            else
            {
                Point.projection = ProjectionType.PERSPECTIVE;
            }
            redrawScene();
        }

        System.Drawing.Point previousLocation;
        private void canvas_MouseDown(object sender, MouseEventArgs e)
        {
            isMoving = true;
            previousLocation = e.Location;
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMoving)
            {
                camera.changeViewAngle((e.Y - previousLocation.Y) / 10.0, (e.X - previousLocation.X) / -10.0);
                label3.Text = $"{camera.Vector} => {Math.Round(camera.currentAngleAlpha,2)}/{Math.Round(camera.currentAnglePolar,2)}";
                previousLocation = e.Location;
                redrawScene();
            }
        }

        private void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            isMoving = false;
        }

        private void buttonShift_Click(object sender, EventArgs e)
        {
            sceneShapes[listBox.SelectedIndex] = AffineTransformations.shift(sceneShapes[listBox.SelectedIndex], int.Parse(textShiftX.Text), int.Parse(textShiftY.Text), int.Parse(textShiftZ.Text));
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case 'w': camera.move(shiftY: 5); break;
                case 'a': camera.move(shiftX: -5); break;
                case 's': camera.move(shiftY: -5); break;
                case 'd': camera.move(shiftX: 5); break;
                case 'q': camera.move(shiftZ: 5); break;
                case 'e': camera.move(shiftZ: -5); break;
            }
            redrawScene();
            e.Handled = true;
        }
    }
}
