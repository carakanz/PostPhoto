using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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

namespace WpfApp4
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void PostClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog myDialog = new OpenFileDialog();
            myDialog.Multiselect = false;
            if (myDialog.ShowDialog() == true)
            {
                this.Path.Text = myDialog.FileName;
                byte[] responseByte;
                try
                {
                    using (var client = new System.Net.WebClient())
                    {
                        //responseByte = client.UploadFile(new Uri("http://localhost:7515/api/values/"), myDialog.FileName);
                        responseByte = client.UploadFile(new Uri("http://postphoto20170522081051.azurewebsites.net/api/values/"), myDialog.FileName);
                        Encoding enc8 = Encoding.UTF8;
                        string response = enc8.GetString(responseByte);
                        this.Result.Text = response;
                    }
                }
                catch (Exception exception)
                {
                    this.Result.Text = exception.Message;
                }
            }
        }
    }
}
