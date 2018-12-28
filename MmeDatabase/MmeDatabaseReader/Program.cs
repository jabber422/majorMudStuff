using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;

namespace MmeDatabaseReader
{
    class Program
    {
        static void Main(string[] args)
        {
            string myConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;" +
                               @"Data Source=C:\sandbox\majorMudStuff\MmeDatabase\test.mdb;" +
                               "Persist Security Info=True;" +
                               "Jet OLEDB:Database Password=myPassword;";
            try
            {
                // Open OleDb Connection
                OleDbConnection myConnection = new OleDbConnection();
                myConnection.ConnectionString = myConnectionString;
                myConnection.Open();

                string getDbName = @"SELECT MSysObjects.Name AS table_name " +
                    @"FROM MSysObjects " +
                    "WHERE(((Left([Name], 1)) <> \"~\") " +
                    "AND((Left([Name], 4)) <> \"MSys\") " +
                    @"AND((MSysObjects.Type)In(1, 4, 6))) " +
                    @"order by MSysObjects.Name";
                string getAllTableNames = "" +
                    "SELECT Name, 'Summoned By' "+
                    "FROM   Monsters " +
                    "WHERE(RegenTime <> 0)";


                // Execute Queries
                OleDbCommand cmd = myConnection.CreateCommand();
                cmd.CommandText = getAllTableNames;
                    
                OleDbDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection); // close conn after complete

                // Load the result into a DataTable
                DataTable myDataTable = new DataTable();
                myDataTable.Load(reader);
            }
            catch (Exception ex)
            {
                Console.WriteLine("OLEDB Connection FAILED: " + ex.Message);
            }

            Console.ReadKey();
        }
    }
}

