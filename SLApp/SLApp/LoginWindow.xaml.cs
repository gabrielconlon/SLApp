using System.Linq;
using System.Windows;

namespace SLApp_Beta
{
	/// <summary>
	/// Interaction logic for LoginWindow.xaml
	/// </summary>
        public partial class LoginWindow : Window
        {
                private bool isAdmin;
                DatabaseMethods dbMethods = new DatabaseMethods();
                private int loginAttempts = 0;

                public LoginWindow()
                {
                        InitializeComponent();
                        username_TB.Focus();
                }

                private void login_BTN_Click(object sender, RoutedEventArgs e)
                {
                        if (dbMethods.CheckDatabaseConnection())
                        {
                                using(PubsDataContext db = new PubsDataContext())
                                {
                                        var users = (from u in db.Application_Users
                                                where u.Username == username_TB.Text && u.Password == password_TB.Password
                                                select u).Distinct();
                                        if (users.Count() > 0)
                                        {
                                                isAdmin = users.First().IsAdmin;
                                                MainWindow main = new MainWindow(isAdmin);
                                                main.Show();
                                                Close();
                                        }
                                        //else if (loginAttempts >= 5)
                                        //{
                                        //        ; // TODO: feature- lockout user after 5 loginAttempts
                                        //}
                                        else
                                        {
                                                MessageBox.Show("Username or Password does not match!", "Login Error", MessageBoxButton.OK, MessageBoxImage.Stop);
                                                loginAttempts++;
                                        }

                                }
                        }
		}
	}
}
