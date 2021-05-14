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

        Arbol.Arbol<Prioridad> Heap = new Arbol.Arbol<Prioridad>();
        const string RutaUsuario = "\\files\\Users\\Users.csv";
        const string RutaCentros = "\\files\\InformacionCentro\\";
        const string RutaPacientes = "\\files\\InformacionPacientes\\";
        const string ListasEspera = "\\files\\ListasEspera\\";

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
                            HttpContext.Session.SetString((HttpContext.Session.Id + "Departamento"), Usuario.Departamento);
                            GuardarInfoCitas(HostEnvi);
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
                                        TablasHashPacientes.Insertar(Paciente, Paciente.DPI_CUI.ToString());
                                    }
                                }

                                ListaDoble<paciente> ListaPacientes = new ListaDoble<paciente>();
                                Prioridad DatosPrioridad = new Prioridad();
                                ColaPrioridad Cola = new ColaPrioridad();
                                for (int i = 0; i < 3; i++)
                                {
                                    for (int j = 0; j < TablasHashPacientes.HashTable[i].contador; j++)
                                    {
                                        paciente nuevo = TablasHashPacientes.HashTable[i].ObtenerValor(j);
                                        Prioridad Paciente = new Prioridad();
                                        Paciente.prioridad = nuevo.Prioridad;
                                        Paciente.Cui = nuevo.DPI_CUI.ToString();
                                        Cola.Insertar(Paciente);
                                    }
                                }
                                for (int i = 0; i < Cola.ObtenerCola().contador; i++)
                                {
                                    OrdenarPrioridad(Cola.ObtenerCola().ObtenerValor(i).valor.Cui, Cola.ObtenerCola().ObtenerValor(i).valor.prioridad, i + 1);
                                }
                                LlenarLista(ListaPacientes, Heap, TablasHashPacientes);
                                return View("Registro", ListaPacientes);
                            }
                            else
                            {
                                Directory.CreateDirectory($"{HostEnvi.WebRootPath}{RutaPacientes}{Regex.Replace(Centro, @"\s", "")}");
                                using (StreamWriter sw = new StreamWriter(FileName))
                                {
                                    sw.WriteLine("Nombre,Apellido,DPI_CUI,Departamento,Municipio_residencia,Edad,Vacunado,Prioridad,Grupo,Enfermedad");
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
        public IActionResult CreateNewUser(string User, string Password, string Centro, string AUser, string APassword, String Departamento, [FromServices] IHostingEnvironment HostEnvi) 
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
                            NewUser.Departamento = Departamento;
                            NewUser.Centro = Centro;
                            NewUser.User = User;
                            NewUser.Password = Password;

                            ListaUsuarios.InsertarFinal(NewUser);
                        }
                    }
                }
                using (StreamWriter sw = new StreamWriter(fileName))
                {
                    sw.WriteLine("User,Password,Centro,Departamento");
                    foreach (Usuarios Usuario in ListaUsuarios)
                    {
                        sw.WriteLine(Usuario.User + "," + Usuario.Password + "," + Usuario.Centro + ","  + Usuario.Departamento);
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
                    TablasHashPacientes.Insertar(Paciente, Paciente.DPI_CUI.ToString());
                }


                Prioridad DatosPrioridad = new Prioridad();
                ColaPrioridad Cola = new ColaPrioridad();
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < TablasHashPacientes.HashTable[i].contador; j++)
                    {
                        paciente nuevo = TablasHashPacientes.HashTable[i].ObtenerValor(j);
                        Prioridad Paciente = new Prioridad();
                        Paciente.prioridad = nuevo.Prioridad;
                        Paciente.Cui = nuevo.DPI_CUI.ToString();
                        Cola.Insertar(Paciente);
                    }
                }
                for (int i = 0; i < Cola.ObtenerCola().contador; i++)
                {
                    OrdenarPrioridad(Cola.ObtenerCola().ObtenerValor(i).valor.Cui, Cola.ObtenerCola().ObtenerValor(i).valor.prioridad, i + 1);
                }
                LlenarLista(ListaPacientes, Heap, TablasHashPacientes);
            }
            return View(ListaPacientes);
        }

        [HttpGet]
        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registrar([FromServices] IHostingEnvironment HostEnvi, string Nombre, string Apellido, long DPI_CUI, int Departamento, string Municipio_residencia, int Edad, bool Vacunado, string Grupo, paciente model)
        {
            THash<paciente> TablasHashPacientes = new THash<paciente>();
            if (Nombre != null && Apellido != null &&  ValidarCui(DPI_CUI) && Departamento != 0 && Municipio_residencia != null && Edad > 17)
            {
                string Centro = HttpContext.Session.GetString(HttpContext.Session.Id + "Centro");
                var FileName = $"{HostEnvi.WebRootPath}{RutaPacientes}{Regex.Replace(Centro, @"\s", "")}\\Pacientes.csv";

                ListaDoble<paciente> ListaPacientes = new ListaDoble<paciente>();
                using (var lector = new StreamReader(FileName))
                using (var CSV = new CsvReader(lector, CultureInfo.InvariantCulture))
                {
                    // Pasa del archivo csv a tabla hash
                    CSV.Read();
                    CSV.ReadHeader();
                    while (CSV.Read())
                    {
                        var Paciente = CSV.GetRecord<paciente>();
                        TablasHashPacientes.Insertar(Paciente, Paciente.DPI_CUI.ToString());
                    }

                    Prioridad DatosPrioridad = new Prioridad();
                    ColaPrioridad Cola = new ColaPrioridad();
                    for (int i = 0; i < 3; i++)
                    {
                        // Pasa datos de tabla hash a lista doble
                        for (int j = 0; j < TablasHashPacientes.HashTable[i].contador; j++)
                        {
                            paciente nuevo = TablasHashPacientes.HashTable[i].ObtenerValor(j);
                            Prioridad Paciente = new Prioridad();
                            Paciente.prioridad = nuevo.Prioridad;
                            Paciente.Cui = nuevo.DPI_CUI.ToString();
                            Cola.Insertar(Paciente);
                            Singleton.Instance.ListCui.InsertarInicio(nuevo.DPI_CUI);
                        }
                    }
                    for (int i = 0; i < Cola.ObtenerCola().contador; i++)
                    {
                        OrdenarPrioridad(Cola.ObtenerCola().ObtenerValor(i).valor.Cui, Cola.ObtenerCola().ObtenerValor(i).valor.prioridad, i + 1);
                    }
                }
                bool enfermedad;

                //Agregado Datos del paciente a un paciente nuevo recien creado
                paciente PacienteAgregar = new paciente();
                PacienteAgregar.Nombre = Nombre;
                PacienteAgregar.Apellido = Apellido;
                PacienteAgregar.DPI_CUI = DPI_CUI;
                PacienteAgregar.Departamento = DefinirDept(Departamento);
                PacienteAgregar.Municipio_residencia = Municipio_residencia;
                PacienteAgregar.Edad = Edad;
                PacienteAgregar.Vacunado = Vacunado;
                PacienteAgregar.Grupo = Grupo;
                if (model.Enfermedad == "Sí")
                {
                    enfermedad = true;
                    PacienteAgregar.Enfermedad = "Aplica";
                }
                else
                {
                    enfermedad = false;
                    PacienteAgregar.Enfermedad = "No Aplica";
                }

                PacienteAgregar.Prioridad = DefinirPrioridad(Convert.ToInt32(Grupo), Edad, enfermedad);
                TablasHashPacientes.Insertar(PacienteAgregar, PacienteAgregar.DPI_CUI.ToString());

                // Ordenar según prioridad
                ColaPrioridad ColaHeap = new ColaPrioridad();
                ColaHeap.InsertarConArbol(Heap);
                Prioridad paciete = new Prioridad();
                paciete.Cui = PacienteAgregar.DPI_CUI.ToString();
                paciete.prioridad = PacienteAgregar.Prioridad;
                ColaHeap.Insertar(paciete);
                OrdenarPrioridad(PacienteAgregar.DPI_CUI.ToString(), PacienteAgregar.Prioridad, ColaHeap.Buscar(paciete) + 1);

                // Se adjunta paciente a la lista doble
                LlenarLista(ListaPacientes, Heap, TablasHashPacientes);

                // Crear cita al paciente y guardar dentro del csv cita
                CrearCita(ListaPacientes, HostEnvi);

                //Volver a escrivir el CSV para mantener guardad y actualizada la informacion
                using (StreamWriter sw = new StreamWriter(FileName))
                {
                    sw.WriteLine("Nombre,Apellido,DPI_CUI,Departamento,Municipio_residencia,Edad,Vacunado,Prioridad,Grupo,Enfermedad");
                    for (int i = 0; i < ListaPacientes.contador; i++)
                    {
                        sw.WriteLine(JuntarString(ListaPacientes.ObtenerValor(i)));
                    }
                }

                return View("Registro",ListaPacientes);
            }
            ViewBag.Mensaje = "Datos inválidos, intente nuevamente";
            return View();
        }

        //Busquedas -------------------------------------------------------------
        public IActionResult Buscar([FromServices] IHostingEnvironment HostEnvi)
        {
            Singleton.Instance.TablaHashBuscarPacientes.Lista0.Vaciar();
            Singleton.Instance.TablaHashBuscarPacientes.Lista1.Vaciar();
            Singleton.Instance.TablaHashBuscarPacientes.Lista2.Vaciar();
            Singleton.Instance.ArbolPacientesNombres.Delete();
            Singleton.Instance.ArbolPacientesApellidos.Delete();
            Singleton.Instance.ArbolPacientesDPI.Delete();

            string Centro = HttpContext.Session.GetString(HttpContext.Session.Id + "Centro");
            var FileName = $"{HostEnvi.WebRootPath}{RutaPacientes}{Regex.Replace(Centro, @"\s", "")}\\Pacientes.csv";

            using (var lector = new StreamReader(FileName))
            using (var CSV = new CsvReader(lector, CultureInfo.InvariantCulture))
            {
                CSV.Read();
                CSV.ReadHeader();
                while (CSV.Read())
                {
                    var Paciente = CSV.GetRecord<paciente>();
                    Singleton.Instance.TablaHashBuscarPacientes.Insertar(Paciente, Paciente.DPI_CUI.ToString());

                }
                for (int i = 0; i < 3; i++)
                {
                    // Pasa datos de tabla hash a lista doble
                    for (int j = 0; j < Singleton.Instance.TablaHashBuscarPacientes.HashTable[i].contador; j++)
                    {
                        paciente nuevo = Singleton.Instance.TablaHashBuscarPacientes.HashTable[i].ObtenerValor(j);
                        ArbolAVL.PacienteArbol ValorIndice = new ArbolAVL.PacienteArbol();

                        ValorIndice.Nombre = nuevo.Nombre;
                        ValorIndice.Apellido = nuevo.Apellido;
                        ValorIndice.DPI_CUI = nuevo.DPI_CUI;


                        Singleton.Instance.ArbolPacientesNombres.InsertarNombres(ValorIndice, Singleton.Instance.ArbolPacientesNombres);
                        Singleton.Instance.ArbolPacientesApellidos.InsertarApellidos(ValorIndice, Singleton.Instance.ArbolPacientesApellidos);
                        Singleton.Instance.ArbolPacientesDPI.InsertarNombres(ValorIndice, Singleton.Instance.ArbolPacientesDPI);
                    }
                }
            }
            return View();
        }

        public IActionResult BuscarNombre(string Nombre, [FromServices] IHostingEnvironment HostEnvi)
        {
            paciente Buscado = new paciente();
            ArbolAVL.PacienteArbol info = Singleton.Instance.ArbolPacientesNombres.RetornarValor(Singleton.Instance.ArbolPacientesNombres, Nombre, Singleton.Instance.ArbolPacientesNombres.Buscar);
            if(info != null)
            {
                Buscado = ObtenerValor(Singleton.Instance.TablaHashBuscarPacientes, Nombre, Singleton.Instance.TablaHashBuscarPacientes.Llave(info.DPI_CUI.ToString()));
                return View("Buscar", Buscado);
            }
            else
            {
                ViewBag.Mensaje = "Paciente no encontrado";
                return View("Buscar");
            }

        }
        public IActionResult BuscarApellido(string Apellido)
        {
            paciente Buscado = new paciente();
            ArbolAVL.PacienteArbol info = Singleton.Instance.ArbolPacientesApellidos.RetornarValor(Singleton.Instance.ArbolPacientesApellidos, Apellido, Singleton.Instance.ArbolPacientesApellidos.BuscarA);
            if(info != null)
            {
                Buscado = ObtenerValor(Singleton.Instance.TablaHashBuscarPacientes, Apellido, Singleton.Instance.TablaHashBuscarPacientes.Llave(info.DPI_CUI.ToString()));
                return View("Buscar", Buscado);
            }
            else
            {
                ViewBag.Mensaje = "Paciente no encontrado";
                return View("Buscar");
            }
        }
        public IActionResult BuscarDPIoCUI(long DPI_CUI)
        {
            paciente Buscado = new paciente();
            ArbolAVL.PacienteArbol info = Singleton.Instance.ArbolPacientesDPI.RetornarValor(Singleton.Instance.ArbolPacientesDPI, DPI_CUI.ToString(), Singleton.Instance.ArbolPacientesDPI.BuscarNumero);
            if(info != null)
            {
                Buscado = ObtenerValor(Singleton.Instance.TablaHashBuscarPacientes, DPI_CUI.ToString(), Singleton.Instance.TablaHashBuscarPacientes.Llave(info.DPI_CUI.ToString()));
                return View("Buscar", Buscado);
            }
            else
            {
                ViewBag.Mensaje = "Paciente no encontrado";
                return View("Buscar");
            }
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
                    TablasHashPacientes.Insertar(Paciente, Paciente.DPI_CUI.ToString());
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
                sw.WriteLine("Nombre,Apellido,DPI_CUI,Departamento,Municipio_residencia,Edad,Vacunado,Prioridad,Grupo,Enfermedad");
                for (int i = 0; i < ListaPacientes.contador; i++)
                {
                    sw.WriteLine(JuntarString(ListaPacientes.ObtenerValor(i)));
                }
            }

            return Redirect("Editar");
        }

        // Muestra la cola de prioridad, se podrá elegir la opción de vacunado o reagendar cita
        public IActionResult Espera()
        {
            return View("Espera", Singleton.Instance.ListadoCitas);
        }

        // Permite guardar los datos de vacunación para el paciente 
        [HttpPost]
        public IActionResult Vacunacion()
        {
            return View();
        }

        public IActionResult Reagendar()
        {
            return View("Espera");
        }

        [HttpGet]
        public IActionResult Agendar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Agendar([FromServices] IHostingEnvironment HostEnvi, string Nombre, string Apellido, long DPI_CUI, int Edad, string Grupo, paciente model)
        {
            string fecha = "";
            bool enfermedad;
            if (model.Enfermedad == "Sí")
                enfermedad = true;
            else
                enfermedad = false;
            int Prioridad = DefinirPrioridad(Convert.ToInt32(Grupo), Edad, enfermedad);
            string Centro = HttpContext.Session.GetString(HttpContext.Session.Id + "Centro");
            var FileName = $"{HostEnvi.WebRootPath}{RutaCentros}{Regex.Replace(Centro, @"\s", "")}\\Cita.csv";

            ListaDoble<Citas> ListaPacientes = new ListaDoble<Citas>();
            ColaPrioridad cola = new ColaPrioridad();
            using (var lector = new StreamReader(FileName))
            using (var CSV = new CsvReader(lector, CultureInfo.InvariantCulture))
            {
                THash<Citas> TablasHashPacientes = new THash<Citas>();
                CSV.Read();
                CSV.ReadHeader();
                while (CSV.Read())
                {
                    var Paciente = CSV.GetRecord<Citas>();
                    TablasHashPacientes.Insertar(Paciente, Paciente.DPI_CUI.ToString());
                }

                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < TablasHashPacientes.HashTable[i].contador; j++)
                    {
                        Citas nuevo = TablasHashPacientes.HashTable[i].ObtenerValor(j);
                        ListaPacientes.InsertarFinal(nuevo);
                        Prioridad NuevoenCola = new Prioridad();
                        NuevoenCola.prioridad = nuevo.Prioridad;
                        NuevoenCola.Cui = nuevo.DPI_CUI.ToString();
                        cola.Insertar(NuevoenCola);
                    }
                }
            }
            for (int i = 0; i < cola.ObtenerCola().contador; i++)
            {

            }
            fecha = DateTime.Now.AddDays(31).ToShortDateString();
            //Agregado Datos del paciente a un paciente nuevo recien creado
            Citas PacienteAgregar = new Citas();
            PacienteAgregar.Nombre = Nombre;
            PacienteAgregar.Apellido = Apellido;
            PacienteAgregar.DPI_CUI = DPI_CUI;
            PacienteAgregar.Edad = Edad;
            PacienteAgregar.Fecha = fecha;
            PacienteAgregar.Prioridad = Prioridad;


            //Se adjunta el paciente a la Lista doble
            ListaPacientes.InsertarFinal(PacienteAgregar);

            //Volver a escribir el CSV para mantener guardad y actualizada la informacion
            using (StreamWriter sw = new StreamWriter(FileName))
            {
                sw.WriteLine("Nombre,Apellido,DPI_CUI,Edad,Prioridad,fecha,Hora,MarcaVacuna,Dosis");
                for (int i = 0; i < ListaPacientes.contador; i++)
                {
                    Citas reorganizar = ListaPacientes.ObtenerValor(i);
                    string Retornar = reorganizar.Nombre;
                    Retornar += "," + reorganizar.Apellido;
                    Retornar += "," + Convert.ToString(reorganizar.DPI_CUI);
                    Retornar += "," + Convert.ToString(reorganizar.Edad);
                    Retornar += "," + Convert.ToString(reorganizar.Prioridad);
                    Retornar += "," + reorganizar.Fecha;
                    Retornar += "," + reorganizar.Hora;
                    Retornar += ",";
                    Retornar += ",";
                    sw.WriteLine(Retornar);
                }
            }

            return View("Espera", ListaPacientes);
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
            Retornar += "," + Paciente.Grupo;
            Retornar += "," + Paciente.Enfermedad;

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

        bool ValidarCui(long cui)
        {
            int contador = 0;
            string extension = cui.ToString();
            if (extension.Length == 13)
            {
                if(Singleton.Instance.ListCui.contador == 0)
                {
                    Singleton.Instance.ListCui.InsertarInicio(cui);
                    return true;
                }
                else
                {
                    for (int i = 0; i < Singleton.Instance.ListCui.contador; i++)
                    {
                        if(Singleton.Instance.ListCui.ObtenerValor(i) != cui)
                        {
                            contador++;
                        }
                    }
                }
            }
            else
            {
                return false;
            }
            if(contador == Singleton.Instance.ListCui.contador)
            {
                Singleton.Instance.ListCui.InsertarInicio(cui);
                return true;
            }
            else
            {
                return false;
            }
        }

        int DefinirPrioridad(int Grupo, int edad, bool enfermedad)
        {
            if (Grupo >= 1 && Grupo <= 6)
            {
                return 1;
            }
            if (edad > 50 || (edad >= 18 && enfermedad == true) || Grupo == 7)
            {
                return 2;
            }
            if (Grupo >= 8 && Grupo <= 15)
            {
                return 3;
            }
            if (Grupo == 16 && (edad >= 18 && edad <= 49 && enfermedad == false))
            {
                return 4;
            }
            else
            {
                return 0;
            }
        }

        string DefinirDept(int Departamento)
        {
            switch (Departamento)
            {
                case 1: return "Alta Verapaz";
                case 2: return "Baja Verapaz";
                case 3: return "Chimaltenango";
                case 4: return "Chiquimula";
                case 5: return "El Progreso";
                case 6: return "Escuintla";
                case 7: return "Guatemala";
                case 8: return "Huehuetenango";
                case 9: return "Izabal";
                case 10: return "Jalapa";
                case 11: return "Jutiapa";
                case 12: return "Petén";
                case 13: return "Quetzaltenango";
                case 14: return "Quiché";
                case 15: return "Retalhuleu";
                case 16: return "Sacátepequez";
                case 17: return "San Marcos";
                case 18: return "Santa Rosa";
                case 19: return "Sololá";
                case 20: return "Suchitepéquez";
                case 21: return "Totonicapán";
                case 22: return "Zacapa";
                default:
                    break;
            }
            return "";
        }

        void OrdenarPrioridad(string cui, int prioridad, int indice)
        {
            Prioridad dato = new Prioridad();
            dato.prioridad = prioridad;
            dato.Cui = cui;

            Heap.Insertar(dato, indice);
        }

        void LlenarLista(ListaDoble<paciente> ListaDoble, Arbol.Arbol<Prioridad> Heap, THash<paciente> THash)
        {
            Arbol.Nodo<Prioridad> valorPrioridad = new Arbol.Nodo<Prioridad>();
            paciente infoPaciente = new paciente();
            ColaPrioridad Cola = new ColaPrioridad();

            Cola.InsertarConArbol(Heap);
            for (int i = 0; i < Cola.ObtenerCola().contador; i++)
            {
                valorPrioridad = Cola.ObtenerCola().ObtenerValor(i);
                infoPaciente = ObtenerValor(THash, valorPrioridad.valor.Cui, THash.Llave(valorPrioridad.valor.Cui.ToString()));
                ListaDoble.InsertarFinal(infoPaciente);
            }
        }

        paciente ObtenerValor(THash<paciente> Hash, string dato, int llave)
        {
            paciente valor = new paciente();
            bool encontar = false;
            switch (llave)
            {
                case 0:
                    for (int i = 0; i < Hash.HashTable[0].contador; i++)
                    {
                        valor = Hash.HashTable[0].ObtenerValor(i);
                        if (valor.DPI_CUI.ToString().ToLower().Replace(" ", "") == dato || valor.Nombre.ToLower().Replace(" ","") == dato || valor.Apellido.ToLower().Replace(" ","") == dato)
                        {
                            return valor;
                        }
                    }
                    break;
                case 1:
                    for (int i = 0; i < Hash.HashTable[1].contador; i++)
                    {
                        valor = Hash.HashTable[1].ObtenerValor(i);
                        if (valor.DPI_CUI.ToString().ToLower().Replace(" ", "") == dato || valor.Nombre.ToLower().Replace(" ","") == dato || valor.Apellido.ToLower().Replace(" ", "") == dato)
                        {
                            return valor;
                        }
                    }
                    break;
                case 2:
                    for (int i = 0; i < Hash.HashTable[2].contador; i++)
                    {
                        valor = Hash.HashTable[2].ObtenerValor(i);
                        if (valor.DPI_CUI.ToString().ToLower().Replace(" ", "") == dato || valor.Nombre.ToLower().Replace(" ", "") == dato || valor.Apellido.ToLower().Replace(" ", "") == dato)
                        {
                            return valor;
                        }
                    }
                    break; 
                default:
                    break;
            }
            return null;
        }

        void GuardarInfoCitas([FromServices] IHostingEnvironment HostEnvi)
        {
            string Centro = HttpContext.Session.GetString(HttpContext.Session.Id + "Centro");
            var FileName = $"{HostEnvi.WebRootPath}{RutaCentros}{Regex.Replace(Centro, @"\s", "")}\\Cita.csv";

            FileInfo Myfile = new FileInfo(FileName);
            if (Myfile.Exists)
            {
                using (var lector = new StreamReader(FileName))
                using (var CSV = new CsvReader(lector, CultureInfo.InvariantCulture))
                {
                    CSV.Read();
                    CSV.ReadHeader();
                    while (CSV.Read())
                    {
                        // Guarda la info. de las citas por prioridad dentro de una lista
                        var Cita = CSV.GetRecord<Citas>();
                        Singleton.Instance.ListadoCitas.InsertarFinal(Cita);
                    }
                }
            }
            else
            {
                Directory.CreateDirectory($"{HostEnvi.WebRootPath}{RutaCentros}{Regex.Replace(Centro, @"\s", "")}");
                using (StreamWriter sw = new StreamWriter(FileName))
                {
                    sw.WriteLine("Nombre,Apellido,DPI_CUI,Edad,Prioridad,Fecha,Hora");
                }
            }
        }

        // método que asigna el horario de la cita de los pacientes según la prioridad
        void CrearCita(ListaDoble<paciente> ListaPacientes, [FromServices] IHostingEnvironment HostEnvi)
        {
            int contador = 0;
            string Centro = HttpContext.Session.GetString(HttpContext.Session.Id + "Centro");
            var FileName = $"{HostEnvi.WebRootPath}{RutaCentros}{Regex.Replace(Centro, @"\s", "")}\\Cita.csv";

            DateTime fecha = new DateTime(2021, 05, 14, 7, 0, 0);
            TimeSpan duracion = new TimeSpan(0, 15, 0);

            // Se obtiene la información de cada paciente por orden de prioridad y se agrega a la lista de espera en singleton y en csv 
            for (int i = 0; i < ListaPacientes.contador; i++)
            {
                paciente Paciente = new paciente();
                Citas cita = new Citas();

                Paciente = ListaPacientes.ObtenerValor(i);
                cita.Nombre = Paciente.Nombre;
                cita.Apellido = Paciente.Apellido;
                cita.DPI_CUI = Paciente.DPI_CUI;
                cita.Edad = Paciente.Edad;
                cita.Prioridad = Paciente.Prioridad;

                // aumenta 15 minutos a la hora
                if (contador == 3)
                {
                    DateTime aux = fecha.Add(duracion);
                    fecha = aux;
                    contador = 0;
                }
                cita.Fecha = fecha.ToShortDateString();
                cita.Hora = fecha.ToShortTimeString();
                contador++;

                // Se inserta la cita dentro de la lista que se mostrará en la vista 
                Singleton.Instance.ListadoCitas.InsertarFinal(cita);
            }

            // Insertar método para insertar la info de la lista en el csv
            using(StreamWriter sw = new StreamWriter(FileName))
            {
                sw.WriteLine("Nombre,Apellido,DPI_CUI,Edad,Prioridad,Fecha,Hora");
                for (int i = 0; i < Singleton.Instance.ListadoCitas.contador; i++)
                {
                    Citas valor = Singleton.Instance.ListadoCitas.ObtenerValor(i);
                    
                    string Retornar = valor.Nombre;
                    Retornar += "," + valor.Apellido;
                    Retornar += "," + Convert.ToString(valor.DPI_CUI);
                    Retornar += "," + valor.Edad;
                    Retornar += "," + valor.Prioridad;
                    Retornar += "," + valor.Fecha;
                    Retornar += "," + valor.Hora;

                    sw.WriteLine(Retornar);
                }
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
