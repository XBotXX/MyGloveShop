using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGloveShop.Classes
{
    class AddEditClass
    {
        public enum EditAddClient
        {
            Greate,
            Edit
        }

        public static EditAddClient StatusEditAdd { get; set; }
    }
}
