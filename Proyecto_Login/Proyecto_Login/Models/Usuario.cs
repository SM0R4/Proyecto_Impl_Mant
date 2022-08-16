using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto_Login.Models
{
    //Creación de la clase usuario con los atributos necesarios
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Correo { get; set; }
        public string Clave { get; set; }
        public string ConfirmarClave { get; set; }
    }
}