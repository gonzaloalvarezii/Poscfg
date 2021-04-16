using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PosCFG.Dto
{
    public class AddDefaultHandyTerminalSystemDto: AddFullTerminalSystemDto
    {

        public AddDefaultHandyTerminalSystemDto() {
           
            HeaderLine1 = "HANDY TERMINAL INACTIVA";
            HeaderLine2 = "RIZAL 3555";
            HeaderLine3 = "218417260011";
            Custom4 = "0|0";
            Custom5 = "0|0";
            Custom6 = "0|0";
            Custom7 = "0|0";
            Custom8 = "0|0";
            Custom19 = "0";
            Custom9 = "0|0";
            Custom10 = "0|0";
            Custom11 = "0|0";
            Custom12 = "0|0";
            Custom13 = "0|0";
            Custom14 = "0|0";
            Custom15 = "0|0";
            ParamDefault = 1;
            ControlGroup = 52;
            ConnectGroup = 700;
            ParameterGroup = 3;
            ParameterVersion = 2;
            ProgramID = 1;
            ProgramVersion = 236;
            Paquete = "AL210223";
        }
    }
}
