using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
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
        bool BatchBActivo = false;

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

                    //Limpieza de controles
                    foreach (char c in tBoxReelA.Text)
                    {
                        if (!char.IsControl(c))
                        {
                            scanInfo = scanInfo + c;
                        }
                    }

                    try
                    {
                        if (scanInfo != "")
                        {
                            //Busqueda de la información del ID
                            var fetchInv = client.fetchInventoryItems(scanInfo, "", "", "", "", "", 0, "", "", out error, out msg);

                            //Si no falla y existe
                            if (error == 0 & fetchInv.Length > 0)
                            {
                                string partStatus = fetchInv[0].status;
                                string partNum = fetchInv[0].partnum;
                                string partRev = fetchInv[0].partrev;
                                string serial = fetchInv[0].serial;
                                float quantity = fetchInv[0].qty;

                                //Si la cantidad es mayor a 0 y esta disponible/Si la cantidad es mayor a 0 y esta recibido
                                if (quantity > 0 & partStatus == "COMPLETE" || partStatus == "AVAILABLE")
                                {
                                    //Por cada unidad del BOM
                                    foreach (unitBOM unit in getBOM)
                                    {
                                        //Sí el número de parte escaneado fue encontrado en el BOM
                                        if (unit.partnum == partNum & unit.partrev == partRev)
                                        {
                                            //Activa la Boleana
                                            partBOM = true;
                                            break;
                                        }
                                    }

                                    foreach (var item in getBOM)
                                    {
                                        if (item.partnum == fetchInv[0].partnum && item.partrev == fetchInv[0].partrev)
                                        {
                                            valExist = true;
                                            break;
                                        }
                                    }

                                    if (partBOM)
                                    {
                                        //se recorre cada fila
                                        for (int x = 0; x < bomCount; x++)
                                        {
                                            string PART = dataGridView1.Rows[x].Cells[0].Value.ToString();
                                            string REVISION = dataGridView1.Rows[x].Cells[1].Value.ToString();
                                            //Si el valor de la posicion contiene el número de parte y revisión
                                            if(PART == fetchInv[0].partnum & REVISION == fetchInv[0].partrev)
                                            {
                                                //Si la posicion de la tabla no ha sido asignado
                                                if (dataGridView1.Rows[x].Cells[2].Value.ToString() == "-" & dataGridView1.Rows[x].Cells[3].Value.ToString() =="-")
                                                {
                                                    //Añade los datos 
                                                    dataGridView1.Rows[x].Cells[2].Value = serial;
                                                    dataGridView1.Rows[x].Cells[3].Value = quantity.ToString();
                                                    break;
                                                }
                                                else
                                                {
                                                    Message message1 = new Message(scanInfo + " ya escaneado.");
                                                    message1.ShowDialog();
                                                    break;
                                                }
                                            }else if (dataGridView1.Rows[x].Cells[0].Value.ToString().Contains(partNum) & !dataGridView1.Rows[x].Cells[1].Value.ToString().Contains(partRev))
                                            {
                                                Message message2 = new Message("El número de parte escaneado no pertenece al BOM " + scanInfo + "," + partNum + "-" + partRev + ", notificar.");
                                                message2.ShowDialog();
                                                errExpendor = 1;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Message message = new Message("El número de parte escaneado no pertenece al BOM " + scanInfo + "," + partNum + "-" + partRev + ", notificar.");
                                        message.ShowDialog();
                                        errExpendor = 1;
                                    }
                                }
                                else
                                {
                                    Message message = new Message(scanInfo + ", cantidad " + quantity + ", " + fetchInv[0].status + ".");
                                    message.ShowDialog();
                                    errExpendor = 1;


                                }
                            }
                            else
                            {
                                //Log
                                File.AppendAllText(Directory.GetCurrentDirectory() + @"\errorLog.txt", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + scanInfo + ", Serial no registrado en sistema.\n");

                                Message message = new Message(scanInfo + ", Serial no registrado en sistema.");
                                message.ShowDialog();
                                errExpendor = 1;
                            }
                        }
                        tLayoutMessageA.BackColor = Color.White;
                        lblMessage.Text = "";

                        //Check Data
                        //checkBOMData();
                        if (errExpendor == 0)
                        {
                            //Control Adjust
                            tBoxReelA.Clear();
                            tBoxReelA.Enabled = false;
                            tBoxLabelA.Enabled = true;
                            btnLimpiarA.Enabled = true;
                            tBoxLabelA.Clear();
                            tBoxLabelA.Focus();
                        }
                        else
                        {
                            tBoxReelA.Clear();
                            tBoxReelA.Focus();
                        }
                        
                        
                    }
                    catch (Exception ex)
                    {
                        //Log
                        File.AppendAllText(Directory.GetCurrentDirectory() + @"\errorLog.txt", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + scanInfo + ", El escaneo no pertenece al BOM, no es UNIQUE ID.\n");

                        Message message = new Message(scanInfo + ", El escaneo no pertenece al BOM, no es UNIQUE ID.");
                        message.ShowDialog();
                        errExpendor = 1;
                        
                    }
                }
                catch (Exception ex){
                    //Log
                    File.AppendAllText(Directory.GetCurrentDirectory() + @"\errorLog.txt", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + ",Error al consultar el status del serial " + ex.Message + "\n");
                }
            }
        }

        private void tBoxLabelA_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter & tBoxLabelA.Text != string.Empty) {

                BatchBActivo = false;
                //temporal Data
                int response = 0;

                //Register Unit
                serialRegister(tBoxLabelA.Text, out  response);

                if (response != 0) {
                    //Control Adjust
                    tBoxLabelA.Clear();
                    tBoxLabelA.Focus();
                    return;
                }

                //Transaction Unit
                serialTransaction(tBoxLabelA.Text, out response);

                if (response != 0) {
                    //Control Adjust
                    tBoxLabelA.Clear();
                    tBoxLabelA.Focus();
                    return;
                }

                //Control Adjust
                tBoxLabelA.Enabled = false;
                tBoxReelA.Enabled = false;
                tBoxLabelA.Clear();
                tBoxReelA.Clear();
                tBoxReelB.Enabled = true;
                tBoxReelB.Focus();
            }
        }

        private void serialRegister(string serial, out int response)
        {
            int register = -1;
            response = 0;
            int qty = 0;

            try
            {
                qty = 1;

                register = client.registerUnitToWorkOrder(cBoxWorkOrder.Text, serial, qty, "", "", "WIP", "PRODUCTION FLOOR", "ftest", out string msg);

                if (error != 0) { 
                    //Retroalimentación
                    Message message = new Message("Error al registrar el serial " + serial);
                    message.ShowDialog();

                    //Log
                    File.AppendAllText(Directory.GetCurrentDirectory() + @"\errorLog.txt", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + ",Error al registrar el serial " + serial + ":" + msg + "\n");

                    //Response
                    response = -1;
                    return;
                }

                if (msg.Contains("is already registered"))
                {
                    //Retroalimentación 
                    Message message = new Message("Serial " + serial + " Ya registrado");
                    message.ShowDialog();

                    //Log
                    File.AppendAllText(Directory.GetCurrentDirectory() + @"\errorLog.txt", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + "Serial " + serial + " YA registrado:" + msg + "\n");

                    response = -1;
                    return;
                }
            }
            catch (Exception ex) {
                //Feedback
                Message message = new Message("Error al registar el serial " + serial);
                message.ShowDialog();

                //Log
                File.AppendAllText(Directory.GetCurrentDirectory() + @"\errorLog.txt", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + ",Error al registar el serial " + serial + ":" + ex.Message + "\n");

                //Response
                response = -1;
            }
        }

        private void serialTransaction(string serial, out int response) {
            InventoryItem[] fetchInv = null;
            string workorder = string.Empty;
            string operation = string.Empty;
            string partnum = string.Empty;
            string partrev = string.Empty; 
            string status = string.Empty;
            int step = 0;

            //Response
            response = 0;

            try
            {
                fetchInv = client.fetchInventoryItems(serial, "", "", "", "", "", 0, "", "", out error, out msg);
                workorder = fetchInv[0].workorder;
                operation = fetchInv[0].opcode;
                partnum = fetchInv[0].partnum;
                partrev = fetchInv[0].partrev;
                status = fetchInv[0].status;
                step = fetchInv[0].seqnum;
            }
            catch (Exception ex) {
                //Feedback
                Message message = new Message("Error al consultar el status del serial " + serial);
                message.ShowDialog();

                //Log
                File.AppendAllText(Directory.GetCurrentDirectory() + @"\errorLog.txt", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + ",Error al consultar el status del serial " + serial + ":" + ex.Message + "\n");

                //Response
                response = -1;
                return;
            }

            if (status == "IN QUEUE" & operation == opcode | status == "IN PROGRESS" & operation == opcode)
            {
                //Transaction Item
                transactionItem transitem = new transactionItem();
                transitem.workorder = cBoxWorkOrder.Text;
                transitem.warehouseloc = warehouseLoc;
                transitem.warehousebin = warehouseBin;
                transitem.username = "ftest";
                transitem.machine_id = machineId;
                transitem.transaction = "MOVE";
                transitem.opcode = operation;
                transitem.serial = serial;
                transitem.trans_qty = 1;
                transitem.seqnum = step;
                transitem.comment = "TRANSACCION HECHA POR SISTEMA";

                //Data/BOM Item
                bomItem[] bomData = new bomItem[getBOM.Length];
                dataItem[] inputData = new dataItem[] { };

                //Counter
                int bom = 0;

                //Seriales para retirar
                List<string> partToRemove = new List<string>();
                string partnum1 = string.Empty;
                string uniqueId = string.Empty;
                int cantidad = 0;
                string rev = string.Empty;
                if (BatchBActivo == false)
                {
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        uniqueId = row.Cells[2].Value.ToString();
                        partnum1 = row.Cells[0].Value.ToString();
                        rev = row.Cells[1].Value.ToString();

                        cantidad = Convert.ToInt32(row.Cells[3].Value.ToString());
                        etiqExpandor++;

                        //Load BOM
                        bomData[bom] = new bomItem();
                        bomData[bom].item_serial = uniqueId;
                        bomData[bom].item_partnum = partnum1;
                        bomData[bom].item_partrev = rev;

                        foreach (unitBOM part in getBOM)
                            if (partnum1 == part.partnum)
                            {
                                bomData[bom].item_qty = 1;
                                //Por cada pieza del BOM
                                for (int x = 0; x < dataGridView1.Rows.Count; x++)
                                {
                                    //Si el número de parte coincide con el encontrado
                                    if (partnum1.Contains(part.partnum))
                                    {
                                        if (uniqueId.ToString() != "-")
                                        {
                                            cantidad = (cantidad - Convert.ToInt32(part.qty));
                                            //Si el id expiro
                                            if (cantidad <= 0)
                                            {
                                                //Reinicia los valores del campo
                                                dataGridView1.Rows[x].Cells[2].Value = "-";
                                                dataGridView1.Rows[x].Cells[3].Value = "-";

                                                //
                                                partToRemove.Add(part.partnum);

                                                tBoxLabelA.Enabled = true;
                                            }
                                            dataGridView1.Rows[x].Cells[3].Value = Convert.ToString(cantidad);
                                        }
                                        break;
                                    }
                                }
                                break;
                            }
                        //Count
                        bom++;
                        if (dataGridView1.Rows.Count == 0)
                            break;
                    }

                }
                else
                {
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        uniqueId = row.Cells[2].Value.ToString();
                        partnum1 = row.Cells[0].Value.ToString();
                        rev = row.Cells[1].Value.ToString();

                        cantidad = Convert.ToInt32(row.Cells[3].Value.ToString());
                        etiqExpandor++;

                        //Load BOM
                        bomData[bom] = new bomItem();
                        bomData[bom].item_serial = uniqueId;
                        bomData[bom].item_partnum = partnum1;
                        bomData[bom].item_partrev = rev;

                        foreach (unitBOM part in getBOM)
                            if (partnum1 == part.partnum)
                            {
                                bomData[bom].item_qty = 1;
                                //Por cada pieza del BOM
                                for (int x = 0; x < dataGridView2.Rows.Count; x++)
                                {
                                    //Si el número de parte coincide con el encontrado
                                    if (partnum1.Contains(part.partnum))
                                    {
                                        if (uniqueId.ToString() != "-")
                                        {
                                            cantidad = (cantidad - Convert.ToInt32(part.qty));
                                            //Si el id expiro
                                            if (cantidad <= 0)
                                            {
                                                //Reinicia los valores del campo
                                                dataGridView2.Rows[x].Cells[2].Value = "-";
                                                dataGridView2.Rows[x].Cells[3].Value = "-";

                                                //
                                                partToRemove.Add(part.partnum);

                                                tBoxLabelB.Enabled = true;
                                            }
                                            dataGridView2.Rows[x].Cells[3].Value = Convert.ToString(cantidad);
                                        }
                                        break;
                                    }
                                }
                                break;
                            }
                        //Count
                        bom++;
                        if (dataGridView2.Rows.Count == 0)
                            break;
                    }
                }
                try
                {
                    //Transaction
                    var transaction = client.transactUnit(transitem, inputData, bomData, out msg);

                    //MessageBox.Show(msg);
                    if (!msg.Contains("ADVANCE"))
                    {
                        if (BatchBActivo == false)
                        {
                            //Feedback
                            lblMessage.Text = "Pase NO otorgado al serial " + serial;
                            tLayoutMessageA.BackColor = Color.Crimson;
                            MostrarMensajeFlotanteNoPass(" NO PASS");
                        }
                        else
                        {
                            //Feedback
                            lblMessage2.Text = "Pase NO otorgado al serial " + serial;
                            panelBatchB.BackColor = Color.Crimson;
                            MostrarMensajeFlotanteNoPass(" NO PASS");
                        }

                        //Log
                        File.AppendAllText(Directory.GetCurrentDirectory() + @"\errorLog.txt", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + ",Pase NO otorgado al serial " + serial + ":" + msg + "\n");

                        //Response
                        response = -1;
                        return;
                    }

                    if (BatchBActivo == false)
                    {
                        //Feedback
                        lblMessage.Text = "Serial " + serial + " Completado";
                        tLayoutMessageA.BackColor = Color.FromArgb(58, 196, 123);
                        MostrarMensajeFlotante("P A S S");
                    }
                    else
                    {
                        //Feedback
                        lblMessage2.Text = "Serial " + serial + " Completado";
                        panelBatchB.BackColor = Color.FromArgb(58, 196, 123);
                        MostrarMensajeFlotante("P A S S");
                    }

                    //Log
                    File.AppendAllText(Directory.GetCurrentDirectory() + @"\Log.txt", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + "," + msg + "\n");
                }
                catch (Exception ex)
                {
                    //Feedback
                    Message message = new Message("Error al dar el pase al serial " + serial);
                    message.ShowDialog();

                    //Log
                    File.AppendAllText(Directory.GetCurrentDirectory() + @"\errorLog.txt", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + ",Error al dar el pase al serial " + serial + ":" + ex.Message + "\n");
                    //Response
                    response = -1;
                    return;
                }
            }
            else
            {
                //Get Instructions
                var getInstructions = client.getWorkOrderStepInstructions(cBoxWorkOrder.Text, step.ToString(), out error, out msg);

                //Feedback
                lblMessage.Text = "Serial " + serial + " sin flujo, " + status + ":" + getInstructions.opdesc;
                tLayoutMessageA.BackColor = Color.Crimson;

                //Response
                response = -1;
            }
        }

        // Método para mostrar el mensaje flotante gigante
        private void MostrarMensajeFlotante(string mensaje)
        {
            // Crear un formulario emergente flotante
            Form flotanteForm = new Form();
            flotanteForm.FormBorderStyle = FormBorderStyle.None;  // Sin bordes
            flotanteForm.StartPosition = FormStartPosition.CenterScreen;  // Centrado en la pantalla
            flotanteForm.BackColor = Color.Green;  // Fondo verde (puedes cambiar el color)
            flotanteForm.Opacity = 0.9;  // Opacidad para hacerlo semitransparente
            flotanteForm.TopMost = true;  // Asegura que esté sobre otras ventanas
            flotanteForm.Width = 600;  // Ancho de la ventana flotante
            flotanteForm.Height = 200;  // Alto de la ventana flotante

            // Crear un label para mostrar el mensaje
            Label mensajeLabel = new Label();
            mensajeLabel.AutoSize = false;
            mensajeLabel.Size = new Size(flotanteForm.Width, flotanteForm.Height);
            mensajeLabel.Text = mensaje;
            mensajeLabel.Font = new Font("Arial", 48, FontStyle.Bold);  // Tamaño grande de la fuente
            mensajeLabel.ForeColor = Color.White;  // Color de texto blanco
            mensajeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;  // Centrado en el label

            // Añadir el label al formulario flotante
            flotanteForm.Controls.Add(mensajeLabel);

            // Mostrar el mensaje durante 3 segundos y luego cerrar
            flotanteForm.Show();
            Timer timer = new Timer();
            timer.Interval = 3000;  // 3000 milisegundos = 3 segundos
            timer.Tick += (sender, e) =>
            {
                flotanteForm.Close();
                timer.Stop();
            };
            timer.Start();
        }

        private void MostrarMensajeFlotanteNoPass(string mensaje)
        {
            // Crear un formulario emergente flotante
            Form flotanteForm = new Form();
            flotanteForm.FormBorderStyle = FormBorderStyle.None;  // Sin bordes
            flotanteForm.StartPosition = FormStartPosition.CenterScreen;  // Centrado en la pantalla
            flotanteForm.BackColor = Color.Red;  // Fondo verde (puedes cambiar el color)
            flotanteForm.Opacity = 0.9;  // Opacidad para hacerlo semitransparente
            flotanteForm.TopMost = true;  // Asegura que esté sobre otras ventanas
            flotanteForm.Width = 600;  // Ancho de la ventana flotante
            flotanteForm.Height = 200;  // Alto de la ventana flotante

            // Crear un label para mostrar el mensaje
            Label mensajeLabel = new Label();
            mensajeLabel.AutoSize = false;
            mensajeLabel.Size = new Size(flotanteForm.Width, flotanteForm.Height);
            mensajeLabel.Text = mensaje;
            mensajeLabel.Font = new Font("Arial", 48, FontStyle.Bold);  // Tamaño grande de la fuente
            mensajeLabel.ForeColor = Color.White;  // Color de texto blanco
            mensajeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;  // Centrado en el label

            // Añadir el label al formulario flotante
            flotanteForm.Controls.Add(mensajeLabel);

            // Mostrar el mensaje durante 3 segundos y luego cerrar
            flotanteForm.Show();
            Timer timer = new Timer();
            timer.Interval = 3000;  // 3000 milisegundos = 3 segundos
            timer.Tick += (sender, e) =>
            {
                flotanteForm.Close();
                timer.Stop();
            };
            timer.Start();
        }

        private void tBoxReelB_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter & tBoxReelB.Text != string.Empty)
            {
                try
                {
                    //Boleana
                    bool partBOM = false;

                    //Almacena el valor escaneado
                    string scanInfo = "";

                    tBoxLabelB.Enabled = false;
                    valExist = false;

                    errExpendor = 0;

                    //Limpieza de controles
                    foreach (char c in tBoxReelB.Text)
                    {
                        if (!char.IsControl(c))
                        {
                            scanInfo = scanInfo + c;
                        }
                    }

                    try
                    {
                        if (scanInfo != "")
                        {
                            //Busqueda de la información del ID
                            var fetchInv = client.fetchInventoryItems(scanInfo, "", "", "", "", "", 0, "", "", out error, out msg);

                            //Si no falla y existe
                            if (error == 0 & fetchInv.Length > 0)
                            {
                                string partStatus = fetchInv[0].status;
                                string partNum = fetchInv[0].partnum;
                                string partRev = fetchInv[0].partrev;
                                string serial = fetchInv[0].serial;
                                float quantity = fetchInv[0].qty;

                                //Si la cantidad es mayor a 0 y esta disponible/Si la cantidad es mayor a 0 y esta recibido
                                if (quantity > 0 & partStatus == "COMPLETE" || partStatus == "AVAILABLE")
                                {
                                    //Por cada unidad del BOM
                                    foreach (unitBOM unit in getBOM)
                                    {
                                        //Sí el número de parte escaneado fue encontrado en el BOM
                                        if (unit.partnum == partNum & unit.partrev == partRev)
                                        {
                                            //Activa la Boleana
                                            partBOM = true;
                                            break;
                                        }
                                    }

                                    foreach (var item in getBOM)
                                    {
                                        if (item.partnum == fetchInv[0].partnum && item.partrev == fetchInv[0].partrev)
                                        {
                                            valExist = true;
                                            break;
                                        }
                                    }

                                    if (partBOM)
                                    {
                                        //se recorre cada fila
                                        for (int x = 0; x < bomCount; x++)
                                        {
                                            string PART = dataGridView2.Rows[x].Cells[0].Value.ToString();
                                            string REVISION = dataGridView2.Rows[x].Cells[1].Value.ToString();
                                            //Si el valor de la posicion contiene el número de parte y revisión
                                            if (PART == fetchInv[0].partnum & REVISION == fetchInv[0].partrev)
                                            {
                                                //Si la posicion de la tabla no ha sido asignado
                                                if (dataGridView2.Rows[x].Cells[2].Value.ToString() == "-" & dataGridView2.Rows[x].Cells[3].Value.ToString() == "-")
                                                {
                                                    //Añade los datos 
                                                    dataGridView2.Rows[x].Cells[2].Value = serial;
                                                    dataGridView2.Rows[x].Cells[3].Value = quantity.ToString();
                                                    break;
                                                }
                                                else
                                                {
                                                    Message message1 = new Message(scanInfo + " ya escaneado.");
                                                    message1.ShowDialog();
                                                    break;
                                                }
                                            }
                                            else if (dataGridView2.Rows[x].Cells[0].Value.ToString().Contains(partNum) & !dataGridView2.Rows[x].Cells[1].Value.ToString().Contains(partRev))
                                            {
                                                Message message2 = new Message("El número de parte escaneado no pertenece al BOM " + scanInfo + "," + partNum + "-" + partRev + ", notificar.");
                                                message2.ShowDialog();
                                                errExpendor = 1;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Message message = new Message("El número de parte escaneado no pertenece al BOM " + scanInfo + "," + partNum + "-" + partRev + ", notificar.");
                                        message.ShowDialog();
                                        errExpendor = 1;
                                    }
                                }
                                else
                                {
                                    Message message = new Message(scanInfo + ", cantidad " + quantity + ", " + fetchInv[0].status + ".");
                                    message.ShowDialog();
                                    errExpendor = 1;


                                }
                            }
                            else
                            {
                                //Log
                                File.AppendAllText(Directory.GetCurrentDirectory() + @"\errorLog.txt", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + scanInfo + ", Serial no registrado en sistema.\n");

                                Message message = new Message(scanInfo + ", Serial no registrado en sistema.");
                                message.ShowDialog();
                                errExpendor = 1;
                            }
                        }
                        panelBatchB.BackColor = Color.White;
                        lblMessage2.Text = "";

                        //Check Data
                        //checkBOMData();

                        if (errExpendor == 0)
                        {
                            //Control Adjust
                            tBoxReelB.Clear();
                            tBoxReelB.Enabled = false;
                            tBoxLabelB.Enabled = true;
                            btnLimpiarB.Enabled = true;
                            tBoxLabelB.Clear();
                            tBoxLabelB.Focus();
                        }
                        else
                        {
                            tBoxReelB.Clear();
                            tBoxReelB.Focus();
                        }
                        

                    }
                    catch (Exception ex)
                    {
                        //Log
                        File.AppendAllText(Directory.GetCurrentDirectory() + @"\errorLog.txt", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + scanInfo + ", El escaneo no pertenece al BOM, no es UNIQUE ID.\n");

                        Message message = new Message(scanInfo + ", El escaneo no pertenece al BOM, no es UNIQUE ID.");
                        message.ShowDialog();
                        errExpendor = 1;

                    }
                }
                catch (Exception ex)
                {
                    //Log
                    File.AppendAllText(Directory.GetCurrentDirectory() + @"\errorLog.txt", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + ",Error al consultar el status del serial " + ex.Message + "\n");
                }
            }
            
        }

        private void tBoxLabelB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter & tBoxLabelB.Text != string.Empty)
            {
                BatchBActivo = true;
                //temporal Data
                int response = 0;

                //Register Unit
                serialRegister(tBoxLabelB.Text, out response);

                if (response != 0)
                {
                    //Control Adjust
                    tBoxLabelB.Clear();
                    tBoxLabelB.Focus();
                    return;
                }

                //Transaction Unit
                serialTransaction(tBoxLabelB.Text, out response);

                if (response != 0)
                {
                    //Control Adjust
                    tBoxLabelB.Clear();
                    tBoxLabelB.Focus();
                    return;
                }

                //Control Adjust
                tBoxLabelA.Enabled = false;
                tBoxReelA.Enabled = true;
                tBoxLabelB.Clear();
                tBoxReelB.Clear();
                tBoxReelB.Enabled = false;
                tBoxReelA.Focus();
            }
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            //Control Adjust
            tLayoutMessageA.BackColor = Color.White;
            panelBatchB.BackColor = Color.White;
            cBoxWorkOrder.SelectedIndex = -1;
            cBoxPartNum.SelectedIndex = -1;
            cBoxWorkOrder.Enabled = false;
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            cBoxPartNum.Enabled = true;
            tBoxLabelA.Enabled = false;
            tBoxLabelB.Enabled = false;
            btnChange.Enabled = false;
            tBoxReelA.Enabled = false;
            tBoxReelB.Enabled = false;
            lblMessage.Text = "";
            lblMessage2.Text = "";
            bomCount = 0;
            contador = 0;
            BatchBActivo = false;
        }

        private void btnLimpiarA_Click(object sender, EventArgs e)
        {
            //Control Adjust
            tLayoutMessageA.BackColor = Color.White;
            tBoxLabelA.Enabled = false;
            btnChange.Enabled = true;
            tBoxReelA.Enabled = true;
            lblMessage.Text = "";


            for (int x = 0; x < getBOM.Length; x++)
            {
                dataGridView1.Rows[x].Cells["UniqueId"].Value = "-";
                dataGridView1.Rows[x].Cells["Cantidad"].Value = "-";

                contador = 0;
            }
            //Check BOM Data
            checkBOMData();
            tBoxReelA.Clear();
            tBoxReelA.Focus();
        }

        private void btnLimpiarB_Click(object sender, EventArgs e)
        {
            //Control Adjust
            panelBatchB.BackColor = Color.White;
            tBoxLabelB.Enabled = false;
            btnChange.Enabled = true;
            tBoxReelB.Enabled = true;
            lblMessage2.Text = "";


            for (int x = 0; x < getBOM.Length; x++)
            {
                dataGridView2.Rows[x].Cells["UniqueId"].Value = "-";
                dataGridView2.Rows[x].Cells["Cantidad"].Value = "-";

                contador = 0;
            }
            //Check BOM Data
            checkBOMData();
            tBoxReelB.Clear();
            tBoxReelB.Focus();
        }

        private void timerTextReset_Tick(object sender, EventArgs e)
        {
            //Timer Stop
            timerTextReset.Stop();

            if (BatchBActivo == false)
            {
                //Control Adjust
                tLayoutMessageA.BackColor = Color.White;
                lblMessage.Text = string.Empty;
            }
            else {
                //Control Adjust
                panelBatchB.BackColor = Color.White;
                lblMessage2.Text = string.Empty;
            }
        }

        private void lblMessage_TextChanged(object sender, EventArgs e)
        {
            //Timer Stop
            timerTextReset.Stop();

            //Control Adjust
            tLayoutMessageA.BackColor = Color.White;
            lblMessage.Text = string.Empty;
        }

        private void lblMessage2_TextChanged(object sender, EventArgs e)
        {
            //Timer Stop
            timerTextReset.Stop();

            //Control Adjust
            panelBatchB.BackColor = Color.White;
            lblMessage2.Text = string.Empty;
        }
    }
}
