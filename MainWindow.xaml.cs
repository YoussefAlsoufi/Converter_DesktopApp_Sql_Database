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
        private readonly List<UnitCategoriesParams> CategoriesList = new();
        private readonly List<UnitParameters> UnitsList = new();
        private readonly MyConnection connection = new();
        private string fromCobCurrentValue = "";
        private string toCobCurrentValue = "";
        public MainWindow()
        {
            InitializeComponent();
            //Cob_Units.SelectedIndex = 0;
            Reader();
        }

        public void Reader()
        {
            //Category ComboBox:
            SqlConnection connection = MyConnection.GetConnection();
            connection.Open();
            SqlCommand Categories = new ("select * from dbo.CATEGORIES;", connection);
            SqlDataReader ComboCategories = Categories.ExecuteReader();

            while (ComboCategories.Read())
            {
                Cob_Units.Items.Add(ComboCategories["CATE_NAME"]);
                CategoriesList.Add(new UnitCategoriesParams()
                {
                    CateId = (int)ComboCategories["CATE_ID"],
                    CateName = ComboCategories["CATE_NAME"] as string
                });
            }
            connection.Close();

            //from Unit ComboBox:
            connection.Open();
            SqlCommand Units = new ("select * from dbo.UNITS;", connection);
            SqlDataReader ComboUnits = Units.ExecuteReader();

            while (ComboUnits.Read())
            {
                UnitsList.Add(new UnitParameters()
                {
                    UnitId = (int)ComboUnits["UNIT_ID"],
                    UnitName = ComboUnits["UNIT_NAME"] as string,
                    CateId = (int)ComboUnits["CATE_ID"],
                    Value = ComboUnits["VALUE"] as string
                });
            }

            connection.Close();

        }

        private string[] GetUnitByCategory(int cateId)
        {
            return UnitsList.Where(cate => cate.CateId == cateId).Select(UnitName => UnitName.UnitName).ToArray();
        }
        private string GetValue(string unitName)
        {
            return UnitsList.Where(value => value.UnitName == unitName).Select(value => value.Value).FirstOrDefault();
        }
        private void Cob_Units_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Cob_From.Items.Clear();
            Cob_To.Items.Clear();
            int cateId = CategoriesList[Cob_Units.SelectedIndex].CateId;
            foreach (string UnitName in GetUnitByCategory(cateId))
            {
                _ = Cob_From.Items.Add(UnitName);
                _ = Cob_To.Items.Add(UnitName);
            }
        }

        private void Add_Button_Click(object sender, RoutedEventArgs e) // the checking betweein Length input and Length in DataBase should be both Capital letters.
        {

            int newCateId = CategoriesList.Select(cateId => cateId.CateId).ToArray().Max();
            int newUnitId = UnitsList.Select(unitId => unitId.UnitId).ToArray().Max();
            if (Cob_Unit_Label.Text.Any() && Cob_To_Label.Text.Any() && Input_Value.Text.Any())
            {
                if (!CategoriesList.Select(cateName => cateName.CateName).ToArray().Contains(Cob_Unit_Label.Text.ToUpper()))
                {

                    connection.InsertCategory(newCateId, Cob_Unit_Label.Text.ToUpper());

                    if (!UnitsList.Select(cateName => cateName.UnitName).ToArray().Contains(Cob_To_Label.Text.ToUpper()))
                    {
                        int curruntCateId = Convert.ToInt32(CategoriesList.Where(cateName => cateName.CateName== Cob_Unit_Label.Text.ToUpper()).Select(cateId=>cateId.CateId).ToString());
                        connection.InsertUnit(newUnitId, Cob_Unit_Label.Text.ToUpper(), curruntCateId, Input_Value.Text);
                    }
                }
            }
            else
            {
                confirmationMessage.Content = "Fill in Category , Unit Name and Value, please!";
            }
        }
        private void Remove_Button_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Edit_Button_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string cateName = CategoriesList[Cob_Units.SelectedIndex].CateName;
            double inputValue;
            if (Input.Text.Any() && fromCobCurrentValue.Any() && toCobCurrentValue.Any() && double.TryParse(Input.Text, out _))
            {
                inputValue = Convert.ToDouble(Input.Text);
                if (inputValue >= 0)
                {
                    if (cateName is "Length" or "DataType")
                    {

                        if (fromCobCurrentValue != toCobCurrentValue)
                        {
                            double fromValue = Convert.ToDouble(GetValue(fromCobCurrentValue));
                            double toValue = Convert.ToDouble(GetValue(toCobCurrentValue));
                            results.Content = (inputValue * fromValue / toValue).ToString();
                        }
                        else
                        {
                            results.Content = "You have chosen the same Units, Correct it please!";
                        }

                    }
                    else if (cateName == "Temperature")
                    {
                        results.Content = fromCobCurrentValue == "CELSIU" && toCobCurrentValue == "FAHRENHEIT"
                            ? ((inputValue * 1.8) + 32).ToString()
                            : fromCobCurrentValue == "FAHRENHEIT" && toCobCurrentValue == "CELSIU"
                                ? ((inputValue - 32) / 1.8).ToString()
                                : "You have chosen the same Units, Correct it please!";
                    }

                }
                else
                {
                    results.Content = "Enter a positive number, please!";
                    
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

    }


}
