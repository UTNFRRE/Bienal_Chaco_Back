using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Entidades;
using Servicios;
using Requests;
using Microsoft.AspNetCore.Http.HttpResults;
using static System.Net.WebRequestMethods;

namespace APIBienal.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EsculturasController : ControllerBase
    {
        private readonly ICRUDEsculturaService esculturaService;

        public EsculturasController(ICRUDEsculturaService esculturasService)
        {
            this.esculturaService = esculturasService;
        }

        // Crear Escultura (CRUD para esculturas)
        [HttpPost]
        public async Task<IActionResult> CrearEscultura([FromForm] EsculturaPostPut request)
        {
            Esculturas esculturaCreate = await this.esculturaService.CreateAsync(request);
            if (esculturaCreate == null)
            {
                return BadRequest("Ya existe una escultura con nombre asignado");
            }
            return CreatedAtAction(nameof(ObtenerEscultura), new { id = esculturaCreate.EsculturaId }, esculturaCreate);
        }

        // Obtener escultura por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerEscultura(int id)
        {
        var escultura = await this.esculturaService.GetByAsync(id);
        if (escultura == null)
        {
            return NotFound("No se encontro una escultura con el id proporcionado");
        }

        //devolver link de imagen hardcodeo url de azure
        escultura.Imagenes = "https://bienalobjectstorage.blob.core.windows.net/imagenes/" + escultura.Imagenes;

        return Ok(escultura);
        }

        [HttpGet("GetDetail/{id}")]
        public async Task<IActionResult> ObtenerDetalleEscultura(int id)
        {
            var esculturaDetail = await this.esculturaService.GetDetail(id);
            if (esculturaDetail == null)
            {
                return NotFound("No se encontro una escultura con el id proporcionado");
            }

            return Ok(esculturaDetail);
        }
        // Este usa el front
        [HttpGet("GetAll")]
        public async Task<IActionResult> ObtenerListaEsculturas( int pageNumber = 1, int pageSize = 10, int? AnioEdicion = null)
        {
            var esculturaDetail = await this.esculturaService.GetAllList(pageNumber, pageSize, AnioEdicion);
            if (esculturaDetail == null)
            {
                return NotFound("No se encontro ninguna escultura");
            }

            

            return Ok(esculturaDetail);
        }

        // Actualizar escultura
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarTodaEscultura(int id, [FromForm] EsculturaPostPut request)
        {
            Esculturas? esculturaUpdate = await this.esculturaService.UpdatePutEsculturaAsync(id, request);
                if (esculturaUpdate == null)
                { 
                    return NotFound("Ocurrio un error al actualizar escultura. Intentelo nuevamente. Verifique si existe la escultura");
                }   
             return Ok(esculturaUpdate);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> ActualizarPropiedadEscultura(int id, [FromForm] EsculturaPatch request)
        {
            Esculturas? esculturaUpdate = await this.esculturaService.UpdatePatchAsync(id, request);
            if (esculturaUpdate == null)
            {
                return NotFound("Ocurrio un error al actualizar escultura. Intentelo nuevamente. Verifique si existe la escultura o si ya existe otra escultura con el nombre proporcionado");
            }
            return Ok(esculturaUpdate);
        }
        //Voto de Escultura
        [HttpPatch("{id}/Votar")]
        public async Task<IActionResult> Votacion(int id, [FromForm] EsculturaVoto request)
        {
            Esculturas? esculturaUpdate = await this.esculturaService.VoteEscultura(id, request);
            if (esculturaUpdate == null)
            {
                return NotFound("Ocurrio un error al votar la escultura. Intentelo nuevamente. Verifique si existe la escultura");
            }
            return Ok(esculturaUpdate);
        }


        //implementar patch con imagen para escultura

        // Eliminar escultura
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarEscultura(int id)
        {
            var resultadoDelete = await this.esculturaService.DeleteAsync(id);
            if (resultadoDelete == false)
            {
                return NotFound("No se encontro una escultura con el id proporcionado");
            }
            return Ok("Se elimino exitosamente la escultura");
        }


        // Upload de imagen y de multiimagen
        [HttpPut("UploadImage")]
        public async Task<IActionResult> UploadImage(IFormFile formFile, string esculturaID)
        {
            try
            {
                string filePath = GetFilepath(esculturaID);

                if (!System.IO.Directory.Exists(filePath))
                {
                    System.IO.Directory.CreateDirectory(filePath);
                }

                string imagePath = filePath + "\\" + esculturaID + ".png";

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                using (FileStream stream = System.IO.File.Create(imagePath))
                {
                    await formFile.CopyToAsync(stream);
                }

                return Ok("Imagen subida exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpPut("MultiUploadImage")]
        public async Task<IActionResult> MultiUploadImage(IFormFileCollection fileCollection, string esculturaID)
        {
            int passCount = 0;
            int errorCount = 0;
            try
            {
                string filePath = GetFilepath(esculturaID);

                if (!System.IO.Directory.Exists(filePath))
                {
                    System.IO.Directory.CreateDirectory(filePath);
                }

                foreach (var file in fileCollection)
                {
                    string imagePath = filePath + "\\" + file.FileName;

                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }

                    using (FileStream stream = System.IO.File.Create(imagePath))
                    {
                        await file.CopyToAsync(stream);
                        passCount++;
                    }
                }

                return Ok($"{passCount} imágenes subidas exitosamente, {errorCount} imágenes fallaron.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet("GetImage")]
        public IActionResult GetImage(string esculturaID)
        {
            string hostUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            string filePath = GetFilepath(esculturaID);
            string imagePath = filePath + "\\" + esculturaID + ".png";

            if (System.IO.File.Exists(imagePath))
            {
                string imageUrl = hostUrl + "/Upload/escultura/" + esculturaID + "/" + esculturaID + ".png";
                return Ok(imageUrl);
            }
            else
            {
                return NotFound("Imagen no encontrada.");
            }
        }

        [HttpGet("GetMultiImage")]
        public async Task<IActionResult> GetMultiImage(string esculturaID)
        {
            List<string> imageUrls = new List<string>();
            var containerClient = new BlobContainerClient("<connection_string>", "esculturas");

            try
            {
                await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: $"{esculturaID}/"))
                {
                    var blobClient = containerClient.GetBlobClient(blobItem.Name);
                    imageUrls.Add(blobClient.Uri.ToString());
                }

                if (imageUrls.Count == 0)
                    return NotFound("No se encontraron imágenes para esta escultura.");

                return Ok(imageUrls);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("download")]
        public async Task<IActionResult> download(string productcode)
        {
            // string Imageurl = string.Empty;
            //string hosturl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            try
            {
                string Filepath = GetFilepath(productcode);
                string imagepath = Filepath + "\\" + productcode + ".png";
                if (System.IO.File.Exists(imagepath))
                {
                    MemoryStream stream = new MemoryStream();
                    using (FileStream fileStream = new FileStream(imagepath, FileMode.Open))
                    {
                        await fileStream.CopyToAsync(stream);
                    }
                    stream.Position = 0;
                    return File(stream, "image/png", productcode + ".png");
                    //Imageurl = hosturl + "/Upload/product/" + productcode + "/" + productcode + ".png";
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }


        }

        [HttpDelete("remove")]
        public async Task<IActionResult> Remove(string sculptureId)
        {
            try
            {
                var containerClient = new BlobContainerClient("<connection_string>", "sculptures");
                var blobClient = containerClient.GetBlobClient($"{sculptureId}.png");

                if (await blobClient.ExistsAsync())
                {
                    await blobClient.DeleteAsync();
                    return Ok("Image deleted successfully.");
                }
                else
                {
                    return NotFound("Image not found.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("multiremove")]
        public async Task<IActionResult> MultiRemove(string sculptureId)
        {
            var containerClient = new BlobContainerClient("<connection_string>", "sculptures");
            int deleteCount = 0;

            try
            {
                await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: $"{sculptureId}/"))
                {
                    var blobClient = containerClient.GetBlobClient(blobItem.Name);
                    await blobClient.DeleteAsync();
                    deleteCount++;
                }

                return Ok($"{deleteCount} images deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    [HttpPut("DBMultiUploadImage")]
        public async Task<IActionResult> DBMultiUploadImage(IFormFileCollection filecollection, string productcode)
        {
            APIResponse response = new APIResponse();
            int passcount = 0; int errorcount = 0;
            try
            {
                foreach (var file in filecollection)
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        this.context.TblProductimages.Add(new Repos.Models.TblProductimage()
                        {
                            Productcode = productcode,
                            Productimage = stream.ToArray()
                        });
                        await this.context.SaveChangesAsync();
                        passcount++;
                    }
                }


            }
            catch (Exception ex)
            {
                errorcount++;
                response.Message = ex.Message;
            }
            response.ResponseCode = 200;
            response.Result = passcount + " Files uploaded &" + errorcount + " files failed";
            return Ok(response);
        }


        [HttpGet("GetDBMultiImage")]
        public async Task<IActionResult> GetDBMultiImage(string productcode)
        {
            List<string> Imageurl = new List<string>();
            //string hosturl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            try
            {
                var _productimage = this.context.TblProductimages.Where(item => item.Productcode == productcode).ToList();
                if (_productimage != null && _productimage.Count > 0)
                {
                    _productimage.ForEach(item =>
                    {
                        Imageurl.Add(Convert.ToBase64String(item.Productimage));
                    });
                }
                else
                {
                    return NotFound();
                }
                //string Filepath = GetFilepath(productcode);

                //if (System.IO.Directory.Exists(Filepath))
                //{
                //    DirectoryInfo directoryInfo = new DirectoryInfo(Filepath);
                //    FileInfo[] fileInfos = directoryInfo.GetFiles();
                //    foreach (FileInfo fileInfo in fileInfos)
                //    {
                //        string filename = fileInfo.Name;
                //        string imagepath = Filepath + "\\" + filename;
                //        if (System.IO.File.Exists(imagepath))
                //        {
                //            string _Imageurl = hosturl + "/Upload/product/" + productcode + "/" + filename;
                //            Imageurl.Add(_Imageurl);
                //        }
                //    }
                //}

            }
            catch (Exception ex)
            {
            }
            return Ok(Imageurl);

        }


        [HttpGet("dbdownload")]
        public async Task<IActionResult> dbdownload(string productcode)
        {

            try
            {

                var _productimage = await this.context.TblProductimages.FirstOrDefaultAsync(item => item.Productcode == productcode);
                if (_productimage != null)
                {
                    return File(_productimage.Productimage, "image/png", productcode + ".png");
                }


                //string Filepath = GetFilepath(productcode);
                //string imagepath = Filepath + "\\" + productcode + ".png";
                //if (System.IO.File.Exists(imagepath))
                //{
                //    MemoryStream stream = new MemoryStream();
                //    using (FileStream fileStream = new FileStream(imagepath, FileMode.Open))
                //    {
                //        await fileStream.CopyToAsync(stream);
                //    }
                //    stream.Position = 0;
                //    return File(stream, "image/png", productcode + ".png");
                //    //Imageurl = hosturl + "/Upload/product/" + productcode + "/" + productcode + ".png";
                //}
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }


        }

        [NonAction]
        private string GetFilepath(string productcode)
        {
            return this.environment.WebRootPath + "\\Upload\\product\\" + productcode;
        }

    }
}