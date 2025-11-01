using CadParcial2SAZ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace ClnParcial2SAZ
{
    public class CanalCln
    {
        public static List<Canal> listar()
        {
            using (var context = new Parcial2SAZEntities())
            {
                return context.Canal
                    .Where(x => x.estado == 1)
                    .OrderBy(x => x.nombre)
                    .ToList();
            }
        }
    }
}
