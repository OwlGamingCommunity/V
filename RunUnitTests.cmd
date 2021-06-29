cls
SET SolutionDir=%~dp0
dotnet test -c Release Source/UnitTests/UnitTests.csproj /p:TreatWarningsAsErrors=true /warnaserror /nowarn:msb3277