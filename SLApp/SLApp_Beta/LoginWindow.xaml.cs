using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SLApp_Beta
{
	/// <summary>
	/// Interaction logic for LoginWindow.xaml
	/// </summary>
	public partial class LoginWindow : Window
	{
		 private bool isAdmin;

        //TODO: Disable user from using the X button to skirt the login
		public LoginWindow(ref bool IsAdmin)
		{
			InitializeComponent();
			isAdmin = IsAdmin;
		}

		private void login_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (username_TB.Text == "admin")
			{
				isAdmin = true;
			}
			Close();
		}
	}
}
