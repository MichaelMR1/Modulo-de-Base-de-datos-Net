Modulo que facilita la conexión a bases de datos SQL Server y contiene métodos para ejecutar consultas SQL.

Pasos para la implementación:

1-. Importar el modulo de base de datos al proyecto. </br>
2-. Referenciar el modulo en el proyecto o submódulo donde lo usaremos. </br>
3-. Agregar en Web.config del proyecto principal un connectionStrings con el nombre "dsn". 

Ejemplo:</br>
</br>

<label> 
  <connectionStrings>
   
    <add name="dsn" connectionString="Data Source=(local);Initial Catalog=Ejemplo;Persist Security Info=True;User ID=sa;Password=1234" providerName="System.Data.SqlClient" />
  </connectionStrings>

</label>
  
El modulo de base de datos esta codificado para reconocer automáticamente un connection strings con ese nombre. 

Una vez establecida la conexión podremos utilizar nuestra clase DataBase.cs, la cual contiene diferentes métodos para ejecutar procedimientos almacenados así como comandos directos que retornaran la información como nosotros lo necesitemos.
