using System;
using System.Collections.Generic;

namespace SERVICESAPI.Models.Entities
{
    public class ACCOUNT : EntityKeyId
    {

        public string FIRSTNAME { get; set; }

        public string LASTNAME { get; set; }

        public string IDENTIFICATION { get; set; }

        public string COUNTRY { get; set; }

        public int BANKID { get; set; }

        public long ACCTID {get;set;}

        public DateTimeOffset DATECREATED { get; set; }        
        
        public ICollection<OFX_STMTTRN> STMTTRNS { get; set; }

    }
}
