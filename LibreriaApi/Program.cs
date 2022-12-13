using LibreriaApi.Interfaces;
using LibreriaApi.Services;
using MySql.Data.MySqlClient;
using System.Configuration;


var builder = WebApplication.CreateBuilder( args );

var allowCorsOrigin = "_localNetworkIntern";

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddCors( options => {
	options.AddPolicy( name: allowCorsOrigin, builder => { 
		builder.WithOrigins("*");
		builder.WithMethods("*");
		builder.WithHeaders("*");
	});
} );

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Inyectar la base de datos
builder.Services.AddTransient( _ => {
	var connection = new MySqlConnection(
		builder.Configuration.GetConnectionString( "MySqlDatabase" )
		);
	connection.Open();
	return connection;
} );

builder.Services.AddScoped<IGenresService, GenresService>();
builder.Services.AddScoped<IBooksService, BooksService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if( app.Environment.IsDevelopment() ) {
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors(allowCorsOrigin);

app.MapControllers();

app.Run();
