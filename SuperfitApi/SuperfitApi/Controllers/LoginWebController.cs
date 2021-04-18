using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using SuperfitApi.Models;
using SuperfitApi.Models.Entity;

namespace SuperfitApi.Controllers
{
    public class LoginWebController : Controller
    {
        #region Variables 
        public SuperfitEntities  Db;
        public Clientes clientes;
        public Cuestionario cuestionario;
        public Mensualidad mensualidad;
        public Asesoria_antropometria asesoria_antropometria;
        //modelos
        public ClientesModel clientesMdl;
        public UsuariosModel usuariosMdl;
        public CuestionarioModel cuestionarioMdl;
        public MensualidadModel mensualidadMdl;
        public AntropometriaModel asesoria_antropometriaMdl;
        public AlertasModel alertasModel;
        #endregion       
        public LoginWebController()
        {
            Db = new SuperfitEntities();
            clientes = new Clientes();
            cuestionario = new Cuestionario();
            mensualidad = new Mensualidad();
            asesoria_antropometria = new Asesoria_antropometria();
            //modelos
            clientesMdl = new ClientesModel();
            usuariosMdl = new UsuariosModel();
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
                                       Id_cliente = c.Id_cliente,
                                       Nombres = c.Nombres,
                                       Foto_perfil = c.Foto_perfil
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
                                           Id_cliente = c.Id_cliente,
                                           Nombres = c.Nombres,
                                           Foto_perfil = c.Foto_perfil
                                       }).FirstOrDefault();
                    }
                }

                if (clientesMdl.Id_cliente != 0)
                {
                    Session["Id_cliente"] = clientesMdl.Id_cliente;
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
        
        //Administrador        
        public ActionResult LoginAdministradorWeb()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoginAdministradorWeb(string User, string Pass)
        {
            try
            {

                usuariosMdl = (from c in Db.Usuarios 
                                where c.Clave_usuario  == User
                                && c.Contraseña == Pass
                                select new UsuariosModel()
                                {
                                    Id_Usuario = c.Id_usuario,
                                    Nombres = c.Nombres,
                                }).FirstOrDefault();                
                if (usuariosMdl != null)
                {
                    Session["Id_cliente"] = clientesMdl.Id_cliente;
                    return RedirectToAction("AdministracionWebHome", "AdministracionWebHome");
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