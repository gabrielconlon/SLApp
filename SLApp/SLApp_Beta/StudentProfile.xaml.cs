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
			using (PubsDataContext db = new PubsDataContext())
			{
				Completion_of_Hour completion = (from s in db.Completion_of_Hours
												 where s.Student_ID == stud.Student_ID
												 select s).Single();
				if (completion.LiabilityWaver == true)
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
			using (PubsDataContext db = new PubsDataContext())
			{
				if (!isEdit)
				{
					student.Student_ID = Convert.ToInt32(studentID_TB.Text);
					student.FirstName = studentFirstName_TB.Text;
					student.LastName = studentLastName_TB.Text;
					student.GraduationYear = Convert.ToInt32(graduationYear_TB.Text);
					student.Email = studentemail_TB.Text;

					Completion_of_Hour coh = new Completion_of_Hour();
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
						coh.LiabilityWaver = true;
					}
					else
					{
						coh.LiabilityWaver = false;
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
					db.Completion_of_Hours.InsertOnSubmit(coh);
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

					Completion_of_Hour coh = (from s in db.Completion_of_Hours
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
						coh.LiabilityWaver = true;
					}
					else
					{
						coh.LiabilityWaver = false;
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
            this.Close();
        }

		private void delete_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (MessageBox.Show("Are you sure you want to delete this student?", "Confirm Delete!", MessageBoxButton.YesNo) ==
				  MessageBoxResult.Yes)
			{
				using (PubsDataContext db = new PubsDataContext())
				{
					Student stud = (from s in db.Students
									where s.Student_ID == student.Student_ID
									select s).Single();
					db.Students.DeleteOnSubmit(stud);
					db.SubmitChanges();
				}
			}
			this.Close();
		}
	}
}
