using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ORM;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

namespace Front.Controllers
{
    public class LivresController : Controller
    {
        private Model1 db = new Model1();

        // GET: Livres
        public async Task<ActionResult> Index()
        {
            //au lieu d'utiliser l'ORM, on utilise l'API REST
            string url = "https://localhost:44301/api/Livres"; // appel de l'api

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);

                //test de succès
                if (!response.IsSuccessStatusCode)
                    throw new Exception("Une erreur est survenue lors de l'appel de l'api");

                var Livres = await response.Content.ReadAsAsync<IEnumerable<Livre>>();

                return View(Livres);
            }
        }

        // GET: Livres/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Livre livre = await db.Livres.FindAsync(id);
            if (livre == null)
            {
                return HttpNotFound();
            }
            return View(livre);
        }

        // GET: Livres/Create
        public ActionResult Create()
        {
            ViewBag.IdAuteur = new SelectList(db.Auteurs, "IdAuteur", "Nom");
            return View();
        }

        // POST: Livres/Create
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IdLivre,Titre,Prix,IdAuteur,Genre")] Livre livre)
        {
            string json = JsonConvert.SerializeObject(livre);
            using (HttpClient client = new HttpClient())
            {
                //au lieu d'utiliser l'ORM, on utilise l'API REST
                string url = "https://localhost:44301/api/Livres"; // appel de l'api

                using (var request = new HttpRequestMessage(HttpMethod.Post, url))
                {
                    var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                    request.Content = stringContent;

                    var send = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

                    //test de succès
                    if (!send.IsSuccessStatusCode)
                        throw new Exception("Une erreur est survenue kors de l'appel de l'api");

                    send.EnsureSuccessStatusCode();

                    return RedirectToAction("Index");
                }
            }
        }

        // GET: Livres/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Livre livre = await db.Livres.FindAsync(id);
            if (livre == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdAuteur = new SelectList(db.Auteurs, "IdAuteur", "Nom", livre.IdAuteur);
            return View(livre);
        }

        // POST: Livres/Edit/5
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IdLivre,Titre,Prix,IdAuteur,Genre")] Livre livre)
        {
            if (ModelState.IsValid)
            {
                db.Entry(livre).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.IdAuteur = new SelectList(db.Auteurs, "IdAuteur", "Nom", livre.IdAuteur);
            return View(livre);
        }

        // GET: Livres/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Livre livre = await db.Livres.FindAsync(id);
            if (livre == null)
            {
                return HttpNotFound();
            }
            return View(livre);
        }

        // POST: Livres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Livre livre = await db.Livres.FindAsync(id);
            db.Livres.Remove(livre);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
