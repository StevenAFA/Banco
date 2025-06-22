namespace ExamenBanco.Models
{
    public class Usuario
    {
        // Los campos están en inglés por cuestión de resumen de nombre
        public int Id { get; set; } // Identificador único del usuario
        public string Username { get; set; } // Nombre de usuario (username)
        public string Password { get; set; } // Contraseña del usuario
        public decimal Balance { get; set; } // Balance o saldo actual del usuario
    }
}
