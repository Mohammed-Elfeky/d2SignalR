using Microsoft.AspNetCore.SignalR.Client;
using System.Data;
using System.Net.Http.Headers;
namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        HubConnection con;
        async Task<List<Employee>> getData()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await client.GetAsync("http://localhost:48571/Employee/IndexJson");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<List<Employee>>();
            }
            return null;
        }
        private async void Form1_Load(object sender, EventArgs e)
        {
            List<Employee> emps = await getData();
            dataGridView1.DataSource = emps;

            con = new HubConnectionBuilder().WithUrl("http://localhost:48571/empHup").Build();
            await con.StartAsync();

            con.On<Employee>("whenAdd", (emp) => {
                updateGrid(emp);
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            con.InvokeAsync("addEmp", new Employee()
            {
                Name = textBox1.Text,
                age = int.Parse(textBox2.Text),
                address = textBox3.Text
            });
        }
        void updateGrid(Employee emp)
        {
            List<Employee> dt = (List<Employee>)dataGridView1.DataSource;
            dt.Add(emp);

            dataGridView1.DataSource = null;
            dataGridView1.DataSource = dt;
            dataGridView1.Update();
            dataGridView1.Refresh();
        }
    }
}