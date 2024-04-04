using Essensoft.Paylink.Alipay;
using Essensoft.Paylink.WeChatPay;
using Silver.Basic;
using Silver.Pay.Extensions;
using Silver.WatchDog;
using Silver.WatchDog.src.Enums;

var builder = WebApplication.CreateBuilder(args);
ConfigurationUtil.Configuration = builder.Configuration;
builder.Services.AddPay(builder.Configuration);
builder.Services.Configure<AlipayOptions>(builder.Configuration.GetSection("Alipay"));
builder.Services.Configure<WeChatPayOptions>(builder.Configuration.GetSection("WeChatPay"));
builder.Services.AddControllers().AddNewtonsoftJson(opt => { 

});
builder.Services.AddRazorPages();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddWatchDogServices(opt =>
{
    opt.IsAutoClear = true;
    opt.SetExternalDbConnString = ConfigurationUtil.GetSection("WatchDog:Connection");
    opt.DbDriverOption = WatchDogDbDriverEnum.MySql;
    opt.ClearTimeSchedule = (WatchDogAutoClearScheduleEnum)ConfigurationUtil.GetSection("WatchDog:ClearTimeSchedule").ToInt();
});


var app = builder.Build(); 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapRazorPages();
app.UseWatchDog(opt =>
{
    opt.WatchPageUsername = ConfigurationUtil.GetSection("WatchDog:UserName");
    opt.WatchPagePassword = ConfigurationUtil.GetSection("WatchDog:Password");
});
app.Run();
