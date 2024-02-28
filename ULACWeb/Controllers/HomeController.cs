﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ULACWeb.Models;

namespace ULACWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Registro()
        {
            
            return View();
        }

        public ActionResult Login()
        {

            return View();
        }
        public ActionResult Inicio()
        {

            return View();
        }

        public ActionResult Seguimiento()
        {

            return View();
        }
        public ActionResult SeguimientoLista(List<SeguimientoModel> seguimientos)
        {
            return View(seguimientos);
        }
        public ActionResult DetallesOperacion()
        {

            return View();
        }
    }
}