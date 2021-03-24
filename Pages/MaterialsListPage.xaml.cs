using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
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

        private void LstMaterials_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string newPhotoPath = "";

            bool flag = false;

            if (LstMaterials.SelectedItem is Materials materials)
            {

                Classes.AddEditClass.StatusEditAdd = Classes.AddEditClass.EditAddClient.Edit;

                Windows.AddEditMaterials addEditMaterials = new Windows.AddEditMaterials();

                addEditMaterials.Owner = Classes.ParentWindow.parentWindow;

                addEditMaterials.IdMaterial = materials.IdMaterial;

                if(materials.CountOnWarehouse > materials.MinCount)
                {
                    decimal res = Math.Ceiling(Convert.ToDecimal(materials.MinCount) / Convert.ToDecimal(materials.CountPackaging));
                    res *= materials.CountPackaging;
                    res *= materials.Price;
                    addEditMaterials.LblMinPartion.Content = res.ToString();
                }
                else
                {
                    int buff_count = materials.MinCount - materials.CountOnWarehouse;
                    decimal res = Math.Ceiling(Convert.ToDecimal(buff_count) / Convert.ToDecimal(materials.CountPackaging));
                    res *= materials.CountPackaging;
                    res *= materials.Price;
                    addEditMaterials.LblMinPartion.Content = res.ToString();
                }
                

                addEditMaterials.TxtNameMaterial.Text = materials?.NameMaterial.ToString();
                addEditMaterials.TxtCountOnWarehouse.Text = materials?.CountOnWarehouse.ToString();
                addEditMaterials.TxtCountPack.Text = materials?.CountPackaging.ToString();
                addEditMaterials.TxtMinCount.Text = materials?.MinCount.ToString();
                addEditMaterials.TxtPrice.Text = materials?.Price.ToString();
                addEditMaterials.TxtDescription.Text = materials?.Description;

                //byte[] imgdata = System.IO.File.ReadAllBytes(materials?.Photo);
                //System.Windows.Resources.StreamResourceInfo res_img = Application.GetResourceStream(new Uri(materials?.Photo, UriKind.Relative));
                //addEditMaterials.ImgMaterial.Source = materials?.Photo != null ? byteToImage(imgdata) : null;
                addEditMaterials.ImgMaterial.Source = materials?.Photo != null ? new BitmapImage(new Uri(materials?.Photo, UriKind.Relative)) : null;
                Classes.FilePathFoto.path = materials?.Photo;


                addEditMaterials.ComboTypeMaterial.SelectedItem = materials?.TypeMaterial.NameTypeMaterial;
                addEditMaterials.ComboTypeUnit.SelectedItem = materials?.TypeUnit.NameTypeUnit;

                addEditMaterials.DgSupplair.ItemsSource = materials?.ListSuppliers;

                addEditMaterials.ShowDialog();

                if (addEditMaterials.DialogResult.HasValue && addEditMaterials.DialogResult.Value)
                {

                    if(addEditMaterials.StatusDelete == false)
                    {
                        var materialSave = Entities.GetContext().Materials.Where(i => i.IdMaterial == materials.IdMaterial).FirstOrDefault();

                        if (materialSave.Photo != Classes.FilePathFoto.path)
                        {
                            newPhotoPath = $"/materials/mat_{materialSave.IdMaterial}.jpeg";
                            materialSave.Photo = newPhotoPath;
                            flag = true;
                        }


                        materialSave.NameMaterial = addEditMaterials.TxtNameMaterial.Text;
                        materialSave.CountOnWarehouse = Int32.Parse(addEditMaterials.TxtCountOnWarehouse.Text);
                        materialSave.CountPackaging = Int32.Parse(addEditMaterials.TxtCountPack.Text);
                        materialSave.MinCount = Int32.Parse(addEditMaterials.TxtMinCount.Text);
                        materialSave.Price = Decimal.Parse(addEditMaterials.TxtPrice.Text);
                        materialSave.Description = addEditMaterials.TxtDescription.Text;



                        materialSave.IdTypeMaterial = Entities.GetContext().TypeMaterial.Where(i => i.NameTypeMaterial == addEditMaterials.ComboTypeMaterial.Text).FirstOrDefault().IdTypeMaterial;
                        materialSave.IdTypeUnit = Entities.GetContext().TypeUnit.Where(i => i.NameTypeUnit == addEditMaterials.ComboTypeUnit.Text).FirstOrDefault().IdTypeUnit;

                        Entities.GetContext().Entry(materialSave).State = EntityState.Modified;

                        Entities.GetContext().SaveChanges();
                    }

                    LstMaterials.SelectedItem = materials;

                    // string tempFileName = AppDomain.CurrentDomain.BaseDirectory + "Content\\AdContent\\" + System.IO.Path.GetRandomFileName();

                    CopyFoto(flag, newPhotoPath);
                }
                

                Filter();
            }
        }

        public void CopyFoto(bool flag, string newPhotoPath)
        {
            if (flag)
            {
                string b = Assembly.GetExecutingAssembly().Location;

                b = b.Replace("\\", "/");
                string[] mass = b.Split('/');

                string res = "";

                for (int i = 0; i < mass.Count() - 3; i++)
                {
                    res += mass[i] + '/';
                }

                string result = res + newPhotoPath;
                result = result.Replace("//", "/");

                //try
                //{
                if (File.Exists(result))
                {
                    File.Delete(result);
                }

                //System.Windows.Resources.StreamResourceInfo res_img = Application.GetResourceStream(new Uri(Classes.FilePathFoto.path, UriKind.Absolute));

                //res_img.Stream.CopyTo(new System.IO.FileStream(result, System.IO.FileMode.Create));

                File.Copy(Classes.FilePathFoto.path, result);

                //File.Move(Classes.FilePathFoto.path, result);
                //}
                //catch(Exception ex)
                //{

                //}

            }
        }

        private void BtnAddMaterial_Click(object sender, RoutedEventArgs e)
        {
            Classes.AddEditClass.StatusEditAdd = Classes.AddEditClass.EditAddClient.Greate;

            Classes.FilePathFoto.path = "";

            Windows.AddEditMaterials addEditMaterials = new Windows.AddEditMaterials();

            addEditMaterials.Owner = Classes.ParentWindow.parentWindow;

            addEditMaterials.BtnDel.Visibility = Visibility.Hidden;

            addEditMaterials.ShowDialog();

            if (addEditMaterials.DialogResult.HasValue && addEditMaterials.DialogResult.Value)
            {


                var materials = new Materials()
                {
                    NameMaterial = addEditMaterials.TxtNameMaterial.Text,
                    CountOnWarehouse = Int32.Parse(addEditMaterials.TxtCountOnWarehouse.Text),
                    CountPackaging = Int32.Parse(addEditMaterials.TxtCountPack.Text),
                    MinCount = Int32.Parse(addEditMaterials.TxtMinCount.Text),
                    Price = Decimal.Parse(addEditMaterials.TxtPrice.Text),
                    Description = addEditMaterials.TxtDescription.Text,
                    IdTypeMaterial = Entities.GetContext().TypeMaterial.Where(i => i.NameTypeMaterial == addEditMaterials.ComboTypeMaterial.Text).FirstOrDefault().IdTypeMaterial,
                    IdTypeUnit = Entities.GetContext().TypeUnit.Where(i => i.NameTypeUnit == addEditMaterials.ComboTypeUnit.Text).FirstOrDefault().IdTypeUnit
                };

                Entities.GetContext().Entry(materials).State = EntityState.Added;

                Entities.GetContext().SaveChanges();


                if (Classes.FilePathFoto.path != "")
                {
                    string newPhotoPath = $"/materials/mat_{materials.IdMaterial}.jpeg";
                    materials.Photo = newPhotoPath;
                    bool flag = true;

                    Entities.GetContext().Entry(materials).State = EntityState.Modified;
                    Entities.GetContext().SaveChanges();

                    CopyFoto(flag, newPhotoPath);

                }
            }

            Filter();

        }
    }
}
