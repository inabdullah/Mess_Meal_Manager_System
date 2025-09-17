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
    public partial class Deposite : Form
    {
        string connectionString = "data source=DESKTOP-2SPBHR5\\SQLEXPRESS; database=MMS; integrated security=SSPI";

        public Deposite()
        {
            InitializeComponent();
        }


        public delegate void DataSavedEventHandler();
        public event DataSavedEventHandler DataSaved;

        private void button1_Click(object sender, EventArgs e)
        {





        }

        private void button1_Click_1(object sender, EventArgs e)
        {

            string name = textBox2.Text.Trim();
            DateTime date = dateTimePicker1.Value;

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Please enter member name.", "Validation Error");
                return;
            }

            decimal amount;
            if (!decimal.TryParse(textBox1.Text, out amount) || amount <= 0)
            {
                MessageBox.Show("Please enter a valid amount!", "Validation Error");
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

                        
                        int amountInt = Convert.ToInt32(amount);  
                        string dateString = date.ToString("yyyy-MM-dd"); 

                        
                        string insertQuery = "INSERT INTO Deposite (MemberID, Amount, Date) VALUES (@MemberID, @Amount, @Date)";

                        using (SqlCommand insertCmd = new SqlCommand(insertQuery, connection))
                        {
                            insertCmd.Parameters.AddWithValue("@MemberID", memberID);
                            insertCmd.Parameters.AddWithValue("@Amount", amountInt);
                            insertCmd.Parameters.AddWithValue("@Date", dateString);       

                            int rowsAffected = insertCmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Fund Submitted successfully!", "Success");
                                this.Close();
                                //Home f1 = new Home();
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
    }

}