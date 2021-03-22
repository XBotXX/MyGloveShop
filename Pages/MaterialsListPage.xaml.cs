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

        public MaterialsListPage()
        {
            InitializeComponent();

            ListMaterialsBuff.CollectionChanged += ListMaterialsBuff_CollectionChanged;

            ListMaterialsBuff.Add(Entities.GetContext().Materials.ToList());

            LstMaterials.ItemsSource = Entities.GetContext().Materials.ToList().Take(15);

            for(int i = 15, j = 1; i < Entities.GetContext().Materials.ToList().Count; i += 15, j++)
            {
                CountPage.Add(j);
            }

            LstPages.ItemsSource = CountPage.ToList();

            LstPages.SelectedIndex = 0;
        }

        private void ListMaterialsBuff_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            LstMaterials.ItemsSource = ListMaterialsBuff[0].ToList();
        }

        private void LstPages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(LstPages.SelectedItem is int countPage)
            {
                if(countPage == 1)
                {
                    LstMaterials.ItemsSource = Entities.GetContext().Materials.ToList().Take(15);
                }
                else
                {
                    LstMaterials.ItemsSource = Entities.GetContext().Materials.ToList().Skip(15 * countPage).Take(15);
                }
            }
        }

        public void Filter()
        {

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
    }
}
