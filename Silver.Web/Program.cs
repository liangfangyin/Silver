using Essensoft.Paylink.Alipay;
using Essensoft.Paylink.WeChatPay;
using Silver.Pay.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddPay(builder.Configuration);
builder.Services.Configure<AlipayOptions>(builder.Configuration.GetSection("Alipay"));
builder.Services.Configure<WeChatPayOptions>(builder.Configuration.GetSection("WeChatPay"));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build(); 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
