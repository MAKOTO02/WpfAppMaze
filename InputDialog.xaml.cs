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
using System.Windows.Shapes;

namespace WpfAppMaze
{
    /// <summary>
    /// InputDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class InputDialog : Window
    {
        public int SelectedWidth { get; private set; }
        public int SelectedHeight { get; private set; }
        public bool Randomize = false;
        public bool ShowPath = false;

        public InputDialog()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(WidthTextBox.Text, out int width))
            {
                SelectedWidth = width;
            }
            else
            {
                MessageBox.Show("Please enter a valid number for width.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (int.TryParse(HeightTextBox.Text, out int height))
            {
                SelectedHeight = height;
            }
            else
            {
                MessageBox.Show("Please enter a valid number for height.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DialogResult = true;
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void RandomizeCheckBox_Click(object sender, RoutedEventArgs e)
        {
            Randomize = !Randomize;
        }

        private void DrawPathCheckBox_Click(object sender, RoutedEventArgs e)
        {
            ShowPath = !ShowPath;
        }

    }
}
