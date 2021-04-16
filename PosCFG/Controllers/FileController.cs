using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PosCFG.Models;
using Microsoft.AspNetCore.Authorization;


namespace PosCFG.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]")]
    [Authorize(Policy = "ApiScope")]
    [Authorize(Roles="TerminalsFull")]
    public class FileController : Controller
    {
    private readonly IFileService _fileService;
    private readonly IOptions<PathSettings> _pathsettings;
    
    public FileController(IFileService fileService, IOptions<PathSettings> path)
       {
           _fileService = fileService;
           _pathsettings = path;
       }
    
    
    // upload file(s) to server that palce under path: rootDirectory/subDirectory
        /*[HttpPost("upload")]
        public IActionResult UploadFile([FromForm(Name = "files")] List<IFormFile> files, string subDirectory)
        {
            try
            {
                _fileService.SaveFile(files, subDirectory);
                _fileService.InsertFileToDB();
                //_fileService.DBtoFile();

                return Ok(new { files.Count, Size = FileService.SizeConverter(files.Sum(f => f.Length)) });
            }
            catch (Exception exception)
            {
                return BadRequest($"Error: {exception.Message}");
            }
        }*/

        [HttpPost("upload")]
        public IActionResult UploadFileToDirectory([FromForm(Name = "")] IFormFile file)
        {
            try
            {
                string path = _pathsettings.Value.PathUpload;
                _fileService.SaveFileToDirectory(file, path);
                //_fileService.InsertDB(file);
                return Ok();
            }
            catch (Exception exception)
            {
                return BadRequest($"Error: {exception.Message}");
            }
        }


        [HttpPost("insertTerminalToDB")]
        public IActionResult insertToDB()
        {
            try
            {
                string path1 = _pathsettings.Value.PathUpload;
                _fileService.InsertDBTerminal(path1);
                return Ok();
            }
            catch (Exception exception)
            {
                return BadRequest($"Error: {exception.Message}");
            }
        }

        [HttpPost("generateTerminalTXT")]
        
        public IActionResult generateTerminalsTXT()
        {
            try
            {
                var path = _pathsettings.Value.PathRepository;
                _fileService.DBtoFileTerminal(path);
                return Ok();
            }
            catch (Exception exception)
            {
                return BadRequest($"Error: {exception.Message}");
            }
        }

        [HttpPost("generateSystemTXT")]
        public async Task<ActionResult> generateSystemTXT()
        {
                var path = _pathsettings.Value.PathRepository;
                var response = await _fileService.DBtoFileSystem(path);
                
                if(response.Data == null)
                {
                    return NotFound(response);
                }
                else
                    return Ok(response);
               
        }

        [HttpPost("insertSystemToDB")]
        public async Task<ActionResult> insertSysToDB()
        {
            
                string path1 = _pathsettings.Value.PathUpload;
                var response = await _fileService.InsertDBSystem(path1);
                if(response.Data == null)
                {
                    return NotFound(response);    
                }
                else
                    return Ok(response);
            
        }

    }
}