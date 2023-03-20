using DBHelper.DbBase;
using ResourcesInApp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebAuto
{
    public partial class frmEditToDo : Form
    {
        public int id = 0;
        private ToDo m_todo;
        private DataTable m_object_dt;
        public frmEditToDo()
        {
            InitializeComponent();
        }

        private void frmEditDataRow_Load(object sender, EventArgs e)
        {
            lvData.Columns.Add("Value");
            lvData.Columns.Add("Field");

            lvData.Columns[0].DisplayIndex = 1;
            lvData.Columns[1].DisplayIndex = 0;

            lvData.Columns[0].Width = lvData.Width / 2 - 5;
            lvData.Columns[1].Width = lvData.Width / 2 - 5;

            DataTable dt = MainApp.g_db.select($"SELECT * FROM {ConstEnv.to_do_table_name} WHERE id = {id} ;");
            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show("No data for this id");
                this.Close();
                return;
            }
            m_todo = TableSerialize<ToDo>.from_dt(dt);
            if (m_todo == null)
            {
                this.Close();
                return;
            }

            if (m_todo.type == ConstEnv.TODO_TYPE_PROXY_EXPIRED)
            {
                m_object_dt = MainApp.g_db.select($"SELECT * FROM {ConstEnv.proxy_group_table_name} WHERE id = '{m_todo.object_id}' ;");
                if (m_object_dt == null || m_object_dt.Rows.Count == 0)
                {
                    MessageBox.Show("No object data for this id");
                    this.Close();
                    return;
                }
                DataRow row = m_object_dt.Rows[0];
                foreach (DataColumn col in m_object_dt.Columns)
                {
                    if (col.ColumnName == "id")
                        continue;

                    ListViewItem lv1 = new ListViewItem(row[col.ColumnName].ToString());
                    lv1.SubItems.Add(col.ColumnName);
                    lv1.Tag = false;

                    lvData.Items.Add(lv1);
                }

                lblTitle.Text = $"{ConstEnv.ToDo_Type[ConstEnv.TODO_TYPE_PROXY_EXPIRED]} : {row["name"].ToString()}";
            }
            else
            {
                MessageBox.Show($"Unknown ToDo Task type. {m_todo.type}");
                this.Close();
                return;
            }

        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            List<int> changed_idx_list = new List<int>();

            for (int i = 0; i < lvData.Items.Count; i++)
            {
                if ((bool)lvData.Items[i].Tag)
                    changed_idx_list.Add(i);
            }
            if (changed_idx_list.Count == 0)
                return;

            string query = "";
            if (m_todo.type == ConstEnv.TODO_TYPE_PROXY_EXPIRED)
            {
                query = $"UPDATE {ConstEnv.proxy_group_table_name} SET";
            }
            else
            {

            }

            for (int i = 0; i < changed_idx_list.Count; i++)
            {
                if (i != 0)
                    query += ", ";

                query += $" {lvData.Items[changed_idx_list[i]].SubItems[1].Text} = '{lvData.Items[changed_idx_list[i]].SubItems[0].Text}'";
            }
            query += $" WHERE id = '{m_todo.object_id}' ;";

            try
            {
                MainApp.g_db.update(query);
            }
            catch (Exception excetion)
            {
                MessageBox.Show($"Exception Error : query:\n{query}\nMessage:\n{excetion.Message}");
                return;
            }

            for (int i = 0; i < changed_idx_list.Count; i++)
            {
                lvData.Items[changed_idx_list[i]].Tag = false;
                lvData.Items[changed_idx_list[i]].ForeColor = Color.Black;
                lvData.Items[changed_idx_list[i]].BackColor = Color.White;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            List<int> changed_idx_list = new List<int>();

            for (int i = 0; i < lvData.Items.Count; i++)
            {
                if ((bool)lvData.Items[i].Tag)
                    changed_idx_list.Add(i);
            }
            if (changed_idx_list.Count > 0)
            {
                if (MessageBox.Show("Do you want close without saving changes?", "Information", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    return;
            }

            this.Close();
        }

        private void lvData_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (e.Label == null)
                return;
            string x = m_object_dt.Rows[0][lvData.Items[e.Item].SubItems[1].Text].ToString();
            if (e.Label != x)
            {
                lvData.Items[e.Item].Tag = true;

                lvData.Items[e.Item].ForeColor = Color.Red;
                lvData.Items[e.Item].BackColor = Color.LightYellow;
            }
            else
            {
                lvData.Items[e.Item].Tag = false;

                lvData.Items[e.Item].ForeColor = Color.Black;
                lvData.Items[e.Item].BackColor = Color.White;
            }
        }
    }
}
