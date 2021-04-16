namespace PosCFG.Dto
{
    public class GetTerminalsExternalClientListDto
    {
        public string TerminalID { get; set; }
        public string SerialNumber { get; set; }
        public int terminalEnabled { get; set; } //Actualizar Nombre
    }
}