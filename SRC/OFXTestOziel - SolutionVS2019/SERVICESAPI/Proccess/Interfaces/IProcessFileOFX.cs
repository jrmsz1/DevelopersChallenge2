using SERVICESAPI.DataAccess.Repository;
using SERVICESAPI.Models.Entities;
using System;
using System.Threading.Tasks;

namespace SERVICESAPI.Proccess.Interfaces
{
    public interface IProcessFileOFX : IDisposable
    {
     
        Task<bool> ProcessFile(in string content);

        OFX_BATCH ReadFile_OFX(in string content, out string errors, bool seeStack = false);

        void setUOW(IUnitOfWork uow);
    }
}
