namespace ExamenBanco.Models
{
    public class Usuario
    {
        //Los campos estan en ingles por cuestion de resumen de nombre
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public decimal Balance { get; set; }
    }
}
