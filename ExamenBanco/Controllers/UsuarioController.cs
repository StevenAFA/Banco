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
        // Método para mostrar la vista de registro
        public IActionResult Registrarse()
        {
            return View();
        }

        // POST: Usuario/Registro
        // Método para registrar un nuevo usuario
        [HttpPost]
        public IActionResult Registrarse(Usuario user)
        {
            if (ModelState.IsValid)
            {
                _context.Usuarios.Add(user);
                _context.SaveChanges();
                return RedirectToAction("Index", "Home"); // Redirecciona a la página principal después de registrarse
            }
            return View(user);
        }

        // GET: Usuario/Login
        // Método para mostrar la vista de inicio de sesión
        public IActionResult Login()
        {
            return View();
        }

        // POST: Usuario/Login
        // Método para autenticar al usuario
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _context.Usuarios.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                // Almacena el ID del usuario en la sesión
                HttpContext.Session.SetInt32("UserId", user.Id);
                // Redirige a una página de "Dashboard" para mostrar el saldo del usuario
                return RedirectToAction("Dashboard", new { id = user.Id });
            }
            else
            {
                ViewBag.Error = "Usuario o Contraseña incorrecta";
                return View();
            }
        }

        // GET: Usuario/Dashboard
        // Método para mostrar el dashboard del usuario después de iniciar sesión
        public IActionResult Dashboard(int id)
        {
            var user = _context.Usuarios.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // GET: Usuario/CerrarSesion
        // Método para cerrar sesión del usuario
        public IActionResult CerrarSesion()
        {
            HttpContext.Session.Clear(); // Limpiar la sesión
            return RedirectToAction("Index", "Home");
        }

        // GET: Usuario/Transferencia
        // Método para mostrar la vista de transferencia
        public IActionResult Transferencia()
        {
            // Obtener el ID del usuario logueado desde la sesión
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login"); // Redirige a la página de login si no está logueado
            }

            // Pasar el ID del usuario logueado a la vista (opcional, para mostrar)
            ViewBag.SenderId = userId;
            return View();
        }

        // POST: Usuario/Transferencia
        // Método para procesar una transferencia entre usuarios
        [HttpPost]
        public IActionResult Transferencia(int recipientId, decimal amount)
        {
            // Obtener el ID del usuario logueado desde la sesión
            var senderId = HttpContext.Session.GetInt32("UserId");
            if (senderId == null)
            {
                return RedirectToAction("Login"); // Redirige a la página de login si no está logueado
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

            // Actualizar los saldos
            sender.Balance -= amount;
            recipient.Balance += amount;

            // Registrar la transacción
            var transaction = new Transaccion
            {
                SenderId = senderId.Value,
                RecipientId = recipientId,
                Amount = amount
            };
            _context.Transacciones.Add(transaction);

            // Guardar los cambios en la base de datos
            _context.SaveChanges();

            ViewBag.Success = $"¡La transferencia de ${amount} a {recipient.Username} fue exitosa!";
            ViewBag.SenderId = senderId; // Pasar el ID del remitente a la vista
            return View();
        }

        // GET: Usuario/HistoricoTransaccion
        // Método para mostrar el historial de transacciones del usuario
        public IActionResult HistorialTransaccion(int userId)
        {
            var transactions = _context.Transacciones
                .Where(t => t.SenderId == userId || t.RecipientId == userId)
                .OrderByDescending(t => t.TransactionDate)
                .ToList();
            // Pasar el userId a la vista
            ViewBag.UserId = userId;
            return View(transactions);
        }

        // GET: Usuario/Index
        // Método para mostrar la página de inicio
        public IActionResult Index()
        {
            return View();
        }
    }
}

