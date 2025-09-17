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

namespace MMS3
{
    public partial class AddCost : Form
    {

        string connectionString = "data source=DESKTOP-2SPBHR5\\SQLEXPRESS; database=MMS; integrated security=SSPI";

        public AddCost()
        {
            InitializeComponent();
        }

        public delegate void DataSavedEventHandler();
        public event DataSavedEventHandler DataSaved;

        private void AddCost_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {

            DateTime date = dateTimePicker1.Value;
            string details = richTextBox1.Text;
            string name = textBox2.Text.Trim();

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

                        
                        string insertQuery = "INSERT INTO Cost (Amount, Date, Details, MemberID) VALUES (@Amount, @Date, @Details, @MemberID)";

                        using (SqlCommand insertCmd = new SqlCommand(insertQuery, connection))
                        {
                            insertCmd.Parameters.AddWithValue("@Amount", amount);
                            insertCmd.Parameters.AddWithValue("@Date", date);
                            insertCmd.Parameters.AddWithValue("@Details", details);
                            insertCmd.Parameters.AddWithValue("@MemberID", memberID);

                            int rowsAffected = insertCmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Cost Submitted successfully!", "Success");
                                this.Close();
                            }
                            else
                            {
                                MessageBox.Show("Failed to Submit Cost.", "Error");
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
