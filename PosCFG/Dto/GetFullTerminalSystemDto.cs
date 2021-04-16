namespace PosCFG.Dto
{
    public class GetFullTerminalSystemDto
    {
        //TERMINAL
        public string TerminalID { get; set; }
        //Nro de Serie (HW)
        public string SerialNumber { get; set; }

        //rst-69
        public string ParmConnectTime { get; set;}
        
        //Nombre del Comercio
        //custom1
        public string HeaderLine1 { get; set; }
        //custom2
        //Direccion
        public string HeaderLine2 { get; set; }
        //RUT
        //custom3
        public string HeaderLine3 { get; set; }
        //visanet
        //custom4
        public string Custom4 {get; set;}
        //first data
        //custom5
        public string Custom5 {get; set;}
        //oca
        //custom6
        public string Custom6 {get; set;}
        //creditos directos
        //custom7
        public string Custom7 {get; set;}
        //cabal
        //custom8
        public string Custom8 {get; set;}
        //creditel
        //custom9
        public string Custom9 {get; set;}
        //passcard
        //custom10
        public string Custom10 {get; set;}
        //edenred
        //custom11
        public string Custom11 {get; set;}
        //anda
        //custom12
        public string Custom12 {get; set;}
        //amex
        //custom13
        public string Custom13 {get; set;}
        //club del este
        //custom14
        public string Custom14 {get; set;}
        //mides
        //custom15
        public string Custom15 {get; set;}
        //brou 
        //custom16
        public string Custom16 {get; set;}
        //cabal u$s
        //custom19
        public string Custom19 {get; set;}
        
        //SYSTEM
        //system 
        //Tipo de comercio
        public int merchanType {get; set;}
        //actualizar datos de Terminal
        public bool TerminalChecksum { get; set; }
        //simcard?
        //public int ControlGroup{get; set;}
        //actualizar tipo comercio
        public bool ControlCheckSum{get; set;}
        //Parametergroup
        public int ParameterGroup{get; set;}
        //cargar parametro de conexion
        public bool ParameterReload{get; set;}
        //parameter version
        public int ParameterVersion{get; set;}
        //Program ID
        public int ProgramID{get; set;}
        //Cargar programa
        public bool ProgramReload{get; set;}
        //Program Version
        public int ProgramVersion{get; set;}
        //Package Name
        public string Paquete{get; set;}
        //Connect Group
        public int ConnectGroup{get; set;}
        //Actualizar parametros de conexion
        public bool ParmConnChecksum{get; set;}

        //Campos JPOS
        //JPOS
        //campo 43
        public string ca {get; set;}
        //campo18
        public string mcc {get; set;}
        //campo 104
        public string pf_id {get; set;}
        //RST-34
        public string visa_spnsrd_mercht {get; set;}
        public string amex_id_comercio {get; set;}
        //activar Terminal en JPOS
        public bool enabled_JPOS {get; set;}




    }
}