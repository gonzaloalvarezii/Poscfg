dotnet new api --name PosCFG

dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet restore
dotnet build
code .

dotnet add PosCFG.csproj package Swashbuckle.AspNetCore -v 5.5.0
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection 
dotnet add package FileHelpers
dotnet add package MySqlConnector
dotnet add package MySql.Data.EntityFrameworkCore


Antes de empezar a trabjar:
git pull

Crear nuevo branch:
git checkout -b "nuevo branch"

Cambiar de branch:
git checkout "branch al que quiero cambiar"

Listar Branchs:
git branchgit com

Previo al commit agregar cambios:
Todos los archivos
git add -A . 
Elegir archivos:
git add "nombre de archivo"

Comitear cambios al repo: 
git commit -m "mensaje"

Subir cambios al repo:
git push

Obtener status del repo:
git status



dotnet add package Microsoft.AspNetCore
dotnet add package Microsoft.AspNetCore.Mvc

