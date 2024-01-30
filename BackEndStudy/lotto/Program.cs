using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

// Ioc Container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<LottoService>();

var app = builder.Build();

// Middleware
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapGet("/lotto", ([FromQuery] int? count,LottoService service) =>
    service.GetLotto(count ?? 1)).WithOpenApi();

app.Run();

public class LottoService
{
   private HashSet<int> GetLottoOne()
   {
       Random rand = new((int)DateTime.Now.Ticks);
       HashSet<int> lottoData = new HashSet<int>(6);

       while (lottoData.Count < 6)
       {
           lottoData.Add(rand.Next(1, 45));
       }

       return lottoData;
   }

   public List<HashSet<int>> GetLotto(int count )
   {
       List<HashSet<int>> list = new(count);

       for(int i = 0 ; i< count ;i++)
       {
            list.Add(GetLottoOne());
       }
       
       return list;
   }
}
