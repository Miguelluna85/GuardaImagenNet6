using System;
using System.Collections.Generic;

namespace GuardaImagenNet6.Models
{
    public partial class Usuario
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FotoPath { get; set; }
        public bool? Estatus { get; set; }
        public DateTime FechaAlta { get; set; }
        public int IdUsuarioAlta { get; set; }
        public DateTime FechaModifico { get; set; }
        public int IdUsuarioModifico { get; set; }
        public byte[] FotoBd { get; set; }
        public string FotoName { get; set; }
    }
}
