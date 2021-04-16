
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FileHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PosCFG.Dto;
using PosCFG.Models;

public class FileService : IFileService
    {
        
        private readonly PosCFGDbContext _context;
        private readonly IMapper _mapper;


        public FileService (PosCFGDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /*public void SaveFile(List<IFormFile> files, string subDirectory)
        {
            subDirectory = subDirectory ?? string.Empty;
            var target = Path.Combine("/Users/hernandeleon/Documents/manisoft/poscfgterm/PosCFG", subDirectory);
                                      
            Directory.CreateDirectory(target);

            files.ForEach(async file =>
            {
                if (file.Length <= 0) return;
                var filePath = Path.Combine(target, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            });
        }

        public (string fileType, byte[] archiveData, string archiveName) FetechFiles(string subDirectory)
        {
            var zipName = $"archive-{DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss")}.zip";

            var files = Directory.GetFiles(Path.Combine("/Users/hernandeleon/Documents/manisoft/poscfgterm/PosCFG", subDirectory)).ToList();
                                                        
            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    files.ForEach(file =>
                    {
                        var theFile = archive.CreateEntry(file);
                        using (var streamWriter = new StreamWriter(theFile.Open()))
                        {
                            streamWriter.Write(File.ReadAllText(file));
                        }

                    });
                }

                return ("application/zip", memoryStream.ToArray(), zipName);
            }
            
        }

        public static string SizeConverter(long bytes)
        {
            var fileSize = new decimal(bytes);
            var kilobyte = new decimal(1024);
            var megabyte = new decimal(1024 * 1024);
            var gigabyte = new decimal(1024 * 1024 * 1024);

            switch (fileSize)
            {
                case var _ when fileSize < kilobyte:
                    return $"Less then 1KB";
                case var _ when fileSize < megabyte:
                    return $"{Math.Round(fileSize / kilobyte, 0, MidpointRounding.AwayFromZero):##,###.##}KB";
                case var _ when fileSize < gigabyte:
                    return $"{Math.Round(fileSize / megabyte, 2, MidpointRounding.AwayFromZero):##,###.##}MB";
                case var _ when fileSize >= gigabyte:
                    return $"{Math.Round(fileSize / gigabyte, 2, MidpointRounding.AwayFromZero):##,###.##}GB";
                default:
                    return "n/a";
            }
        }*/

        //lee archivo y lo inserta en la BD
        /*
        public void InsertFileToDB()
        {
            
            string path = @"/Users/hernandeleon/Documents/manisoft/poscfgterm/PosCFG/files/";
            var engine = new FileHelperEngine<Terminal>();

            /// to Read use:
            var res = engine.ReadFile(path + "terminal_prueba.txt");

            /// to Write use:
            engine.WriteFile(path + "output.txt", res);


            foreach (Terminal term in res)
            {
                //controlar que no exista en la BD, si existe tomar el termina_id y mostrar al final
                _context.Terminals.Add(term);
                _context.SaveChanges();
                
                Console.WriteLine("Terminal Info:");
                Console.WriteLine(term.TerminalID + " - " + term.SerialNumber);
                                    //order.OrderDate.ToString("dd/MM/yy"));                
                //_context.Terminals.Add(newTerminal);
                 
            }
            
        }
        */
        public void DBtoFileTerminal(string path)
        {
            //string path = @"/Users/hernandeleon/Documents/manisoft/poscfgterm/PosCFG/files/";
            try
            {
                FileHelperEngine engine = new FileHelperEngine(typeof(Terminal));   
                var ListTerminals = new List<Terminal>();

                ListTerminals = _context.Terminals.ToList();

                engine.WriteFile(path + "terminals.txt",ListTerminals);

            }
            catch (System.Exception)
            {
                
                throw;
            }

        }

        public void InsertDBTerminal(string path)
        {

            //string path = @"/Users/hernandeleon/Documents/manisoft/poscfgterm/PosCFG/files/";
            var engine = new FileHelperEngine<Terminal>();
            int total=0, no_insert=0;
            Terminal terminal;
            var res = engine.ReadFile(path + "terminal_prueba.txt");

            foreach (Terminal term in res)
            {

                terminal = _context.Terminals.FirstOrDefault(ct => ct.TerminalID.Equals(term.TerminalID));
                _context.SaveChangesAsync();
                
                if(terminal == null)
                {
                    
                    _context.Terminals.Add(term);
                    _context.SaveChanges();
                    total = total+1;
                    Console.WriteLine("Terminal Info:");
                    Console.WriteLine(term.TerminalID + " - " + term.SerialNumber); 

                }
                else
                {
                    no_insert = no_insert+1;
                }
            }

            Console.WriteLine("Terminals Insertados:");
            Console.WriteLine("Total:  " + total);
            Console.WriteLine("Terminals YA existentes: ");
            Console.WriteLine("Total: " + no_insert);

        }

        public void SaveFileToDirectory(IFormFile file, string path)
        {
            
            //var target = Path.Combine("/Users/hernandeleon/Documents/manisoft/poscfgterm/PosCFG/files/");                      
            Directory.CreateDirectory(path);

            if (file.Length <= 0) return;
            var filePath = Path.Combine(path, file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyToAsync(stream);
            }        
        }

        public async Task<ServiceResponse<List<GetSystemPOSDto>>>  DBtoFileSystem(string path)
        {
            ServiceResponse<List<GetSystemPOSDto>> serviceResponse = new ServiceResponse<List<GetSystemPOSDto>>();
            try
            {
                FileHelperEngine engine = new FileHelperEngine(typeof(SystemPOS));   
                //var ListSystem = new List<SystemPOS>();

                var ListSystem = await _context.SystemPOSs.ToListAsync();

                engine.WriteFile(path + "system.txt",ListSystem);
                serviceResponse.Data = _mapper.Map<List<GetSystemPOSDto>>(ListSystem);

            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;

        }

        public async Task<ServiceResponse<List<GetSystemPOSDto>>> InsertDBSystem(string path)
        {
            ServiceResponse<List<GetSystemPOSDto>> serviceResponse = new ServiceResponse<List<GetSystemPOSDto>>();
            try
            {
                //string path = @"/Users/hernandeleon/Documents/manisoft/poscfgterm/PosCFG/files/";
                var engine = new FileHelperEngine<SystemPOS>();
                int total=0, no_insert=0;
                bool existsInTerminal=false;
                SystemPOS systempos;
                Terminal term;
                var res = engine.ReadFile(path + "system_prueba.txt");

                foreach (SystemPOS sys in res)
                {
                    term = await _context.Terminals.FirstOrDefaultAsync(ct => ct.TerminalID.Equals(sys.TerminalID));
                    await _context.SaveChangesAsync();

                    if (term != null)
                    {
                        existsInTerminal = true;

                        systempos = await _context.SystemPOSs.FirstOrDefaultAsync(ct => ct.TerminalID.Equals(sys.TerminalID));
                        await _context.SaveChangesAsync();
                    
                        if(systempos == null && existsInTerminal == true)
                        {
                            
                            await _context.SystemPOSs.AddAsync(sys);
                            await _context.SaveChangesAsync();
                            total = total+1;
                            Console.WriteLine("System Info:");
                            Console.WriteLine(sys.TerminalID + " - " + sys.ControlGroup); 

                        }
                        //Si existe actualizo solo checksum Crear nuevo servicio Chequeador de Checksum//
                        else if(systempos != null && existsInTerminal == true)
                        {
                            systempos.ControlCheckSum = sys.ControlCheckSum;
                            await _context.SystemPOSs.AddAsync(sys);
                            await _context.SaveChangesAsync();
                            total = total+1;
                            Console.WriteLine("System Info:");
                            Console.WriteLine(sys.TerminalID + " - " + sys.ControlGroup); 

                        }
                        else
                        {
                            no_insert = no_insert+1;
                        }
                    }
                    else
                    {
                        Console.WriteLine("TerminalID of System NO exists on Terminals");
                    }
                }

                var ListSystem = await _context.SystemPOSs.ToListAsync();
                serviceResponse.Data = _mapper.Map<List<GetSystemPOSDto>>(ListSystem);

                Console.WriteLine("System Insertados:");
                Console.WriteLine("Total:  " + total);
                Console.WriteLine("System YA existentes: ");
                Console.WriteLine("Total: " + no_insert);

                serviceResponse.Message = "Total inserted System : " + total + " Total System already existing  : " + no_insert ;
                    
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;

        }

    }
