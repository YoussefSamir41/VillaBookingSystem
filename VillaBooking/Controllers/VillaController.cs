using Application.Common.Interfaces;
using Application.Common.Utility;
using Domain.Entities;
using Infrastucture.Data;
using Infrastucture.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace VillaBooking.Controllers
{
    [Authorize]
    public class VillaController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IWebHostEnvironment webHostEnvironment;

        public VillaController(IUnitOfWork unitOfWork , IWebHostEnvironment webHostEnvironment)
        {
            this.unitOfWork = unitOfWork;
            this.webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            var villas = unitOfWork.Villa.GetAll();
            return View(villas);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]

        public IActionResult Create(Villa villa) {

            if (villa.Name == villa.Description)
            {
                ModelState.AddModelError("", "Description and Name Can not be the same"); 
            }
         
           if (ModelState.IsValid)
           {
                if (villa.Image is not null)
                {
                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(villa.Image.FileName);
                    string imagePath = Path.Combine(webHostEnvironment.WebRootPath, @"Images\Villa");
                    using var filestream = new FileStream(Path.Combine(imagePath, filename), FileMode.Create);

                    villa.Image.CopyTo(filestream);

                    villa.ImageUrl = @"/Images/Villa/" + filename;
                }

                else
                {
                    villa.ImageUrl = null;
                }
                unitOfWork.Villa.Add(villa);
                unitOfWork.Villa.Save();
                TempData["success"] = "The villa has ben Created Successfuly";
                return RedirectToAction(nameof(Index));
           }else
           {
                return View();
           }
        
        }



        
        public IActionResult Update(int id)
        {
            Villa? villa = unitOfWork.Villa.Get(x => x.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
           
            return View(villa);
            
        }



        [HttpPost]
        public IActionResult Update(Villa villa)
        {
            if (ModelState.IsValid)
            {
                if (villa.Image is not null)
                {
                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(villa.Image.FileName);
                    string imagePath = Path.Combine(webHostEnvironment.WebRootPath, @"Images\Villa");
                    if (!string.IsNullOrEmpty(villa.ImageUrl))
                    {
                        var oldImage = Path.Combine(webHostEnvironment.WebRootPath, villa.ImageUrl.TrimStart('\\')); 

                        if (System.IO.File.Exists(oldImage))
                        {
                            System.IO.File.Delete(oldImage);
                        }
                    }
                    using var filestream = new FileStream(Path.Combine(imagePath, filename), FileMode.Create);
                    villa.Image.CopyTo(filestream);
                    villa.ImageUrl = @"/Images/Villa/" + filename;
                }
                unitOfWork.Villa.Update(villa);
                unitOfWork.Villa.Save();
                TempData["success"] = "The villa has ben updated Successfuly";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View();
            }

        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var villa = unitOfWork.Villa.Get(x => x.Id == id);
            if (villa!= null)
            {
                return View(villa); 
            }else
            {
                return BadRequest(); 
            }
        }


        [HttpPost]
        public IActionResult Delete(Villa villa)
        {
            var removedVilla = unitOfWork.Villa.Get(u=>u.Id == villa.Id);
            if (removedVilla is not null)
            {

                if (!string.IsNullOrEmpty(removedVilla.ImageUrl))
                {
                    var oldImage = Path.Combine(webHostEnvironment.WebRootPath, removedVilla.ImageUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImage))
                    {
                        System.IO.File.Delete(oldImage);
                    }
                }


                unitOfWork.Villa.Remove(removedVilla);
                unitOfWork.Villa.Save();
                TempData["success"] = "The villa has ben Deleted Successfuly";
                return RedirectToAction(nameof(Index));
            }
            else
            { 
                return View();
            }

        }
    }
}
