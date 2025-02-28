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
using System.Windows.Forms.VisualStyles;
using HPCExpando2.Clases;
using HPCExpando2.RuncardServices;

namespace HPCExpando2
{
    public partial class Form1 : Form
    {
        //Config Connection
        INIFile localConfig = new INIFile(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\HPC Consume\config.ini");

        //Runcard Connection
        runcard_wsdlPortTypeClient client = new runcard_wsdlPortTypeClient("runcard_wsdlPort");
        string msg = string.Empty;
        unitBOM[] getBOM = null;
        int error = 0;

        //Config Data
        string warehouseBin = string.Empty;
        string warehouseLoc = string.Empty;
        string partClass = string.Empty;
        string machineId = string.Empty;
        string opcode = string.Empty;
        string seqnum = string.Empty;
        string etiqnum = string.Empty;

        //General Data
        int bomCount = 0;
        int pos = 0;
        int contador = 0;
        bool valExist = false;
        int contDatagrid = 0;
        int etiqExpandor = 0;
        int errExpendor = 0;

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

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            // Verificar si el formulario está maximizado
            if (this.WindowState == FormWindowState.Maximized)
            {
                // Calcular la posición del centro para el panel
                int panelX = (this.ClientSize.Width - tableLayoutPanel5.Width) / 2;
                int panelY = 179;//(this.ClientSize.Height - flowLayoutPanel1.Height) / 2;

                // Establecer la posición del panel
                tableLayoutPanel5.Location = new System.Drawing.Point(panelX, panelY);

                int X = (this.ClientSize.Width - tableLayoutPanel1.Width) / 2;
                int Y = 300;

                tableLayoutPanel1.Location = new System.Drawing.Point(X, Y);
            }
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            // Ajustar el panel al iniciar
            Form1_SizeChanged(sender, e);

            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(localConfig.FilePath)))
                {
                    //Config Directory
                    Directory.CreateDirectory(Path.GetDirectoryName(localConfig.FilePath));
                    File.Copy(Directory.GetCurrentDirectory() + "\\config.ini", localConfig.FilePath);
                }

                dataGridView1.DefaultCellStyle.Font = new Font("Franklin Gothic Medium Cond", 13.8F);
                dataGridView1.DefaultCellStyle.ForeColor = Color.Black;
                dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Ebrima", 19.8000011F, FontStyle.Bold);
                dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

                dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

                //formato Datagrid2

                dataGridView2.DefaultCellStyle.Font = new Font("Franklin Gothic Medium Cond", 13.8F);
                dataGridView2.DefaultCellStyle.ForeColor = Color.Black;
                dataGridView2.ColumnHeadersDefaultCellStyle.Font = new Font("Ebrima", 19.8000011F, FontStyle.Bold);
                dataGridView2.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

                dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

                warehouseBin = localConfig.Read("RUNCARD_INFO", "warehouseBin");
                warehouseLoc = localConfig.Read("RUNCARD_INFO", "warehouseLoc");
                partClass = localConfig.Read("RUNCARD_INFO", "partClass");
                machineId = localConfig.Read("RUNCARD_INFO", "machineID");
                opcode = localConfig.Read("RUNCARD_INFO", "opcode");
                seqnum = localConfig.Read("RUNCARD_INFO", "seqnum");

                lblopcode.Text = opcode;
                lblMessage.Text = "";
                lblMessage2.Text = "";

                dataGridView1.Dock = DockStyle.Fill;

                DataGridViewTextBoxColumn tbId = new DataGridViewTextBoxColumn();
                tbId.HeaderText = "ID";
                tbId.Name = "ID";
                tbId.FillWeight = 50;
                tbId.Width = 144;

                dataGridView1.Columns.Add(tbId);

                DataGridViewTextBoxColumn tbMaterial = new DataGridViewTextBoxColumn();
                tbMaterial.HeaderText = "Material";
                tbMaterial.Name = "Material";
                tbMaterial.FillWeight = 100;
                tbMaterial.Width = 287;

                dataGridView1.Columns.Add(tbMaterial);

                DataGridViewTextBoxColumn tbRev = new DataGridViewTextBoxColumn();
                tbRev.HeaderText = "Rev";
                tbRev.Name = "Rev";
                tbRev.FillWeight = 50;
                tbRev.Width = 144;

                dataGridView1.Columns.Add(tbRev);

                DataGridViewTextBoxColumn tbUniqueId = new DataGridViewTextBoxColumn();
                tbUniqueId.HeaderText = "Batch A";
                tbUniqueId.Name = "Batch A";
                tbUniqueId.FillWeight = 100;
                tbUniqueId.Width = 288;

                dataGridView1.Columns.Add(tbUniqueId);

                DataGridViewTextBoxColumn tbCantidad = new DataGridViewTextBoxColumn();
                tbCantidad.HeaderText = "Cantidad";
                tbCantidad.Name = "Cantidad";
                tbCantidad.FillWeight = 55;
                tbCantidad.Width = 158;

                dataGridView1.Columns.Add(tbCantidad);
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
