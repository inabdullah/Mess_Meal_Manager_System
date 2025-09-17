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
    public partial class AddMeal : Form
    {
        string connectionString = "data source=DESKTOP-2SPBHR5\\SQLEXPRESS; database=MMS; integrated security=SSPI";
        public AddMeal()
        {
            InitializeComponent();
        }

        public delegate void DataSavedEventHandler();
        public event DataSavedEventHandler DataSaved;

        private void button1_Click(object sender, EventArgs e)
        {
            string name = textBox1.Text.Trim();
            DateTime date = dateTimePicker1.Value;

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Please enter member name.", "Validation Error");
                return;
            }

            decimal numberOfMeals;

            if (!decimal.TryParse(textBox3.Text, out numberOfMeals) || numberOfMeals < 0)
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

                        
                        int amountInt = Convert.ToInt32(numberOfMeals); 
                        string dateString = date.ToString("yyyy-MM-dd"); 

                        
                        string insertQuery = "INSERT INTO Meal (Date, NumberOfMeal, MemberID) VALUES (@Date, @NumberOfMeal, @MemberID)";

                        using (SqlCommand insertCmd = new SqlCommand(insertQuery, connection))
                        {
                            insertCmd.Parameters.AddWithValue("@Date", date);
                            insertCmd.Parameters.AddWithValue("@NumberOfMeal", numberOfMeals);
                            insertCmd.Parameters.AddWithValue("@MemberID", memberID);

                            int rowsAffected = insertCmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Meal Submitted successfully!", "Success");
                               this.Close();
                                
                            }
                            else
                            {
                                MessageBox.Show("Failed to Submit Meal.", "Error");
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
