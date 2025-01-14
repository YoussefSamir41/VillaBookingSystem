using Application.Common.Interfaces;
using Application.Common.Utility;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VillaBooking.ViewModels;

namespace VillaBooking.Controllers
{
    [Authorize(Roles =SD.RoleAdmin)]
    public class AmenityController : Controller
    {

        private readonly IUnitOfWork unitOfWork;

        public AmenityController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }


        public IActionResult Index()
        {
            var villas = unitOfWork.Amenity.GetAll(includeProperties: "Villa");
            return View(villas);
        }


        public IActionResult Create()
        {
            AmenityVM villaNumberVM = new AmenityVM()
            {
                AmenityList = unitOfWork.Villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                })
            };


            return View(villaNumberVM);
        }

        [HttpPost]

        public IActionResult Create(AmenityVM villa)
        {

            //ModelState.Remove("Villa"); 

            if (ModelState.IsValid)
            {
                unitOfWork.Amenity.Add(villa.Amenity);
                unitOfWork.Amenity.Save();
                TempData["success"] = "The villa has been Created Successfuly";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View();
            }

        }




        public IActionResult Update(int amenityid)
        {
            AmenityVM villaNumberVM = new AmenityVM()
            {
                AmenityList = unitOfWork.Villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                }),

                Amenity = unitOfWork.Amenity.Get(u => u.Id == amenityid)
            };
            return View(villaNumberVM);

        }



        [HttpPost]
        public IActionResult Update(AmenityVM villaNumberVM)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.Amenity.Update(villaNumberVM.Amenity);
                unitOfWork.Amenity.Save();
                TempData["success"] = "The villa has ben updated Successfuly";
                return RedirectToAction(nameof(Index));
            }
            villaNumberVM = new AmenityVM()
            {
                AmenityList = unitOfWork.Villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                })
            };

            return View(villaNumberVM);

        }


        public IActionResult Delete(int amenityid)
        {
            AmenityVM villaNumberVM = new AmenityVM()
            {
                AmenityList = unitOfWork.Villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                }),

                Amenity = unitOfWork.Amenity.Get(u => u.Id == amenityid)
            };
            return View(villaNumberVM);

        }


        [HttpPost]
        public IActionResult Delete(AmenityVM amenityVm)
        {

            Amenity RemovedVilla = unitOfWork.Amenity.Get(u => u.Id == amenityVm.Amenity.Id);
            if (ModelState.IsValid)
            {
                unitOfWork.Amenity.Remove(RemovedVilla);
                unitOfWork.Amenity.Save();
                TempData["success"] = "The villa has been Deleted Successfuly";
                return RedirectToAction(nameof(Index));
            }

            return View();

        }










    }
}
