
namespace _3DFacesProcessing
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnEditor = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnEditor
            // 
            this.btnEditor.Location = new System.Drawing.Point(65, 42);
            this.btnEditor.Name = "btnEditor";
            this.btnEditor.Size = new System.Drawing.Size(209, 55);
            this.btnEditor.TabIndex = 0;
            this.btnEditor.Text = "Редактор фигур";
            this.btnEditor.UseVisualStyleBackColor = true;
            this.btnEditor.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1378, 844);
            this.Controls.Add(this.btnEditor);
            this.Name = "Form1";
            this.Text = "Просмотр сцены";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnEditor;
    }
}

