using System;
using System.Collections;
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
    public partial class Home : Form
    {
        string connectionString = "data source=DESKTOP-2SPBHR5\\SQLEXPRESS; database=MMS; integrated security=SSPI";
        string currentMemberName; 
        int currentMemberID; 
        
        public Home()
        {
            InitializeComponent();
            CustomizeDesign();
        }

        public void RefreshAllData()
        {
            RefreshCalculations();
        }

        public Home(string memberName)
        {
            currentMemberName = memberName;
            InitializeComponent();
            CustomizeDesign();
            
           
            GetMemberID();
            LoadTotalCalculations();
            LoadIndividualCalculations();
        }

        private void GetMemberID()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT MemberID FROM Member WHERE Name = @Name";
                    using (SqlCommand command = new SqlCommand(query, con))
                    {
                        command.Parameters.AddWithValue("@Name", currentMemberName);
                        con.Open();
                        object result = command.ExecuteScalar();
                        if (result != null)
                        {
                            currentMemberID = Convert.ToInt32(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting member ID: " + ex.Message, "Database Error", 
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadTotalCalculations()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                   
                    string query = @"
                        SELECT 
                            'Total Cost' as Category,
                            ISNULL(SUM(Amount), 0) as Amount
                        FROM Cost
                        UNION ALL
                        SELECT 
                            'Total Meal' as Category,
                            ISNULL(SUM(NumberOfMeal), 0) as Amount
                        FROM Meal
                        UNION ALL
                        SELECT 
                            'Total Deposit' as Category,
                            ISNULL(SUM(Amount), 0) as Amount
                        FROM Deposite
                        UNION ALL
                        SELECT 
                            'Meal Rate' as Category,
                            CASE 
                                WHEN ISNULL(SUM(m.NumberOfMeal), 0) = 0 THEN 0
                                ELSE ISNULL(SUM(c.Amount), 0) / ISNULL(SUM(m.NumberOfMeal), 0)
                            END as Amount
                        FROM Cost c
                        CROSS JOIN (SELECT SUM(NumberOfMeal) as NumberOfMeal FROM Meal) m";

                    using (SqlCommand command = new SqlCommand(query, con))
                    {
                        con.Open();
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        
                       
                        dataGridView1.DataSource = dataTable;
                        
                      
                        if (dataGridView1.Columns.Count > 1)
                        {
                            dataGridView1.Columns[0].HeaderText = "Category";
                            dataGridView1.Columns[1].HeaderText = "Amount";
                            dataGridView1.Columns[1].DefaultCellStyle.Format = "N2";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading total calculations: " + ex.Message, "Database Error", 
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadIndividualCalculations()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    decimal totalMeal = 0;
                    string mealQuery = "SELECT ISNULL(SUM(NumberOfMeal), 0) FROM Meal WHERE MemberID = @MemberID";
                    using (SqlCommand cmd = new SqlCommand(mealQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@MemberID", currentMemberID);
                        totalMeal = Convert.ToDecimal(cmd.ExecuteScalar());
                    }

                   

                    decimal totalDeposit = 0;
                    string depositQuery = "SELECT ISNULL(SUM(Amount), 0) FROM Deposite WHERE MemberID = @MemberID";
                    using (SqlCommand cmd = new SqlCommand(depositQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@MemberID", currentMemberID);
                        totalDeposit = Convert.ToDecimal(cmd.ExecuteScalar());
                    }

                   


                    decimal totalCost = 0;
                    string costQuery = "SELECT ISNULL(SUM(Amount), 0) FROM Cost WHERE MemberID = @MemberID";
                    using (SqlCommand cmd = new SqlCommand(costQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@MemberID", currentMemberID);
                        totalCost = Convert.ToDecimal(cmd.ExecuteScalar());
                    }

                    


                    decimal mealRate = 0;
                    string mealRateQuery = @"
                        SELECT 
                            CASE 
                                WHEN ISNULL(SUM(m.NumberOfMeal), 0) = 0 THEN 0
                                ELSE ISNULL(SUM(c.Amount), 0) / ISNULL(SUM(m.NumberOfMeal), 0)
                            END
                        FROM Cost c
                        CROSS JOIN (SELECT SUM(NumberOfMeal) as NumberOfMeal FROM Meal) m";
                    using (SqlCommand cmd = new SqlCommand(mealRateQuery, con))
                    {
                        mealRate = Convert.ToDecimal(cmd.ExecuteScalar());
                    }

                   

                    decimal individualBalance = totalDeposit - (totalMeal * mealRate) - totalCost;

                   
                    UpdatePanelLabel("label8", totalMeal.ToString("0.00"));
                    UpdatePanelLabel("label9", totalDeposit.ToString("0.00"));
                    UpdatePanelLabel("label10", totalCost.ToString("0.00")); 
                    UpdatePanelLabel("label11", individualBalance.ToString("0.00")); 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading individual calculations: " + ex.Message, "Database Error", 
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdatePanelLabel(string labelName, string value)
        {
            try
            {
                
                Label label = this.Controls.Find(labelName, true).FirstOrDefault() as Label;
                if (label != null)
                {
                    label.Text = value;
                }
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Error updating label {labelName}: {ex.Message}");
            }
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

       
        public void RefreshCalculations()
        {
            LoadTotalCalculations();
            LoadIndividualCalculations();
        }

        private void CustomizeDesign()
        {
            HomeSubMenu.Visible = false;
        }
        
        private void hideSubMenu()
        {
            if (HomeSubMenu.Visible == true)
                HomeSubMenu.Visible = false;
        }

        private void showSubMenu(Panel subMenu)
        {
            if (subMenu.Visible == false)
            {
                hideSubMenu();
                subMenu.Visible = true;
            }
            else
                subMenu.Visible = false;
        }

        private Form activeForm = null;
        private void openChildForm(Form childForm)
        {
            if (activeForm != null)
                activeForm.Close();
            activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            panelChildForm.Controls.Add(childForm);
            panelChildForm.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
            
           
            childForm.FormClosed += (s, e) => RefreshCalculations();
        }

       
        private void panelChildForm_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnMedia_Click_1(object sender, EventArgs e)
        {
            showSubMenu(HomeSubMenu);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            hideSubMenu();

        }

        private void button8_Click(object sender, EventArgs e)
        {
            hideSubMenu();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            hideSubMenu();
            openChildForm(new DeleteMess());
        }

        private void panelSideMenu_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            openChildForm(new AddMember());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            openChildForm(new Deposite());

          // Deposite.FormClosed(object sender,EventArgs e)
           // {
           //    RefreshCalculations();
           // }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            openChildForm(new CreateMess());
        }

        private void button6_Click(object sender, EventArgs e)
        {
            openChildForm(new AllMembers());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            openChildForm(new AddCost());
        }

        private void button12_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 f1 = new Form1();
            f1.Show();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            openChildForm(new BazarSchedule());
        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            openChildForm(new ActiveMonthDetails());
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            openChildForm(new AddMeal());
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void a_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void backgroundWorker4_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void backgroundWorker5_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void backgroundWorker6_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void HomeSubMenu_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panelPlayer_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel10_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void panel9_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void panel8_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void panel7_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void Home_Load(object sender, EventArgs e)
        {
           
            if (!string.IsNullOrEmpty(currentMemberName))
            {
                RefreshCalculations();
            }
        }
    }
}