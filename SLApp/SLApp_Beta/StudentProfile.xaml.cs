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
	/// Interaction logic for CreateStudentProfile.xaml
	/// </summary>
	public partial class StudentProfile : Window
	{
		private Student student = new Student();
		private bool isEdit;
		DatabaseMethods dbMethods = new DatabaseMethods();

		public StudentProfile(bool isAdmin)
		{
			InitializeComponent();
            if (isAdmin == false)
            {
                studentNotes_DataGrid.IsEnabled = false;
            }
		}

		public StudentProfile(Student stud, bool isAdmin, bool IsEdit)
		{
			InitializeComponent();
			if (isAdmin == false)
			{
				studentNotes_DataGrid.IsEnabled = false;
			}
			student = stud;
			isEdit = IsEdit;
			

			this.studentFirstName_TB.Text = stud.FirstName;
			this.studentLastName_TB.Text = stud.LastName;
			this.studentID_TB.Text = stud.Student_ID.ToString();
			this.studentemail_TB.Text = stud.Email;
			this.graduationYear_TB.Text = stud.GraduationYear.ToString();
			if (dbMethods.CheckDatabaseConnection())
			{
				using (PubsDataContext db = new PubsDataContext())
				{
					var completionList = new List<Learning_Experience>(from s in db.Learning_Experiences
					                                  where s.Student_ID == stud.Student_ID
					                                  select s);
					var completion = completionList.Single();
					if (completion.LiabilityWaiver == true)
					{
						this.liabilityWaiver_RBTN.IsChecked = true;
						
					}
					if (completion.ConfirmedHours == true)
					{
						this.confirmedHours_RBTN.IsChecked = true;
					}
					if (completion.ProjectAgreement == true)
					{
						this.projectAgreement_RBTN.IsChecked = true;
					}
					if (completion.TimeLog == true)
					{
						this.timeLog_RBTN.IsChecked = true;
					}
					studentLearningExperiences_DataGrid.DataContext = completionList;
				}
			}


		}

        public bool areStudentNotesLocked()
        {
            if (studentNotes_DataGrid.IsEnabled)
                return true;
            else
                return false;
        }

        private void cancel_BTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void save_BTN_Click(object sender, RoutedEventArgs e)
        {
            ///TODO: saves the profile to the database
            ///performs a check if saved properly before closing the window
			///http://www.c-sharpcorner.com/uploadfile/raj1979/showdeleteedit-data-in-wpf-datagrid-using-linq-to-sql-classes/

	        if (dbMethods.CheckDatabaseConnection())
	        {
		        using (PubsDataContext db = new PubsDataContext())
		        {
			        if (!isEdit)
			        {
				        student.Student_ID = Convert.ToInt32(studentID_TB.Text);
				        student.FirstName = studentFirstName_TB.Text;
				        student.LastName = studentLastName_TB.Text;
				        student.GraduationYear = Convert.ToInt32(graduationYear_TB.Text);
				        student.Email = studentemail_TB.Text;


				        //HACK possibly move completion of paperwork fields into the learning_experience table to simplify?
				        Learning_Experience coh = new Learning_Experience();
				        coh.Student_ID = Convert.ToInt32(studentID_TB.Text);
				        if (confirmedHours_RBTN.IsChecked == true)
				        {
					        coh.ConfirmedHours = true;
				        }
				        else
				        {
					        coh.ConfirmedHours = false;
				        }
				        if (liabilityWaiver_RBTN.IsChecked == true)
				        {
					        coh.LiabilityWaiver = true;
				        }
				        else
				        {
					        coh.LiabilityWaiver = false;
				        }
				        if (projectAgreement_RBTN.IsChecked == true)
				        {
					        coh.ProjectAgreement = true;
				        }
				        else
				        {
					        coh.ProjectAgreement = false;
				        }
				        if (timeLog_RBTN.IsChecked == true)
				        {
					        coh.TimeLog = true;
				        }
				        else
				        {
					        coh.TimeLog = false;
				        }
				        db.Students.InsertOnSubmit(student);
				        db.Learning_Experiences.InsertOnSubmit(coh);
				        db.SubmitChanges();
			        }
			        else
			        {
				        Student stud = (from s in db.Students
				                        where s.Student_ID == student.Student_ID
				                        select s).Single();
				        stud.Student_ID = Convert.ToInt32(studentID_TB.Text);
				        stud.FirstName = studentFirstName_TB.Text;
				        stud.LastName = studentLastName_TB.Text;
				        stud.GraduationYear = Convert.ToInt32(graduationYear_TB.Text);
				        stud.Email = studentemail_TB.Text;

				        Learning_Experience coh = (from s in db.Learning_Experiences
				                                   where s.Student_ID == student.Student_ID
				                                   select s).Single();
				        if (confirmedHours_RBTN.IsChecked == true)
				        {
					        coh.ConfirmedHours = true;
				        }
				        else
				        {
					        coh.ConfirmedHours = false;
				        }
				        if (liabilityWaiver_RBTN.IsChecked == true)
				        {
					        coh.LiabilityWaiver = true;
				        }
				        else
				        {
					        coh.LiabilityWaiver = false;
				        }
				        if (projectAgreement_RBTN.IsChecked == true)
				        {
					        coh.ProjectAgreement = true;
				        }
				        else
				        {
					        coh.ProjectAgreement = false;
				        }
				        if (timeLog_RBTN.IsChecked == true)
				        {
					        coh.TimeLog = true;
				        }
				        else
				        {
					        coh.TimeLog = false;
				        }

				        db.SubmitChanges();
			        }

		        }
	        }
	        this.Close();
        }

		private void delete_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (dbMethods.CheckDatabaseConnection())
			{
				if (MessageBox.Show("Are you sure you want to delete this student?", "Confirm Delete!", MessageBoxButton.YesNo) ==
				    MessageBoxResult.Yes)
				{
					using (PubsDataContext db = new PubsDataContext())
					{
						Student stud = (from s in db.Students
						                where s.Student_ID == student.Student_ID
						                select s).Single();
						Learning_Experience exp = (from ex in db.Learning_Experiences
						                           where ex.Student_ID == student.Student_ID
						                           select ex).Single();
						db.Students.DeleteOnSubmit(stud);
						db.Learning_Experiences.DeleteOnSubmit(exp);
						db.SubmitChanges();
						this.Close();
					}
				}
			}
			
		}
	}
}
