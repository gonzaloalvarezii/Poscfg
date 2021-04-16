using System;

namespace PosCFG.JPOS{

    public class ca_name{
        public char[] ca;
        public char[] city;
        public char[] country;

        public ca_name(){

            ca=null;
            city=null;
            country=null;

        }
        public ca_name(string card_acceptor){

            ca = card_acceptor.ToCharArray(0, 25);
            city = card_acceptor.ToCharArray(25, 13);
            country = card_acceptor.ToCharArray(38, 2);

        }

    }

    public class jpos{

        private ca_name ca;
        private char[] mcc;
        private char[] pf_id;
        private char[] visa_spnsrd_mercht;
        private char[] amex_id_comercio;

        public jpos(string ca, string mcc, string pf_id, string visa_spnsrd_mercht, string amex_id_comercio){

            this.ca=new ca_name(ca);
            this.mcc=mcc.ToCharArray(0, 4);
            this.pf_id=pf_id.ToCharArray(0, 11);
            this.visa_spnsrd_mercht=visa_spnsrd_mercht.ToCharArray(0, 15);
            this.amex_id_comercio=amex_id_comercio.ToCharArray(0, 20);

        }

        public jpos(){

            this.ca=null;
            this.mcc=null;
            this.pf_id=null;
            this.visa_spnsrd_mercht=null;
            this.amex_id_comercio=null;

        }

        public string getCA(){
            return 
                new string(ca?.ca  == null ? (new char[]{}) : ca.ca )
                +new string(ca?.city  == null ? (new char[]{}) : ca.city)
                +new string(ca?.country  == null ? (new char[]{}) : ca.country);
        }

        public string getMcc(){
            return new string(mcc == null ? (new char[]{}) : mcc );
        }

        public string getPf_id(){
            return new string(pf_id  == null ? (new char[]{}) : pf_id);
        }

        public string getVisa_spnsrd_mercht(){
            return new string(visa_spnsrd_mercht  == null ? (new char[]{}) : visa_spnsrd_mercht);
        }

        public string getAmex_id_comercio(){
            return new string(amex_id_comercio  == null ? (new char[]{}) : amex_id_comercio);
        }

        public string genSysconfigValue_CA(){
            
            return 
                new string(ca?.ca  == null ? (new char[]{}) : ca.ca )
                +new string(ca?.city  == null ? (new char[]{}) : ca.city)
                +new string(ca?.country  == null ? (new char[]{}) : ca.country)
                +","
                +new string(mcc == null ? (new char[]{}) : mcc )
                +","
                +new string(pf_id  == null ? (new char[]{}) : pf_id)
                +","
                +new string(visa_spnsrd_mercht  == null ? (new char[]{}) : visa_spnsrd_mercht)
                +","
                +new string(amex_id_comercio  == null ? (new char[]{}) : amex_id_comercio);

        }

        public void setSysconfigValue(string[] jpos_values){

            for (int index = 0; index < jpos_values.Length; index++)
            {
                if ( jpos_values[index] != null && jpos_values[index].Length > 0 )
                {
                    int tam=jpos_values[index].Length;
                    switch (index)
                    { 
                        case 0:
                            for (int i=tam; i < 40; i++ )
                            {
                                jpos_values[index]+="";
                            }
                            ca=new ca_name(jpos_values[index]);
                            break;
                        case 1:
                            for (int i=tam; i < 4; i++ )
                            {
                                jpos_values[index]+="";
                            }
                            mcc=jpos_values[index].ToCharArray(0, 4);
                            break;
                        case 2:
                            for (int i=tam; i < 11; i++ )
                            {
                                jpos_values[index]+="";
                            }
                            pf_id=jpos_values[index].ToCharArray(0, 11);
                            break;
                        case 3:
                            for (int i=tam; i < 15; i++ )
                            {
                                jpos_values[index]+="";
                            }
                            visa_spnsrd_mercht=jpos_values[index].ToCharArray(0, 15);
                            break;
                        case 4:
                            for (int i=tam; i < 20; i++ )
                            {
                                jpos_values[index]+="";
                            }
                            amex_id_comercio=jpos_values[index].ToCharArray(0, 20);
                            break;
                        default:
                            Console.WriteLine("Opciones de 0 a 4");
                            break;
                    }


                }

            }

        }

        public void setSysconfigValue_CA(string sys_value){

            string[] jpos_values=null;
            char[] Separator = new char[] {','};

            jpos_values=sys_value.Split(Separator, StringSplitOptions.None);

            if (jpos_values.Length == 5){
                setSysconfigValue(jpos_values);
            }
        }


        public void updateSysconfigValue_CA(string[] parameters){
            setSysconfigValue(parameters);
        }

    }
 

}