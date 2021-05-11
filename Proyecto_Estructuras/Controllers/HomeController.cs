using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Proyecto_Estructuras.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Arbol;
using ArbolAVL;
using ListaDobleEnlace;
using THash;
using CsvHelper;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Proyecto_Estructuras.Controllers
{
    public class HomeController : Controller
    {
        const string RutaUsuario = "\\files\\Users\\Users.csv";
        const string RutaCentros = "\\files\\InformacionCentro\\";
        const string RutaPacientes = "\\files\\InformacionPacientes\\";

        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }


        //-----------------------------Controladores Views--------------------------------------
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index([FromServices] IHostingEnvironment HostEnvi, string Password, string User)
        {
            if (Password != null && User != null)
            {
                ListaDoble<Usuarios> ListaUsuarios = new ListaDoble<Usuarios>();
                Usuarios Login = new Usuarios();
                Login.User = User;
                Login.Password = Password;
                var fileName = $"{HostEnvi.WebRootPath}{RutaUsuario}";
                using (var reader = new StreamReader(fileName))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Read();
                    csv.ReadHeader();
                    while (csv.Read())
                    {
                        var Usuario = csv.GetRecord<Usuarios>();
                        if(Usuario.Password == Login.Password && Login.User == Usuario.User)
                        {
                            HttpContext.Session.SetString((HttpContext.Session.Id + "Centro"), Usuario.Centro);
                            string Centro = HttpContext.Session.GetString(HttpContext.Session.Id + "Centro");
                            var FileName = $"{HostEnvi.WebRootPath}{RutaPacientes}{Regex.Replace(Centro, @"\s", "")}\\Pacientes.csv";


                            FileInfo Myfile = new FileInfo(FileName);
                            if (Myfile.Exists)
                            {
                                THash<paciente> TablasHashPacientes = new THash<paciente>();
                                using (var lector = new StreamReader(FileName))
                                using (var CSV = new CsvReader(lector, CultureInfo.InvariantCulture))
                                {
                                    CSV.Read();
                                    CSV.ReadHeader();
                                    while (CSV.Read())
                                    {
                                        var Paciente = CSV.GetRecord<paciente>();
                                        TablasHashPacientes.Insertar(Paciente, Paciente.Prioridad);
                                    }
                                }

                                ListaDoble<paciente> ListaPacientes = new ListaDoble<paciente>();
                                for (int i = 0; i < 3; i++)
                                {
                                    
                                    for (int j = 0; j < TablasHashPacientes.HashTable[i].contador; j++)
                                    {
                                        paciente nuevo = TablasHashPacientes.HashTable[i].ObtenerValor(j);
                                        ListaPacientes.InsertarFinal(nuevo);
                                    }
                                }
                                return View("Registro", ListaPacientes);
                            }
                            else
                            {
                                Directory.CreateDirectory($"{HostEnvi.WebRootPath}{RutaPacientes}{Regex.Replace(Centro, @"\s", "")}");
                                using (StreamWriter sw = new StreamWriter(FileName))
                                {
                                    sw.WriteLine("Nombre,Apellido,DPI_CUI,Departamento,Municipio_residencia,Edad,Vacunado,Prioridad");
                                }
                            }

                            return View("Registro");
                        }
                    }
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult CreateNewUser()
        {
            return View();
        }
        [HttpPost]//Se quiere agregar un mensaje para cuando no se puede agregar un usuario
        public IActionResult CreateNewUser(string User, string Password, string Centro, string AUser, string APassword, [FromServices] IHostingEnvironment HostEnvi) 
        {
            if(User != null && Password != null && Centro != null && AUser != null && APassword != null)
            {
                ListaDoble<Usuarios> ListaUsuarios = new ListaDoble<Usuarios>();
                Usuarios AdminAutentication = new Usuarios();
                AdminAutentication.User = AUser;
                AdminAutentication.Password = APassword;
                var fileName = $"{HostEnvi.WebRootPath}" + RutaUsuario;
                using (var reader = new StreamReader(fileName))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Read();
                    csv.ReadHeader(); while (csv.Read())
                    {
                        var Usuario = csv.GetRecord<Usuarios>();
                        ListaUsuarios.InsertarFinal(Usuario);
                        if (Usuario.Password == AdminAutentication.Password && AdminAutentication.User == Usuario.User)
                        {
                            Usuarios NewUser = new Usuarios();
                            NewUser.Centro = Centro;
                            NewUser.User = User;
                            NewUser.Password = Password;

                            ListaUsuarios.InsertarFinal(NewUser);
                        }
                    }
                }
                using (StreamWriter sw = new StreamWriter(fileName))
                {
                    sw.WriteLine("User,Password,Centro");
                    foreach (Usuarios Usuario in ListaUsuarios)
                    {
                        sw.WriteLine(Usuario.User + "," + Usuario.Password + "," + Usuario.Centro);
                    }
                }
                return View("Index");
            }
            return View();
        }

        public IActionResult Registro([FromServices] IHostingEnvironment HostEnvi)
        {
            string Centro = HttpContext.Session.GetString(HttpContext.Session.Id + "Centro");
            var FileName = $"{HostEnvi.WebRootPath}{RutaPacientes}{Regex.Replace(Centro, @"\s", "")}\\Pacientes.csv";

            ListaDoble<paciente> ListaPacientes = new ListaDoble<paciente>();
            using (var lector = new StreamReader(FileName))
            using (var CSV = new CsvReader(lector, CultureInfo.InvariantCulture))
            {
                THash<paciente> TablasHashPacientes = new THash<paciente>();
                CSV.Read();
                CSV.ReadHeader();
                while (CSV.Read())
                {
                    var Paciente = CSV.GetRecord<paciente>();
                    TablasHashPacientes.Insertar(Paciente, Paciente.Prioridad);
                } 


                for (int i = 0; i < 3; i++)
                {

                    for (int j = 0; j < TablasHashPacientes.HashTable[i].contador; j++)
                    {
                        paciente nuevo = TablasHashPacientes.HashTable[i].ObtenerValor(j);
                        ListaPacientes.InsertarFinal(nuevo);
                    }
                }
            }
            return View(ListaPacientes);
        }

        [HttpGet]
        public IActionResult Registrar()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Registrar([FromServices] IHostingEnvironment HostEnvi, int Prioridad, string Nombre, string Apellido, long DPI_CUI, string Departamento, string Municipio_residencia, int Edad, bool Vacunado)
        {
            if (Nombre != null && Apellido != null &&  DPI_CUI != 0 &&  Departamento != null && Municipio_residencia != null && Edad != 0)
            {
                string Centro = HttpContext.Session.GetString(HttpContext.Session.Id + "Centro");
                var FileName = $"{HostEnvi.WebRootPath}{RutaPacientes}{Regex.Replace(Centro, @"\s", "")}\\Pacientes.csv";

                ListaDoble<paciente> ListaPacientes = new ListaDoble<paciente>();
                using (var lector = new StreamReader(FileName))
                using (var CSV = new CsvReader(lector, CultureInfo.InvariantCulture))
                {
                    THash<paciente> TablasHashPacientes = new THash<paciente>();
                    CSV.Read();
                    CSV.ReadHeader();
                    while (CSV.Read())
                    {
                        var Paciente = CSV.GetRecord<paciente>();
                        TablasHashPacientes.Insertar(Paciente, Paciente.Prioridad);
                    }



                    for (int i = 0; i < 3; i++)
                    {

                        for (int j = 0; j < TablasHashPacientes.HashTable[i].contador; j++)
                        {
                            paciente nuevo = TablasHashPacientes.HashTable[i].ObtenerValor(j);
                            ListaPacientes.InsertarFinal(nuevo);
                        }
                    }
                }
                //Agregado Datos del paciente a un paciente nuevo recien creado
                paciente PacienteAgregar = new paciente();
                PacienteAgregar.Nombre = Nombre;
                PacienteAgregar.Apellido = Apellido;
                PacienteAgregar.DPI_CUI = DPI_CUI;
                PacienteAgregar.Departamento = Departamento;
                PacienteAgregar.Municipio_residencia = Municipio_residencia;
                PacienteAgregar.Edad = Edad;
                PacienteAgregar.Vacunado = Vacunado;
                PacienteAgregar.Prioridad = Prioridad;

                //Se adjunta el paciente a la Lista doble
                ListaPacientes.InsertarFinal(PacienteAgregar);

                //Volver a escrivir el CSV para mantener guardad y actualizada la informacion
                using (StreamWriter sw = new StreamWriter(FileName))
                {
                    sw.WriteLine("Nombre,Apellido,DPI_CUI,Departamento,Municipio_residencia,Edad,Vacunado,Prioridad");
                    for (int i = 0; i < ListaPacientes.contador; i++)
                    {
                        sw.WriteLine(JuntarString(ListaPacientes.ObtenerValor(i)));
                    }
                }

                return View("Registro",ListaPacientes);
            }
            return View();
        }
        
        //Busquedas -------------------------------------------------------------
        public IActionResult Buscar([FromServices] IHostingEnvironment HostEnvi)
        {
            string Centro = HttpContext.Session.GetString(HttpContext.Session.Id + "Centro");
            var FileName = $"{HostEnvi.WebRootPath}{RutaPacientes}{Regex.Replace(Centro, @"\s", "")}\\Pacientes.csv";

            using (var lector = new StreamReader(FileName))
            using (var CSV = new CsvReader(lector, CultureInfo.InvariantCulture))
            {
                CSV.Read();
                CSV.ReadHeader();
                while (CSV.Read())
                {
                    var Paciente = CSV.GetRecord<ArbolAVL.PacienteArbol>();
                    Singleton.Instance.ArbolPacientesNombres.InsertarNombres(Paciente);
                    Singleton.Instance.ArbolPacientesApellidos.InsertarApellidos(Paciente);
                    Singleton.Instance.ArbolPacientesDPI.InsertarValor(Paciente);
                }
            }
            
            return View();
        }

        public IActionResult BuscarNombre(string Nombre, [FromServices] IHostingEnvironment HostEnvi)
        {
            ArbolAVL.PacienteArbol Buscado = Singleton.Instance.ArbolPacientesNombres.Buscar(Nombre);
            return View("Buscar", Buscado);
        }
        public IActionResult BuscarApellido(string Apellido)
        {
            ArbolAVL.PacienteArbol Buscado = Singleton.Instance.ArbolPacientesApellidos.BuscarA(Apellido);
            return View("Buscar", Buscado);
        }
        public IActionResult BuscarDPIoCUI(long DPI_CUI)
        {
            ArbolAVL.PacienteArbol Buscado = Singleton.Instance.ArbolPacientesDPI.BuscarNumero(DPI_CUI);
            return View("Buscar", Buscado);
        }
        //-----------------------------------------------------------------------


        [HttpGet]//Edicieones
        public IActionResult Editar([FromServices] IHostingEnvironment HostEnvi)
        {
            ViewBag.variable = false;
            string Centro = HttpContext.Session.GetString(HttpContext.Session.Id + "Centro");
            var FileName = $"{HostEnvi.WebRootPath}{RutaPacientes}{Regex.Replace(Centro, @"\s", "")}\\Pacientes.csv";

            ListaDoble<paciente> ListaPacientes = new ListaDoble<paciente>();
            using (var lector = new StreamReader(FileName))
            using (var CSV = new CsvReader(lector, CultureInfo.InvariantCulture))
            {
                THash<paciente> TablasHashPacientes = new THash<paciente>();
                CSV.Read();
                CSV.ReadHeader();
                while (CSV.Read())
                {
                    var Paciente = CSV.GetRecord<paciente>();
                    TablasHashPacientes.Insertar(Paciente, Paciente.Prioridad);
                }

                for (int i = 0; i < 3; i++)
                {

                    for (int j = 0; j < TablasHashPacientes.HashTable[i].contador; j++)
                    {
                        paciente nuevo = TablasHashPacientes.HashTable[i].ObtenerValor(j);
                        ListaPacientes.InsertarFinal(nuevo);
                    }
                }
            }
            return View(ListaPacientes);
        }
        [HttpGet]
        public IActionResult Editar2(long KeyValue, [FromServices] IHostingEnvironment HostEnvi)
        {
            HttpContext.Session.SetString(HttpContext.Session.Id + "KeyValue", Convert.ToString(KeyValue));
            ViewBag.variable = true;
            string Centro = HttpContext.Session.GetString(HttpContext.Session.Id + "Centro");
            var FileName = $"{HostEnvi.WebRootPath}{RutaPacientes}{Regex.Replace(Centro, @"\s", "")}\\Pacientes.csv";

            ListaDoble<paciente> ListaPacientes = new ListaDoble<paciente>();
            using (var lector = new StreamReader(FileName))
            using (var CSV = new CsvReader(lector, CultureInfo.InvariantCulture))
            {
                CSV.Read();
                CSV.ReadHeader();
                while (CSV.Read())
                {
                    var Paciente = CSV.GetRecord<paciente>();
                    ListaPacientes.InsertarFinal(Paciente);
                }
            }
            return View("Editar", ListaPacientes);
        }
        [HttpPost]
        public RedirectResult Editar2([FromServices] IHostingEnvironment HostEnvi, string Nombre, string Apellido,int Prioridad , long DPI_CUI, string Departamento, string Municipio_residencia, int Edad, bool Vacunado)
        {
            string Centro = HttpContext.Session.GetString(HttpContext.Session.Id + "Centro");
            long DPI = Convert.ToInt64(HttpContext.Session.GetString(HttpContext.Session.Id + "KeyValue"));
            var FileName = $"{HostEnvi.WebRootPath}{RutaPacientes}{Regex.Replace(Centro, @"\s", "")}\\Pacientes.csv";

            ListaDoble<paciente> ListaPacientes = new ListaDoble<paciente>();
            using (var lector = new StreamReader(FileName))
            using (var CSV = new CsvReader(lector, CultureInfo.InvariantCulture))
            {
                CSV.Read();
                CSV.ReadHeader();
                while (CSV.Read())
                {
                    var Paciente = CSV.GetRecord<paciente>();
                    if (Paciente.DPI_CUI == DPI)
                    {
                        Paciente.Nombre = RetornarActualizacion(Paciente.Nombre, Nombre);
                        Paciente.Apellido = RetornarActualizacion(Paciente.Apellido, Apellido);
                        Paciente.Departamento = RetornarActualizacion(Paciente.Departamento, Departamento);
                        Paciente.Municipio_residencia = RetornarActualizacion(Paciente.Municipio_residencia, Municipio_residencia);
                        if (Edad != 0)
                            Paciente.Edad = Edad;
                        if (Paciente.Vacunado != Vacunado)
                            Paciente.Vacunado = Vacunado;
                        if (DPI_CUI != 0)
                            Paciente.DPI_CUI = DPI_CUI;
                        if (Prioridad != 0)
                            Paciente.Prioridad = Prioridad;
                    }
                    ListaPacientes.InsertarFinal(Paciente);
                }
            }

            using (StreamWriter sw = new StreamWriter(FileName))
            {
                sw.WriteLine("Nombre,Apellido,DPI_CUI,Departamento,Municipio_residencia,Edad,Vacunado,Prioridad");
                for (int i = 0; i < ListaPacientes.contador; i++)
                {
                    sw.WriteLine(JuntarString(ListaPacientes.ObtenerValor(i)));
                }
            }

            return Redirect("Editar");
        }


        public IActionResult Espera([FromServices] IHostingEnvironment HostEnvi)
        {
            string Centro = HttpContext.Session.GetString(HttpContext.Session.Id + "Centro");
            var FileName = $"{HostEnvi.WebRootPath}{RutaCentros}{Regex.Replace(Centro, @"\s", "")}\\Cita.csv";


            FileInfo Myfile = new FileInfo(FileName);
            if (Myfile.Exists)
            {
                THash<Citas> TablasHashPacientes = new THash<Citas>();
                using (var lector = new StreamReader(FileName))
                using (var CSV = new CsvReader(lector, CultureInfo.InvariantCulture))
                {
                    CSV.Read();
                    CSV.ReadHeader();
                    while (CSV.Read())
                    {
                        var Cita = CSV.GetRecord<Citas>();
                        TablasHashPacientes.Insertar(Cita, Cita.Prioridad);
                    }
                }

                ListaDoble<Citas> ListaPacientes = new ListaDoble<Citas>();
                for (int i = 0; i < 3; i++)
                {

                    for (int j = 0; j < TablasHashPacientes.HashTable[i].contador; j++)
                    {
                        Citas nuevo = TablasHashPacientes.HashTable[i].ObtenerValor(j);
                        ListaPacientes.InsertarFinal(nuevo);
                    }
                }
                return View("Espera", ListaPacientes);
            }
            else
            {
                Directory.CreateDirectory($"{HostEnvi.WebRootPath}{RutaCentros}{Regex.Replace(Centro, @"\s", "")}");
                using (StreamWriter sw = new StreamWriter(FileName))
                {
                    sw.WriteLine("Nombre,Apellido,DPI_CUI,Edad,Prioridad,fecha");
                }
            }

            return View("Registro");
        }
        [HttpGet]
        public IActionResult Agendar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Agendar([FromServices] IHostingEnvironment HostEnvi, string Nombre, string Apellido, long DPI_CUI, int Edad, int Prioridad, string fecha)
        {
            string Centro = HttpContext.Session.GetString(HttpContext.Session.Id + "Centro");
            var FileName = $"{HostEnvi.WebRootPath}{RutaCentros}{Regex.Replace(Centro, @"\s", "")}\\Cita.csv";

            ListaDoble<Citas> ListaPacientes = new ListaDoble<Citas>();
            using (var lector = new StreamReader(FileName))
            using (var CSV = new CsvReader(lector, CultureInfo.InvariantCulture))
            {
                THash<Citas> TablasHashPacientes = new THash<Citas>();
                CSV.Read();
                CSV.ReadHeader();
                while (CSV.Read())
                {
                    var Paciente = CSV.GetRecord<Citas>();
                    TablasHashPacientes.Insertar(Paciente, Paciente.Prioridad);
                }



                for (int i = 0; i < 3; i++)
                {

                    for (int j = 0; j < TablasHashPacientes.HashTable[i].contador; j++)
                    {
                        Citas nuevo = TablasHashPacientes.HashTable[i].ObtenerValor(j);
                        ListaPacientes.InsertarFinal(nuevo);
                    }
                }
            }
            //Agregado Datos del paciente a un paciente nuevo recien creado
            Citas PacienteAgregar = new Citas();
            PacienteAgregar.Nombre = Nombre;
            PacienteAgregar.Apellido = Apellido;
            PacienteAgregar.DPI_CUI = DPI_CUI;
            PacienteAgregar.Edad = Edad;
            PacienteAgregar.fecha = fecha;
            PacienteAgregar.Prioridad = Prioridad;

            //Se adjunta el paciente a la Lista doble
            ListaPacientes.InsertarFinal(PacienteAgregar);

            //Volver a escrivir el CSV para mantener guardad y actualizada la informacion
            using (StreamWriter sw = new StreamWriter(FileName))
            {
                sw.WriteLine("Nombre,Apellido,DPI_CUI,Edad,Prioridad,fecha");
                for (int i = 0; i < ListaPacientes.contador; i++)
                {
                    Citas reorganizar = ListaPacientes.ObtenerValor(i);
                    string Retornar = reorganizar.Nombre;
                    Retornar += "," + reorganizar.Apellido;
                    Retornar += "," + Convert.ToString(reorganizar.DPI_CUI);
                    Retornar += "," + Convert.ToString(reorganizar.Edad);
                    Retornar += "," + Convert.ToString(reorganizar.Prioridad);
                    Retornar += "," + reorganizar.fecha;
                    sw.WriteLine(Retornar);
                }
            }

            return View("Agendar", ListaPacientes);
        }
            public IActionResult Privacy()
        {
            return View();
        }

        //-----------------------------Fuenciones y procedimientos complementarios--------------
        string JuntarString(paciente Paciente)
        {
            string Retornar = Paciente.Nombre;
            Retornar += "," + Paciente.Apellido;
            Retornar += "," + Convert.ToString(Paciente.DPI_CUI);
            Retornar += "," + Paciente.Departamento;
            Retornar += "," + Paciente.Municipio_residencia;
            Retornar += "," + Convert.ToString(Paciente.Edad);
            Retornar += "," + Convert.ToString(Paciente.Vacunado);
            Retornar += "," + Convert.ToString(Paciente.Prioridad);

            return Retornar;
        }

        string RetornarActualizacion(string Retornar, string Cambiar)
        {
            if(Cambiar == null)
            {
                return Retornar;
            }
            return Cambiar;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
