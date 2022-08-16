using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Proyecto_Login.Models;

namespace Proyecto_Login.Controllers
{
    public class AccesoController : Controller
    {
        //Cadena de conexión a base de datos
        static string cadena = "Data Source=SQL8004.site4now.net;Initial Catalog=db_a8b518_loginsarahmora;User Id=db_a8b518_loginsarahmora_admin;Password=UIA123456";

        //Métodos tipo GET: Acceso
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Registrar()
        {
            return View();
        }

        //Método HttpPOST
        [HttpPost]
        public ActionResult Registrar(Usuario oUsuario)
        {
            bool registrado;
            string mensaje;

            //Verificar si las contraseñas coinciden
            if (oUsuario.Clave == oUsuario.ConfirmarClave){

                //Encriptar la contraseña para guardarla encriptada
                oUsuario.Clave = ConvertirSha256(oUsuario.Clave); 

            }
            else
            {
                //Si no coinciden devolver un msj
                ViewData["Mensaje"] = "Las contraseñas no coinciden";
                return View();
            }

            //Conectar con SQL con la cadena de conexión
            using(SqlConnection cn = new SqlConnection(cadena))
            {
                //Utilizando el proceso almacenado, pasar los atributos del objeto como parámtetros
                SqlCommand cmd = new SqlCommand("sp_RegistrarUsuario", cn);
                cmd.Parameters.AddWithValue("Correo", oUsuario.Correo);
                cmd.Parameters.AddWithValue("Clave", oUsuario.Clave);
                cmd.Parameters.Add("Registrado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("Mensaje", SqlDbType.VarChar,100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                //Abrir conexión
                cn.Open();

                //Ejecutar el proceso
                cmd.ExecuteNonQuery();

                //Obtener valores de los output que devuelve el proceso almacenado
                registrado = Convert.ToBoolean(cmd.Parameters["Registrado"].Value);
                mensaje = cmd.Parameters["Mensaje"].Value.ToString();
            }

            //Almacenar en el View de Mensaje el valor de la variable mensaje
            ViewData["Mensaje"] = mensaje;

            //Si registrado tiene algún valor, rediccionará a la página de Login
            if (registrado)
            {
                return RedirectToAction("Login", "Acceso");
            }
            else
            {
                //Si registrado está vacío devolverá el msj del View
                return View();
            }
        }

        //Método HttpPOST
        [HttpPost]
        public ActionResult Login(Usuario oUsuario)
        {
            //Encriptar la clave ya que en la base de datos está encriptada
            oUsuario.Clave = ConvertirSha256(oUsuario.Clave);

            //Conectarse con la base de datos mediante la cadena de conexión
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                //Pasar los valores de los atributos como parámtros al procedimiento sp_ValidarUsuario
                SqlCommand cmd = new SqlCommand("sp_ValidarUsuario", cn);
                cmd.Parameters.AddWithValue("Correo", oUsuario.Correo);
                cmd.Parameters.AddWithValue("Clave", oUsuario.Clave);
                cmd.CommandType = CommandType.StoredProcedure;

                //Abrir conexión
                cn.Open();

                //Obtener primera fila y primera columna de la consulta del procedimiento
                //y asignar el valor al atributo IdUsuario
                oUsuario.IdUsuario = Convert.ToInt32(cmd.ExecuteScalar().ToString());

            }
            //Si el usuario existe (es diferente de 0), redireccionar al index
            if (oUsuario.IdUsuario != 0)
            {
                Session["usuario"] = oUsuario;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                //Si no existe, devolver mensaje
                ViewData["Mensaje"] = "Usuario no encontrado";
                return View();
            }

        }

        //Método para encriptar en Sha256
        public static string ConvertirSha256(string texto)
        {
            //using System.Text;
            //Usar la referencia de "System.Security.Cryptography"
            StringBuilder Sb = new StringBuilder();
            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(texto));

                foreach (byte b in result)
                    Sb.Append(b.ToString("x2"));
            }
            //Devolver el texto encriptado
            return Sb.ToString();
        }


    }
}