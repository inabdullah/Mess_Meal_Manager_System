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
    public partial class Register : Form
    {
        public Register()
        {
            InitializeComponent();
        }

        private void Register_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
           
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            this.Hide();
            Form1 f1 = new Form1();
            f1.Show();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            string connectionString = "data source=DESKTOP-2SPBHR5\\SQLEXPRESS; database=MMS; integrated security=SSPI";

            string name = textBox1.Text.Trim();
            string email = textBox2.Text.Trim();
            string phone = textBox3.Text.Trim();
            string pass = textBox4.Text.Trim();


            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(pass))
            {
                MessageBox.Show("All fields must be filled out.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool isManager = false;
            bool isMember = false;

            if (radioButton1 != null && radioButton1.Checked) 
            {
                isMember = true;
            }
            else if (radioButton2 != null && radioButton2.Checked)
            {
                isManager = true;
            }

            
            if (!isMember && !isManager)
            {
                MessageBox.Show("Please select a role (Member or Manager).", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            
                            string memberQuery = "INSERT INTO Member (Name, Phone, Email, Password) VALUES (@Name, @Phone, @Email, @Password)";

                            using (SqlCommand memberCommand = new SqlCommand(memberQuery, connection, transaction))
                            {
                                memberCommand.Parameters.AddWithValue("@Name", name);
                                memberCommand.Parameters.AddWithValue("@Phone", phone);
                                memberCommand.Parameters.AddWithValue("@Email", email);
                                memberCommand.Parameters.AddWithValue("@Password", pass);

                                memberCommand.ExecuteNonQuery();
                            }

                            
                            if (isManager)
                            {
                                string managerQuery = "INSERT INTO Manager (Name, Email, Phone, Password) VALUES (@Name, @Email, @Phone, @Password)";

                                using (SqlCommand managerCommand = new SqlCommand(managerQuery, connection, transaction))
                                {
                                    managerCommand.Parameters.AddWithValue("@Name", name);
                                    managerCommand.Parameters.AddWithValue("@Email", email);
                                    managerCommand.Parameters.AddWithValue("@Phone", phone);
                                    managerCommand.Parameters.AddWithValue("@Password", pass);

                                    managerCommand.ExecuteNonQuery();
                                }
                            }

                            
                            transaction.Commit();

                            
                            string roleText = isManager ? "Manager" : "Member";
                            MessageBox.Show($"Profile created successfully as {roleText}!", "Success",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);


                            this.Hide();
                            Form1 f1 = new Form1();
                            f1.Show();
                        }
                        catch (Exception ex)
                        {
                            
                            transaction.Rollback();

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to create the profile. Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
