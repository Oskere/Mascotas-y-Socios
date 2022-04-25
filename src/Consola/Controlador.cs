using App;
using Modelos;

namespace Consola
{
    public class Controlador
    {

        public GestorClub gestor;

        public Vista vista = new Vista();
        public Dictionary<string, Action> casosDeUso;
        public Controlador(GestorClub gestor)
        {
            this.gestor = gestor;

            casosDeUso = new Dictionary<string, Action>(){
            {"Obtener listado de mascotas por especie",ListaMascotasEspecie},
            {"Obtener listado de mascotas por edad", ListaMascotasEdad},
            {"Buscar mascotas de socio",buscarMascotasSocio},
            {"Cambiar dueño de mascota",cambiarDuenos},
            {"Ver listado de socios",verSocios},
            {"Dar socio de baja",bajaSocio},
            {"Registrar nuevo socio",darAltaSocio},
            {"Registrar nueva mascota",registrarMascota},
            {"Dar de baja mascota",darBajaMascota}
        };
        }

        public void Run()
        {
            while (true)
            {
                try
                {
                    string eleccion = vista.TryObtenerElementoDeLista("Operaciones disponibles", casosDeUso.Keys.ToList(), "Elija una operacion");
                    casosDeUso[eleccion].Invoke();
                    vista.MostrarYReturn("Pulsa <Return> para continuar");
                    vista.LimpiarPantalla();
                }
                catch { return; }
            }
        }

        public void darAltaSocio()
        {
            string _dni = vista.TryObtenerDatoDeTipo<string>("Introduzca su DNI");
            if (gestor.buscarSocio(_dni) == null)
            {
                string name = vista.TryObtenerDatoDeTipo<string>("Introduzca su nombre");
                Sexo sex = vista.TryObtenerElementoDeLista<Sexo>("Elija su sexo", vista.EnumToList<Sexo>(), "Introduzca numero de la lista");
                Socio socio = new Socio
                {
                    dni = _dni,
                    nombre = name,
                    sexo = sex
                };
                gestor.altaSocio(socio);
            }
            else { vista.Mostrar("Ya hay un socio registrado con ese DNI"); }
        }

        public void darBajaMascota()
        {
            List<Mascota> mascotas = gestor.mascotas;
            Mascota mascota = vista.TryObtenerElementoDeLista("Elija la mascota", mascotas, "Introduzca numero de la lista");
            if (vista.Confirmar("Desea confirmar la baja?"))
            {
                gestor.bajaMascota(mascota);
            }
        }

        public void registrarMascota()
        {
            List<Socio> socios = gestor.socios;
            Socio socio = vista.TryObtenerElementoDeLista("Elija el socio al cargo de la mascota", socios, "Introduzca numero de la lista");
            string nombreMascota = vista.TryObtenerDatoDeTipo<string>("Introduzca el nombre de la mascota");
            Especie especieMasc = vista.TryObtenerElementoDeLista<Especie>("Cual es la especie de la mascota?", vista.EnumToList<Especie>(), "Introduzca numero de la lista");
            DateTime fnnMascot = vista.TryObtenerFecha("Introduzca una fecha aproximada de nacimiento");
            Mascota mascota = new Mascota
            {
                fnn = fnnMascot,
                nombre = nombreMascota,
                especie = especieMasc,
                dniSocio = socio.dni,
                dueno = socio,
            };
            gestor.altaMascota(mascota);
        }

        public void buscarMascotasSocio()
        {
            List<Socio> socios = gestor.socios;
            Socio socio = vista.TryObtenerElementoDeLista("Elija el socio del cual buscar", socios, "Introduzca numero de la lista");
            List<Mascota> mascotas = gestor.mascotasDeSocio(socio);
            vista.MostrarListaEnumerada($"Mascotas de {socio.nombre}", mascotas);
        }

        public void verSocios()
        {
            List<Socio> socios = gestor.socios;
            vista.MostrarListaEnumerada("Socios", socios);
        }

        public void cambiarDuenos()
        {
            List<Mascota> mascotas = gestor.mascotas;
            Mascota mascota = vista.TryObtenerElementoDeLista("Elija la mascota", mascotas, "Introduzca numero de la lista");
            List<Socio> socios = gestor.socios;
            Socio socio = vista.TryObtenerElementoDeLista("Elija el nuevo dueño", socios, "Introduzca numero de la lista");
            gestor.comprarMascota(mascota, socio);
        }
        public void ListaMascotasEspecie()
        {
            List<Mascota> mascotas = gestor.mascotasPorEspecie();
            vista.MostrarListaEnumerada("Mascotas por especie", mascotas);
        }
        public void bajaSocio()
        {
            List<Socio> socios = gestor.socios;
            Socio socio = vista.TryObtenerElementoDeLista("Elija el socio a dar de baja", socios, "Introduzca numero de la lista");
            List<Mascota> mascotas = gestor.mascotasDeSocio(socio);
            vista.MostrarListaEnumerada("Las siguentes mascotas serán eliminadas", mascotas);
            if (vista.Confirmar("Desea confirmar la baja?"))
            {
                mascotas.ForEach(mascota => gestor.bajaMascota(mascota));
                gestor.bajaSocio(socio);
            }
        }
        public void ListaMascotasEdad()
        {
            List<Mascota> mascotas = gestor.mascotasPorEdad();
            vista.MostrarListaEnumerada("Mascotas por edad", mascotas);
        }
    }
}