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
        List<Shape> scene;
        bool isMoving = false;
        Camera camera; 
        List<Color> colorrange;
       
        public Form1()
        {
            sceneShapes = new BindingList<Shape>();
            scene = new List<Shape>();
            InitializeComponent();
            listBox.DataSource = sceneShapes;
            canvas.Image = new Bitmap(canvas.Width, canvas.Height);
            camera = new Camera();
            // А здесь задаём точку начала координат
            Point.worldCenter = new PointF(canvas.Width / 2, canvas.Height / 2);
            Point.projection = ProjectionType.PARALLEL;
            Point.setProjection(canvas.Size, 0.1, 100, 90);
           
           
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
                scene.Add(Shape.readShape(openFileDialog1.FileName));
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
                //camera.changeViewAngle((previousLocation.Y - e.Y) / 10.0, (e.X - previousLocation.X) / 10.0);
                //label5.Text = $"{camera.} => {Math.Round(camera.currentAzimuthalAlpha,2)}/{Math.Round(camera.currentAnglePolar,2)}";
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
                //case 'w': camera.move('f'); break;
                //case 'a': camera.move('l'); break;
                //case 's': camera.move('b'); break;
                //case 'd': camera.move('r'); break;
                //case 'q': camera.move(shiftZ: 5); break;
                //case 'e': camera.move(shiftZ: -5); break;
                default: return;
            }
            redrawScene();
            //label7.Text = $"{camera.Location}";
            e.Handled = true;
        }

        private void z_buffer_Click(object sender, EventArgs e)
        {
            colorrange = GenerateColors();
            Bitmap bmp = Z_buffer.z_buf(canvas.Width, canvas.Height,scene, colorrange);
            canvas.Image = bmp;
            canvas.Invalidate();
        }
    }
}
