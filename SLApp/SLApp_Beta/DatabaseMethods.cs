using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Input;

namespace SLApp_Beta
{
    class DatabaseMethods
    {
        public bool CheckDatabaseConnection()
        {
            Mouse.SetCursor(Cursors.Wait);
            using (PubsDataContext db = new PubsDataContext())
            {
                if (db.DatabaseExists())
                {
                    Mouse.SetCursor(Cursors.Arrow);
                    return true;
                }
                else
                {
                    MessageBox.Show("Database connection is down.", "Database Connection Error", MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    Mouse.SetCursor(Cursors.Arrow);
                    return false;
                }
            }
        }

    }
}
