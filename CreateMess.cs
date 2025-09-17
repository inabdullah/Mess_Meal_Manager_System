using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace MMS3
{
    public partial class CreateMess : Form
    {
        string connectionString = "data source=DESKTOP-2SPBHR5\\SQLEXPRESS; database=MMS; integrated security=SSPI";
        public CreateMess()
        {
            InitializeComponent();
        }

        private void CreateMess_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string managerID = textBox3.Text.Trim();
            string mName = textBox1.Text.Trim();
            DateTime date = dateTimePicker1.Value;



            if (string.IsNullOrWhiteSpace(managerID) || string.IsNullOrWhiteSpace(mName))
            {
                MessageBox.Show("All fields must be filled out.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            string query = "INSERT INTO Mess (MessName, CreatedDate, ManagerID) VALUES (@MessName, @CreatedDate, @ManagerID)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MessName", mName);
                    command.Parameters.AddWithValue("@CreatedDate", date);
                    command.Parameters.AddWithValue("@ManagerID",managerID);



                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Mess created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //this.Hide();
                        //Home f1 = new Home();
                        //f1.Show();
                    }
                    else
                    {
                        MessageBox.Show("Failed to create the Mess. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }




        }
    }
}
