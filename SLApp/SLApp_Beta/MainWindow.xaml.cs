using System;
using System.Windows;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Input;
using System.Windows.Documents;

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

            if (dbMethods.CheckDatabaseConnection())
            {
	            try
	            {
		            using (PubsDataContext db = new PubsDataContext())
		            {
			            serviceType_CBX.DataContext = from service in db.Types_of_Services
			                                          select service;
		            }
	            }
	            catch (Exception ex)
	            {
		            ;
	            }
            }
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
            ///TODO: setup for a check in the login window
            ///will need to check against the database, so the database must
            ///be open and a 'users' table exists with the App login present
			this.IsEnabled = false;
			LoginWindow lgn = new LoginWindow(ref IsAdmin);
			lgn.Closed += new EventHandler(lgn_Closed);
			lgn.ShowDialog();
		}

		private void lgn_Closed(object sender, EventArgs e)
		{
			this.IsEnabled = true;
		}

        #region Menu

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void aboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }

        private void manualMenuItem_Click(object sender, RoutedEventArgs e)
        {
             UserDocumentation manual = new UserDocumentation();
			manual.Show();
        }

        private void menuCreateStudent_Click(object sender, RoutedEventArgs e)
        {
            StudentProfile Studentform = new StudentProfile(IsAdmin);
			Studentform.Closed += new EventHandler((s0, e0) => { studentSearch_BTN_Click(s0, null); });
            Studentform.Show();
        }

		private void menuCreateAgency_Click(object sender, RoutedEventArgs e)
		{
			AgencyProfile Agencyform = new AgencyProfile();
			Agencyform.Show();
		}

        #endregion

        #region Student Tab

        private void newStudentProfile_BTN_Click(object sender, RoutedEventArgs e)
        {
            StudentProfile form = new StudentProfile(IsAdmin);
			form.Closed += new EventHandler((s0, e0) => studentSearch_BTN_Click(s0, null));
            form.Show();
        }

        private void studentSearch_BTN_Click(object sender, RoutedEventArgs e)
        {
            /// <summary>
            ///Data grid based off of which fields have data entered into them,
            ///if a box has no txt entered then it is not included in the query generation
            ///
            /// Fields are: studentID_TB, studentLastName_TB, studentMiddleName_TB, graduatinoYear_TB
            /// </summary>
            /// 
			if(dbMethods.CheckDatabaseConnection())
			{
				using (PubsDataContext db = new PubsDataContext())
				{
					var allStudents = (from stud in db.Students
					                   join experience in db.Learning_Experiences on stud.Student_ID equals experience.Student_ID

									   //TODO: almost works, the join forces students to come up who have a course that matches an experience
									   //join course in db.Courses on experience.CourseNumber equals course.CourseNumber

					                   where
						                   //student search section
									   //NOW ABLE TO SEARCH FOR PARTIALS, accept for the ints
										   (studentFirstName_TB.Text.Length == 0 || stud.FirstName.Contains(studentFirstName_TB.Text)) && //studentFirstName_TB.Text == stud.FirstName
						                   (studentLastName_TB.Text.Length == 0 || stud.LastName.Contains(studentLastName_TB.Text)) && //studentLastName_TB.Text == stud.LastName
						                   (studentID_TB.Text.Length == 0 || studentID_TB.Text == stud.Student_ID.ToString()) &&
										   //TODO graduation year duplicates
						                   (graduationYear_TB.Text.Length == 0 || graduationYear_TB.Text == stud.GraduationYear.ToString()) &&

										   //course search section
										   //TODO: need to input data that matches the course up
										   //(course_TB.Text.Length == 0 || course_TB.Text == course.CourseName) &&
										   //(professor_TB.Text.Length == 0 || professor_TB.Text == course.Professor) &&

										   //service and hours section
										   //HACK Combo Boxes do not work
										   (serviceType_CBX.Text.Length == 0 || experience.TypeofLearning.Contains(serviceType_CBX.Text) ) && //serviceType_CBX.Text == experience.TypeofLearning
										   (semester_CBX.Text.Length == 0 || experience.Semester.Contains(semester_CBX.Text)) && //semester_CBX.Text == experience.Semester
										   (year_TB.Text.Length == 0 || year_TB.Text == experience.Year.ToString()) 

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
				studentForm.Closed += new EventHandler((s0, e0) => studentSearch_BTN_Click(s0, null));
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

        private void newAgencyProfile_BTN_Click(object sender, RoutedEventArgs e)
        {
            AgencyProfile Agencyform = new AgencyProfile();
            Agencyform.Show();
        }

        #endregion


        #region ContextMenu

        private void Add_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            StudentProfile form = new StudentProfile(IsAdmin);
            form.Show();
        }

        private void Edit_MenuItem_Click(object sender, RoutedEventArgs e)
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

        private void Delete_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (dbMethods.CheckDatabaseConnection())
            {
                if (MessageBox.Show("Are you sure you want to delete this student?", "Confirm Delete!", MessageBoxButton.YesNo) ==
                    MessageBoxResult.Yes)
                {
                    
                    using (PubsDataContext db = new PubsDataContext())
                    {
                        Student studentRow = studentSearch_DataGrid.SelectedItem as Student;
                        Student stud = (from s in db.Students
                                        where s.Student_ID == studentRow.Student_ID
                                        select s).Single();
                        Learning_Experience exp = (from ex in db.Learning_Experiences
                                                   where ex.Student_ID == stud.Student_ID
                                                   select ex).Single();
                        db.Students.DeleteOnSubmit(stud);
                        db.Learning_Experiences.DeleteOnSubmit(exp);
                        db.SubmitChanges();
                        this.Close();
                    }
                }
            }
        }

        #endregion


    }
}
