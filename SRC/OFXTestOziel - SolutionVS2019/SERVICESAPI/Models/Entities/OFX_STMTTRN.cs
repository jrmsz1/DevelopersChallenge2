using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SERVICESAPI.Models.Entities
{
    /// <summary>
    /// Class Transaction Credit/Debit
    /// </summary>
    public class OFX_STMTTRN : EntityKeyId
    {

        public long AccountId { get; set; }
        public ACCOUNT ACCOUNT { get; set; }


        public long BATCHId { get; set; }
        public OFX_BATCH BATCH {get;set;}

        /// <summary>
        ///  Type transaction (Credit/Debit)
        /// </summary>      
        public string TRNTYPE { get; set; }

        //https://stackoverflow.com/questions/40671160/converting-utc-datetime-to-est-datetime-with-correct-offset
        /// <summary>
        /// Date transaction
        /// </summary>
        public DateTimeOffset DTPOSTED { get; set; }

        //https://pt.stackoverflow.com/questions/166588/hor%C3%A1rio-atual-dos-estados-do-brasil-c/166600
       /// <summary>
       /// Trn Time Zone
       /// </summary>
        public string DTPOSTED_TMZ { get; set; }

        /// <summary>
        /// Value
        /// </summary>

        public decimal TRNAMT { get; set; }

        /// <summary>
        /// Note description
        /// </summary>
        public  string MEMO { get; set; }


        public DateTimeOffset? DT_RECONCILIATION { get; set; }
 


        public override string ToString()
        {
            return $"{AccountId}{DTPOSTED.ToFileTime()}{TRNTYPE.Trim()}{TRNAMT}";
        }

    }
}