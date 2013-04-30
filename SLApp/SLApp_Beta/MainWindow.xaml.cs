using System;
using System.Windows;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Input;

namespace SLApp_Beta
{
	public class DatabaseMethods
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

		private DatabaseMethods dbMethods = new DatabaseMethods();
		

		public MainWindow()
		{
            ///http://msdn.microsoft.com/en-us/library/aa969773.aspx messagebox info
            ///http://msdn.microsoft.com/en-us/library/bb386876.aspx OLE DB info
            ///http://msdn.microsoft.com/en-us/library/bb399398.aspx more OLE DB info
            ///http://msdn.microsoft.com/en-us/library/aa288452%28v=vs.71%29.aspx OLE DB tutorial from MS
			/// http://www.codeproject.com/Tips/362436/Data-binding-in-WPF-DataGrid-control datagrid info and binding
            ///TODO: create a login for the app, so the database can be
            ///accessed without a user ID (e.g. Ross or student worker putting in a university login)
            ///


			InitializeComponent();
			DatabaseMethods dbMethods = new DatabaseMethods();
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

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void menuCreateStudent_Click(object sender, RoutedEventArgs e)
        {
            StudentProfile Studentform = new StudentProfile(IsAdmin);
            Studentform.Show();
        }

        private void menuCreateAgency_Click(object sender, RoutedEventArgs e)
        {
            AgencyProfile Agencyform = new AgencyProfile();
            Agencyform.Show();
        }

        #region Student Tab

        private void newStudentProfile_BTN_Click(object sender, RoutedEventArgs e)
        {
            StudentProfile form = new StudentProfile(IsAdmin);
            form.Show();
        }

        private void studentSearch_BTN_Click(object sender, RoutedEventArgs e)
        {
            /// <summary>
            ///Data grid based off of which fields have data entered into them,
            ///if a box has no txt entered then it is not included in the query generation
            ///
            /// Fields are: studentID_TB, studentLastName_TB, studentMiddleName_TB, graduatinoYear_TB
            /// HACK FIX THIS CODE: all matching needs to be reset to match the database (i.e. stud.First_Name)
            /// HACK BUG: Need to properly link the database so the SQL works
            /// </summary>
            /// 
			if(dbMethods.CheckDatabaseConnection())
			{
				using (PubsDataContext db = new PubsDataContext())
				{
					var allStudents = (from stud in db.Students
					                   //from course in db.Courses
					                   //from experience in db.Learning_Experiences
					                   where
						                   //student search section
						                   (studentFirstName_TB.Text.Length == 0 || studentFirstName_TB.Text == stud.FirstName) &&
						                   (studentLastName_TB.Text.Length == 0 || studentLastName_TB.Text == stud.LastName) &&
						                   (studentID_TB.Text.Length == 0 || studentID_TB.Text == stud.Student_ID.ToString()) &&
						                   (graduationYear_TB.Text.Length == 0 ||
						                    graduationYear_TB.Text == stud.GraduationYear.ToString())

					                   ////course search section
					                   //(course_TB.Text.Length == 0 || course_TB.Text == course.CourseName) &&
					                   //(semester_CBX.SelectedIndex != 0 ||
					                   // semester_CBX.SelectedIndex.ToString() == experience.Semester) &&
					                   //(year_TB.Text.Length == 0 || year_TB.Text == experience.Year.ToString()) &&
					                   //(professor_TB.Text.Length == 0 || professor_TB.Text == course.Professor) 

					                   //                       ////service and hours section
					                   //                       ////HACK not sure about this one...
					                   //                       //(serviceType_CBX.SelectedIndex != 0 ||
					                   //                       // serviceType_CBX.SelectedIndex.ToString() == experience.TypeofLearning)

					                   select stud);
					studentSearch_DataGrid.DataContext = allStudents;
				}
			}


        }

		/// <summary>
		/// http://stackoverflow.com/questions/11070873/how-to-get-value-of-a-cell-from-datagrid-in-wpf
		/// http://stackoverflow.com/questions/5549321/how-to-read-value-from-a-cell-from-a-wpf-datagrid
		/// http://www.scottlogic.co.uk/blog/colin/2008/12/wpf-datagrid-detecting-clicked-cell-and-row/
		/// http://www.codeproject.com/Articles/24192/Simple-Demo-of-Binding-to-a-Database-in-WPF-using
		/// http://www.codeproject.com/Articles/46422/A-LINQ-Tutorial-Adding-Updating-Deleting-Data
		/// http://stackoverflow.com/questions/3913580/get-selected-row-item-in-datagrid-wpf
		/// http://www.youtube.com/watch?v=LCfvcBObX8k
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>

		private void StudentSearch_DataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			using (PubsDataContext datab = new PubsDataContext())
			{
				Student studentRow = studentSearch_DataGrid.SelectedItem as Student;
				Student stud = (from s in datab.Students
				                where s.Student_ID == studentRow.Student_ID
				                select s).Single();

				StudentProfile studentForm = new StudentProfile(stud, IsAdmin, true);
				studentForm.Show();
			}
		}
        #endregion

        #region Agency Tab
        private void agencySearch_BTN_Click(object sender, RoutedEventArgs e)
        {
            using (PubsDataContext db = new PubsDataContext())
            {
                var allAgency = (from agency in db.Agencies
                                   where
                                 (agencyName_TB.Text.Length == 0 || agencyName_TB.Text == agency.Name)

                                   select agency);
                agencySearch_DataGrid.DataContext = allAgency;
            }
        }

        #endregion
    }
    ///code to build and run if Debug mode
    ///TODO: code to be run in debug mode
    ///HACK ASK PETE, is this better than using a unit test?

}
