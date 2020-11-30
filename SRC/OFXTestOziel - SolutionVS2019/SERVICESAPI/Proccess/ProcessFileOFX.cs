using AutoMapper;
using SERVICESAPI.DataAccess.Repository;
using SERVICESAPI.Models.Entities;
using SERVICESAPI.Proccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SERVICESAPI.Proccess
{
    public class ProcessFileOFX : IProcessFileOFX
    {
        private readonly IMapper _mapper;

        public ProcessFileOFX(IMapper mapper)
        {
            _mapper = mapper;
        }

        private IUnitOfWork _uow;

        public void setUOW(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Task<bool> ProcessFile(in string content)
        {

            var ofx_batch = ReadFile_OFX(content, out string errors, true);

            if (errors == string.Empty)
                if (ofx_batch != null)
                    ImportOFXBatch(ref ofx_batch);

            //var dto = _mapper.Map<List<OFX_STMTTRN_DTO>>(lista);

            return Task.FromResult(true);
        }

        public void ImportOFXBatch(ref OFX_BATCH ofx_batch)
        {

            DateTimeOffset DTSTART;
            DateTimeOffset DTEND;

            if (ofx_batch.STMTTRNS.Count == 0)
                return;
            else if (ofx_batch.STMTTRNS.Count == 1)
            {
                DTSTART = ofx_batch.STMTTRNS.ElementAt(0).DTPOSTED;
                DTEND = ofx_batch.STMTTRNS.ElementAt(0).DTPOSTED;
            }
            else
            {
                DTSTART = ofx_batch.STMTTRNS.ElementAt(0).DTPOSTED;
                DTEND = ofx_batch.STMTTRNS.ElementAt(ofx_batch.STMTTRNS.Count - 1).DTPOSTED;
            }

            //Future check
            //bool Ordened = true;

            int BANKID = ofx_batch.BANKID;
            long ACCTID = ofx_batch.ACCTID;

            var Account = _uow.GetRepository<ACCOUNT>().Get(a => a.BANKID == BANKID && a.ACCTID == ACCTID).FirstOrDefault();

          

            if (Account != null)
            {

                //Index
                // t.AccountId, t.DTPOSTED, t.TRNTYPE, t.TRNAMT
                var listDB = _uow.GetRepository<OFX_STMTTRN>().Get(a => a.AccountId == Account.Id && a.DTPOSTED >= DTSTART && a.DTPOSTED <= DTEND).ToList();
                List<OFX_STMTTRN> STMTTRNS = new List<OFX_STMTTRN>(ofx_batch.STMTTRNS);
                List<OFX_STMTTRN> Duplicates = new List<OFX_STMTTRN>();

                ofx_batch.STMTTRNS = null;

                if (listDB.Count > 0)
                {
                    for (int i = 0; i < STMTTRNS.Count; i++)
                    {
                        STMTTRNS[i].AccountId = Account.Id;
                        if (listDB.Any(l => l.ToString() == STMTTRNS[i].ToString()))
                            Duplicates.Add(STMTTRNS[i]);
                    }
                }

                Duplicates.ForEach((e) => { STMTTRNS.Remove(e); });

                if (STMTTRNS.Count > 0)
                {
                    _uow.GetRepository<OFX_BATCH>().Insert(ofx_batch);
                    _uow.Commit();
                    try
                    {

                        //var BatchID = _uow.Context.Entry(ofx_batch).GetDatabaseValues();
                        //var value = ((OFX_BATCH)(object)ofx_batch).Id;

                        for (int i = 0; i < STMTTRNS.Count; i++)
                        {
                            STMTTRNS[i].AccountId = Account.Id;
                            STMTTRNS[i].BATCHId = ofx_batch.Id;
                        }

                        _uow.GetRepository<OFX_STMTTRN>().InsertMany(STMTTRNS);
                        _uow.Commit();

                    }
                    catch (Exception ex)
                    {
                        var ar = ex.Message;
                    }
                }
            }
        }

        //I could to make with Reflection and CustomAttribute, too.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="content">File content OFX</param>
        /// <param name="errors">Output with errors</param>
        /// <returns></returns>
        public OFX_BATCH ReadFile_OFX(in string content, out string errors, bool seeStack = false)
        {
            bool isHeader;
            bool isValue;

            errors = string.Empty;


            int currentLine = 0;
            string currentRegion = string.Empty;


            //Regions order
            List<string> Regions = new List<string> { "<OFX>", "<SIGNONMSGSRSV1>", "<SONRS>", "<STATUS>", "<BANKMSGSRSV1>", "<STMTTRNRS>", "<STATUS>", "<STMTRS>", "<BANKACCTFROM>", "<BANKTRANLIST>", "<STMTTRN>" };

            Stack<string> tags = new Stack<string>();

            var ofx = new OFX_BATCH { DT_CREATED = DateTimeOffset.UtcNow };

            if (seeStack) Console.WriteLine($"Tree OFX:{Environment.NewLine}");

            OFX_STMTTRN stmttrn = default(OFX_STMTTRN);

            IList<OFX_STMTTRN> listOFX_STMTTRN = new List<OFX_STMTTRN>();

            try
            {
                //Pattern end file OFX - "\r\n"
                foreach (var line in content.Split(new char[] { '\r', '\n' }))
                {
                    currentLine++;

                    if (!string.IsNullOrEmpty(line))
                    {
                        isHeader = false;
                        if (!line.StartsWith("<"))
                        {
                            isHeader = true;
                        }

                        if (isHeader)
                        {
                            var tagHeader = line.Substring(0, line.IndexOf(":"));
                            var tagHeaderValue = line.Remove(0, line.IndexOf(":") + 1).Trim();

                            switch (tagHeader)
                            {
                                case "OFXHEADER":
                                    ofx.OFXHEADER = Convert.ToInt64(tagHeaderValue);
                                    break;

                                case "DATA":
                                    ofx.DATA = tagHeaderValue;
                                    break;

                                case "VERSION":
                                    ofx.VERSION = tagHeaderValue;
                                    break;

                                case "SECURITY":
                                    ofx.SECURITY = tagHeaderValue;
                                    break;

                                case "ENCODING":
                                    ofx.ENCODING = tagHeaderValue;
                                    break;

                                case "CHARSET":
                                    ofx.ENCODING = tagHeaderValue;
                                    break;

                                case "COMPRESSION":
                                    ofx.COMPRESSION = tagHeaderValue;
                                    break;

                                case "OLDFILEUID":
                                    ofx.OLDFILEUID = tagHeaderValue;
                                    break;

                                case "NEWFILEUID":
                                    ofx.NEWFILEUID = tagHeaderValue;
                                    break;
                            }
                        }
                        else
                        {

                            string tag = line.Substring(0, line.IndexOf(">") + 1);
                            string tagValue = string.Empty;

                            isValue = false;


                            if (!tag.StartsWith("</"))
                            {

                                isValue = line.Length - (line.IndexOf(">") + 1) > 0;

                                if (!isValue)
                                {
                                    tags.Push(tag);
                                    if (seeStack) Console.WriteLine($"{new string('\t', tags.Count() - 1)} PUSH {tag}");

                                    #region Push                                   
                                    currentRegion = tags.Peek();

                                    if (currentRegion == "<STMTTRN>")
                                        stmttrn = new OFX_STMTTRN();

                                }
                                else
                                {
                                    tagValue = line.Remove(0, line.IndexOf(">") + 1).Trim();

                                    if (currentRegion == "<STATUS>")
                                    {
                                        //It could be Regions
                                        //Preview element of stack
                                        //Top == elemtent at 0
                                        if (tags.ElementAt(1) == "<SONRS>")
                                        {
                                            if (tag == "<CODE>")
                                                ofx.SONRS_STA_CODE = Convert.ToInt32(tagValue);
                                            else if (tag == "<SEVERITY>")
                                                ofx.SONRS_STA_SEVERITY = tagValue;
                                        }
                                        if (tags.ElementAt(1) == "<STMTTRNRS>")
                                        {
                                            if (tag == "<CODE>")
                                                ofx.STMTTRNRS_STA_CODE = Convert.ToInt32(tagValue);
                                            else if (tag == "<SEVERITY>")
                                                ofx.STMTTRNRS_STA_SEVERITY = tagValue;
                                        }
                                    }
                                    else if (currentRegion == "<STMTTRNRS>")
                                    {
                                        if (tag == "<TRNUID>")
                                        {
                                            ofx.STMTTRNRS_TRNUID = Convert.ToInt32(tagValue);
                                        }
                                    }
                                    else if (currentRegion == "<SONRS>")
                                    {

                                        if (tag == "<DTSERVER>")
                                        {

                                            var date = ParseAsDateTimeOffset(tag, tagValue, out string TMZ, out errors);
                                            if (date.HasValue)
                                            {
                                                ofx.DTSERVER = date.Value;
                                                ofx.DTSERVER_TMZ = TMZ;
                                            }
                                        }
                                        else if (tag == "<LANGUAGE>")
                                        {
                                            ofx.LANGUAGE = tagValue;
                                        }
                                    }
                                    else if (currentRegion == "<STMTRS>")
                                    {
                                        if (tag == "<CURDEF>")
                                        {
                                            ofx.CURDEF = tagValue;
                                        }
                                    }
                                    else if (currentRegion == "<BANKACCTFROM>")
                                    {
                                        if (tag == "<BANKID>")
                                        {
                                            ofx.BANKID = Convert.ToInt32(tagValue);
                                        }
                                        else if (tag == "<ACCTID>")
                                        {
                                            ofx.ACCTID = Convert.ToInt64(tagValue);
                                        }
                                        else if (tag == "<ACCTTYPE>")
                                        {
                                            ofx.ACCTTYPE = tagValue;
                                        }
                                    }
                                    else if (currentRegion == "<BANKTRANLIST>")
                                    {

                                        if (tag == "<DTSTART>")
                                        {
                                            var date = ParseAsDateTimeOffset(tag, tagValue, out string TMZ, out errors);
                                            if (date.HasValue)
                                            {
                                                ofx.DTSTART = date.Value;
                                                ofx.DTSTART_TMZ = TMZ;
                                            }
                                        }
                                        else if (tag == "<DTEND>")
                                        {
                                            var date = ParseAsDateTimeOffset(tag, tagValue, out string TMZEnd, out errors);
                                            if (date.HasValue)
                                            {
                                                ofx.DTEND = date.Value;
                                                ofx.DTEND_TMZ = TMZEnd;
                                            }
                                        }
                                    }
                                    else if (currentRegion == "<STMTTRN>")
                                    {
                                        if (tag == "<TRNTYPE>")
                                        {
                                            stmttrn.TRNTYPE = tagValue;
                                        }
                                        else if (tag == "<DTPOSTED>")
                                        {
                                            var date = ParseAsDateTimeOffset(tag, tagValue, out string TMZ, out errors);
                                            if (date.HasValue)
                                            {
                                                stmttrn.DTPOSTED = date.Value;
                                                stmttrn.DTPOSTED_TMZ = TMZ;
                                            }
                                        }
                                        else if (tag == "<TRNAMT>")
                                        {
                                            stmttrn.TRNAMT = Convert.ToDecimal(tagValue.Replace(".",","));
                                        }
                                        else if (tag == "<MEMO>")
                                        {
                                            stmttrn.MEMO = tagValue;
                                        }
                                    }
                                    else if (currentRegion == "<LEDGERBAL>")
                                    {
                                        if (tag == "<BALAMT>")
                                        {
                                            ofx.BALAMT = Convert.ToDecimal(tagValue.Replace(".",","));
                                        }
                                        else if (tag == "<DTASOF>")
                                        {
                                            var date = ParseAsDateTimeOffset(tag, tagValue, out string TMZ, out errors);
                                            if (date.HasValue)
                                            {
                                                ofx.DTASOF = date.Value;
                                                ofx.DTASOF_TMZ = TMZ;
                                            }
                                        }
                                    }
                                }

                                #endregion
                            }
                            else
                            {

                                var checkStack = tags.Pop();
                                if (seeStack) Console.WriteLine($"{new string('\t', tags.Count())} POP {tag}");

                                if (tags.Count > 0)
                                    currentRegion = tags.Peek();

                                if (tag == "</STMTTRN>" && checkStack == "<STMTTRN>")
                                {
                                    if (stmttrn != null)
                                        listOFX_STMTTRN.Add(stmttrn);

                                    stmttrn = null;
                                }
                            }

                            if (seeStack && isValue) Console.WriteLine($"{new string('\t', tags.Count())} CurrentRegion {currentRegion} - tag: {tag} - value: {tagValue}");

                        }
                    }
                }
            }
            catch (ArgumentOutOfRangeException ofRange)
            {
                errors = $"Error Parser OFX line {currentLine} - {ofRange.Message}";
            }
            catch (FormatException eFormat)
            {
                errors = $"Error Parser OFX line {currentLine} - {eFormat.Message}";
            }
            catch (OverflowException eOver)
            {
                errors = $"Error Parser OFX line {currentLine} - {eOver.Message}";
            }
            catch (Exception Err)
            {
                errors = $"Error Parser OFX line {currentLine} - {Err.Message}";
            }
            finally
            {
                if (errors == string.Empty)
                    if (tags.Count > 0)
                    {
                        errors = $"Error Parser OFX - Bad Format.";
                    }
            }

            ofx.STMTTRNS = listOFX_STMTTRN;

            return ofx;
        }

        private DateTimeOffset? ParseAsDateTimeOffset(in string tag, in string value, out string TMZ, out string errorDate)
        {

            TMZ = string.Empty;
            string longDate = string.Empty;
            string milliseconds = string.Empty;
            string fuse = string.Empty;
            errorDate = string.Empty;


            string pattern = @"(?<longDate>\d{8,14})(?<milliseconds>\.[0-9]{3})?\[(?<fuse>[+-]\d{1,2})?\:(?<tmz>\w+)\]";

            //https://www.ofx.net/downloads/OFX%202.2.pdf

            //See PAGE 89!!!
            ///YYYYMMDD
            ///YYYYMMDDHHMMSS
            ///YYYYMMDDHHMMSS.XXX
            ///19961005132200.124[-5:EST]
            //https://schemas.liquid-technologies.com/OFX/2.1.1/?page=datetimetype.html
            //string pattern = @"[0-9]{4}((0[1-9])|(1[0-2]))((0[1-9])|([1-2][0-9])|(3[0-1]))|[0-9]{4}((0[1-9])|(1[0-2]))((0[1-9])|([1-2][0-9])|(3[0-1]))(([0-1][0-9])|(2[0-3]))[0-5][0-9](([0-5][0-9])|(60))|[0-9]{4}((0[1-9])|(1[0-2]))((0[1-9])|([1-2][0-9])|(3[0-1]))(([0-1][0-9])|(2[0-3]))[0-5][0-9](([0-5][0-9])|(60))\.[0-9]{3}|[0-9]{4}((0[1-9])|(1[0-2]))((0[1-9])|([1-2][0-9])|(3[0-1]))(([0-1][0-9])|(2[0-3]))[0-5][0-9](([0-5][0-9])|(60))\.[0-9]{3}(\[[\+\-]?.+(:.+)?\])?";
            Match match = Regex.Match(value, pattern, RegexOptions.IgnoreCase);

            longDate = match.Groups["longDate"].Value;

            fuse = match.Groups["fuse"].Value;
            TMZ = match.Groups["tmz"].Value;
            milliseconds = match.Groups["milliseconds"].Value;


            if (match.Success)
            {
                try
                {
                    //Console.WriteLine("Local time zone: {0}\n", TimeZoneInfo.Local.DisplayName);
                    //est = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    //return TimeZoneInfo.ConvertTime(new DateTimeOffset(Convert.ToInt64(longDate), new TimeSpan(Convert.ToInt32(fuse), 0, 0)), est);
                    if (longDate.Length == 8)
                        return new DateTimeOffset(new DateTime(Convert.ToInt32(longDate.AsSpan(0, 4).ToString()), Convert.ToInt32(longDate.AsSpan(4, 2).ToString()), Convert.ToInt32(longDate.AsSpan(6, 2).ToString())), TimeSpan.Zero);
                    else if (longDate.Length == 14)
                    {

                        if (!string.IsNullOrEmpty(fuse))
                        {
                            if (!string.IsNullOrEmpty(milliseconds))
                                return new DateTimeOffset(new DateTime(Convert.ToInt32(longDate.AsSpan(0, 4).ToString()), Convert.ToInt32(longDate.AsSpan(4, 2).ToString()), Convert.ToInt32(longDate.AsSpan(6, 2).ToString()), Convert.ToInt32(longDate.AsSpan(8, 2).ToString()), Convert.ToInt32(longDate.AsSpan(10, 2).ToString()), Convert.ToInt32(longDate.AsSpan(12, 2).ToString()), Convert.ToInt32(milliseconds.AsSpan(0, 3).ToString())), new TimeSpan(Convert.ToInt32(fuse), 0, 0));
                            else
                                return new DateTimeOffset(new DateTime(Convert.ToInt32(longDate.AsSpan(0, 4).ToString()), Convert.ToInt32(longDate.AsSpan(4, 2).ToString()), Convert.ToInt32(longDate.AsSpan(6, 2).ToString()), Convert.ToInt32(longDate.AsSpan(8, 2).ToString()), Convert.ToInt32(longDate.AsSpan(10, 2).ToString()), Convert.ToInt32(longDate.AsSpan(12, 2).ToString())), new TimeSpan(Convert.ToInt32(fuse), 0, 0));
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(milliseconds))
                                return new DateTimeOffset(new DateTime(Convert.ToInt32(longDate.AsSpan(0, 4).ToString()), Convert.ToInt32(longDate.AsSpan(4, 2).ToString()), Convert.ToInt32(longDate.AsSpan(6, 2).ToString()), Convert.ToInt32(longDate.AsSpan(8, 2).ToString()), Convert.ToInt32(longDate.AsSpan(10, 2).ToString()), Convert.ToInt32(longDate.AsSpan(12, 2).ToString()), Convert.ToInt32(milliseconds.AsSpan(0, 3).ToString())), TimeSpan.Zero);
                            else
                                return new DateTimeOffset(new DateTime(Convert.ToInt32(longDate.AsSpan(0, 4).ToString()), Convert.ToInt32(longDate.AsSpan(4, 2).ToString()), Convert.ToInt32(longDate.AsSpan(6, 2).ToString()), Convert.ToInt32(longDate.AsSpan(8, 2).ToString()), Convert.ToInt32(longDate.AsSpan(10, 2).ToString()), Convert.ToInt32(longDate.AsSpan(12, 2).ToString())), TimeSpan.Zero);
                        }
                    }

                }
                catch (ArgumentException)
                {
                    errorDate = $"Bad format DateTime - Tag {tag}.";
                }
                catch (TimeZoneNotFoundException)
                {
                    errorDate = $"Bad format DateTime - TimeZone - Tag {tag}.";
                }
                catch (InvalidTimeZoneException)
                {
                    errorDate = $"Bad format DateTime - Invalid TimeZone - Tag {tag}.";
                    //throw new Exception("Unable to retrieve the Eastern Standard time zone.");
                }
            }
            else
            {
                //throw new Exception($"Bad format DateTime - Tag {tag}.");
                errorDate = $"Bad format DateTime - Tag {tag}.";
            }
            return new Nullable<DateTimeOffset>();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _uow.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}