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
        public MaterialsListPage()
        {
            InitializeComponent();

            LstMaterials.ItemsSource = Entities.GetContext().Materials.ToList().Take(15);

            for(int i = 15, j = 1; i < Entities.GetContext().Materials.ToList().Count; i += 15, j++)
            {
                CountPage.Add(j);
            }

            LstPages.ItemsSource = CountPage.ToList();

            LstPages.SelectedIndex = 0;
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
    }
}
