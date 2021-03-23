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

namespace MyGloveShop.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddEditMaterials.xaml
    /// </summary>
    public partial class AddEditMaterials : Window
    {
        public AddEditMaterials()
        {
            InitializeComponent();

            ComboTypeMaterial.ItemsSource = Entities.GetContext().TypeMaterial.ToList();

            ComboTypeMaterial.SelectedIndex = 0;

            ComboTypeUnit.ItemsSource = Entities.GetContext().TypeUnit.ToList();

            ComboTypeUnit.SelectedIndex = 0;
        }
    }
}
