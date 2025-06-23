using ExamenBanco.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExamenBanco.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly Data _context;

        public UsuarioController (Data context)
        {
            _context = context;
        }
        
        // GET: Usuario/Registro
        public IActionResult Registrarse()
        {
            return View();
        }

        // POST: Usuario/Registro
        [HttpPost]
        public IActionResult Registrarse(Usuario user)
        {
            if (ModelState.IsValid)
            {
                _context.Usuarios.Add(user);
                _context.SaveChanges();
                return RedirectToAction("Index", "Home"); // Redirecciona a la pagina despues de registrase
            }
            return View(user);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _context.Usuarios.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                // Store user ID in session
                HttpContext.Session.SetInt32("UserId", user.Id);
                // Redirect to a "Dashboard" page to show the user's balance
                return RedirectToAction("Dashboard", new { id = user.Id });
            }
            else
            {
                ViewBag.Error = "Usuario o Contraseña incorrecta";
                return View();
            }
        }

        public IActionResult Dashboard(int id)
        {
            var user = _context.Usuarios.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        public IActionResult CerrarSesion()
        {
            HttpContext.Session.Clear(); // Clear the session
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Transferencia()
        {
            // Get the logged-in user's ID from the session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login"); // Redirect to login if not logged in
            }

            // Pass the logged-in user's ID to the view (optional, for display purposes)
            ViewBag.SenderId = userId;
            return View();
        }

        [HttpPost]
        public IActionResult Transferencia(int recipientId, decimal amount)
        {
            // Get the logged-in user's ID from the session
            var senderId = HttpContext.Session.GetInt32("UserId");
            if (senderId == null)
            {
                return RedirectToAction("Login"); // Redirect to login if not logged in
            }

            var sender = _context.Usuarios.FirstOrDefault(u => u.Id == senderId);
            var recipient = _context.Usuarios.FirstOrDefault(u => u.Id == recipientId);

            if (sender == null || recipient == null)
            {
                ViewBag.SenderId = senderId;
                ViewBag.Error = "Cuenta de destinatario no válida";
                return View();
            }

            if (sender.Balance < amount)
            {
                ViewBag.SenderId = senderId;
                ViewBag.Error = "Fondos insuficientes";
                return View();
            }

            // Update balances
            sender.Balance -= amount;
            recipient.Balance += amount;

            // Record the transaction
            var transaction = new Transaccion
            {
                SenderId = senderId.Value,
                RecipientId = recipientId,
                Amount = amount
            };
            _context.Transacciones.Add(transaction);

            // Save changes to the database
            _context.SaveChanges();

            ViewBag.Success = $"¡La transferencia de ${amount} a {recipient.Username} fue exitosa!";
            ViewBag.SenderId = senderId; // Pasar el ID del remitente a la vista
            return View();
        }

        public IActionResult HistorialTransaccion(int userId)
        {
            var transactions = _context.Transacciones
                .Where(t => t.SenderId == userId || t.RecipientId == userId)
                .OrderByDescending(t => t.TransactionDate)
                .ToList();
            // Pass the userId to the view
            ViewBag.UserId = userId;
            return View(transactions);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
