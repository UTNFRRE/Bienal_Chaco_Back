using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Entidades;
using Microsoft.AspNetCore.Mvc;
using Requests;
using Servicios;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace APIController.Controllers
{
    
    [Route("[controller]")]
    [ApiController]
    public class VotosController : ControllerBase
    {
        private readonly ICRUDServicesVotos votosService;

        public VotosController(ICRUDServicesVotos votosService)
        {
            this.votosService = votosService;
        }

        //Create: api/Votos
        [HttpPost]
        public async Task<IActionResult> CreateVoto([FromBody] VotosListRequest votos)
        {
            Votos votoCreado = await this.votosService.CreateAsync(votos);
            return Ok(votoCreado);
        }

        [HttpGet]
        public async Task<ActionResult> GetAllVotos()
        {
            var lista_votos = await this.votosService.GetAllAsync();
            return Ok(lista_votos);
        }

        [HttpHead]
        public async Task<ActionResult> ExistVoto(string userid, int esculturaid)
        {
            var existe = await this.votosService.ExistAsync(userid, esculturaid);
            if (existe == false) {
                return NotFound();
            }
            return Ok(existe);
        }
    }
}
