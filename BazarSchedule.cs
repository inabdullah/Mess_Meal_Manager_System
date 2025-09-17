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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MMS3
{
    public partial class BazarSchedule : Form
    {
        string connectionString = "data source=DESKTOP-2SPBHR5\\SQLEXPRESS; database=MMS; integrated security=SSPI";
        public BazarSchedule()
        {
            InitializeComponent();
        }

        public delegate void DataSavedEventHandler();
        public event DataSavedEventHandler DataSaved;

        private void button1_Click(object sender, EventArgs e)
        {
            string name = textBox1.Text;
            DateTime date = dateTimePicker1.Value;
            string remarks = richTextBox1.Text;

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Please enter member name.", "Validation Error");
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    
                    string getMemberQuery = "SELECT MemberID FROM Member WHERE Name = @Name";

                    using (SqlCommand getMemberCmd = new SqlCommand(getMemberQuery, connection))
                    {
                        getMemberCmd.Parameters.AddWithValue("@Name", name);
                        object result = getMemberCmd.ExecuteScalar();

                        if (result == null)
                        {
                            MessageBox.Show($"Member '{name}' not found!");
                            return;
                        }

                        int memberID = Convert.ToInt32(result);

                        
                        string dateString = date.ToString("yyyy-MM-dd");


                        string insertQuery = "INSERT INTO BazarSchedule (MemberID, Date, Remarks) VALUES (@MemberID, @Date, @Remarks)";

                        using (SqlCommand insertCmd = new SqlCommand(insertQuery, connection))
                        {
                            insertCmd.Parameters.AddWithValue("@MemberID", memberID);
                            insertCmd.Parameters.AddWithValue("@Date", dateString);        
                            insertCmd.Parameters.AddWithValue("@Remarks", remarks);

                            int rowsAffected = insertCmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Date Added successfully!", "Success");
                                //this.Hide();
                                //Home f1 = new Home();
                             this.Close();
                                //f1.Show();
                            }
                            else
                            {
                                MessageBox.Show("Failed to insert deposit.", "Error");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Database Error");
                }
            }



        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
