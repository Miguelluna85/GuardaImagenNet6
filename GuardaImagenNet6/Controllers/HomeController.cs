using GuardaImagenNet6.Helpers;
using GuardaImagenNet6.Models;
using GuardaImagenNet6.Models.Contexto;
using GuardaImagenNet6.ViewModel.Usuario;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace GuardaImagenNet6.Controllers;
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly PruebasDBContext context;
    private readonly IWebHostEnvironment env;

    public HomeController(IWebHostEnvironment _env, ILogger<HomeController> logger, PruebasDBContext _context)
    {
        _logger = logger;
        env = _env;
        context = _context;
    }
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Contacto()
    {
        return View();
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
