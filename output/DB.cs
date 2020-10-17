using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace output
{
    class DB
    {
        protected OracleConnection Connection;
        private string connectionString;
        public DB()
        {
            string connStr;
           
    connStr = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=" + Form1.IP + ")(PORT = 1521))(CONNECT_DATA =(SERVICE_NAME =" + Form1.DB + ")));Persist Security Info=True;User ID=" +Form1.User + ";Password= " + Form1.PWD;

          //  connStr = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=10.228.140.50)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=ORCL)));Persist Security Info=True;User ID=JLS;Password=000409;";
            connectionString = connStr;
            Connection = new OracleConnection(connectionString);


        }
        #region 打开数据库
        /// <summary>
        /// 打开数据库
        /// </summary>
        public void OpenConn()
        {
            if (this.Connection.State != ConnectionState.Open)
                this.Connection.Open();
        }
        #endregion
        #region 关闭数据库联接
        /// <summary>
        /// 关闭数据库联接
        /// </summary>
        public void CloseConn()
        {
            if (Connection.State == ConnectionState.Open)
                Connection.Close();
        }
        #endregion

        #region 执行SQL语句，返回数据到DataSet中
        /// <summary>
        /// 执行SQL语句，返回数据到DataSet中
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="DataSetName">自定义返回的DataSet表名</param>
        /// <returns>返回DataSet</returns>
        public DataSet ReturnDataSet(string sql, string DataSetName)
        {
            DataSet dataSet = new DataSet();
            OpenConn();
            OracleDataAdapter OraDA = new OracleDataAdapter(sql, Connection);
            OraDA.Fill(dataSet, DataSetName);
            //   CloseConn();
            return dataSet;
        }
        #endregion

        #region 执行Sql语句,返回带分页功能的dataset
        /// <summary>
        /// 执行Sql语句,返回带分页功能的dataset
        /// </summary>
        /// <param name="sql">Sql语句</param>
        /// <param name="PageSize">每页显示记录数</param>
        /// <param name="CurrPageIndex"><当前页/param>
        /// <param name="DataSetName">返回dataset表名</param>
        /// <returns>返回DataSet</returns>
        public DataSet ReturnDataSet(string sql, int PageSize, int CurrPageIndex, string DataSetName)
        {
            DataSet dataSet = new DataSet();
            OpenConn();
            OracleDataAdapter OraDA = new OracleDataAdapter(sql, Connection);
            OraDA.Fill(dataSet, PageSize * (CurrPageIndex - 1), PageSize, DataSetName);
            //   CloseConn();
            return dataSet;
        }
        #endregion

        #region 执行SQL语句，返回 DataReader,用之前一定要先.read()打开,然后才能读到数据
        /// <summary>
        /// 执行SQL语句，返回 DataReader,用之前一定要先.read()打开,然后才能读到数据
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>返回一个OracleDataReader</returns>
        public OracleDataReader ReturnDataReader(String sql)
        {
            OpenConn();
            OracleCommand command = new OracleCommand(sql, Connection);
            return command.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        }
        #endregion

        #region 执行SQL语句，返回记录总数数
        /// <summary>
        /// 执行SQL语句，返回记录总数数
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>返回记录总条数</returns>
        public int GetRecordCount(string sql)
        {
            int recordCount = 0;
            OpenConn();
            OracleCommand command = new OracleCommand(sql, Connection);
            OracleDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                recordCount++;
            }
            dataReader.Close();
            //   CloseConn();
            return recordCount;
        }
        #endregion

        #region 取当前序列,条件为seq.nextval或seq.currval
        /// <summary>
        /// 取当前序列
        /// </summary>
        /// <param name="seqstr"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public decimal GetSeq(string seqstr)
        {
            decimal seqnum = 0;
            string sql = "select " + seqstr + " from dual";
            OpenConn();
            OracleCommand command = new OracleCommand(sql, Connection);
            OracleDataReader dataReader = command.ExecuteReader();
            if (dataReader.Read())
            {
                seqnum = decimal.Parse(dataReader[0].ToString());
            }
            dataReader.Close();
            //   CloseConn();
            return seqnum;
        }
        #endregion

        #region 执行SQL语句,返回所影响的行数
        /// <summary>
        /// 执行SQL语句,返回所影响的行数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int ExecuteSQL(string sql)
        {
            int Cmd = 0;
            OpenConn();
            OracleCommand command = new OracleCommand(sql, Connection);
            try
            {
                Cmd = command.ExecuteNonQuery();
            }
            catch
            {

            }
            finally
            {
                //    CloseConn();
            }

            return Cmd;
        }
        #endregion
    }
}
