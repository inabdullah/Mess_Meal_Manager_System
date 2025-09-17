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
    public partial class AllMembers : Form
    {
        string connectionString = "data source=DESKTOP-2SPBHR5\\SQLEXPRESS; database=MMS; integrated security=SSPI";
        public AllMembers()
        {
            InitializeComponent();
            string query = "SELECT MemberID, Name, Phone, Email FROM Member"; 

            FillDataGridView(query);
        }

        private void FillDataGridView(string query)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    DataTable dataTable = new DataTable();
                    dataTable.Load(reader);
                    dataGridView1.DataSource = dataTable;
                }
            }

        }


        private void AllMembers_Load(object sender, EventArgs e)
        {

        }
    }
}
