using Attendance.Common;
using Attendance.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AttClient
{
    public partial class Form1 : Form
    {
        List<Person> list;
        public Form1()
        {
            InitializeComponent();
        }

        private void button_Open_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Excel文件(*.xls;*.xlsx)|*.xls;*.xlsx|所有文件|*.*";
            ofd.ValidateNames = true;
            ofd.CheckPathExists = true;
            ofd.CheckFileExists = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string strFileName = ofd.FileName;
                AttBiz attBiz = new AttBiz();
                list = attBiz.StcAttFromLoaclExcel(strFileName, StartDate.Value.ToString("yyyy-MM-dd"), EndDate.Value.ToString("yyyy-MM-dd"));
                int n = list.Count;
                listView.Items.Clear();
                for (int i = 0; i < n; i++)
                {
                    ListViewItem lt = new ListViewItem();
                    lt.Text = list[i].LASTNAME;
                    lt.SubItems.Add(list[i].MOBILE);
                    lt.SubItems.Add(list[i].Department);
                    lt.SubItems.Add(list[i].WorkDay.ToString());
                    lt.SubItems.Add(list[i].AttDay.ToString());
                    lt.SubItems.Add(list[i].LateCount.ToString());
                    lt.SubItems.Add(list[i].EarlyCount.ToString());
                    lt.SubItems.Add(list[i].Trip.ToString());
                    lt.SubItems.Add(list[i].Leave0.ToString());
                    lt.SubItems.Add(list[i].Leave1.ToString());
                    lt.SubItems.Add(list[i].Leave2.ToString());
                    lt.SubItems.Add(list[i].Leave3.ToString());
                    lt.SubItems.Add(list[i].Leave4.ToString());
                    lt.SubItems.Add(list[i].Leave5.ToString());
                    lt.SubItems.Add(list[i].Leave6.ToString());
                    lt.SubItems.Add(list[i].Leave7.ToString());
                    listView.Items.Add(lt);
                }
            }
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog ofd = new SaveFileDialog();
            ofd.Filter = "csv文件(*.csv)|*.csv|所有文件|*.*";
            ofd.ValidateNames = true;
            ofd.CheckPathExists = true;
            //ofd.CheckFileExists = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string path = ofd.FileName;
                if (File.Exists(path))
                {
                    try
                    {
                        File.Delete(path);
                    }
                    finally { };
                }

                System.IO.FileStream fs = new System.IO.FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter m_streamWriter = new StreamWriter(fs, System.Text.Encoding.Default);
                m_streamWriter.WriteLine("姓名,电话,部门,应勤,实勤,迟到,早退,出差,事假,病假,婚假,产假,丧假,年休假,其他,陪产假");
                for (int i = 0; i < listView.Items.Count; i++)
                {
                    string line = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}",
                        listView.Items[i].Text,
                        listView.Items[i].SubItems[1].Text,
                        listView.Items[i].SubItems[2].Text,
                        listView.Items[i].SubItems[3].Text,
                        listView.Items[i].SubItems[4].Text,
                        listView.Items[i].SubItems[5].Text,
                        listView.Items[i].SubItems[6].Text,
                        listView.Items[i].SubItems[7].Text,
                        listView.Items[i].SubItems[8].Text,
                        listView.Items[i].SubItems[9].Text,
                        listView.Items[i].SubItems[10].Text,
                        listView.Items[i].SubItems[11].Text,
                        listView.Items[i].SubItems[12].Text,
                        listView.Items[i].SubItems[13].Text,
                        listView.Items[i].SubItems[14].Text,
                        listView.Items[i].SubItems[15].Text);
                    m_streamWriter.WriteLine(line);
                }
                m_streamWriter.Flush();
                m_streamWriter.Close();
                MessageBox.Show("保存完成");

            }
        }
    }
}
