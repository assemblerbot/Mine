rmdir /Q /S Template\Libraries

mkdir Template\Libraries
xcopy src\Framework\bin\Release\net8.0\*.dll Template\Libraries\ /Y

@echo ================ DONE ===================
timeout /T 3