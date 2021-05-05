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
using ArbolAVL;
using ListaDobleEnlace;
using THash;
using CsvHelper;
using System.IO;
using System.Globalization;

namespace Proyecto_Estructuras.Controllers
{
    public class HomeController : Controller
    {
        const string RutaUsuario = "\\files\\Users\\Users.csv";
        const string RutaCentros = "\\files\\InformacionCentro\\";
        const string RutaPacientes = "\\files\\InformacionPacientes\\Pacientes.csv";

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
                var fileName = $"{HostEnvi.WebRootPath}" + RutaUsuario;
                using (var reader = new StreamReader(fileName))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Read();
                    csv.ReadHeader();
                    while (csv.Read())
                    {
                        var Usuario = csv.GetRecord<Usuarios>();
                        ListaUsuarios.InsertarInicio(Usuario);
                        if(Usuario.Password == Login.Password && Login.User == Usuario.User)
                        {
                            HttpContext.Session.SetString((HttpContext.Session.Id + "Centro"), Usuario.Centro);
                            //De aca deberia retornar a la view de la interfaz de usuario
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
        [HttpPost]
        public IActionResult CreateNewUser(string User, string Password, string Centro, string AUser, string APassword) 
        {
            if(User != null && Password != null && Centro != null && AUser != null && APassword != null)
            {
                //Pendiente por codigo
            }
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        //-----------------------------Fuenciones y procedimientos complementarios--------------



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
