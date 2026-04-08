using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IO;

namespace BitirmeProjesiPortal.Controllers
{
    [AllowAnonymous]
    public class ScriptsController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public ScriptsController(IWebHostEnvironment env)
        {
            _env = env;
        }

        public IActionResult Index()
        {
            string scriptsPath = Path.Combine(_env.WebRootPath, "uploads", "scripts");

            if (!Directory.Exists(scriptsPath))
            {
                Directory.CreateDirectory(scriptsPath);
            }

            var existingFiles = Directory.GetFiles(scriptsPath, "*.ps1");
            if (existingFiles.Length == 0)
            {
                System.IO.File.WriteAllText(Path.Combine(scriptsPath, "CheckServerTime.ps1"), "Get-Date");
                System.IO.File.WriteAllText(Path.Combine(scriptsPath, "HelloSystem.ps1"), "Write-Output 'System Online. Ready for commands.'");
                System.IO.File.WriteAllText(Path.Combine(scriptsPath, "ListDirectory.ps1"), "Get-ChildItem -Name");
            }

            var files = Directory.GetFiles(scriptsPath, "*.ps1")
                                 .Select(Path.GetFileName)
                                 .ToList();

            return View(files);
        }

        [HttpPost]
        public IActionResult Execute(string scriptName)
        {
            string scriptPath = Path.Combine(_env.WebRootPath, "uploads", "scripts", scriptName);

            if (!System.IO.File.Exists(scriptPath))
            {
                TempData["Error"] = "Script not found.";
                return RedirectToAction("Index");
            }

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-ExecutionPolicy Bypass -File \"{scriptPath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(psi))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    ViewBag.Result = string.IsNullOrEmpty(error) ? output : $"ERROR: {error}";
                    ViewBag.ExecutedScript = scriptName;
                }
            }
            catch (Exception ex)
            {
                ViewBag.Result = "Execution Error: " + ex.Message;
            }

            var files = Directory.GetFiles(Path.Combine(_env.WebRootPath, "uploads", "scripts"), "*.ps1")
                                 .Select(Path.GetFileName)
                                 .ToList();
            return View("Index", files);
        }
    }
}