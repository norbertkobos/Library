using Library.Domain.Entities;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Library.Api.SoapServices
{
    [ServiceContract]
    public interface IBookService
    {
        [OperationContract]
        Task<IEnumerable<Book>> GetBooks();

        [OperationContract]
        Task<Book> GetBook(int id);

        [OperationContract]
        Task AddBook(Book book);

        [OperationContract]
        Task UpdateBook(Book book);

        [OperationContract]
        Task DeleteBook(int id);
    }
}
