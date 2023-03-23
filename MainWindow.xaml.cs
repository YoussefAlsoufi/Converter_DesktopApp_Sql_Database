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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public NameValueCollection lengthSection, dataTypeSection, tempretureSection;
        string currentCobValue = "";
        string fromCobCurrentValue = "";
        string toCobCurrentValue = "";
        string currentValue = "";
        public MainWindow()
        {
            InitializeComponent();
            Cob_Units.SelectedIndex = 0;
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
