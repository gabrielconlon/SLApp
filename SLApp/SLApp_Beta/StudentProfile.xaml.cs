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
		public StudentProfile(bool isAdmin)
		{
			InitializeComponent();
            if (isAdmin == false)
            {
                studentNotes_DataGrid.IsEnabled = false;
            }
            ///TODO: if opening a student profile from the DB,
            ///needs to populate all fields, checkboxes, etc. from the data
		}

		public StudentProfile(Student stud)
		{
			InitializeComponent();

			this.studentFirstName_TB.Text = stud.FirstName;
			this.studentLastName_TB.Text = stud.LastName;
			this.studentID_TB.Text = stud.Student_ID.ToString();
			this.studentemail_TB.Text = stud.Email;
			this.graduationYear_TB.Text = stud.GraduationYear.ToString();
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
				Student addStudent = new Student();
				addStudent.Student_ID = Convert.ToInt32(studentID_TB.Text);
				addStudent.FirstName = studentFirstName_TB.Text;
				addStudent.LastName = studentLastName_TB.Text;
				addStudent.GraduationYear = Convert.ToInt32(graduationYear_TB.Text);
				addStudent.Email = studentemail_TB.Text;

				db.Students.InsertOnSubmit(addStudent);
				//breaks if you try to save something you previously opened
				db.SubmitChanges();
			}
            this.Close();
        }
	}
}
