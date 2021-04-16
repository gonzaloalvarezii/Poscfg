using PosCFG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PosCFG.Dto
{
    public static class GetConnectTime
    {

        public static string getConnectTime(PosCFGDbContext _context)
        {

            string paramConnect = "0000";
            string formatString = "yyyyMMddHHmmss";

            Terminal t = _context.Terminals.OrderByDescending(x => x.ParmConnectTime).Take(1).FirstOrDefault();

            if (t != null)
            {

                DateTime aux = t.TranConnectTime;

                //si es el mismo dia, le sumo un minuto al connect time
                //if (aux.ToString("yyyyMMdd") == DateTime.Now.ToString("yyyyMMdd"))

                if (t.ParmConnectTime != null)
                {
                    if (t.ParmConnectTime != "2359")
                    {
                        //valido como hora para contar los minutos
                        aux = DateTime.ParseExact(aux.ToString("yyyyMMdd") + t.ParmConnectTime + "00", formatString, null);
                        aux = aux.AddMinutes(1);
                        paramConnect = agregarCeroUnDigito(aux.Hour.ToString()) + agregarCeroUnDigito(aux.Minute.ToString());

                    }
                }

            }


            return paramConnect;
        }
        public static string agregarCeroUnDigito(string num)
        {

            if (num.Length == 1)
            {
                return "0" + num;
            }
            else
            {
                return num;
            }

        }
    }
}
