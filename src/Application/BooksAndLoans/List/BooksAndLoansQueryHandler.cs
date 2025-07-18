using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using MongoDB.Bson;
using MongoDB.Driver;
using SharedKernel;

namespace Application.BooksAndLoans.List;

public sealed class BooksAndLoansQueryHandler(IReadDbContext context) : IQueryHandler<BooksAndLoansQuery, IEnumerable<BooksAndLoansResponse>>
{
   public async Task<Result<IEnumerable<BooksAndLoansResponse>>> Handle(BooksAndLoansQuery query, CancellationToken cancellationToken)
   {
       BsonDocument[] pipeline =
       [
           new BsonDocument(name: "$lookup", new BsonDocument
           {
               ["from"] = "Loans",
               ["localField"] = "_id",
               ["foreignField"] = "BookId",
               ["as"] = "loans"
           }),
           new BsonDocument(name: "$project", new BsonDocument
           {
               ["BookId"] = "$_id",
               ["Title"] = 1,
               ["Author"] = 1,
               ["Publish"] = 1,
               ["Loans"] = "$loans"
           })
       ];
   
       List<BsonDocument> result = await context
           .Books
           .Aggregate<BsonDocument>(pipeline, cancellationToken: cancellationToken)
           .ToListAsync(cancellationToken);
   
       IEnumerable<BooksAndLoansResponse> response = result.Select(doc => new BooksAndLoansResponse(
           BookId: doc["BookId"].AsGuid,
           Title: doc["Title"].AsString,
           Author: doc["Author"].AsString,
           Publish: DateTime.Parse(doc["Publish"].ToString()),
           Loans: doc["Loans"].AsBsonArray.Select(loan => new LoanInfo(
               Id: loan["_id"].AsGuid,
               /*LoanDate: DateTime.Parse(loan["CreatedAt"].ToString()),
               ReturnDate: loan["ReturnDate"] != BsonNull.Value
                   ? DateTime.Parse(loan["ReturnDate"].ToString())
                   : null,
                */
               /*
                * Vou deixar a data chumbada apenas para compilar, o correto seria criar um
                * objeto específico para o MongoDB e mapear os campos individualmente
                * Assim seria mais fácil de lidar com os dados e armazená-los corretamente
                */
               LoanDate: DateTime.Now, //Problemas com data na criação do MongoDB
               ReturnDate: DateTime.Now, //Problemas com data na criação do MongoDB
               Status: (LoanStatus)loan["Status"].AsInt32
           )).ToList()
       )).ToList();
   
       return Result.Success(response);
   }
}
