﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HPCExpando2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void groupBox1_Paint(object sender, PaintEventArgs e)
        {
            //GroupBox groupBox = sender as GroupBox;
            if (gbBatchA != null)
            {
                // Dibujar un borde con el color deseado
                Pen pen = new Pen(Color.Black, 3);  // Color rojo, grosor 3
                e.Graphics.DrawRectangle(pen, 0, 0, gbBatchA.Width - 1, gbBatchA.Height - 1);
            }
        }

        private void gbBatchB_Paint(object sender, PaintEventArgs e)
        {
            //GroupBox groupBox = sender as GroupBox;
            if (gbBatchB != null)
            {
                // Dibujar un borde con el color deseado
                Pen pen = new Pen(Color.Black, 3);  // Color rojo, grosor 3
                e.Graphics.DrawRectangle(pen, 0, 0, gbBatchB.Width - 1, gbBatchB.Height - 1);
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            panel1.Width = this.ClientSize.Width;
        }

        private void tableLayoutPanel5_Resize(object sender, EventArgs e)
        {
            // Centrar el TableLayoutPanel horizontalmente dentro del formulario
            tableLayoutPanel5.Left = (this.ClientSize.Width - tableLayoutPanel5.Width) / 2;

        }
    }
}
