using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using static ULACWeb.Models.RegistroModel;
using System.Security.Cryptography;
using System.Text;
using System.Net.Mail;
using System.Data.Entity;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Utilities.Encoders;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace ULACWeb.Models
{
    public class RegistroModel
    {



        [Required(ErrorMessage = "El ID de su empresa es obligatorio")]
        public int IDEmpresa { get; set; }

        [Required(ErrorMessage = "La Identificación es obligatorio")]
        public string Identificacion { get; set; }

        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El Número de Teléfono es obligatorio")]
        public string Telefono { get; set; }
        [Required(ErrorMessage = "El  Correo es obligatorio")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "El Contraseña es obligatorio")]
        public string Contraseña { get; set; }

        [Required(ErrorMessage = "La selección de la pregunta es obligatoria")]
        public string Pregunta1 { get; set; }
        [Required(ErrorMessage = "La selección de la pregunta es obligatoria")]
        public string Pregunta2 { get; set; }
        [Required(ErrorMessage = "La selección de la pregunta es obligatoria")]
        public string Pregunta3 { get; set; }
        [Required(ErrorMessage = "La respuesta de la pregunta es obligatoria")]
        public string Respuesta1 { get; set; }
        [Required(ErrorMessage = "La respuesta de la pregunta es obligatoria")]
        public string Respuesta2 { get; set; }
        [Required(ErrorMessage = "La respuesta de la pregunta es obligatoria")]
        public string Respuesta3 { get; set; }

        public string VerificationToken { get; set; }
        public Guid VerificationUID { get; set; }
        public string MensajeGeneral { get; set; }

        //DATOS PARA EL MAIL
        public string Destinatario { get; set; }
        public string Asunto { get; set; }
        public string Mensaje { get; set; }

        public string AsuntoToken { get; set; }
        public string MensajeToken { get; set; }

        [Required(ErrorMessage = "Elegir el pais es obligatoria")]
        public string NombrePais { get; set; }
        [Required(ErrorMessage = "Elegir la provincia es obligatoria")]
        public string NombreProvincia { get; set; }
        [Required(ErrorMessage = "Elegir el canton es obligatoria")]
        public string NombreCanton { get; set; }
        [Required(ErrorMessage = "Elegir el distrito es obligatoria")]
        public string NombreDistrito { get; set; }

        public class Pais
        {
            public int IDPais { get; set; }
            public string NombrePais { get; set; } 
            public virtual ICollection<Provincia> Provincias { get; set; }
        }

        public class Provincia
        {
            public int IDProvincia { get; set; }
            public int IDPais { get; set; }
            public string NombreProvincia { get; set; }
            public virtual Pais Pais { get; set; }
            public virtual ICollection<Canton> Cantones { get; set; }
        }

        public class Canton
        {
            public int IDCanton { get; set; }
            public string NombreCanton { get; set; }
            public int IDProvincia { get; set; }
            public virtual Provincia Provincia { get; set; }
            public virtual ICollection<Distrito> Distritos { get; set; }
        }

        public class Distrito
        {
            public int IDDistrito { get; set; }
            public string NombreDistrito { get; set; }
            public int IDCanton { get; set; }
            public virtual Canton Canton { get; set; }
        }


    }
}