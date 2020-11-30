using System;

namespace SERVICESAPI.DTOs
{

    public class OFX_STMTTRN_DTO
    {
        /// <summary>
        ///  Tipo da Transação
        /// </summary>      
        public string TIPOTRN { get; set; }

        //https://stackoverflow.com/questions/40671160/converting-utc-datetime-to-est-datetime-with-correct-offset
        /// <summary>
        /// Data da transação
        /// </summary>
        public DateTimeOffset DATATRN { get; set; }

        //https://pt.stackoverflow.com/questions/166588/hor%C3%A1rio-atual-dos-estados-do-brasil-c/166600
        /// <summary>
        /// Time zone da data da transação
        /// </summary>
        public string DATATRN_TMZ { get; set; }

        /// <summary>
        /// Valor da transação
        /// </summary>
        public decimal VALOR { get; set; }

        /// <summary>
        /// Descrição da transação
        /// </summary>
        public string DESCRICAO { get; set; }

        /// <summary>
        /// Data da reconciliação
        /// </summary>
        public DateTimeOffset? DATA_RECONCILIACAO { get; set; }

    }
}