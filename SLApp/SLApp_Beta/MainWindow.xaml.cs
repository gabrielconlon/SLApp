using System;
using System.Windows;
using Test_app_1;

namespace SLApp_Beta
{
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
		/// </summary>
		private bool IsAdmin;

		#endregion

		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			this.IsEnabled = false;
			LoginWindow lgn = new LoginWindow(IsAdmin);
			lgn.Closed += new EventHandler(lgn_Closed);
			lgn.ShowDialog();
		}

		void lgn_Closed(object sender, EventArgs e)
		{
			this.IsEnabled = true;
		}

		private void menuCreateStudent_Click (object sender, RoutedEventArgs e)
		{
			StudentProfile Studentform = new StudentProfile();
			Studentform.Show();
		}

        private void menuCreateAgency_Click(object sender, RoutedEventArgs e)
        {
            AgencyProfile Agencyform = new AgencyProfile();
            Agencyform.Show();
        }

        private void newStudentProfile_BTN_Click(object sender, RoutedEventArgs e)
        {
            StudentProfile form = new StudentProfile();
            form.Show();
        }

        private void studentSearch_BTN_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Present a way for the prototype to populate some basic data which can be clicked to show what a student profile window will look like
        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

	}
}
