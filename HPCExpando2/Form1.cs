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
        INIFile localConfig = new INIFile(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\HPCExpando2\config.ini");

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

                dataGridView1.DefaultCellStyle.Font = new Font("Ebrima", 12F, FontStyle.Bold);
                dataGridView1.DefaultCellStyle.ForeColor = Color.Black;
                dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Century Gothic", 16F, FontStyle.Bold);
                dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

                dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

                //formato Datagrid2

                dataGridView2.DefaultCellStyle.Font = new Font("Ebrima", 12F, FontStyle.Bold);
                dataGridView2.DefaultCellStyle.ForeColor = Color.Black;
                dataGridView2.ColumnHeadersDefaultCellStyle.Font = new Font("Century Gothic", 16F, FontStyle.Bold);
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
                dataGridView2.Dock = DockStyle.Fill;


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


                DataGridViewTextBoxColumn tbMaterialB = new DataGridViewTextBoxColumn();
                tbMaterialB.HeaderText = "Material";
                tbMaterialB.Name = "Material";
                tbMaterialB.FillWeight = 100;
                tbMaterialB.Width = 287;

                dataGridView2.Columns.Add(tbMaterialB);

                DataGridViewTextBoxColumn tbRevB = new DataGridViewTextBoxColumn();
                tbRevB.HeaderText = "Rev";
                tbRevB.Name = "Rev";
                tbRevB.FillWeight = 50;
                tbRevB.Width = 144;

                dataGridView2.Columns.Add(tbRevB);

                DataGridViewTextBoxColumn tbUniqueIdB = new DataGridViewTextBoxColumn();
                tbUniqueIdB.HeaderText = "Batch B";
                tbUniqueIdB.Name = "Batch B";
                tbUniqueIdB.FillWeight = 100;
                tbUniqueIdB.Width = 288;

                dataGridView2.Columns.Add(tbUniqueIdB);

                DataGridViewTextBoxColumn tbCantidadB = new DataGridViewTextBoxColumn();
                tbCantidadB.HeaderText = "Cantidad";
                tbCantidadB.Name = "Cantidad";
                tbCantidadB.FillWeight = 55;
                tbCantidadB.Width = 158;

                dataGridView2.Columns.Add(tbCantidadB);

                //Temporal Data
                string dBMsg = string.Empty;
                int dBError = 0;

                //Data Base Connection 
                DBConnection dB = new DBConnection();
                DataTable dtResult = new DataTable();

                dB.dataBase = "datasource=mlxgumvlptfrd01.molex.com;port=3306;username=ftest;password=Ftest123#;database=runcard_tempflex;";
                dB.query = "SELECT partnum FROM runcard_tempflex.prod_master_config"
                         + " INNER JOIN runcard_tempflex.prod_step_config ON runcard_tempflex.prod_step_config.prr_config_id = runcard_tempflex.prod_master_config.prr_config_id AND runcard_tempflex.prod_step_config.prr_config_rev = runcard_tempflex.prod_master_config.prr_config_rev"
                         + " WHERE status = \"ACTIVE\" AND opcode = \"" + opcode + "\" AND part_class IN ('" + partClass + "');";
                var dBResult = dB.getData(out dBMsg, out dBError);

                if (dBError != 0)
                {
                    //Control Adjust
                    cBoxPartNum.Enabled = false;

                    //Feedback
                    Message message = new Message(dBMsg);
                    message.ShowDialog();
                    return;
                }

                //Fill Data Table
                dBResult.Fill(dtResult);
                foreach (DataRow row in dtResult.Rows)
                {
                    if (!cBoxPartNum.Items.Contains(row.ItemArray[0]))
                        cBoxPartNum.Items.Add(row.ItemArray[0]);
                }
            }
            catch (Exception ex)
            {
                //Control Adjust
                cBoxPartNum.Enabled = false;

                //Feedback
                Message message = new Message("Error al obtener la configuración");
                message.ShowDialog();

                //Log
                File.AppendAllText(Directory.GetCurrentDirectory() + @"\errorLog.txt", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + ",Error al obtener la configuración:" + ex.Message + "\n");
            }
        }

        private void cBoxPartNum_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cBoxPartNum.Text != string.Empty)
            {
                try
                {
                    //Clear Save Data
                    cBoxWorkOrder.Items.Clear();

                    //Get Work Orders
                    var getWorkOrders = client.getAvailableWorkOrders(cBoxPartNum.Text, "", out error, out msg);

                    foreach (workOrderItem order in getWorkOrders)
                        if (!cBoxWorkOrder.Items.Contains(order.workorder))
                            cBoxWorkOrder.Items.Add(order.workorder);

                    //Control Adjust
                    cBoxWorkOrder.Enabled = true;
                }
                catch (Exception ex)
                {
                    //Feedback
                    Message message = new Message("Error al obtener las ordenes");
                    message.ShowDialog();

                    //Log
                    File.AppendAllText(Directory.GetCurrentDirectory() + @"\errorLog.txt", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + ",Error al obtener las ordenes:" + ex.Message + "\n");
                }
            }
        }

        private void cBoxWorkOrder_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cBoxWorkOrder.Text != string.Empty)
            {
                //Control Adjust
                dataGridView1.Controls.Clear();

                try
                {
                    //Get BOM
                    getBOM = client.getUnitBOMConsumption(cBoxWorkOrder.Text, seqnum, out error, out msg);

                    if (getBOM.Length == 0)
                    {
                        Message message = new Message("La orden actual no cuenta con BOM");
                        message.ShowDialog();

                        //Log
                        File.AppendAllText(Directory.GetCurrentDirectory() + @"\errorLog.txt", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + ",La orden actual no cuenta con BOM\n");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    //Retroalimentación
                    Message message = new Message("Error al obtener el BOM");
                    message.ShowDialog();

                    //Log
                    File.AppendAllText(Directory.GetCurrentDirectory() + @"\errorLog.txt", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + ",Error al obtener el BOM:" + ex.Message + "\n");
                    return;
                }

                //Internal Counter
                int bom = 0;
                int row = 0;
                int col = 0;



                foreach (unitBOM item in getBOM)
                {
                    if (item.alt_for_item == 0)
                    {
                        dataGridView1.Rows.Add(item.partnum, item.partrev, "-", "-");
                        dataGridView2.Rows.Add(item.partnum, item.partrev, "-", "-");

                        bomCount++;
                        bom++;
                        foreach (unitBOM subItem in getBOM)
                        {
                            if (subItem.alt_for_item == item.item)
                            {
                                //In case of altern add it
                                dataGridView1.Rows[0].Cells[bomCount - 1].Value = dataGridView1.Rows[0].Cells[bomCount - 1].Value + "\n" + subItem.partnum;
                                dataGridView1.Rows[1].Cells[bomCount - 1].Value = dataGridView1.Rows[1].Cells[bomCount - 1].Value + "\n" + subItem.partrev;

                                dataGridView2.Rows[0].Cells[bomCount - 1].Value = dataGridView2.Rows[0].Cells[bomCount - 1].Value + "\n" + subItem.partnum;
                                dataGridView2.Rows[1].Cells[bomCount - 1].Value = dataGridView2.Rows[1].Cells[bomCount - 1].Value + "\n" + subItem.partrev;
                                break;
                            }
                        }
                    }
                    //habilitar barras de desplazamiento si el contenido excede el tamaño del datagridview

                    cBoxWorkOrder.Enabled = false;
                    cBoxPartNum.Enabled = false;
                    dataGridView1.ResumeLayout();
                    btnChange.Enabled = true;

                    dataGridView1.ScrollBars = ScrollBars.Both;
                    dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                }

                //Check Data
                checkBOMData();
            }
        }

        private void checkBOMData()
        {
            //temporal Data
            string missing = "";
            string desc = "";
            for (int x=0; x < getBOM.Length; x++)
            {
                var CellValue = dataGridView1.Rows[x].Cells[2].Value;
                if (CellValue.ToString() == "-") 
                    missing = missing + "-" + dataGridView1.Rows[x].Cells[0].Value.ToString();

                var CellValue1 = dataGridView2.Rows[x].Cells[2].Value;
                if (CellValue1.ToString() == "-")
                    missing = missing + "-" + dataGridView2.Rows[x].Cells[0].Value.ToString();
            }

            if (missing.Length == 0)
            {
                //Control Adjust
                cBoxWorkOrder.Enabled = false;
                cBoxPartNum.Enabled = false;
                tBoxLabelA.Enabled = true;
                tBoxLabelB.Enabled = true;
                tBoxReelA.Enabled = true;
                tBoxReelB.Enabled = false;
                btnChange.Enabled = true;
                tBoxLabelA.Focus();

                return;
            }
            foreach (char c in missing)
                if (!char.IsControl(c))
                    desc = desc + c;
                else if (c.Equals('\n'))
                    desc = desc + "/";


            //Control Adjust
            tBoxLabelA.Enabled = false;
            tBoxLabelB.Enabled = false;
            tBoxReelB.Enabled = false;
            tBoxReelA.Enabled = true;
            tBoxReelA.Focus();
        }

        private void tBoxReelA_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter & tBoxReelA.Text != string.Empty)
            {
                try
                {
                    //Boleana
                    bool partBOM = false;

                    //Almacena el valor escaneado
                    string scanInfo = "";

                    tBoxLabelA.Enabled = false;
                    valExist = false;

                    errExpendor = 0;
                }
                catch { }
            }
        }
    }
}
