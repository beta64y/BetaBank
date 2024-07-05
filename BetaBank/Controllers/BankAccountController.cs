using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BetaBank.Controllers
{
    public class BankAccountController : Controller
    {
        // GET: BankAccountController
        public ActionResult Index()
        {
            return View();
        }

        // GET: BankAccountController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: BankAccountController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BankAccountController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: BankAccountController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: BankAccountController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: BankAccountController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: BankAccountController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
