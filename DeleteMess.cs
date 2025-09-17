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
    public partial class DeleteMess : Form
    {
        string connectionString = "data source=DESKTOP-2SPBHR5\\SQLEXPRESS; database=MMS; integrated security=SSPI";
        private int id;

        public DeleteMess()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
               "Are you sure you want to delete this mess and all related data?",
               "Confirm Deletion",
               MessageBoxButtons.YesNo,
               MessageBoxIcon.Warning
           );

            if (result == DialogResult.Yes)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {

                            string deleteCost = "DELETE FROM Cost WHERE MessId = @Id";
                            using (SqlCommand cmd = new SqlCommand(deleteCost, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@Id", id);
                                cmd.ExecuteNonQuery();
                            }

                            
                            string deleteDeposite = "DELETE FROM Deposite WHERE MessId = @Id";
                            using (SqlCommand cmd = new SqlCommand(deleteDeposite, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@Id", id);
                                cmd.ExecuteNonQuery();
                            }

                            
                            string deleteMeal = "DELETE FROM Meal WHERE MessId = @Id";
                            using (SqlCommand cmd = new SqlCommand(deleteMeal, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@Id", id);
                                cmd.ExecuteNonQuery();
                            }

                            
                            string deleteMonth = "DELETE FROM Month WHERE MessId = @Id";
                            using (SqlCommand cmd = new SqlCommand(deleteMonth, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@Id", id);
                                cmd.ExecuteNonQuery();
                            }

                            
                            string deleteBazarSchedule = "DELETE FROM BazarSchedule WHERE MessId = @Id";
                            using (SqlCommand cmd = new SqlCommand(deleteBazarSchedule, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@Id", id);
                                cmd.ExecuteNonQuery();
                            }

                            
                            string deleteMess = "DELETE FROM Mess WHERE Id = @Id";
                            using (SqlCommand cmd = new SqlCommand(deleteMess, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@Id", id);
                                int rowsAffected = cmd.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    transaction.Commit();
                                    MessageBox.Show("Mess and all related data deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    Close();
                                }
                                else
                                {
                                    transaction.Rollback();
                                    MessageBox.Show("No mess was found to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show($"Error deleting mess: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }
    }
}