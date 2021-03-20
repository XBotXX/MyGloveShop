using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGloveShop
{
    class CustomClass
    {
    }

    public partial class Materials
    {
        public List<Suppliers> ListSuppliers
        {
            get
            {
                return MaterialToSupplier.Select(i => i.Suppliers).ToList();
            }
        }
    }
}
