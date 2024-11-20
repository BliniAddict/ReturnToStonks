using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReturnToStonks
{
  public interface IModel
  {
    List<Category> GetCategories();
    Category GetCategory(string name);
  }
}
