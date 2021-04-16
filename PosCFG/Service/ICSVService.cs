    using System.Collections.Generic;
    using PosCFG.Dto;

    namespace PosCFG.Service{
    public interface ICSVService
    {
        //TERMINALS.TXT
        public List<CSVService.TerminalsLine> GetTerminalsTxt();
        public List<CSVService.TerminalsLine> ReadTerminalsFile();
        public void AppendTerminalsLine(object newline);
        public void UpdateTerminalsLine(string id, object tline );
        public void DeleteTerminalsLine(string id);
        
        //SYSTEM.TXT
        public List<CSVService.SystemLine> GetSystemTxt();
        public List<CSVService.SystemLine> ReadSystemFile();
        public void AppendSystemLine(object newline);
        public void UpdateSystemLine(string id, object sline, bool rstTerminalChecksum, bool rstTranConnChecksum,bool rstControlChecksum, bool newPackage);
        public void DeleteSystemLine(string id);
        public bool TerminalInitialized(string id);

        //ConnectGroups
        public List<string> getConnectGroups();

        //ControlGroups
        public List<CSVService.TipoComercio> getControlGroups();

        //ProgramIDs
        public List<string> getProgramIDs();

        //ProgramVersions
        public List<string> getProgramVersionsByID(string id);

        //ParameterGroups
        public List<string> getParameterGroups();

        //ParameterVersion
        public List<string> getParameterVersionsByID(string id);

        public Dictionary<string,CSVService.TerminalChecksums> checksumsUpdate();

    }

}