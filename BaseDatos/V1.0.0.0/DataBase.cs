using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Comun.DataBase
{
    public class DataBase
    {
        private Database dsn;


        #region GetObject

        public T GetObject<T>(string storeProcedureName, params object[] parametros) where T : class, new()
        {
            T obj = new T();

            DataRow dr = GetDatos(storeProcedureName, parametros);

            if (dr != null)
            {
                foreach (System.Reflection.PropertyInfo prop in obj.GetType().GetProperties())
                {
                    if (dr.Table.Columns.Contains(prop.Name))
                    {
                        try
                        {
                            PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                            propertyInfo.SetValue(obj, Convert.ChangeType(dr[prop.Name], propertyInfo.PropertyType), null);
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }

            }
            return obj;
        }

        public T GetObjectWithSubObject<T>(string storeProcedureName, params object[] parametros) where T : class, new()
        {
            T obj = new T();

            foreach (System.Reflection.PropertyInfo prop in obj.GetType().GetProperties())
            {
                string name = prop.Name;
            }

            DataRow dr = GetDatos(storeProcedureName, parametros);
            if (dr != null)
            {
                foreach (System.Reflection.PropertyInfo prop in obj.GetType().GetProperties())
                {
                    if (dr.Table.Columns.Contains(prop.Name))
                    {
                        try
                        {
                            PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                            propertyInfo.SetValue(obj, Convert.ChangeType(dr[prop.Name], propertyInfo.PropertyType), null);
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }

            }
            return obj;
        }


        public List<T> GetObjectList<T>(string storeProcedureName, params object[] parametros) where T : class, new()
        {
            List<T> list = new List<T>();

            List<DataRow> dr = GetListaFilas(storeProcedureName, parametros);

            foreach (var row in dr)
            {
                T obj = new T();

                foreach (System.Reflection.PropertyInfo prop in obj.GetType().GetProperties())
                {
                    if (row.Table.Columns.Contains(prop.Name))
                    {
                        try
                        {
                            PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                            propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
                list.Add(obj);
            }
            return list;
        }
        #endregion GetObject


        public DataBase()
        {
            dsn = new DatabaseProviderFactory().Create("dsn");
        }


        #region "Get Estructuras Datos"

        public List<DataRow> GetListaFilas(string NombreProcedimientoAlmacenado, params object[] parametros)
        {
            return getListaFilas(GetTabla(NombreProcedimientoAlmacenado, parametros));
        }

        public List<DataRow> GetListaFilasSQL(string strSQL)
        {
            DataSet ds;

            ds = GetDataSetSQL(strSQL);

            if (ds != null)
            {
                return getListaFilas(ds.Tables[0]);
            }
            else
            {
                return null;
            }


        }
        private List<DataRow> getListaFilas(DataTable dt)
        {
            if (dt != null)
            {
                return dt.AsEnumerable().ToList();
            }
            else
            {
                List<DataRow> dr = new List<DataRow>();
                return dr;
            }
            //IEnumerable<DataRow> sequence = dt.AsEnumerable();
        }

        public int GuardarBinario(byte[] binario, string Tabla, string NombreCampo, string NombreID, int ValorID)
        {

            string strSQL = "UPDATE " + Tabla + " SET " + NombreCampo + " = @binaryValue " + " WHERE " + NombreID + " = " + ValorID.ToString();

            SqlConnection conexion = new SqlConnection(dsn.ConnectionString);

            conexion.Open();

            using (SqlCommand cmd = new SqlCommand(strSQL, conexion))
            {
                cmd.Parameters.Add("@binaryValue", SqlDbType.VarBinary).Value = binario;
                cmd.ExecuteNonQuery();
            }

            conexion.Close();


            return 0;

        }

        public DataTable GetTablaDirecta(string NombreTabla)
        {

            string SQL = "SELECT * FROM " + NombreTabla;

            DataSet ds = null;
            DataTable dt = null;

            try
            {
                ds = GetDataSetSQL(SQL);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Print(e.Message);
            }
            try
            {
                dt = ds.Tables[0];
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }

            return dt;
        }


        public List<List<string>> GetListaTabla(string NombreProcedimientoAlmacenado, params object[] parametros)
        {
            return getListaTabla(GetTabla(NombreProcedimientoAlmacenado, parametros));
        }
        private List<List<string>> getListaTabla(DataTable dt)
        {

            List<List<string>> lstTable = new List<List<string>>();

            foreach (DataRow row in dt.Rows)
            {
                List<string> lstRow = new List<string>();
                foreach (var item in row.ItemArray)
                {
                    lstRow.Add(item.ToString().Replace("\r\n", string.Empty));
                }
                lstTable.Add(lstRow);
            }

            return lstTable;

        }

        public void Ejecuta(string NombreProcedimientoAlmacenado, params object[] parametros)
        {
            try
            {
                dsn.ExecuteNonQuery(NombreProcedimientoAlmacenado, parametros);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Print(e.Message);
            }

        }


        public DataTable GetTabla(string NombreProcedimientoAlmacenado, params object[] parametros)
        {
            //string R;
            DataSet ds = null;
            DataTable dt = null;

            try
            {
                ds = dsn.ExecuteDataSet(NombreProcedimientoAlmacenado, parametros);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Print(e.Message);
            }
            try
            {
                dt = ds.Tables[0];
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }

            return dt;
        }

        public DataTable GetTablaTimeOut(string NombrePA, params object[] Parametros)
        {
            DataTable dt = new DataTable();
            try
            {

                System.Data.Common.DbCommand cmd = dsn.GetStoredProcCommand(NombrePA, Parametros);
                cmd.CommandTimeout = 600;
                var dataReader = dsn.ExecuteReader(cmd);
                dt.Load(dataReader);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Print(e.Message);
            }


            return dt;
        }

        public DataTable GetTablaSQL(string SQL)
        {

            DataSet ds = null;
            DataTable dt = null;

            try
            {
                ds = GetDataSetSQL(SQL);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Print(e.Message);
            }
            try
            {
                dt = ds.Tables[0];
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }

            return dt;
        }

        public void EjecutaPA(string NombreProcedimientoAlmacenado, params object[] parametros)
        {
            try
            {
                dsn.ExecuteNonQuery(NombreProcedimientoAlmacenado, parametros);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Print(e.Message);
            }

        }

        public void Ejecuta(string SQL)
        {
            try
            {
                dsn.ExecuteNonQuery(CommandType.Text, SQL);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Print(e.Message);
            }

        }



        public DataSet GetDataSet(string NombreProcedimientoAlmacenado, params object[] parametros)
        {

            DataSet ds = null;

            try
            {

                ds = dsn.ExecuteDataSet(NombreProcedimientoAlmacenado, parametros);

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Print(e.Message);
            }


            return ds;

        }

        public DataSet GetDataSetSQL(string SQL)
        {

            DataSet ds = null;

            try
            {

                ds = dsn.ExecuteDataSet(CommandType.Text, SQL);

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Print(e.Message);
            }


            return ds;

        }

        public DataRow GetDatos(string NombreProcedimientoAlmacenado, params object[] parametros)
        {
            // Devuelve datos PrimeraFila
            DataTable dt = null;
            DataRow dr = null;
            try
            {
                dt = GetTabla(NombreProcedimientoAlmacenado, parametros);
                dr = dt.Rows[0];
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }

            return dr;
        }

        public DataRow GetDatos(string Sql)
        {
            // Devuelve datos PrimeraFila
            DataTable dt = null;
            DataRow dr = null;
            try
            {
                dt = GetTablaSQL(Sql);
                dr = dt.Rows[0];
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }

            return dr;
        }

        public DataRow GetDatosSQL(string SQL)
        {
            // Devuelve datos PrimeraFila
            DataTable dt = null;
            DataRow dr = null;
            try
            {
                dt = GetTablaSQL(SQL);
                dr = dt.Rows[0];
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }

            return dr;
        }


        public string GetEscalarString(string NombreProcedimientoAlmacenado, params object[] parametros)
        {
            string R = "";

            try
            {
                R = dsn.ExecuteScalar(NombreProcedimientoAlmacenado, parametros).ToString();
                //R = SqlHelper.ExecuteScalar(this.dsn, NombreProcedimientoAlmacenado, parametros).ToString();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }

            return R;
        }

        public int GetEscalarEntero(string NombreProcedimientoAlmacenado, params object[] parametros)
        {
            int R = 0;

            try
            {
                R = Convert.ToInt32(dsn.ExecuteScalar(NombreProcedimientoAlmacenado, parametros));
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }

            return R;
        }

        public bool GetEscalarBoleano(string NombreProcedimientoAlmacenado, params object[] parametros)
        {
            bool R = false;

            try
            {
                R = Convert.ToBoolean(dsn.ExecuteScalar(NombreProcedimientoAlmacenado, parametros));
            }
            catch (Exception e)
             {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }

            return R;
        }

        public double GetEscalarDouble(string NombreProcedimientoAlmacenado, params object[] parametros)
        {
            double R = 0;

            try
            {
                R = Convert.ToDouble(dsn.ExecuteScalar(NombreProcedimientoAlmacenado, parametros));
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }

            return R;
        }


        public string GetEscalarStringBytes(string NombreProcedimientoAlmacenado, params object[] parametros)
        {
            string R = "";

            try
            {

                byte[] Rb = (byte[])dsn.ExecuteScalar(NombreProcedimientoAlmacenado, parametros);
                R = Encoding.Default.GetString(Rb);


            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }

            return R;
        }

        public byte[] GetBytesPA(string NombrePA, params object[] Parametros)
        {
            byte[] Rb = null;

            try
            {

                Rb = (byte[])dsn.ExecuteScalar(NombrePA, Parametros);


            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }

            return Rb;
        }

        #endregion

    }
}
