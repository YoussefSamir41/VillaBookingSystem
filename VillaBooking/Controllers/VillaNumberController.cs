using Application.Common.Interfaces;
using Domain.Entities;
using Infrastucture.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VillaBooking.ViewModels;

namespace VillaBooking.Controllers
{
    public class VillaNumberController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public VillaNumberController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var villas = unitOfWork.VillaNumber.GetAll(includeProperties:"Villa");
            return View(villas);
        }

        public IActionResult Create()
        {
            VillaNumberVM villaNumberVM = new VillaNumberVM()
            {
                VillaList = unitOfWork.Villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                })
            };
        
            
            return View(villaNumberVM);
        }

        [HttpPost]

        public IActionResult Create(VillaNumberVM villa)
        {

            //ModelState.Remove("Villa"); 

            if (ModelState.IsValid)
            {
                unitOfWork.VillaNumber.Add(villa.VillaNumber);
                unitOfWork.VillaNumber.Save(); 
                TempData["success"] = "The villa has been Created Successfuly";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View();
            }

        }




        public IActionResult Update(int VillaNumberId)
        {
            VillaNumberVM villaNumberVM = new VillaNumberVM()
            {
                VillaList = unitOfWork.Villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                }),

                VillaNumber = unitOfWork.VillaNumber.Get(u=>u.Villa_Number== VillaNumberId) 
            };
            return View(villaNumberVM);

        }



        [HttpPost]
        public IActionResult Update(VillaNumberVM villaNumberVM)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.VillaNumber.Update(villaNumberVM.VillaNumber);
                 unitOfWork.VillaNumber.Save();
                TempData["success"] = "The villa has ben updated Successfuly";
                return RedirectToAction(nameof(Index));
            }
             villaNumberVM = new VillaNumberVM()
            {
                VillaList = unitOfWork.Villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                })
            };

            return View(villaNumberVM); 

        }


        public IActionResult Delete(int VillaNumberId)
        {
            VillaNumberVM villaNumberVM = new VillaNumberVM()
            {
                VillaList = unitOfWork.Villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                }),

                VillaNumber = unitOfWork.VillaNumber.Get(u => u.Villa_Number == VillaNumberId)
            };
            return View(villaNumberVM);

        }


        [HttpPost]
        public IActionResult Delete(VillaNumberVM villaNumberVM)
        {

            VillaNumber RemovedVilla = unitOfWork.VillaNumber.Get
            (u => u.Villa_Number == villaNumberVM.VillaNumber.Villa_Number); 
            if (ModelState.IsValid)
            {
                unitOfWork.VillaNumber.Remove(RemovedVilla);
                unitOfWork.VillaNumber.Save();
                TempData["success"] = "The villa has been Deleted Successfuly";
                return RedirectToAction(nameof(Index));
            }
           
            return View();

        }


    }
}
