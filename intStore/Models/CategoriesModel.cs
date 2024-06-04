using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace intStore.Models
{
    public  class CategoriesModel
    {
        public int id_Categories { get; set; }
        public string NameCategories { get; set; }
        public string Desciptions { get; set; }
        public ImageSource ImageCategories { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
