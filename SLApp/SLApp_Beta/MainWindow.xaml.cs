using System;
using System.Windows;
using System.Data.SqlClient;
using System.Data.OleDb;

namespace SLApp_Beta
{
    ///code to build and run if Debug mode
    ///TODO: code to be run in debug mode
    ///HACK: ASK PETE, is this better than using a unit test?
#if (DEBUG)
    public class Conditionals
    {
        private bool mustbe(bool condition1, bool condition2)
        {
            if (condition1 = condition2)
                return true;
            else
                return false;
        }
    }
#else
#endif

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		#region Application State Variables

		/// <summary>
		/// this bool will be used to determine the state of the application
		/// if someone other than an admin (i.e. student worker) is signed in
		/// then all notes fields, and any other fields deemed FERPA protected
		/// will have IsEnabled = false
        /// 
        /// FERPA Fields currently include:
        /// -Notes
		/// </summary>
		private bool IsAdmin;

		#endregion

		public MainWindow()
		{
            ///HACK: ASK PETE if this is appropriate or if there is a better way
            /// Attempt Db connection, catch all exceptions and handle with a Dialog Window to user
            ///http://msdn.microsoft.com/en-us/library/aa969773.aspx messagebox info
            ///http://msdn.microsoft.com/en-us/library/bb386876.aspx OLE DB info
            ///http://msdn.microsoft.com/en-us/library/bb399398.aspx more OLE DB info
            ///http://msdn.microsoft.com/en-us/library/aa288452%28v=vs.71%29.aspx OLE DB tutorial from MS
            try
            {
                string stConnect = "Proivder=sqloledb; Data Source=cs1;" + "Initial Catalog=SLDatabase; Integrated Security=SSPI;";
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = new OleDbConnection(stConnect);
                cmd.Connection.Open();
            }
            catch (Exception ex)
            {
                string messageBoxText = "The database could not be opened.  This application cannot load any information without a database connection."+ 
                    "\n\nContinue anyways?";
                string caption = "Database Error";
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxImage icon = MessageBoxImage.Warning;

                //show a dialog indicating the database has not opened successfully
                MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

                //handles the user input
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        //user pressed yes, wants to open App anyways
                        break;
                    case MessageBoxResult.No:
                        this.Close();
                        break;
                }
            }
            ///TODO: create a login for the app, so the database can be
            ///accessed without a user ID (e.g. Ross or student worker putting in a university login)
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
            ///TODO: setup for a check in the login window
            ///will need to check against the database, so the database must
            ///be open and a 'users' table exists with the App login present
			this.IsEnabled = false;
			LoginWindow lgn = new LoginWindow(IsAdmin);
			lgn.Closed += new EventHandler(lgn_Closed);
			lgn.ShowDialog();
		}

		private void lgn_Closed(object sender, EventArgs e)
		{
			this.IsEnabled = true;
		}

		private void menuCreateStudent_Click (object sender, RoutedEventArgs e)
		{
			StudentProfile Studentform = new StudentProfile(IsAdmin);
			Studentform.Show();
		}

        private void menuCreateAgency_Click(object sender, RoutedEventArgs e)
        {
            AgencyProfile Agencyform = new AgencyProfile();
            Agencyform.Show();
        }

        private void newStudentProfile_BTN_Click(object sender, RoutedEventArgs e)
        {
            StudentProfile form = new StudentProfile(IsAdmin);
            form.Show();
        }

        private void studentSearch_BTN_Click(object sender, RoutedEventArgs e)
        {
            ///TODO: Present a way for the prototype to populate some basic data 
            ///which can be clicked to show what a student profile window will look like
        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

	}
}
