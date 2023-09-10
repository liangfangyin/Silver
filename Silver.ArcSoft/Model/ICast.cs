using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Silver.ArcSoft.Model
{
    public interface ICast<out T>
    {
        T Cast();
    }
}
