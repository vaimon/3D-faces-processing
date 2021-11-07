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
        public Form1()
        {
            sceneShapes = new BindingList<Shape>();
            InitializeComponent();
            listBox.DataSource = sceneShapes;
            g = canvas.CreateGraphics();

            // Здесь мы задаём Декартову систему координат на канвасе
            g.ScaleTransform(1.0F, -1.0F);
            g.TranslateTransform(0.0F, -(float)canvas.Height);

            // А здесь задаём точку начала координат
            Point.worldCenter = new PointF(canvas.Width / 2, canvas.Height / 2);
            Point.projection = ProjectionType.DIMETRIC;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var form = new FormEditShape();
            form.Show();
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
        }

        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            redrawScene();
        }
    }
}
