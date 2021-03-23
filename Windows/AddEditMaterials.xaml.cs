using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        public int IdMaterial;
        public AddEditMaterials()
        {
            InitializeComponent();

            ComboTypeMaterial.ItemsSource = Entities.GetContext().TypeMaterial.ToList();

            ComboTypeMaterial.SelectedIndex = 0;

            ComboTypeUnit.ItemsSource = Entities.GetContext().TypeUnit.ToList();

            ComboTypeUnit.SelectedIndex = 0;

            ComboListSupplair.ItemsSource = Entities.GetContext().Suppliers.ToList();

            ComboListSupplair.SelectedIndex = 0;
        }

        private void ViewFoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.ShowDialog();

            var filePath = openFileDialog.FileName;

            Classes.FilePathFoto.path = filePath.ToString();

            //string FileName = filePath.Substring(filePath.LastIndexOf('\\') + 1);

            ImgMaterial.Source = new BitmapImage(new Uri(filePath, UriKind.RelativeOrAbsolute));
        }

        private void BtnAddSupplier_Click(object sender, RoutedEventArgs e)
        {
            if(ComboListSupplair.SelectedItem is Suppliers suppliers)
            {
                var materialToSupplier = new MaterialToSupplier()
                {
                    IdMaterial = IdMaterial,
                    IdSupplier = suppliers.IdSupplier
                };

                Entities.GetContext().Entry(materialToSupplier).State = EntityState.Added;

                Materials materials = Entities.GetContext().Materials.Where(i => i.IdMaterial == IdMaterial)?.FirstOrDefault();
                DgSupplair.ItemsSource = materials?.ListSuppliers;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void BtnDelateSupplier_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Удалить?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {

                if (DgSupplair.SelectedItem is Suppliers suppliers)
                {
                    var MatYoSuppl = Entities.GetContext().MaterialToSupplier.Where(i => i.IdMaterial == IdMaterial && i.IdSupplier == suppliers.IdSupplier).FirstOrDefault();

                    if (MatYoSuppl != null)
                    {
                        Entities.GetContext().Entry(MatYoSuppl).State = EntityState.Deleted;

                        Materials materials = Entities.GetContext().Materials.Where(i => i.IdMaterial == IdMaterial)?.FirstOrDefault();
                        DgSupplair.ItemsSource = materials?.ListSuppliers;

                    }
                    else
                    {
                        MessageBox.Show("Невозможно удалить");
                    }

                }
            }
        }
    }
}
