using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SERVICESAPI.Models.Entities
{
    /// <summary>
    /// OFX Batch, main class of credit/debit transactions at a bank
    /// </summary>
    public class OFX_BATCH : EntityKeyId
    {



        #region HEADER FILE OFX
        public  long OFXHEADER { get; set; }
        public string DATA { get; set; }
        public string VERSION { get; set; }
        public string SECURITY { get; set; }
        public string ENCODING { get; set; }
        public string CHARSET { get; set; }
        public string COMPRESSION { get; set; }
        public string OLDFILEUID { get; set; }
        public string NEWFILEUID { get; set; }
        #endregion

        
        public DateTimeOffset DT_CREATED { get; set; }

        #region OFX

        #region SIGNONMSGSRSV1

        public int SONRS_STA_CODE { get; set; }

        public string SONRS_STA_SEVERITY { get; set; }

        public DateTimeOffset? DTSERVER { get; set; }

        public string DTSERVER_TMZ { get; set; }

        public string LANGUAGE { get; set; }

        #endregion

        #region BANKMSGSRSV1

        #region STMTTRNRS

        public int STMTTRNRS_TRNUID { get; set; }
        public int STMTTRNRS_STA_CODE { get;set;}
        public string STMTTRNRS_STA_SEVERITY { get; set; }

        #region STMTRS

        public string CURDEF { get; set; }


        #region BANKACCTFROM
            public int BANKID { get; set; }

            public long ACCTID { get; set; }

            public string ACCTTYPE { get; set; }

        #endregion


        #region BANKTRANLIST

        public ICollection<OFX_STMTTRN> STMTTRNS { get; set; }

        /// <summary>
        /// BANKTRANLIST - Date start
        /// </summary>
        public DateTimeOffset DTSTART { get; set; }

        /// <summary>
        /// BANKTRANLIST - Date start with global timezone
        /// </summary>
        public string DTSTART_TMZ { get; set; }

        /// <summary>
        /// BANKTRANLIST - Date end
        /// </summary>
        public DateTimeOffset DTEND { get; set; }

        /// <summary>
        /// BANKTRANLIST - Date end with global timezone
        /// </summary>
        public string DTEND_TMZ { get; set; }     

        #endregion

        #region LEDGERBAL
        public decimal BALAMT { get; set; } = default;

        public DateTimeOffset DTASOF { get; set; } = default;
        public string DTASOF_TMZ { get; set; } = default;
        #endregion

        #endregion

        #endregion

        #endregion

        #endregion
    }
}
