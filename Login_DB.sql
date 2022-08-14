use db_a8b518_loginsarahmora

Create table Usuario(
	idUsuario int primary key identity(1,1) not null
	,Correo varchar(100) not null
	,Clave varchar(500) not null
);

create proc sp_RegistrarUsuario(
	@Correo varchar(100)
	,@Clave varchar(500)
	,@Registrado bit output
	,@Mensaje varchar(100) output
)
as begin
	if(not exists(select * from Usuario where Correo = @Correo))
	begin
		insert into Usuario(Correo, Clave) values(@Correo, @Clave)
		set @Registrado = 1
		set @Mensaje = 'Usuario registrado'
	end
	else
	begin
		set @Registrado = 0
		set @Mensaje = 'Correo ya existe'
	end
end

create proc sp_ValidarUsuario(
@Correo varchar(100),
@Clave varchar(500)
)
as
begin
	if(exists(select * from Usuario where Correo = @Correo and Clave = @Clave))
		select idUsuario from Usuario where Correo = @Correo and Clave = @Clave
	else
		select '0'
end

declare @registrado bit, @mensaje varchar(100)

exec sp_RegistrarUsuario 'sarah@hotmail.com','djakslfjlsajfdslkja', @registrado output, @mensaje output

select @registrado
select @mensaje

select * from Usuario
exec sp_ValidarUsuario 'sarahq@hotmail.com','djakslfjlsajfdslkja'