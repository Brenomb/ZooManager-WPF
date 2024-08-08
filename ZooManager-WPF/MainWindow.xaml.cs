using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace ZooManager_WPF
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection sqlConnection;
        public MainWindow()
        {
            InitializeComponent();

            //CsharpTestDBConnectionString
            string connectionString = ConfigurationManager.ConnectionStrings["ZooManager-WPF.Properties.Settings.CsharpTestDBConnectionString"].ConnectionString;
            sqlConnection = new SqlConnection(connectionString);

            ShowZoos();
            ShowAnimals();

        }

        private void ShowZoos()
        {
            try 
            {
                string query = "SELECT * FROM Zoo";
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable zooTable = new DataTable();
                    sqlDataAdapter.Fill(zooTable);

                    //Which information we want to see
                    listZoos.DisplayMemberPath = "Location";
                    //Which information we want to use to find it
                    listZoos.SelectedValuePath = "Id";
                    //Displaying data
                    listZoos.ItemsSource = zooTable.DefaultView;
                }
            }catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }
           
        }

        private void ShowAnimals()
        {
            string query = "SELECT * FROM Animal";
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

            using (sqlDataAdapter)
            {
                DataTable animalTable = new DataTable();
                sqlDataAdapter.Fill(animalTable);

                //Which information we want to see
                listAssociatedAnimals.DisplayMemberPath = "Name";
                //Which information we want to use to find it
                listAssociatedAnimals.SelectedValuePath = "Id";
                //Displaying data
                listAssociatedAnimals.ItemsSource = animalTable.DefaultView;
            }

        }
    }
}
