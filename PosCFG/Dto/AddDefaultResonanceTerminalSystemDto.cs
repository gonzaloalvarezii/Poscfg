using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PosCFG.Dto
{
    public class AddDefaultResonanceTerminalSystemDto : AddFullTerminalSystemDto
    {
        public AddDefaultResonanceTerminalSystemDto()
        {

            HeaderLine1 = "RESONANCE TEST PRODUCTIVO";
            HeaderLine2 = "MONZON 1839";
            HeaderLine3 = "215500380016";
            Custom4 = "HYA00001|02112352001";
            Custom5= "30400100|20523072";
            Custom6 = "HY000001|158084";
            Custom7 = "0|0";
            Custom8 = "HY000001|17300016017";
            Custom19 = "17300017010";
            Custom9 = "0|0";
            Custom10 = "0|0";
            Custom11 = "0|0";
            Custom12 = "HY000001|500029";
            Custom13 = "0|0";
            Custom14 = "0|0";
            Custom15 = "0|0";
            ParamDefault = 2;

            ControlGroup = 52;
            ConnectGroup = 700;
            ParameterGroup = 3;
            ParameterVersion = 2;
            ProgramID = 1;
            ProgramVersion = 237;
            Paquete = "ALLV201";

        }
    }
}
