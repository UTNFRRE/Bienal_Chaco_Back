using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal; // Solo si necesitas HeaderDictionary
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Entidades
{
    public class CustomFormFile : IFormFile
    {
        private readonly Stream _stream;

        public CustomFormFile(Stream stream, string fileName)
        {
            _stream = stream;
            FileName = fileName;
        }

        public string FileName { get; }
        public string ContentType => "application/octet-stream";
        public long Length => _stream.Length;

        public IHeaderDictionary Headers => new HeaderDictionary();
        public string ContentDisposition { get; set; }
        public string Name { get; set; }

        public void CopyTo(Stream target)
        {
            _stream.CopyTo(target);
        }

        public Task CopyToAsync(Stream target, CancellationToken cancellationToken)
        {
            return _stream.CopyToAsync(target, cancellationToken);
        }

        public Stream OpenReadStream()
        {
            return _stream;
        }
    }
}