using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyGloveShop.Pages
{
    /// <summary>
    /// Логика взаимодействия для MaterialsListPage.xaml
    /// </summary>
    public partial class MaterialsListPage : Page
    {
        public List<int> CountPage = new List<int>();

        public List<Materials> isCheckList = new List<Materials>();

        public static ObservableCollection<List<Materials>> ListMaterialsBuff = new ObservableCollection<List<Materials>>();

        public List<string> LstSort = new List<string>()
        {
            "(-- выбирите --)",
            "Сортировать по нименованию и возратанию",
            "Сортировать по наимонованию и убыванию",
            "Сортировать по остатку на складе и возратанию",
            "Сортировать по остатку на складе и убыванию",
            "Сортировать по стоимости и возратанию",
            "Сортировать по стоимости и убыванию"
        };

        public MaterialsListPage()
        {
            InitializeComponent();

            var nameTypeMaterial = Entities.GetContext().TypeMaterial.Select(i => i.NameTypeMaterial).ToList();

            nameTypeMaterial.Insert(0, "(-- выбирите --)");

            ComboFilter.ItemsSource = nameTypeMaterial;

            //ListMaterialsBuff.CollectionChanged += ListMaterialsBuff_CollectionChanged;

            ComboSort.ItemsSource = LstSort.ToList();

            ListMaterialsBuff.Add(Entities.GetContext().Materials.ToList());

            LstMaterials.ItemsSource = Entities.GetContext().Materials.ToList().Take(15);

            for(int i = 15, j = 1; i < Entities.GetContext().Materials.ToList().Count; i += 15, j++)
            {
                CountPage.Add(j);
            }

            LstPages.ItemsSource = CountPage.ToList();

            LstPages.SelectedIndex = 0;
        }

        //private void ListMaterialsBuff_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    LstMaterials.ItemsSource = ListMaterialsBuff[0].ToList();
        //}

        private void LstPages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filter();
        }

        public void Filter()
        {
            // Search Start
            ListMaterialsBuff[0] = Entities.GetContext().Materials.Where(i => i.NameMaterial.Contains(TxtSeach.Text) || i.Description.Contains(TxtSeach.Text)).ToList();
            // Search End

            switch (ComboSort.SelectedItem)
            {
                case "Сортировать по нименованию и возратанию":
                    ListMaterialsBuff[0] = ListMaterialsBuff[0].OrderBy(i => i.NameMaterial).ToList();
                    break;
                case "Сортировать по наимонованию и убыванию":
                    ListMaterialsBuff[0] = ListMaterialsBuff[0].OrderByDescending(i => i.NameMaterial).ToList();
                    break;
                case "Сортировать по остатку на складе и возратанию":
                    ListMaterialsBuff[0] = ListMaterialsBuff[0].OrderBy(i => i.CountOnWarehouse).ToList();
                    break;
                case "Сортировать по остатку на складе и убыванию":
                    ListMaterialsBuff[0] = ListMaterialsBuff[0].OrderByDescending(i => i.CountOnWarehouse).ToList();
                    break;
                case "Сортировать по стоимости и возратанию":
                    ListMaterialsBuff[0] = ListMaterialsBuff[0].OrderBy(i => i.Price).ToList();
                    break;
                case "Сортировать по стоимости и убыванию":
                    ListMaterialsBuff[0] = ListMaterialsBuff[0].OrderByDescending(i => i.Price).ToList();
                    break;
                default:
                    ListMaterialsBuff[0] = ListMaterialsBuff[0].OrderBy(i => i.IdMaterial).ToList();
                    break;
            }

            if(ComboFilter.SelectedIndex != 0)
            {
                ListMaterialsBuff[0] = ListMaterialsBuff[0].Where(i => i.IdTypeMaterial == ComboFilter.SelectedIndex).ToList();
            }

            int buffCountRows = ListMaterialsBuff[0].Count;

            // Count page Start
            CountPage.Clear();

            for (int i = 15, j = 1; i < ListMaterialsBuff[0].ToList().Count; i += 15, j++)
            {
                CountPage.Add(j);
            }

            LstPages.ItemsSource = CountPage.ToList();

            if (LstPages.SelectedItem is int countPage)
            {
                if (countPage == 1)
                {
                    ListMaterialsBuff[0] = ListMaterialsBuff[0].Take(15).ToList();
                }
                else
                {
                    ListMaterialsBuff[0] = ListMaterialsBuff[0].Skip(15 * countPage).Take(15).ToList();
                }
            }

            // Count Rows Start
            TxtCountRows.Text = ListMaterialsBuff[0].Count.ToString() + " из " + buffCountRows.ToString();
            // Count Rows End

            LstMaterials.ItemsSource = ListMaterialsBuff[0].ToList();
            // Count page End
        }

        private void BtnMinCountEdit_Click(object sender, RoutedEventArgs e)
        {
            Classes.AddEditClass.StatusEditAdd = Classes.AddEditClass.EditAddClient.Edit;

            Windows.WindowEditMinCount windowEditMinCount = new Windows.WindowEditMinCount();

            windowEditMinCount.Owner = Classes.ParentWindow.parentWindow;

            windowEditMinCount.TxtMinCount.Text = isCheckList.Min(i=>i.MinCount).ToString();

            windowEditMinCount.ShowDialog();

            if (windowEditMinCount.DialogResult.HasValue && windowEditMinCount.DialogResult.Value)
            {

                int buff = Int32.Parse(windowEditMinCount.TxtMinCount.Text);

                foreach(var i in isCheckList)
                {
                    i.MinCount = buff;
                    Entities.GetContext().Entry(i).State = EntityState.Modified;
                }

                Entities.GetContext().SaveChanges();

                isCheckList.Clear();

                BtnMinCountEdit.Visibility = Visibility.Hidden;
            }

            LstMaterials.ItemsSource = Entities.GetContext().Materials.ToList().Take(15);
        }

        private void Check_Mat_ISChecked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            var item = checkBox.DataContext;
            LstMaterials.SelectedItem = item;

            BtnMinCountEdit.Visibility = Visibility.Visible;
            isCheckList.Add(LstMaterials.SelectedItem as Materials);
        }

        private void Check_Mat_UnISChecked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            var item = checkBox.DataContext;
            LstMaterials.SelectedItem = item;

            isCheckList.Remove(LstMaterials.SelectedItem as Materials);
            if(isCheckList.Count <= 0)
            {
                BtnMinCountEdit.Visibility = Visibility.Hidden;
            }
        }

        private void TxtSeach_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filter();
        }

        private void ComboSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filter();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ComboSort.SelectedIndex = 0;
            ComboFilter.SelectedIndex = 0;
        }

        private void ComboFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filter();
        }
    }
}
