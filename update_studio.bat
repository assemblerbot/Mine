del /Q Studio\*
rmdir /Q /S Studio\runtimes
rmdir /Q /S Studio\Template

xcopy src\Studio\bin\Release\net8.0\*.dll Studio\
xcopy src\Studio\bin\Release\net8.0\*.exe Studio\
xcopy src\Studio\bin\Release\net8.0\*.json Studio\

mkdir Studio\runtimes
xcopy src\Studio\bin\Release\net8.0\runtimes Studio\runtimes\ /S

mkdir Studio\Template

xcopy Template\*.sln Studio\Template\ /S
xcopy Template\*.csproj Studio\Template\ /S
xcopy Template\*.cs Studio\Template\ /S
xcopy Template\Project.json Studio\Template\ /S

mkdir Studio\Template\Assets
mkdir Studio\Template\Assets\Plugins
mkdir Studio\Template\GameLibrary\Plugins
mkdir Studio\Template\Resources
mkdir Studio\Template\Libraries

rmdir /Q /S Studio\Template\GameExecutable\obj
rmdir /Q /S Studio\Template\GameLibrary\obj

xcopy Template\Libraries\* Studio\Template\Libraries\

@echo ================ DONE ===================
timeout /T 3