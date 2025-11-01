using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CadParcial2SAZ;

namespace ClnParcial2Lfms
{
    public class ProgramaCln
    {
        public static int insertar(Programa programa)
        {
            using (var context = new Parcial2SAZEntities())
            {
                context.Programa.Add(programa);
                context.SaveChanges();
                return programa.id;
            }
        }
        public static int actualizar(Programa programa)
        {
            using (var context = new Parcial2SAZEntities())
            {
                var existe = context.Programa.Find(programa.id);
                existe.titulo = programa.titulo;
                existe.descripcion = programa.descripcion;
                existe.duracion = programa.duracion;
                existe.productor = programa.productor;
                existe.fechaEstreno = programa.fechaEstreno;
                existe.idCanal = programa.idCanal;
                existe.usuarioRegistro = programa.usuarioRegistro;
                return context.SaveChanges();
            }
        }
        public static int eliminar(int id, string usuarioRegistro)
        {
            using (var context = new Parcial2SAZEntities())
            {
                var existe = context.Programa.Find(id);
                existe.estado = -1;
                existe.usuarioRegistro = usuarioRegistro;
                return context.SaveChanges();
            }
        }
        public static Programa obtenerUno(int id)
        {
            using (var context = new Parcial2SAZEntities())
            {
                return context.Programa.Find(id);
            }
        }
        public static List<Programa> listar()
        {
            using (var context = new Parcial2SAZEntities())
            {
                return context.Programa.Where(x => x.estado != -1).ToList();
            }
        }
        public static List<paListarProgramas_Result> listarPa(string parametro)
        {
            using (var context = new Parcial2SAZEntities())
            {
                return context.paListarProgramas(parametro).ToList();
            }
        }

    }
}

