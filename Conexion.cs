using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1
{
    internal class Conexion
    {
        public static string LocalServer = "localhost";
        public static string LocalPort = "1433";
        public static string LocalPassword = "root";
        public static string LocalDbName = "arduino";
        public static string LocalUser = "arduino";
  
        public static string CadenaConexionLocal = "Persist Security Info=False;User ID=" + LocalUser + ";Password=" +
                                    LocalPassword + ";Initial Catalog=" + LocalDbName + ";Server=" + LocalServer;

        static SqlConnection ConecSql = new SqlConnection();
        static SqlCommand ComandSql = new SqlCommand();
        public static void AbreConeccionLocal()
        {
            try
            {
                
                switch (ConecSql.State)
                {
                    case ConnectionState.Closed:
                        ConecSql.ConnectionString = CadenaConexionLocal;
                        ConecSql.Open();
                        break;
                }
            }
            catch (Exception e)
            {
                ConecSql.Close();
                throw;
            }
        }

        public static void CierraConeccionLocal()
        {
            switch (ConecSql.State)
            {
                case ConnectionState.Open:
                    ConecSql.Close();
                    break;
            }                   
        }

        public static Boolean ExecuteDmlLocal(string SqlDml, string NombreFuncion)
        {
            Boolean VarExecuteDml = false;
            AbreConeccionLocal();
            try
            {
                ComandSql.CommandText = SqlDml;
                ComandSql.Connection = ConecSql;
                ComandSql.CommandTimeout = 0;

                if (ComandSql.ExecuteNonQuery() > 0)
                    VarExecuteDml = true;

                else
                    VarExecuteDml = false;
            }
            catch (Exception ex)
            {
                VarExecuteDml = false;
                //MsgBox("DESCRIPCION DE ERROR -->: " & ex.ToString & Chr(13) & Chr(13) & "FUNCION CON PROBLEMAS -->: " & NombreFuncion & Chr(13) & Chr(13) & " DML -->: " & SqlDml)
                //LogLocal.WriteLine("DESCRIPCION DE ERROR -->: " + ex.ToString() + "\n\n" + "FUNCION CON PROBLEMAS -->: " + NombreFuncion + "\n\n" + " DML -->: " + SqlDml);
            }
            //CierraConeccionLocal();

            return VarExecuteDml;
        }



        public static DataTable EXECUTE_CONSULTA_LOCAL(string commandText, string nameProcesoInvoque)
        {
            //AbreConeccionLocal();
            DataTable DtConsulta = new DataTable();
            string er = "";

            try
            {
                SqlCommand SqlCommand = new SqlCommand();

                SqlCommand.Connection = ConecSql;
                SqlCommand.CommandTimeout = 0;
                SqlCommand.CommandText = commandText;

                SqlCommand.ExecuteNonQuery();
                SqlDataAdapter Myread = new SqlDataAdapter();
                Myread.SelectCommand = SqlCommand;
                Myread.Fill(DtConsulta);
            }
            catch (SqlException ex)
            {
                er = ex.Message;
            }
            if (er != "")
                throw new Exception(er + (char)10 + " Procedimiento Origen : " + nameProcesoInvoque);
            
            //CierraConeccionLocal();
            return DtConsulta;
        }


    }
}
