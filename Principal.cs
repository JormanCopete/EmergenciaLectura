using System.Data.SqlTypes;
using System.IO.Ports;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace WinFormsApp1
{
    public partial class Principal : Form
    {
        private SerialPort serialPort;
        public Principal()
        {
            InitializeComponent();
            serialPort = new SerialPort();
            serialPort.PortName = "COM3";
            serialPort.BaudRate = 16000;
            serialPort.DtrEnable = true;
            serialPort.Open();
            serialPort.DataReceived+= serialPort_DataReceived;
        }

        private void serialPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            string line = serialPort.ReadLine();
            this.BeginInvoke(new LineReceivedEvent(LineReceived), line);
        }

        private delegate void LineReceivedEvent(string line);
        private void LineReceived(string line)
        {
            string SqlDml = "";
            string idFechaHora = string.Format("{0:yyyyMMddHHss}", DateTime.Now);

            if (line.StartsWith("ERR:")==true)
            {
                SqlDml = "insert into test(data) " + "values('" + line + "')";
            }
            else if(line.StartsWith("PRED:") == true)
            {
                line = line.Substring(5);
                string InsertInto = "insert into emergencia_resumen(idFechaHora,";
                string ValuesInto = "values( "+ idFechaHora + ",";
                var ArrayInferencia = line.Split(';').ToList();
                foreach (var item in ArrayInferencia)
                {
                    var Valores = item.Split(":");
                    if(Valores.Length == 2)
                    {
                        InsertInto += Valores[0] + ",";
                        ValuesInto += "'" + Valores[1] + "',";
                        SqlDml = $"insert into emergencia_detalle(idFechaHora,organinismo,valor) " + "values(" + idFechaHora + ",'" + Valores[0] + "'," + Valores[1] + ")";
                        InsertaDatos(SqlDml, "LineReceived");
                    }                    
                }
                SqlDml = InsertInto.Substring(0,InsertInto.Length - 1) + ") " 
                            + ValuesInto.Substring(0,ValuesInto.Length -1) + ")";
            }
            
            InsertaDatos(SqlDml, "LineReceived");

            richTextBox1.Text += line;
        }

        private bool InsertaDatos(string sqlDml, string nameFunction)
        {
           return Conexion.ExecuteDmlLocal(sqlDml, nameFunction);
        }
    }
}