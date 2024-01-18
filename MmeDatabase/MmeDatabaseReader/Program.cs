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
                   @"Data Source=master.mdb;" +
                   "Persist Security Info=True;" +
                   "Jet OLEDB:Database Password=myPassword;";

            try
            {
                MMudData.GetNpc(new MMudObjects.Entity("giant rat"));

                using (OleDbConnection myConnection = new OleDbConnection(myConnectionString))
                {
                    myConnection.Open();

                    string getDbName = @""+
                        "SELECT *" +
                        "FROM   Monsters " +
                        "WHERE(RegenTime <> 0)";


                    using (OleDbCommand cmd = new OleDbCommand(getDbName, myConnection))
                    {
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            DataTable myDataTable = new DataTable();
                            myDataTable.Load(reader);

                            // Print table names
                            foreach (DataRow row in myDataTable.Rows)
                            {
                                Console.WriteLine(row[0].ToString() + " - " + row[1].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }


            Console.ReadKey();
        }
    }
}

