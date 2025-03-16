namespace ExamenBanco.Models
{
    public class Transaccion
    {
        //Los campos estan en ingles por cuestion de resumen de nombre
        public int Id { get; set; } // ID de transacción único
        public int SenderId { get; set; } // ID del remitente
        public int RecipientId { get; set; } // Identificación del destinatario
        public decimal Amount { get; set; } // Cantidad transferida
        public DateTime TransactionDate { get; set; } = DateTime.Now; // Marca de tiempo de la transacción
    }
}
