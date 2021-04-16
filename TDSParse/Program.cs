


namespace TDSParse
{
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

    public class TerminalChecksums{

        public string TerminalChecksum {get; set;} = null;
        public string ControlChecksum {get; set;}  = null;
        public string ParmConnChecksum {get; set;} = null ;     
        public string [] TranConnChecksum { get; set; } = new string[12];


    }

    public class ReadTdsLog
    {
        public static void Main()
        {
            try
            {
                Dictionary<string,TerminalChecksums> SystemChecksums = new Dictionary<string, TerminalChecksums>();
                string lastTerminalConn=null;
                //TerminalChecksums tc=null;
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader(@"C:\Users\ssoch\poscfgterm\PosCFG\TDS\TDS20210312.LOG"))
               // using (StreamReader sr = new StreamReader(@"C:\Users\ssoch\Desktop\TDS\TDS20210322.LOG"))
                {
                    string TerminalChecksumPattern=@">\sREQ:\s(\d+)\sSetup\sProg=\d+,\d+\sParm=\d+,\d+\sChecksums=(\w{4})H,\w{4}H,\d+$";
                    string ConnPattern=@">\sCON:\s(\d+)\sConnected,.*$";
                    string ControlChecksumPattern=@">\sREQ:\s(\d+)\sSetupACK,\sControl\s(\w{4})H$";
                    string ConnChecksumPattern=@">\sREQ:\s(\d+)\sSetupACK,\sConnect(\d{1,})\s(\w{4})H$";

                    string line;
                    // Read and display lines from the file until the end of
                    // the file is reached.
                    
                    while ((line = sr.ReadLine()) != null)
                    {
                        
                        string terminalID=null;
                        
                        //Terminal inicia conexion "CON: terminalID"
                        Match conn=Regex.Match(line, ConnPattern, RegexOptions.Compiled);
                        if (conn.Success){

                            terminalID=conn.Groups[1].Value;

                            //tc=new TerminalChecksums();
                            SystemChecksums.TryAdd(terminalID,new TerminalChecksums());
                            //SystemChecksums[terminalID]=tc;
                            
                            //Ultima terminal que se conecto al TDS
                            lastTerminalConn=terminalID;

                            continue;
                        }
                        //TerminalChecksum "Checksums=3E32H,B075H,10"
                        Match termcheck=Regex.Match(line, TerminalChecksumPattern, RegexOptions.Compiled);
                        if (termcheck.Success){

                            terminalID=termcheck.Groups[1].Value;

                            string hexChecksum=termcheck.Groups[2].Value;
                            int decChecksum = Convert.ToInt32(hexChecksum, 16);

                            SystemChecksums.TryAdd(terminalID,new TerminalChecksums());
                            SystemChecksums[terminalID].TerminalChecksum=Convert.ToString(decChecksum);
                            
                            continue;
                        }

                        //ControlChecksum "Control 7041H"
                        Match cont=Regex.Match(line, ControlChecksumPattern, RegexOptions.Compiled);
                        if (cont.Success){

                            terminalID=cont.Groups[1].Value;

                            string hexChecksum=cont.Groups[2].Value;
                            int decChecksum = Convert.ToInt32(hexChecksum, 16);

                            SystemChecksums.TryAdd(terminalID,new TerminalChecksums());
                            SystemChecksums[terminalID].ControlChecksum=Convert.ToString(decChecksum);
                            
                            continue;
                        }

                        //TranConnChecksum "Connect[0-12] E32FH"
                        Match chk=Regex.Match(line, ConnChecksumPattern, RegexOptions.Compiled);
                        if (chk.Success){

                            terminalID=chk.Groups[1].Value;

                            int indexConn=Convert.ToInt32(chk.Groups[2].Value);
                            string hexChecksum=chk.Groups[3].Value;
                            int decChecksum = Convert.ToInt32(hexChecksum, 16);

                            SystemChecksums.TryAdd(terminalID,new TerminalChecksums());
                            if (indexConn < 1){
                                SystemChecksums[terminalID].ParmConnChecksum=Convert.ToString(decChecksum);
                            }else{
                                SystemChecksums[terminalID].TranConnChecksum[indexConn-1]=Convert.ToString(decChecksum);
                            }
                            
                            //Console.WriteLine(chk.Groups[1].Value);
                            //Console.WriteLine(decChecksum);

                            continue;
                        }

                            
                    }
                }

                foreach( KeyValuePair<string, TerminalChecksums> kvp in SystemChecksums )
                {
                    Console.WriteLine("TerminalID = {0}, TerminalChecksum = {1}, ControlChecksum = {2}, ParmConnChecksum = {3}, TranConnChecksum = [{4}]", 
                        kvp.Key, kvp.Value.TerminalChecksum, kvp.Value.ControlChecksum, kvp.Value.ParmConnChecksum, string.Join(", ", kvp.Value.TranConnChecksum) );
                }

                if ( lastTerminalConn != null){
                    //Console.WriteLine(lastTerminalConn);
                        Console.WriteLine("TerminalID = {0}, TerminalChecksum = {1}, ControlChecksum = {2}, ParmConnChecksum = {3}, TranConnChecksum = [{4}]", 
                        lastTerminalConn, SystemChecksums[lastTerminalConn].TerminalChecksum, SystemChecksums[lastTerminalConn].ControlChecksum, SystemChecksums[lastTerminalConn].ParmConnChecksum, string.Join(", ", SystemChecksums[lastTerminalConn].TranConnChecksum) );
                }


            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("No se puede leer el archivo:");
                Console.WriteLine(e.Message);
            }
        }
    }
}
