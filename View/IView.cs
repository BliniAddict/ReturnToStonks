using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReturnToStonks
{
  public interface IView
  {
    virtual void CloseCategoryPopup() { }

    virtual void OpenCategoryPopup() { }
  }
}
