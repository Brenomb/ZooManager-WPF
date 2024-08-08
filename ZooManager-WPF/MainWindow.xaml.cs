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
            ShowAssociatedAnimals();

        }

        private void ValidateEntry()
        {
            if (txtAddUpdate.Text == "")
            {
                MessageBox.Show("Please enter a value");
            }
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

        private void ShowAssociatedAnimals()
        {
            try { 
                // Create a SQL query to select all columns from the "Animal" table and join it with the "ZooAnimal" table based on the animal ID and zoo ID
                string query = "SELECT * FROM Animal a INNER JOIN ZooAnimal za on a.Id = za.AnimalID where za.ZooID = @ZooID";
                // Create a new instance of the SqlCommand class with the query and the SqlConnection object
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                // Create a new instance of the SqlDataAdapter class with the SqlCommand object
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {
                    sqlCommand.Parameters.AddWithValue("@ZooID", listZoos.SelectedValue);

                    DataTable animalTable = new DataTable();
                    sqlDataAdapter.Fill(animalTable);

                    //Which information we want to see
                    listAssociatedAnimals.DisplayMemberPath = "Name";
                    //Which information we want to use to find it
                    listAssociatedAnimals.SelectedValuePath = "Id";
                    //Displaying data
                    listAssociatedAnimals.ItemsSource = animalTable.DefaultView;
                }
            }catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }

        }

        private void listZoos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowAssociatedAnimals();
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
                listAnimals.DisplayMemberPath = "Name";
                //Which information we want to use to find it
                listAnimals.SelectedValuePath = "Id";
                //Displaying data
                listAnimals.ItemsSource = animalTable.DefaultView;
            }
        }

        private void OnDeleteZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(listZoos.SelectedValue == null)
                    MessageBox.Show("Please select a zoo to delete");

                string query = "DELETE FROM Zoo WHERE Id = @ZooID";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ZooID", listZoos.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowZoos();
            }

        }
        private void OnRemoveAnimal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (listAssociatedAnimals.SelectedValue == null)
                    MessageBox.Show("Please select an animal to remove");

                string query = "DELETE FROM ZooAnimal WHERE AnimalID = @AnimalID AND ZooID = @ZooID";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@AnimalID", listAssociatedAnimals.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@ZooID", listZoos.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowAssociatedAnimals();
            }

        }
        private void OnAddZoo_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAddUpdate.Text))
            {
                try
                {
                    string query = "INSERT INTO Zoo VALUES (@Location)";
                    SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                    sqlConnection.Open();
                    sqlCommand.Parameters.AddWithValue("@Location", txtAddUpdate.Text);
                    sqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    sqlConnection.Close();
                    txtAddUpdate.Clear();
                    ShowZoos();
                }
            }else
            {
                MessageBox.Show("Please enter a value");
            }

        }
        private void OnAddAnimal_Click(object sender, RoutedEventArgs e)
        {
            try 
            {
                ValidateEntry();
                string query = "INSERT INTO Animal VALUES (@Name)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Name", txtAddUpdate.Text);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                txtAddUpdate.Clear();
                ShowAnimals();
            }

        }
        private void OnUpdateZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ValidateEntry();
                string query = "UPDATE Zoo SET Location = @Location WHERE Id = @ZooID";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Location", txtAddUpdate.Text);
                sqlCommand.Parameters.AddWithValue("@ZooID", listZoos.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                txtAddUpdate.Clear();
                ShowZoos();
            }

        }
        private void OnUpdateAnimal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ValidateEntry();
                string query = "UPDATE Animal SET Name = @Name WHERE Id = @AnimalID";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Name", txtAddUpdate.Text);
                sqlCommand.Parameters.AddWithValue("@AnimalID", listAnimals.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                txtAddUpdate.Clear();
                ShowAnimals();
            }

        }
        private void OnDeleteAnimal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (listAnimals.SelectedValue == null)
                    MessageBox.Show("Please select an animal to delete");

                string query = "DELETE FROM Animal WHERE Id = @AnimalID";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@AnimalID", listAnimals.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowAnimals();
            }

        }
        private void OnAddAnimalToZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (listZoos.SelectedValue == null || listAnimals.SelectedValue == null)
                    MessageBox.Show("Please select a zoo and an animal to add");

                string query = "INSERT INTO ZooAnimal VALUES (@ZooID, @AnimalID)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ZooID", listZoos.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@AnimalID", listAnimals.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowAssociatedAnimals();
            }
        }
    }
}
