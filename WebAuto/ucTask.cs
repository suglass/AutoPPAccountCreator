using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using ResourcesInApp;
using DBHelper.DbBase;
using DbHelper;
using DbHelper.DbBase;

namespace WebAuto
{
    public partial class ucTask : UserControl
    {
        public ucTask()
        {
            InitializeComponent();
            picRefresh.SetFocusBackgroundColor(Color.FromArgb(64, 64, 64));
            picEdit.SetFocusBackgroundColor(Color.FromArgb(64, 64, 64));
            picDel.SetFocusBackgroundColor(Color.FromArgb(64, 64, 64));
        }
        private void ucTask_Load(object sender, EventArgs e)
        {
            refresh_todo_listview();
        }
        public void update_log(string log)
        {
            this.InvokeOnUiThreadIfRequired(() =>
            {
                rtbLog.Text += "\n" + log;
                rtbLog.SelectionStart = rtbLog.Text.Length;
                rtbLog.ScrollToCaret();
                refresh_todo_listview();
            });
        }
        public void refresh_todo_listview()
        {
            lvTasks.Clear();

            lvTasks.Columns.Add("No");
            lvTasks.Columns.Add("Type");
            lvTasks.Columns.Add("Info");
            lvTasks.Columns.Add("Event Time");

            DataTable dt = MainApp.g_db.load_table(ConstEnv.to_do_table_name);
            if (dt == null || dt.Rows.Count == 0)
                return;
            List<ToDo> todo_list = TableSerialize<ToDo>.dt_2_list(dt);
            if (todo_list == null || todo_list.Count == 0)
                return;
            foreach (ToDo todo in todo_list)
            {
                ListViewItem lv1 = new ListViewItem((lvTasks.Items.Count + 1).ToString());
                lv1.SubItems.Add(ConstEnv.ToDo_Type[todo.type]);
                lv1.SubItems.Add(get_todo_object_simple_info(todo.object_id, todo.type));
                lv1.SubItems.Add(todo.create_time);

                lv1.Tag = todo.id;

                lvTasks.Items.Add(lv1);

            }
            lvTasks.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }
        private string get_todo_object_simple_info(int id, int type)
        {
            string info = "";

            if (type == ConstEnv.TODO_TYPE_PROXY_EXPIRED)
            {
                string name, expired_date;
                MainApp.g_db.get_expired_proxy_simple_info(id, out name, out expired_date);
                info = $"Name : {name}, Expired Date : {expired_date}";
            }
            else if (type == ConstEnv.TODO_TYPE_MAIL_BLOCKED)
            {
                PaypalAccount account = MainApp.g_db.get_account_by_id(id);
                if (account != null)
                    info = $"Mail : {account.mail}, Balance : {account.money_balance}";
            }
            else if (type == ConstEnv.TODO_TYPE_CHARGE_LEVEL_1)
            {
                info = "-";
            }
            else if (type == ConstEnv.TODO_TYPE_CHARGE_LEVEL_2)
            {
                info = "-";
            }

            return info;
        }
        private void btnCreateAccount_Click(object sender, EventArgs e)
        {
            frmCreateAccount dlg = new frmCreateAccount();
            dlg.ShowDialog();
        }

        private void btnLevel1_Click(object sender, EventArgs e)
        {
            frmLevel1 dlg = new frmLevel1();
            dlg.ShowDialog();
        }
        private void btnLevel2_Click(object sender, EventArgs e)
        {
        }

        private void btnSendMoney_Click(object sender, EventArgs e)
        {
            frmSendMoney dlg = new frmSendMoney();
            dlg.ShowDialog();
        }

        private void picRefresh_Click(object sender, EventArgs e)
        {
            refresh_todo_listview();
        }

        private void picEdit_Click(object sender, EventArgs e)
        {
            if (lvTasks.SelectedItems.Count != 1)
            {
                MessageBox.Show("Please select one item to edit.");
                return;
            }
            int type = int.Parse(lvTasks.Items[0].SubItems[0].Text);
            if (type == ConstEnv.TODO_TYPE_MAIL_BLOCKED)
            {
                MessageBox.Show("You can not edit the item of this type.");
                return;
            }
            if (type == ConstEnv.TODO_TYPE_CHARGE_LEVEL_1 || type == ConstEnv.TODO_TYPE_CHARGE_LEVEL_2)
            {
                MessageBox.Show("Sorry, This type is not supported now.");
                return;
            }
            frmEditToDo dlg = new frmEditToDo();
            dlg.id = (int)lvTasks.SelectedItems[0].Tag;
            dlg.ShowDialog();
            refresh_todo_listview();
        }

        private void picDel_Click(object sender, EventArgs e)
        {
            int sel_num = lvTasks.SelectedItems.Count;

            if (sel_num == 0)
            {
                MessageBox.Show("Please select the items to remove.");
                return;
            }
            try
            {
                if (MessageBox.Show($"Are you sure fixed {sel_num} To-Do tasks?\nIf you click YES, these will be removed from To-Do list.", "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    return;
                for (int i = sel_num - 1; i >= 0; i--)
                {
                    int idx = lvTasks.SelectedItems[i].Index;
                    int id = (int)lvTasks.SelectedItems[i].Tag;

                    MainApp.g_db.delete_todo_task(id);

                    lvTasks.Items.RemoveAt(idx);
                }
                for (int i = 0; i < lvTasks.Items.Count; i++)
                    lvTasks.Items[i].SubItems[0].Text = (i + 1).ToString();

                MessageBox.Show($"Delete {sel_num} todo task successfully.");
            }
            catch (Exception exception)
            {
                MainApp.log_error($"Exception Error ({System.Reflection.MethodBase.GetCurrentMethod().Name}): {exception.Message}");
                MessageBox.Show($"Failed to Delete todo task(s). {exception.Message}");
            }
        }

        private void btnSimulator_Click(object sender, EventArgs e)
        {
            frmTransSimul dlg = new frmTransSimul();
            dlg.ShowDialog();
        }
    }
}
