using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PosCFG.Dto;

public interface IFileService
{
    /*
    public void SaveFile(List<IFormFile> files, string subDirectory);
    public (string fileType, byte[] archiveData, string archiveName) FetechFiles(string subDirectory);
    public void InsertFileToDB();
    */
    
    public void DBtoFileTerminal(string path);
    public void InsertDBTerminal(string path);
    public void SaveFileToDirectory(IFormFile file, string path);
    Task<ServiceResponse<List<GetSystemPOSDto>>>  DBtoFileSystem(string path);
    Task<ServiceResponse<List<GetSystemPOSDto>>> InsertDBSystem(string path);
    
} 