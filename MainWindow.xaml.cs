using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;


namespace Converter_DesktopApp_Sql_Database
{
    public partial class MainWindow : Window
    {
        List<UnitCategoriesParams> CategoriesList = new();
        List<UnitParameters> UnitsList = new();
        

        public NameValueCollection lengthSection, dataTypeSection, tempretureSection;
        string currentCobValue = "";
        string fromCobCurrentValue = "";
        string toCobCurrentValue = "";
        string currentValue = "";
        public MainWindow()
        {
            InitializeComponent();
            Cob_Units.SelectedIndex = 0;
            Reader();

        }

        public void Reader()
        {
            SqlConnection connection = MyConnection.TestGetConnection();
            connection.Open();
            SqlCommand Categories = new ("select * from dbo.tb_UnitCategories; ", connection);
            SqlDataReader ComboCategories = Categories.ExecuteReader();

            while (ComboCategories.Read())
            {
                ComboTest.Items.Add(ComboCategories["Cate_Name"]);
                CategoriesList.Add(new UnitCategoriesParams()
                {
                    CateId = ComboCategories["Cate_Id"] as string,
                    CateName = ComboCategories["Cate_Name"] as string
                });
            }
            connection.Close();


            connection.Open();
            SqlCommand Units = new ("select * from dbo.tb_Units;", connection);
            SqlDataReader ComboUnits = Units.ExecuteReader();

            while (ComboUnits.Read())
            {
                UnitsList.Add(new UnitParameters()
                {
                    UnitId = (int)ComboUnits["Unit_Id"],
                    UnitName = ComboUnits["Unit_Name"] as string ,
                    CateId = ComboUnits["Cate_Id"] as string
                });
            }

            connection.Close();



            /*
            SqlConnection connection = MyConnection.GetConnection();
            connection.Open();
            SqlCommand command = new ("select * from [dbo].[Length]", connection);
            command.CommandType = CommandType.Text;

            SqlDataAdapter adapter = new (command);
            _ = adapter.Fill(dt);

            DataRow newRow = dt.NewRow();
            newRow["LengthId"] = 0;
            newRow["Unit_Name"] = "meter";
            dt.Rows.InsertAt(newRow, 0);
            connection.Close();
            var test = dt.Rows;
            var test1 = dt.Columns;
            var tes = dt.PrimaryKey;
            Console.WriteLine(dt.Rows.Count);
            */


        }
        private string[] GetUnitByCategory (string cateId)
        {
            return UnitsList.Where(cate => cate.CateId == cateId).Select(UnitName => UnitName.UnitName).ToArray();
        }
        private void Cob_Units_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            currentValue = (e.AddedItems[0] as ComboBoxItem).Content as string;
            if (currentCobValue != currentValue)
            {
                currentCobValue = currentValue;
                UpdateValues();

            }
            
            

        }
        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Remove_Button_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Edit_Button_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var inputValue = 0.0;
            if (Input.Text.Any() && fromCobCurrentValue.Any() && toCobCurrentValue.Any() && currentValue.Any() && double.TryParse(Input.Text, out double n))
            {
                var fromValue = 0.0;
                var toValue = 0.0;
                inputValue = Convert.ToDouble(Input.Text);
                if (currentValue == "Length" || currentValue == "Data")
                {
                    if (inputValue >= 0)
                    {
                        if (currentValue == "Length")
                        {
                            if (fromCobCurrentValue != toCobCurrentValue)
                            {
                                fromValue = Convert.ToDouble(lengthSection[fromCobCurrentValue]);
                                toValue = Convert.ToDouble(lengthSection[toCobCurrentValue]);
                                results.Content = (inputValue * fromValue / toValue).ToString();
                                fromCobCurrentValue = toCobCurrentValue = "";
                            }
                            else
                            {
                                results.Content = "You have chosen the same Units, Correct it please!";
                            }

                        }
                        else if (currentValue == "Data")
                        {
                            if (fromCobCurrentValue != toCobCurrentValue)
                            {
                                fromValue = Convert.ToDouble(dataTypeSection[fromCobCurrentValue]);
                                toValue = Convert.ToDouble(dataTypeSection[toCobCurrentValue]);
                                results.Content = (inputValue * fromValue / toValue).ToString();
                                fromCobCurrentValue = toCobCurrentValue = "";
                            }
                            else
                            {
                                results.Content = "You have chosen the same Units, Correct it please!";
                            }
                        }

                    }
                    else
                    {
                        results.Content = "Enter a positive number, please!";
                    }
                }


                else if (currentValue == "Tempreture")
                {
                    if (fromCobCurrentValue == "celsiu" && toCobCurrentValue == "fahrenheit")
                    {
                        results.Content = ((inputValue * 1.8) + 32).ToString();
                        fromCobCurrentValue = toCobCurrentValue = "";
                    }
                    else if (fromCobCurrentValue == "fahrenheit" && toCobCurrentValue == "celsiu")
                    {
                        results.Content = ((inputValue - 32) / 1.8).ToString();
                        fromCobCurrentValue = toCobCurrentValue = "";
                    }
                    else
                    {
                        results.Content = "You have chosen the same Units, Correct it please!";
                    }
                }
            }
            else
            {
                results.Content = "Choose your Units and add a correct Number, please!";
            }




        }

        private void Cob_To_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Cob_To.SelectedItem != null)
            {
                toCobCurrentValue = Cob_To.SelectedItem.ToString();
            }

        }
        private void Cob_From_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Cob_From.SelectedItem != null)
            {
                fromCobCurrentValue = Cob_From.SelectedItem.ToString();
            }

        }
        private void ComboBoxTest(object sender, SelectionChangedEventArgs e)
        {
            ComboTest2.Items.Clear();
            string cateId = CategoriesList[ComboTest.SelectedIndex].CateId;
            foreach (string UnitName in GetUnitByCategory(cateId))
            {
                ComboTest2.Items.Add(UnitName);
            }

        }
        private void ComboBoxTest2(object sender, SelectionChangedEventArgs e)
        {

        }
        public void UpdateValues()
        {
            switch (currentCobValue)
            {
                case "Length":
                    {
                        Cob_From.Items.Clear();
                        Cob_To.Items.Clear();
                        lengthSection = (NameValueCollection)ConfigurationManager.GetSection("Length");
                        foreach (string s in lengthSection.AllKeys)
                        {
                            Cob_From.Items.Add(s);
                            Cob_To.Items.Add(s);
                        }
                        break;
                    }
                case "Data":
                    {
                        Cob_From.Items.Clear();
                        Cob_To.Items.Clear();
                        dataTypeSection = (NameValueCollection)ConfigurationManager.GetSection("Data");
                        foreach (string s in dataTypeSection.AllKeys)
                        {
                            Cob_From.Items.Add(s);
                            Cob_To.Items.Add(s);
                        }
                        break;
                    }
                case "Tempreture":
                    {
                        Cob_From.Items.Clear();
                        Cob_To.Items.Clear();
                        tempretureSection = (NameValueCollection)ConfigurationManager.GetSection("Tempreture");
                        foreach (string s in tempretureSection.AllKeys)
                        {
                            Cob_From.Items.Add(s);
                            Cob_To.Items.Add(s);
                        }
                        break;
                    }

            }
        }


    }
}
