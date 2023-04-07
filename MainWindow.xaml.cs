using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using DevExpress.Utils.CommonDialogs.Internal;

namespace Converter_DesktopApp_Sql_Database
{
    public partial class MainWindow : Window
    {
        private readonly List<UnitCategoriesParams> CategoriesList = new();
        private (string UnitName, int CateId, string Value)[] RelatedUnitsList;
        private readonly List<UnitParameters> UnitsList = new();
        private readonly MyConnection connection = new();
        private string fromCobCurrentValue = "";
        private string toCobCurrentValue = "";
        private string ActionName = "";
        private string LastCateName = "";
        public MainWindow()
        {
            InitializeComponent();
            Reader();
        }

        public void Reader()
        {
            //Category ComboBox:
            CategoriesList.Clear();
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
            // Start the App with --Select a Unit Category -- :
            Cob_Units.SelectedIndex = CategoriesList.Select(i => i.CateId).FirstOrDefault() - 1;
            //from Unit ComboBox:
            UnitsList.Clear();
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
        private string GetCategoryName(string unitNameInput)
        {
            return CategoriesList[UnitsList.Where(unitName => unitName.UnitName == unitNameInput).Select(cateId => cateId.CateId).FirstOrDefault()].CateName;
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
            int cateId= Cob_Units.SelectedIndex == -1 ? 1 : CategoriesList[Cob_Units.SelectedIndex].CateId; 
            foreach (string UnitName in GetUnitByCategory(cateId))
            {
                _ = Cob_From.Items.Add(UnitName);
                _ = Cob_To.Items.Add(UnitName);
            }
        }
        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Cob_Unit_Label.Text.Any() && Cob_To_Label.Text.Any() && Input_Value.Text.Any())
            {
                AddAction(Cob_Unit_Label.Text.ToUpper());
            }
            else
            {
                confirmationMessage.Content = "Fill in all fields, please!";
                _ = MessageBox.Show("Fill All fields 'Category' , 'Unit Name' and 'Value', please!", "Converter");
            }
        }
        private void AddAction(string CurrantCateName)
        {
            int newCateId = CategoriesList.Select(cateId => cateId.CateId).ToArray().Max();
            int newUnitId = UnitsList.Select(unitId => unitId.UnitId).ToArray().Max();

            if (!CategoriesList.Select(cateName => cateName.CateName).ToArray().Contains(CurrantCateName))
            {
                if (!UnitsList.Select(cateName => cateName.UnitName).ToArray().Contains(Cob_To_Label.Text.ToUpper()))
                {
                    connection.InsertCategory(newCateId, CurrantCateName);
                    connection.InsertUnit(newUnitId, Cob_To_Label.Text.ToUpper(), newCateId + 1, Input_Value.Text);
                    Cob_Units.Items.Clear();
                    Reader();
                    confirmationMessage.Content = "Adding to DataBase is Done!";
                    _ = MessageBox.Show($"New Categort '{CurrantCateName}' and New Unit '{Cob_To_Label.Text.ToUpper()}' have been added to the Database successfully", "Converter");
                    Cob_Unit_Label.Text = Cob_To_Label.Text = Input_Value.Text = "";
                    confirmationMessage.Content = "";
                }
                else
                {
                    confirmationMessage.Content = " Please, Check again!";
                    _ = MessageBox.Show($"'{Cob_To_Label.Text.ToUpper()}' Unit is existed under '{GetCategoryName(Cob_To_Label.Text.ToUpper())}' Category", "Converter");
                }
            }
            else
            {
                if (!UnitsList.Select(cateName => cateName.UnitName).ToArray().Contains(Cob_To_Label.Text.ToUpper()))
                {
                    int curruntCateId = CategoriesList.Where(cateName => cateName.CateName == CurrantCateName).Select(cateId => cateId.CateId).FirstOrDefault();
                    connection.InsertUnit(newUnitId, Cob_To_Label.Text.ToUpper(), curruntCateId, Input_Value.Text);
                    Cob_Units.Items.Clear();
                    Reader();
                    confirmationMessage.Content = "Adding to DataBase is Done!";
                    _ = MessageBox.Show($" '{Cob_To_Label.Text.ToUpper()}' Unit under '{CurrantCateName}' Category has been added successfully", "Converter");
                    Cob_Unit_Label.Text = Cob_To_Label.Text = Input_Value.Text = "";
                    confirmationMessage.Content = "";
                }
                else
                {
                    confirmationMessage.Content = " Please, Check again!";
                    _ = MessageBox.Show($"'{Cob_To_Label.Text.ToUpper()}' Unit and '{CurrantCateName}' Category are already existed", "Converter");
                }
            }

            Input.Text = "";
            results.Content = "";
        }
        private void Remove_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Cob_Unit_Label.Text.Any())
            {
                RemoveAction(Cob_Unit_Label.Text.ToUpper());
            }
            else
            {
                confirmationMessage.Content = "Fill Category field";
                _ = MessageBox.Show("you have to specify the Category.", "Converter");
                confirmationMessage.Content = "";
            }
        }

        private void RemoveAction(string CurrantCateName)
        {
            int curruntCateId = CategoriesList.Where(cateName => cateName.CateName == CurrantCateName).Select(cateId => cateId.CateId).FirstOrDefault();

            if (!CategoriesList.Select(cateName => cateName.CateName).ToArray().Contains(CurrantCateName))
            {
                confirmationMessage.Content = "Category is NOT in DataBase, Please check!";
                _ = MessageBox.Show($" '{CurrantCateName}' Category is NOT existed in DataBase, Please check your inputs!", "Converter");
                confirmationMessage.Content = "";

            }
            else
            {
                MessageBoxButton buttons = MessageBoxButton.YesNo;
                DialogResult result = (DialogResult)MessageBox.Show("Do you want to delete all the Category ?", "Converter", buttons);
                if (result == DevExpress.Utils.CommonDialogs.Internal.DialogResult.Yes)
                {
                    (LastCateName, RelatedUnitsList) = BackupAction(CurrantCateName, curruntCateId);
                    connection.DeleteCategory(CurrantCateName, curruntCateId);
                    Cob_Units.Items.Clear();
                    Reader();
                    confirmationMessage.Content = "Delete from DataBase is Done!";
                    _ = MessageBox.Show($" '{CurrantCateName}' Categry with all related Units have been deleted Successfully", "Converter");
                    Cob_Unit_Label.Text = Cob_To_Label.Text = Input_Value.Text = "";
                    confirmationMessage.Content = "";
                    ActionName = "deleteAllCategory";
                }
                else
                {
                    if (Cob_To_Label.Text.Any())
                    {
                        if (!UnitsList.Select(cateName => cateName.UnitName).ToArray().Contains(Cob_To_Label.Text.ToUpper()))
                        {
                            confirmationMessage.Content = "This Unit is NOT in DataBase, Please check!";
                            _ = MessageBox.Show("The Unit is NOT exist in DataBase, Please check your inputs!", "Converter");
                            confirmationMessage.Content = "";
                        }
                        else
                        {
                            if (UnitsList.Where(unitName => unitName.UnitName == Cob_To_Label.Text.ToUpper()).Select(cateId => cateId.CateId).FirstOrDefault() == curruntCateId)
                            {
                                connection.DeleteUnit(Cob_To_Label.Text.ToUpper());
                                Cob_Units.Items.Clear();
                                Reader();
                                confirmationMessage.Content = "Delete from DataBase is Done!";
                                _ = MessageBox.Show($" '{Cob_To_Label.Text.ToUpper()}' Unit has been deleted from '{CurrantCateName}' Categoty Successfully", "Converter");
                                Cob_Unit_Label.Text = Cob_To_Label.Text = Input_Value.Text = "";
                                confirmationMessage.Content = "";
                                ActionName = "deleteOnlyUnit";

                            }
                            else
                            {
                                confirmationMessage.Content = "Check Your Inputs, Please!";
                                _ = MessageBox.Show($"{Cob_To_Label.Text.ToUpper()} is Not under {CurrantCateName} Category, Check your inputs, please!", "Converter");
                                confirmationMessage.Content = "";
                            }

                        }
                    }
                    else
                    {
                        confirmationMessage.Content = "Check again, Please!";
                        _ = MessageBox.Show($"Fill Unit field and try again, Please!", "Converter");
                        confirmationMessage.Content = "";

                    }
                }
            }
        }
        private (string, (string UnitName, int CateId, string Value)[]) BackupAction(string CateName, int currantCateId)
        {
            string currantCateName = CateName;
            (string UnitName, int CateId, string Value)[] unitsDetails = UnitsList.Where(cateId => cateId.CateId == currantCateId).Select(i => (i.UnitName, i.CateId, i.Value)).ToArray();
            return (currantCateName, unitsDetails);
        }
        private void Edit_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Input_Value.Text.Any() && Cob_Unit_Label.Text.Any() && Cob_To_Label.Text.Any() && double.TryParse(Input_Value.Text, out _))
            {
                EditAction(Cob_Unit_Label.Text.ToUpper());
            }

            else
            {
                confirmationMessage.Content = "Check your Inputs, please!";
                _ = MessageBox.Show("You have to fill correct inputs in Category, Unit and Value fields.", "Converter");
                confirmationMessage.Content = "";
            }
        }
    private void EditAction(string CurrantCateName)
    {
        int curruntCateId = CategoriesList.Where(cateName => cateName.CateName == CurrantCateName).Select(cateId => cateId.CateId).FirstOrDefault();
        if (CategoriesList.Select(cateName => cateName.CateName).ToArray().Contains(CurrantCateName))
        {
            if (UnitsList.Select(cateName => cateName.UnitName).ToArray().Contains(Cob_To_Label.Text.ToUpper()))
            {
                if (UnitsList.Where(unitName => unitName.UnitName == Cob_To_Label.Text.ToUpper()).Select(cateId => cateId.CateId).FirstOrDefault() == curruntCateId)
                {
                    connection.EditValue(Cob_To_Label.Text.ToUpper(), curruntCateId);
                    Cob_Units.Items.Clear();
                    Reader();
                    confirmationMessage.Content = "Update DataBase is Done!";
                    _ = MessageBox.Show($" The Value of '{Cob_To_Label.Text.ToUpper()}' Unit under '{CurrantCateName}' Categoty has been updated Successfully", "Converter");
                    Cob_Unit_Label.Text = Cob_To_Label.Text = Input_Value.Text = "";
                    confirmationMessage.Content = "";
                }
                else
                {
                    confirmationMessage.Content = "Check Your Inputs, Please!";
                    _ = MessageBox.Show($"{Cob_To_Label.Text.ToUpper()} is Not under {CurrantCateName} Category, Check your inputs, please!", "Converter");
                    confirmationMessage.Content = "";
                }
            }
            else
            {
                confirmationMessage.Content = "The Unit is Not under the Category!";
                _ = MessageBox.Show($"'{Cob_To_Label.Text.ToUpper()}' is NOT exist in the Unit list, please check your inputs!", "Converter");
                confirmationMessage.Content = "";
            }
        }
        else
        {
            confirmationMessage.Content = "Category is Not exist in DataBase!";
            _ = MessageBox.Show($"'{CurrantCateName}' is NOT exist in Category List, please check your inputs!", "Converter");
            confirmationMessage.Content = "";
        }
           
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
                    if (cateName == "TEMPERATURE")
                    {
                        results.Content = fromCobCurrentValue == "CELSIU" && toCobCurrentValue == "FAHRENHEIT"
                            ? ((inputValue * 1.8) + 32).ToString()
                            : fromCobCurrentValue == "FAHRENHEIT" && toCobCurrentValue == "CELSIU"
                                ? ((inputValue - 32) / 1.8).ToString()
                                : "You have chosen the same Units, Correct it please!";
                    }
                    else
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

        private void Undo_Button_Click(object sender, RoutedEventArgs e)
        {
            int newCateId = CategoriesList.Select(cateId => cateId.CateId).ToArray().Max();
            int newUnitId = UnitsList.Select(unitId => unitId.UnitId).ToArray().Max();
            switch (ActionName)
            {
                
                case "deleteAllCategory":
                    {
                        connection.InsertCategory(newCateId, LastCateName);
                        for (int i = 0; i < RelatedUnitsList.Length; i++)
                        {
                            var (unitName, cateId, value) = RelatedUnitsList[i];
                            connection.InsertUnit(newUnitId, unitName, newCateId + 1, value);
                        }
                    }; break;

                case "deleteOnlyUnit":
                    {
                        var (unitName, cateId, value) = RelatedUnitsList[0];
                        connection.InsertUnit(newUnitId, unitName, cateId, value);
                    }; break;
            }
            RelatedUnitsList.Select(i => (i.UnitName, i.CateId, i.Value)).ToList().Clear();
            Cob_Units.Items.Clear();
            Reader();
            confirmationMessage.Content = "Undo is done!";
            _ = MessageBox.Show($"Undo the process is done!", "Converter");
            confirmationMessage.Content = "";
        }
    }
}
