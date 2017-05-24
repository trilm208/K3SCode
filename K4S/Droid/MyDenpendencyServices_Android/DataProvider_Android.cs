using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using DataAccess;
using Xamarin.Forms;

[assembly: Dependency(typeof(DataProvider_Android))]
namespace DataAccess
{
	public class DataProvider_Android : IDataProvider
	{
		private string _serverString = @"Data Source=113.161.97.235,6969;Network Library=DBMSSOCN;User Id=sa;
Password=dkcnyh20081992;";

		//private static string servername = "113.161.97.235,6969\\Kadence";

		public DataSet Excute(RequestCollection requests)
		{
			var session = Guid.NewGuid().ToString();
			var response = new DataSet();

			SqlTransaction transaction = null;

			try
			{

				using (SqlConnection connection = new SqlConnection(_serverString))
				{
					connection.Open();
					transaction = connection.BeginTransaction();

					foreach (var request in requests)
					{
						var category = request["Attributes"]["Category"].Value;
						var command = request["Attributes"]["Command"].Value;
						var parameters = request["Parameters"];

						if (command.StartsWith("ws_"))
						{
							if (ProcessNative(session, category, command, parameters, response) == false)
							{
								ProcessSql(connection, transaction, session, category, command, parameters, response);
							}
						}
					}

					transaction.Commit();
				}
			}
			catch (Exception e)
			{
				var table = new DataTable("Error");
				table.Columns.Add("Message");
				table.Columns.Add("Source");
				table.Columns.Add("StackTrace");
				table.Columns.Add("HelpLink");

				var row = table.NewRow();
				row["Message"] = e.Message;
				row["Source"] = e.Source;
				row["StackTrace"] = e.StackTrace;
				row["HelpLink"] = e.HelpLink;

				table.Rows.Add(row);

				response = new DataSet();
				response.Tables.Add(table);
			}

			return response;
		}

		private static void ProcessSql(SqlConnection connection, SqlTransaction transaction, string session, string category, string command, NameValueCollection parameters, DataSet response)
		{
			using (SqlCommand cmd = new SqlCommand(category + ".." + command, connection))
			{
				cmd.Transaction = transaction;
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@SessionID", SqlDbType.VarChar).Value = session;

				foreach (var parameter in parameters)
				{
					var name = parameter.Name;
					var value = parameter.Value;

					name = Misc.SafeSqlName(name);

					if (parameter.IsNull)
					{
						value = null;
					}

					cmd.Parameters.Add("@" + name, SqlDbType.NVarChar).Value = value;
				}

				using (SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd))
				{
					var result = new DataSet();
					dataAdapter.Fill(result);
					AppendDataSet(response, result);
				}
			}
		}


		private static bool ProcessNative(string session, string category, string command, NameValueCollection parameters, DataSet response)
		{
			return false;
		}


		private static void AppendDataSet(DataSet target, DataSet source)
		{
			var source_tables = source.Tables.Cast<DataTable>().ToArray();

			foreach (DataTable table in source_tables)
			{
				source.Tables.Remove(table);

				string tableName = "Table";
				if (target.Tables.Count > 0)
					tableName += target.Tables.Count;

				table.TableName = tableName;
				target.Tables.Add(table);

			}
		}

        public DataSet ExcuteAsync(RequestCollection requests)
        {
            return Excute(requests);
        }
    }
}
