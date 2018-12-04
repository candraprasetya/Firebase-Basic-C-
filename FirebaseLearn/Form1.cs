using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;

namespace FirebaseLearn
{
    public partial class Form1 : Form
    {
        DataTable dataTable = new DataTable();

        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "c9oua6rAONSy6QygoBcEKzJJwrc3RQbLpgwELZDt",
            BasePath = "https://fir-loginregister-a9758.firebaseio.com/"
        };

        IFirebaseClient client;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            buttonDelete.Enabled = false;
            buttonUpdate.Enabled = false;

            client = new FireSharp.FirebaseClient(config);

            if(client != null)
            {
                MessageBox.Show("Connection is established");
            }

            dataTable.Columns.Add("ID");
            dataTable.Columns.Add("NAME");
            dataTable.Columns.Add("ADDRESS");
            dataTable.Columns.Add("AGE");

            dataGridView.DataSource = dataTable;
            
        }

        private void clearTextBox()
        {
            textBoxID.Clear();
            textBoxName.Clear();
            textBoxAddress.Clear();
            textBoxAge.Clear();
        }

        private async void button_insert_Click(object sender, EventArgs e)
        {
            FirebaseResponse responseDatabase = await client.GetAsync("Counter/node");
            CounterClass getCount = responseDatabase.ResultAs<CounterClass>();

            //MessageBox.Show(getCount.Count);

            var data = new Data
            {
                //Id = textBoxID.Text,

                Id = (Convert.ToInt32(getCount.Count)+1).ToString(),
                Name = textBoxName.Text,
                Address = textBoxAddress.Text,
                Age = textBoxAge.Text
            };

            SetResponse response = await client.SetAsync("Datanya/" + data.Id, data);
            Data result = response.ResultAs<Data>();

            MessageBox.Show("Data Inserted " + result.Id);

            var objectCount = new CounterClass
            {
                Count = data.Id
            };

            SetResponse responseCounter = await client.SetAsync("Counter/node", objectCount);

            clearTextBox();
            
        }

        private async void buttonRetrieve_Click(object sender, EventArgs e)
        {
            FirebaseResponse response = await client.GetAsync("Datanya/" + textBoxID.Text);
            Data dataObject = response.ResultAs<Data>();

            textBoxID.Text = dataObject.Id;
            textBoxName.Text = dataObject.Name;
            textBoxAddress.Text = dataObject.Address;
            textBoxAge.Text = dataObject.Age;

            MessageBox.Show("Data Retrieved Successfully");

            button_insert.Enabled = false;
            buttonUpdate.Enabled = true;
            buttonDelete.Enabled = true;
        }

        private async void buttonUpdate_Click(object sender, EventArgs e)
        {
            var data = new Data
            {
                Id = textBoxID.Text,
                Name = textBoxName.Text,
                Address = textBoxAddress.Text,
                Age = textBoxAge.Text
            };

            FirebaseResponse response = await client.UpdateAsync("Datanya/" + textBoxID.Text, data);
            Data result = response.ResultAs<Data>();

            MessageBox.Show("Data Updated at ID : " + result.Id);

            clearTextBox();

            buttonUpdate.Enabled = false;
            buttonDelete.Enabled = false;
            button_insert.Enabled = true;
        }

        private async void buttonDelete_Click(object sender, EventArgs e)
        {
            FirebaseResponse response = await client.DeleteAsync("Datanya/" + textBoxID.Text);
            MessageBox.Show("Deleted Record of ID : " + textBoxID.Text);

            buttonUpdate.Enabled = false;
            buttonDelete.Enabled = false;
            button_insert.Enabled = true;
        }

        private async void buttonClear_Click(object sender, EventArgs e)
        {
            FirebaseResponse response = await client.DeleteAsync("Datanya");
            MessageBox.Show("All Data Deleted / Datanya");
        }

        private async void exportData()
        {

            dataTable.Rows.Clear();
            int i = 0;
            FirebaseResponse exportResponse = await client.GetAsync("Counter/node");
            CounterClass objectData = exportResponse.ResultAs<CounterClass>();
            int count = Convert.ToInt32(objectData.Count);

            while (true)
            {
                if (i == count)
                {
                    break;
                }

                i++;

                try
                {

                    FirebaseResponse exportResponseData = await client.GetAsync("Datanya/" + i);
                    Data getDataToExport = exportResponseData.ResultAs<Data>();

                    DataRow row = dataTable.NewRow();
                    row["Id"] = getDataToExport.Id;
                    row["Name"] = getDataToExport.Name;
                    row["Address"] = getDataToExport.Address;
                    row["Age"] = getDataToExport.Age;

                    dataTable.Rows.Add(row);

                }

                catch
                {

                }

            }
            MessageBox.Show("Done!!");
        }

        private void buttonDataToGridView_Click(object sender, EventArgs e)
        {
            exportData();
        }
    }
}
