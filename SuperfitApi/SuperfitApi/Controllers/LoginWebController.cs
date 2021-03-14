﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using SuperfitApi.Models;

namespace SuperfitApi.Controllers
{
    public class LoginWebController : Controller
    {
        #region Variables 
        public SuperflyfitEntities Db;
        public Clientes clientes;
        public Cuestionario cuestionario;
        public Mensualidad mensualidad;
        public Asesoria_Antropometria asesoria_antropometria;
        //modelos
        public ClientesModel clientesMdl;
        public CuestionarioModel cuestionarioMdl;
        public MensualidadModel mensualidadMdl;
        public AntropometriaModel asesoria_antropometriaMdl;
        public AlertasModel alertasModel;
        #endregion       
        public LoginWebController()
        {
            Db = new SuperflyfitEntities();
            clientes = new Clientes();
            cuestionario = new Cuestionario();
            mensualidad = new Mensualidad();
            asesoria_antropometria = new Asesoria_Antropometria();
            //modelos
            clientesMdl = new ClientesModel();
            cuestionarioMdl = new CuestionarioModel();
            mensualidadMdl = new MensualidadModel();
            asesoria_antropometriaMdl = new AntropometriaModel();
            alertasModel = new AlertasModel();
        }
        // GET: LoginWeb
        public ActionResult LoginWeb()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LoginWeb(string User, string Pass)
        {
            try
            {
                if (ValidacionUser(User) == true)
                {
                    clientesMdl = (from c in Db.Clientes
                                   where c.Correo_electronico == User
                                   && c.Contraseña == Pass
                                   select new ClientesModel()
                                   {
                                       Id_Cliente = c.Id_Cliente,
                                       Nombres = c.Nombres,
                                       Fotoperfil = c.Fotoperfil
                                   }).FirstOrDefault();
                }
                else
                {
                    if (ValidarCelular(User) == true)
                    {
                        decimal celular = 0;
                        celular = decimal.Parse(User);
                        clientesMdl = (from c in Db.Clientes
                                       where c.Telefono == celular
                                       && c.Contraseña == Pass
                                       select new ClientesModel()
                                       {
                                           Id_Cliente = c.Id_Cliente,
                                           Nombres = c.Nombres,
                                           Fotoperfil = c.Fotoperfil
                                       }).FirstOrDefault();
                    }
                }

                if (clientesMdl.Id_Cliente != 0)
                {
                    Session["Id_Cliente"] = clientesMdl.Id_Cliente;
                    return RedirectToAction("Perfil", "ClientesWeb");
                }
                else
                {
                    ViewBag.Mensaje = "Usuario y/o contraseña Incorrectos";
                    return View(mensualidadMdl);
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Shared", new { Error = ex.Message });
            }
        }
        public bool ValidacionUser(string email)
        {
            string expresion;
            expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(email, expresion))
            {
                if (Regex.Replace(email, expresion, string.Empty).Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public bool ValidarCelular(string strNumber)
        {
            Regex regex = new Regex(@"\A[0-9]{7,10}\z");
            Match match = regex.Match(strNumber);
            if (match.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}