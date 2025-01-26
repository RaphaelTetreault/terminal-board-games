echo on

:: Delete old builds folder and contents
:: /s	Deletes a directory tree (the specified directory and all its subdirectories, including all files).
:: /q	Specifies quiet mode. Does not prompt for confirmation when deleting a directory tree.
::      The /q parameter works only if /s is also specified.
rmdir /s /q builds

:: %1 (argument 0) is dotnet publish -r runtime 
call ./script-build-runtime.bat win-x64
call ./script-build-runtime.bat osx-x64
call ./script-build-runtime.bat linux-x64