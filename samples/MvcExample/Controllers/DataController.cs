/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MvcExample.Database;
using MvcExample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcExample.Controllers
{
    public class DataController : Controller
    {
        private readonly DataDbContext _dataDbContext;
        private readonly IAuthorizationService _authorizationService;

        public DataController(DataDbContext dataDbContext, IAuthorizationService authorizationService)
        {
            _dataDbContext = dataDbContext;
            _authorizationService = authorizationService;
        }

        // GET: DataController
        public async Task<ActionResult> Index()
        {
            var authResult = await _authorizationService.AuthorizeQueryable(User, _dataDbContext.Data, "read");

            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            return View(authResult.Queryable);
        }

        // GET: DataController/Details/5
        public async Task<ActionResult> Details(string id)
        {
            var resource = _dataDbContext.Data.FirstOrDefault(x => x.Name == id);

            if (resource == null)
            {
                return NotFound();
            }

            var authResult = await _authorizationService.AuthorizeAsync(User, resource, "read");

            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            return View(resource);
        }

        // GET: DataController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DataController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([FromForm] DataModel model)
        {
            try
            {
                //Normal data validation should be done first, to create good user feedback.

                var authResult = await _authorizationService.AuthorizeAsync(User, model, "create");

                if (authResult.Succeeded)
                {
                    _dataDbContext.Data.Add(model);
                    await _dataDbContext.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return Forbid();
                }
            }
            catch(Exception e)
            {
                return View();
            }
        }

        // GET: DataController/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            var resource = _dataDbContext.Data.FirstOrDefault(x => x.Name == id);

            if (resource == null)
            {
                return NotFound();
            }

            //We mock an API with mvc just for some front end, so we just check read permissions
            var authResult = await _authorizationService.AuthorizeAsync(User, resource, "read");

            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            return View(resource);
        }

        // POST: DataController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, [FromForm] DataModel dataModel)
        {
            try
            {
                var old = _dataDbContext.Data.FirstOrDefault(x => x.Name == id);

                if (old == null)
                {
                    return NotFound();
                }

                //Authorize with different input and data object
                var authResult = await _authorizationService.AuthorizeAsync(User, dataModel, old, "update");

                if (!authResult.Succeeded)
                {
                    return Forbid();
                }

                old.Owner = dataModel.Owner;
                old.Text = dataModel.Text;

                await _dataDbContext.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: DataController/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            var resource = _dataDbContext.Data.FirstOrDefault(x => x.Name == id);

            if (resource == null)
            {
                return NotFound();
            }

            //We mock an API with mvc just for some front end, so we just check read permissions
            var authResult = await _authorizationService.AuthorizeAsync(User, resource, "read");

            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            return View(resource);
        }

        // POST: DataController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id, IFormCollection collection)
        {
            try
            {
                var resource = _dataDbContext.Data.FirstOrDefault(x => x.Name == id);

                if (resource == null)
                {
                    return NotFound();
                }

                var authResult = await _authorizationService.AuthorizeAsync(User, resource, "delete");

                if (!authResult.Succeeded)
                {
                    return Forbid();
                }

                _dataDbContext.Data.Remove(resource);
                await _dataDbContext.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
